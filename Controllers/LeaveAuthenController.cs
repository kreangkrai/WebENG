using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        private IDepartment Department;
        private IApprover Approver;
        private IChecker Checker;
   
        public LeaveAuthenController()
        {
            Accessory = new AccessoryService();
            CTLEmployees = new CTLServices.EmployeeService();
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
            var data = new { departments = departments, approvers = approvers };
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetDataChecker()
        {
            List<CheckerModel> checkers = Checker.GetCheckers();
            var data = new {checkers = checkers };
            return Json(data);
        }

        [HttpPost]
        public IActionResult UpdateData(string department, List<DepartmentModel> approver_manager, List<ApproverModel> approver_director)
        {
            if (string.IsNullOrWhiteSpace(department)) {
                return Json("Invalid department");
            }

            approver_manager.ForEach(f => {
                f.department = department;
                f.is_active = true;
                f.department_name = department;
                f.level = 1;
            });
            approver_director.ForEach(f => {
                f.department = department;
                f.is_active = true;
                f.level = 2;               
            });
        
            var connect = new ConnectSQL();
            using (SqlConnection con = connect.OpenLeaveConnect())
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        var deptService = new DepartmentService();
                        var apprService = new ApproverService();

                        deptService.Delete(department, tran);
                        deptService.Inserts(approver_manager, tran);

                        apprService.Delete(department, tran);
                        apprService.Inserts(approver_director, tran);

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

        [HttpPost]
        public IActionResult UpdateDataChecker(List<CheckerModel> approver_checker)
        {
            approver_checker.ForEach(f => {
                f.is_active = true;
                f.level = 3;
            });

            var connect = new ConnectSQL();
            using (SqlConnection con = connect.OpenLeaveConnect())
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        var checkerService = Checker;
                        checkerService.Delete();
                        checkerService.Inserts(approver_checker);
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
    }
}
