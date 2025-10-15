using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class LeaveTypeModel
    {
        public string leave_type_id { get; set; }
        public string leave_name_th { get; set; }
        public string leave_name_en { get; set; }
        public string description { get; set; }
        public decimal min_request_hours { get; set; }
        public string request_timing { get; set; }
        public bool max_consecutive { get; set; }
        public decimal max_consecutive_days { get; set; }
        public string gender_restriction { get; set; }
        public bool attachment_required { get; set; }
        public decimal attachment_threshold_days { get; set; }
        public bool count_holidays_as_leave { get; set; }
        public bool is_unpaid { get; set; }
        public bool is_active { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime updated_at { get; set; }
        public string updated_by { get; set; }
    }
}
