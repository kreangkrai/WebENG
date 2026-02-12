using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IAccessory Accessory;
        private ILeaveType LeaveType;
        private IRequest Requests;
        private ILeave Leave;
        private ILevel Level;
        private IRequestLog RequestLog;
        private readonly IHostingEnvironment env;
        private IMail Mail;
        private INotification Notification;
        private IWorkingHours WorkingHours;
        private CTLInterfaces.IHoliday Holiday;
        private CTLInterfaces.IEmployee Employee;

        public StatusLeaveController(IHostingEnvironment _env)
        {
            Accessory = new AccessoryService();
            LeaveType = new LeaveTypeService();
            Requests = new RequestService();
            Leave = new LeaveService();
            Level = new LevelService();
            RequestLog = new RequestLogService();
            env = _env;
            Mail = new MailService();
            Notification = new NotificationService();
            WorkingHours = new WorkingHoursService();
            Holiday = new CTLServices.HolidayService();
            Employee = new CTLServices.EmployeeService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");

                List<UserModel> users = Accessory.getAllUser();
                List<CTLModels.EmployeeModel> emps = Employee.GetEmployees();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).FirstOrDefault();
                if (u == null)
                {
                    List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
                    CTLModels.EmployeeModel employee = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();
                    u = new UserModel()
                    {
                        emp_id = employee.emp_id,
                        name = employee.name_en,
                        role = "User",
                        department = employee.department,
                    };
                }
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);

                List<int> years = new List<int>();
                for (int y = DateTime.Now.AddYears(1).Year; y > DateTime.Now.AddYears(-5).Year; y--)
                {
                    years.Add(y);
                }

                ViewBag.listYears = years;

                List<LevelModel> levels = Level.GetLevelByEmpID(u.emp_id);
                ViewBag.levels = levels;
                ViewBag.emp = u.emp_id;

                ViewBag.role = u.role;

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
        public IActionResult GetLeaveById(string leave_type_id, string emp_id, int year)
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
            for (int i = 0; i < requests.Count; i++)
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

        [HttpGet]
        public IActionResult GetEmployee(string start, string stop)
        {
            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            string emp_id = employees.Where(w => w.name_en.ToLower() == user.ToLower()).Select(s => s.emp_id).FirstOrDefault();
            List<LevelModel> levels = Level.GetLevelByEmpID(emp_id);
            int max_level = levels.Where(w => w.emp_id == emp_id).Max(m => m.level);
            List<string> departments = new List<string>();
            if (max_level == 0)
            {
                departments = levels.Where(w => w.emp_id == emp_id && w.level == max_level).GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).ToList();
            }
            if (max_level == 1 || max_level == 2)
            {
                departments = levels.Where(w => w.emp_id == emp_id).GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).ToList();
            }
            else if (max_level == 3)
            {
                departments = employees.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).ToList();
            }

            departments = departments.OrderBy(o => o).ToList();

            List<RequestModel> requests = Requests.GetRequestByDurationDay(start, stop);
            List<string> emps = new List<string>();
            if (max_level == 0)
            {
                emps = requests.Where(g => g.emp_id == emp_id).Select(s => s.emp_id).ToList();
            }
            else
            {
                emps = requests.GroupBy(g => g.emp_id).Select(s => s.FirstOrDefault().emp_id).ToList();
            }

            employees = employees.Where(w => emps.Contains(w.emp_id) && departments.Contains(w.department)).OrderBy(o => o.name_en).ToList();
            var data = new { employees = employees, departments = departments };
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetLeaveUsedByEmpID(string emp_id, int year)
        {
            List<UsedLeaveModel> useds = new List<UsedLeaveModel>();
            List<string> status_pending = new List<string>()
            {
                "Created",
                "Pending",
                "Resubmit",
                "Approved",
                "Returned",
                "Completed"
            };
            List<LeaveTypeModel> leaves = LeaveType.GetLeaveTypes();
            List<CTLModels.EmployeeModel> emps = Employee.GetEmployees();
            List<RequestModel> requests = Requests.GetRequestByEmpID(emp_id);

            for (int i = 0; i < leaves.Count; i++)
            {
                List<RequestModel> _requests = requests.Where(w => w.leave_type_code == leaves[i].leave_type_code &&
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
                    leave_name_th = leaves[i].leave_name_th,
                    amount_entitlement = leaves[i].amount_entitlement,
                    used = (decimal)used_leave,
                    priority = leaves[i].priority
                };
                useds = useds.GroupBy(g => g.leave_type_code).Select(s => new UsedLeaveModel()
                {
                    leave_type_code = s.Key,
                    leave_type_id = s.FirstOrDefault().leave_type_id,
                    leave_name_en = s.FirstOrDefault().leave_name_en,
                    leave_name_th = s.FirstOrDefault().leave_name_th.Split(' ')[0],
                    amount_entitlement = s.FirstOrDefault().amount_entitlement,
                    used = s.FirstOrDefault().used,
                    priority = s.FirstOrDefault().priority
                }).ToList();
                useds.Add(used);
            }
            useds = useds.OrderBy(o => o.priority).ThenBy(t => t.leave_name_th).ToList();
            return Json(useds);
        }

        [HttpGet]
        public IActionResult GetDepartmentRequestHistory(string department, string start, string stop)
        {
            DateTime _start = DateTime.Parse(start);
            DateTime _stop = DateTime.Parse(stop);
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();

            List<RequestModel> requests = Requests.GetRequestByDepartment(department);
            requests = requests.Where(w => w.start_request_date.Date >= _start.Date && w.start_request_date.Date <= _stop).ToList();
            requests = requests.OrderBy(o => o.start_request_date).ToList();

            var data = requests.Select(s => new
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
                amount_leave_hour = s.amount_leave_hour,
                leave_name_th = s.leave_name_th,
                description = s.description,
                path_file = s.path_file,
                is_full_day = s.is_full_day,
                status_request = s.status_request,
                attachment_required = LeaveType.GetLeaveTypeByID(s.leave_type_id).attachment_required,
                attachment_threshold_days = LeaveType.GetLeaveTypeByID(s.leave_type_id).attachment_threshold_days,
                comment = s.comment,
                level_step = s.level_step,
                is_two_step_approve = s.is_two_step_approve
            }).ToList();

            data = data.OrderByDescending(o => o.start_request_date).ToList();
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetRequestHistory(string emp_id, string start, string stop)
        {
            DateTime _start = DateTime.Parse(start);
            DateTime _stop = DateTime.Parse(stop);
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();

            List<RequestModel> requests = Requests.GetRequestByEmpID(emp_id);
            requests = requests.Where(w => w.start_request_date.Date >= _start.Date && w.start_request_date.Date <= _stop).ToList();
            requests = requests.OrderBy(o => o.start_request_date).ToList();

            var data = requests.Select(s => new
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
                amount_leave_hour = s.amount_leave_hour,
                leave_name_th = s.leave_name_th,
                description = s.description,
                path_file = s.path_file,
                is_full_day = s.is_full_day,
                status_request = s.status_request,
                attachment_required = LeaveType.GetLeaveTypeByID(s.leave_type_id).attachment_required,
                attachment_threshold_days = LeaveType.GetLeaveTypeByID(s.leave_type_id).attachment_threshold_days,
                comment = s.comment,
                level_step = s.level_step,
                is_two_step_approve = s.is_two_step_approve
            }).ToList();

            data = data.OrderByDescending(o => o.start_request_date).ToList();
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetRequestLog(string request_id)
        {
            List<RequestLogModel> requests = RequestLog.GetLogByRequestId(request_id);

            return Json(requests);
        }

        [HttpGet]
        public IActionResult GetRequest(string request_id)
        {
            RequestModel request = Requests.GetRequestByID(request_id);
            LeaveTypeModel leaveType = LeaveType.GetLeaveTypeByID(request.leave_type_id);
            var data = new
            {
                request_id = request.request_id,
                leave_type_id = request.leave_type_id,
                emp_id = request.emp_id,
                request_date = request.request_date,
                start_request_date = request.start_request_date,
                end_request_date = request.end_request_date,
                start_request_time = request.start_request_time,
                end_request_time = request.end_request_time,
                amount_leave_day = request.amount_leave_day,
                amount_leave_hour = request.amount_leave_hour,
                leave_name_th = request.leave_name_th,
                description = request.description,
                path_file = request.path_file,
                is_full_day = request.is_full_day,
                status_request = request.status_request,
                attachment_required = leaveType.attachment_required,
                attachment_threshold_days = leaveType.attachment_threshold_days,
                comment = request.comment,
                level_step = request.level_step,
                is_two_step_approve = request.is_two_step_approve
            };
            return Json(data);
        }

        [HttpDelete]
        public IActionResult DeleteTemp(string request_id)
        {
            var tempPath = Path.Combine(env.WebRootPath, "Uploads", "temp", request_id);
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteTempFile(string request_id, string filename)
        {
            if (string.IsNullOrWhiteSpace(request_id) ||
                string.IsNullOrWhiteSpace(filename) ||
                request_id.Contains("..") || request_id.Contains("/") || request_id.Contains("\\") ||
                filename.Contains("..") || filename.Contains("/") || filename.Contains("\\"))
            {
                return BadRequest("Invalid characters in parameters");
            }

            var safeFileName = Path.GetFileName(filename);
            if (string.IsNullOrWhiteSpace(safeFileName) || safeFileName != filename)
            {
                return BadRequest("Invalid filename");
            }

            var tempPath = Path.Combine(env.WebRootPath, "Uploads", "temp", request_id, safeFileName);
            var fullPath = Path.GetFullPath(tempPath);
            var allowedRoot = Path.Combine(env.WebRootPath, "Uploads", "temp");
            allowedRoot = Path.GetFullPath(allowedRoot);
            //allowedRoot = allowedRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(allowedRoot + Path.DirectorySeparatorChar) &&
                !fullPath.StartsWith(allowedRoot + Path.AltDirectorySeparatorChar))
            {
                return BadRequest("Access denied");
            }

            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return Ok(new { success = true, deleted = fullPath });
                }

                return Ok(new { success = true, message = "File not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult EditRequest(string request_id, string str, bool is_attach_file, string text_btn_send)
        {
            DateTime now = DateTime.Now;
            RequestModel request = JsonConvert.DeserializeObject<RequestModel>(str);
            request.request_date = now;
            request.request_id = request_id;
            request.status_request = "Resubmit";
            request.comment = "";
            request.amount_leave_hour = Math.Round((decimal)(request.end_request_time - request.start_request_time).TotalHours, 0);

            int year = request.start_request_date.Year;

            if (text_btn_send == "แก้ไขวันลา")
            {
                List<LevelModel> level = Level.GetLevelByEmpID(request.emp_id);
                int current_level = level.Min(m => m.level);
                int next_level = current_level + 1;

                LeaveTypeModel leaveType = LeaveType.GetLeaveTypeByID(request.leave_type_id);
                string leave_type_th = leaveType.leave_name_th;

                List<CTLModels.EmployeeModel> emps = Employee.GetEmployees();

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

                if (is_attach_file) // มีไฟล์แนบมา
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
                string message = "";

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
                            requestService.Update(request);

                            RequestLogModel last_request_log = RequestLog.GetLogByRequestId(request_id).LastOrDefault();

                            RequestLogModel requestLog = new RequestLogModel()
                            {
                                action_by = request.emp_id,
                                action_by_name = level.FirstOrDefault().emp_name_th,
                                action_by_level = level.FirstOrDefault().level,
                                old_status = last_request_log.new_status,
                                new_status = "Canceled",
                                comment = "",
                                old_level_step = last_request_log.new_level_step,
                                new_level_step = level.FirstOrDefault().level,
                                request_id = request.request_id,
                                log_date = DateTime.Now
                            };
                            requestLogService.Insert(requestLog);

                            RequestLogModel requestLog_ = new RequestLogModel()
                            {
                                action_by = request.emp_id,
                                action_by_name = level.FirstOrDefault().emp_name_th,
                                action_by_level = level.FirstOrDefault().level,
                                old_status = "Canceled",
                                new_status = "Resubmit",
                                comment = "",
                                old_level_step = -1,
                                new_level_step = level.FirstOrDefault().level,
                                request_id = request.request_id,
                                log_date = DateTime.Now
                            };
                            requestLogService.Insert(requestLog_);

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
                    if (is_attach_file)
                    {
                        //Remove Uploads Request id

                        RemoveUploadsFiles(request.leave_type_id, year, request_id);

                        var tempFolder = Path.Combine(env.WebRootPath, "Uploads", "temp", request_id);
                        var files = Directory.GetFiles(tempFolder);
                        foreach (var oldFile in files)
                        {
                            var fileName = Path.GetFileName(oldFile);
                            var newPath = Path.Combine(env.WebRootPath,
                                "Uploads", request.leave_type_id,
                                year.ToString(),
                                request_id,
                                fileName
                            );
                            Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                            System.IO.File.Move(oldFile, newPath);
                        }
                        Directory.Delete(tempFolder, true);
                    }
                    else
                    {
                        RemoveUploadsFiles(request.leave_type_id, year, request.request_id);
                    }

                    // Notification
                    List<LevelModel> level_approves = Level.GetLevelByDepartment(level.FirstOrDefault().department);
                    level_approves = level_approves.Where(w => w.level == next_level).ToList();
                    for (int i = 0; i < level_approves.Count; i++)
                    {
                        NotificationModel notification = new NotificationModel()
                        {
                            emp_id = level_approves[i].emp_id,
                            notification_date = DateTime.Now,
                            notification_description = "ขออนุมัติการแก้ไขใบลา",
                            notification_path = "Management",
                            notification_type = "Leave",
                            notification_issue = "ใบลารอการอนุมัติ",
                            status = "Pending"
                        };

                        Notification.Insert(notification);
                    }

                    // Send Mail
                    List<string> email_approvers = level_approves.GroupBy(g => g.email).Select(s => s.FirstOrDefault().email).ToList();
                    string status = "ขออนุมัติการแก้ไขใบลา";
                    CTLModels.EmployeeModel name = emps.Where(w => w.emp_id == request.emp_id).FirstOrDefault();
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

                    //Delete Working Hours
                    WorkingHoursModel _wh = WorkingHours.GetWorkingHourByLeave(request.emp_id, request.start_request_date.ToString("yyyy-MM-dd"));

                    WorkingHours.DeleteWorkingHours(_wh);

                    //Insert Leave Working Hours
                    List<CTLModels.HolidayModel> holidays = Holiday.GetHolidays(request.start_request_date.Year.ToString());
                    List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
                    for (DateTime date = request.start_request_date; date <= request.end_request_date; date = date.AddDays(1))
                    {
                        if (holidays.Any(a => a.date.Date != date.Date) && date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        {
                            if (request.is_full_day)
                            {
                                WorkingHoursModel wh = new WorkingHoursModel()
                                {
                                    emp_id = request.emp_id,
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
                                    emp_id = request.emp_id,
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
                if (is_attach_file)
                {
                    //Remove Uploads Request id

                    RemoveUploadsFiles(request.leave_type_id, year, request_id);

                    var tempFolder = Path.Combine(env.WebRootPath, "Uploads", "temp", request_id);
                    var files = Directory.GetFiles(tempFolder);
                    foreach (var oldFile in files)
                    {
                        var fileName = Path.GetFileName(oldFile);
                        var newPath = Path.Combine(env.WebRootPath,
                            "Uploads", request.leave_type_id,
                            year.ToString(),
                            request_id,
                            fileName
                        );
                        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                        System.IO.File.Move(oldFile, newPath);
                    }
                    Directory.Delete(tempFolder, true);

                    //Update File Path
                    string path_file = request.request_id;
                    Requests.UpdateFilePath(request.request_id, path_file);
                }
                else
                {
                    RemoveUploadsFiles(request.leave_type_id, year, request.request_id);
                }
                return Json("Success");
            }
        }

        public string AddWorkingHours(List<WorkingHoursModel> whs)
        {
            try
            {
                for (int i = 0; i < whs.Count; i++)
                {
                    int id = WorkingHours.GetLastWorkingHoursID() + 1;
                    string wh_id = "WH" + id.ToString().PadLeft(6, '0');

                    whs[i].index = wh_id;
                    var result = WorkingHours.AddWorkingHours(whs[i]);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpDelete]
        public IActionResult DeleteRequest(string request_id, string emp_id)
        {
            List<LevelModel> level = Level.GetLevelByEmpID(emp_id);
            level = level.Where(w => w.emp_id == emp_id).ToList();

            RequestModel request = Requests.GetRequestByID(request_id);
            request.status_request = "Canceled";
            request.comment = "";
            request.path_file = "";
            request.end_request_time = new TimeSpan(request.end_request_time.Hours, request.end_request_time.Minutes, request.end_request_time.Seconds);
            request.start_request_time = new TimeSpan(request.start_request_time.Hours, request.start_request_time.Minutes, request.start_request_time.Seconds);
            string message = "";
            int year = request.start_request_date.Year;

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
                        requestService.Update(request);
                        List<RequestLogModel> requests_log = RequestLog.GetLogByRequestId(request_id);
                        RequestLogModel first_request_log = requests_log.FirstOrDefault();
                        RequestLogModel last_request_log = requests_log.LastOrDefault();

                        RequestLogModel requestLog = new RequestLogModel()
                        {
                            action_by = request.emp_id,
                            action_by_name = level.FirstOrDefault().emp_name_th,
                            action_by_level = level.FirstOrDefault().level,
                            old_status = last_request_log.new_status,
                            new_status = "Canceled",
                            comment = "",
                            old_level_step = last_request_log.new_level_step,
                            new_level_step = first_request_log.new_level_step,
                            request_id = request.request_id,
                            log_date = DateTime.Now
                        };
                        requestLogService.Insert(requestLog);

                        tran.Commit();
                        message = "Success";

                        //Remove File Uploads

                        RemoveTempFiles(request.request_id);
                        RemoveUploadsFiles(request.leave_type_id, year, request.request_id);
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        message = $"Error {ex.Message}";
                    }
                    finally
                    {
                        if (message == "Success")
                        {
                            //Delete Working Hours
                            WorkingHoursModel wh = WorkingHours.GetWorkingHourByLeave(request.emp_id, request.start_request_date.ToString("yyyy-MM-dd"));

                            WorkingHours.DeleteWorkingHours(wh);
                        }
                    }
                }
            }
            return Json(message);
        }

        [HttpDelete]
        public IActionResult CheckerDeleteRequest(string request_id, string emp_id)
        {
            List<LevelModel> level = Level.GetLevelByEmpID(emp_id);
            level = level.Where(w => w.emp_id == emp_id).ToList();

            RequestModel request = Requests.GetRequestByID(request_id);
            request.status_request = "Canceled";
            request.comment = "";
            request.path_file = "";
            request.end_request_time = new TimeSpan(request.end_request_time.Hours, request.end_request_time.Minutes, request.end_request_time.Seconds);
            request.start_request_time = new TimeSpan(request.start_request_time.Hours, request.start_request_time.Minutes, request.start_request_time.Seconds);
            string message = "";
            int year = request.start_request_date.Year;

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
                        requestService.Update(request);
                        List<RequestLogModel> requests_log = RequestLog.GetLogByRequestId(request_id);
                        RequestLogModel first_request_log = requests_log.FirstOrDefault();
                        RequestLogModel last_request_log = requests_log.LastOrDefault();

                        RequestLogModel requestLog = new RequestLogModel()
                        {
                            action_by = request.emp_id,
                            action_by_name = level.FirstOrDefault().emp_name_th,
                            action_by_level = level.LastOrDefault().level,
                            old_status = last_request_log.new_status,
                            new_status = "Canceled",
                            comment = "",
                            old_level_step = last_request_log.new_level_step,
                            new_level_step = first_request_log.new_level_step,
                            request_id = request.request_id,
                            log_date = DateTime.Now
                        };
                        requestLogService.Insert(requestLog);

                        tran.Commit();
                        message = "Success";

                        //Remove File Uploads

                        RemoveTempFiles(request.request_id);
                        RemoveUploadsFiles(request.leave_type_id, year, request.request_id);
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        message = $"Error {ex.Message}";
                    }
                    finally
                    {
                        if (message == "Success")
                        {
                            //Delete Working Hours
                            WorkingHoursModel wh = WorkingHours.GetWorkingHourByLeave(request.emp_id, request.start_request_date.ToString("yyyy-MM-dd"));

                            WorkingHours.DeleteWorkingHours(wh);
                        }
                    }
                }
            }
            return Json(message);
        }

        [HttpPost]
        public IActionResult CopyUplodstoTempFile(string leave_type_id, int year, string request_id)
        {
            string sourceFolder = Path.Combine(env.WebRootPath, "Uploads", leave_type_id, year.ToString(), request_id);
            string tempFolder = Path.Combine(env.WebRootPath, "Uploads", "temp", request_id);

            if (!Directory.Exists(sourceFolder))
            {
                return Json("Error");
            }

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            var filePaths = Directory.GetFiles(sourceFolder);
            var fileNames = filePaths.Select(Path.GetFileName);

            foreach (var oldFilePath in filePaths)
            {
                string fileName = Path.GetFileName(oldFilePath);

                string newFilePath = Path.Combine(tempFolder, fileName);

                if (System.IO.File.Exists(newFilePath))
                {
                    continue;
                }
                try
                {
                    System.IO.File.Copy(oldFilePath, newFilePath);
                }
                catch
                {
                    return Json("Error");
                }
            }
            return Json("Success");
        }

        [HttpPost]
        public IActionResult MoveTempBackToRequest(string leaveId, int year, string requestId)
        {
            try
            {
                var basePath = Path.Combine(Directory.GetCurrentDirectory(), env.WebRootPath, "Uploads");

                string sourceFolder = Path.Combine(basePath, "temp", requestId);

                string targetFolder = Path.Combine(basePath, leaveId, year.ToString(), requestId);

                if (!Directory.Exists(sourceFolder))
                {
                    return Json("ไม่พบไฟล์ใน temp สำหรับ request นี้");
                }

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                var oldFiles = Directory.GetFiles(targetFolder);
                foreach (var file in oldFiles)
                {
                    System.IO.File.Delete(file);
                }

                var filesToMove = Directory.GetFiles(sourceFolder);
                int movedCount = 0;

                foreach (var tempFilePath in filesToMove)
                {
                    string fileName = Path.GetFileName(tempFilePath);
                    string destPath = Path.Combine(targetFolder, fileName);

                    System.IO.File.Move(tempFilePath, destPath);
                    movedCount++;
                }

                if (Directory.Exists(sourceFolder) && !Directory.EnumerateFileSystemEntries(sourceFolder).Any())
                {
                    Directory.Delete(sourceFolder);
                }

                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult RemoveTempFiles(string requestId)
        {
            try
            {
                var basePath = Path.Combine(Directory.GetCurrentDirectory(), env.WebRootPath, "Uploads");
                var requestTempFolder = Path.Combine(basePath, "temp", requestId);

                if (Directory.Exists(requestTempFolder))
                {
                    Directory.Delete(requestTempFolder, true);
                }

                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult RemoveUploadsFiles(string leaveId, int year, string requestId)
        {
            try
            {
                var basePath = Path.Combine(Directory.GetCurrentDirectory(), env.WebRootPath, "Uploads");
                string targetFolder = Path.Combine(basePath, leaveId, year.ToString(), requestId);
                if (!Directory.Exists(targetFolder))
                {
                    return Json("ไม่พบโฟลเดอร์ หรือไม่มีไฟล์ให้ลบ");
                }
                var files = Directory.GetFiles(targetFolder);

                foreach (var file in files)
                {
                    System.IO.File.Delete(file);
                }

                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadTempFile(IFormFile file, string request_id)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file");

            var tempId = request_id;
            var tempPath = Path.Combine(env.WebRootPath, "Uploads", "temp", tempId);
            Directory.CreateDirectory(tempPath);

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(tempPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var previewUrl = Url.Action(
                "PreviewTempFile",
                "StatusLeave",
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
        public IActionResult GetFiles(string leave_type_id, string request_id, int year)
        {
            List<FileModel> files = new List<FileModel>();
            var tempFolder = Path.Combine(env.WebRootPath, "Uploads", leave_type_id, year.ToString(), request_id);

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
                    string type = GetFileType(relativePath);
                    var previewUrl = Url.Action(
                        "PreviewFile",
                        "StatusLeave",
                        new { filename = relativePath, leave_type_id = leave_type_id, year = year, request_id = request_id },
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
        public IActionResult GetTempFileInfo(string leave_type_id, int year, string request_id)
        {
            // Copy Uploads to Temp
            CopyUplodstoTempFile(leave_type_id, year, request_id);

            var tempPath = Path.Combine(env.WebRootPath, "Uploads", "temp", request_id);

            if (!Directory.Exists(tempPath))
                return NotFound("ไม่พบโฟลเดอร์ temp ของ request_id นี้");

            var files = Directory.GetFiles(tempPath);

            if (!files.Any())
                return NotFound("ไม่พบไฟล์ในโฟลเดอร์ temp");

            var fileInfos = files.Select(file =>
            {
                var fileName = Path.GetFileName(file);
                var previewUrl = Url.Action(
                    "PreviewTempFile",
                    "StatusLeave",
                    new { tempId = request_id, fileName },
                    Request.Scheme
                );

                return new
                {
                    request_id,
                    fileName,
                    fileType = GetMimeType(fileName),
                    previewUrl
                };
            }).ToList();

            return Json(new
            {
                request_id,
                files = fileInfos
            });
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

        [HttpGet]
        public IActionResult PreviewFile(string fileName, string leave_type_id, int year, string request_id)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest();

            if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\") ||
                fileName.Contains(":") || Path.IsPathRooted(fileName))
                return BadRequest("Invalid file name");

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), env.WebRootPath, "Uploads", leave_type_id, year.ToString(), request_id);

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

        public async Task ExportToText(string start, string stop)
        {
            List<CTLModels.HolidayModel> holidays = Holiday.GetHolidays(Convert.ToDateTime(start).Year.ToString());
            List<RequestModel> requests = Requests.GetRequestByDurationDay(start, stop);
            requests = requests.Where(w => w.status_request == "Completed").OrderBy(o => o.emp_id).ToList();
            StringBuilder sb = new StringBuilder();
            string headers = string.Empty;
            headers += "รหัสพนักงาน    วันที่ลา ตามวันลาจริง    รหัสกะ   รหัสผลข้อตกลงเงินหัก    รหัสลักษณะการรูดบัตร    วิธีลา    จำนวนที่ลา" + Environment.NewLine;
            sb.Append(headers);

            List<RequestModel> new_requests = new List<RequestModel>();
            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].is_full_day)
                {
                    if (requests[i].amount_leave_day > 1)
                    {
                        for (DateTime date = requests[i].start_request_date; date <= requests[i].end_request_date; date = date.AddDays(1))
                        {
                            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday && !holidays.Any(a => a.date.Date == date.Date))
                            {
                                RequestModel request = new RequestModel()
                                {
                                    emp_id = requests[i].emp_id,
                                    leave_name_th = requests[i].leave_name_th,
                                    start_request_date = date,
                                    end_request_date = date,
                                    amount_leave_day = 1,
                                    start_request_time = new TimeSpan(8, 30, 0),
                                    end_request_time = new TimeSpan(17, 30, 0),
                                };

                                new_requests.Add(request);
                            }
                        }
                    }
                    else
                    {
                        RequestModel request = new RequestModel()
                        {
                            emp_id = requests[i].emp_id,
                            leave_name_th = requests[i].leave_name_th,
                            start_request_date = requests[i].start_request_date,
                            end_request_date = requests[i].end_request_date,
                            amount_leave_day = 1,
                            start_request_time = new TimeSpan(8, 30, 0),
                            end_request_time = new TimeSpan(17, 30, 0),
                        };

                        new_requests.Add(request);
                    }
                }
                else
                {
                    new_requests.Add(requests[i]);
                }
            }

            requests = new_requests;

            foreach (var request in requests)
            {
                string row = string.Empty;
                row += string.Format($"{request.emp_id}    {request.start_request_date.ToString("yyyyMMdd")}    00    {request.leave_type_code}    0    1    {request.amount_leave_day}") + Environment.NewLine;

                sb.Append(row);
            }
            byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());

            byte[] bom = new byte[] { 0xEF, 0xBB, 0xBF };
            byte[] finalBytes = bom.Concat(byteArray).ToArray();

            string fileName = $"LeaveExport_{DateTime.Now:yyyyMMddHHmmss}.txt";

            Response.Clear();
            Response.Headers.Clear();

            Response.ContentType = "text/plain; charset=utf-8";
            Response.Headers["Content-Disposition"] = $"attachment; filename=\"{fileName}\"";
            await Response.Body.WriteAsync(finalBytes);
            await Response.Body.FlushAsync();
        }
    }
}