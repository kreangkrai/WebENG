using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.CTLInterfaces;
using WebENG.CTLServices;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class StatusLeaveController : Controller
    {
        readonly IAccessory Accessory;
        readonly IHierarchy Hierarchy;
        readonly IEmployee Employee;
        public StatusLeaveController()
        {
            Accessory = new AccessoryService();
            Hierarchy = new HierarchyService();
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
    }
}
