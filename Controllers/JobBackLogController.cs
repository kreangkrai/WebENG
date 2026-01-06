using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class JobBackLogController : Controller
    {
        readonly IAccessory Accessory;
        readonly ISummaryJobInHand SummaryJobInHand;
        readonly CTLInterfaces.IEmployee Employees;
        public JobBackLogController()
        {
            Accessory = new AccessoryService();
            SummaryJobInHand = new SummaryJobInHandService();
            Employees = new CTLServices.EmployeeService();
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

                List<CTLModels.EmployeeModel> emps = Employees.GetEmployees();
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
        public JsonResult GetJobBackLogProject(string department,int year)
        {
            if (department == "ENG")
            {
                List<JobENGInhandModel> jobs = SummaryJobInHand.GetsENGJobBackLog(year);
                jobs = jobs.Where(w => w.job_type == "Project").ToList();
                return Json(jobs);
            }
            if (department == "CIS")
            {
                List<JobCISInhandModel> jobs = SummaryJobInHand.GetsCISJobBackLog(year);
                jobs = jobs.Where(w => w.job_type == "Project").ToList();
                return Json(jobs);
            }
            if (department == "AIS")
            {
                List<JobAISInhandModel> jobs = SummaryJobInHand.GetsAISJobBackLog(year);
                jobs = jobs.Where(w => w.job_type == "Project").ToList();
                return Json(jobs);
            }
            return Json(null);
        }

        [HttpGet]
        public JsonResult GetJobBackLogService(string department,int year)
        {
            if (department == "ENG")
            {
                List<JobENGInhandModel> jobs = SummaryJobInHand.GetsENGJobBackLog(year);
                jobs = jobs.Where(w => w.job_type == "Service").ToList();
                return Json(jobs);
            }
            if (department == "CIS")
            {
                List<JobCISInhandModel> jobs = SummaryJobInHand.GetsCISJobBackLog(year);
                jobs = jobs.Where(w => w.job_type == "Service").ToList();
                return Json(jobs);
            }
            if (department == "AIS")
            {
                List<JobAISInhandModel> jobs = SummaryJobInHand.GetsAISJobBackLog(year);
                jobs = jobs.Where(w => w.job_type == "Service").ToList();
                return Json(jobs);
            }
            return Json(null);
        }
    }
}
