using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class EngineerTasksController : Controller
    {
        readonly IWorkingHours WorkingHoursService;
        readonly IAccessory Accessory;
        readonly IEngUser EngineerService;
        readonly IJobResponsible JobResponsibleService;

        public EngineerTasksController()
        {
            WorkingHoursService = new WorkingHoursService();
            Accessory = new AccessoryService();
            EngineerService = new EngUserService();
            JobResponsibleService = new JobResponsibleService();
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
        public List<string> GetDepartments()
        {
            List<string> departments = EngineerService.GetUsers().Select(s => s.department).Distinct().ToList();
            return departments;
        }

        [HttpGet]
        public List<EngUserModel> GetUsers()
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
        public JsonResult GetWorkingUser()
        {
            List<UserModel> users = Accessory.getWorkingUser();

            return Json(users);
        }
    }
}
