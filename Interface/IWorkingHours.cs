using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IWorkingHours
    {
        List<WorkingHoursModel> GetWorkingHours();
        List<WorkingHoursModel> GetWorkingHours(string emp_id);      
        List<WorkingDayModel> GetWorkingHours(DateTime start, DateTime stop, string emp_id);
        List<WorkingDayModel> GetAllWorkingHours();
        List<WorkingHoursModel> GetWorkingHours(string emp_id, DateTime working_date);
        WorkingHoursModel GetWorkingHourByLeave(string emp_id, string working_date);
        string AddWorkingHours(WorkingHoursModel wh);
        string UpdateWorkingHours(WorkingHoursModel wh);
        string UpdateRestTime(WorkingHoursModel wh);
        string DeleteWorkingHours(WorkingHoursModel wh);
        List<JobWeeklyWorkingHoursModel> GetAllJobWorkingHours(int year, int week);
        List<EngWeeklyWorkingHoursModel> GetAllEngWorkingHours(int year, int week);
        List<WorkingHoursModel> GetWorkingHours(int year, int week);

        int GetLastWorkingHoursID();

        List<WorkingHoursModel> CalculateWorkingHours(string emp_id, DateTime start, DateTime stop);
        List<WorkingHoursSummaryModel> CalculateMonthlySummary(List<WorkingHoursModel> workings);
    }
}
