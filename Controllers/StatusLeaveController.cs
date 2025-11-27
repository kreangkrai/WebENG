using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class StatusLeaveController : Controller
    {
        readonly IAccessory Accessory;
        readonly CTLInterfaces.IEmployee Employees;
        private ILeaveType LeaveType;
        private IRequest Requests;
        private ILeave Leave;
        private ILevel Level;
        public StatusLeaveController()
        {
            Accessory = new AccessoryService();
            Employees = new CTLServices.EmployeeService();
            LeaveType = new LeaveTypeService();
            Requests = new RequestService();
            Leave = new LeaveService();
            Level = new LevelService();
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

                List<int> years = new List<int>();
                for(int y = DateTime.Now.AddYears(1).Year;y > DateTime.Now.AddYears(-5).Year; y--)
                {
                    years.Add(y);
                }
                ViewBag.listYears = years;

                List<LevelModel> levels = Level.GetLevelByEmpID(u.emp_id);
                ViewBag.levels = levels;

                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }


        [HttpGet]
        public IActionResult GetEmployee(string start, string stop)
        {
            List<RequestModel> requests = Requests.GetRequestByDurationDay(start,stop);
            List<string> emps = requests.GroupBy(g => g.emp_id).Select(s => s.FirstOrDefault().emp_id).ToList();

            List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
            employees = employees.Where(w => emps.Contains(w.emp_id)).OrderBy(o => o.name_en).ToList();
            List<string> departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).OrderBy(o => o).ToList();
            var data = new { employees = employees, departments = departments };
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetLeaveUsedByEmpID(string emp_id)
        {
            int year = DateTime.Now.Year;
            List<UsedLeaveModel> useds = new List<UsedLeaveModel>();
            List<string> status_pending = new List<string>()
            {
                "Pending",
                "Resubmit",
                "Approved",
                "Returned",
                "Completed"
            };
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            List<CTLModels.EmployeeModel> emps = Employees.GetEmployees();
            List<RequestModel> requests = Requests.GetRequestByEmpID(emp_id);
           
            for (int i = 0; i < leaves.Count; i++)
            {
                List<RequestModel>  _requests = requests.Where(w => w.leave_type_code == leaves[i].leave_type_code &&
                w.start_request_date.Year == year &&
                status_pending.Contains(w.status_request)).ToList();

                CTLModels.EmployeeModel emp = new CTLModels.EmployeeModel();
                var em = emps.Where(w => w.emp_id == emp_id).FirstOrDefault();
                emp = new CTLModels.EmployeeModel()
                {
                    emp_id = emp_id,
                    position = em.position,
                    start_date = em.start_date,
                    promote_manager_date = em.promote_manager_date,
                    gender = em.gender
                };
                if (leaves[i].calculate_auto == true)
                {
                    double _leave = Leave.CalculateLeaveDays(emp, year, 6, 10, 10, 12);
                    leaves[i].amount_entitlement = (decimal)_leave;
                }
                double used_leave = 0;
                for (int j = 0; j < _requests.Count; j++)
                {
                    if (_requests[j].is_full_day)
                    {
                        used_leave += _requests[j].amount_leave_day;
                    }
                    else
                    {
                        used_leave += Math.Round(((double)_requests[j].amount_leave_hour) / 8.0, 2);
                    }
                }
                //decimal b = (decimal)((double)leaves[i].amount_entitlement - used_leave);

                UsedLeaveModel used = new UsedLeaveModel()
                {
                    leave_type_id = leaves[i].leave_type_id,
                    leave_type_code = leaves[i].leave_type_code,
                    leave_name_en = leaves[i].leave_name_en,
                    leave_name_th =leaves[i].leave_name_th,
                    amount_entitlement = leaves[i].amount_entitlement,
                    used = (decimal)used_leave
                };
                useds = useds.GroupBy(g => g.leave_type_code).Select(s => new UsedLeaveModel()
                {
                    leave_type_code = s.Key,
                    leave_type_id = s.FirstOrDefault().leave_type_id,
                    leave_name_en = s.FirstOrDefault().leave_name_en,
                    leave_name_th= s.FirstOrDefault().leave_name_th.Split(' ')[0],
                    amount_entitlement = s.FirstOrDefault().amount_entitlement,
                    used = s.FirstOrDefault().used

                }).ToList();
                useds.Add(used);

            }
            
            return Json(useds);
        }

        [HttpGet]
        public IActionResult GetRequestHistory(string emp_id,string start , string stop)
        {
            DateTime _start = DateTime.Parse(start);
            DateTime _stop = DateTime.Parse(stop);
            List<RequestModel> requests = Requests.GetRequestByEmpID(emp_id);
            requests = requests.Where(w => w.start_request_date.Date >= _start.Date && w.start_request_date.Date <= _stop).ToList();
            requests = requests.OrderBy(o => o.start_request_date).ToList();
            return Json(requests);
        }
    }
}
