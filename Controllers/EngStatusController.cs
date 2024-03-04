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
        public EngStatusController()
        {
            Accessory = new AccessoryService();
            Status = new EngStatusService();
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
