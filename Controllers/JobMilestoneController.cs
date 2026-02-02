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

namespace WebForecastReport.Controllers
{
    public class JobMilestoneController : Controller
    {
        IJobMilestone JobMilestone;
        readonly IAccessory Accessory;
        readonly WebENG.CTLInterfaces.IEmployee Employees;

        public JobMilestoneController()
        {
            JobMilestone = new JobMilestoneService();
            this.Accessory = new AccessoryService();
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
        public List<JobMilestoneModel> GetJobsMilestones()
        {
            List<JobMilestoneModel> jms = JobMilestone.GetJobsMilestones();
            return jms;
        }

        [HttpGet]
        public List<JobMilestoneModel> GetJobMilestones(string jobId)
        {
            List<JobMilestoneModel> jobMilestones = JobMilestone.GetJobMilestones(jobId);
            return jobMilestones;
        }

        [HttpGet]
        public List<JobMilestoneModel> GetJobsMilestonesAfterDate(DateTime date)
        {
            List<JobMilestoneModel> jms = JobMilestone.GetJobsMilestonesAfterDate(date);
            return jms;
        }

        [HttpPost]
        public string CreateJobMilestone(string jmStr)
        {
            JobMilestoneModel jm = JsonConvert.DeserializeObject<JobMilestoneModel>(jmStr);
            string result = JobMilestone.CreateJobMilestone(jm);
            return result;
        }

        [HttpPatch]
        public string EditJobMilestone(string jmStr)
        {
            JobMilestoneModel jm = JsonConvert.DeserializeObject<JobMilestoneModel>(jmStr);
            string result = JobMilestone.EditJobMilestone(jm);
            return result;
        }

        [HttpDelete]
        public string DeleteJobMilestone(string jmStr)
        {
            JobMilestoneModel jm = JsonConvert.DeserializeObject<JobMilestoneModel>(jmStr);
            string result = JobMilestone.DeleteJobMilestone(jm);
            return result;
        }

        [HttpDelete]
        public string DeleteAllJobMilestones(string jmStr)
        {
            JobMilestoneModel jm = JsonConvert.DeserializeObject<JobMilestoneModel>(jmStr);
            string result = JobMilestone.DeleteAllJobMilestones(jm);
            return result;
        }
    }
}
