﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class ServiceReportController : Controller
    {
        readonly IAccessory Accessory;
        private IJob Job;
        readonly IDailyReport DailyReport;
        private IExport Export;
        private readonly IHostingEnvironment _hostingEnvironment;
        static string _job = "";
        public ServiceReportController(IHostingEnvironment hostingEnvironment)
        {
            Accessory = new AccessoryService();
            Job = new JobService();
            DailyReport = new DailyReportService();
            Export = new ExportService();
            _hostingEnvironment = hostingEnvironment;
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
                HttpContext.Session.SetString("Role", u.role);
                HttpContext.Session.SetString("Department", u.department);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        
        [HttpGet]
        public List<string> GetJobs()
        {
            List<string> jobs = Job.GetAllJobs().Select(s => s.job_id).ToList();
            jobs = jobs.OrderBy(o => o).ToList();
            return jobs;
        }

        [HttpGet]
        public IActionResult GetReportByJob(string job)
        {
            _job = job;
            List<DailyActivityModel> reports = DailyReport.GetServiceActivities(job);
            return Json(reports);
        }

        public ActionResult ExportReport()
        {
            List<DailyActivityModel> reports = DailyReport.GetServiceActivities(_job);
            //Download Excel
            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/Template", "service_report.xlsx"));
            var stream = Export.ExportServiceReport(templateFileInfo, reports);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "service_report_" + $"{_job}.xlsx");
        }
    }
}
