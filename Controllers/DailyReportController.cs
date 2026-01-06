using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Rotativa.AspNetCore;
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
    public class DailyReportController : Controller
    {
        readonly IAccessory Accessory;
        readonly IDailyReport DailyReport;
        readonly CTLInterfaces.IEmployee Employees;
        public DailyReportController()
        {
            Accessory = new AccessoryService();
            DailyReport = new DailyReportService();
            Employees = new CTLServices.EmployeeService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { 
                    name = s.name, 
                    department = s.department, 
                    role = s.role,
                    user_id = s.user_id,
                    emp_id = s.emp_id
                }).FirstOrDefault();
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Role", u.role);
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
        public JsonResult GetWorkingUser()
        {
            List<UserModel> users = Accessory.getWorkingUser();

            return Json(users);
        }

        [HttpGet]
        public List<DailyActivityModel> GetDailyActivities(string user_name, string month)
        {
            List<DailyActivityModel> drs = DailyReport.GetDailyActivities(user_name, month);
            Form_DailyReportModel form_model = new Form_DailyReportModel()
            {
                name = user_name,
                month = month,
                datas = drs
            };
            drs = drs.OrderBy(o => o.date).ToList();
            return drs;
        }

        [HttpPatch]
        public JsonResult UpdateActivity(string activity_string)
        {
            DailyActivityModel da = JsonConvert.DeserializeObject<DailyActivityModel>(activity_string);
            var result = DailyReport.EditDailyReport(da);
            return Json(result);
        }

        public IActionResult FormDailyReport(string user_name, string month)
        {
            List<DailyActivityModel> drs = DailyReport.GetDailyActivities(user_name, month);
            Form_DailyReportModel form_model = new Form_DailyReportModel()
            {
                name = user_name,
                month = month,
                datas = drs
            };

            //string footer = "" +
            //    "--print-media-type " + 
            //    "--footer-left \"Job No : J99-9999\" " +
            //    "--footer-center \"Page 1 of 1\" " + 
            //    "--footer-right \"Report Date : 08-07-2022\" " +
            //    "--footer-font-size \"14\"";
            var form_dailyreport = new ViewAsPdf("FormDailyReport")
            {
                Model = form_model,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Rotativa.AspNetCore.Options.Margins { Top = 2, Left = 5, Right = 5, Bottom = 2 },
                /*CustomSwitches = footer*/
            };
            return form_dailyreport;
        }
    }
}
