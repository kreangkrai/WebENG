using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class EngSkillController : Controller
    {
        readonly IAccessory Accessory;
        ISkill Skill;
        readonly CTLInterfaces.IEmployee Employees;
        public EngSkillController()
        {
            this.Accessory = new AccessoryService();
            this.Skill = new SkillService();
            Employees = new CTLServices.EmployeeService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { name = s.name, department = s.department, role = s.role, user_id = s.user_id, emp_id = s.emp_id }).FirstOrDefault();
                HttpContext.Session.SetString("Role", u.role);
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);

                List<CTLModels.EmployeeModel> emps = Employees.GetEmployees();
                if (!u.role.Contains("Admin"))
                {
                    string position = emps.Where(w => w.emp_id == u.emp_id).Select(s => s.position).FirstOrDefault();
                    u.role = position;
                }
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public List<EngSkillModel> GetSkills()
        {
            List<EngSkillModel> skills = Skill.GetSkills();
            return skills;
        }

        [HttpGet]
        public int GetLastSkillID()
        {
            int id = Skill.GetLastSkillID();
            return id;
        }

        [HttpPost]
        public JsonResult CreateSkill(string skill_str)
        {
            EngSkillModel skill = JsonConvert.DeserializeObject<EngSkillModel>(skill_str);
            var result = Skill.CreateSkill(skill);
            return Json(result);
        }

        [HttpPatch]
        public JsonResult EditSkill(string skill_str)
        {
            EngSkillModel skill = JsonConvert.DeserializeObject<EngSkillModel>(skill_str);
            var result = Skill.EditSkill(skill);
            return Json(result);
        }

        [HttpDelete]
        public JsonResult DeleteSkill(string skill_str)
        {
            EngSkillModel skill = JsonConvert.DeserializeObject<EngSkillModel>(skill_str);
            var result = Skill.DeleteSkill(skill);
            return Json(result);
        }
    }
}
