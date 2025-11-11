using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.CTLModels;

namespace WebENG.LeaveInterfaces
{
    public interface ILeave
    {
        double CalculateLeaveDays(EmployeeModel emp, int targetYear, int min_leave_staft, int max_leave_staft, int min_leave_manager, int max_leave_manager);
    }
}
