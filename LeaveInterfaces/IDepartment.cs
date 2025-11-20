using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IDepartment
    {
        List<DepartmentModel> GetDepartments();
        string Inserts(List<DepartmentModel> departments);
        string Delete(string department);
    }
}
