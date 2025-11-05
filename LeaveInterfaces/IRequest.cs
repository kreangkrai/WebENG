using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IRequest
    {
        List<RequestModel> GetRequests();
        RequestModel GetRequestByID(string request_id);
        List<RequestModel> GetRequestByEmpID(string emp_id);
        string Insert(RequestModel request);
        string Update(RequestModel request);
    }
}
