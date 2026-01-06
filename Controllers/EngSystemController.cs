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
    public class EngSystemController : Controller
    {
        readonly IAccessory Accessory;
        ISystem System;
        IJob Job;
        readonly CTLInterfaces.IEmployee Employees;
        public EngSystemController()
        {
            this.Accessory = new AccessoryService();
            this.System = new SystemService();
            Job = new JobService();
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
        public List<EngSystemModel> GetSystems()
        {
            List<EngSystemModel> systems = System.GetSystems();
            return systems;
        }

        [HttpGet]
        public List<EngSystemModel> GetSystemsByJob(string job_id)
        {
            List<EngSystemModel> systems = Job.GetSystemByJob(job_id);
            return systems;
        }

        [HttpGet]
        public List<EngSystemModel> GetSystemsByUser(string user)
        {
            List<EngSystemModel> systems = Job.GetSystemByUser(user);
            return systems;
        }

        [HttpGet]
        public int GetLastSystemID()
        {
            int id = System.GetLastSystemID();
            return id;
        }

        [HttpPost]
        public JsonResult CreateSystem(string system_str)
        {
            EngSystemModel system = JsonConvert.DeserializeObject<EngSystemModel>(system_str);
            var result = System.CreateSystem(system);
            return Json(result);
        }

        [HttpPatch]
        public JsonResult EditSystem(string system_str)
        {
            EngSystemModel system = JsonConvert.DeserializeObject<EngSystemModel>(system_str);
            var result = System.EditSystem(system);
            return Json(result);
        }

        [HttpDelete]
        public JsonResult DeleteSystem(string system_str)
        {
            EngSystemModel system = JsonConvert.DeserializeObject<EngSystemModel>(system_str);
            var result = System.DeleteSystem(system);
            return Json(result);
        }
    }
}
