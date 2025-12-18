using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.HRModel
{
    public class TimeInOutModel
    {
        public string emp_id { get; set; }
        public int count_late { get; set; }
        public int minute_late { get; set; }
        public int count_forgot_scan { get; set; }
    }
}
