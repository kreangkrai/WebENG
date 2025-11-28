using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebENG.CTLInterfaces;
using WebENG.CTLModels;
using WebENG.CTLServices;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;
using System.Linq;
using WebENG.LeaveInterfaces;
using WebENG.LeaveServices;
using WebENG.LeaveModels;
using System;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace WebENG.Controllers
{
    public class LeaveSettingController : Controller
    {
        readonly IAccessory Accessory;
        private ILeaveType LeaveType;
        private IEmployee Employee;
        public LeaveSettingController()
        {
            Accessory = new AccessoryService();
            LeaveType = new LeaveTypeService();
            Employee = new EmployeeService();
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

        [HttpPost]
        public IActionResult CreateLeaveGroup(string leave_code, string leave_th, string leave_en)
        {
            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            string admin = employees.Where(w => w.name_en.ToLower() == user.ToLower()).Select(s => s.emp_id).FirstOrDefault();
            DateTime date = DateTime.Now;
            string leave_id = Guid.NewGuid().ToString("N").Substring(0, 10);

            var connect = new ConnectSQL();
            using (SqlConnection con = connect.OpenLeaveConnect())
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        var leaveTypeService = LeaveType;
                        leaveTypeService.Insert(new LeaveTypeModel()
                        {
                            leave_type_id = leave_id,
                            leave_name_th = leave_th,
                            leave_name_en = leave_en,
                            created_by = admin,
                            created_at = date,
                            updated_by = admin,
                            updated_at = date,
                            is_active = true,
                            leave_type_code = leave_code,
                            is_unpaid = false,
                            description = "",
                            attachment_threshold_days = 0,
                            attachment_required = false,
                            max_consecutive_days = 3,
                            count_holidays_as_leave = false,
                            gender_restriction = "Both",
                            is_consecutive = false,
                            min_request_hours = 2,
                            request_timing = "Both",
                            is_two_step_approve = false,
                            over_consecutive_days_for_two_step = 0,
                            color_code = "",
                            calculate_auto = false,
                            amount_entitlement = 0,
                            length_start_date = 0
                        });
                        tran.Commit();
                        return Json("Success");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json($"Error {ex.Message}");
                    }
                }
            }
        }

        [HttpPut]
        public IActionResult UpdateLeaveGroup(string leave)
        {
            LeaveTypeModel leave_ = JsonConvert.DeserializeObject<LeaveTypeModel>(leave);
            LeaveTypeModel m_leave = LeaveType.GetLeaveTypeByID(leave_.leave_type_id);
            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            string admin = employees.Where(w => w.name_en.ToLower() == user.ToLower()).Select(s => s.emp_id).FirstOrDefault();     
            DateTime date = DateTime.Now;
            leave_.created_at = m_leave.created_at;
            leave_.created_by = m_leave.created_by;
            leave_.updated_at =date;
            leave_.updated_by = admin;

            var connect = new ConnectSQL();
            using (SqlConnection con = connect.OpenLeaveConnect())
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        var leaveTypeService = LeaveType;
                        leaveTypeService.Update(leave_);
                        tran.Commit();
                        return Json("Success");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return Json($"Error {ex.Message}");
                    }
                }
            }                      
        }

        [HttpGet]
        public IActionResult GetLeaveGroup()
        {
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            leaves = leaves.OrderBy(o => o.priority).ThenBy(t=>t.leave_name_th).ToList();
            var data = new { leaves = leaves};
            return Json(data);
        }
    }
}
