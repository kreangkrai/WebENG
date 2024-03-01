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

        public AssignJobController()
        {
            WorkingHoursService = new WorkingHoursService();
            Accessory = new AccessoryService();
            EngineerService = new EngUserService();
            JobService = new JobService();
            JobResponsibleService = new JobResponsibleService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { name = s.name, department = s.department, role = s.role, user_id = s.user_id }).FirstOrDefault();
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
                JobResponsibleModel jr = JsonConvert.DeserializeObject<JobResponsibleModel>(jr_string);
                var result = JobResponsibleService.AddJobResponsible(jr);
                return Json(result);
            }
            catch(Exception exception)
            {
                return Json(exception.Message);
            }
        }
    }
}
