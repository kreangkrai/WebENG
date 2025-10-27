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
        public int hours { get; set; }
    }
}
