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
using Newtonsoft.Json;
using System.IO;
/*

Pending , Resubmit => Operation, Manager
Cancelled , Approved, Rejected ,Returned
Successed

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
        private IWorkingHours WorkingHoursService;
        readonly CTLInterfaces.IHoliday Holiday;
        public LeaveController()
        {
            Accessory = new AccessoryService();
            Hierarchy = new HierarchyService();
            Notification = new NotificationService();
            LeaveType = new LeaveTypeService();
            Leave = new LeaveService();
            Employee = new CTLServices.EmployeeService();
            Requests = new RequestService();
            WorkingHoursService = new WorkingHoursService();
            Holiday = new CTLServices.HolidayService();

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
                List<HierarchyDepartmentModel> hierarchies_depaartment = Hierarchy.GetDepartmentHierarchies();

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

        public string AddWorkingHours(List<WorkingHoursModel> whs)
        {
            try
            {
                for (int i = 0; i < whs.Count; i++)
                {
                    int id = WorkingHoursService.GetLastWorkingHoursID() + 1;
                    string wh_id = "WH" + id.ToString().PadLeft(6, '0');

                    whs[i].index = wh_id;
                    var result = WorkingHoursService.AddWorkingHours(whs[i]);
                }
                return "Success";
            }
            catch(Exception ex)
            {
                return ex.Message;
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
        public IActionResult GetLeaveById(string leave_type_id, string emp_id,int year)
        {
            List<string> status_pending = new List<string>()
            {
                "Pending",
                "Resubmit",
                "Approved",
                "Returned",
                "Successed"
            };
            LeaveTypeModel leave = LeaveType.GetLeaveTypeByID(leave_type_id);
            string leave_type_code = leave.leave_type_code;
            List<CTLModels.EmployeeModel> emps = Employee.GetEmployees();
            List<RequestModel> requests = Requests.GetRequestByEmpID(emp_id);
            requests = requests.Where(w => w.leave_type_code == leave_type_code &&
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
            if (leave.calculate_auto == true)
            {               
                double _leave = Leave.CalculateLeaveDays(emp, year, 6, 10, 10, 12);
                leave.amount_entitlement = (decimal)_leave;
            }
            double used_leave = 0;
            for(int i = 0; i < requests.Count; i++)
            {
                if (requests[i].is_full_day)
                {
                    used_leave += requests[i].amount_leave_day;
                }
                else
                {
                    used_leave += Math.Round(((double)requests[i].amount_leave_hour) / 8.0, 2);
                }
            }
            double balance = (double)leave.amount_entitlement - used_leave;
            var data = new { leave = leave, balance = balance, gender = em.gender, hire_date = em.start_date };
            return Json(data);
        }

        public string ConvertNameToUserID(string name)
        {
            string first = name.Split(' ')[0];
            string last = name.Split(' ')[0].Substring(0,1).ToUpper();
            return $"{first}.{last}";
        }
        [HttpPost]
        public IActionResult CreateRequest(string str,string[] tempFileIds)
        {
            DateTime now = DateTime.Now;           
            RequestModel request = JsonConvert.DeserializeObject<RequestModel>(str);
            string request_id = $"{request.emp_id}_{now.ToString("yyyyMMddHHmmss")}";
            request.request_date = now;
            request.request_id = request_id;
            request.amount_leave_hour = Math.Round((decimal)(request.end_request_time - request.start_request_time).TotalHours,0);
            request.path_file = request_id;
            List<RequestModel> requests = Requests.GetRequestByEmpID(request.emp_id);
            if (!requests.Any(a=>a.start_request_date.Date == request.start_request_date.Date)) //  Check Date
            {
                string message = Requests.Insert(request);
                if (message == "Success")
                {
                    if (tempFileIds != null)
                    {
                        foreach (var tempId in tempFileIds)
                        {
                            var tempFolder = Path.Combine("Uploads", "temp", tempId);
                            var files = Directory.GetFiles(tempFolder);
                            foreach (var oldFile in files)
                            {
                                var fileName = Path.GetFileName(oldFile);
                                var newPath = Path.Combine(
                                    "Uploads", request.leave_type_id,
                                    now.Year.ToString(),
                                    request_id,
                                    fileName
                                );
                                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                                System.IO.File.Move(oldFile, newPath);
                            }
                            Directory.Delete(tempFolder, true);
                        }
                    }

                    //Insert Leave Working Hours
                    List<CTLModels.HolidayModel> holidays = Holiday.GetHolidays(request.start_request_date.Year.ToString());
                    List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
                    List<UserModel> users = Accessory.getAllUser();
                    for(DateTime date = request.start_request_date; date <= request.end_request_date; date = date.AddDays(1))
                    {
                        if (holidays.Any(a => a.date.Date != date.Date) && date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        {
                            if (request.is_full_day)
                            {
                                WorkingHoursModel wh = new WorkingHoursModel()
                                {
                                    user_id = users.Where(w=>w.emp_id == request.emp_id).Select(s=>s.user_id).FirstOrDefault(),
                                    working_date = date.Date,
                                    job_id = "J999999",
                                    task_id = "T002",
                                    start_time = new TimeSpan(8, 30, 0),
                                    stop_time = new TimeSpan(17, 30, 0),
                                    lunch_full = false,
                                    lunch_half = false,
                                    dinner_full = false,
                                    dinner_half = false,
                                    note = "",
                                };
                                whs.Add(wh);
                            }
                            else
                            {
                                WorkingHoursModel wh = new WorkingHoursModel()
                                {
                                    user_id = users.Where(w => w.emp_id == request.emp_id).Select(s => s.user_id).FirstOrDefault(),
                                    working_date = date.Date,
                                    job_id = "J999999",
                                    task_id = "T002",
                                    start_time = request.start_request_time,
                                    stop_time = request.end_request_time,
                                    lunch_full = false,
                                    lunch_half = false,
                                    dinner_full = false,
                                    dinner_half = false,
                                    note = "",
                                };
                                whs.Add(wh);
                            }
                        }
                    }
                    message = AddWorkingHours(whs);
                }
                return Json(message);
            }
            else
            {
                return Json("ใช้สิทธิ์วันลาไปแล้ว");
            }  
        }

        /*
          /Uploads
               /leave_id {guid}
                 /year
                     /request_id {emp_id_yyyyMMddHHmmss}
                          /xxxx.csv
         */
        //[HttpPost]
        //public async Task<IActionResult> UploadTempFile(IFormFile file)
        //{
        //    if (file == null || file.Length == 0) return BadRequest("No file");

        //    var tempId = Guid.NewGuid().ToString();
        //    var tempPath = Path.Combine("Uploads", "temp", tempId);
        //    Directory.CreateDirectory(tempPath);

        //    var filePath = Path.Combine(tempPath, file.FileName);
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }
        //    return Json(new { tempId });
        //}
        [HttpPost]
        public async Task<IActionResult> UploadTempFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file");

            var tempId = Guid.NewGuid().ToString();
            var tempPath = Path.Combine("Uploads", "temp", tempId);
            Directory.CreateDirectory(tempPath);

            var fileName = Path.GetFileName(file.FileName); // ป้องกัน path traversal
            var filePath = Path.Combine(tempPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var previewUrl = Url.Action(
                "PreviewTempFile",
                "Leave",
                new { tempId, fileName },
                Request.Scheme
            );

            return Json(new
            {
                tempId,
                fileName,
                fileType = GetFileType(fileName),
                previewUrl
            });
        }
        private string GetFileType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            switch (ext)
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".webp":
                    return "image/webp";
                default:
                    return "application/octet-stream";
            }
        }

        [HttpGet]
        public IActionResult PreviewTempFile(string tempId, string fileName)
        {
            if (string.IsNullOrEmpty(tempId) || string.IsNullOrEmpty(fileName))
                return BadRequest();

            if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                return BadRequest("Invalid file name");

            var filePath = Path.Combine("Uploads", "temp", tempId, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var mimeType = GetMimeType(fileName);
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            Response.Headers.Add("Content-Disposition", "inline");

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
        private string GetImageMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            switch (ext)
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".webp":
                    return "image/webp";
                default:
                    return "application/octet-stream";
            }
        }

        [HttpDelete]
        public IActionResult DeleteTemp(string tempId)
        {
            var tempPath = Path.Combine("Uploads", "temp", tempId);
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
            return Ok();
        }
        [HttpGet]
        public IActionResult GetTempFileInfo(string tempId)
        {
            var tempPath = Path.Combine("Uploads", "temp", tempId);
            var file = Directory.GetFiles(tempPath).FirstOrDefault();
            if (file == null) return NotFound();

            var fileName = Path.GetFileName(file);
            var ext = Path.GetExtension(fileName).TrimStart('.').ToLower();
            return Json(new { fileName, fileType = ext });
        }
    }
}
