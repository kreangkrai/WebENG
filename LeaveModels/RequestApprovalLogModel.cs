using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class RequestApprovalLogModel
    {
        public int log_id { get; set; }
        public string request_id { get; set; }
        public string request_emp_id { get; set; }
        public string action_by { get; set; }
        public DateTime action_date { get; set; }
        public string action_type { get; set; }
        public string action_level { get; set; } //'Pending', 'Approved', 'Rejected', 'Returned', 'Cancelled'
        public string comment { get; set; }

    }
}
