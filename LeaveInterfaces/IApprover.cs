using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IApprover
    {
        List<ApproverModel> GetDepartments();
        string Insert(ApproverModel approver);
        string Update(ApproverModel approver);
    }
}
