using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IDepartmentApprovalRule
    {
        List<DepartmentApprovalRuleModel> GetDepartmentApprovalRules();
        string Insert(DepartmentApprovalRuleModel department);
        string Update(DepartmentApprovalRuleModel department);
    }
}
