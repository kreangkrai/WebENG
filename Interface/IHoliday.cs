using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Interface
{
    interface IHoliday
    {
        List<HolidayModel> GetHolidays(string year);
        List<HolidayModel> GetAllHolidays();
        string CreateHoliday(HolidayModel model);
    }
}
