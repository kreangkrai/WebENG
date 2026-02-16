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
        readonly IJobResponsible JobResponsible;
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
            JobResponsible = new JobResponsibleService();
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
                        department = employee.department
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

            List<JobsWorkingHoursModel> jwh = new List<JobsWorkingHoursModel>();
            List<JobResponsibleModel> jr = JobResponsible.GetJobsResponsible();
            List<WorkingHoursModel> whs = WorkingHours.CalculateWorkingHours(responsible, start,stop);
            whs = whs.Where(w => deps.Contains(w.department) && w.job_id != "" && w.job_id != "J999999").ToList();
            //whs = whs.Where(w => w.job_id == "J250585").ToList();
            List<string> jobs = whs.GroupBy(g => g.job_id).Select(s => s.FirstOrDefault().job_id).OrderBy(o => o).ToList();

            for (int i = 0; i < jobs.Count; i++)
            {
                List<string> emps = whs.Where(w => w.job_id == jobs[i]).GroupBy(g => g.emp_id).Select(s => s.FirstOrDefault().emp_id).ToList();
                JobsWorkingHoursModel sum_jwh = new JobsWorkingHoursModel();
                for (int j = 0; j < emps.Count; j++)
                {
                    int level = 1;
                    if (jr.Any(w => w.job_id == jobs[i] && w.emp_id == emps[j]))
                    {
                        level = jr.Where(w => w.job_id == jobs[i] && w.emp_id == emps[j]).FirstOrDefault().level;
                    }
                    //List<WorkingHoursModel> workings_ = whs.Where(w => w.job_id == jobs[i] && w.emp_id == emps[j]).ToList();
                    //List<WorkingDayModel> wd = workings_.GroupBy(g => g.working_date).Select(s => new WorkingDayModel()
                    //{
                    //    date = s.Key,
                    //    workings = workings_.Where(w => w.working_date == s.Key).ToList()
                    //}).ToList();

                    var _whs = whs.Where(w => w.job_id == jobs[i] && w.emp_id == emps[j]).ToList();
                    List<EngineerJobTimeModel> wh = _whs.GroupBy(g => g.job_id).Select(s => new EngineerJobTimeModel()
                    {
                        normal = s.Aggregate(TimeSpan.Zero, (sum_, x) => sum_ + x.normal).TotalHours,
                        ot1_5 = s.Aggregate(TimeSpan.Zero, (sum_, x) => sum_ + x.ot1_5).TotalHours,
                        ot3_0 = s.Aggregate(TimeSpan.Zero, (sum_, x) => sum_ + x.ot3_0).TotalHours,
                        job = s.Key,
                        job_name = s.FirstOrDefault().job_name
                    }).ToList();

                    //List<WorkingHoursModel> wh = CalculateWorkingHours(wd, holidays);
                    JobsWorkingHoursModel jwh_ = new JobsWorkingHoursModel()
                    {
                        job_id = jobs[i],
                        job_name = wh.FirstOrDefault().job_name,
                        normal = wh.Sum(s => s.normal) * level,
                        ot1_5 = wh.Sum(s => s.ot1_5) * level,
                        ot3_0 = wh.Sum(s => s.ot3_0) * level,
                        total = ((wh.Sum(s => s.normal) * level) +
                        (wh.Sum(s => s.ot1_5)  * 1.5 * level) +
                        (wh.Sum(s => s.ot3_0)  * 3.0 * level))
                    };

                    sum_jwh.job_id = jwh_.job_id;
                    sum_jwh.normal += jwh_.normal;
                    sum_jwh.ot1_5 += jwh_.ot1_5;
                    sum_jwh.ot3_0 += jwh_.ot3_0;
                    sum_jwh.total += jwh_.total;
                    sum_jwh.job_name = jwh_.job_name;
                }
                jwh.Add(sum_jwh);
            }

            List<EngineerJobTimeModel> j_whs = jwh.GroupBy(g => g.job_id).Select(s => new EngineerJobTimeModel()
            {
                normal = s.Sum(sum => sum.normal),
                ot1_5 = s.Sum(sum => sum.ot1_5),
                ot3_0 = s.Sum(sum => sum.ot3_0),
                job = s.Key,
                job_name = s.FirstOrDefault().job_name
            }).ToList();
            return Json(j_whs);
        }
    }
}
