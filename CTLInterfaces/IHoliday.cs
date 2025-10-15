using System.Collections.Generic;
using WebENG.CTLModels;

namespace WebENG.CTLInterfaces
{
    public interface IHoliday
    {
        List<HolidayModel> GetHolidays(string year);
    }
}
