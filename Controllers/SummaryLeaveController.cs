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
        readonly IHierarchy Hierarchy;
        readonly CTLInterfaces.IEmployee Employee;
        readonly IRequest Requests;
        readonly ILeaveType LeaveType;
        public SummaryLeaveController()
        {
            Accessory = new AccessoryService();
            Hierarchy = new HierarchyService();
            Employee = new CTLServices.EmployeeService();
            Requests = new RequestService();
            LeaveType = new LeaveTypeService();
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

            var startDate = Convert.ToDateTime(start);
            var endDate = Convert.ToDateTime(end);

            var allEmps = Employee.GetEmps();
            var allEmployees = Employee.GetEmployees();


            var positionDict = allEmployees
                .Where(e => e.department == department)
                .ToDictionary(e => e.emp_id, e => e.position);

            var deptEmpIds = positionDict.Keys.ToHashSet();

            var empsInDept = allEmps
                .Where(e => deptEmpIds.Contains(e.emp_id))
                .ToList();

            if (!empsInDept.Any())
                return Json(new object[0]);

            var requests = Requests.GetRequestByDurationDay(
                startDate.ToString("yyyy-MM-dd"),
                endDate.ToString("yyyy-MM-dd"))
                .Where(r => r.status_request == "Pending" || r.status_request == "Successed")
                .ToList();

            var empIdsWithLeave = requests
                .Select(r => r.emp_id)
                .Distinct()
                .Where(id => deptEmpIds.Contains(id))
                .ToHashSet();

            var finalEmps = empsInDept
                .Where(e => empIdsWithLeave.Contains(e.emp_id))
                .ToList();

            var dateRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                      .Select(i => startDate.AddDays(i))
                                      .ToList();
            var requestLookup = requests
                .GroupBy(r => new { r.emp_id, Date = r.start_request_date.Date })
                .ToDictionary(
                    g => g.Key,
                    g => g.First()
                );


            var result = finalEmps.Select(emp => new
            {
                emp_id = emp.emp_id,
                name = emp.name,
                img = emp.img,
                position = positionDict.GetValueOrDefault(emp.emp_id, "-"),

                leaves = dateRange.Select(day =>
                {
                    var key = new { emp_id = emp.emp_id, Date = day.Date };
                    if (!requestLookup.TryGetValue(key, out var req))
                        return null;

                    string duration = "";
                    string time = "";

                    if (req.is_full_day)
                    {
                        duration = "full";
                        time = "08:30 - 17:30";
                    }
                    else
                    {
                        var startTs = new TimeSpan(req.start_request_time.Hours, req.start_request_time.Minutes, 0);
                        var endTs = new TimeSpan(req.end_request_time.Hours, req.end_request_time.Minutes, 0);

                        var t0830 = new TimeSpan(8, 30, 0);
                        var t1200 = new TimeSpan(12, 0, 0);
                        var t1300 = new TimeSpan(13, 0, 0);
                        var t1730 = new TimeSpan(17, 30, 0);

                        if (startTs == t0830 && endTs == t1200) duration = "half-left";
                        else if (startTs == t1300 && endTs == t1730) duration = "half-right";
                        else if ((endTs - startTs) == TimeSpan.FromHours(2))
                        {
                            if (startTs >= t0830 && endTs <= new TimeSpan(10, 30, 0)) duration = "quarter-top-left";
                            else if (startTs >= t0830 && endTs <= t1200) duration = "quarter-bottom-left";
                            else if (startTs >= t1300 && endTs <= new TimeSpan(15, 0, 0)) duration = "quarter-top-right";
                            else if (startTs >= new TimeSpan(15, 0, 0) && endTs <= t1730) duration = "quarter-bottom-right";
                        }
                        else if ((endTs - startTs) >= TimeSpan.FromHours(6))
                        {
                            duration = "three-quarter-left";
                        }

                        time = req.start_request_time.ToString(@"hh\:mm") + " - " + req.end_request_time.ToString(@"hh\:mm");
                    }

                    return new
                    {
                        date = day.ToString("yyyy-MM-dd"),
                        type = req.leave_name_th,
                        duration = duration,
                        time = time,
                        color_code = req.color_code,
                        status = req.status_request
                    };

                }).ToArray()

            }).ToArray();

            return Json(result);

        }

        [HttpGet]
        public IActionResult GetDataLeave(string date)
        {
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            List<RequestModel> requests = Requests.GetRequestByDate(date);
            requests = requests.Where(w => w.status_request != "Canceled").ToList();
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
