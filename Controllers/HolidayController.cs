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

        public HolidayController()
        {
            HolidayService = new CTLServices.HolidayService();
            Accessory = new AccessoryService();
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
