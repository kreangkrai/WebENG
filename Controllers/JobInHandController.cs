using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
        readonly IExport Export;
        protected readonly IHostingEnvironment _hostingEnvironment;
        public JobInHandController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Accessory = new AccessoryService();
            SummaryJobInHand = new SummaryJobInHandService();
            Export = new ExportService();
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

        [HttpGet]
        public JsonResult GetProjectInHand(int year)
        {
            List<JobInhandModel> jobs = SummaryJobInHand.GetsJobInhand(year);
            jobs = jobs.Where(w => w.job_type == "Project").ToList();
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetServiceInHand(int year)
        {
            List<JobInhandModel> jobs = SummaryJobInHand.GetsJobInhand(year);
            jobs = jobs.Where(w => w.job_type == "Service").ToList();
            return Json(jobs);
        }
        public IActionResult ExportSummaryJobInHand(int year)
        {
            List<SummaryJobInHandModel> all = SummaryJobInHand.GetsAccJobInHand(year, "ALL");
            List<SummaryJobInHandModel> projects = SummaryJobInHand.GetsProjectJobInHand(year,"Project");
            List<SummaryJobInHandModel> services = SummaryJobInHand.GetsServiceJobInHand(year, "Service");

            //Download Excel
            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "summary_job_in_hand.xlsx"));
            var stream = Export.ExportSummaryJobInHand(templateFileInfo, all, projects, services);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary_job_in_hand_" + year + ".xlsx");
        }
    }
}
