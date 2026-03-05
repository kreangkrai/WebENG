using Microsoft.AspNetCore.Hosting;
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
    public class InvoiceController : Controller
    {
        readonly IAccessory Accessory;
        readonly ISummaryInvoice SummaryInvoice;
        readonly ISummaryJobInHand SummaryJobInHand;
        readonly IExport Export;
        readonly IJob Job;
        protected readonly IHostingEnvironment _hostingEnvironment;
        readonly CTLInterfaces.IEmployee Employees;
        public InvoiceController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Accessory = new AccessoryService();
            SummaryInvoice = new SummayInvoiceService();
            SummaryJobInHand = new SummaryJobInHandService();
            Export = new ExportService();
            Employees = new CTLServices.EmployeeService();
            Job = new JobService();
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");

                List<UserModel> users = Accessory.getAllUser();
                List<CTLModels.EmployeeModel> emps = Employees.GetEmployees();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).FirstOrDefault();
                if (u == null)
                {
                    List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
                    CTLModels.EmployeeModel employee = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();
                    u = new UserModel()
                    {
                        emp_id = employee.emp_id,
                        name = employee.name_en,
                        role = "User",
                        department = employee.department
                    };
                }
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);

                if (!u.role.Contains("Admin"))
                {
                    string position = emps.Where(w => w.emp_id == u.emp_id).Select(s => s.position).FirstOrDefault();
                    u.role = position;
                }

                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public JsonResult GetInvoice(string department ,int year)
        {
            List<JobInHandModel> jobInHands = Job.GetJobInHands(year);
            jobInHands = jobInHands.Where(w => w.department == department).ToList();

            var monthlyGroups = jobInHands
            .Where(j => j.job_date != default)
            .GroupBy(j => new { j.job_date.Year, j.job_date.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalCES = g.Sum(j => j.eng_invoice),
                TotalCIS = g.Sum(j => j.cis_invoice),
                TotalAES = g.Sum(j => j.ais_invoice),
            })
            .OrderBy(g => g.Year)
            .ThenBy(g => g.Month)
            .ToList();

            var result = new List<SummaryInvoiceModel>();
            if (department == "CES")
            {
                double runningTotal = 0;

                foreach (var group in monthlyGroups)
                {
                    runningTotal = group.TotalCES;

                    result.Add(new SummaryInvoiceModel
                    {
                        month = $"{group.Year}-{group.Month:D2}",
                        invoice = Math.Round(runningTotal, 2),
                        target_month = 0
                    });
                }
            }
            if (department == "CIS")
            {
                double runningTotal = 0;

                foreach (var group in monthlyGroups)
                {
                    runningTotal = group.TotalCIS;

                    result.Add(new SummaryInvoiceModel
                    {
                        month = $"{group.Year}-{group.Month:D2}",
                        invoice = Math.Round(runningTotal, 2),
                        target_month = 0
                    });
                }
            }
            if (department == "AES")
            {
                double runningTotal = 0;

                foreach (var group in monthlyGroups)
                {
                    runningTotal = group.TotalAES;

                    result.Add(new SummaryInvoiceModel
                    {
                        month = $"{group.Year}-{group.Month:D2}",
                        invoice = Math.Round(runningTotal, 2),
                        target_month = 0
                    });
                }
            }
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetAccInvoice(string department,int year)
        {
            List<JobInHandModel> jobInHands = Job.GetJobInHands(year);
            jobInHands = jobInHands.Where(w => w.department == department).ToList();

            var monthlyGroups = jobInHands
            .Where(j => j.job_date != default)
            .GroupBy(j => new { j.job_date.Year, j.job_date.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalCES = g.Sum(j => j.eng_invoice),
                TotalCIS = g.Sum(j => j.cis_invoice),
                TotalAES = g.Sum(j => j.ais_invoice),
            })
            .OrderBy(g => g.Year)
            .ThenBy(g => g.Month)
            .ToList();
           
            var result = new List<SummaryInvoiceModel>();
            if (department == "CES")
            {
                double runningTotal = 0;

                foreach (var group in monthlyGroups)
                {
                    runningTotal += group.TotalCES;

                    result.Add(new SummaryInvoiceModel
                    {
                        month = $"{group.Year}-{group.Month:D2}",
                        invoice = Math.Round(runningTotal, 2),
                        target_month = 0
                    });
                }
            }
            if (department == "CIS")
            {
                double runningTotal = 0;

                foreach (var group in monthlyGroups)
                {
                    runningTotal += group.TotalCIS;

                    result.Add(new SummaryInvoiceModel
                    {
                        month = $"{group.Year}-{group.Month:D2}",
                        invoice = Math.Round(runningTotal, 2),
                        target_month = 0
                    });
                }
            }
            if (department == "AES")
            {
                double runningTotal = 0;

                foreach (var group in monthlyGroups)
                {
                    runningTotal += group.TotalAES;

                    result.Add(new SummaryInvoiceModel
                    {
                        month = $"{group.Year}-{group.Month:D2}",
                        invoice = Math.Round(runningTotal, 2),
                        target_month = 0
                    });
                }
            }
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetSummaryQuarter(string department,int year)
        {
            List<JobInHandModel> jobInHands = Job.GetJobInHands(year);
            jobInHands = jobInHands.Where(w => w.department == department).ToList();

            List<BackLogModel> backLogs = Job.GetBackLogs(year);
            backLogs = backLogs.Where(w => w.department == department).ToList();

            List<BackLogModel> remainingBackLog = Job.GetRemainingBackLogs(year);
            remainingBackLog = remainingBackLog.Where(w => w.department == department).ToList();

            if (department == "CES")
            {
                double backlog_volume = backLogs.Select(s => s.remaining_eng_in_hand).Sum();
                int backlog = backLogs.Select(s => s.job_id).Count();
                double backlog_pending_volume = remainingBackLog.Select(s => s.remaining_eng_in_hand).Sum();
                int backlog_incomplete = remainingBackLog.Select(s => s.job_id).Count();
                double backlog_invoice_volume = backlog_volume - backlog_pending_volume;
                int backlog_complete = backlog - backlog_incomplete;

                List<SummaryENGQuarterModel> quarters = new List<SummaryENGQuarterModel>();
                quarters.Add(new SummaryENGQuarterModel()
                {
                    quarter = "Backlog",
                    job_in_hand_volume = backlog_volume,
                    job_in_hand = backlog,
                    invoice_volume = backlog_invoice_volume,
                    invoice = backlog_complete,
                    pending_volume = backlog_pending_volume,
                    pending = backlog_incomplete
                });

                var job = jobInHands.Where(w => w.quarter == 1).ToList();
                double q1_volume = job.Select(s => s.job_eng_in_hand).Sum();
                int q1 = job.Select(s => s.job_id).Count();
                double q1_invoice_volume = job.Select(s => s.eng_invoice).Sum();
                int q1_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" || w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q1_pending_volume = q1_volume - q1_invoice_volume;
                int q1_incomplete = q1 - q1_complete;

                quarters.Add(new SummaryENGQuarterModel()
                {
                    quarter = $"Quarter 1' {year}",
                    job_in_hand_volume = q1_volume,
                    job_in_hand = q1,
                    invoice_volume = q1_invoice_volume,
                    invoice = q1_complete,
                    pending_volume = q1_pending_volume,
                    pending = q1_incomplete
                });

                job = jobInHands.Where(w => w.quarter == 2).ToList();
                double q2_volume = job.Select(s => s.job_eng_in_hand).Sum();
                int q2 = job.Select(s => s.job_id).Count();
                double q2_invoice_volume = job.Select(s => s.eng_invoice).Sum();
                int q2_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" ||  w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q2_pending_volume = q2_volume - q2_invoice_volume;
                int q2_incomplete = q2 - q2_complete;

                quarters.Add(new SummaryENGQuarterModel()
                {
                    quarter = $"Quarter 2' {year}",
                    job_in_hand_volume = q2_volume,
                    job_in_hand = q2,
                    invoice_volume = q2_invoice_volume,
                    invoice = q2_complete,
                    pending_volume = q2_pending_volume,
                    pending = q2_incomplete
                });

                job = jobInHands.Where(w => w.quarter == 3).ToList();
                double q3_volume = job.Select(s => s.job_eng_in_hand).Sum();
                int q3 = job.Select(s => s.job_id).Count();
                double q3_invoice_volume = job.Select(s => s.eng_invoice).Sum();
                int q3_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" || w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q3_pending_volume = q3_volume - q3_invoice_volume;
                int q3_incomplete = q3 - q3_complete;

                quarters.Add(new SummaryENGQuarterModel()
                {
                    quarter = $"Quarter 3' {year}",
                    job_in_hand_volume = q3_volume,
                    job_in_hand = q3,
                    invoice_volume = q3_invoice_volume,
                    invoice = q3_complete,
                    pending_volume = q3_pending_volume,
                    pending = q3_incomplete
                });

                job = jobInHands.Where(w => w.quarter == 4).ToList();
                double q4_volume = job.Select(s => s.job_eng_in_hand).Sum();
                int q4 = job.Select(s => s.job_id).Count();
                double q4_invoice_volume = job.Select(s => s.eng_invoice).Sum();
                int q4_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" ||w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q4_pending_volume = q4_volume - q4_invoice_volume;
                int q4_incomplete = q4 - q4_complete;

                quarters.Add(new SummaryENGQuarterModel()
                {
                    quarter = $"Quarter 4' {year}",
                    job_in_hand_volume = q4_volume,
                    job_in_hand = q4,
                    invoice_volume = q4_invoice_volume,
                    invoice = q4_complete,
                    pending_volume = q4_pending_volume,
                    pending = q4_incomplete
                });

                quarters.Add(new SummaryENGQuarterModel()
                {
                    quarter = "Summary",
                    job_in_hand_volume = backlog_volume + q1_volume + q2_volume + q3_volume + q4_volume,
                    job_in_hand = backlog + q1 + q2 + q3 + q4,
                    invoice_volume = backlog_invoice_volume + q1_invoice_volume + q2_invoice_volume + q3_invoice_volume + q4_invoice_volume,
                    invoice = backlog_complete + q1_complete + q2_complete + q3_complete + q4_complete,
                    pending_volume = backlog_pending_volume + q1_pending_volume + q2_pending_volume + q3_pending_volume + q4_pending_volume,
                    pending = backlog_incomplete + q1_incomplete + q2_incomplete + q3_incomplete + q4_incomplete
                });
                return Json(quarters);
            }
            if (department == "CIS")
            {

                List<SummaryCISQuarterModel> quarters = new List<SummaryCISQuarterModel>();

                double backlog_volume = backLogs.Select(s => s.remaining_cis_in_hand).Sum();
                int backlog = backLogs.Select(s => s.job_id).Count();
                double backlog_pending_volume = remainingBackLog.Select(s => s.remaining_cis_in_hand).Sum();
                int backlog_incomplete = remainingBackLog.Select(s => s.job_id).Count();
                double backlog_invoice_volume = backlog_volume - backlog_pending_volume;
                int backlog_complete = backlog - backlog_incomplete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = "Backlog",
                    job_in_hand_volume = backlog_volume,
                    job_in_hand = backlog,
                    invoice_volume = backlog_invoice_volume,
                    invoice = backlog_complete,
                    pending_volume = backlog_pending_volume,
                    pending = backlog_incomplete
                });

                var job = jobInHands.Where(w => w.quarter == 1).ToList();
                double q1_volume = job.Select(s => s.job_cis_in_hand).Sum();
                int q1 = job.Select(s => s.job_id).Count();
                double q1_invoice_volume = job.Select(s => s.cis_invoice).Sum();
                int q1_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" || w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q1_pending_volume = q1_volume - q1_invoice_volume;
                int q1_incomplete = q1 - q1_complete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = $"Quarter 1' {year}",
                    job_in_hand_volume = q1_volume,
                    job_in_hand = q1,
                    invoice_volume = q1_invoice_volume,
                    invoice = q1_complete,
                    pending_volume = q1_pending_volume,
                    pending = q1_incomplete
                });

                job = jobInHands.Where(w => w.quarter == 2).ToList();
                double q2_volume = job.Select(s => s.job_cis_in_hand).Sum();
                int q2 = job.Select(s => s.job_id).Count();
                double q2_invoice_volume = job.Select(s => s.cis_invoice).Sum();
                int q2_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" || w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q2_pending_volume = q2_volume - q2_invoice_volume;
                int q2_incomplete = q2 - q2_complete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = $"Quarter 2' {year}",
                    job_in_hand_volume = q2_volume,
                    job_in_hand = q2,
                    invoice_volume = q2_invoice_volume,
                    invoice = q2_complete,
                    pending_volume = q2_pending_volume,
                    pending = q2_incomplete
                });

                job = jobInHands.Where(w => w.quarter == 3).ToList();
                double q3_volume = job.Select(s => s.job_cis_in_hand).Sum();
                int q3 = job.Select(s => s.job_id).Count();
                double q3_invoice_volume = job.Select(s => s.cis_invoice).Sum();
                int q3_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" || w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q3_pending_volume = q3_volume - q3_invoice_volume;
                int q3_incomplete = q3 - q3_complete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = $"Quarter 3' {year}",
                    job_in_hand_volume = q3_volume,
                    job_in_hand = q3,
                    invoice_volume = q3_invoice_volume,
                    invoice = q3_complete,
                    pending_volume = q3_pending_volume,
                    pending = q3_incomplete
                });

                job = jobInHands.Where(w => w.quarter == 4).ToList();
                double q4_volume = job.Select(s => s.job_cis_in_hand).Sum();
                int q4 = job.Select(s => s.job_id).Count();
                double q4_invoice_volume = job.Select(s => s.cis_invoice).Sum();
                int q4_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" ||w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q4_pending_volume = q4_volume - q4_invoice_volume;
                int q4_incomplete = q4 - q4_complete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = $"Quarter 4' {year}",
                    job_in_hand_volume = q4_volume,
                    job_in_hand = q4,
                    invoice_volume = q4_invoice_volume,
                    invoice = q4_complete,
                    pending_volume = q4_pending_volume,
                    pending = q4_incomplete
                });

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = "Summary",
                    job_in_hand_volume = backlog_volume + q1_volume + q2_volume + q3_volume + q4_volume,
                    job_in_hand = backlog + q1 + q2 + q3 + q4,
                    invoice_volume = backlog_invoice_volume + q1_invoice_volume + q2_invoice_volume + q3_invoice_volume + q4_invoice_volume,
                    invoice = backlog_complete + q1_complete + q2_complete + q3_complete + q4_complete,
                    pending_volume = backlog_pending_volume + q1_pending_volume + q2_pending_volume + q3_pending_volume + q4_pending_volume,
                    pending = backlog_incomplete + q1_incomplete + q2_incomplete + q3_incomplete + q4_incomplete
                });
                return Json(quarters);
            }
            if (department == "AES")
            {
                List<SummaryAISQuarterModel> quarters = new List<SummaryAISQuarterModel>();

                double backlog_volume = backLogs.Select(s => s.remaining_ais_in_hand).Sum();
                int backlog = backLogs.Select(s => s.job_id).Count();
                double backlog_pending_volume = remainingBackLog.Select(s => s.remaining_ais_in_hand).Sum();
                int backlog_incomplete = remainingBackLog.Select(s => s.job_id).Count();
                double backlog_invoice_volume = backlog_volume - backlog_pending_volume;
                int backlog_complete = backlog - backlog_incomplete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = "Backlog",
                    job_in_hand_volume = backlog_volume,
                    job_in_hand = backlog,
                    invoice_volume = backlog_invoice_volume,
                    invoice = backlog_complete,
                    pending_volume = backlog_pending_volume,
                    pending = backlog_incomplete
                });

                var job = jobInHands.Where(w => w.quarter == 1).ToList();
                double q1_volume = job.Select(s => s.job_ais_in_hand).Sum();
                int q1 = job.Select(s => s.job_id).Count();
                double q1_invoice_volume = job.Select(s => s.ais_invoice).Sum();
                int q1_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" || w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q1_pending_volume = q1_volume - q1_invoice_volume;
                int q1_incomplete = q1 - q1_complete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = $"Quarter 1' {year}",
                    job_in_hand_volume = q1_volume,
                    job_in_hand = q1,
                    invoice_volume = q1_invoice_volume,
                    invoice = q1_complete,
                    pending_volume = q1_pending_volume,
                    pending = q1_incomplete
                });

                job = jobInHands.Where(w => w.quarter == 2).ToList();
                double q2_volume = job.Select(s => s.job_ais_in_hand).Sum();
                int q2 = job.Select(s => s.job_id).Count();
                double q2_invoice_volume = job.Select(s => s.ais_invoice).Sum();
                int q2_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" || w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q2_pending_volume = q2_volume - q2_invoice_volume;
                int q2_incomplete = q2 - q2_complete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = $"Quarter 2' {year}",
                    job_in_hand_volume = q2_volume,
                    job_in_hand = q2,
                    invoice_volume = q2_invoice_volume,
                    invoice = q2_complete,
                    pending_volume = q2_pending_volume,
                    pending = q2_incomplete
                });

                job = jobInHands.Where(w => w.quarter == 3).ToList();
                double q3_volume = job.Select(s => s.job_ais_in_hand).Sum();
                int q3 = job.Select(s => s.job_id).Count();
                double q3_invoice_volume = job.Select(s => s.ais_invoice).Sum();
                int q3_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" || w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q3_pending_volume = q3_volume - q3_invoice_volume;
                int q3_incomplete = q3 - q3_complete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = $"Quarter 3' {year}",
                    job_in_hand_volume = q3_volume,
                    job_in_hand = q3,
                    invoice_volume = q3_invoice_volume,
                    invoice = q3_complete,
                    pending_volume = q3_pending_volume,
                    pending = q3_incomplete
                });

                job = jobInHands.Where(w => w.quarter == 4).ToList();
                double q4_volume = job.Select(s => s.job_ais_in_hand).Sum();
                int q4 = job.Select(s => s.job_id).Count();
                double q4_invoice_volume = job.Select(s => s.ais_invoice).Sum();
                int q4_complete = job.Where(w => w.finished_date.Year == year && (w.status_name == "Warranty" || w.status_name == "Finished")).Select(s => s.job_id).Count();
                double q4_pending_volume = q4_volume - q4_invoice_volume;
                int q4_incomplete = q4 - q4_complete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = $"Quarter 4' {year}",
                    job_in_hand_volume = q4_volume,
                    job_in_hand = q4,
                    invoice_volume = q4_invoice_volume,
                    invoice = q4_complete,
                    pending_volume = q4_pending_volume,
                    pending = q4_incomplete
                });

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = "Summary",
                    job_in_hand_volume = backlog_volume + q1_volume + q2_volume + q3_volume + q4_volume,
                    job_in_hand = backlog + q1 + q2 + q3 + q4,
                    invoice_volume = backlog_invoice_volume + q1_invoice_volume + q2_invoice_volume + q3_invoice_volume + q4_invoice_volume,
                    invoice = backlog_complete + q1_complete + q2_complete + q3_complete + q4_complete,
                    pending_volume = backlog_pending_volume + q1_pending_volume + q2_pending_volume + q3_pending_volume + q4_pending_volume,
                    pending = backlog_incomplete + q1_incomplete + q2_incomplete + q3_incomplete + q4_incomplete
                });
                return Json(quarters);
            }
            return Json(null);
        }

        [HttpGet]
        public JsonResult GetOrderInTake(string department, int year)
        {
            List<OrderInTakeModel> orderInTakes = new List<OrderInTakeModel>();
            for (int y = 2019; y <= year; y++)
            {
                List<JobInHandModel> jobInHands = Job.GetJobInHands(y);
                jobInHands = jobInHands.Where(w => w.department == department).ToList();

                var yearInHandGroups = jobInHands
                .Where(j => j.job_date != default)
                .GroupBy(j => new { j.job_date.Year })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    TotalCES = g.Sum(j => j.job_eng_in_hand),
                    TotalCIS = g.Sum(j => j.job_cis_in_hand),
                    TotalAES = g.Sum(j => j.job_ais_in_hand),
                })
                .OrderBy(g => g.Year)
                .ToList();

                List<BackLogModel> backLogs = Job.GetRemainingBackLogs(y);
                backLogs = backLogs.Where(w => w.department == department).ToList();

                var yearBackLogGroups = backLogs
               .Where(j => j.job_date != default)
               .GroupBy(j => new { j.job_date.Year })
               .Select(g => new
               {
                   Year = g.Key.Year,
                   TotalCES = g.Sum(j => j.remaining_eng_in_hand),
                   TotalCIS = g.Sum(j => j.remaining_cis_in_hand),
                   TotalAES = g.Sum(j => j.remaining_ais_in_hand),
               })
               .OrderBy(g => g.Year)
               .ToList();

                OrderInTakeModel orderInTake = new OrderInTakeModel();
                if (department == "CES")
                {
                    orderInTake = new OrderInTakeModel()
                    {
                        year = y,
                        target = 200,
                        backlog = yearBackLogGroups.Select(s => s.TotalCES).Sum(),
                        job_in_hand = yearInHandGroups.Select(s => s.TotalCES).Sum(),
                    };
                }
                if (department == "CIS")
                {
                    orderInTake = new OrderInTakeModel()
                    {
                        year = y,
                        target = 200,
                        backlog = yearBackLogGroups.Select(s => s.TotalCIS).Sum(),
                        job_in_hand = yearInHandGroups.Select(s => s.TotalCIS).Sum(),
                    };
                }
                if (department == "AES")
                {
                    orderInTake = new OrderInTakeModel()
                    {
                        year = y,
                        target = 200,
                        backlog = yearBackLogGroups.Select(s => s.TotalAES).Sum(),
                        job_in_hand = yearInHandGroups.Select(s => s.TotalAES).Sum(),
                    };
                }
                orderInTakes.Add(orderInTake);
            }

            return Json(orderInTakes);
        }
        public IActionResult ExportSummarySaleTurnOver(string department, int year)
        {
            List<JobInHandModel> jobInHands = Job.GetJobInHands(year);
            jobInHands = jobInHands.Where(w => w.department == department).ToList();

            var monthlyGroups = jobInHands
            .Where(j => j.job_date != default)
            .GroupBy(j => new { j.job_date.Year, j.job_date.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalCES = g.Sum(j => j.eng_invoice),
                TotalCIS = g.Sum(j => j.cis_invoice),
                TotalAES = g.Sum(j => j.ais_invoice),
            })
            .OrderBy(g => g.Year)
            .ThenBy(g => g.Month)
            .ToList();

            var invoices = new List<SummaryInvoiceModel>();
            if (department == "CES")
            {
                double runningTotal = 0;
                double runningAccTotal = 0;
                foreach (var group in monthlyGroups)
                {
                    runningTotal = group.TotalCES;
                    runningAccTotal += group.TotalCES;
                    invoices.Add(new SummaryInvoiceModel
                    {
                        month = $"{group.Year}-{group.Month:D2}",
                        invoice = Math.Round(runningTotal, 2),
                        invoice_acc = Math.Round(runningAccTotal, 2),
                        target_month = 0
                    });
                }
            }
            if (department == "CIS")
            {
                double runningTotal = 0;
                double runningAccTotal = 0;
                foreach (var group in monthlyGroups)
                {
                    runningTotal = group.TotalCIS;
                    runningAccTotal += group.TotalCIS;
                    invoices.Add(new SummaryInvoiceModel
                    {
                        month = $"{group.Year}-{group.Month:D2}",
                        invoice = Math.Round(runningTotal, 2),
                        invoice_acc = Math.Round(runningAccTotal, 2),
                        target_month = 0
                    });
                }
            }
            if (department == "AES")
            {
                double runningTotal = 0;
                double runningAccTotal = 0;
                foreach (var group in monthlyGroups)
                {
                    runningTotal = group.TotalAES;
                    runningAccTotal += group.TotalAES;
                    invoices.Add(new SummaryInvoiceModel
                    {
                        month = $"{group.Year}-{group.Month:D2}",
                        invoice = Math.Round(runningTotal, 2),
                        invoice_acc = Math.Round(runningAccTotal, 2),
                        target_month = 0
                    });
                }
            }

            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "summary_sale_turnover.xlsx"));
            var stream = Export.ExportSummarySaleTurnOver(templateFileInfo, invoices);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary_sale_turnover_" + year + ".xlsx");
        }
    }
}
