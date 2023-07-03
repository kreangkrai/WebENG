using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class WeekModel
    {
        [JsonProperty("year")]
        public string year { get; set; }
        [JsonProperty("week")]
        public string week { get; set; }
    }
}
