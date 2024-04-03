using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class JobInHandController : Controller
    {
        readonly IAccessory Accessory;
        readonly ISummaryJobInHand SummaryJobInHand;
        public JobInHandController()
        {
            Accessory = new AccessoryService();
            SummaryJobInHand = new SummaryJobInHandService();
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { name = s.name, department = s.department, role = s.role, user_id = s.user_id }).FirstOrDefault();
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
        public JsonResult GetAccJobInHand(int year,string type)
        {
            List<SummaryJobInHandModel> jobs = SummaryJobInHand.GetsAccJobInHand(year, type);
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetJobInHandProject(int year, string type)
        {
            List<SummaryJobInHandModel> jobs = SummaryJobInHand.GetsProjectJobInHand(year, type);
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetJobInHandService(int year, string type)
        {
            List<SummaryJobInHandModel> jobs = SummaryJobInHand.GetsServiceJobInHand(year, type);
            return Json(jobs);
        }
    }
}
