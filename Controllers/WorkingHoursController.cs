using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Service;
using Microsoft.AspNetCore.Http;
using WebENG.Models;
using Newtonsoft.Json;
using Rotativa.AspNetCore;

namespace WebENG.Controllers
{
    public class WorkingHoursController : Controller
    {
        readonly IAccessory Accessory;
        readonly IWorkingHours WorkingHoursService;
        readonly IHoliday HolidayService;

        static List<WorkingHoursModel> monthly = new List<WorkingHoursModel>();
        static string form_employee_name;
        static string form_department;
        static string form_phone_number;
        static string form_start_time;
        static string form_month;
        static Form_OvertimeModel form_data;
        
        public WorkingHoursController()
        {
            Accessory = new AccessoryService();
            WorkingHoursService = new WorkingHoursService();
            HolidayService = new HolidayService();
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
                    user_id = s.user_id}).FirstOrDefault();
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        
        [HttpGet]
        public string GetLastWorkingHoursID()
        {
            int id = WorkingHoursService.GetLastWorkingHoursID() + 1;
            string wh = "WH" + id.ToString().PadLeft(6, '0');
            return wh;
        }

        [HttpGet]
        public JsonResult GetWorkingHours(string user_name, string month)
        {
            monthly = new List<WorkingHoursModel>();
            form_employee_name = user_name;
            form_department = "";
            form_phone_number = "";
            form_start_time = "";
            form_month = month;

            int yy = Convert.ToInt32(month.Split("-")[0]);
            int mm = Convert.ToInt32(month.Split("-")[1]);

            List<WorkingHoursModel> whs = WorkingHoursService.GetWorkingHours(yy.ToString(), mm.ToString().PadLeft(2,'0'), user_name);
            List<HolidayModel> holidays = HolidayService.GetHolidays(yy.ToString());
            form_department = whs.Select(s => s.department).FirstOrDefault();

            whs = whs.OrderBy(o => o.working_date).ToList();
            int days = DateTime.DaysInMonth(yy, mm);
            for(int i = 0;i < days; i++)
            {
                DateTime date = new DateTime(yy, mm, i + 1);
                List<WorkingHoursModel> whd = whs.Where(w => w.working_date == date).ToList();
                if (whd.Count > 0)
                {
                    TimeSpan substraction = new TimeSpan(8, 0, 0);
                    TimeSpan anHour = new TimeSpan(1, 0, 0);
                    TimeSpan noon = new TimeSpan(12, 0, 0);
                    TimeSpan after_noon = new TimeSpan(13, 0, 0);

                    TimeSpan evening = new TimeSpan(17, 30, 0);
                    TimeSpan end_evening = new TimeSpan(18, 30, 0);


                    bool isHoliday = holidays.Where(w => w.date == whd[0].working_date).Count() > 0 ? true : false;
                    bool isWeekend = (whd[0].working_date.DayOfWeek == DayOfWeek.Saturday || whd[0].working_date.DayOfWeek == DayOfWeek.Sunday) ? true : false;
                    whd.OrderBy(o => o.start_time);
                    for (int j = 0; j < whd.Count; j++)
                    {
                        TimeSpan regular = new TimeSpan();
                        TimeSpan ot15 = new TimeSpan();
                        TimeSpan ot3 = new TimeSpan();
                        WorkingHoursModel wh = new WorkingHoursModel();
                        wh.working_date = whd[j].working_date;
                        wh.job_id = whd[j].job_id;
                        wh.job_name = whd[j].job_name;
                        wh.task_id = whd[j].task_id;
                        wh.task_name = whd[j].task_name;
                        wh.start_time = whd[j].start_time;
                        wh.stop_time = whd[j].stop_time;
                        wh.lunch = whd[j].lunch;
                        wh.dinner = whd[j].dinner;

                        if (isHoliday || isWeekend)
                        {
                            if (wh.stop_time == new TimeSpan(23, 59, 0))
                            {
                                ot15 += (wh.stop_time - wh.start_time).Add(new TimeSpan (0,1,0));
                            }
                            else
                            {
                                ot15 += wh.stop_time - wh.start_time;
                            }
                            
                            if (wh.lunch)
                            {
                                ot15 -= anHour;
                            }

                            if (wh.dinner)
                            {
                                ot15 -= anHour;
                            }

                            /*if (wh.lunch && wh.start_time <= noon && wh.stop_time > after_noon)
                            {
                                ot15 -= after_noon - noon;
                            }

                            if (wh.dinner && wh.start_time <= evening && wh.stop_time > end_evening)
                            {
                                ot15 -= end_evening - evening;
                            }*/
                        }
                        else
                        {
                            //Check Start and Stop Time to calculate hours
                            if (wh.start_time < evening && wh.stop_time > evening)
                            {
                                //Start before 17.30 and stop after 17.30
                                regular += evening - wh.start_time;

                                //Check if task is not equal to travel
                                if (wh.task_id[0] != 'T')
                                {
                                    //Add hours to overtime 1.5 if task is not travel
                                    if (wh.stop_time == new TimeSpan(23, 59, 0))
                                    {
                                        ot15 += (wh.stop_time - evening).Add(new TimeSpan(0, 1, 0));
                                    }
                                    else
                                    {
                                        ot15 += wh.stop_time - evening;
                                    }
                                }
                                else
                                {
                                    //Add hours to regular if task is travel
                                    if (wh.stop_time == new TimeSpan(23, 59, 0))
                                    {
                                        regular += (wh.stop_time - evening).Add(new TimeSpan(0, 1, 0));
                                    }
                                    else
                                    {
                                        regular += wh.stop_time - evening;
                                    }
                                }

                            }
                            else if (wh.start_time < evening && wh.stop_time <= evening)
                            {
                                //Start before 17.30 and stop before 17.30
                                if (wh.stop_time == new TimeSpan(23, 59, 0))
                                {
                                    regular += (wh.stop_time - wh.start_time).Add(new TimeSpan(0, 1, 0));
                                }
                                else
                                {
                                    regular += wh.stop_time - wh.start_time;
                                }
                            }
                            else
                            {
                                //Start and stop after 17.30
                                //Check if task is not equal to travel
                                if (wh.task_id[0] != 'T')
                                {
                                    //Add hours to overtime 1.5 if task is not travel        

                                    if (wh.stop_time == new TimeSpan(23, 59, 0))
                                    {
                                        ot15 += (wh.stop_time - wh.start_time).Add(new TimeSpan(0, 1, 0));
                                    }
                                    else
                                    {
                                        ot15 += wh.stop_time - wh.start_time;
                                    }
                                }
                                else
                                {
                                    //Add hours to regular if task is travel

                                    if (wh.stop_time == new TimeSpan(23, 59, 0))
                                    {
                                        regular += (wh.stop_time - wh.start_time).Add(new TimeSpan(0, 1, 0));
                                    }
                                    else
                                    {
                                        regular += wh.stop_time - wh.start_time;
                                    }
                                }
                            }

                            if (wh.lunch && wh.start_time <= noon && wh.stop_time > after_noon && regular.Hours > 1)
                            {
                                regular -= after_noon - noon;
                            }

                            if (wh.dinner && wh.start_time <= evening && wh.stop_time > end_evening && ot15.Hours > 1)
                            {
                                ot15 -= end_evening - evening;
                            }
                        }

                        if ((ot15 > substraction) && (isHoliday || isWeekend))
                        {
                            ot3 += ot15 - substraction;
                            ot15 -= ot15 - substraction;
                        }

                        List<WorkingHoursModel> lastDays = monthly.Where(w => w.working_date.Date == date.Date).ToList();
                        TimeSpan lastOT15 = TimeSpan.Zero;
                        TimeSpan lastNormal = TimeSpan.Zero;
                        for (int k = 0; k < lastDays.Count; k++)
                        {
                            lastOT15 += lastDays[k].ot1_5;
                            lastNormal += lastDays[k].normal;
                        }

                        bool isTaskT = wh.task_id[0] == 'T';
                        if (!isTaskT)
                        {
                            wh.normal = TimeSpan.Zero;
                            TimeSpan remain_ot15 = TimeSpan.Zero;
                            remain_ot15 = substraction - lastOT15;

                            if (isWeekend || isHoliday)
                            {
                                if (lastOT15.TotalHours >= 8)
                                {
                                    wh.ot1_5 = TimeSpan.Zero;
                                    wh.ot3_0 = ot15 - remain_ot15;
                                }
                                else
                                {
                                    if (remain_ot15.TotalHours >= 8)
                                    {
                                        wh.ot1_5 = ot15;
                                        wh.ot3_0 = ot3;
                                    }
                                    else
                                    {
                                        wh.ot1_5 = remain_ot15;
                                        wh.ot3_0 = ot3;
                                    }
                                }
                            }
                            else
                            {
                                if (lastNormal.TotalHours >= 8)
                                {
                                    if (remain_ot15 < regular)
                                    {
                                        wh.ot1_5 = remain_ot15;
                                        wh.ot3_0 = regular - remain_ot15;
                                    }
                                    else
                                    {
                                        if (remain_ot15.TotalHours >= 8)
                                        {
                                            wh.ot1_5 = ot15;
                                            wh.ot3_0 = TimeSpan.Zero;
                                        }
                                        else
                                        {
                                            wh.ot1_5 = remain_ot15;
                                            wh.ot3_0 = TimeSpan.Zero;
                                        }
                                    }
                                }
                                else
                                {
                                    wh.normal = regular;
                                    wh.ot1_5 = ot15;
                                    wh.ot3_0 = ot3;
                                }
                            }
                        }
                        else
                        {
                            if (wh.task_name == "Leave")
                            {
                                wh.normal = TimeSpan.Zero;
                                wh.ot1_5 = TimeSpan.Zero;
                                wh.ot3_0 = TimeSpan.Zero;
                            }
                            if (wh.task_name == "Traveling")
                            {
                                if (regular.TotalHours >= 8)
                                {
                                    wh.normal = new TimeSpan(8, 0, 0);
                                }
                                else
                                {
                                    wh.normal = regular;
                                }
                                
                                wh.ot1_5 = TimeSpan.Zero;
                                wh.ot3_0 = TimeSpan.Zero;
                            }
                        }
                        monthly.Add(wh);
                    }
                }
                else
                {
                    WorkingHoursModel wh = new WorkingHoursModel()
                    {
                        working_date = date,
                        job_id = "",
                        job_name = "",
                        task_id = "",
                        task_name = "",
                        start_time = default(TimeSpan),
                        stop_time = default(TimeSpan),
                        lunch = false,
                        dinner = false,
                        normal = default(TimeSpan),
                        ot1_5 = default(TimeSpan),
                        ot3_0 = default(TimeSpan)
                    };
                    monthly.Add(wh);
                }
            }
            return Json(monthly);
        }

        [HttpGet]
        public JsonResult GetMonthlySummary()
        {
            List<WorkingHoursSummaryModel> whs = new List<WorkingHoursSummaryModel>();
            string[] jobs = monthly.Where(w => w.job_id != "" && w.job_id != "J999999").Select(s => s.job_id).Distinct().ToArray();
            for(int i = 0; i < jobs.Count(); i++)
            {
                WorkingHoursSummaryModel js = new WorkingHoursSummaryModel();
                js.job_id = jobs[i];
                js.job_name = monthly.Where(w => w.job_id == jobs[i]).Select(s => s.job_name).FirstOrDefault();
                js.normal = Convert.ToInt32(monthly.Where(s => s.job_id == jobs[i]).Sum(t => t.normal.TotalMinutes));
                js.ot1_5 = Convert.ToInt32(monthly.Where(s => s.job_id == jobs[i]).Sum(t => t.ot1_5.TotalMinutes));
                js.ot3_0 = Convert.ToInt32(monthly.Where(s => s.job_id == jobs[i]).Sum(t => t.ot3_0.TotalMinutes));
                whs.Add(js);
            }
            return Json(whs);
        }

        [HttpPatch]
        public JsonResult UpdateRestTime(string task_str)
        {
            WorkingHoursModel wh = JsonConvert.DeserializeObject<WorkingHoursModel>(task_str);
            var result = WorkingHoursService.UpdateRestTime(wh);
            return Json(result);
        }

        public IActionResult FormOvertime()
        {
            List<Form_OvertimeDataModel> datas = new List<Form_OvertimeDataModel>();
            List<HolidayModel> holidays = HolidayService.GetHolidays(monthly[0].working_date.Year.ToString());
            TimeSpan sum_normal = default(TimeSpan);
            TimeSpan sum_ot1_5 = default(TimeSpan);
            TimeSpan sum_ot3_0 = default(TimeSpan);
            for(int i = 0; i < monthly.Count(); i++)
            {
                Form_OvertimeDataModel data = new Form_OvertimeDataModel()
                {
                    date = monthly[i].working_date.ToString("dd MMMM yyyy").ToUpper(),
                    day = monthly[i].working_date.DayOfWeek.ToString().ToUpper(),
                    job = monthly[i].job_id,
                    task = monthly[i].task_name,
                    lunch = monthly[i].lunch == true ? "✓" : "",
                    dinner = monthly[i].dinner == true ? "✓" : "",
                    weekend = (monthly[i].working_date.DayOfWeek == DayOfWeek.Saturday || monthly[i].working_date.DayOfWeek == DayOfWeek.Sunday) ? true : false,
                    holiday = (holidays.Where(w=>w.date == monthly[i].working_date).ToList().Count() > 0) ? true : false,
                };

                sum_normal += monthly[i].normal;
                sum_ot1_5 += monthly[i].ot1_5;
                sum_ot3_0 += monthly[i].ot3_0;

                if(monthly[i].job_id == "")
                {
                    data.job = "";
                    data.location = "";
                    data.task = "";
                    data.start_time = "";
                    data.stop_time = "";
                    data.normal = "";
                    data.ot1_5 = "";
                    data.ot3_0 = "";
                }
                else
                {
                    if (monthly[i].task_id.Substring(0, 1) == "O")
                    {
                        data.location = "Office";
                    }
                    if (monthly[i].task_id.Substring(0, 1) == "S")
                    {
                        data.location = "Site";
                    }
                    if (monthly[i].task_id.Substring(0, 1) == "T")
                    {
                        data.location = "Other";
                    }

                    data.start_time = monthly[i].start_time.ToString().Substring(0,5) != "00:00" || monthly[i].stop_time.ToString().Substring(0, 5) != "00:00" ? 
                        monthly[i].start_time.ToString().Substring(0, 5) : "";
                    data.stop_time = monthly[i].stop_time.ToString().Substring(0, 5) != "00:00" || monthly[i].start_time.ToString().Substring(0, 5) != "00:00" ? 
                        monthly[i].stop_time.ToString().Substring(0, 5) : "";
                    data.normal = monthly[i].normal.ToString().Substring(0, 5) != "00:00" ? monthly[i].normal.ToString().Substring(0, 5) : "";
                    data.ot1_5 = monthly[i].ot1_5.ToString().Substring(0, 5) != "00:00" ? monthly[i].ot1_5.ToString().Substring(0, 5) : "";
                    data.ot3_0 = monthly[i].ot3_0.ToString().Substring(0, 5) != "00:00" ? monthly[i].ot3_0.ToString().Substring(0, 5) : "";
                }
                datas.Add(data);
            }

            List<WorkingHoursModel> summaries = new List<WorkingHoursModel>();
            string[] jobs = monthly.Select(s => s.job_id).Where(w => w != "" && w != "J999999").Distinct().ToArray();
            for(int i = 0; i < jobs.Count(); i++)
            {
                WorkingHoursModel summary = new WorkingHoursModel()
                {
                    job_id = jobs[i],
                    normal = new TimeSpan(monthly.Where(w => w.job_id == jobs[i]).Sum(s => s.normal.Ticks)),
                    ot1_5 = new TimeSpan(monthly.Where(w => w.job_id == jobs[i]).Sum(s => s.ot1_5.Ticks)),
                    ot3_0 = new TimeSpan(monthly.Where(w => w.job_id == jobs[i]).Sum(s => s.ot3_0.Ticks)),
                };
                summaries.Add(summary);
            }

            TimeSpan twhs = sum_normal + sum_ot1_5 + sum_ot3_0;

            form_data = new Form_OvertimeModel()
            {
                employee_name = form_employee_name,
                department = form_department,
                phone_number = form_phone_number,
                normal_start_time = form_start_time,
                month = form_month,
                datas = datas,
                summary = summaries,
                total_working_hours = ((int)(twhs.TotalHours)).ToString().PadLeft(2,'0') + ":" + twhs.Minutes.ToString().PadLeft(2,'0'),
                total_normal = ((int)(sum_normal.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_normal.Minutes.ToString().PadLeft(2, '0'),
                total_ot1_5 = ((int)(sum_ot1_5.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_ot1_5.Minutes.ToString().PadLeft(2, '0'),
                total_ot3_0 = ((int)(sum_ot3_0.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_ot3_0.Minutes.ToString().PadLeft(2, '0'),
                hours_normal = ((int)(sum_normal.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_normal.Minutes.ToString().PadLeft(2, '0'),
                hours_1_5 = ((int)(sum_ot1_5.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_ot1_5.Minutes.ToString().PadLeft(2, '0'),
                hours_3_0 = ((int)(sum_ot3_0.TotalHours)).ToString().PadLeft(2, '0') + ":" + sum_ot3_0.Minutes.ToString().PadLeft(2, '0')
            };
            var form_overtime = new ViewAsPdf("FormOvertime")
            {
                Model = form_data,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Rotativa.AspNetCore.Options.Margins { Top = 5, Left = 5, Right = 5, Bottom = 2 }
            };
            return form_overtime;
        }
    }
}
