using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IRequestApprovalLog
    {
        List<RequestApprovalLogModel> GetApprovalLogByEmpID(string emp_id);
        string Insert(RequestApprovalLogModel request);
    }
}
