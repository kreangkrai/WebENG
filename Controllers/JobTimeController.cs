using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class JobTimeController : Controller
    {
        readonly IAccessory Accessory;
        readonly IWorkingHours WorkingHours;
        readonly IHoliday Holiday;
        readonly IExport Export;
        protected readonly IHostingEnvironment _hostingEnvironment;
        readonly CTLInterfaces.IEmployee Employees;
        public JobTimeController(IHostingEnvironment hostingEnvironment)
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

        [HttpGet]
        public IActionResult GetEmployees(string department)
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

            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();     
            employees = employees.Where(w => deps.Contains(w.department) && w.active).OrderBy(o=>o.name_en).ToList();         
            return Json(employees);
        }

        [HttpGet]
        public IActionResult GetJobTimes(string department, string responsible)
        {
            DateTime start = new DateTime(2022, 1, 1);
            DateTime stop = DateTime.Now;
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
            List<WorkingHoursModel> whs = WorkingHours.CalculateWorkingHours(responsible, start,stop);
            whs = whs.Where(w => deps.Contains(w.department) && w.job_id != "" && w.job_id != "J999999").ToList();

            List<EngineerJobTimeModel> j_whs = whs.GroupBy(g => g.job_id).Select(s => new EngineerJobTimeModel()
            {
                normal = s.Aggregate(TimeSpan.Zero, (sum, x) => sum + x.normal).TotalHours,
                ot1_5 = s.Aggregate(TimeSpan.Zero, (sum, x) => sum + x.ot1_5).TotalHours,
                ot3_0 = s.Aggregate(TimeSpan.Zero, (sum, x) => sum + x.ot3_0).TotalHours,
                job = s.Key,
                job_name = s.FirstOrDefault().job_name
            }).ToList();           
            return Json(j_whs);
        }
    }
}
