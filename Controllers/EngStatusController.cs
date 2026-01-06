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
    public class EngStatusController : Controller
    {
        readonly IAccessory Accessory;
        readonly IStatus Status;
        readonly CTLInterfaces.IEmployee Employees;
        public EngStatusController()
        {
            Accessory = new AccessoryService();
            Status = new EngStatusService();
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
        public List<EngStatusModel> GetStatuses()
        {
            List<EngStatusModel> statuses = Status.GetStatuses();
            return statuses;
        }

        [HttpGet]
        public int GetLastStatusID()
        {
            int id = Status.GetLastStatusID();
            return id;
        }

        [HttpPost]
        public JsonResult CreateStatus(string status_str)
        {
            EngStatusModel status = JsonConvert.DeserializeObject<EngStatusModel>(status_str);
            var result = Status.CreateStatus(status);
            return Json(result);
        }

        [HttpPatch]
        public JsonResult EditStatus(string status_str)
        {
            EngStatusModel status = JsonConvert.DeserializeObject<EngStatusModel>(status_str);
            var result = Status.EditStatus(status);
            return Json(result);
        }

        [HttpDelete]
        public JsonResult DeleteStatus(string status_str)
        {
            EngStatusModel status = JsonConvert.DeserializeObject<EngStatusModel>(status_str);
            var result = Status.DeleteStatus(status);
            return Json(result);
        }
    }
}
