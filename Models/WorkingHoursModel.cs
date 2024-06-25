using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class WorkingHoursModel
    {
        public string index { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string department { get; set; }
        public DateTime working_date { get; set; }
        public string day { get; set; }
        public int week_number { get; set; }
        public string job_id { get; set; }
        public string process_id { get; set; }
        public string process_name { get; set; }
        public string system_id { get; set; }
        public string system_name { get; set; }
        public string job_name { get; set; }
        public string task_id { get; set; }
        public string task_name { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan stop_time { get; set; }
        public string wh_type { get; set; }
        public bool lunch_full { get; set; }
        public bool lunch_half { get; set; }
        public bool dinner_full { get; set; }
        public bool dinner_half { get; set; }
        public string note { get; set; }
        public TimeSpan normal { get; set; }
        public TimeSpan ot1_5 { get; set; }
        public TimeSpan ot3_0 { get; set; }
        public TimeSpan leave { get; set; }
    }
}
