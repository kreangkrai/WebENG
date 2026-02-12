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
    public class AssignMilestoneController : Controller
    {
        IAssignMilestone AssignMilestone;
        readonly IAccessory Accessory;
        readonly CTLInterfaces.IEmployee Employees;
        public AssignMilestoneController()
        {
            AssignMilestone = new AssignMilestoneService();
            this.Accessory = new AccessoryService();
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
        public List<AssignMilestoneModel> GetAssignedEngineers()
        {
            List<AssignMilestoneModel> engs = AssignMilestone.GetAssignedEngineers();
            return engs;
        }

        [HttpGet]
        public List<AssignMilestoneModel> GetEngineerAssignedJobs(string engId)
        {
            List<AssignMilestoneModel> jobs = AssignMilestone.GetEngineerAssignedJobs(engId);
            return jobs;
        }

        [HttpGet]
        public List<AssignMilestoneModel> GetJobAssignedEngineers(string jobId)
        {
            List<AssignMilestoneModel> engineers = AssignMilestone.GetJobAssignedEngineers(jobId);
            return engineers;
        }

        [HttpPost]
        public string AddEngineer(string asgStr)
        {
            AssignMilestoneModel engineer = JsonConvert.DeserializeObject<AssignMilestoneModel>(asgStr);
            string result = AssignMilestone.AddEngineer(engineer);
            return result;
        }

        [HttpPatch]
        public string EditEngineer(string asgStr)
        {
            AssignMilestoneModel engineer = JsonConvert.DeserializeObject<AssignMilestoneModel>(asgStr);
            string result = AssignMilestone.EditEngineer(engineer);
            return "Success";
        }

        [HttpDelete]
        public string DeleteEngineer(string asgStr)
        {
            AssignMilestoneModel engineer = JsonConvert.DeserializeObject<AssignMilestoneModel>(asgStr);
            string result = AssignMilestone.DeleteEngineer(engineer);
            return "Success";
        }
    }
}
