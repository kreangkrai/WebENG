using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class RequestModel
    {
        public string request_id { get; set; }
        public string emp_id { get; set; }
        public string leave_type_id { get; set; }
        public bool is_full_day { get; set; }
        public DateTime start_request_date { get; set; }
        public DateTime end_request_date { get; set; }
        public int amount_leave_day { get; set; }
        public TimeSpan start_request_time { get; set; }
        public TimeSpan end_request_time { get; set; }
        public decimal amount_leave_hour { get; set; }
        public string path_file { get; set; }
        public DateTime request_date { get; set; }
        public string manager_approver_status { get; set; }
        public string manager_approver { get; set; }
        public DateTime manager_approve_date { get; set; }
        public string admin_approver_status { get; set; }
        public string admin_approver { get; set; }
        public DateTime admin_approve_date { get; set; }
        public string decsription { get; set; }
        public string status_request { get; set; }

    }
}
