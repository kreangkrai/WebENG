using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
        readonly IExport Export;
        protected readonly IHostingEnvironment _hostingEnvironment;
        readonly CTLInterfaces.IEmployee Employees;
        public IdleTimeController(IHostingEnvironment hostingEnvironment)
        {
            Accessory = new AccessoryService();
            WorkingHours = new WorkingHoursService();
            Holiday = new HolidayService();
            Export = new ExportService();
            _hostingEnvironment = hostingEnvironment;
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
                        user_id = ConvertUserID(employee.name_en)
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
        public string ConvertUserID(string user)
        {
            string first = user.Split(' ')[0];
            string last = user.Split(' ')[1];
            string name = first.Substring(0, 1).ToUpper() + first.Substring(1, first.Length - 1);
            string lastname = last.Substring(0, 1).ToUpper();
            return name + "." + lastname;
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
        public List<EngineerIdleTimeModel> GetIdleTimes(string department, string start_month, string stop_month)
        {
            List<string> deps = new List<string>();
            if (department == "ALL")
            {
                deps = new List<string>()
                {
                    "CES-CIS",
                    "CES-System",
                    "CES-QIR",
                    "CES-PMD",
                    "CES-Exp",
                    "CES-ENG",
                    "AES"
                };
            }
            else
            {
                deps = new List<string>()
                {
                    department
                };
            }
            DateTime _start = Convert.ToDateTime(start_month);
            DateTime _stop = Convert.ToDateTime(stop_month);
            int diff = ((_stop.Year - _start.Year) * 12) + _stop.Month - _start.Month;

            int start_yy = Convert.ToInt32(start_month.Split("-")[0]);
            int start_mm = Convert.ToInt32(start_month.Split("-")[1]);
            int stop_yy = Convert.ToInt32(stop_month.Split("-")[0]);
            int stop_mm = Convert.ToInt32(stop_month.Split("-")[1]);

            List<HolidayModel> holidays = new List<HolidayModel>();
            for (int i = start_yy; i <= stop_yy; i++)
            {
                holidays.AddRange(Holiday.GetHolidays(i.ToString()));
            }
            
            holidays = holidays.GroupBy(g => g.date).Select(s => new HolidayModel()
            {
                date = s.Key,
                name = holidays.Where(w => w.date == s.Key).Select(s1 => s1.name).FirstOrDefault(),
                no = holidays.Where(w => w.date == s.Key).Select(s2 => s2.no).FirstOrDefault()
            }).ToList();

            List<EngineerIdleTimeModel> idles = new List<EngineerIdleTimeModel>();

            var users_ = Accessory.getWorkingUser(_start,_stop.AddMonths(1));
            users_ = users_.Where(w => deps.Contains(w.department)).ToList();
            string[] users = users_.Select(s => s.name).Distinct().ToArray();
            int end = DateTime.DaysInMonth(stop_yy, stop_mm);
            DateTime start = new DateTime(start_yy, start_mm, 1);
            DateTime stop = new DateTime(stop_yy, stop_mm, end);

            double working_hours = 0;
            for (DateTime date = start; date <= stop; date = date.AddDays(1))
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
                double normal = 0.0;
                double ot1_5 = 0.0;
                double ot3_0 = 0.0;
                double leave = 0.0;
                double idleTime = 0.0;
                for (DateTime month = _start; month <= _start.AddMonths(diff); month = month.AddMonths(1))
                {
                    List<WorkingHoursModel> monthly = WorkingHours.CalculateWorkingHours(users[i], month.ToString("yyyy-MM-dd"));
                    List<WorkingHoursSummaryModel> summaries = WorkingHours.CalculateMonthlySummary(monthly);

                    normal += summaries.Select(s => s.normal).Sum() / 60;
                    ot1_5 += summaries.Select(s => s.ot1_5).Sum() / 60;
                    ot3_0 += summaries.Select(s => s.ot3_0).Sum() / 60;
                    leave += summaries.Select(s => s.leave).Sum() / 60;                    
                }
                idleTime = (working_hours * 8) - (normal + leave);
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
        public IActionResult ExportIdleTime(string department,string start_year,string stop_year)
        {
            List<EngineerIdleTimeModel> idles = GetIdleTimes(department,start_year, stop_year);
            //Download Excel
            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "idletime.xlsx"));
            var stream = Export.ExportIdleTime(templateFileInfo, idles);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "idletime_" + start_year + "_" + stop_year +".xlsx");
        }
    }
}
