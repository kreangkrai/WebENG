using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.HRModel
{
    public class MonthlyTimeInOutModel
    {
        public int month { get; set; }
        public int count_late { get; set; }
        public int minute_late { get; set; }
        public int count_forgot_scan { get; set; }
    }
    public partial class TimeInOutModel
    {
        public string emp_id { get; set; }
        public List<MonthlyTimeInOutModel> months { get; set; } = new List<MonthlyTimeInOutModel>();
        public int TotalCountLate => months.Sum(m => m.count_late);
        public int TotalMinuteLate => months.Sum(m => m.minute_late);
        public int TotalCountForgotScan => months.Sum(m => m.count_forgot_scan);
        public int TotalMonths => months.Count;
    }
}
