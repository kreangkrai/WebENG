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
            //List<EmpModel> emps = Employee.GetEmps();
            List<RequestModel> requests = Requests.GetRequestByMonth(month);
            int year = Convert.ToInt32(month.Split('-')[0]);
            int m = Convert.ToInt32(month.Split('-')[1]);
            requests = requests.Where(w => w.status_request == "Pending" || w.status_request == "Successed").ToList();
            List<EmpModel> emps = new List<EmpModel>();
            emps.Add(new EmpModel()
            {
                emp_id = "059197",
                name = "kriangkrai rattanawan",

            });
            var data = new { emps = emps, requests = requests };
            var result = emps.Select(emp => new
            {
                emp_id = emp.emp_id,
                name = emp.name,
                img = emp.img,
                leaves = Enumerable.Range(1, DateTime.DaysInMonth(year, m))
                           .Select(day =>
                           {
                               var req = requests.FirstOrDefault(r =>
                                   r.emp_id == emp.emp_id &&
                                   r.start_request_date.Date == new DateTime(year, m, day).Date);
                               if (req == null) return null;

                               return new
                               {
                                   type = req.leave_name_th,
                                   duration = req.is_full_day ? "full":"hours",
                                   time = $"{req.start_request_time} - {req.end_request_time}"
                               };
                           })
                           .ToArray()
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
