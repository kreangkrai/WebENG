using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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

                string fileName = "J25-0203 AGC TANK MONITOR - AutoBackup.pdf";
                var previewUrl = Url.Action(
                "PreviewFile",
                "ManagementLeave",
                new { fileName },
                Request.Scheme
                );

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
                request_id = s.request_id,
                leave_type_id = s.leave_type_id,
                emp_id = s.emp_id,
                emp_name_en = employees.Where(w => w.emp_id == s.emp_id).Select(x => x.name_en).FirstOrDefault(),
                emp_name_th = employees.Where(w => w.emp_id == s.emp_id).Select(x => x.name_th).FirstOrDefault(),
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

        [HttpGet]
        public IActionResult GetFiles(string leave_type_id, string request_id,int year)
        {
            List<FileModel> files = new List<FileModel>();
            var tempFolder = Path.Combine("Uploads", leave_type_id, year.ToString(), request_id);

            if (!Directory.Exists(tempFolder))
                return Json(files);

            try
            {
                foreach (var fullPath in Directory.GetFiles(tempFolder, "*.*", SearchOption.AllDirectories))
                {
                    var fileNameOnly = Path.GetFileName(fullPath);
                    
                    var relativePath = Path.GetRelativePath(
                        Path.Combine(Directory.GetCurrentDirectory(), tempFolder),
                        fullPath).Replace("\\", "/");
                    string type = GetMimeType(relativePath);
                    var previewUrl = Url.Action(
                        "PreviewFile",
                        "ManagementLeave",
                        new { filename = relativePath , leave_type_id = leave_type_id,year = year , request_id = request_id },
                        Request.Scheme);

                    files.Add(new FileModel()
                    {
                        type = type,
                        filename = fileNameOnly,
                        path = previewUrl
                    });
                }
            }
            catch { }

            return Json(files);

        }

        [HttpGet]
        public IActionResult PreviewFile(string fileName, string leave_type_id, int year,string request_id)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest();

            if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\") ||
                fileName.Contains(":") || Path.IsPathRooted(fileName))
                return BadRequest("Invalid file name");

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", leave_type_id, year.ToString(), request_id);

            var fullPath = Path.GetFullPath(Path.Combine(basePath, fileName));

            if (!fullPath.StartsWith(basePath + Path.DirectorySeparatorChar))
                return BadRequest("Access denied");

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var mimeType = GetMimeType(fileName);
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            Response.Headers["Content-Disposition"] = "inline";
            return new FileStreamResult(stream, mimeType);
        }

        private string GetMimeType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "application/octet-stream";

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";

                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";

                case ".png":
                    return "image/png";

                case ".xls":
                    return "application/vnd.ms-excel";

                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                default:
                    return "application/octet-stream";
            }
        }
    }
}
