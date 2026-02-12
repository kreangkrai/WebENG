using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Service;
using Microsoft.AspNetCore.Http;
using WebENG.Models;
using Newtonsoft.Json;

namespace WebENG.Controllers
{
    public class HolidayController : Controller
    {
        readonly CTLInterfaces.IHoliday HolidayService;
        readonly IAccessory Accessory;
        readonly CTLInterfaces.IEmployee Employees;
        public HolidayController()
        {
            HolidayService = new CTLServices.HolidayService();
            Accessory = new AccessoryService();
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
        public JsonResult GetHolidays(string year)
        {
            List<CTLModels.HolidayModel> holidays = HolidayService.GetHolidays(year);
            return Json(holidays);
        }

        //[HttpPost]
        //public string CreateHoliday(string str)
        //{
        //    HolidayModel h = JsonConvert.DeserializeObject<HolidayModel>(str);
        //    string result = HolidayService.CreateHoliday(h);
        //    return result;
        //}
    }
}
