using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IDailyReport
    {
        List<DailyActivityModel> GetDailyActivities(string user_name, DateTime start_date, DateTime stop_date);
        string EditDailyReport(DailyActivityModel dlr);
    }
}
