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
    public class IdleTimeController : Controller
    {
        readonly IAccessory Accessory;
        readonly IWorkingHours WorkingHours;
        readonly IHoliday Holiday;

        public IdleTimeController()
        {
            this.Accessory = new AccessoryService();
            this.WorkingHours = new WorkingHoursService();
            this.Holiday = new HolidayService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel
                {
                    name = s.name,
                    department = s.department,
                    role = s.role,
                    user_id = s.user_id
                }).FirstOrDefault();
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

        public IActionResult EngineerIdleTimes()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel
                {
                    name = s.name,
                    department = s.department,
                    role = s.role,
                    user_id = s.user_id
                }).FirstOrDefault();
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

        public IActionResult MonthlyIdleTimes()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel
                {
                    name = s.name,
                    department = s.department,
                    role = s.role,
                    user_id = s.user_id
                }).FirstOrDefault();
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
        public List<EngineerIdleTimeModel> GetIdleTimes(DateTime startDate, DateTime stopDate)
        {
            List<WorkingHoursModel> whs = WorkingHours.GetWorkingHours().OrderBy(o => o.user_name).ThenBy(t => t.working_date).ThenBy(t => t.start_time).ToList();
            List<HolidayModel> holidays = Holiday.GetHolidays(startDate.ToString("yyyy"));
            List<EngineerIdleTimeModel> idles = new List<EngineerIdleTimeModel>();
            string[] users = whs.Select(s => s.user_name).Distinct().ToArray();
            for (int i = 0;i<users.Length;i++)
            {
                TimeSpan zeroHour = new TimeSpan(0, 0, 0, 0, 0);
                TimeSpan anHour = new TimeSpan(0, 1, 0, 0, 0);
                TimeSpan eightHours = new TimeSpan(0, 8, 0, 0, 0);
                TimeSpan aDay = new TimeSpan(0, 24, 0, 0, 0);
                TimeSpan idleTime = zeroHour;
                TimeSpan normal = zeroHour;
                TimeSpan overtime = zeroHour;
                TimeSpan businessHours = zeroHour;
                TimeSpan leaveTime = zeroHour;

                for (DateTime date = startDate; date <= stopDate; date = date.AddDays(1))
                {
                    List<WorkingHoursModel> daily = whs.Where(w => w.working_date.ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd") && w.user_name == users[i]).ToList();
                    
                    //Check if Running Date is Holiday or Not
                    bool holiday = holidays.Where(w => w.date.ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd")).Count() > 0 ? true : false;
                    
                    //Check if Running Date is Weekend or Not
                    bool workingDay = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)? false : true;

                    //Check Leave
                    WorkingHoursModel leave = whs.Where(w => w.working_date.ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd") && w.user_name == users[i] && w.task_name.Contains("Leave")).FirstOrDefault();
                    if (leave != null)
                    {
                        TimeSpan time = leave.stop_time.Subtract(leave.start_time);
                        if (time.Hours > 8)
                        {
                            leaveTime = leaveTime.Add(eightHours);
                        }
                        else
                        {
                            leaveTime = leaveTime.Add(time);
                        }
                        
                    }
                    //Add 8 Hours to Business Working Hours
                    if (!holiday && workingDay)
                    {
                        businessHours = businessHours.Add(eightHours);
                    }
                    
                    //If no WorkingHours Add 8 Hours to Idle Time
                    if (daily.Count() == 0)
                    {
                        if (!holiday && workingDay) idleTime = idleTime.Add(eightHours);
                        continue;
                    }
                    
                    bool lunch = false;
                    bool dinner = false;
                    TimeSpan hours = zeroHour;
                    
                    for(int j = 0;j<daily.Count;j++)
                    {
                        //Add 24 Hours to Stop Time if Stop Time is Less Than Start Time
                        /*if (daily[j].stop_time < daily[j].start_time)
                        {
                            daily[j].stop_time.Add(aDay);
                        }
                        hours = hours.Add(daily[j].stop_time - daily[j].start_time);*/
                        TimeSpan duration = zeroHour;
                        if(daily[j].stop_time > daily[j].start_time)
                        {
                            duration = daily[j].stop_time - daily[j].start_time;
                        } else
                        {
                            duration = daily[j].start_time - daily[j].stop_time;
                        }
                        hours = hours.Add(duration);
                        lunch = (lunch || daily[j].lunch);
                        dinner = (dinner || daily[j].dinner);
                    }

                    //Minus 1 Hour if Lunch
                    if (lunch)
                    {
                        hours = hours.Subtract(anHour);
                    }

                    //Minus 1 Hour if Dinner
                    if (dinner)
                    {
                        hours = hours.Subtract(anHour);
                    }

                    //Set Hours to 0
                    if (hours < zeroHour)
                    {
                        hours = zeroHour;
                    }

                    //Normal Working Day
                    if(!holiday && workingDay)
                    {
                        TimeSpan remain = zeroHour;
                        //Over Time
                        if(hours > eightHours)
                        {
                            remain = hours - eightHours;                           
                            overtime = overtime.Add(remain);
                        }
                        normal = normal.Add(eightHours);
                    }
                    //Weekend and Holiday
                    else
                    {
                        overtime = overtime.Add(hours);
                    }
                }

                normal = normal.Subtract(leaveTime);

                int hoursNormal = (int)(normal.TotalMinutes / 60);
                int hoursOvertime = (int)(overtime.TotalMinutes / 60);
                int hoursBusiness = (int)(businessHours.TotalMinutes / 60);
                int hoursLeave = (int)(leaveTime.TotalMinutes / 60);

                int hoursIdle = 0;
                int total_Work = hoursNormal + hoursOvertime + hoursLeave;
                if (total_Work < hoursBusiness)
                {
                    hoursIdle = (int)(hoursBusiness - total_Work);
                }
                EngineerIdleTimeModel idle = new EngineerIdleTimeModel()
                {
                    userName = users[i],
                    workingHours = hoursBusiness,
                    idle = hoursIdle,
                    normal = hoursNormal,
                    overtime = hoursOvertime,
                    leave = hoursLeave
                };
                idles.Add(idle);
            }
            return idles;
        }
    }
}
