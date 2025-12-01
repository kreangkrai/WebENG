using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebENG.CTLInterfaces;
using WebENG.CTLServices;
using WebENG.Interface;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;
using WebENG.LeaveServices;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class StatusLeaveController : Controller
    {
        readonly IAccessory Accessory;
        readonly IEmployee Employees;
        private ILeaveType LeaveType;
        private IRequest Requests;
        private ILeave Leave;
        private ILevel Level;
        private IRequestLog RequestLog;
        public StatusLeaveController()
        {
            Accessory = new AccessoryService();
            Employees = new EmployeeService();
            LeaveType = new LeaveTypeService();
            Requests = new RequestService();
            Leave = new LeaveService();
            Level = new LevelService();
            RequestLog = new RequestLogService();
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
                    user_id = s.user_id,
                    emp_id = s.emp_id
                }).FirstOrDefault();
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);

                List<int> years = new List<int>();
                for(int y = DateTime.Now.AddYears(1).Year;y > DateTime.Now.AddYears(-5).Year; y--)
                {
                    years.Add(y);
                }
                ViewBag.listYears = years;

                List<LevelModel> levels = Level.GetLevelByEmpID(u.emp_id);
                ViewBag.levels = levels;
                ViewBag.emp = u.emp_id;
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }


        [HttpGet]
        public IActionResult GetEmployee(string start, string stop)
        {
            List<RequestModel> requests = Requests.GetRequestByDurationDay(start,stop);
            List<string> emps = requests.GroupBy(g => g.emp_id).Select(s => s.FirstOrDefault().emp_id).ToList();

            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.Where(w => emps.Contains(w.emp_id)).OrderBy(o => o.name_en).ToList();
            List<string> departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).OrderBy(o => o).ToList();
            var data = new { employees = employees, departments = departments };
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetLeaveUsedByEmpID(string emp_id , int year)
        {
            List<UsedLeaveModel> useds = new List<UsedLeaveModel>();
            List<string> status_pending = new List<string>()
            {
                "Created",
                "Pending",
                "Resubmit",
                "Approved",
                "Returned",
                "Completed"
            };
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            List<CTLModels.EmployeeModel> emps = Employees.GetEmployees();
            List<RequestModel> requests = Requests.GetRequestByEmpID(emp_id);
           
            for (int i = 0; i < leaves.Count; i++)
            {
                List<RequestModel>  _requests = requests.Where(w => w.leave_type_code == leaves[i].leave_type_code &&
                w.start_request_date.Year == year &&
                status_pending.Contains(w.status_request)).ToList();

                CTLModels.EmployeeModel emp = new CTLModels.EmployeeModel();
                var em = emps.Where(w => w.emp_id == emp_id).FirstOrDefault();
                emp = new CTLModels.EmployeeModel()
                {
                    emp_id = emp_id,
                    position = em.position,
                    start_date = em.start_date,
                    promote_manager_date = em.promote_manager_date,
                    gender = em.gender
                };
                if (leaves[i].calculate_auto == true)
                {
                    double _leave = Leave.CalculateLeaveDays(emp, year, 6, 10, 10, 12);
                    leaves[i].amount_entitlement = (decimal)_leave;
                }
                double used_leave = 0;
                for (int j = 0; j < _requests.Count; j++)
                {
                    if (_requests[j].is_full_day)
                    {
                        used_leave += _requests[j].amount_leave_day;
                    }
                    else
                    {
                        used_leave += Math.Round(((double)_requests[j].amount_leave_hour) / 8.0, 2);
                    }
                }
                //decimal b = (decimal)((double)leaves[i].amount_entitlement - used_leave);

                UsedLeaveModel used = new UsedLeaveModel()
                {
                    leave_type_id = leaves[i].leave_type_id,
                    leave_type_code = leaves[i].leave_type_code,
                    leave_name_en = leaves[i].leave_name_en,
                    leave_name_th =leaves[i].leave_name_th,
                    amount_entitlement = leaves[i].amount_entitlement,
                    used = (decimal)used_leave,
                    priority = leaves[i].priority
                };
                useds = useds.GroupBy(g => g.leave_type_code).Select(s => new UsedLeaveModel()
                {
                    leave_type_code = s.Key,
                    leave_type_id = s.FirstOrDefault().leave_type_id,
                    leave_name_en = s.FirstOrDefault().leave_name_en,
                    leave_name_th= s.FirstOrDefault().leave_name_th.Split(' ')[0],
                    amount_entitlement = s.FirstOrDefault().amount_entitlement,
                    used = s.FirstOrDefault().used,
                    priority = s.FirstOrDefault().priority

                }).ToList();
                useds.Add(used);

            }
            useds = useds.OrderBy(o => o.priority).ThenBy(t => t.leave_name_th).ToList();
            return Json(useds);
        }

        [HttpGet]
        public IActionResult GetRequestHistory(string emp_id,string start , string stop)
        {
            DateTime _start = DateTime.Parse(start);
            DateTime _stop = DateTime.Parse(stop);
            List<RequestModel> requests = Requests.GetRequestByEmpID(emp_id);
            requests = requests.Where(w => w.start_request_date.Date >= _start.Date && w.start_request_date.Date <= _stop).ToList();
            requests = requests.OrderBy(o => o.start_request_date).ToList();
            return Json(requests);
        }

        [HttpGet]
        public IActionResult GetRequestLog(string request_id)
        {
            List<RequestLogModel> requests = RequestLog.GetLogByRequestId(request_id);
            return Json(requests);
        }

        [HttpPost]
        public IActionResult EditRequest(string request_id, string str, string[] tempFileIds)
        {
            DateTime now = DateTime.Now;
            RequestModel request = JsonConvert.DeserializeObject<RequestModel>(str);
            request.request_date = now;
            request.request_id = request_id;
            request.status_request = "Resubmit";
            request.comment = "";
            request.amount_leave_hour = Math.Round((decimal)(request.end_request_time - request.start_request_time).TotalHours, 0);

            // Check Two Step Approve
            bool is_full_day = request.is_full_day;
            if (is_full_day)
            {
                LeaveTypeModel leaveType = LeaveType.GetLeaveTypeByID(request.leave_type_id);
                decimal over_consecutive_days_for_two_step = leaveType.over_consecutive_days_for_two_step;
                double diff_day = request.amount_leave_day;
                if (diff_day >= (double)over_consecutive_days_for_two_step)
                {
                    request.is_two_step_approve = true;
                }
                else
                {
                    request.is_two_step_approve = false;
                }
            }
            else
            {
                request.is_two_step_approve = false;
            }

            if (tempFileIds.Length > 0) // มีไฟล์แนบมา
            {
                request.path_file = request_id;
            }
            else
            {
                request.path_file = "";
            }
            List<LevelModel> level = Level.GetLevelByEmpID(request.emp_id);
            level = level.Where(w => w.emp_id == request.emp_id).ToList();

            request.level_step = level.FirstOrDefault().level;

            List<RequestModel> requests = Requests.GetRequestByEmpID(request.emp_id);
            string message = "";
            //if (!requests.Any(a => a.start_request_date.Date == request.start_request_date.Date)) //  Check Date
            {
                var connect = new ConnectSQL();
                using (SqlConnection con = connect.OpenLeaveConnect())
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            var requestService = Requests;
                            var requestLogService = RequestLog;
                            requestService.Update(request);

                            RequestLogModel last_request_log = RequestLog.GetLogByRequestId(request_id).LastOrDefault();

                            RequestLogModel requestLog = new RequestLogModel()
                            {
                                action_by = request.emp_id,
                                action_by_name = level.FirstOrDefault().emp_name,
                                action_by_level = level.FirstOrDefault().level,
                                old_status = last_request_log.new_status,
                                new_status = "Canceled",
                                comment = "",
                                old_level_step = last_request_log.new_level_step,
                                new_level_step = level.FirstOrDefault().level,
                                request_id = request.request_id,
                                log_date = DateTime.Now
                            };
                            requestLogService.Insert(requestLog);

                            RequestLogModel requestLog_ = new RequestLogModel()
                            {
                                action_by = request.emp_id,
                                action_by_name = level.FirstOrDefault().emp_name,
                                action_by_level = level.FirstOrDefault().level,
                                old_status = "Canceled",
                                new_status = "Resubmit",
                                comment = "",
                                old_level_step = -1,
                                new_level_step = level.FirstOrDefault().level,
                                request_id = request.request_id,
                                log_date = DateTime.Now
                            };
                            requestLogService.Insert(requestLog_);

                            tran.Commit();
                            message = "Success";
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            message = $"Error {ex.Message}";
                        }
                    }
                }

                if (message == "Success")
                {
                    //if (tempFileIds != null)
                    //{
                    //    foreach (var tempId in tempFileIds)
                    //    {
                    //        var tempFolder = Path.Combine("Uploads", "temp", tempId);
                    //        var files = Directory.GetFiles(tempFolder);
                    //        foreach (var oldFile in files)
                    //        {
                    //            var fileName = Path.GetFileName(oldFile);
                    //            var newPath = Path.Combine(
                    //                "Uploads", request.leave_type_id,
                    //                now.Year.ToString(),
                    //                request_id,
                    //                fileName
                    //            );
                    //            Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    //            System.IO.File.Move(oldFile, newPath);
                    //        }
                    //        Directory.Delete(tempFolder, true);
                    //    }
                    //}

                    ////Insert Leave Working Hours
                    //List<CTLModels.HolidayModel> holidays = Holiday.GetHolidays(request.start_request_date.Year.ToString());
                    //List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
                    //List<UserModel> users = Accessory.getAllUser();
                    //for(DateTime date = request.start_request_date; date <= request.end_request_date; date = date.AddDays(1))
                    //{
                    //    if (holidays.Any(a => a.date.Date != date.Date) && date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    //    {
                    //        if (request.is_full_day)
                    //        {
                    //            WorkingHoursModel wh = new WorkingHoursModel()
                    //            {
                    //                user_id = users.Where(w=>w.emp_id == request.emp_id).Select(s=>s.user_id).FirstOrDefault(),
                    //                working_date = date.Date,
                    //                job_id = "J999999",
                    //                task_id = "T002",
                    //                start_time = new TimeSpan(8, 30, 0),
                    //                stop_time = new TimeSpan(17, 30, 0),
                    //                lunch_full = false,
                    //                lunch_half = false,
                    //                dinner_full = false,
                    //                dinner_half = false,
                    //                note = "",
                    //            };
                    //            whs.Add(wh);
                    //        }
                    //        else
                    //        {
                    //            WorkingHoursModel wh = new WorkingHoursModel()
                    //            {
                    //                user_id = users.Where(w => w.emp_id == request.emp_id).Select(s => s.user_id).FirstOrDefault(),
                    //                working_date = date.Date,
                    //                job_id = "J999999",
                    //                task_id = "T002",
                    //                start_time = request.start_request_time,
                    //                stop_time = request.end_request_time,
                    //                lunch_full = false,
                    //                lunch_half = false,
                    //                dinner_full = false,
                    //                dinner_half = false,
                    //                note = "",
                    //            };
                    //            whs.Add(wh);
                    //        }
                    //    }
                    //}
                    //message = AddWorkingHours(whs);
                }
                return Json(message);
            }
            //else
            //{
            //    return Json("ใช้สิทธิ์วันลาไปแล้ว");
            //}
        }

        [HttpDelete]
        public IActionResult DeleteRequest(string request_id, string emp_id)
        {
            List<LevelModel> level = Level.GetLevelByEmpID(emp_id);
            level = level.Where(w => w.emp_id == emp_id).ToList();

            RequestModel request = Requests.GetRequestByID(request_id);
            request.status_request = "Canceled";
            request.comment = "";
            request.end_request_time = new TimeSpan(request.end_request_time.Hours, request.end_request_time.Minutes, request.end_request_time.Seconds);
            request.start_request_time = new TimeSpan(request.start_request_time.Hours, request.start_request_time.Minutes, request.start_request_time.Seconds);
            string message = "";

            var connect = new ConnectSQL();
            using (SqlConnection con = connect.OpenLeaveConnect())
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        var requestService = Requests;
                        var requestLogService = RequestLog;
                        requestService.Update(request);

                        RequestLogModel last_request_log = RequestLog.GetLogByRequestId(request_id).LastOrDefault();

                        RequestLogModel requestLog = new RequestLogModel()
                        {
                            action_by = request.emp_id,
                            action_by_name = level.FirstOrDefault().emp_name,
                            action_by_level = level.FirstOrDefault().level,
                            old_status = last_request_log.new_status,
                            new_status = "Canceled",
                            comment = "",
                            old_level_step = last_request_log.new_level_step,
                            new_level_step = level.FirstOrDefault().level,
                            request_id = request.request_id,
                            log_date = DateTime.Now
                        };
                        requestLogService.Insert(requestLog);

                        tran.Commit();
                        message = "Success";
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        message = $"Error {ex.Message}";
                    }
                }
            }
            return Json(message);
        }
    }
}
