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

        public EngProcessController()
        {
            this.Accessory = new AccessoryService();
            this.Process = new ProcessService();
            Job = new JobService();
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
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
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
