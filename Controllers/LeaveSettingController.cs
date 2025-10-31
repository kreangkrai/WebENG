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

namespace WebENG.Controllers
{
    public class LeaveSettingController : Controller
    {
        readonly IAccessory Accessory;
        readonly IHierarchy Hierarchy;
        private ILeaveType LeaveType;
        private ILeaveEntitlementRule LeaveEntitlement;
        private CTLInterfaces.IEmployee Employee;
        public LeaveSettingController()
        {
            Accessory = new AccessoryService();
            Hierarchy = new HierarchyService();
            LeaveType = new LeaveTypeService();
            Employee = new CTLServices.EmployeeService();
            LeaveEntitlement = new LeaveEntitlementRuleService();
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

                List<HierarchyPersonalModel> hierarchies_personal = Hierarchy.GetPersonalHierarchies();
                List<HierarchyDepartmentModel> hierarchies_depaartment = Hierarchy.GetDepartmentHierarchies();

                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpPost]
        public IActionResult CreateLeaveGroup(string leave_code,string leave_th,string leave_en)
        {
            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            string admin = employees.Where(w => w.name.ToLower() == user.ToLower()).Select(s => s.emp_id).FirstOrDefault();
            DateTime date = DateTime.Now;
            string leave_id = Guid.NewGuid().ToString("N").Substring(0, 10);
            string message = LeaveType.Insert(new LeaveTypeModel()
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
                max_consecutive = false,
                min_request_hours = 2,
                request_timing = "Both"
            });
            return Json(message);
        }

        [HttpPut]
        public IActionResult UpdateLeaveGroup(string leave , string entitlement_manager, string entitlement_operation)
        {
            LeaveTypeModel leave_ = JsonConvert.DeserializeObject<LeaveTypeModel>(leave);
            LeaveTypeModel m_leave = LeaveType.GetLeaveTypeByID(leave_.leave_type_id);
            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            string admin = employees.Where(w => w.name.ToLower() == user.ToLower()).Select(s => s.emp_id).FirstOrDefault();
            string create_by = employees.Where(w => w.name.ToLower() == m_leave.created_by.ToLower()).Select(s => s.emp_id).FirstOrDefault();
            DateTime date = DateTime.Now;
            leave_.created_at = m_leave.created_at;
            leave_.created_by = create_by;
            leave_.updated_at =date;
            leave_.updated_by = admin;

            string message = LeaveType.Update(leave_);

            EntitlementModel manager = JsonConvert.DeserializeObject<EntitlementModel>(entitlement_manager);
            EntitlementModel oeration = JsonConvert.DeserializeObject<EntitlementModel>(entitlement_operation);

            List<LeaveEntitlementRuleModel> leaves_manager = new List<LeaveEntitlementRuleModel>();
            for(int i = 0; i < manager.entitlement.Count; i++)
            {
                string entitlement_rule_id = Guid.NewGuid().ToString("N").Substring(0, 10);
                LeaveEntitlementRuleModel m = new LeaveEntitlementRuleModel()
                {
                    entitlement_rule_id = entitlement_rule_id,
                    position = "Manager",
                    leave_type_id = leave_.leave_type_id,
                    start_age = manager.entitlement[i].start_age,
                    before_age = manager.entitlement[i].before_age,
                    days_per_year= manager.entitlement[i].days,
                    hours_per_year = manager.entitlement[i].hours
                };
                leaves_manager.Add(m);
            }

            List<LeaveEntitlementRuleModel> leaves_operation = new List<LeaveEntitlementRuleModel>();
            for (int i = 0; i < oeration.entitlement.Count; i++)
            {
                string entitlement_rule_id = Guid.NewGuid().ToString("N").Substring(0, 10);
                LeaveEntitlementRuleModel m = new LeaveEntitlementRuleModel()
                {
                    entitlement_rule_id = entitlement_rule_id,
                    position = "Operation",
                    leave_type_id = leave_.leave_type_id,
                    start_age = oeration.entitlement[i].start_age,
                    before_age = oeration.entitlement[i].before_age,
                    days_per_year = oeration.entitlement[i].days,
                    hours_per_year = oeration.entitlement[i].hours
                };
                leaves_operation.Add(m);
            }

            message = LeaveEntitlement.Delete(leave_.leave_type_id);

            if (message == "Success")
            {
                message = LeaveEntitlement.Insert(leaves_manager);
                if (message == "Success")
                {
                    message = LeaveEntitlement.Insert(leaves_operation);
                }
            }
            return Json(message);
        }

        [HttpGet]
        public IActionResult GetLeaveGroup()
        {
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            leaves = leaves.OrderBy(o => o.leave_type_code).ToList();
            List<LeaveEntitlementRuleModel> entitlements = LeaveEntitlement.GetLeaveEntitlementRules();
            var data = new { leaves = leaves, entitlements = entitlements };
            return Json(data);
        }
    }
}
