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

/*

 Pending , Resubmit => Operation, Manager
Cancelled , Approved, Rejected ,Returned
Successed
 * 
 */
namespace WebENG.Controllers
{
    public class LeaveController : Controller
    {
        readonly IAccessory Accessory;
        readonly IHierarchy Hierarchy;
        readonly INotification Notification;
        readonly ILeaveType LeaveType;
        readonly ILeave Leave;
        private CTLInterfaces.IEmployee Employee;
        readonly IRequest Requests;
        public LeaveController()
        {
            Accessory = new AccessoryService();
            Hierarchy = new HierarchyService();
            Notification = new NotificationService();
            LeaveType = new LeaveTypeService();
            Leave = new LeaveService();
            Employee = new CTLServices.EmployeeService();
            Requests = new RequestService();

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
                List<HierarchyDepartmentModel> hierarchies_depaartmenr = Hierarchy.GetDepartmentHierarchies();

                CTLModels.EmployeeModel emp = new CTLModels.EmployeeModel()
                {
                    start_date = new DateTime (2025,6,15),
                    promote_manager_date = new DateTime (2025,6,15),
                    position = "Manager"
                };

                double leave = Leave.CalculateLeaveDays(emp, 2025, 6, 10, 10, 12); 
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpPatch]
        public IActionResult ClearNotification(string emp_id)
        {            
            string message = Notification.UpdateStatus(emp_id,"Read");
            return Json(message);
        }

        [HttpGet]
        public IActionResult GetLeaves()
        {
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            
            var data = new { leaves = leaves };
            return Json(data);
        }
        [HttpGet]
        public IActionResult GetLeaveById(string leave_type_id, string emp_id)
        {
            LeaveTypeModel leave = LeaveType.GetLeaveTypeByID(leave_type_id);
            int year = DateTime.Now.Year;
            List<CTLModels.EmployeeModel> emps = Employee.GetEmployees();
            List<RequestModel> requests = Requests.GetRequestByEmpID(emp_id);
            requests = requests.Where(w => w.leave_type_id == leave_type_id &&
            w.start_request_date.Year == year &&
            w.status_request == "Successed").ToList();
            if (leave.calculate_auto == true)
            {
                var em = emps.Where(w => w.emp_id == emp_id).FirstOrDefault();
                CTLModels.EmployeeModel emp = new CTLModels.EmployeeModel()
                {
                    emp_id = emp_id,
                    position = em.position,
                    start_date = em.start_date,
                    promote_manager_date = em.promote_manager_date,                   
                };
                double _leave = Leave.CalculateLeaveDays(emp, year, 6, 10, 10, 12);
                leave.amount_entitlement = (decimal)_leave;
            }
            double balance = (double)leave.amount_entitlement - requests.Count;
            var data = new { leave = leave ,balance = balance };
            return Json(data);
        }
    }
}
