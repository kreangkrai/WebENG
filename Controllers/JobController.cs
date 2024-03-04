using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Service;
using Microsoft.AspNetCore.Http;
using WebENG.Models;

namespace WebENG.Controllers
{
    public class JobController : Controller
    {
        readonly IJob JobService;
        readonly IAccessory Accessory;
        readonly IStatus Status;
        public JobController()
        {
            JobService = new JobService();
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

        public IActionResult JobsSummary()
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
        public JsonResult GetJobs()
        {
            List<JobModel> jobs = JobService.GetAllJobs();
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetStatus()
        {
            List<EngStatusModel> statuses = Status.GetStatuses();
            return Json(statuses);
        }

        [HttpGet]
        public JsonResult GetJobsSummary()
        {
            List<JobSummaryModel> jobsSummary = JobService.GetJobsSummary();
            return Json(jobsSummary);
        } 

        [HttpPost]
        public JsonResult AddJob(string job_string)
        {
            JobModel job = JsonConvert.DeserializeObject<JobModel>(job_string);
            var result = JobService.CreateJob(job);
            return Json(result);
        }

        [HttpPatch]
        public JsonResult UpdateJob(string job_string)
        {
            JobModel job = JsonConvert.DeserializeObject<JobModel>(job_string);
            var result = JobService.UpdateJob(job);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetQuotations(string year)
        {
            List<JobQuotationModel> quots = JobService.GetJobQuotations(year);
            return Json(quots);
        }
    }
}
