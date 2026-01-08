using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.CTLInterfaces;
using WebENG.CTLModels;
using WebENG.CTLServices;
using WebENG.Interface;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;
using WebENG.LeaveServices;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class SummaryLeaveController : Controller
    {
        readonly IAccessory Accessory;
        readonly IEmployee Employee;
        readonly IRequest Requests;
        readonly ILeaveType LeaveType;
        readonly CTLInterfaces.IHoliday Holiday;
        public SummaryLeaveController()
        {
            Accessory = new AccessoryService();
            Employee = new EmployeeService();
            Requests = new RequestService();
            LeaveType = new LeaveTypeService();
            Holiday = new CTLServices.HolidayService();
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

                List<CTLModels.HolidayModel> holidays = Holiday.GetHolidays(DateTime.Now.Year.ToString());
                ViewBag.holiday = holidays;

                List<CTLModels.EmployeeModel> emps = Employee.GetEmployees();
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
        public IActionResult GetDapartments()
        {
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            employees = employees.OrderBy(o => o.name_en).ToList();
            List<string> departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).OrderBy(o => o).ToList();
            var data = new { departments = departments };
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetDataByMonth(string department, string start, string end)
        {
            var _startDate = DateTime.Parse(start);
            var startDate = new DateTime(_startDate.Year, _startDate.Month, 1);
            var endDate = DateTime.Parse(end).AddDays(1).AddTicks(-1);

            var allEmps = Employee.GetEmps();
            var allEmployees = Employee.GetEmployees();

            List<CTLModels.HolidayModel> holidays = Holiday.GetHolidays(startDate.Year.ToString());

            Dictionary<string, string> positionDict = new Dictionary<string, string>();
            if (department == "ALL")
            {
                positionDict = allEmployees
                    .ToDictionary(e => e.emp_id, e => e.position);
            }
            else
            {
                positionDict = allEmployees
                    .Where(e => e.department == department)
                    .ToDictionary(e => e.emp_id, e => e.position);
            }
            var deptEmpIds = positionDict.Keys.ToHashSet();

            var empsInDept = allEmps
                .Where(e => deptEmpIds.Contains(e.emp_id))
                .ToList();

            if (!empsInDept.Any())
                return Json(new object[0]);

            var allRequests = Requests.GetRequestByDurationDay(startDate.ToString("yyyy-MM-dd"), end);               

            List<RequestModel> new_requests = new List<RequestModel>();
            for (int i = 0; i < allRequests.Count; i++)
            {
                if (allRequests[i].is_full_day)
                {
                    if (allRequests[i].amount_leave_day > 1)
                    {
                        for (DateTime date = allRequests[i].start_request_date; date <= allRequests[i].end_request_date; date = date.AddDays(1))
                        {
                            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday && !holidays.Any(a => a.date.Date == date.Date))
                            {
                                RequestModel request = new RequestModel()
                                {
                                    emp_id = allRequests[i].emp_id,
                                    leave_name_th = allRequests[i].leave_name_th,
                                    start_request_date = date,
                                    end_request_date = date,
                                    amount_leave_day = 1,
                                    start_request_time = new TimeSpan(8, 30, 0),
                                    end_request_time = new TimeSpan(17, 30, 0),
                                    is_full_day = allRequests[i].is_full_day,
                                    description = allRequests[i].description,
                                    request_date = allRequests[i].request_date,
                                    path_file = allRequests[i].path_file,
                                    amount_leave_hour = allRequests[i].amount_leave_hour,
                                    color_code = allRequests[i].color_code,
                                    status_request = allRequests[i].status_request,
                                    comment = allRequests[i].comment,
                                    is_two_step_approve = allRequests[i].is_two_step_approve,
                                    leave_name_en = allRequests[i].leave_name_en,
                                    leave_type_code = allRequests[i].leave_type_code,
                                    leave_type_id = allRequests[i].leave_type_id,
                                    level_step = allRequests[i].level_step,
                                    request_id = allRequests[i].request_id
                                };

                                new_requests.Add(request);
                            }
                        }
                    }
                    else
                    {
                        RequestModel request = new RequestModel()
                        {
                            emp_id = allRequests[i].emp_id,
                            leave_name_th = allRequests[i].leave_name_th,
                            start_request_date = allRequests[i].start_request_date,
                            end_request_date = allRequests[i].end_request_date,
                            amount_leave_day = 1,
                            start_request_time = new TimeSpan(8, 30, 0),
                            end_request_time = new TimeSpan(17, 30, 0),
                            is_full_day = allRequests[i].is_full_day,
                            description = allRequests[i].description,
                            request_date = allRequests[i].request_date,
                            path_file = allRequests[i].path_file,
                            amount_leave_hour = allRequests[i].amount_leave_hour,
                            color_code = allRequests[i].color_code,
                            status_request = allRequests[i].status_request,
                            comment = allRequests[i].comment,
                            is_two_step_approve = allRequests[i].is_two_step_approve,
                            leave_name_en = allRequests[i].leave_name_en,
                            leave_type_code = allRequests[i].leave_type_code,
                            leave_type_id = allRequests[i].leave_type_id,
                            level_step = allRequests[i].level_step,
                            request_id = allRequests[i].request_id
                        };

                        new_requests.Add(request);
                    }
                }
                else
                {
                    new_requests.Add(allRequests[i]);
                }
            }
            allRequests = new_requests;
            allRequests = allRequests
                .Where(r => deptEmpIds.Contains(r.emp_id))
                .Where(r => r.status_request != "Rejected" && r.status_request != "Canceled")
                //.Where(r => r.end_request_date >= startDate && r.start_request_date <= endDate)
                .ToList();

            if (!allRequests.Any())
                return Json(new object[0]);

            var empIdsWithLeave = allRequests.Select(r => r.emp_id).Distinct().ToHashSet();

            var finalEmps = empsInDept
                .Where(e => empIdsWithLeave.Contains(e.emp_id))
                .ToList();

            var dateRange = Enumerable.Range(0, (endDate.Date - _startDate.Date).Days + 1)
                                      .Select(i => _startDate.Date.AddDays(i))
                                      .ToList();

            var leaveCoverage = new Dictionary<string, object>();

            foreach (var req in allRequests)
            {
                var current = req.start_request_date.Date;
                while (current <= req.end_request_date.Date && current <= endDate.Date)
                {
                    if (current >= _startDate.Date)
                    {
                        bool IsWeekend = current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday;
                        bool IsHoliday = holidays.Any(a => a.date.Date == current.Date);
                        if (!IsHoliday && !IsWeekend)
                        {
                            var key = $"{req.emp_id}|{current:yyyy-MM-dd}";

                            if (req.is_full_day)
                            {
                                leaveCoverage[key] = new
                                {
                                    date = current.ToString("yyyy-MM-dd"),
                                    type = req.leave_name_th,
                                    duration = "full",
                                    time = "08:30 - 17:30",
                                    color_code = req.color_code,
                                    status = req.status_request
                                };
                            }
                            else if (!leaveCoverage.ContainsKey(key))
                            {
                                string duration = "";
                                string time = $"{req.start_request_time:hh\\:mm} - {req.end_request_time:hh\\:mm}";

                                var startTs = new TimeSpan(req.start_request_time.Hours, req.start_request_time.Minutes, 0);
                                var endTs = new TimeSpan(req.end_request_time.Hours, req.end_request_time.Minutes, 0);
                                var diff = endTs - startTs;

                                var t0830 = new TimeSpan(8, 30, 0);
                                var t1200 = new TimeSpan(12, 0, 0);
                                var t1300 = new TimeSpan(13, 0, 0);
                                var t1730 = new TimeSpan(17, 30, 0);

                                if (startTs == t0830 && endTs == t1200) duration = "half-left";
                                else if (startTs == t1300 && endTs == t1730) duration = "half-right";
                                else if (diff == TimeSpan.FromHours(2))
                                {
                                    if (startTs >= t0830 && endTs <= new TimeSpan(10, 30, 0)) duration = "quarter-top-left";
                                    else if (startTs >= t0830 && endTs <= t1200) duration = "quarter-bottom-left";
                                    else if (startTs >= t1300 && endTs <= new TimeSpan(15, 0, 0)) duration = "quarter-top-right";
                                    else if (startTs >= new TimeSpan(15, 0, 0) && endTs <= t1730) duration = "quarter-bottom-right";
                                }
                                else if (diff >= TimeSpan.FromHours(6))
                                    duration = "three-quarter-left";

                                leaveCoverage[key] = new
                                {
                                    date = current.ToString("yyyy-MM-dd"),
                                    type = req.leave_name_th,
                                    duration,
                                    time,
                                    color_code = req.color_code,
                                    status = req.status_request
                                };
                            }
                        }
                    }
                    current = current.AddDays(1);
                }
            }

            var result = finalEmps.Select(emp => new
            {
                emp_id = emp.emp_id,
                name = emp.name,
                img = emp.img,
                position = positionDict.GetValueOrDefault(emp.emp_id, "-"),
                leaves = dateRange.Select(day =>
                {
                    var key = $"{emp.emp_id}|{day:yyyy-MM-dd}";
                    return leaveCoverage.TryGetValue(key, out var leave) ? leave : null;
                }).ToArray()
            }).ToArray();            
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetDataLeave(string date)
        {
            DateTime startDate = Convert.ToDateTime(date);
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            List<RequestModel> allRequests = Requests.GetRequestByDate(date);
            allRequests = allRequests.Where(w => w.status_request != "Canceled").ToList();
            List<CTLModels.HolidayModel> holidays = Holiday.GetHolidays(startDate.Year.ToString());
            List<RequestModel> new_requests = new List<RequestModel>();
            for (int i = 0; i < allRequests.Count; i++)
            {
                if (allRequests[i].is_full_day)
                {
                    if (allRequests[i].amount_leave_day > 1)
                    {
                        for (DateTime date_ = allRequests[i].start_request_date; date_ <= allRequests[i].end_request_date; date_ = date_.AddDays(1))
                        {
                            if (date_.DayOfWeek != DayOfWeek.Saturday && date_.DayOfWeek != DayOfWeek.Sunday && !holidays.Any(a => a.date.Date == date_.Date))
                            {
                                RequestModel request = new RequestModel()
                                {
                                    emp_id = allRequests[i].emp_id,
                                    leave_name_th = allRequests[i].leave_name_th,
                                    start_request_date = date_,
                                    end_request_date = date_,
                                    amount_leave_day = 1,
                                    start_request_time = new TimeSpan(8, 30, 0),
                                    end_request_time = new TimeSpan(17, 30, 0),
                                    is_full_day = allRequests[i].is_full_day,
                                    description = allRequests[i].description,
                                    request_date = allRequests[i].request_date,
                                    path_file = allRequests[i].path_file,
                                    amount_leave_hour = allRequests[i].amount_leave_hour,
                                    color_code = allRequests[i].color_code,
                                    status_request = allRequests[i].status_request,
                                    comment = allRequests[i].comment,
                                    is_two_step_approve = allRequests[i].is_two_step_approve,
                                    leave_name_en = allRequests[i].leave_name_en,
                                    leave_type_code = allRequests[i].leave_type_code,
                                    leave_type_id = allRequests[i].leave_type_id,
                                    level_step = allRequests[i].level_step,
                                    request_id = allRequests[i].request_id
                                };

                                new_requests.Add(request);
                            }
                        }
                    }
                    else
                    {
                        RequestModel request = new RequestModel()
                        {
                            emp_id = allRequests[i].emp_id,
                            leave_name_th = allRequests[i].leave_name_th,
                            start_request_date = allRequests[i].start_request_date,
                            end_request_date = allRequests[i].end_request_date,
                            amount_leave_day = 1,
                            start_request_time = new TimeSpan(8, 30, 0),
                            end_request_time = new TimeSpan(17, 30, 0),
                            is_full_day = allRequests[i].is_full_day,
                            description = allRequests[i].description,
                            request_date = allRequests[i].request_date,
                            path_file = allRequests[i].path_file,
                            amount_leave_hour = allRequests[i].amount_leave_hour,
                            color_code = allRequests[i].color_code,
                            status_request = allRequests[i].status_request,
                            comment = allRequests[i].comment,
                            is_two_step_approve = allRequests[i].is_two_step_approve,
                            leave_name_en = allRequests[i].leave_name_en,
                            leave_type_code = allRequests[i].leave_type_code,
                            leave_type_id = allRequests[i].leave_type_id,
                            level_step = allRequests[i].level_step,
                            request_id = allRequests[i].request_id
                        };

                        new_requests.Add(request);
                    }
                }
                else
                {
                    new_requests.Add(allRequests[i]);
                }
            }

            List<RequestModel> requests = new_requests;

            List<GroupRequestAmountModel> group_request = requests
                .GroupBy(g => g.leave_type_code)
                .Select(g =>
                {
                    decimal fullDayAmount = g.Where(x => x.is_full_day == true)
                                             .Sum(x => x.amount_leave_day);

                    decimal halfDayAmount = g.Where(x => x.is_full_day == false)
                                             .Sum(x => x.amount_leave_hour) / 8.0m;

                    return new GroupRequestAmountModel()
                    {
                        leave_type_code = g.Key,
                        start_request_date = g.FirstOrDefault().start_request_date,
                        leave_name_en = g.FirstOrDefault().leave_name_en,
                        leave_name_th = g.FirstOrDefault().leave_name_th,
                        color_code = leaves.Where(v=>v.leave_type_code == g.Key).Select(s=>s.color_code).FirstOrDefault(),
                        amount_day = fullDayAmount + halfDayAmount,
                        amount_emp = g.Count()
                    };
                }).ToList();
            group_request = group_request.OrderByDescending(o => o.amount_day).ToList();
            return Json(group_request);
        }
    }
}
