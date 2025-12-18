using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.HRModel;

namespace HRManagement.Interface
{
    public interface IHr
    {
        List<DataModel> GetDataByDate(DateTime start , DateTime stop);
    }
}
