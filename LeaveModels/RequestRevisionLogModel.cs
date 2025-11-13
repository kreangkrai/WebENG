using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class RequestRevisionLogModel
    {
        public int log_id { get; set; }
        public string request_id { get; set; }
        public string request_emp_id { get; set; }
        public string revised_by { get; set; }
        public DateTime revised_date { get; set; }
        public string comment { get; set; }
        public string revision_level { get; set; }
        public string revision_type { get; set; }
    }
}
