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
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebENG.Controllers
{
    public class JobController : Controller
    {
        readonly IJob JobService;
        readonly IJobFile JobFile;
        readonly IAccessory Accessory;
        readonly IStatus Status;
        readonly IInvoice Invoice;
        readonly IExport Export;
        readonly IEngUser EngUserService;
        protected readonly IHostingEnvironment _hostingEnvironment;
        static string _job_id;
        static string _item;
        public JobController(IHostingEnvironment hostingEnvironment)
        {
            JobService = new JobService();
            JobFile = new JobFileService();
            Accessory = new AccessoryService();
            Status = new EngStatusService();
            Invoice = new InvoiceService();
            Export = new ExportService();
            EngUserService = new EngUserService();
            _hostingEnvironment = hostingEnvironment;
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

                // Update Status With Warranty
                List<string> jobs = JobService.GetDueWarranty();
                for (int i = 0; i < jobs.Count; i++)
                {
                    JobService.UpdateFinish(jobs[i]);
                }
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
        public JsonResult GetAllUsers()
        {
            List<EngUserModel> users = EngUserService.GetUsers().Where(w=>w.group != "" && w.active == true).OrderBy(o => o.user_id).ToList();
            return Json(users);
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
            List<JobSummaryModel> sum = jobsSummary.GroupBy(g => g.jobId).Select(s => new JobSummaryModel()
            {
                jobId = s.Key,
                jobName = jobsSummary.Where(w=>w.jobId == s.Key).FirstOrDefault().jobName,
                customer = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().customer,
                cost = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().cost,
                factor = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().factor,
                totalManhour = s.Sum(k=>k.totalManhour),
                status = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().status,
                process = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().process,
                system = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().system,
                remainingCost = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().cost - s.Sum(k=>k.totalEngCost)

            }).ToList();
            return Json(sum);
        }

        [HttpGet]
        public JsonResult GetJobsSummaryByJob(string job)
        {
            List<JobSummaryModel> jobsSummary = JobService.GetJobsSummary();
            List<JobSummaryModel> sum = jobsSummary.GroupBy(g => g.jobId).Select(s => new JobSummaryModel()
            {
                jobId = s.Key,
                jobName = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().jobName,
                customer = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().customer,
                cost = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().cost,
                factor = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().factor,
                totalManhour = s.Sum(k => k.totalManhour),
                status = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().status,
                process = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().process,
                system = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().system,
                remainingCost = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().cost - s.Sum(k => k.totalEngCost)

            }).ToList();
            JobSummaryModel _sum = sum.Where(w => w.jobId == job).FirstOrDefault();
            return Json(_sum);
        }

        [HttpPost]
        public JsonResult AddJob(string job_string)
        {
            JobModel job = JsonConvert.DeserializeObject<JobModel>(job_string);
            var result = JobService.CreateJob(job);
            if (result == "Success")
            {
                result = JobService.CreateTermPayment(job.term_payment);
                if (result == "Success")
                {
                    result = Invoice.Insert(job.invoices);
                    if (result == "Success")
                    {
                        result = JobFile.CreateJobFile(job.job_id);
                    }
                }
            }
            return Json(result);
        }

        [HttpPatch]
        public JsonResult UpdateJob(string job_string)
        {
            JobModel job = JsonConvert.DeserializeObject<JobModel>(job_string);
            string result = JobService.UpdateJob(job);

            if (result == "Success")
            {
                result = JobService.UpdateTermPayment(job.term_payment);
                if (result == "Success")
                {
                    string delete = Invoice.Delete(job.job_id);
                    if (delete == "Success")
                    {
                        result = Invoice.Insert(job.invoices);
                    }
                }
            }
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetQuotations()
        {
            List<JobQuotationModel> quots = JobService.GetJobQuotations();
            return Json(quots);
        }

        [HttpGet]
        public JsonResult GetJobFileByJob(string job_id)
        {
            JobFileModel job = JobFile.GetJobFile(job_id);
            return Json(job);
        }

        [HttpPost]
        public string InsertFile(string job_id,string item)
        {
            _job_id = job_id;
            _item = item;
            return "Success";
        }

        [HttpPost]
        public IActionResult ImportJobFile()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "";
            string item = "";
            if (_item == "Quotation")
            {
                folderName = $"backup/{_job_id}/Quotation/";
                item = "quotation";
            }
            if (_item == "PO")
            {
                folderName = $"backup/{_job_id}/PO/";
                item = "po";
            }
            if (_item == "Hand Over")
            {
                folderName = $"backup/{_job_id}/Hand Over/";
                item = "hand_over";
            }
            
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(newPath);
                foreach (FileInfo f in di.GetFiles())
                {
                    f.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {

                    dir.Delete(true);
                }
                Directory.CreateDirectory(newPath);

            }

            if (file.Length > 0)
            {
                string fullPath = Path.Combine(newPath, file.FileName);
                FileStream stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);

                stream.Position = 0;
                stream.Close();

                string scheme = Request.Scheme;
                string host = Request.Host.Host;
                string path = folderName + file.FileName;
                //string _path = scheme + "://" + host + "/eng/Job/" + path;
                string _path = scheme + "://" + host +"/eng/" + path;
                string msg = JobFile.UpdateJobFileByItem(_job_id, item, _path);
            }
            return Json("Success");
        }
        public IActionResult ExportJob()
        {
            List<JobModel> jobs = JobService.GetAllJobs();
            //Download Excel
            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/Template", "jobs.xlsx"));
            var stream = Export.ExportJob(templateFileInfo, jobs);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "jobs_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx");
        }
    }
}
