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
        string Insert(LeaveEntitlementRuleModel leave);
        string Update(LeaveEntitlementRuleModel leave);
    }
}
