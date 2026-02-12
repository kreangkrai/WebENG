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
    public class AuthenController : Controller
    {
        readonly IAccessory Accessory;
        readonly IEngUser EngUserService;
        readonly IAuthen Authen;
        readonly CTLInterfaces.IEmployee Employees;
        public AuthenController()
        {
            Accessory = new AccessoryService();
            EngUserService = new EngUserService();
            Authen = new AuthenService();
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
                        department = employee.department
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

        [HttpGet]
        public JsonResult GetUsers()
        {
            List<EngUserModel> users = EngUserService.GetUsers().OrderBy(o => o.user_name).ToList();
            users = users.OrderBy(o => o.user_name).ToList();
            return Json(users);
        }

        [HttpPost]
        public JsonResult CreateAuthen(string user_string)
        {
            AuthenModel authen = JsonConvert.DeserializeObject<AuthenModel>(user_string);
            var result = Authen.Insert(authen);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetAuthenUsers()
        {
            List<AuthenModel> users = Authen.GetAuthens().Where(w=>w.role == "User").OrderBy(o => o.name).ToList();
            return Json(users);
        }

        [HttpGet]
        public JsonResult GetAuthenSuperAdmin()
        {
            List<AuthenModel> users = Authen.GetAuthens().Where(w => w.role == "Admin").OrderBy(o => o.name).ToList();
            return Json(users);
        }
        [HttpGet]
        public JsonResult GetAuthenAdminOperation()
        {
            List<AuthenModel> users = Authen.GetAuthens().Where(w => w.role == "Admin_Operation").OrderBy(o => o.name).ToList();
            return Json(users);
        }

        [HttpGet]
        public JsonResult GetAuthenAdminLeave()
        {
            List<AuthenModel> users = Authen.GetAuthens().Where(w => w.role == "Admin_Leave").OrderBy(o => o.name).ToList();
            return Json(users);
        }

        [HttpGet]
        public JsonResult GetAuthenSale()
        {
            List<AuthenModel> users = Authen.GetAuthens().Where(w => w.role == "Sale").OrderBy(o => o.name).ToList();
            return Json(users);
        }

        [HttpGet]
        public JsonResult GetAuthenManager()
        {
            List<AuthenModel> users = Authen.GetAuthens().Where(w => w.role != "User" && !w.role.Contains("Admin") && w.role != "Sale").OrderBy(o => o.name).ToList();
            return Json(users);
        }

        [HttpPut]
        public JsonResult UpdateRole(string user_string)
        {
            AuthenModel authen = JsonConvert.DeserializeObject<AuthenModel>(user_string);
            var result = Authen.UpdateRole(authen);
            return Json(result);
        }

        [HttpPut]
        public JsonResult UpdateLevel(string user_string)
        {
            AuthenModel authen = JsonConvert.DeserializeObject<AuthenModel>(user_string);
            var result = Authen.UpdateLevel(authen);
            return Json(result);
        }
    }
}
