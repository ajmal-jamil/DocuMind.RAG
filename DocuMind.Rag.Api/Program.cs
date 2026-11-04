using DocuMind.Application;
using DocuMind.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
//using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
    .AddPolicy("RequireAdmin", policy => policy
        .RequireAuthenticatedUser()
        .RequireAssertion(x =>
            x.User.IsInRole("Admin") ||
            x.User.HasClaim("groups", builder.Configuration["AdminRole"]!)));

var azure = builder.Configuration.GetSection("AzureAd");
var tenantId = azure["TenantId"]!;
var clientId = azure["ClientId"]!;
var apiScopes = $"api://{clientId}/access_as_user";

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, o =>
{
    o.TokenValidationParameters.ValidAudiences = [clientId, $"api://{clientId}"];
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DocuMind RAG API", Version = "v1" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string> {
                    {
                        apiScopes, "Access the API as signed-in user"
                    }
                }
            }
        }
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("oauth2", doc)] = [apiScopes]
    });
});


builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddAIServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();


    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.OAuthClientId(builder.Configuration["ClientApp:ClientId"]);
        o.OAuthUsePkce();
        o.OAuthScopes(apiScopes);
    });
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
