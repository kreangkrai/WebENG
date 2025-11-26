using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class RequestLogModel
    {
        public int log_id { get; set; }
        public string request_id { get; set; }
        public string action_by { get; set; }
        public string action_by_name { get; set; }
        public int action_by_level { get; set; }
        public string old_status { get; set; }
        public string new_status { get; set; }
        public int old_level_step { get; set; }
        public int new_level_step { get; set; }     
        public string comment { get; set; }
        public DateTime log_date { get; set; }

    }
}
