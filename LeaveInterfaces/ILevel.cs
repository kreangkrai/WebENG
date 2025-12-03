using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface ILevel
    {
        List<LevelModel> GetLevelByEmpID(string emp_id);
        //List<LevelModel> GetHierarchyByEmpID(string emp_id);
    }
}
