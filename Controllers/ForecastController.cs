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
        readonly ISummaryJobInHand SummaryJobInHand;
        public ForecastController()
        {
            Accessory = new AccessoryService();
            Employees = new CTLServices.EmployeeService();
            Forecast = new ForecastService();
            SummaryJobInHand = new SummaryJobInHandService();
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
            List<string> deps = new List<string>();
            if (department == "ALL")
            {
                deps = new List<string>()
                {
                    "CES-System",
                    "CES-Exp",
                    "CES-PMD",
                    "CES-QIR",
                    "CES-CIS",
                    "AES"
                };
            }
            if (department == "CES")
            {
                deps = new List<string>()
                {
                    "CES-System",
                    "CES-Exp",
                    "CES-PMD",
                    "CES-QIR"
                };
            }

            if (department == "CIS")
            {
                deps = new List<string>()
                {
                    "CES-CIS"
                };
            }
            if (department == "AES")
            {
                deps = new List<string>()
                {
                    "AES"
                };
            }

            double job_in_hand = 0;

            List<InvoicesModel> invoices = Forecast.GetInvoice(year);
            if (mode == "Department")
            {
                if (department == "ALL")
                {
                    forecasts = forecasts.Where(w => deps.Contains(w.department)).ToList();

                    invoices = invoices.Where(w => w.job_in_hand > 0).ToList();

                    List<JobInhandModel> j = SummaryJobInHand.GetsJobInhand(year);
                    job_in_hand = j.Sum(s => s.job_in_hand);

                    List<JobInhandModel> jobs = SummaryJobInHand.GetsJobBackLog(year);
                    backlog = jobs.Sum(s => s.remaining_amount);

                    //List<JobInhandModel> jobs_next = SummaryJobInHand.GetsJobBackLog(year + 1);
                    //backlog_next_year = jobs_next.Sum(s => s.remaining_amount);
                }
                else
                {
                    forecasts = forecasts.Where(w => deps.Contains(w.department)).ToList();
                    if (department == "CES")
                    {
                        invoices = invoices.Where(w => w.job_eng_in_hand > 0 && deps.Contains(w.department)).ToList();
                        List<JobENGInhandModel> jobs = SummaryJobInHand.GetsENGJobBackLog(year);
                        //List<JobENGInhandModel> jobs_next = SummaryJobInHand.GetsENGJobBackLog(year + 1);
                        List<JobENGInhandModel> j = SummaryJobInHand.GetsENGJobInhand(year);
                        job_in_hand = j.Sum(s => s.job_eng_in_hand);
                        backlog = jobs.Where(w => w.job_eng_in_hand > 0).Sum(s => s.remaining_amount);
                        //backlog_next_year = jobs_next.Where(w => w.job_eng_in_hand > 0).Sum(s => s.remaining_amount);
                    }
                    if (department == "CIS")
                    {
                        invoices = invoices.Where(w => w.job_cis_in_hand > 0 && deps.Contains(w.department)).ToList();
                        List<JobCISInhandModel> jobs = SummaryJobInHand.GetsCISJobBackLog(year);
                        //List<JobCISInhandModel> jobs_next = SummaryJobInHand.GetsCISJobBackLog(year + 1);
                        List<JobCISInhandModel> j = SummaryJobInHand.GetsCISJobInhand(year);
                        job_in_hand = j.Sum(s => s.job_cis_in_hand);
                        backlog = jobs.Where(w => w.job_cis_in_hand > 0).Sum(s => s.remaining_amount);
                        //backlog_next_year = jobs_next.Where(w => w.job_cis_in_hand > 0).Sum(s => s.remaining_amount);
                    }
                    if (department == "AES")
                    {
                        invoices = invoices.Where(w => w.job_ais_in_hand > 0 && deps.Contains(w.department)).ToList();
                        List<JobAISInhandModel> jobs = SummaryJobInHand.GetsAISJobBackLog(year);
                        //List<JobAISInhandModel> jobs_next = SummaryJobInHand.GetsAISJobBackLog(year + 1);
                        List<JobAISInhandModel> j = SummaryJobInHand.GetsAISJobInhand(year);
                        job_in_hand = j.Sum(s => s.job_ais_in_hand);
                        backlog = jobs.Where(w => w.job_ais_in_hand > 0).Sum(s => s.remaining_amount);
                        //backlog_next_year = jobs_next.Where(w => w.job_ais_in_hand > 0).Sum(s => s.remaining_amount);
                    }
                }
            }
            else
            {
                string dep = employees.Where(w => w.name_en.ToLower() == responsible.ToLower()).Select(s => s.department).FirstOrDefault();
                if (dep.Contains("PMD"))
                {
                    invoices = invoices.Where(w => w.job_in_hand > 0 && w.responsible.ToLower() == responsible.ToLower()).ToList();

                    forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();

                    List<JobInhandModel> j = SummaryJobInHand.GetsJobInhand(year);
                    job_in_hand = j.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_in_hand);

                    List<JobInhandModel> jobs = SummaryJobInHand.GetsJobBackLog(year);
                    backlog = jobs.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_amount);

                }
                else
                {
                    if (dep == "CES-System" || dep == "CES-Exp" || dep == "CES-QIR")
                    {
                        invoices = invoices.Where(w => w.job_eng_in_hand > 0 && w.responsible.ToLower() == responsible.ToLower()).ToList();
                        forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();
                        List<JobENGInhandModel> jobs = SummaryJobInHand.GetsENGJobBackLog(year);
                        List<JobENGInhandModel> j = SummaryJobInHand.GetsENGJobInhand(year);
                        job_in_hand = j.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_eng_in_hand);
                        backlog = jobs.Where(w => w.job_eng_in_hand > 0 && w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_amount);
                    }
                    if (dep == "CES-CIS")
                    {
                        invoices = invoices.Where(w => w.job_cis_in_hand > 0 && w.responsible.ToLower() == responsible.ToLower()).ToList();
                        forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();
                        List<JobCISInhandModel> jobs = SummaryJobInHand.GetsCISJobBackLog(year);
                        List<JobCISInhandModel> j = SummaryJobInHand.GetsCISJobInhand(year);
                        job_in_hand = j.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_cis_in_hand);
                        backlog = jobs.Where(w => w.job_cis_in_hand > 0 && w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_amount);
                    }
                    if (dep == "AES")
                    {
                        invoices = invoices.Where(w => w.job_ais_in_hand > 0 && w.responsible.ToLower() == responsible.ToLower()).ToList();
                        forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();
                        List<JobAISInhandModel> jobs = SummaryJobInHand.GetsAISJobBackLog(year);
                        List<JobAISInhandModel> j = SummaryJobInHand.GetsAISJobInhand(year);
                        job_in_hand = j.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_ais_in_hand);
                        backlog = jobs.Where(w => w.job_ais_in_hand > 0 && w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_amount);
                    }
                }
                //List<JobInhandModel> jobs_next = SummaryJobInHand.GetsJobBackLog(year + 1);
                //backlog_next_year = jobs_next.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_amount);
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
            foreach (var f in forecasts)
            {
                int monthIndex = GetMonthIndex(f.forecast_month ?? f.month);

                if (monthIndex >= 0 && monthIndex <= 12)
                {
                    if (f.forecast_amount > 0)
                    {
                        tempForecast[monthIndex + 1] += f.forecast_amount / 1_000_000;
                    }
                }
            }

            var inv = invoices.GroupBy(g => new { month = g.invoice_date.Month })
                .Select(s => new
                {
                    responsible = s.FirstOrDefault().responsible,
                    department = s.FirstOrDefault().department,
                    invoice = s.Sum(x=>x.invoice),
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
                        tempActual[monthIndex + 1] += invoice.invoice / 1_000_000;
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

            job_in_hand = job_in_hand / 1_000_000;


            backlog = backlog / 1_000_000;

            backlog_next_year = (job_in_hand + backlog) - sum_invoice;
            //backlog_next_year = backlog_next_year / 1_100_000;

            var data = new
            {
                forecast = forecast,
                job_in_hand = job_in_hand,
                backlog = backlog,
                sum_invoice = sum_invoice,
                backlog_next_year = backlog_next_year
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
