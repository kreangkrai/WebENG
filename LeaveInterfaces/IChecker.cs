using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IChecker
    {
        List<CheckerModel> GetDepartments();
        string Insert(CheckerModel checker);
        string Update(CheckerModel checker);
    }
}
