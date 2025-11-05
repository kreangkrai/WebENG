using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface ILeaveEntitlementRule
    {
        List<LeaveEntitlementRuleModel> GetLeaveEntitlementRules();
        List<LeaveEntitlementRuleModel> GetLeaveEntitlementRulesByLeaveID(string leave_type_id);
        string Insert(List<LeaveEntitlementRuleModel> leaves);
        string Delete(string leave_id);
        LeaveBalanceModel GetLeaveBalances(string emp_id, int year);
    }
}
