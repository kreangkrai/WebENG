using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class ApprovalRuleModel
    {
        public string approval_rule_id { get; set; }
        public string leave_type_id { get; set; }
        public decimal min_consecutive_days { get; set; }
        public int amount_step { get; set; }
        public bool is_active { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime updated_at { get; set; }
        public string updated_by { get; set; }
    }
}
