using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IRequestRevisionLog
    {
        List<RequestRevisionLogModel> GetRevisionLogByEmpID(string emp_id);
        string Insert(RequestRevisionLogModel request);
    }
}
