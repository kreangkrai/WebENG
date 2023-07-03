using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Interfaces
{
    interface ICalculateWorkingHours
    {
        WorkingHoursModel CalculateOvertime(WorkingHoursModel wh);
    }
}
