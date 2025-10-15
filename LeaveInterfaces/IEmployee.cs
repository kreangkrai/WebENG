using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IEmployee
    {
        List<EmployeeModel> GetEmployees();
        string Insert(EmployeeModel employee);
        string Update(EmployeeModel employee);
    }
}
