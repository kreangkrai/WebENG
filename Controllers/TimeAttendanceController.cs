using HRManagement.Interface;
using HRManagement.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebENG.CTLInterfaces;
using WebENG.CTLServices;
using WebENG.HRInterface;
using WebENG.HRModel;
using WebENG.HRService;
using WebENG.Interface;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;
using WebENG.LeaveServices;
using WebENG.Models;
using WebENG.Service;
using WebENG.TripInterface;
using WebENG.TripModels;

namespace WebENG.Controllers
{
    public class TimeAttendanceController : Controller
    {
        private readonly IAccessory Accessory;
        private readonly ILeaveType LeaveType;
        private readonly IEmployee Employees;
        private readonly IRequest Requests;
        private readonly ILevel Level;
        private readonly ILeave Leave;
        private ILeaveExport LeaveExport;
        private IDeviceGroup DeviceGroup;
        private ITripExpense TripExpense;
        private readonly INEWTripExpense NewTripExpense;
        private IHr Hr;
        private CTLInterfaces.IHoliday Holiday;
        private IHostingEnvironment _hostingEnvironment;
        public TimeAttendanceController(IHostingEnvironment hostingEnvironment)
        {
            Accessory = new AccessoryService();
            LeaveType = new LeaveTypeService();
            Employees = new EmployeeService();
            Requests = new RequestService();
            Level = new LevelService();
            Leave = new LeaveService();
            LeaveExport = new LeaveExportService();
            DeviceGroup = new DeviceGroupService();
            TripExpense = new TripExpenseService();
            Hr = new HrService();
            Holiday = new CTLServices.HolidayService();
            NewTripExpense = new NewTripExpenseService();
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");

                List<UserModel> users = Accessory.getAllUser();
                List<WebENG.CTLModels.EmployeeModel> emps = Employees.GetEmployees();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).FirstOrDefault();
                if (u == null)
                {
                    List<WebENG.CTLModels.EmployeeModel> employees = Employees.GetEmployees();
                    WebENG.CTLModels.EmployeeModel employee = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();
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
        public string ConvertUserID(string user)
        {
            string first = user.Split(' ')[0];
            string last = user.Split(' ')[1];
            string name = first.Substring(0, 1).ToUpper() + first.Substring(1, first.Length - 1);
            string lastname = last.Substring(0, 1).ToUpper();
            return name + "." + lastname;
        }

        [HttpGet]
        public IActionResult GetLeaveType()
        {
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            leaves = leaves.OrderBy(o => o.priority).ToList();
            return Json(leaves);
        }

        [HttpGet]
        public IActionResult GetDapartments()
        {
            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.OrderBy(o => o.name_en).ToList();
            List<string> departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).OrderBy(o => o).ToList();
            return Json(departments);
        }

        [HttpGet]
        public IActionResult GetTimeAttendance(string department, int year)
        {
            List<TimeAttendanceModel> timeAttendances = CalculateTimeAttendance(department, year);
            List<TimeInOutModel> calLatetimes = CalLateTime(year);
            var data = new { timeAttendances = timeAttendances, calLatetimes = calLatetimes };
            return Json(data);
        }

        List<TimeAttendanceModel> CalculateTimeAttendance(string department, int year)
        {
            List<TimeAttendanceModel> objs = new List<TimeAttendanceModel>();
            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.Where(w => w.active).OrderBy(o => o.department).ThenBy(t => t.emp_id).ToList();
            if (department != "ALL")
            {
                employees = employees.Where(w => w.department == department).OrderBy(o => o.emp_id).ToList();
            }

            for (int i = 0; i < employees.Count; i++)
            {
                double entitlement_al = Leave.CalculateLeaveDays(employees[i], year, 6, 10, 10, 12);

                List<RequestModel> requests = Requests.GetRequestByEmpID(employees[i].emp_id);
                requests = requests.Where(w => w.start_request_date.Year == year && w.status_request != "Canceled" && w.status_request != "Rejected").ToList();

                var monthlyGroups = requests
                    .GroupBy(r => new { r.leave_type_code, Month = r.start_request_date.Month })
                    .Select(g => new
                    {
                        leave_type_code = g.Key.leave_type_code,
                        Month = g.Key.Month,
                        MonthlyAmount = g.Sum(x => x.amount_leave_day) + g.Sum(x => x.amount_leave_hour) / 8.0M,
                        leave_name_en = g.FirstOrDefault().leave_name_en,
                        leave_name_th = g.FirstOrDefault().leave_name_th,

                    }).ToList();

                List<LeaveTimeAttendanceModel> accumulatedByType = monthlyGroups
                    .GroupBy(g => g.leave_type_code)
                    .Select(typeGroup =>
                    {
                        var first = typeGroup.First();
                        var monthlyData = typeGroup
                                            .OrderBy(m => m.Month)
                                            .Select(m => new { m.Month, m.MonthlyAmount })
                                            .ToList();

                        decimal cum = 0;
                        var accumList = new List<MonthlyAccumulatedModel>();
                        for (int month = 1; month <= 12; month++)
                        {
                            var inc = monthlyData.FirstOrDefault(d => d.Month == month)?.MonthlyAmount ?? 0;
                            cum += inc;

                            accumList.Add(new MonthlyAccumulatedModel()
                            {
                                Month = month,
                                MonthlyAmount = Math.Round(inc, 2),
                                AccumulatedAmount = Math.Round(cum, 2)
                            });
                        }

                        return new LeaveTimeAttendanceModel()
                        {
                            leave_type_code = typeGroup.Key,
                            leave_name_en = first.leave_name_en ?? typeGroup.Key + "-",
                            leave_name_th = first.leave_name_th ?? typeGroup.Key + "-",
                            MonthlyAccumulated = accumList,

                        };
                    }).ToList();

                objs.Add(new TimeAttendanceModel()
                {
                    emp_id = employees[i].emp_id,
                    name_en = employees[i].name_en,
                    name_th = employees[i].name_th,
                    department = employees[i].department,
                    entitlement_al = entitlement_al,
                    leaves = accumulatedByType,
                    position = employees[i].position
                });
            }
            return objs;
        }
        public IActionResult ExportTimeAttendance(string department, int year)
        {
            List<TimeAttendanceModel> objs = CalculateTimeAttendance(department, year);
            objs = objs.OrderBy(o => o.department).ThenBy(t => t.position).ToList();
            List<TimeInOutModel> calLatetimes = CalLateTime(year);

            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            leaves = leaves.OrderBy(o => o.priority).ToList();

            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/Template", "TimeAttendance.xlsx"));
            var stream = LeaveExport.ExportData(templateFileInfo, objs, calLatetimes,leaves, year);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TimeAttendance_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx");
        }

        public List<TimeInOutModel> CalLateTime(int year)
        {
            DateTime start = new DateTime(year, 1, 1);
            DateTime stop = new DateTime(year, 12, 31);
            List<CTLModels.HolidayModel> holidays = Holiday.GetHolidays(year.ToString());
            var holidayDates = new HashSet<DateTime>(holidays.Select(h => h.date.Date));
            List<EmployeeWorkModel> dailyRecords = CalData(start, stop);

            var groupedByEmp = dailyRecords.GroupBy(g => g.emp_id);

            var summary = groupedByEmp.Select(g =>
            {
                var records = g.ToList();
                var monthlyDict = new Dictionary<int, MonthlyTimeInOutModel>();

                List<RequestModel> request_leave = Requests.GetRequestByEmpID(g.Key)
                    .Where(w => w.status_request != "Canceled" && w.status_request != "Rejected")
                    .ToList();

                foreach (var rec in records)
                {
                    DateTime date = rec.date;
                    int month = date.Month;
                    bool isHoliday = holidayDates.Contains(date);
                    bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                    bool isNonWorking = isHoliday || isWeekend;

                    if (isNonWorking) continue;

                    if (!monthlyDict.TryGetValue(month, out var monthly))
                    {
                        monthly = new MonthlyTimeInOutModel { month = month };
                        monthlyDict[month] = monthly;
                    }

                    if (rec.start_time_trip_expense == TimeSpan.Zero)
                    {
                        if (rec.start_time_face_scan != TimeSpan.Zero) // Face Scan
                        {
                            if (rec.start_time_face_scan > TimeSpan.Parse(rec.shift_time)) // Late
                            {
                                bool check_leave = request_leave.Any(w => w.start_request_date.Date == date); // Check Leave
                                if (!check_leave)
                                {
                                    var lateMinutes = (int)(rec.start_time_face_scan - TimeSpan.Parse(rec.shift_time)).TotalMinutes;
                                    if (lateMinutes > 0)
                                    {
                                        if (rec.start_time_face_scan > new TimeSpan(13, 0, 0))
                                        {
                                            lateMinutes -= 60;
                                        }
                                        if (lateMinutes > 480)
                                        {
                                            lateMinutes = 480;
                                        }
                                        monthly.minute_late += lateMinutes;
                                        monthly.count_late++;
                                    }
                                }
                                else
                                {
                                    var leave = request_leave.Where(w => w.start_request_date.Date == date).FirstOrDefault();
                                    if (!leave.is_full_day)
                                    {
                                        var lateMinutes = (int)(rec.start_time_face_scan - leave.end_request_time).TotalMinutes;
                                        if (lateMinutes > 0)
                                        {
                                            if (rec.start_time_face_scan > new TimeSpan(13, 0, 0))
                                            {
                                                lateMinutes -= 60;
                                            }
                                            if (lateMinutes > 480)
                                            {
                                                lateMinutes = 480;
                                            }
                                            monthly.minute_late += lateMinutes;
                                            monthly.count_late++;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool check_leave = request_leave.Any(w => w.start_request_date.Date == date);
                            if (!check_leave)
                            {
                                monthly.count_forgot_scan++;
                            }
                        }
                    }
                }

                var monthsList = monthlyDict.Values.ToList();

                return new TimeInOutModel
                {
                    emp_id = g.Key,
                    months = monthsList
                };
            })
            .OrderBy(x => x.emp_id)
            .ToList();

            return summary;
        }

        public List<EmployeeWorkModel> CalData(DateTime start, DateTime stop)
        {
            if (stop > DateTime.Now)
            {
                stop = DateTime.Now;
            }

            //Get Employee ID
            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.GroupBy(g => new { g.emp_id, g.name_en, g.department }).Select(s => new CTLModels.EmployeeModel()
            {
                emp_id = s.Key.emp_id,
                name_en = s.Key.name_en,
                department = s.Key.department
            }).ToList();

            //Data Trip Expense
            List<TripExpenseModel> trips = TripExpense.GetData(start, stop);

            //Data Trip Expense New
            List<TripExpenseModel> new_trips = NewTripExpense.GetData(start, stop);

            //Combine Trip
            trips.AddRange(new_trips);

            List<EmployeeTimesModel> all = trips
            .GroupBy(g => new { g.date, g.emp_id })
            .AsParallel()
            .Select(group =>
            {
                var sortedTrips = group.OrderBy(t => t.date).ToList();

                var workTrips = sortedTrips
                    .Where(t => t.location.ToLower().Contains("customer") ||
                                t.location.ToLower().Contains("hotel") ||
                                t.location.ToLower().Contains("โรงแรม"))
                    .ToList();

                bool hasWorkLocation = workTrips.Any();

                var firstTrip = sortedTrips.First();
                var lastTrip = sortedTrips.Last();

                var firstWork = hasWorkLocation ? workTrips.First() : firstTrip;
                var lastWork = hasWorkLocation ? workTrips.Last() : lastTrip;

                return new EmployeeTimesModel
                {
                    date = group.Key.date,
                    emp_id = group.Key.emp_id,
                    type = "Trip Expense",
                    location = string.Join(",", sortedTrips.Select(x => x.location)),

                    actual_start_time = new TimeSpan(firstTrip.date.TimeOfDay.Ticks),
                    actual_last_time = new TimeSpan(lastTrip.date.TimeOfDay.Ticks),

                    start_time = new TimeSpan(firstWork.date.TimeOfDay.Ticks),
                    last_time = new TimeSpan(lastWork.date.TimeOfDay.Ticks),

                    start = new TimeSpan(8, 30, 0),
                    stop = new TimeSpan(17, 30, 0)
                };
            }).ToList();

            List<DataModel> hrs = Hr.GetDataByDate(start, stop);
            hrs = hrs.Where(w => employees.Any(a => a.emp_id == w.cn)).ToList();

            List<EmployeeTimesModel> _all = hrs
                .GroupBy(g => new { date = g.date.Date, g.cn })
                .Select(group =>
                {
                    var sorted = group.OrderBy(x => x.date).ToList();

                    if (!sorted.Any())
                        return null;

                    var firstRecord = sorted.First();
                    var lastRecord = sorted.Last();

                    return new EmployeeTimesModel
                    {
                        date = group.Key.date,
                        emp_id = group.Key.cn,
                        type = "Face Scan",
                        start_time = firstRecord.time_in,
                        last_time = lastRecord.time_out,
                        name = firstRecord.personname,
                        location = firstRecord.device_in,
                        start = firstRecord.start,
                        stop = firstRecord.stop,
                        personal_group = firstRecord.persongroup,
                        sn = firstRecord.sn
                    };
                })
                .Where(x => x != null)
                .ToList();



            all.AddRange(_all);

            var allDict = all
                .GroupBy(x => new { x.date.Date, x.emp_id, x.type })
                .ToDictionary(
                    g => new { g.Key.Date, g.Key.emp_id, g.Key.type },
                    g => g.OrderBy(x => x.date).ToList()  // เรียงเวลาไว้เลย เผื่อใช้ First/Last
                );

            var tripExpenseGroups = allDict
                .Where(kv => kv.Key.type == "Trip Expense")
                .ToDictionary(kv => new { kv.Key.Date, kv.Key.emp_id }, kv => kv.Value);

            var faceScanGroups = allDict
                .Where(kv => kv.Key.type == "Face Scan")
                .ToDictionary(kv => new { kv.Key.Date, kv.Key.emp_id }, kv => kv.Value);

            // Employee lookup
            var employeeDict = employees
                .ToDictionary(e => e.emp_id, e => new { e.department, e.name_en });


            var deviceGroupDict = DeviceGroup.GetDevicesGroup()
    .GroupBy(d => new { d.device, d.groupname })
    .ToDictionary(
        g => g.Key,
        g => g.First().starttime.ToString(@"hh\:mm"));

            var allGroups = all
                .GroupBy(g => new { Date = g.date.Date, g.emp_id })
                .ToList();

            List<EmployeeWorkModel> employees_ = allGroups.Select(g =>
            {
                var key = new { Date = g.Key.Date, emp_id = g.Key.emp_id };

                // Trip Expense data
                tripExpenseGroups.TryGetValue(key, out var tripList);
                var tripFirst = tripList?.FirstOrDefault();
                var tripLast = tripList?.LastOrDefault();

                // Face Scan data
                faceScanGroups.TryGetValue(key, out var faceList);
                var faceFirst = faceList?.FirstOrDefault();
                var faceLast = faceList?.LastOrDefault();

                var anyRecord = g.FirstOrDefault();
                string shift_time = null;
                if (faceFirst != null)
                {
                    var dgKey = new { device = faceFirst.sn, groupname = faceFirst.personal_group };
                    deviceGroupDict.TryGetValue(dgKey, out shift_time);
                }
                employeeDict.TryGetValue(g.Key.emp_id, out var empInfo);

                return new EmployeeWorkModel
                {
                    date = g.Key.Date,
                    emp_id = g.Key.emp_id,

                    start_time_trip_expense = tripFirst?.start_time ?? TimeSpan.Zero,
                    actual_start_time_trip_expense = tripFirst?.actual_start_time ?? TimeSpan.Zero,
                    last_time_trip_expense = tripLast?.last_time ?? TimeSpan.Zero,
                    actual_last_time_trip_expense = tripLast?.actual_last_time ?? TimeSpan.Zero,
                    location_trip_expense = tripLast?.location ?? tripFirst?.location,

                    start_time_face_scan = faceFirst?.start_time ?? TimeSpan.Zero,
                    last_time_face_scan = faceLast?.last_time ?? TimeSpan.Zero,
                    location_face_scan = faceFirst?.location,

                    department = empInfo?.department,
                    name = empInfo?.name_en,

                    start = anyRecord?.start ?? new TimeSpan(8, 30, 0),
                    stop = anyRecord?.stop ?? new TimeSpan(17, 30, 0),

                    shift_time = shift_time ?? ""
                };
            }).ToList();            
            return employees_;
        }
    }
}