using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IDailyReport
    {
        List<DailyActivityModel> GetDailyActivities(string user_name, string month);
        List<DailyActivityModel> GetServiceActivities(string job_id);
        string EditDailyReport(DailyActivityModel dlr);
    }
}
