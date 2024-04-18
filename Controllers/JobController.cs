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

namespace WebENG.Controllers
{
    public class JobController : Controller
    {
        readonly IJob JobService;
        readonly IAccessory Accessory;
        readonly IStatus Status;
        readonly IInvoice Invoice;
        public JobController()
        {
            JobService = new JobService();
            Accessory = new AccessoryService();
            Status = new EngStatusService();
            Invoice = new InvoiceService();
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
    }
}
