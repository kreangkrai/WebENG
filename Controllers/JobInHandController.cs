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
        public JsonResult GetAccJobInHand(string department, int year,string type)
        {
            if (department == "ENG")
            {
                List<SummaryENGJobInHandModel> jobs = SummaryJobInHand.GetsAccENGJobInHand(year, type);
                return Json(jobs);
            }
            if (department == "CIS")
            {
                List<SummaryCISJobInHandModel> jobs = SummaryJobInHand.GetsAccCISJobInHand(year, type);
                return Json(jobs);
            }
            if (department == "AIS")
            {
                List<SummaryAISJobInHandModel> jobs = SummaryJobInHand.GetsAccAISJobInHand(year, type);
                return Json(jobs);
            }
            return Json(null);
        }

        [HttpGet]
        public JsonResult GetJobInHandProject(string department, int year, string type)
        {
            if (department == "ENG")
            {
                List<SummaryENGJobInHandModel> jobs = SummaryJobInHand.GetsProjectENGJobInHand(year, type);
                return Json(jobs);
            }
            if (department == "CIS")
            {
                List<SummaryCISJobInHandModel> jobs = SummaryJobInHand.GetsProjectCISJobInHand(year, type);
                return Json(jobs);
            }
            if (department == "AIS")
            {
                List<SummaryAISJobInHandModel> jobs = SummaryJobInHand.GetsProjectAISJobInHand(year, type);
                return Json(jobs);
            }
            return Json(null);
        }

        [HttpGet]
        public JsonResult GetJobInHandService(string department,int year, string type)
        {
            if (department == "ENG")
            {
                List<SummaryENGJobInHandModel> jobs = SummaryJobInHand.GetsServiceENGJobInHand(year, type);
                return Json(jobs);
            }
            if (department == "CIS")
            {
                List<SummaryCISJobInHandModel> jobs = SummaryJobInHand.GetsServiceCISJobInHand(year, type);
                return Json(jobs);
            }
            if (department == "AIS")
            {
                List<SummaryAISJobInHandModel> jobs = SummaryJobInHand.GetsServiceAISJobInHand(year, type);
                return Json(jobs);
            }
            return Json(null);
        }

        [HttpGet]
        public JsonResult GetProjectInHand(string department,int year)
        {
            if (department == "ENG")
            {
                List<JobENGInhandModel> jobs = SummaryJobInHand.GetsENGJobInhand(year);
                jobs = jobs.Where(w => w.job_type == "Project").ToList();
                return Json(jobs);
            }
            if (department == "CIS")
            {
                List<JobCISInhandModel> jobs = SummaryJobInHand.GetsCISJobInhand(year);
                jobs = jobs.Where(w => w.job_type == "Project").ToList();
                return Json(jobs);
            }
            if (department == "AIS")
            {
                List<JobAISInhandModel> jobs = SummaryJobInHand.GetsAISJobInhand(year);
                jobs = jobs.Where(w => w.job_type == "Project").ToList();
                return Json(jobs);
            }
            return Json(null);
        }

        [HttpGet]
        public JsonResult GetServiceInHand(string department,int year)
        {
            if (department == "ENG")
            {
                List<JobENGInhandModel> jobs = SummaryJobInHand.GetsENGJobInhand(year);
                jobs = jobs.Where(w => w.job_type == "Service").ToList();
                return Json(jobs);
            }
            if (department == "CIS")
            {
                List<JobCISInhandModel> jobs = SummaryJobInHand.GetsCISJobInhand(year);
                jobs = jobs.Where(w => w.job_type == "Service").ToList();
                return Json(jobs);
            }
            if (department == "AIS")
            {
                List<JobAISInhandModel> jobs = SummaryJobInHand.GetsAISJobInhand(year);
                jobs = jobs.Where(w => w.job_type == "Service").ToList();
                return Json(jobs);
            }
            return Json(null);
        }
        public IActionResult ExportSummaryJobInHand(string department,int year)
        {
            if (department == "ENG")
            {
                List<SummaryENGJobInHandModel> all = SummaryJobInHand.GetsAccENGJobInHand(year, "ALL");
                List<SummaryENGJobInHandModel> projects = SummaryJobInHand.GetsProjectENGJobInHand(year, "Project");
                List<SummaryENGJobInHandModel> services = SummaryJobInHand.GetsServiceENGJobInHand(year, "Service");

                //Download Excel
                var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "summary_job_in_hand.xlsx"));
                var stream = Export.ExportSummaryENGJobInHand(templateFileInfo, all, projects, services);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary_job_in_hand_ENG_" + year + ".xlsx");
            }
            if (department == "CIS")
            {
                List<SummaryCISJobInHandModel> all = SummaryJobInHand.GetsAccCISJobInHand(year, "ALL");
                List<SummaryCISJobInHandModel> projects = SummaryJobInHand.GetsProjectCISJobInHand(year, "Project");
                List<SummaryCISJobInHandModel> services = SummaryJobInHand.GetsServiceCISJobInHand(year, "Service");

                //Download Excel
                var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "summary_job_in_hand.xlsx"));
                var stream = Export.ExportSummaryCISJobInHand(templateFileInfo, all, projects, services);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary_job_in_hand_CIS_" + year + ".xlsx");
            }
            if (department == "AIS")
            {
                List<SummaryAISJobInHandModel> all = SummaryJobInHand.GetsAccAISJobInHand(year, "ALL");
                List<SummaryAISJobInHandModel> projects = SummaryJobInHand.GetsProjectAISJobInHand(year, "Project");
                List<SummaryAISJobInHandModel> services = SummaryJobInHand.GetsServiceAISJobInHand(year, "Service");

                //Download Excel
                var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "summary_job_in_hand.xlsx"));
                var stream = Export.ExportSummaryAISJobInHand(templateFileInfo, all, projects, services);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary_job_in_hand_AIS_" + year + ".xlsx");
            }
            return null;
        }
    }
}
