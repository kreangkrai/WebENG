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
namespace WebENG.Controllers
{
    public class LeaveSettingController : Controller
    {
        readonly IAccessory Accessory;
        readonly IHierarchy Hierarchy;
        private ILeaveType LeaveType;
        public LeaveSettingController()
        {
            Accessory = new AccessoryService();
            Hierarchy = new HierarchyService();
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
                    user_id = s.user_id
                }).FirstOrDefault();
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);

                List<HierarchyPersonalModel> hierarchies_personal = Hierarchy.GetPersonalHierarchies();
                List<HierarchyDepartmentModel> hierarchies_depaartmenr = Hierarchy.GetDepartmentHierarchies();

                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public IActionResult GetLeaveGroup()
        {
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            return Json(leaves);
        }
    }
}
