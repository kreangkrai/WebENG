using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class CalendarController : Controller
    {
        readonly IWorkingHours WorkingHoursService;
        readonly IAccessory Accessory;
        readonly IEngUser EngineerService;
        readonly IJobResponsible JobResponsibleService;
        readonly IJob Job;
        readonly IAuthen Authen;
        public CalendarController()
        {
            WorkingHoursService = new WorkingHoursService();
            Accessory = new AccessoryService();
            EngineerService = new EngUserService();
            JobResponsibleService = new JobResponsibleService();
            Job = new JobService();
            Authen = new AuthenService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { 
                    name = s.name, 
                    department = s.department, 
                    role = s.role,
                    user_id = s.user_id,
                    emp_id = s.emp_id
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
            List<AuthenModel> users = Authen.GetAuthens();
            return Json(users);
        }

        [HttpGet]
        public List<JobProcessSystemModel> GetProcessSystemByUser(string user)
        {
            List<JobProcessSystemModel> jps = Job.getsJobprocessSystemByUser(user);
            return jps;
        }

        [HttpGet]
        public List<JobResponsibleModel> GetJobs(string user_name)
        {
            List<JobResponsibleModel> jrs = JobResponsibleService.GetJobResponsible(user_name);
            return jrs;
        }

        [HttpGet]
        public List<QuotationResponsibleModel> GetQuotations(string user_name)
        {
            List<QuotationResponsibleModel> qrs = JobResponsibleService.GetQuotationResponsible(user_name);
            return qrs;
        }

        [HttpGet]
        public bool CheckAllowEditable(string user_name)
        {
            bool allow = EngineerService.CheckAllowEditable(user_name);
            return allow;
        }

        [HttpGet]
        public JsonResult GetWorkingHours(string user_name)
        {
            List<WorkingHoursModel> whs = WorkingHoursService.GetWorkingHours(user_name);
            whs = whs.OrderByDescending(w => w.working_date).ToList();
            return Json(whs);
        }

        [HttpGet]
        public JsonResult GetWorkingUser()
        {
            List<UserModel> users = Accessory.getWorkingUser();
            return Json(users);
        }
        [HttpGet] 
        public JsonResult GetEngineerUser(string user_name)
        {
            EngUserModel eng = EngineerService.GetEngineerUser(user_name);
            return Json(eng);
        }

        [HttpGet]
        public JsonResult GetWorkingHoursByDate(string user_name, DateTime working_date)
        {
            List<WorkingHoursModel> whs = WorkingHoursService.GetWorkingHours(user_name, working_date);
            whs = whs.OrderByDescending(w => w.working_date).ToList();
            return Json(whs);
        }

        [HttpPost]
        public JsonResult AddWorkingHours(string wh_string)
        {
            try
            {
                int id = WorkingHoursService.GetLastWorkingHoursID() + 1;
                string wh_id = "WH" + id.ToString().PadLeft(6, '0');
                WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(wh_string);
                wh.index = wh_id;
                var result = WorkingHoursService.AddWorkingHours(wh);
                return Json(result);
            }
            catch(Exception exception)
            {
                return Json(exception.Message);
            }
        }

        [HttpPost]
        public JsonResult AddWorkingHoursDays(string[] wh_strings)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            for (int i = 0; i < wh_strings.Count(); i++)
            {
                WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(wh_strings[i]);
                var result = WorkingHoursService.AddWorkingHours(wh);
            }
            return Json("Success");
        }

        [HttpPatch]
        public JsonResult EditWorkingHours(string wh_string)
        {
            try
            {
                WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(wh_string);
                var result = WorkingHoursService.UpdateWorkingHours(wh);
                return Json(result);
            }
            catch(Exception exception)
            {
                return Json(exception.Message);
            }
        }

        [HttpDelete]
        public JsonResult DeleteWorkingHours(string wh_string)
        {
            try
            {
                WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(wh_string);
                var result = WorkingHoursService.DeleteWorkingHours(wh);
                return Json(result);
            }
            catch(Exception exception)
            {
                return Json(exception.Message);
            }
        }
    }
}
