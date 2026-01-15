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
        public ForecastController()
        {
            Accessory = new AccessoryService();
            Employees = new CTLServices.EmployeeService();
            Forecast = new ForecastService();
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

                //ForecastPaymentModel forecast = GetData(2025);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public IActionResult GetData(int year)
        {
            List<ForecastModel> forecasts = Forecast.GetForecasts(year);

            ForecastPaymentModel forecast = new ForecastPaymentModel();
            double job_in_hand = Forecast.GetJonInHand(year);
            double backlog = Forecast.GetBacklog(year);
            double backlog_next_year = 50;
             
            forecast.month_label = new string[14]
            { $"{year}",
    "JAN", "FEB", "MAR", "APR", "MAY", "JUN",
    "JUL", "AUG", "SET", "OCT", "NOV", "DEC" , $"{year+1}",
            };

            double[] tempForecast = new double[14];
            double[] tempActual = new double[14];
            tempActual[0] = backlog / 1_000_000;
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

            for (int i = 0; i <= 12; i++)
            {
                forecast.forecast_amount[i] = tempForecast[i];
                forecast.actual_amount[i] = tempActual[i];

                if (i != 0)
                {
                    accForecast += tempForecast[i];
                    accActual += tempActual[i];

                    forecast.acc_forecast_amount[i] = accForecast;
                    forecast.acc_actual_amount[i] = accActual;
                }
            }
            forecast.actual_amount[forecast.actual_amount.Length - 1] = backlog_next_year;
            double sum_invoice = forecast.acc_actual_amount[forecast.acc_actual_amount.Length - 2];
            var data = new { forecast = forecast, job_in_hand = job_in_hand / 1_000_000 , sum_invoice = sum_invoice };

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
            var thaiMonths = new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SET", "OCT", "NOV", "DEC" };
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
