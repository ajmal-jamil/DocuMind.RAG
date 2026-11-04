using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DocuMind.Infrastructure.AI.Configuration
{
    public class EmbeddingOptions
    {
        public const string SectionName = "AI:Embedding";

        public string Provider { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        [Url]
        public string BaseUrl { get; set; } = string.Empty;

        [Range(768,768)]
        public int Dimensions { get; set; } = 768;

        [Required]
        public string ApiKey { get; set; } = string.Empty;
    }
}
