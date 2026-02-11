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
        readonly IJob JobService;
        readonly IAccessory Accessory;
        readonly IJobStatus JobStatus;
        readonly IStatus Status;
        readonly IExport Export;
        readonly CTLInterfaces.IEmployee Employees;
        protected readonly IHostingEnvironment _hostingEnvironment;
        public JobStatusController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Accessory = new AccessoryService();
            JobStatus = new JobStatusService();
            Status = new EngStatusService();
            Export = new ExportService();
            JobService = new JobService();
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
                        user_id = ConvertUserID(employee.name_en)
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
        public string ConvertUserID(string user)
        {
            string first = user.Split(' ')[0];
            string last = user.Split(' ')[1];
            string name = first.Substring(0, 1).ToUpper() + first.Substring(1, first.Length - 1);
            string lastname = last.Substring(0, 1).ToUpper();
            return name + "." + lastname;
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
            string job_id = job.job_id;
            var result = JobStatus.UpdateJobByProcessSystem(job);

            result = JobService.UpdateTermPayments(job_id,job.term_payments);

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
