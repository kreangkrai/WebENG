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
        List<RequestModel> GetRequestByDate(string date);
        List<RequestModel> GetRequestByMonth(string month);
        List<RequestModel> GetRequestByDurationDay(string start_day,string end_day);
        string Insert(RequestModel request);
        string Update(RequestModel request);
        string Inserts(List<RequestModel> requests);
        string Updates(List<RequestModel> requests);
    }
}
