using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IApprovalRule
    {
        List<ApprovalRuleModel> GetApprovalRules();
        string Insert(ApprovalRuleModel approval);
        string Update(ApprovalRuleModel approval);
    }
}
