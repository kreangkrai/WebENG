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
        string Insert(DepartmentModel department);
        string Update(DepartmentModel department);
    }
}
