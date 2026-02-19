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
        readonly IJobOwner JobOwner;
        readonly IJobFile JobFile;
        readonly IAccessory Accessory;
        readonly IStatus Status;
        readonly IInvoice Invoice;
        readonly IExport Export;
        readonly IEngUser EngUserService;
        readonly IWorkingHours WorkingHours;
        readonly IHoliday Holiday;
        readonly IJobResponsible JobResponsible;
        protected readonly IHostingEnvironment _hostingEnvironment;
        readonly CTLInterfaces.IEmployee Employees;
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
            JobOwner = new JobOwnerService();
            WorkingHours = new WorkingHoursService();
            Holiday = new HolidayService();
            JobResponsible = new JobResponsibleService();
            _hostingEnvironment = hostingEnvironment;
            Employees = new CTLServices.EmployeeService();
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
                        department = employee.department,
                    };
                }
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);

                // Update Status With Warranty
                List<string> jobs = JobService.GetDueWarranty();
                for (int i = 0; i < jobs.Count; i++)
                {
                    JobService.UpdateFinish(jobs[i]);
                }

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
        public IActionResult JobsSummary()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { name = s.name, department = s.department, role = s.role, emp_id = s.emp_id }).FirstOrDefault();
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
            List<EngUserModel> users = EngUserService.GetUsers().Where(w => w.group != "" && w.active == true).OrderBy(o => o.user_name).ToList();
            return Json(users);
        }

        [HttpPost]
        public JsonResult GetJobs(string[] set_select)
        {
            string[] selections = set_select;
            List<JobModel> jobs = JobService.GetAllJobs();
            List<JobModel> new_jobs = new List<JobModel>();
            List<JobOwnerModel> jobs_owner = JobOwner.GetJobOwner();
            List<string> _jobs = new List<string>();
            List<string> _except_job = jobs.Where(w => !jobs_owner.Any(x => x.job_id == w.job_id)).Select(s => s.job_id).ToList();
            for (int i = 0; i < jobs_owner.Count; i++)
            {
                for (int j = 0; j < selections.Length; j++)
                {
                    if (jobs_owner[i].job_department == selections[j])
                    {
                        if (!_jobs.Any(a => a == jobs_owner[i].job_id))
                        {
                            _jobs.Add(jobs_owner[i].job_id);
                            continue;
                        }
                    }
                }
            }
            for (int i = 0; i < _except_job.Count; i++)
            {
                _jobs.Add(_except_job[i]);
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                for (int j = 0; j < _jobs.Count; j++)
                {
                    if (jobs[i].job_id == _jobs[j])
                    {
                        new_jobs.Add(jobs[i]);
                        continue;
                    }
                }
            }

            var data = new { jobs = new_jobs, jobs_owner = jobs_owner };
            return Json(data);
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
            List<JobResponsibleModel> jr = JobResponsible.GetJobsResponsible();
            List<JobsWorkingHoursModel> jwh = new List<JobsWorkingHoursModel>();
            List<WorkingHoursModel> workings = WorkingHours.GetWorkingHours();
            List<string> jobs = workings.GroupBy(g => g.job_id).Select(s => s.FirstOrDefault().job_id).OrderBy(o => o).ToList();
            List<HolidayModel> holidays = Holiday.GetAllHolidays();

            DateTime start = new DateTime(2022, 1, 1);
            DateTime stop = DateTime.Now;

            List<WorkingHoursModel> whs = WorkingHours.CalculateWorkingHours("ALL", start, stop);
            whs = whs.Where(w => w.job_id != "" && w.job_id != "J999999").ToList();

            for (int i = 0; i < jobs.Count; i++)
            {
                List<CTLModels.EmployeeModel> emps = workings.Where(w => w.job_id == jobs[i]).GroupBy(g => g.emp_id).Select(s => new CTLModels.EmployeeModel()
                {
                    emp_id = s.Key,
                    department = s.FirstOrDefault().department
                }).ToList();
                
                JobsWorkingHoursModel sum_jwh = new JobsWorkingHoursModel();
                for (int j = 0; j < emps.Count; j++)
                {
                    int level = 1;
                    if (jr.Any(w => w.job_id == jobs[i] && w.emp_id == emps[j].emp_id))
                    {
                        level = jr.Where(w => w.job_id == jobs[i] && w.emp_id == emps[j].emp_id).FirstOrDefault().level;
                    }
                    List<WorkingHoursModel> workings_ = workings.Where(w => w.job_id == jobs[i] && w.emp_id == emps[j].emp_id).ToList();
                    List<WorkingDayModel> wd = workings_.GroupBy(g => g.working_date).Select(s => new WorkingDayModel()
                    {
                        date = s.Key,
                        workings = workings_.Where(w => w.working_date == s.Key).ToList()
                    }).ToList();

                    var _whs = whs.Where(w => w.job_id == jobs[i] && w.emp_id == emps[j].emp_id).ToList();
                    List<EngineerJobTimeModel> wh = _whs.GroupBy(g => g.job_id).Select(s => new EngineerJobTimeModel()
                    {
                        normal = s.Aggregate(TimeSpan.Zero, (sum_, x) => sum_ + x.normal).TotalHours,
                        ot1_5 = s.Aggregate(TimeSpan.Zero, (sum_, x) => sum_ + x.ot1_5).TotalHours,
                        ot3_0 = s.Aggregate(TimeSpan.Zero, (sum_, x) => sum_ + x.ot3_0).TotalHours,
                        job = s.Key,
                        job_name = s.FirstOrDefault().job_name
                    }).ToList();

                    string department = emps[j].department;
                    int amount = 3200;
                    if (department == "CES-CIS" || department == "AES")
                    {
                        amount = 2400;
                    }

                    JobsWorkingHoursModel jwh_ = new JobsWorkingHoursModel()
                    {
                        job_id = jobs[i],
                        normal = wh.Sum(s => s.normal) * level,
                        ot1_5 = wh.Sum(s => s.ot1_5) * level,
                        ot3_0 = wh.Sum(s => s.ot3_0) * level,
                        total = ((wh.Sum(s => s.normal) * level) +
                        (wh.Sum(s => s.ot1_5) * 1.5 * level) +
                        (wh.Sum(s => s.ot3_0) * 3.0 * level)),
                        total_amount = (wh.Sum(s => s.normal) / 8 * level * amount),
                        total_ot_amount = ((wh.Sum(s => s.normal) / 8 * level * amount) +
                        (wh.Sum(s => s.ot1_5) / 8 * level * amount * 1.5) +
                        (wh.Sum(s => s.ot3_0) / 8 * level * amount * 3.0)),
                    };

                    sum_jwh.job_id = jwh_.job_id;
                    sum_jwh.normal += jwh_.normal;
                    sum_jwh.ot1_5 += jwh_.ot1_5;
                    sum_jwh.ot3_0 += jwh_.ot3_0;
                    sum_jwh.total += jwh_.total;
                    sum_jwh.total_amount += jwh_.total_amount;
                    sum_jwh.total_ot_amount += jwh_.total_ot_amount;

                }
                jwh.Add(sum_jwh);
            }

            List<JobSummaryModel> jobsSummary = JobService.GetJobsSummary();

            var summaryByJob = jobsSummary
                .GroupBy(j => j.jobId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        First = g.First(),
                        TotalManhour = g.Sum(x => x.totalManhour),
                        TotalCost = g.Sum(x => x.totalCost),
                        RemainingCost = g.Sum(x => x.remainingCost),
                    });

            var otCostByJob = jwh
                .GroupBy(x => x.job_id)
                .ToDictionary(
                    g => g.Key,
                    g => new { total = g.Sum(x => x.normal), total_ot = g.Sum(x => x.total), amount = g.Sum(x => x.total_amount), ot_amount = g.Sum(x => x.total_ot_amount) }
                );

            var sum = summaryByJob.Select(kvp =>
            {
                var jobId = kvp.Key;
                var data = kvp.Value;
                var first = data.First;

                double total = otCostByJob.TryGetValue(jobId, out var t) ? t.total : 0.0;
                double totalOT = otCostByJob.TryGetValue(jobId, out var to) ? to.total_ot : 0.0;
                double amount = otCostByJob.TryGetValue(jobId, out var a) ? a.amount : 0.0;
                double amountOT = otCostByJob.TryGetValue(jobId, out var ao) ? ao.ot_amount : 0.0;

                double totalBudget = first.eng_cost + first.cis_cost + first.ais_cost;

                double remaining = totalBudget - amount;
                double remainingOT = totalBudget - amountOT;

                return new JobSummaryModel
                {
                    jobId = jobId,
                    jobName = first.jobName,
                    customer = first.customer,
                    responsible = first.responsible,
                    eng_cost = first.eng_cost,
                    cis_cost = first.cis_cost,
                    ais_cost = first.ais_cost,
                    factor = first.factor,
                    totalManhour = total,
                    totalOTManhour = totalOT,
                    status = first.status,
                    process = first.process,
                    system = first.system,
                    totalCost = data.TotalCost,
                    totalAmount = amount,
                    totalOTAmount = amountOT,
                    remainingCost = Math.Round(remaining, 0),
                    remainingOTCost = Math.Round(remainingOT, 0),
                    term_payments = first.term_payments
                };
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
                responsible = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().responsible,
                eng_cost = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().eng_cost,
                cis_cost = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().cis_cost,
                ais_cost = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().ais_cost,
                factor = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().factor,
                totalManhour = s.Sum(k => k.totalManhour),
                status = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().status,
                process = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().process,
                system = jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().system,
                remainingCost = (jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().eng_cost
                + jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().cis_cost
                + jobsSummary.Where(w => w.jobId == s.Key).FirstOrDefault().ais_cost) - s.Sum(k => k.totalCost)

            }).ToList();
            JobSummaryModel _sum = sum.Where(w => w.jobId == job).FirstOrDefault();
            return Json(_sum);
        }

        [HttpGet]
        public JsonResult GetManpowerOTRatio(string job_id)
        {
            List<JobResponsibleModel> jr = JobResponsible.GetJobsResponsible();
            List<JobsWorkingHoursModel> jwh = new List<JobsWorkingHoursModel>();
            List<WorkingHoursModel> workings = WorkingHours.GetWorkingHours();

            List<HolidayModel> holidays = Holiday.GetAllHolidays();

            List<string> emps = workings.Where(w => w.job_id == job_id).GroupBy(g => g.emp_id).Select(s => s.FirstOrDefault().emp_id).ToList();
            for (int j = 0; j < emps.Count; j++)
            {
                int level = 1;
                if (jr.Any(w => w.job_id == job_id && w.emp_id == emps[j]))
                {
                    level = jr.Where(w => w.job_id == job_id && w.emp_id == emps[j]).FirstOrDefault().level;
                }
                List<WorkingHoursModel> workings_ = workings.Where(w => w.job_id == job_id && w.emp_id == emps[j]).ToList();
                List<WorkingDayModel> wd = workings_.GroupBy(g => g.working_date).Select(s => new WorkingDayModel()
                {
                    date = s.Key,
                    workings = workings_.Where(w => w.working_date == s.Key).ToList()
                }).ToList();
                List<WorkingHoursModel> wh = CalculateWorkingHours(wd, holidays);
                JobsWorkingHoursModel jwh_ = new JobsWorkingHoursModel()
                {
                    job_id = job_id,
                    emp_id = workings_.FirstOrDefault().emp_id,
                    user_name = workings_.FirstOrDefault().user_name,
                    normal = wh.Sum(s => s.normal.TotalMinutes) / 60.0 * level,
                    ot1_5 = wh.Sum(s => s.ot1_5.TotalMinutes) / 60.0 * level,
                    ot3_0 = wh.Sum(s => s.ot3_0.TotalMinutes) / 60.0 * level,
                    total = ((wh.Sum(s => s.normal.TotalMinutes) / 60.0 * level) +
                    (wh.Sum(s => s.ot1_5.TotalMinutes) / 60.0 * 1.5 * level) +
                    (wh.Sum(s => s.ot3_0.TotalMinutes) / 60.0 * 3.0 * level))
                };
                jwh.Add(jwh_);
            }
            List<ManpowerRatioModel> mrs = jwh.GroupBy(g => g.emp_id).Select(s => new ManpowerRatioModel()
            {
                emp_id = s.Key,
                user_name = s.FirstOrDefault().user_name,
                job_id = job_id,
                percents = 0,
                hours = s.FirstOrDefault().total
            }).ToList();
            mrs = mrs.OrderByDescending(o => o.hours).ToList();
            return Json(mrs);
        }

        [HttpPost]
        public JsonResult AddJob(string job_string)
        {
            JobModel job = JsonConvert.DeserializeObject<JobModel>(job_string);
            var result = JobService.CreateJob(job);
            if (result == "Success")
            {
                if (job.term_payments.Count > 0)
                {
                    result = JobService.CreateTermPayments(job.term_payments);
                }
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
            string job_id = job.job_id;
            string result = JobService.UpdateJob(job);

            if (result == "Success")
            {
               
                result = JobService.UpdateTermPayments(job_id,job.term_payments);
                
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

        [HttpPost]
        public IActionResult InsertJobOwner(string job_id, string job_department)
        {
            string message = JobOwner.Insert(job_id, job_department);
            return Json(message);
        }
        [HttpDelete]
        public IActionResult DeleteJobOwner(string job_id, string job_department)
        {
            string message = JobOwner.DeleteByJobDepartment(job_id, job_department);
            return Json(message);
        }
        public IActionResult ExportJob()
        {
            List<JobModel> jobs = JobService.GetAllJobs();
            //Download Excel
            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/Template", "jobs.xlsx"));
            var stream = Export.ExportJob(templateFileInfo, jobs);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "jobs_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx");
        }

        public List<WorkingHoursModel> CalculateWorkingHours(List<WorkingDayModel> workings, List<HolidayModel> _holidays)
        {
            List<WorkingHoursModel> monthly = new List<WorkingHoursModel>();

            List<HolidayModel> holidays = _holidays;
            WorkingHoursModel wh = new WorkingHoursModel();
            TimeSpan working_date = new TimeSpan(0, 0, 0);
            List<WorkingDayModel> _wd = workings;

            if (_wd.Count > 0)
            {
                for (int i = 0; i < _wd.Count; i++)
                {
                    string day = _wd[i].date.DayOfWeek.ToString();

                    bool isHoliday = holidays.Where(w => w.date.Date == _wd[i].date.Date).Count() > 0 ? true : false;
                    bool isWeekend = (_wd[i].date.DayOfWeek == DayOfWeek.Saturday || _wd[i].date.DayOfWeek == DayOfWeek.Sunday) ? true : false;
                    bool chk_after_office = false;
                    if (isHoliday)
                    {
                        day = "Holiday";
                    }
                    working_date = new TimeSpan(0, 0, 0);

                    TimeSpan regular = new TimeSpan(0, 0, 0);
                    TimeSpan ot15 = new TimeSpan(0, 0, 0);
                    TimeSpan ot3 = new TimeSpan(0, 0, 0);
                    TimeSpan leave = new TimeSpan(0, 0, 0);
                    for (int j = 0; j < _wd[i].workings.Count; j++)
                    {
                        chk_after_office = false;
                        regular = new TimeSpan(0, 0, 0);
                        ot15 = new TimeSpan(0, 0, 0);
                        ot3 = new TimeSpan(0, 0, 0);
                        leave = new TimeSpan(0, 0, 0);
                        // Check Holiday and Get day
                        if (isHoliday || isWeekend)
                        {
                            if (_wd[i].workings[j].task_name == "Traveling")
                            {
                                ot15 = default(TimeSpan);
                                ot3 = default(TimeSpan);

                                if (_wd[i].workings[j].lunch_full)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].lunch_half)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(0, 30, 0);
                                    }
                                }

                                if (_wd[i].workings[j].dinner_full)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].dinner_half)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(0, 30, 0);
                                    }
                                }

                                wh = new WorkingHoursModel()
                                {
                                    working_date = _wd[i].date,
                                    job_id = _wd[i].workings[j].job_id,
                                    job_name = _wd[i].workings[j].job_name,
                                    task_id = _wd[i].workings[j].task_id,
                                    task_name = _wd[i].workings[j].task_name,
                                    start_time = _wd[i].workings[j].start_time,
                                    stop_time = _wd[i].workings[j].stop_time,
                                    lunch_full = _wd[i].workings[j].lunch_full,
                                    lunch_half = _wd[i].workings[j].lunch_half,
                                    dinner_full = _wd[i].workings[j].dinner_full,
                                    dinner_half = _wd[i].workings[j].dinner_half,
                                    day = day,
                                    normal = regular,
                                    ot1_5 = ot15,
                                    ot3_0 = ot3,
                                    leave = leave
                                };
                                monthly.Add(wh);
                                regular = new TimeSpan(0, 0, 0);
                                continue;
                            }
                            else
                            {
                                if (_wd[i].workings[j].stop_time == new TimeSpan(23, 59, 0))
                                {
                                    ot15 += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time).Add(new TimeSpan(0, 1, 0));
                                }
                                else
                                {
                                    ot15 += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                }

                            }
                            if (_wd[i].workings[j].lunch_full)
                            {
                                if (ot15 != default(TimeSpan))
                                {
                                    ot15 -= new TimeSpan(1, 0, 0);
                                }
                            }
                            if (_wd[i].workings[j].lunch_half)
                            {
                                if (ot15 != default(TimeSpan))
                                {
                                    ot15 -= new TimeSpan(0, 30, 0);
                                }
                            }

                            if (_wd[i].workings[j].dinner_full)
                            {
                                if (ot15 != default(TimeSpan))
                                {
                                    ot15 -= new TimeSpan(1, 0, 0);
                                }
                            }
                            if (_wd[i].workings[j].dinner_half)
                            {
                                if (ot15 != default(TimeSpan))
                                {
                                    ot15 -= new TimeSpan(0, 30, 0);
                                }
                            }


                            if (ot15 >= new TimeSpan(8, 0, 0))
                            {
                                ot3 = ot15 - new TimeSpan(8, 0, 0);
                            }

                            if (ot3 > new TimeSpan(0, 0, 0))
                            {
                                if (j == 0)
                                {
                                    ot15 = new TimeSpan(8, 0, 0);
                                }
                                else
                                {
                                    ot15 = default(TimeSpan);
                                }
                            }

                            //Check Sum OT 1.5
                            TimeSpan sum_ot15 = monthly.Where(w => w.working_date.Date == _wd[i].date.Date && w.task_name != "Traveling").ToList().Aggregate(
                                TimeSpan.Zero, (sum_ot, next_ot) => sum_ot + next_ot.ot1_5) + ot15;
                            if (sum_ot15 > new TimeSpan(8, 0, 0))
                            {
                                ot15 = new TimeSpan(8, 0, 0) - (sum_ot15 - ot15);
                                ot3 = sum_ot15 - new TimeSpan(8, 0, 0);
                            }

                            wh = new WorkingHoursModel()
                            {
                                working_date = _wd[i].date,
                                job_id = _wd[i].workings[j].job_id,
                                job_name = _wd[i].workings[j].job_name,
                                task_id = _wd[i].workings[j].task_id,
                                task_name = _wd[i].workings[j].task_name,
                                start_time = _wd[i].workings[j].start_time,
                                stop_time = _wd[i].workings[j].stop_time,
                                lunch_full = _wd[i].workings[j].lunch_full,
                                lunch_half = _wd[i].workings[j].lunch_half,
                                dinner_full = _wd[i].workings[j].dinner_full,
                                dinner_half = _wd[i].workings[j].dinner_half,
                                day = day,
                                normal = default(TimeSpan),
                                ot1_5 = ot15,
                                ot3_0 = ot3,
                                leave = default(TimeSpan)
                            };
                            monthly.Add(wh);
                        }
                        else
                        {
                            day = _wd[i].workings[j].working_date.DayOfWeek.ToString();
                            if (_wd[i].workings[j].task_name == "Traveling")
                            {
                                regular = (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                ot15 = default(TimeSpan);
                                ot3 = default(TimeSpan);

                                if (_wd[i].workings[j].lunch_full)
                                {
                                    if (regular != default(TimeSpan))
                                    {
                                        regular -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].lunch_half)
                                {
                                    if (regular != default(TimeSpan))
                                    {
                                        regular -= new TimeSpan(0, 30, 0);
                                    }
                                }
                                if (_wd[i].workings[j].dinner_full)
                                {
                                    if (regular != default(TimeSpan))
                                    {
                                        regular -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].dinner_half)
                                {
                                    if (regular != default(TimeSpan))
                                    {
                                        regular -= new TimeSpan(0, 30, 0);
                                    }
                                }

                                wh = new WorkingHoursModel()
                                {
                                    working_date = _wd[i].date,
                                    job_id = _wd[i].workings[j].job_id,
                                    job_name = _wd[i].workings[j].job_name,
                                    task_id = _wd[i].workings[j].task_id,
                                    task_name = _wd[i].workings[j].task_name,
                                    start_time = _wd[i].workings[j].start_time,
                                    stop_time = _wd[i].workings[j].stop_time,
                                    lunch_full = _wd[i].workings[j].lunch_full,
                                    lunch_half = _wd[i].workings[j].lunch_half,
                                    dinner_full = _wd[i].workings[j].dinner_full,
                                    dinner_half = _wd[i].workings[j].dinner_half,
                                    day = day,
                                    normal = regular,
                                    ot1_5 = ot15,
                                    ot3_0 = ot3,
                                    leave = leave
                                };
                                monthly.Add(wh);
                                regular = new TimeSpan(0, 0, 0);
                                continue;
                            }
                            else if (_wd[i].workings[j].task_name == "Leave")
                            {
                                regular = default(TimeSpan);
                                ot15 = default(TimeSpan);
                                ot3 = default(TimeSpan);
                                leave = (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                if (leave > new TimeSpan(8, 0, 0))
                                {
                                    leave = new TimeSpan(8, 0, 0);
                                }

                                wh = new WorkingHoursModel()
                                {
                                    working_date = _wd[i].date,
                                    job_id = _wd[i].workings[j].job_id,
                                    job_name = _wd[i].workings[j].job_name,
                                    task_id = _wd[i].workings[j].task_id,
                                    task_name = _wd[i].workings[j].task_name,
                                    start_time = _wd[i].workings[j].start_time,
                                    stop_time = _wd[i].workings[j].stop_time,
                                    lunch_full = _wd[i].workings[j].lunch_full,
                                    lunch_half = _wd[i].workings[j].lunch_half,
                                    dinner_full = _wd[i].workings[j].dinner_full,
                                    dinner_half = _wd[i].workings[j].dinner_half,
                                    day = day,
                                    normal = regular,
                                    ot1_5 = ot15,
                                    ot3_0 = ot3,
                                    leave = leave
                                };
                                monthly.Add(wh);
                                regular = new TimeSpan(0, 0, 0);
                                continue;
                            }
                            else
                            {
                                if (_wd[i].workings[j].task_id.Contains("O") || _wd[i].workings[j].task_id.Contains("H")) //Office,Home
                                {
                                    if (_wd[i].workings[j].start_time <= new TimeSpan(17, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(17, 30, 0))
                                    {
                                        ot15 += _wd[i].workings[j].stop_time - new TimeSpan(17, 30, 0);
                                        regular = new TimeSpan(17, 30, 0) - _wd[i].workings[j].start_time;
                                        chk_after_office = true;

                                        if (_wd[i].workings[j].dinner_full)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].dinner_half)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(0, 30, 0);
                                            }
                                        }
                                    }
                                    if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time < new TimeSpan(8, 30, 0))
                                    {
                                        ot15 += _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                        regular = new TimeSpan(0, 0, 0);
                                        chk_after_office = true;
                                    }

                                    if (_wd[i].workings[j].start_time > new TimeSpan(17, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(17, 30, 0))
                                    {
                                        ot15 += _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                        regular = new TimeSpan(0, 0, 0);
                                        chk_after_office = true;
                                    }
                                }

                                if (!chk_after_office)
                                {
                                    if (_wd[i].workings[j].stop_time == new TimeSpan(23, 59, 0))
                                    {
                                        regular += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time).Add(new TimeSpan(0, 1, 0));
                                    }
                                    else
                                    {
                                        regular += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                    }
                                }

                            }

                            if (!chk_after_office)
                            {
                                if (_wd[i].workings[j].lunch_full)
                                {
                                    if (regular != default(TimeSpan))
                                    {
                                        regular -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].lunch_half)
                                {
                                    if (regular != default(TimeSpan))
                                    {
                                        regular -= new TimeSpan(0, 30, 0);
                                    }
                                }
                                if (_wd[i].workings[j].dinner_full)
                                {
                                    if (regular != default(TimeSpan))
                                    {
                                        regular -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].dinner_half)
                                {
                                    if (regular != default(TimeSpan))
                                    {
                                        regular -= new TimeSpan(0, 30, 0);
                                    }
                                }
                            }

                            //Check Sum Regular
                            TimeSpan sum_regular = monthly.Where(w => w.working_date.Date == _wd[i].date.Date && w.task_name != "Traveling" && w.task_name != "Leave").ToList().Aggregate(
                                TimeSpan.Zero, (sum_reg, next_reg) => sum_reg + next_reg.normal) + regular;
                            if (sum_regular > new TimeSpan(8, 0, 0))
                            {
                                regular = new TimeSpan(8, 0, 0) - (sum_regular - regular);
                                if (!chk_after_office)
                                {
                                    ot15 = sum_regular - new TimeSpan(8, 0, 0);
                                }
                            }

                            wh = new WorkingHoursModel()
                            {
                                working_date = _wd[i].date,
                                job_id = _wd[i].workings[j].job_id,
                                job_name = _wd[i].workings[j].job_name,
                                task_id = _wd[i].workings[j].task_id,
                                task_name = _wd[i].workings[j].task_name,
                                start_time = _wd[i].workings[j].start_time,
                                stop_time = _wd[i].workings[j].stop_time,
                                lunch_full = _wd[i].workings[j].lunch_full,
                                lunch_half = _wd[i].workings[j].lunch_half,
                                dinner_full = _wd[i].workings[j].dinner_full,
                                dinner_half = _wd[i].workings[j].dinner_half,
                                day = day,
                                normal = regular,
                                ot1_5 = ot15,
                                ot3_0 = ot3,
                                leave = leave
                            };
                            monthly.Add(wh);
                        }
                    }
                }
            }
            return monthly;
        }
    }
}
