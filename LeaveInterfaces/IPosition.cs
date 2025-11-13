using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface IPosition
    {
        List<PositionModel> GetPositions();
        string insert(List<PositionModel> positions);
        string delete(string emp_id);
    }
}
