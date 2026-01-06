using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class CalendarController : Controller
    {
        readonly IWorkingHours WorkingHoursService;
        readonly IAccessory Accessory;
        readonly IEngUser EngineerService;
        readonly IJobResponsible JobResponsibleService;
        readonly IJob Job;
        readonly IAuthen Authen;
        readonly CTLInterfaces.IEmployee Employees;
        public CalendarController()
        {
            WorkingHoursService = new WorkingHoursService();
            Accessory = new AccessoryService();
            EngineerService = new EngUserService();
            JobResponsibleService = new JobResponsibleService();
            Job = new JobService();
            Authen = new AuthenService();
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

        [HttpGet]
        public JsonResult GetUsers()
        {
            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            List<AuthenModel> users = Authen.GetAuthens();

            List<string> emps = employees.GroupBy(g => g.name_en).Select(s => s.FirstOrDefault().name_en).ToList();

            List<string> not_emp = users.Where(w => !emps.Contains(w.name.ToLower())).Select(s=>s.name).ToList();

            List<AuthenModel> authens = new List<AuthenModel>();
            for (int i = 0; i < not_emp.Count; i++)
            {
                AuthenModel authen = new AuthenModel()
                {
                    levels = 1,
                    user_id = ConvertUserID(not_emp[i]),
                    department = employees.Where(w=>w.name_en.ToLower() == not_emp[i].ToLower()).Select(s=>s.department).FirstOrDefault(),
                    name = not_emp[i],
                    role = "User"
                };
                authens.Add(authen);
            }
            users.AddRange(authens);
            return Json(users);
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
        public List<JobProcessSystemModel> GetProcessSystemByUser(string user)
        {
            List<JobProcessSystemModel> jps = Job.getsJobprocessSystemByUser(user);
            return jps;
        }

        [HttpGet]
        public List<JobResponsibleModel> GetJobs(string user_name)
        {
            List<JobResponsibleModel> jrs = JobResponsibleService.GetJobResponsible(user_name);
            return jrs;
        }

        [HttpGet]
        public List<QuotationResponsibleModel> GetQuotations(string user_name)
        {
            List<QuotationResponsibleModel> qrs = JobResponsibleService.GetQuotationResponsible(user_name);
            return qrs;
        }

        [HttpGet]
        public bool CheckAllowEditable(string user_name)
        {
            bool allow = EngineerService.CheckAllowEditable(user_name);
            return allow;
        }

        [HttpGet]
        public JsonResult GetWorkingHours(string user_name)
        {
            List<WorkingHoursModel> whs = WorkingHoursService.GetWorkingHours(user_name);
            whs = whs.OrderByDescending(w => w.working_date).ToList();
            return Json(whs);
        }

        [HttpGet]
        public JsonResult GetWorkingUser()
        {
            List<UserModel> users = Accessory.getWorkingUser();
            return Json(users);
        }
        [HttpGet] 
        public JsonResult GetEngineerUser(string user_name)
        {
            EngUserModel eng = EngineerService.GetEngineerUser(user_name);
            if (eng == null)
            {
                List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
                CTLModels.EmployeeModel emp = employees.Where(w => w.name_en.ToLower() == user_name.ToLower()).FirstOrDefault();
                eng = new EngUserModel()
                {
                    role = "User",
                    department = emp.department,
                    active = emp.active,
                    allow_edit = true,
                    group = emp.group,
                    user_id = ConvertUserID(emp.name_en),
                    user_name =emp.name_en
                };
            }
            return Json(eng);
        }

        [HttpGet]
        public JsonResult GetWorkingHoursByDate(string user_name, DateTime working_date)
        {
            List<WorkingHoursModel> whs = WorkingHoursService.GetWorkingHours(user_name, working_date);
            whs = whs.OrderByDescending(w => w.working_date).ToList();
            return Json(whs);
        }

        [HttpPost]
        public JsonResult AddWorkingHours(string wh_string)
        {
            try
            {
                int id = WorkingHoursService.GetLastWorkingHoursID() + 1;
                string wh_id = "WH" + id.ToString().PadLeft(6, '0');
                WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(wh_string);
                wh.index = wh_id;
                var result = WorkingHoursService.AddWorkingHours(wh);
                return Json(result);
            }
            catch(Exception exception)
            {
                return Json(exception.Message);
            }
        }
   
        [HttpPost]
        public JsonResult AddWorkingHoursDays(string[] wh_strings)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            for (int i = 0; i < wh_strings.Count(); i++)
            {
                WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(wh_strings[i]);
                var result = WorkingHoursService.AddWorkingHours(wh);
            }
            return Json("Success");
        }

        [HttpPatch]
        public JsonResult EditWorkingHours(string wh_string)
        {
            try
            {
                WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(wh_string);
                var result = WorkingHoursService.UpdateWorkingHours(wh);
                return Json(result);
            }
            catch(Exception exception)
            {
                return Json(exception.Message);
            }
        }

        [HttpDelete]
        public JsonResult DeleteWorkingHours(string wh_string)
        {
            try
            {
                WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(wh_string);
                var result = WorkingHoursService.DeleteWorkingHours(wh);
                return Json(result);
            }
            catch(Exception exception)
            {
                return Json(exception.Message);
            }
        }
    }
}
