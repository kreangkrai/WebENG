using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class LeaveEntitlementRuleModel
    {
        public string entitlement_rule_id { get; set; }
        public string leave_type_id { get; set; }
        public string position_group_id { get; set; }
        public int min_tenure_months { get; set; }
        public int max_tenure_months { get; set; }
        public string gender_filter { get; set; }
        public decimal days_per_year { get; set; }
        public bool is_active { get; set; }
        public DateTime created_at { get; set; }
        public string created_by { get; set; }
        public DateTime updated_at { get; set; }
        public string updated_by { get; set; }
    }
}
