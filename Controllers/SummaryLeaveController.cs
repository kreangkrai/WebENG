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
        public IActionResult GetDataByMonth(string month)
        {
            List<EmpModel> emps = Employee.GetEmps();
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            List<RequestModel> requests = Requests.GetRequestByMonth(month);
            int year = Convert.ToInt32(month.Split('-')[0]);
            int m = Convert.ToInt32(month.Split('-')[1]);
            requests = requests.Where(w => w.status_request == "Pending" || w.status_request == "Successed").ToList();
            List<string> request_empid = requests.GroupBy(g => g.emp_id).Select(s => s.FirstOrDefault().emp_id).ToList();
            emps = emps.Where(w => request_empid.Contains(w.emp_id)).ToList();
            var result = emps.Select(emp => new
            {
                emp_id = emp.emp_id,
                name = emp.name,
                img = emp.img,
                position = employees.Where(w=>w.emp_id == emp.emp_id).Select(s=>s.position).FirstOrDefault(),
                leaves = Enumerable.Range(1, DateTime.DaysInMonth(year, m))
                           .Select(day =>
                           {
                               var req = requests.FirstOrDefault(r =>
                                   r.emp_id == emp.emp_id &&
                                   r.start_request_date.Date == new DateTime(year, m, day).Date);
                               if (req == null) return null;

                               string duration = "";
                               string time = "";
                               if (req.is_full_day)
                               {
                                   duration = "full";
                                   time = "08:30 - 17:30";
                               }
                               else
                               {
                                   TimeSpan t0830 = new TimeSpan(8, 30, 0);
                                   TimeSpan t1000 = new TimeSpan(10, 0, 0);
                                   TimeSpan t1030 = new TimeSpan(10, 30, 0);
                                   TimeSpan t1200 = new TimeSpan(12, 0, 0);
                                   TimeSpan t1300 = new TimeSpan(13, 0, 0);
                                   TimeSpan t1500 = new TimeSpan(15, 0, 0);
                                   TimeSpan t1730 = new TimeSpan(17, 30, 0);

                                   TimeSpan start = req.start_request_time - TimeSpan.FromDays(req.start_request_time.Days);
                                   TimeSpan end = req.end_request_time - TimeSpan.FromDays(req.start_request_time.Days);
                                   TimeSpan durationSpan = end - start;

                                   if (start == t0830 && end == t1200)
                                   {
                                       duration = "half-left";
                                   }
                                   else if (start == t1300 && end == t1730)
                                   {
                                       duration = "half-right";
                                   }
 
                                   else if (durationSpan == TimeSpan.FromHours(2))
                                   {
                                       if (start >= t0830 && end <= t1030)
                                           duration = "quarter-top-left";
                                       else if (start >= t0830 && end <= t1200)
                                           duration = "quarter-bottom-left";
                                       else if (start >= t1300 && end <= t1500)
                                           duration = "quarter-top-right";
                                       else if (start >= t1500 && end <= t1730)
                                           duration = "quarter-bottom-right";
                                   }
                                   else if (durationSpan >= TimeSpan.FromHours(6))
                                   {
                                       duration = "three-quarter";
                                   }

                                   time = $"{req.start_request_time.ToString(@"hh\:mm")} - {req.end_request_time.ToString(@"hh\:mm")}";
                               }
                               return

                               new
                               {
                                   type = req.leave_name_th,
                                   duration = duration,
                                   time = time,
                                   color_code = req.color_code
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
