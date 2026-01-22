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

        public string ConvertUserID(string user)
        {
            string first = user.Split(' ')[0];
            string last = user.Split(' ')[1];
            string name = first.Substring(0, 1).ToUpper() + first.Substring(1, first.Length - 1);
            string lastname = last.Substring(0, 1).ToUpper();
            return name + "." + lastname;
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


        [HttpGet]
        public IActionResult GetResponsibles(string department)
        {
            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.Where(w => w.active).ToList();
            if (department != "ALL")
            {
                List<string> deps = new List<string>();
                if (department == "CES")
                {
                    deps = new List<string>()
                    {
                        "ces-system"
                    };
                    
                }
                if (department == "CIS")
                {
                    deps = new List<string>()
                    {
                        "ces-cis"
                    };
                }

                if (department == "AES")
                {
                    deps = new List<string>()
                    {
                        "aes"
                    };
                }
                employees = employees.Where(w => deps.Contains(w.department.ToLower())).ToList();
            }
            return Json(employees);
        }

        [HttpGet]
        public IActionResult GetData(int year ,string department , string responsible)
        {
            double backlog = 0;
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

            //Department
            //forecasts = forecasts.Where(w => deps.Contains(w.department)).ToList();

            //if (responsible != "ALL")
            //{
            //    forecasts = forecasts.Where(w => w.responsible.ToLower().Contains(responsible.ToLower())).ToList();
            //}

            //double job_in_hand = Forecast.GetJonInHand(year,department,responsible);
            double job_in_hand = 0;

            List<InvoicesModel> invoices = Forecast.GetInvoice(year);

            
            double total_invoice = 0;

            if (department == "ALL" && responsible == "ALL")
            {
                forecasts = forecasts.Where(w => deps.Contains(w.department)).ToList();

                total_invoice = invoices.Where(w=>w.job_in_hand > 0).Sum(s => s.invoice);

                List<JobInhandModel> j = SummaryJobInHand.GetsJobInhand(year);
                job_in_hand = j.Sum(s => s.job_in_hand);

                List<JobInhandModel> jobs = SummaryJobInHand.GetsJobBackLog(year);
                backlog = jobs.Sum(s => s.remaining_amount);
            }
            else if (department == "ALL" && responsible != "ALL")
            {
                var inv = invoices.Where(w=>w.responsible.ToLower() == responsible.ToLower()).ToList();

                var ces = inv.Where(w=>w.job_eng_in_hand > 0).Sum(s => s.job_eng_in_hand / s.job_in_hand * s.invoice);
                var cis = inv.Where(w => w.job_cis_in_hand > 0).Sum(s => s.job_cis_in_hand / s.job_in_hand * s.invoice);
                var ais = inv.Where(w => w.job_ais_in_hand > 0).Sum(s => s.job_ais_in_hand / s.job_in_hand * s.invoice);
                total_invoice = ces + cis + ais;
                //if (department == "CES")
                //{
                //    total_invoice = inv.Where(w=>w.job_eng_in_hand > 0).Sum(s => s.job_eng_in_hand / s.job_in_hand * s.invoice);
                //}
                //if (department == "CIS")
                //{
                //    total_invoice = inv.Where(w => w.job_cis_in_hand > 0).Sum(s => s.job_cis_in_hand / s.job_in_hand * s.invoice);
                //}
                //if (department == "AIS")
                //{
                //    total_invoice = inv.Where(w => w.job_ais_in_hand > 0).Sum(s => s.job_ais_in_hand / s.job_in_hand * s.invoice);
                //}

                forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();

                List<JobInhandModel> j = SummaryJobInHand.GetsJobInhand(year);
                job_in_hand = j.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_in_hand);

                List<JobInhandModel> jobs = SummaryJobInHand.GetsJobBackLog(year);
                backlog = jobs.Where(w=>w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_amount);
            }
            else if (department != "ALL" && responsible == "ALL")
            {
                forecasts = forecasts.Where(w => deps.Contains(w.department)).ToList();
                
                //backlog = jobs.Where(w => deps.Contains(w.department)).Sum(s => s.remaining_amount);
                if (department == "CES")
                {
                    List<JobENGInhandModel> jobs = SummaryJobInHand.GetsENGJobBackLog(year);
                    List<JobENGInhandModel> j = SummaryJobInHand.GetsENGJobInhand(year);
                    job_in_hand = j.Sum(s => s.job_eng_in_hand);                   
                    total_invoice = invoices.Where(w => w.job_eng_in_hand > 0).Sum(s => s.job_eng_in_hand / s.job_in_hand * s.invoice);
                    backlog = jobs.Where(w => w.job_eng_in_hand > 0).Sum(s => s.remaining_amount);
                }
                if (department == "CIS")
                {
                    List<JobCISInhandModel> jobs = SummaryJobInHand.GetsCISJobBackLog(year);
                    List<JobCISInhandModel> j = SummaryJobInHand.GetsCISJobInhand(year);
                    job_in_hand = j.Sum(s => s.job_cis_in_hand);                 
                    total_invoice = invoices.Where(w => w.job_cis_in_hand > 0).Sum(s => s.job_cis_in_hand / s.job_in_hand * s.invoice);
                    backlog = jobs.Where(w => w.job_cis_in_hand > 0).Sum(s => s.remaining_amount);
                }
                if (department == "AIS")
                {
                    List<JobAISInhandModel> jobs = SummaryJobInHand.GetsAISJobBackLog(year);
                    List<JobAISInhandModel> j = SummaryJobInHand.GetsAISJobInhand(year);
                    job_in_hand = j.Sum(s => s.job_ais_in_hand);
                    total_invoice = invoices.Where(w => w.job_ais_in_hand > 0).Sum(s => s.job_ais_in_hand / s.job_in_hand * s.invoice);
                    backlog = jobs.Where(w => w.job_ais_in_hand > 0).Sum(s => s.remaining_amount);
                }               
            }
            else if (department != "ALL" && responsible != "ALL")
            {
                forecasts = forecasts.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();

                List<JobInhandModel> jobs = SummaryJobInHand.GetsJobBackLog(year);
                backlog = jobs.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.remaining_amount);
                if (department == "CES")
                {
                    List<JobENGInhandModel> j = SummaryJobInHand.GetsENGJobInhand(year);
                    job_in_hand = j.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_eng_in_hand);

                    //total_invoice = invoices.Where(w => deps.Contains(w.department) && w.responsible.ToLower() == responsible.ToLower() && w.job_eng_in_hand > 0).Sum(s => s.job_eng_in_hand / s.job_in_hand * s.invoice);
                    
                }
                if (department == "CIS")
                {
                    List<JobCISInhandModel> j = SummaryJobInHand.GetsCISJobInhand(year);
                    job_in_hand = j.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_cis_in_hand);

                    //total_invoice = invoices.Where(w => deps.Contains(w.department) && w.responsible.ToLower() == responsible.ToLower() && w.job_cis_in_hand > 0).Sum(s => s.job_cis_in_hand / s.job_in_hand * s.invoice);
                }
                if (department == "AIS")
                {
                    List<JobAISInhandModel> j = SummaryJobInHand.GetsAISJobInhand(year);
                    job_in_hand = j.Where(w => w.responsible.ToLower() == responsible.ToLower()).Sum(s => s.job_ais_in_hand);

                    //total_invoice = invoices.Where(w => deps.Contains(w.department) && w.responsible.ToLower() == responsible.ToLower() && w.job_ais_in_hand > 0).Sum(s => s.job_ais_in_hand / s.job_in_hand * s.invoice);
                }

                var inv = invoices.Where(w => w.responsible.ToLower() == responsible.ToLower()).ToList();

                var ces = inv.Where(w => w.job_eng_in_hand > 0).Sum(s => s.job_eng_in_hand / s.job_in_hand * s.invoice);
                var cis = inv.Where(w => w.job_cis_in_hand > 0).Sum(s => s.job_cis_in_hand / s.job_in_hand * s.invoice);
                var ais = inv.Where(w => w.job_ais_in_hand > 0).Sum(s => s.job_ais_in_hand / s.job_in_hand * s.invoice);
                total_invoice = ces + cis + ais;
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
                int monthIndex = GetMonthIndex(f.forecast_month ?? f.month ?? f.actual_month);

                if (monthIndex >= 1 && monthIndex <= 12)
                {

                    if (f.forecast_amount > 0)
                    {
                        tempForecast[monthIndex + 1] += f.forecast_amount / 1_000_000;
                    }

                    if (f.actual_amount > 0)
                    {
                        tempActual[monthIndex + 1] += f.actual_amount / 1_000_000;
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
            total_invoice = total_invoice / 1_000_000;

            backlog = backlog / 1_000_000;

            double backlog_next_year = (job_in_hand + backlog) - sum_invoice;

            //forecast.actual_amount[forecast.actual_amount.Length - 1] = backlog_next_year / 1_000_000;
           
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
