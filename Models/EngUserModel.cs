using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class EngUserModel
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string department { get; set; }
        public string role { get; set; }
        public bool allow_edit { get; set; }
        public string group { get; set; }
        public bool active { get; set; }
    }
}
