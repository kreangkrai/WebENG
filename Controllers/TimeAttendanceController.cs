using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebENG.CTLInterfaces;
using WebENG.CTLServices;
using WebENG.Interface;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;
using WebENG.LeaveServices;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class TimeAttendanceController : Controller
    {
        private readonly IAccessory Accessory;
        private readonly ILeaveType LeaveType;
        private readonly IEmployee Employees;
        public TimeAttendanceController()
        {
            Accessory = new AccessoryService();
            LeaveType = new LeaveTypeService();
            Employees = new EmployeeService();
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
        public IActionResult GetLeaveType()
        {
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            leaves = leaves.OrderBy(o => o.priority).ToList();
            return Json(leaves);
        }

        [HttpGet]
        public IActionResult GetDapartments()
        {
            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.OrderBy(o => o.name_en).ToList();
            List<string> departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).OrderBy(o => o).ToList();
            return Json(departments);
        }
    }
}