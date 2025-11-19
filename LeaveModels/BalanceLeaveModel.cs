using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class BalanceLeaveModel
    {
        public string leave_type_id { get; set; }
        public string leave_type_code { get; set; }
        public string leave_name_th { get; set; }
        public string leave_name_en { get; set; }
        public decimal amount_entitlement { get; set; }
        public decimal balance { get; set; }
    }
}
