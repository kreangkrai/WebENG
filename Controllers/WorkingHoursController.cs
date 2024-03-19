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

        static List<WorkingHoursModel> monthly = new List<WorkingHoursModel>();
        static string form_employee_name;
        static string form_department;
        static string form_phone_number;
        static string form_start_time;
        static string form_month;
        static Form_OvertimeModel form_data;
        static List<UserModel> users;
        public WorkingHoursController()
        {
            Accessory = new AccessoryService();
            WorkingHoursService = new WorkingHoursService();
            HolidayService = new HolidayService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { 
                    name = s.name,
                    department = s.department, 
                    role = s.role,
                    user_id = s.user_id}).FirstOrDefault();
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);
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
        public JsonResult GetWorkingHours(string user_name, string month)
        {
            form_employee_name = user_name;
            form_month = month;
            form_department = users.Where(w => w.name.ToLower() == user_name).Select(s => s.department).FirstOrDefault();
            form_phone_number = "";
            form_start_time = "08:30";
            monthly = WorkingHoursService.CalculateWorkingHours(user_name, month);
            return Json(monthly);
        }

        [HttpGet]
        public JsonResult GetMonthlySummary()
        {
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

        public IActionResult FormOvertime()
        {
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
                    lunch = monthly[i].lunch == true ? "✓" : "",
                    dinner = monthly[i].dinner == true ? "✓" : "",
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

            form_data = new Form_OvertimeModel()
            {
                employee_name = form_employee_name,
                department = form_department,
                phone_number = form_phone_number,
                normal_start_time = form_start_time,
                month = form_month,
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
