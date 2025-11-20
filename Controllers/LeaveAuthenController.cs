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
        public IActionResult GetDapartments()
        {
            List<CTLModels.EmployeeModel> employees = CTLEmployees.GetEmployees();
            employees = employees.OrderBy(o => o.name_en).ToList();
            List<string> departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).OrderBy(o => o).ToList();
            var data = new { employees = employees, departments = departments };
            return Json(data);
        }


        [HttpGet]
        public IActionResult GetData()
        {
            List<CTLModels.EmployeeModel> employees = CTLEmployees.GetEmployees();
            employees = employees.OrderBy(o => o.name_en).ToList();
            List<string> departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).OrderBy(o=>o).ToList();
            List<LeavePositionModel> positions = Position.GetPositions();
            var data = new { employees = employees, departments = departments, positions = positions };
            return Json(data);
        }

        [HttpPost]
        public IActionResult CreateUser(string emp_id)
        {
            List<CTLModels.EmployeeModel> employees = CTLEmployees.GetEmployees();
            CTLModels.EmployeeModel employee = employees.Where(w => w.emp_id == emp_id).FirstOrDefault();
            List<LeavePositionModel> ps = Position.GetPositions();
            bool chk = ps.Any(a => a.emp_id == emp_id);
            if (!chk)
            {
                List<PositionModel> positions = new List<PositionModel>()
            {
                new PositionModel()
                {
                    emp_id = emp_id,
                    emp_name = employee.name_en,
                    department = "",
                    level = "",
                    img = "",
                    is_active = true,
                    position = employee.position,
                }
            };
                string message = Position.insert(positions);
                return Json(message);
            }
            return Json("emp id already exists");
        }

        [HttpPost]
        public IActionResult UpdateUser(string emp_id,string[] manager_departments,bool director,bool auditor)
        {
            string message = Position.delete(emp_id);
            if (message == "Success")
            {
                // Manager Of Department
                List<PositionModel> positions = new List<PositionModel>();
                for (int i = 0; i < manager_departments.Length; i++)
                {
                    PositionModel position = new PositionModel()
                    {
                        emp_id = emp_id,
                        emp_name = "",
                        department = manager_departments[i],
                        level = "Manager",
                        img = "",
                        is_active = true,
                        position = "",
                    };
                    positions.Add(position);
                }

                //Director
                if (director)
                {
                    PositionModel position = new PositionModel()
                    {
                        emp_id = emp_id,
                        emp_name = "",
                        department = "",
                        level = "Director",
                        img = "",
                        is_active = true,
                        position = "",
                    };
                    positions.Add(position);
                }
                //Auditor
                if (auditor)
                {
                    PositionModel position = new PositionModel()
                    {
                        emp_id = emp_id,
                        emp_name = "",
                        department = "",
                        level = "Auditor",
                        img = "",
                        is_active = true,
                        position = "",
                    };
                    positions.Add(position);
                }
                message = Position.insert(positions);
            }
            return Json(message);
        }
    }
}
