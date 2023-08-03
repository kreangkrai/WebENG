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
        readonly IHoliday HolidayService;
        readonly IAccessory Accessory;

        public HolidayController()
        {
            HolidayService = new HolidayService();
            Accessory = new AccessoryService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.fullname.ToLower() == user.ToLower()).Select(s => new UserModel { name = s.name, department = s.department, role = s.role, section = "Eng" }).FirstOrDefault();
                HttpContext.Session.SetString("Role", u.role);
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Section", u.section);
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
            List<HolidayModel> holidays = HolidayService.GetHolidays(year);
            return Json(holidays);
        }

        [HttpPost]
        public string CreateHoliday(string str)
        {
            HolidayModel h = JsonConvert.DeserializeObject<HolidayModel>(str);
            string result = HolidayService.CreateHoliday(h);
            return result;
        }
    }
}
