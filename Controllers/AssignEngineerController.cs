using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Interfaces;
using WebENG.Models;
using WebENG.Service;

namespace WebForecastReport.Controllers
{
    public class AssignEngineerController : Controller
    {
        readonly IWorkingHours WorkingHoursService;
        readonly IAccessory Accessory;
        readonly IEngUser EngineerService;
        readonly IJob JobService;
        readonly IJobResponsible JobResponsibleService;
        readonly IAuthen Authen;
        readonly WebENG.CTLInterfaces.IEmployee Employees;
        public AssignEngineerController()
        {
            WorkingHoursService = new WorkingHoursService();
            Accessory = new AccessoryService();
            EngineerService = new EngUserService();
            JobService = new JobService();
            JobResponsibleService = new JobResponsibleService();
            Authen = new AuthenService();
            Employees = new WebENG.CTLServices.EmployeeService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");

                List<UserModel> users = Accessory.getAllUser();
                List<WebENG.CTLModels.EmployeeModel> emps = Employees.GetEmployees();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).FirstOrDefault();
                if (u == null)
                {
                    List<WebENG.CTLModels.EmployeeModel> employees = Employees.GetEmployees();
                    WebENG.CTLModels.EmployeeModel employee = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();
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
        public List<JobModel> GetJobs()
        {
            List<JobModel> jobs = JobService.GetAllJobs();
            return jobs;
        }

        [HttpGet]
        public List<string> GetDepartments()
        {
            List<string> departments = Authen.GetAuthens().Select(s => s.department).Distinct().ToList();
            return departments;
        }

        [HttpGet]
        public List<AuthenModel> GetUsers()
        {
            //List<EngUserModel> users = EngineerService.GetUsers().OrderBy(o => o.user_name).ToList();
            List<AuthenModel> users = Authen.GetAuthens().OrderBy(o => o.name).ToList();
            return users;
        }

        [HttpGet]
        public List<JobResponsibleModel> GetAssignEngineers(string job_id)
        {
            List<JobResponsibleModel> jrs = JobResponsibleService.GetAssignEngineers(job_id);
            return jrs;
        }

        [HttpGet]
        public List<JobResponsibleModel> GetJobLists()
        {
            List<JobResponsibleModel> jrs = JobResponsibleService.GetJobLists();
            return jrs;
        }
    }
}
