using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IRequestLog
    {
        List<RequestLogModel> GetLogByEmpID(string emp_id);
        List<RequestLogModel> GetLogByRequestId(string leave_id);
        string Insert(RequestLogModel request);
        string Inserts(List<RequestLogModel> requests);
    }
}
