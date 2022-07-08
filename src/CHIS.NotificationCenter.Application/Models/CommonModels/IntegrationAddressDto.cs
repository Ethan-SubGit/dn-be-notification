using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.CommonModels
{
    public class IntegrationAddressDto
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("method")] 
        public string Method { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
    }
}
