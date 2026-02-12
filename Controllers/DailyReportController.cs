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
        public JsonResult GetWorkingUser()
        {
            List<UserModel> users = Accessory.getWorkingUser();

            return Json(users);
        }

        [HttpGet]
        public List<DailyActivityModel> GetDailyActivities(string emp_id, string user_name, string month)
        {
            List<DailyActivityModel> drs = DailyReport.GetDailyActivities(emp_id, month);
            Form_DailyReportModel form_model = new Form_DailyReportModel()
            {
                emp_id = emp_id,
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

        public IActionResult FormDailyReport(string emp_id,string user_name, string month)
        {
            List<DailyActivityModel> drs = DailyReport.GetDailyActivities(emp_id, month);
            Form_DailyReportModel form_model = new Form_DailyReportModel()
            {
                emp_id = emp_id,
                name = user_name,
                month = month,
                datas = drs
            };

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
