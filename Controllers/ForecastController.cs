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
    public class ForecastController : Controller
    {
        readonly IAccessory Accessory;
        readonly CTLInterfaces.IEmployee Employees;
        readonly IForecast Forecast;
        readonly IJob Job;
        public ForecastController()
        {
            Accessory = new AccessoryService();
            Employees = new CTLServices.EmployeeService();
            Forecast = new ForecastService();
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
        public IActionResult GetResponsibles()
        {
            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.Where(w => w.active).ToList();

            List<string> deps = new List<string>();
            deps = new List<string>()
                    {
                        "CES-System",
                        "CES-Exp",
                        "CES-PMD",
                        "CES-QIR",
                        "CES-CIS",
                        "AES"
                    };

            employees = employees.Where(w => deps.Contains(w.department)).ToList();
            employees = employees.OrderBy(o => o.name_en).ToList();
            return Json(employees);
        }

        [HttpGet]
        public IActionResult GetData(int year ,string mode , string department , string responsible)
        {
            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.Where(w => w.active).ToList();

            double backlog = 0;
            double backlog_next_year = 0;
            List<ForecastModel> forecasts = Forecast.GetForecasts(year);

            ForecastPaymentModel forecast = new ForecastPaymentModel();

            List<JobInHandModel> jobInHands = Job.GetJobInHands(year);
            List<BackLogModel> backlogs = Job.GetBackLogs(year);

            double job_in_hand = 0;
            string dep = employees.Where(w => w.name_en.ToLower() == responsible.ToLower()).Select(s => s.department).FirstOrDefault();

            List<InvoicesModel> invoices = Job.GetInvoices(year);
            if (mode == "Department")
            {
                if (department == "ALL")
                {
                    forecasts = forecasts.ToList();
                    invoices = invoices.Where(w => w.job_in_hand > 0).ToList();                 
                    job_in_hand = jobInHands.Sum(s => s.job_in_hand);
                    backlog = backlogs.Sum(s => s.remaining_in_hand);
                }
                else
                {                   
                    if (department == "CES")
                    {
                        invoices = invoices.Where(w => w.department == "CES").ToList();
                        job_in_hand = jobInHands.Where(w=>w.department == "CES").Sum(s => s.job_eng_in_hand);
                        backlog = backlogs.Where(w => w.department == "CES").Sum(s => s.remaining_eng_in_hand);
                        forecasts = forecasts.Where(w => w.department == department).ToList();
                    }
                    if (department == "CIS")
                    {
                        invoices = invoices.Where( w => w.department == "CIS").ToList();
                        job_in_hand = jobInHands.Where(w => w.department == "CIS").Sum(s => s.job_cis_in_hand);
                        backlog = backlogs.Where(w => w.department == "CIS").Sum(s => s.remaining_cis_in_hand);
                        forecasts = forecasts.Where(w => w.department == department).ToList();
                    }
                    if (department == "AES")
                    {
                        invoices = invoices.Where(w => w.department == "AES").ToList();
                        job_in_hand = jobInHands.Where(w => w.department == "AES").Sum(s => s.job_ais_in_hand);
                        backlog = backlogs.Where(w => w.department == "AES").Sum(s => s.remaining_ais_in_hand);
                        forecasts = forecasts.Where(w => w.department == department).ToList();
                    }

                    if (department == "PMD")
                    {
                        invoices = invoices.Where(w => w.responsible_department.Contains("PMD")).ToList();
                        job_in_hand = jobInHands.Where(w => w.responsible_department.Contains("PMD")).Sum(s => s.job_in_hand);
                        backlog = backlogs.Where(w => w.responsible_department.Contains("PMD")).Sum(s => s.remaining_in_hand);
                        forecasts = forecasts.Where(w => w.responsible_department.Contains("PMD")).ToList();
                    }
                }
            }
            else
            {

                invoices = invoices.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();

                forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();

                job_in_hand = jobInHands.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_in_hand);
                backlog = backlogs.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_in_hand);
                //if (dep.Contains("PMD"))
                //{
                //    invoices = invoices.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();

                //    forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();

                //    job_in_hand = jobInHands.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_in_hand);
                //    backlog = backlogs.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_in_hand);

                //}
                //else
                //{
                //    if (dep == "CES-System" || dep == "CES-Exp" || dep == "CES-QIR")
                //    {
                //        invoices = invoices.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();
                //        forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();
                //        job_in_hand = jobInHands.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_eng_in_hand);
                //        backlog = backlogs.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_eng_in_hand);
                //    }
                //    if (dep == "CES-CIS")
                //    {
                //        invoices = invoices.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();
                //        forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();
                //        job_in_hand = jobInHands.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_cis_in_hand);
                //        backlog = backlogs.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_cis_in_hand);
                //    }
                //    if (dep == "AES")
                //    {
                //        invoices = invoices.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();
                //        forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();
                //        job_in_hand = jobInHands.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_ais_in_hand);
                //        backlog = backlogs.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_ais_in_hand);
                //    }
                //}
            }
         
            forecast.month_label = new string[14]
            { $"{year}",
    "JAN", "FEB", "MAR", "APR", "MAY", "JUN",
    "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" , $"{year+1}",
            };

            double[] tempForecast = new double[14];
            double[] tempActual = new double[14];
            tempActual[0] = 0;
            tempForecast[0] = 0;

            //Forecast

            List<ForecastModel> result = new List<ForecastModel>();

            foreach (var f in forecasts)
            {
                int monthIndex = GetMonthIndex(f.forecast_month);
                ForecastModel fore = f;
                if (monthIndex >= 0 && monthIndex <= 12)
                {
                    if (f.forecast_portion_amount > 0)
                    {
                        if (dep.Contains("PMD"))
                        {
                            tempForecast[monthIndex + 1] += f.forecast_portion_amount;
                        }
                        else
                        {
                            tempForecast[monthIndex + 1] += f.forecast_portion_department_amount;
                        }
                    }
                }

                var invoice = invoices.Where(w => w.job_id == f.job_id && w.invoice_month == f.forecast_month && w.milestone == f.milestone).FirstOrDefault();
                if (invoice != null)
                {
                    fore.actual_invoice_date = invoice.invoice_date;
                    fore.total_invoice = invoice.invoice;
                    fore.invoice_department_portion = invoice.portion_invoice;
                }
                result.Add(fore);
            }

            // Invoice 
            foreach (var invoice in invoices)
            {
                var fore = forecasts
                    .FirstOrDefault(f =>
                        f.job_id == invoice.job_id &&
                        f.forecast_month == invoice.invoice_month &&
                        f.milestone == invoice.milestone);

                if (fore != null)
                {
                    var model = new ForecastModel
                    {
                        job_id = invoice.job_id,
                        forecast_month = invoice.invoice_month,
                        milestone = invoice.milestone,
                        job_name = fore.job_name,
                        responsible = fore.responsible,
                        responsible_department = fore.responsible_department,
                        department = fore.department,
                        department_in_hand = fore?.department_in_hand ?? 0,
                        job_in_hand = fore?.job_in_hand ?? 0,
                        forecast_remark = fore.forecast_remark,
                        job_type = fore.job_type,
                        payment_id = fore.payment_id,
                        percent = fore?.percent ?? 0,
                        forecast_portion_amount = fore?.forecast_portion_amount ?? 0,
                        forecast_portion_department_amount = fore?.forecast_portion_department_amount ?? 0,
                        actual_invoice_date = invoice.invoice_date,
                        total_invoice = invoice.invoice,
                        invoice_department_portion = invoice.portion_invoice,
                    };

                    result.Add(model);
                }
                else
                {
                    var model = new ForecastModel
                    {
                        job_id = invoice.job_id,
                        forecast_month = invoice.invoice_month,
                        milestone = invoice.milestone,
                        
                        forecast_portion_amount = fore?.forecast_portion_amount ?? 0,
                        forecast_portion_department_amount = fore?.forecast_portion_department_amount ?? 0,
                        actual_invoice_date = invoice.invoice_date,
                        total_invoice = invoice.invoice,
                        invoice_department_portion = invoice.portion_invoice,
                    };
                    result.Add(model);
                }
                
            }

            result = result.GroupBy(g => new { g.job_id, g.milestone, g.forecast_month, g.total_invoice, g.actual_invoice_date }).Select(s => new ForecastModel()
            {
                job_id = s.Key.job_id,
                job_name = s.FirstOrDefault().job_name,
                responsible= s.FirstOrDefault().responsible,
                responsible_department = s.FirstOrDefault().responsible_department,
                job_in_hand = s.FirstOrDefault().job_in_hand,
                department_in_hand = s.FirstOrDefault().department_in_hand,
                department = s.FirstOrDefault().department,
                milestone = s.Key.milestone,
                forecast_month = s.Key.forecast_month,
                percent = s.FirstOrDefault().percent,
                forecast_portion_amount = s.FirstOrDefault().forecast_portion_amount,
                forecast_portion_department_amount = s.FirstOrDefault().forecast_portion_department_amount,
                invoice_department_portion = s.FirstOrDefault().invoice_department_portion,
                job_type = s.FirstOrDefault().job_type,
                payment_id = s.FirstOrDefault().payment_id,
                total_invoice = s.Key.total_invoice,
                forecast_remark = s.FirstOrDefault().forecast_remark,
                actual_invoice_date = s.Key.actual_invoice_date

            }).ToList();


            result = result.OrderBy(o => o.job_id).ToList();


            var inv = invoices.GroupBy(g => new { month = g.invoice_date.Month })
                .Select(s => new
                {
                    responsible = s.FirstOrDefault().responsible,
                    department = s.FirstOrDefault().department,
                    invoice = dep.Contains("PMD") ? s.Sum(x=>x.invoice) : s.Sum(x => x.portion_invoice),
                    month = s.FirstOrDefault().invoice_date.ToString("yyyy-MM")
                }).ToList();
            inv = inv.OrderBy(o => o.month).ToList();
            foreach(var invoice in inv)
            {
                int monthIndex = GetMonthIndex(invoice.month);

                if (monthIndex >= 0 && monthIndex <= 12)
                {
                    if (invoice.invoice > 0)
                    {
                        tempActual[monthIndex + 1] += invoice.invoice;
                    }
                }
            }

            double accForecast = 0;
            double accActual = 0;

            for (int i = 1; i <= 12; i++)
            {
                forecast.forecast_amount[i] = tempForecast[i];
                forecast.actual_amount[i] = tempActual[i];

                accForecast += tempForecast[i];
                accActual += tempActual[i];

                forecast.acc_forecast_amount[i] = accForecast;
                forecast.acc_actual_amount[i] = accActual;

            }
            double sum_invoice = forecast.acc_actual_amount[forecast.acc_actual_amount.Length - 2];
            backlog_next_year = (job_in_hand + backlog) - sum_invoice;

            var data = new
            {
                forecast = forecast,
                job_in_hand = job_in_hand,
                backlog = backlog,
                sum_invoice = sum_invoice,
                backlog_next_year = backlog_next_year,
                result = result
            };
           
            return Json(data);
        }
        private static int GetMonthIndex(string monthStr)
        {
            if (string.IsNullOrEmpty(monthStr)) return -1;

            if (monthStr.Length >= 7 && monthStr.Contains("-"))
            {
                if (DateTime.TryParseExact(monthStr + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    return dt.Month - 1;
                }
            }
            var thaiMonths = new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            for (int i = 0; i < thaiMonths.Length; i++)
            {
                if (monthStr.Contains(thaiMonths[i]))
                    return i;
            }
            if (int.TryParse(monthStr.TrimStart('0'), out int m) && m >= 1 && m <= 12)
            {
                return m - 1;
            }

            return -1;
        }
    }
}
