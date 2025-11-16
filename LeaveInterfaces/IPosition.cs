using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IPosition
    {
        List<LeavePositionModel> GetPositions();
        List<PositionModel> GetManagerPositions();
        List<PositionModel> GetDirectorPositions();
        List<PositionModel> GetAuditorPositions();
        string insert(List<PositionModel> positions);
        string delete(string emp_id);
    }
}
