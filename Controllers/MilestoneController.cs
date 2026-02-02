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
    public class MilestoneController : Controller
    {
        IMilestone Milestone;
        readonly IAccessory Accessory;
        readonly CTLInterfaces.IEmployee Employees;

        public MilestoneController()
        {
            Milestone = new MilestoneService();
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
        public List<MilestoneModel> GetMilestones()
        {
            List<MilestoneModel> milestones = Milestone.GetMilestones();
            return milestones;
        }

        [HttpGet]
        public int GetLastMilestoneID()
        {
            int id = Milestone.GetLastMilestoneID();
            return id;
        }

        [HttpPost]
        public string CreateMilestone(string ms_str)
        {
            MilestoneModel ms = JsonConvert.DeserializeObject<MilestoneModel>(ms_str);
            string result = Milestone.CreateMilestone(ms);
            return result;
        }

        [HttpPatch]
        public string EditMilestone(string ms_str)
        {
            MilestoneModel ms = JsonConvert.DeserializeObject<MilestoneModel>(ms_str);
            string result = Milestone.EditMilestone(ms);
            return result;
        }
    }
}
