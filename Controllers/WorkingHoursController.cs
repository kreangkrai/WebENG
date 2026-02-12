using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Service;
using Microsoft.AspNetCore.Http;
using WebENG.Models;
using Newtonsoft.Json;
using Rotativa.AspNetCore;

namespace WebENG.Controllers
{
    public class WorkingHoursController : Controller
    {
        readonly IAccessory Accessory;
        readonly IWorkingHours WorkingHoursService;
        readonly IHoliday HolidayService;
        readonly CTLInterfaces.IEmployee Employees;
        public WorkingHoursController()
        {
            Accessory = new AccessoryService();
            WorkingHoursService = new WorkingHoursService();
            HolidayService = new HolidayService();
            Employees = new CTLServices.EmployeeService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");

                List<UserModel> users = Accessory.getAllUser();
                List<CTLModels.EmployeeModel> emps = Employees.GetEmployees();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).FirstOrDefault();
                if (u == null)
                {
                    List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
                    CTLModels.EmployeeModel employee = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();
                    u = new UserModel()
                    {
                        emp_id = employee.emp_id,
                        name = employee.name_en,
                        role = "User",
                        department = employee.department,
                    };
                }
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);

                if (!u.role.Contains("Admin"))
                {
                    string position = emps.Where(w => w.emp_id == u.emp_id).Select(s => s.position).FirstOrDefault();
                    u.role = position;
                }

                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public string GetLastWorkingHoursID()
        {
            int id = WorkingHoursService.GetLastWorkingHoursID() + 1;
            string wh = "WH" + id.ToString().PadLeft(6, '0');
            return wh;
        }

        [HttpGet]
        public JsonResult GetWorkingUser()
        {
            List<UserModel> users = Accessory.getWorkingUser();
            
            return Json(users);
        }

        [HttpGet]
        public JsonResult GetWorkingHours(string emp_id, string month)
        {
            var parts = month.Split('-');
            int y = int.Parse(parts[0]);
            int m = int.Parse(parts[1]);
            DateTime start = new DateTime(y, m, 1);
            DateTime stop = new DateTime(y, m, DateTime.DaysInMonth(y, m));

            List<WorkingHoursModel> monthly = WorkingHoursService.CalculateWorkingHours(emp_id, start,stop);
            return Json(monthly);
        }

        [HttpGet]
        public JsonResult GetMonthlySummary(string emp_id, string month)
        {
            var parts = month.Split('-');
            int y = int.Parse(parts[0]);
            int m = int.Parse(parts[1]);
            DateTime start = new DateTime(y, m, 1);
            DateTime stop = new DateTime(y, m, DateTime.DaysInMonth(y, m));

            List<WorkingHoursModel> monthly = WorkingHoursService.CalculateWorkingHours(emp_id, start, stop);
            List<WorkingHoursSummaryModel> whs = WorkingHoursService.CalculateMonthlySummary(monthly);
            return Json(whs);
        }

        [HttpPatch]
        public JsonResult UpdateRestTime(string task_str)
        {
            WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(task_str);
            var result = WorkingHoursService.UpdateRestTime(wh);
            return Json(result);
        }

        public IActionResult FormOvertime(string emp_id,string month)
        {
            var parts = month.Split('-');
            int y = int.Parse(parts[0]);
            int m = int.Parse(parts[1]);
            DateTime start = new DateTime(y, m, 1);
            DateTime stop = new DateTime(y, m, DateTime.DaysInMonth(y, m));

            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            string department = employees.Where(w => w.emp_id == emp_id).Select(s => s.department).FirstOrDefault();
            List<WorkingHoursModel> monthly = WorkingHoursService.CalculateWorkingHours(emp_id, start,stop);

            List<Form_OvertimeDataModel> datas = new List<Form_OvertimeDataModel>();
            List<HolidayModel> holidays = HolidayService.GetHolidays(monthly[0].working_date.Year.ToString());
            TimeSpan sum_normal = default(TimeSpan);
            TimeSpan sum_ot1_5 = default(TimeSpan);
            TimeSpan sum_ot3_0 = default(TimeSpan);
            for(int i = 0; i < monthly.Count(); i++)
            {
                Form_OvertimeDataModel data = new Form_OvertimeDataModel()
                {
                    date = monthly[i].working_date.ToString("dd MMMM yyyy").ToUpper(),
                    day = monthly[i].working_date.DayOfWeek.ToString().ToUpper(),
                    job = monthly[i].job_id,
                    task = monthly[i].task_name,
                    lunch_full = monthly[i].lunch_full == true ? "✓" : "",
                    lunch_half = monthly[i].lunch_half == true ? "✓" : "",
                    dinner_full = monthly[i].dinner_full == true ? "✓" : "",
                    dinner_half = monthly[i].dinner_half == true ? "✓" : "",
                    weekend = (monthly[i].working_date.DayOfWeek == DayOfWeek.Saturday || monthly[i].working_date.DayOfWeek == DayOfWeek.Sunday) ? true : false,
                    holiday = (holidays.Where(w=>w.date == monthly[i].working_date).ToList().Count() > 0) ? true : false,
                };

                sum_normal += monthly[i].normal;
                sum_ot1_5 += monthly[i].ot1_5;
                sum_ot3_0 += monthly[i].ot3_0;

                if(monthly[i].job_id == "")
                {
                    data.job = "";
                    data.location = "";
                    data.task = "";
                    data.start_time = "";
                    data.stop_time = "";
                    data.normal = "";
                    data.ot1_5 = "";
                    data.ot3_0 = "";
                }
                else
                {
                    if (monthly[i].task_id.Substring(0, 1) == "O")
                    {
                        data.location = "Office";
                    }
                    if (monthly[i].task_id.Substring(0, 1) == "H")
                    {
                        data.location = "Home";
                    }
                    if (monthly[i].task_id.Substring(0, 1) == "S")
                    {
                        data.location = "Site";
                    }
                    if (monthly[i].task_id.Substring(0, 1) == "T")
                    {
                        data.location = "Other";
                    }

                    data.start_time = monthly[i].start_time.ToString().Substring(0,5) != "00:00" || monthly[i].stop_time.ToString().Substring(0, 5) != "00:00" ? 
                        monthly[i].start_time.ToString().Substring(0, 5) : "";
                    data.stop_time = monthly[i].stop_time.ToString().Substring(0, 5) != "00:00" || monthly[i].start_time.ToString().Substring(0, 5) != "00:00" ? 
                        monthly[i].stop_time.ToString().Substring(0, 5) : "";
                    data.normal = monthly[i].normal.ToString().Substring(0, 5) != "00:00" ? monthly[i].normal.ToString().Substring(0, 5) : "";
                    data.ot1_5 = monthly[i].ot1_5.ToString().Substring(0, 5) != "00:00" ? monthly[i].ot1_5.ToString().Substring(0, 5) : "";
                    data.ot3_0 = monthly[i].ot3_0.ToString().Substring(0, 5) != "00:00" ? monthly[i].ot3_0.ToString().Substring(0, 5) : "";
                }
                datas.Add(data);
            }

            List<WorkingHoursModel> summaries = new List<WorkingHoursModel>();
            string[] jobs = monthly.Select(s => s.job_id).Where(w => w != "" && w != "J999999").Distinct().ToArray();
            for(int i = 0; i < jobs.Count(); i++)
            {
                WorkingHoursModel summary = new WorkingHoursModel()
                {
                    job_id = jobs[i],
                    normal = new TimeSpan(monthly.Where(w => w.job_id == jobs[i]).Sum(s => s.normal.Ticks)),
                    ot1_5 = new TimeSpan(monthly.Where(w => w.job_id == jobs[i]).Sum(s => s.ot1_5.Ticks)),
                    ot3_0 = new TimeSpan(monthly.Where(w => w.job_id == jobs[i]).Sum(s => s.ot3_0.Ticks)),
                };
                summaries.Add(summary);
            }

            TimeSpan twhs = sum_normal + sum_ot1_5 + sum_ot3_0;

            Form_OvertimeModel form_data = new Form_OvertimeModel()
            {
                emp_id = emp_id,
                employee_name = employees.Where(w => w.emp_id == emp_id).Select(s => s.name_en).FirstOrDefault(),
                department = department,
                phone_number = "",
                normal_start_time = "08:30",
                month = month,
                datas = datas,
                summary = summaries,
                total_working_hours = ((int)(twhs.TotalHours)).ToString().PadLeft(2,'0') + ":" + twhs.Minutes.ToString().PadLeft(2,'0'),
                total_normal = ((int)(sum_normal.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_normal.Minutes.ToString().PadLeft(2, '0'),
                total_ot1_5 = ((int)(sum_ot1_5.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_ot1_5.Minutes.ToString().PadLeft(2, '0'),
                total_ot3_0 = ((int)(sum_ot3_0.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_ot3_0.Minutes.ToString().PadLeft(2, '0'),
                hours_normal = ((int)(sum_normal.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_normal.Minutes.ToString().PadLeft(2, '0'),
                hours_1_5 = ((int)(sum_ot1_5.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_ot1_5.Minutes.ToString().PadLeft(2, '0'),
                hours_3_0 = ((int)(sum_ot3_0.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_ot3_0.Minutes.ToString().PadLeft(2, '0')
            };
            var form_overtime = new ViewAsPdf("FormOvertime")
            {
                Model = form_data,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Rotativa.AspNetCore.Options.Margins { Top = 5, Left = 5, Right = 5, Bottom = 2 }
            };
            return form_overtime;
        }
    }
}
