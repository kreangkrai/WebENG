﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class TargetController : Controller
    {
        readonly IAccessory Accessory;
        readonly ITarget Target;
        public TargetController()
        {
            Accessory = new AccessoryService();
            Target = new TargetService();
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { name = s.name, department = s.department, role = s.role, user_id = s.user_id }).FirstOrDefault();
                HttpContext.Session.SetString("Role", u.role);
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public JsonResult GetTargetProject(int year)
        {
            List<TargetModel> targets = Target.getData(year,"Project");
            return Json(targets);
        }

        [HttpGet]
        public JsonResult GetTargetService(int year)
        {
            List<TargetModel> targets = Target.getData(year, "Service");
            return Json(targets);
        }

        [HttpGet]
        public JsonResult GetTargetENGInvoice(int year)
        {
            List<TargetModel> targets = Target.getData(year, "Invoice");
            return Json(targets);
        }

        [HttpPost]
        public JsonResult AddProjectTarget(int year)
        {
            List<TargetModel> targets = new List<TargetModel>();
            for (int i = 1; i <= 12; i++)
            {
                targets.Add(new TargetModel()
                {
                    month = new DateTime(year,i,1).ToString("yyyy-MM"),
                    target = 0
                });
            }
            
            string result = Target.Insert(targets, "Project");
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddServiceTarget(int year)
        {
            List<TargetModel> targets = new List<TargetModel>();
            for (int i = 1; i <= 12; i++)
            {
                targets.Add(new TargetModel()
                {
                    month = new DateTime(year, i, 1).ToString("yyyy-MM"),
                    target = 0
                });
            }

            string result = Target.Insert(targets, "Service");
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddENGInvoiceTarget(int year)
        {
            List<TargetModel> targets = new List<TargetModel>();
            for (int i = 1; i <= 12; i++)
            {
                targets.Add(new TargetModel()
                {
                    month = new DateTime(year, i, 1).ToString("yyyy-MM"),
                    target = 0
                });
            }

            string result = Target.Insert(targets, "Invoice");
            return Json(result);
        }

        [HttpPatch]
        public JsonResult UpdateProjectTarget(string datas)
        {
            List<TargetModel> targets = JsonConvert.DeserializeObject<List<TargetModel>>(datas);
            string result = Target.Update(targets, "Project");
            return Json(result);
        }

        [HttpPatch]
        public JsonResult UpdateServiceTarget(string datas)
        {
            List<TargetModel> targets = JsonConvert.DeserializeObject<List<TargetModel>>(datas);
            string result = Target.Update(targets, "Service");
            return Json(result);
        }

        [HttpPatch]
        public JsonResult UpdateENGInvoiceTarget(string datas)
        {
            List<TargetModel> targets = JsonConvert.DeserializeObject<List<TargetModel>>(datas);
            string result = Target.Update(targets, "Invoice");
            return Json(result);
        }
    }
}
