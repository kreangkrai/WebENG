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
    public class EngWorkingHoursController : Controller
    {
        readonly IAccessory Accessory;
        readonly IWorkingHours WorkingHours;
        readonly CTLInterfaces.IEmployee Employees;
        public EngWorkingHoursController()
        {
            this.Accessory = new AccessoryService();
            this.WorkingHours = new WorkingHoursService();
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
        public JsonResult GetWorkingHours(string weeks)
        {
            List<WeekModel> ww = JsonConvert.DeserializeObject<List<WeekModel>>(weeks);
            List<EngWeeklyWorkingHoursModel> whs = new List<EngWeeklyWorkingHoursModel>();
            for (int i = 0; i < ww.Count; i++)
            {
                whs.AddRange(WorkingHours.GetAllEngWorkingHours(Convert.ToInt32(ww[i].year), Convert.ToInt32(ww[i].week)));
            }
            return Json(whs);
        }
    }
}
