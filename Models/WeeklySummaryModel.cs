using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class WeeklySummaryModel
    {
        public string emp_id { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public int week { get; set; }
        public int year { get; set; }
        public string job_id { get; set; }
        public string job_name { get; set; }
        public string task_id { get; set; }
        public string task_name { get; set; }
        public int hours { get; set; }
    }
}
