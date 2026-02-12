using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class AssignJobController : Controller
    {
        readonly IWorkingHours WorkingHoursService;
        readonly IAccessory Accessory;
        readonly IEngUser EngineerService;
        readonly IJob JobService;
        readonly IJobResponsible JobResponsibleService;
        readonly IAuthen Authen;
        readonly CTLInterfaces.IEmployee Employees;
        public AssignJobController()
        {
            WorkingHoursService = new WorkingHoursService();
            Accessory = new AccessoryService();
            EngineerService = new EngUserService();
            JobService = new JobService();
            JobResponsibleService = new JobResponsibleService();
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
        public List<string> GetDepartments()
        {
            List<string> departments = EngineerService.GetUsers().Select(s => s.department).Distinct().ToList();
            return departments;
        }

        [HttpGet]
        public List<EngUserModel> GetEngineers()
        {
            List<EngUserModel> users = EngineerService.GetUsers().OrderBy(o => o.user_name).ToList();
            return users;
        }

        [HttpGet]
        public List<JobResponsibleModel> GetJobResponsibles(string user_id)
        {
            List<JobResponsibleModel> jrs = JobResponsibleService.GetJobResponsible(user_id);
            return jrs;
        }

        [HttpGet]
        public List<JobModel> GetJobs()
        {
            List<JobModel> jobs = JobService.GetAllJobs();
            return jobs;
        }

        [HttpPost]
        public JsonResult AddJobResponsible(string jr_string)
        {
            try
            {
                List<JobResponsibleModel> jrs = new List<JobResponsibleModel>();
                JobResponsibleModel jr = JsonConvert.DeserializeObject<JobResponsibleModel>(jr_string);
                if (jr.emp_id == "ALL")
                {
                    List<AuthenModel> users = Authen.GetAuthens().OrderBy(o => o.name).ToList();
                    users = users.Where(w => w.department == jr.department).ToList();
                    JobResponsibleModel _jr = new JobResponsibleModel();
                    for(int i = 0; i < users.Count; i++)
                    {
                        _jr = new JobResponsibleModel()
                        {
                            emp_id = users[i].emp_id,
                            job_id = jr.job_id,
                            level = users[i].levels,
                            role = users[i].role,
                            assign_by = jr.assign_by,
                            assign_date = jr.assign_date
                        };
                        jrs.Add(_jr);
                    }
                }
                else
                {
                    jrs.Add(jr);
                }
                var result = JobResponsibleService.AddJobResponsible(jrs);
                return Json(result);
            }
            catch(Exception exception)
            {
                return Json(exception.Message);
            }
        }
    }
}
