using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.HRModel
{
    public class DataModel
    {
        public int id { get; set; }
        public DateTime datetime { get; set; }
        public DateTime date { get; set; }
        public TimeSpan time_in { get; set; }
        public double late_time { get; set; }
        public TimeSpan time_out { get; set; }
        public TimeSpan start { get; set; }
        public TimeSpan stop { get; set; }
        public string device_in { get; set; }
        public string device_out { get; set; }
        public string personname { get; set; }
        public string persongroup { get; set; }
        public string cn { get; set; }
        public string sn { get; set; }
        public string emp { get; set; }
        public string autr { get; set; }
        public string autt { get; set; }
        public double man_day { get; set; }
    }
}
