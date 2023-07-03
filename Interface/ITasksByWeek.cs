using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface ITasksByWeek
    {
        List<TasksByWeekModel> GetTasksByWeek(string year, string week);
    }
}
