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
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
/*

1 Step
Returned = Wait Edit and Resubmit
1. Created <==> Resubmit (ส่งใหม่)
2. Approved , Canceled , Rejected , Returned
3. Completed

2 Step
Returned = Wait Edit and Resubmit
1. Created <==> Resubmit (ส่งใหม่)
2. Pending , Canceled , Rejected , Returned
3. Approved , Canceled , Rejected , Returned
4. Completed
*/
namespace WebENG.Controllers
{
    public class LeaveController : Controller
    {
        readonly IAccessory Accessory;
        private IRequestLog RequestLog;
        readonly INotification Notification;
        readonly ILeaveType LeaveType;
        readonly ILeave Leave;
        private IEmployee Employee;
        readonly IRequest Requests;
        private IWorkingHours WorkingHoursService;
        readonly CTLInterfaces.IHoliday Holiday;
        private ILevel Level;
        private IMail Mail;
        private readonly IHostingEnvironment env;

        public LeaveController(IHostingEnvironment _env)
        {
            Accessory = new AccessoryService();
            RequestLog = new RequestLogService();
            Notification = new NotificationService();
            LeaveType = new LeaveTypeService();
            Leave = new LeaveService();
            Employee = new CTLServices.EmployeeService();
            Requests = new RequestService();
            WorkingHoursService = new WorkingHoursService();
            Holiday = new CTLServices.HolidayService();
            Level = new LevelService();
            Mail = new MailService();
            env = _env;

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
                "Completed",
                "Created"
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
            request.status_request = "Created";
            request.comment = "";
            request.amount_leave_hour = Math.Round((decimal)(request.end_request_time - request.start_request_time).TotalHours,0);

            LeaveTypeModel leaveType = LeaveType.GetLeaveTypeByID(request.leave_type_id);
            string leave_type_th = leaveType.leave_name_th;
            List<CTLModels.EmployeeModel> emps = Employee.GetEmployees();

            List<LevelModel> level = Level.GetLevelByEmpID(request.emp_id);
            int current_level = level.Min(m => m.level);
            List<LevelModel> next_level = level.Where(w => w.level == current_level + 1).ToList();

            // Check Two Step Approve
            bool is_full_day = request.is_full_day;
            if (is_full_day)
            {
                
                decimal over_consecutive_days_for_two_step = leaveType.over_consecutive_days_for_two_step;
                double diff_day = request.amount_leave_day;
                if (diff_day >= (double)over_consecutive_days_for_two_step)
                {
                    request.is_two_step_approve = true;
                }
                else
                {
                    request.is_two_step_approve = false;
                }
            }
            else
            {
                request.is_two_step_approve = false;
            }

            if (tempFileIds.Length > 0) // มีไฟล์แนบมา
            {
                request.path_file = request_id;
            }
            else
            {
                request.path_file = "";
            }
           
            level = level.Where(w => w.emp_id == request.emp_id).ToList();

            request.level_step = level.FirstOrDefault().level;

            List<RequestModel> requests = Requests.GetRequestByEmpID(request.emp_id);
            requests = requests.Where(w => w.status_request != "Canceled" && w.status_request != "Rejected").ToList();
            string message = "";
            if (!requests.Any(a=>a.start_request_date.Date == request.start_request_date.Date)) //  Check Date
            {
                var connect = new ConnectSQL();
                using (SqlConnection con = connect.OpenLeaveConnect())
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            var requestService = Requests;
                            var requestLogService = RequestLog;
                            requestService.Insert(request);

                            RequestLogModel requestLog = new RequestLogModel()
                            {
                                action_by = request.emp_id,
                                action_by_name = level.FirstOrDefault().emp_name_th,
                                action_by_level = level.FirstOrDefault().level,
                                old_status = "",
                                new_status = "Created",
                                comment = "",
                                old_level_step = -1,
                                new_level_step = level.FirstOrDefault().level,
                                request_id = request.request_id,
                                log_date = DateTime.Now
                            };
                            requestLogService.Insert(requestLog);

                            tran.Commit();
                            message = "Success";
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            message = $"Error {ex.Message}";
                        }
                    }
                }

                if (message == "Success")
                {
                    if (tempFileIds != null)
                    {
                        foreach (var tempId in tempFileIds)
                        {
                            var tempFolder = Path.Combine(env.WebRootPath,"Uploads", "temp", tempId);
                            var files = Directory.GetFiles(tempFolder);
                            foreach (var oldFile in files)
                            {
                                var fileName = Path.GetFileName(oldFile);
                                var newPath = Path.Combine(env.WebRootPath,
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

                    // Notification

                    for (int i = 0; i < next_level.Count; i++)
                    {
                        NotificationModel notification = new NotificationModel()
                        {
                            emp_id = next_level[i].emp_id,
                            notification_date = DateTime.Now,
                            notification_description = "สร้างใบลา",
                            notification_path = "Management",
                            notification_type = "Leave",
                            notification_issue = "ใบลารอการอนุมัติ",
                            status = "Pending"
                        };

                        Notification.Insert(notification);
                    }

                    // Send Mail
                    List<string> email_approvers = next_level.GroupBy(g => g.email).Select(s => s.FirstOrDefault().email).ToList();
                    string status = "สร้างใบลา";
                    string name = emps.Where(w=>w.emp_id == request.emp_id).Select(s=>s.name_th).FirstOrDefault();
                    string leave_type = leave_type_th;
                    string leave_date = "";
                    string leave_time = "";
                    if (request.is_full_day)
                    {
                        if (request.amount_leave_day > 1)
                        {
                            leave_date = $"{request.start_request_date.ToString("dd/MM/yyyy")} - {request.end_request_date.ToString("dd/MM/yyyy")}";
                        }
                        else
                        {
                            leave_date = request.start_request_date.ToString("dd/MM/yyyy");
                        }
                        leave_time = "08:30-17:30";
                    }
                    else
                    {
                        leave_date = request.start_request_date.ToString("dd/MM/yyyy");
                        leave_time = $"{request.start_request_time.ToString(@"hh\:mm")} - {request.end_request_time.ToString(@"hh\:mm")}";
                    }
                    Mail.Requester(email_approvers, status, name, leave_type, leave_date, leave_time);


                    //Insert Leave Working Hours
                    List<CTLModels.HolidayModel> holidays = Holiday.GetHolidays(request.start_request_date.Year.ToString());
                    List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
                    List<UserModel> users = Accessory.getAllUser();
                    for (DateTime date = request.start_request_date; date <= request.end_request_date; date = date.AddDays(1))
                    {
                        if (holidays.Any(a => a.date.Date != date.Date) && date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        {
                            if (request.is_full_day)
                            {
                                WorkingHoursModel wh = new WorkingHoursModel()
                                {
                                    user_id = users.Where(w => w.emp_id == request.emp_id).Select(s => s.user_id).FirstOrDefault(),
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
              /temp
         */
        [HttpPost]
        public async Task<IActionResult> UploadTempFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file");

            var tempId = Guid.NewGuid().ToString();
            var tempPath = Path.Combine(env.WebRootPath,"Uploads", "temp", tempId);
            Directory.CreateDirectory(tempPath);

            var fileName = Path.GetFileName(file.FileName);
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

            var filePath = Path.Combine(env.WebRootPath, "Uploads", "temp", tempId, fileName);

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

        [HttpDelete]
        public IActionResult DeleteTemp(string tempId)
        {
            var tempPath = Path.Combine(env.WebRootPath, "Uploads", "temp", tempId);
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
            return Ok();
        }
        [HttpGet]
        public IActionResult GetTempFileInfo(string tempId)
        {
            var tempPath = Path.Combine(env.WebRootPath, "Uploads", "temp", tempId);
            var file = Directory.GetFiles(tempPath).FirstOrDefault();
            if (file == null) return NotFound();

            var fileName = Path.GetFileName(file);
            var ext = Path.GetExtension(fileName).TrimStart('.').ToLower();
            return Json(new { fileName, fileType = ext });
        }
    }
}
