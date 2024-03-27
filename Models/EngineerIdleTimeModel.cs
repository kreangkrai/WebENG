using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class EngineerIdleTimeModel
    {
        public string userName { get; set; }
        public double workingHours { get; set; }
        public double idle { get; set; }
        public double normal { get; set; }
        public double ot1_5 { get; set; }
        public double ot3_0 { get; set; }
        public double leave { get; set; }
    }
}
