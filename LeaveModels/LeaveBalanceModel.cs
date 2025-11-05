using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class EntitlementBalanceModel
    {    
        public string leave_type_id { get; set; }
        public string leave_code { get; set; }
        public string leave_name_th { get; set; } 
        public string leave_name_en { get; set; }
        public decimal amount { get; set; }
        public decimal balance { get; set; }     
    }
    public class LeaveBalanceModel
    {
        public int year { get; set; }
        public string emp_id { get; set; }
        public List<EntitlementBalanceModel> entitlements { get; set; }
    }
}
