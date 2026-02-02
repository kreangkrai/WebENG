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
    public class EngProcessController : Controller
    {
        readonly IAccessory Accessory;
        IProcess Process;
        IJob Job;
        readonly CTLInterfaces.IEmployee Employees;
        public EngProcessController()
        {
            this.Accessory = new AccessoryService();
            this.Process = new ProcessService();
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
        public List<EngProcessModel> GetProcesses()
        {
            List<EngProcessModel> processes = Process.GetProcesses();
            return processes;
        }

        [HttpGet]
        public List<EngProcessModel> GetProcessesByJob(string job_id)
        {
            List<EngProcessModel> processes = Job.GetProcessByJob(job_id);
            return processes;
        }

        [HttpGet]
        public List<EngProcessModel> GetProcessesByUser(string user)
        {
            List<EngProcessModel> processes = Job.GetProcessByUser(user);
            return processes;
        }

        [HttpGet]
        public int GetLastProcessID()
        {
            int id = Process.GetLastProcessID();
            return id;
        }

        [HttpPost]
        public JsonResult CreateProcess(string process_str)
        {
            EngProcessModel process = JsonConvert.DeserializeObject<EngProcessModel>(process_str);
            var result = Process.CreateProcess(process);
            return Json(result);
        }

        [HttpPatch]
        public JsonResult EditProcess(string process_str)
        {
            EngProcessModel process = JsonConvert.DeserializeObject<EngProcessModel>(process_str);
            var result = Process.EditProcess(process);
            return Json(result);
        }

        [HttpDelete]
        public JsonResult DeleteProcess(string process_str)
        {
            EngProcessModel process = JsonConvert.DeserializeObject<EngProcessModel>(process_str);
            var result = Process.DeleteProcess(process);
            return Json(result);
        }
    }
}
