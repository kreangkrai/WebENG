using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class EntitlementModel
    {
        public List<DataEntitileModel> entitlement { get; set; }
    }
    public class DataEntitileModel
    {
        public int id { get; set; }
        public decimal start_age { get; set; }
        public decimal before_age { get; set; }
        public int days { get; set; }
        public bool is_prorated { get; set; }
        public int prorated_amount_per_year { get; set; }
        public bool is_calculate_by_year { get; set; }
    }
}
