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
    public class TasksByWeekController : Controller
    {
        readonly IAccessory Accessory;
        readonly ITasksByWeek TBWService;

        public TasksByWeekController()
        {
            Accessory = new AccessoryService();
            TBWService = new TasksByWeekService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { 
                    name = s.name, 
                    department = s.department, 
                    role = s.role,
                    user_id = s.user_id,
                    emp_id = s.emp_id
                }).FirstOrDefault();
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
        public List<TasksByWeekModel> GetTasksByWeek(string year, string week)
        {
            List<TasksByWeekModel> tasks = TBWService.GetTasksByWeek(year, week);
            return tasks;
        }
    }
}
