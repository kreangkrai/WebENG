using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IApprover
    {
        List<ApproverModel> GetApprovers();
        string Inserts(List<ApproverModel> approvers);
        string Delete(string department);
    }
}
