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
        int CalcuLevelStep(List<LevelModel> levels, RequestModel request,LeaveTypeModel leave);
        //List<LevelModel> GetHierarchyByEmpID(string emp_id);
    }
}
