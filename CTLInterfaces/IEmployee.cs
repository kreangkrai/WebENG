using System.Collections.Generic;
using WebENG.CTLModels;

namespace WebENG.CTLInterfaces
{
    public interface IEmployee
    {
        List<EmployeeModel> GetEmployees();
    }
}
