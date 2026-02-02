using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Service;
using Microsoft.AspNetCore.Http;
using WebENG.Models;

namespace WebENG.Controllers
{
    public class EngUserController : Controller
    {
        readonly IAccessory Accessory;
        readonly IEngUser EngUserService;
        readonly CTLInterfaces.IEmployee Employees;
        public EngUserController()
        {
            Accessory = new AccessoryService();
            EngUserService = new EngUserService();
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
        public JsonResult GetUsers()
        {
            List<EngUserModel> users = EngUserService.GetUsers().OrderBy(o => o.user_id).ToList();
            return Json(users);
        }

        [HttpGet]
        public JsonResult GetEngineerUsers()
        {
            List<EngUserModel> engineers = EngUserService.GetEngineerUsers().OrderBy(o => o.user_id).ToList();
            return Json(engineers);
        }

        [HttpPost]
        public JsonResult CreateEngineerUser(string user_string)
        {
            EngUserModel eng = JsonConvert.DeserializeObject<EngUserModel>(user_string);
            eng.role = eng.role == null ? "User" : eng.role;
            var result = EngUserService.CreateEngineerUser(eng);
            return Json(result);
        }

        [HttpPatch]
        public JsonResult UpdateEngineerUser(string user_string)
        {
            EngUserModel eng = JsonConvert.DeserializeObject<EngUserModel>(user_string);
            var result = EngUserService.UpdateEngineerUser(eng);
            return Json(result);
        }
    }
}
