using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TimeAttendanceController : Controller
    {
        private readonly IAccessory Accessory;
        private readonly ILeaveType LeaveType;
        private readonly IEmployee Employees;
        private readonly IRequest Requests;
        private readonly ILevel Level;
        private readonly ILeave Leave;
        public TimeAttendanceController()
        {
            Accessory = new AccessoryService();
            LeaveType = new LeaveTypeService();
            Employees = new EmployeeService();
            Requests = new RequestService();
            Level = new LevelService();
            Leave = new LeaveService();
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

            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.Where(w=>w.active).OrderBy(o => o.department).ThenBy(t => t.emp_id).ToList();
            if (department != "ALL")
            {
                employees = employees.Where(w => w.department == department).OrderBy(o => o.emp_id).ToList();
            }

            List<object> objs = new List<object>();
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

                var accumulatedByType = monthlyGroups
                    .GroupBy(g => g.leave_type_code)
                    .Select(typeGroup =>
                    {
                        var first = typeGroup.First();
                        var monthlyData = typeGroup
                                            .OrderBy(m => m.Month)
                                            .Select(m => new { m.Month, m.MonthlyAmount })
                                            .ToList();

                        decimal cum = 0;
                        var accumList = new List<object>();
                        for (int month = 1; month <= 12; month++)
                        {
                            var inc = monthlyData.FirstOrDefault(d => d.Month == month)?.MonthlyAmount ?? 0;
                            cum += inc;

                            accumList.Add(new
                            {
                                Month = month,
                                MonthlyAmount = Math.Round(inc, 2),
                                AccumulatedAmount = Math.Round(cum, 2)
                            });
                        }

                        return new
                        {
                            leave_type_code = typeGroup.Key,
                            leave_name_en = first.leave_name_en ?? typeGroup.Key + "-",
                            leave_name_th = first.leave_name_th ?? typeGroup.Key + "-",
                            MonthlyAccumulated = accumList,

                        };
                    }).ToList();

                objs.Add(new { emp_id = employees[i].emp_id,
                    name_en = employees[i].name_en,
                    name_th = employees[i].name_th,
                    department = employees[i].department,
                    entitlement_al = entitlement_al,
                    requests = accumulatedByType });               
            }
            
            return Json(objs);
        }
    }
}