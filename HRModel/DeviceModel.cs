using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.HRModel
{
    public class DeviceModel
    {
        public string sn { get; set; }
        public string device { get; set; }
        public string name { get; set; }
        public DateTime sdatetime { get; set; }
        public string comment { get; set; }
        public DateTime? tiggertime { get; set; }
        public string levels { get; set; }
        public bool active { get; set; }
    }
}
