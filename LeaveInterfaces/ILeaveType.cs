using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface ILeaveType
    {
        List<LeaveTypeModel> GetLeaveTypes();
        LeaveTypeModel GetLeaveTypeByID(string leave_type_id);
        string Insert(LeaveTypeModel leave);
        string Update(LeaveTypeModel leave);
    }
}
