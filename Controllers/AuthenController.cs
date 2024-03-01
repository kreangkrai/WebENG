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
        public AuthenController()
        {
            Accessory = new AccessoryService();
            EngUserService = new EngUserService();
            Authen = new AuthenService();
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
                    user_id = s.user_id
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
        public JsonResult GetUsers()
        {
            List<EngUserModel> users = EngUserService.GetUsers().OrderBy(o => o.user_id).ToList();
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
        public JsonResult GetAuthenAdmin()
        {
            List<AuthenModel> users = Authen.GetAuthens().Where(w => w.role == "Admin").OrderBy(o => o.name).ToList();
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
            List<AuthenModel> users = Authen.GetAuthens().Where(w => w.role != "User" && w.role != "Admin" && w.role != "Sale").OrderBy(o => o.name).ToList();
            return Json(users);
        }

        [HttpPut]
        public JsonResult UpdateRole(string user_string)
        {
            AuthenModel authen = JsonConvert.DeserializeObject<AuthenModel>(user_string);
            var result = Authen.UpdateRole(authen);
            return Json(result);
        }
    }
}
