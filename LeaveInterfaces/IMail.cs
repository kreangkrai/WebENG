using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveInterfaces
{
    public interface IMail
    {
        string Approver(string to, string from ,string status, string leave_type, string leave_date, string leave_time, string approver, string comment);
        string Requester(List<string> to, string status, CTLModels.EmployeeModel name, string leave_type, string leave_date, string leave_time);
    }
}
