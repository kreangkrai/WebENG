using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class IdleTimeController : Controller
    {
        readonly IAccessory Accessory;
        readonly IWorkingHours WorkingHours;
        readonly IHoliday Holiday;

        public IdleTimeController()
        {
            this.Accessory = new AccessoryService();
            this.WorkingHours = new WorkingHoursService();
            this.Holiday = new HolidayService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel
                {
                    name = s.name,
                    department = s.department,
                    role = s.role,
                    user_id = s.user_id
                }).FirstOrDefault();
                HttpContext.Session.SetString("Role", u.role);
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult EngineerIdleTimes()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel
                {
                    name = s.name,
                    department = s.department,
                    role = s.role,
                    user_id = s.user_id
                }).FirstOrDefault();
                HttpContext.Session.SetString("Role", u.role);
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult MonthlyIdleTimes()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel
                {
                    name = s.name,
                    department = s.department,
                    role = s.role,
                    user_id = s.user_id
                }).FirstOrDefault();
                HttpContext.Session.SetString("Role", u.role);
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public List<EngineerIdleTimeModel> GetIdleTimes(string month)
        {
            int yy = Convert.ToInt32(month.Split("-")[0]);
            int mm = Convert.ToInt32(month.Split("-")[1]);
            List<HolidayModel> holidays = Holiday.GetHolidays(yy.ToString());

            List<EngineerIdleTimeModel> idles = new List<EngineerIdleTimeModel>();
            string[] users = Accessory.getWorkingUser().Select(s => s.name).Distinct().ToArray();
            int end = DateTime.DaysInMonth(yy, mm);
            DateTime start = new DateTime(yy, mm, 1);
            DateTime stop = new DateTime(yy, mm, end);

            int working_hours = 0;
            for (DateTime date = start; date != stop; date = date.AddDays(1))
            {
                bool isHoliday = holidays.Where(w => w.date == date).Count() > 0 ? true : false;
                bool isWeekend = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? true : false;
                if (!isHoliday && !isWeekend)
                {
                    working_hours++;
                }
            }
            

            for (int i = 0; i < users.Length; i++)
            {
                List<WorkingHoursModel> monthly = WorkingHours.CalculateWorkingHours(users[i], month);
                List<WorkingHoursSummaryModel> summaries = WorkingHours.CalculateMonthlySummary(monthly);

                int normal = summaries.Select(s => s.normal).Sum() / 60;
                int ot1_5 = summaries.Select(s => s.ot1_5).Sum() / 60;
                int ot3_0 = summaries.Select(s => s.ot3_0).Sum() / 60;
                int leave = summaries.Select(s => s.leave).Sum() / 60;
                int idleTime = (working_hours * 8) - (normal + leave);
                if (idleTime < 0)
                {
                    idleTime = 0;
                }
                EngineerIdleTimeModel idle = new EngineerIdleTimeModel()
                {
                    userName = users[i],
                    workingHours = (working_hours * 8),
                    idle = idleTime,
                    normal = normal,
                    ot1_5 = ot1_5,
                    ot3_0 = ot3_0,
                    leave = leave
                };
                idles.Add(idle);
            }
            return idles;
        }
    }
}
