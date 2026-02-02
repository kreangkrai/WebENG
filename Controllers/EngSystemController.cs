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
