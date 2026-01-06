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
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { name = s.name, department = s.department, role = s.role, user_id = s.user_id, emp_id = s.emp_id }).FirstOrDefault();
                HttpContext.Session.SetString("Role", u.role);
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);

                List<CTLModels.EmployeeModel> emps = Employees.GetEmployees();
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
