using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketIntegration.ExternalServices.Slack.Classes
{
    public class Attachment
    {
        [JsonProperty(PropertyName = "fallback")]
        public string Fallback { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; } = "Richie";

        [JsonProperty(PropertyName = "pretext")]
        public string PreText { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public List<Field> Fields { get; set; }

    }
}
