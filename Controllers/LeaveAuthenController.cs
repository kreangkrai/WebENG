using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;
using WebENG.LeaveServices;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class LeaveAuthenController : Controller
    {
        readonly IAccessory Accessory;
        readonly CTLInterfaces.IEmployee CTLEmployees;
        private IPosition Position;
        public LeaveAuthenController()
        {
            Accessory = new AccessoryService();
            CTLEmployees = new CTLServices.EmployeeService();
            Position = new PositionService();
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel
                {
                    name = s.name,
                    department = s.department,
                    role = s.role,
                    user_id = s.user_id,
                    emp_id = s.emp_id
                }).FirstOrDefault();
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);

                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public IActionResult GetData()
        {
            List<CTLModels.EmployeeModel> employees = CTLEmployees.GetEmployees();
            List<string> departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).OrderBy(o=>o).ToList();
            List<LeavePositionModel> positions = Position.GetPositions();
            var data = new { employees = employees, departments = departments, positions = positions };
            return Json(data);
        }
    }
}
