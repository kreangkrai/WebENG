using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;
using WebENG.LeaveServices;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class LeaveAuthenController : Controller
    {
        readonly IAccessory Accessory;
        readonly CTLInterfaces.IEmployee CTLEmployees;
        private IPosition Position;
        private IDepartment Department;
        private IApprover Approver;
        private IChecker Checker;
        public LeaveAuthenController()
        {
            Accessory = new AccessoryService();
            CTLEmployees = new CTLServices.EmployeeService();
            Position = new PositionService();
            Department = new DepartmentService();
            Approver = new ApproverService();
            Checker = new CheckerService();
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
            List<CTLModels.EmployeeModel> employees = CTLEmployees.GetEmployees();
            employees = employees.OrderBy(o => o.name_en).ToList();
            List<string> departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).OrderBy(o => o).ToList();
            var data = new { employees = employees, departments = departments };
            return Json(data);
        }


        [HttpGet]
        public IActionResult GetData(string department)
        {
            List<DepartmentModel> departments = Department.GetDepartments();
            departments = departments.Where(w => w.department == department).OrderBy(o => o.department).ToList();
            List<ApproverModel> approvers = Approver.GetApprovers();
            approvers = approvers.Where(w => w.department == department).OrderBy(o => o.department).ToList();
            List<CheckerModel> checkers = Checker.GetCheckers();
            var data = new { departments = departments, approvers = approvers, checkers = checkers };
            return Json(data);
        }

        [HttpPost]
        public IActionResult UpdateData(string department, List<DepartmentModel> approver_manager, List<ApproverModel> approver_director)
        {
            approver_manager.ForEach(f => {
                f.department = department;
                f.is_active = true;
                f.department_name = department;
                f.approver_level = 1;
            });
            approver_director.ForEach(f => {
                f.department = department;
                f.is_active = true;
                f.approver_level = 2;               
            });
        

            string message = "";
            message = Department.Delete(department);
            {
                if (message == "Success")
                {
                    message = Department.Inserts(approver_manager);
                    if (message == "Success")
                    {
                        message = Approver.Delete(department);
                        if (message == "Success")
                        {
                            message = Approver.Inserts(approver_director);
                        }
                    }
                }
            }
            return Json(message);
        }

    //    [HttpPost]
    //    public IActionResult CreateUser(string emp_id)
    //    {
    //        List<CTLModels.EmployeeModel> employees = CTLEmployees.GetEmployees();
    //        CTLModels.EmployeeModel employee = employees.Where(w => w.emp_id == emp_id).FirstOrDefault();
    //        List<LeavePositionModel> ps = Position.GetPositions();
    //        bool chk = ps.Any(a => a.emp_id == emp_id);
    //        if (!chk)
    //        {
    //            List<PositionModel> positions = new List<PositionModel>()
    //        {
    //            new PositionModel()
    //            {
    //                emp_id = emp_id,
    //                emp_name = employee.name_en,
    //                department = "",
    //                level = "",
    //                img = "",
    //                is_active = true,
    //                position = employee.position,
    //            }
    //        };
    //            string message = Position.insert(positions);
    //            return Json(message);
    //        }
    //        return Json("emp id already exists");
    //    }

    //    [HttpPost]
    //    public IActionResult UpdateUser(string emp_id,string[] manager_departments,bool director,bool auditor)
    //    {
    //        string message = Position.delete(emp_id);
    //        if (message == "Success")
    //        {
    //            // Manager Of Department
    //            List<PositionModel> positions = new List<PositionModel>();
    //            for (int i = 0; i < manager_departments.Length; i++)
    //            {
    //                PositionModel position = new PositionModel()
    //                {
    //                    emp_id = emp_id,
    //                    emp_name = "",
    //                    department = manager_departments[i],
    //                    level = "Manager",
    //                    img = "",
    //                    is_active = true,
    //                    position = "",
    //                };
    //                positions.Add(position);
    //            }

    //            //Director
    //            if (director)
    //            {
    //                PositionModel position = new PositionModel()
    //                {
    //                    emp_id = emp_id,
    //                    emp_name = "",
    //                    department = "",
    //                    level = "Director",
    //                    img = "",
    //                    is_active = true,
    //                    position = "",
    //                };
    //                positions.Add(position);
    //            }
    //            //Auditor
    //            if (auditor)
    //            {
    //                PositionModel position = new PositionModel()
    //                {
    //                    emp_id = emp_id,
    //                    emp_name = "",
    //                    department = "",
    //                    level = "Auditor",
    //                    img = "",
    //                    is_active = true,
    //                    position = "",
    //                };
    //                positions.Add(position);
    //            }
    //            message = Position.insert(positions);
    //        }
    //        return Json(message);
    //    }
    }
}
