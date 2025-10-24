using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class LeaveEntitlementRuleModel
    {
        public string entitlement_rule_id { get; set; }
        public string position { get; set; }
        public string leave_type_id { get; set; }
        public decimal start_age { get; set; }
        public decimal before_age { get; set; }
        public int days_per_year { get; set; }
        public int hours_per_year { get; set; }
    }
}
