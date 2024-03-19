using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class EngineerScoreController : Controller
    {
        readonly IAccessory Accessory;
        readonly IScore ScoreService;

        public EngineerScoreController()
        {
            Accessory = new AccessoryService();
            ScoreService = new EngineerScoreService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { name = s.name, department = s.department, role = s.role ,user_id = s.user_id}).FirstOrDefault();
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
        public JsonResult GetScores(string user_id, string year)
        {
            List<EngineerScoreModel> scores = ScoreService.GetScores(user_id,year);
            return Json(scores);
        }

        [HttpGet]
        public JsonResult GetWorkingUser()
        {
            List<UserModel> users = Accessory.getWorkingUser();

            return Json(users);
        }
    }
}
