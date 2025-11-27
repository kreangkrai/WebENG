using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class LeaveTypeModel
    {
        public string leave_type_id { get; set; }
        public string leave_type_code { get; set; }
        public string leave_name_th { get; set; }
        public string leave_name_en { get; set; }
        public string description { get; set; }
        public decimal min_request_hours { get; set; }
        public string request_timing { get; set; }
        public bool is_consecutive { get; set; }
        public decimal max_consecutive_days { get; set; }
        public bool is_two_step_approve { get; set; }
        public decimal over_consecutive_days_for_two_step { get; set; }
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
        public string color_code { get; set; }
        public bool calculate_auto { get; set; }
        public decimal amount_entitlement { get; set; }
        public decimal length_start_date { get; set; }
        public int priority { get; set; }
    }
}
