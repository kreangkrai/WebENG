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
    public class ManagementLeaveController : Controller
    {
        readonly IAccessory Accessory;
        readonly IRequest Requests;
        readonly IEmployee Employee;
        readonly ILevel Level;
        readonly ILeaveType LeaveType;
        public ManagementLeaveController()
        {
            Accessory = new AccessoryService();
            Requests = new RequestService();
            Employee = new EmployeeService();
            Level = new LevelService();
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

                GetRequest();


                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public IActionResult GetRequest()
        {
            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            string emp_id = employees.Where(w => w.name_en.ToLower() == user.ToLower()).Select(s => s.emp_id).FirstOrDefault();
            
            List<LevelModel> levels = Level.GetLevelByEmpID(emp_id);
            List<int> level = levels.Where(w => w.emp_id == emp_id).Select(s => s.level).ToList();
            List<string> departments = levels.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).ToList();

            List<RequestModel> requests = Requests.GetRequests();
            List<RequestModel> _requests = new List<RequestModel>();
            for (int i = 0; i < requests.Count; i++)
            {              
                if (level.Count > 0)
                {
                    if (requests[i].is_two_step_approve)
                    {
                        LeaveTypeModel leaveType = LeaveType.GetLeaveTypeByID(requests[i].leave_type_id);
                        if (requests[i].amount_leave_day >= leaveType.max_consecutive_days)
                        {
                            // Two Step Approve
                            bool chk_level = level.Any(a => a == requests[i].level_step + 1);
                            if (chk_level)
                            {
                                string request_department = employees.Where(w => w.emp_id == requests[i].emp_id).Select(s => s.department).FirstOrDefault();
                                bool chk_dep = departments.Contains(request_department);
                                if (chk_dep)
                                {
                                    _requests.Add(requests[i]);
                                }
                            }
                        }
                        else
                        {
                            //One Step Approve
                            bool chk_level = level.Any(a => a == requests[i].level_step + 1); //2
                            if (chk_level)
                            {
                                string request_department = employees.Where(w => w.emp_id == requests[i].emp_id).Select(s => s.department).FirstOrDefault();
                                bool chk_dep = departments.Contains(request_department);
                                if (chk_dep)
                                {
                                    _requests.Add(requests[i]);
                                }
                            }
                        }                      
                    }
                    else
                    {
                        bool chk_level = level.Any(a => a == requests[i].level_step + 1);
                        if (chk_level)
                        {
                            string request_department = employees.Where(w => w.emp_id == requests[i].emp_id).Select(s => s.department).FirstOrDefault();
                            bool chk_dep = departments.Contains(request_department);
                            if (chk_dep)
                            {
                                _requests.Add(requests[i]);
                            }
                        }
                    }
                }                
            }

            var data = _requests.Select(s => new
            {
                emp_id = s.emp_id,
                emp_name = employees.Where(w => w.emp_id == s.emp_id).Select(x => x.name_en).FirstOrDefault(),
                request_date = s.request_date,
                start_request_date = s.start_request_date,
                end_request_date = s.end_request_date,
                start_request_time = s.start_request_time,
                end_request_time = s.end_request_time,
                amount_leave_day = s.amount_leave_day,
                leave_name_th = s.leave_name_th,
                description = s.description,
                path_file = s.path_file,
                is_full_day = s.is_full_day,
                status_request = s.status_request,
                attachment_required = LeaveType.GetLeaveTypeByID(s.leave_type_id).attachment_required,
                attachment_threshold_days = LeaveType.GetLeaveTypeByID(s.leave_type_id).attachment_threshold_days,
            }).ToList();

            return Json(data);
        }


        [HttpGet]
        public IActionResult GetData()
        {
            List<EmpModel> emps = Employee.GetEmps();
            return Ok(emps);
        }
    }
}
