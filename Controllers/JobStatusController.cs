using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class JobStatusController : Controller
    {
        readonly IAccessory Accessory;
        readonly IJobStatus JobStatus;
        readonly IStatus Status;
        readonly IExport Export;
        protected readonly IHostingEnvironment _hostingEnvironment;
        public JobStatusController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Accessory = new AccessoryService();
            JobStatus = new JobStatusService();
            Status = new EngStatusService();
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
        public JsonResult GetWorkingUser()
        {
            List<UserModel> users = Accessory.getWorkingUser();

            return Json(users);
        }

        [HttpGet]
        public JsonResult GetJobStatusByUser(string user)
        {
            List<JobModel> jobs = new List<JobModel>();
            if (user == "ALL")
            {
                jobs = JobStatus.GetJobStatusALL();
            }
            else
            {
                jobs = JobStatus.GetJobStatusByUser(user);
            }
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetStatus()
        {
            List<EngStatusModel> statuses = Status.GetStatuses();
            return Json(statuses);
        }

        [HttpPatch]
        public JsonResult UpdateJobStatus(string job,string status)
        {
            var result = JobStatus.UpdateJobStatus(job, status);
            return Json(result);
        }

        [HttpPatch]
        public JsonResult UpdateJobByProcessSystem(string job_string)
        {
            JobModel job = JsonConvert.DeserializeObject<JobModel>(job_string);
            var result = JobStatus.UpdateJobByProcessSystem(job);
            return Json(result);
        }

        public IActionResult ExportData(string user)
        {
            List<JobModel> jobs = new List<JobModel>();
            if (user == "ALL")
            {
                jobs = JobStatus.GetJobStatusALL();
            }
            else
            {
                jobs = JobStatus.GetJobStatusByUser(user);
            }

            //Download Excel
            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "project_inhand.xlsx"));
            var stream = Export.ExportData(templateFileInfo, jobs);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", user + ".xlsx");
        }

    }
}
