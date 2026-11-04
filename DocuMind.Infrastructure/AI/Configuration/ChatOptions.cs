using System.ComponentModel.DataAnnotations;

namespace DocuMind.Infrastructure.AI.Configuration
{
    public class ChatOptions
    {
        public const string SectionName = "AI:Chat";

        public string Provider { get; set; } = string.Empty;

        [Range(0, 2)]
        public double Temperature { get; set; } = 0.2;

        [Range(1, int.MaxValue)]
        public int MaxOutputTokens { get; set; } = 2048;

        [Required]
        [Url]
        public string BaseUrl { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        public string? ApiSecret { get; set; }
    }
}
