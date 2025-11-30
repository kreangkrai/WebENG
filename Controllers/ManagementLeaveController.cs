using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

//            1 Step
//Returned = Wait Edit and Resubmit
//1.Created <==> Resubmit(ส่งใหม่)
//2.Approved , Canceled , Rejected , Returned
//3.Completed

//2 Step
//Returned = Wait Edit and Resubmit
//1.Created <==> Resubmit(ส่งใหม่)
//2.Pending , Canceled , Rejected , Returned
//3.Approved , Canceled , Rejected , Returned
//4.Completed
//
namespace WebENG.Controllers
{
    public class ManagementLeaveController : Controller
    {
        readonly IAccessory Accessory;
        readonly IRequest Requests;
        readonly IEmployee Employee;
        readonly ILevel Level;
        readonly ILeaveType LeaveType;
        private IRequestLog RequestLog;
        public ManagementLeaveController()
        {
            Accessory = new AccessoryService();
            Requests = new RequestService();
            Employee = new EmployeeService();
            Level = new LevelService();
            LeaveType = new LeaveTypeService();
            RequestLog = new RequestLogService();
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

        [HttpPost]
        public IActionResult Approveds(string[] selects)
        {
            string message = "";
            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            CTLModels.EmployeeModel approver = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();
            for (int i = 0; i < selects.Length; i++)
            {
                message = Approver(selects[i], "", approver);
            }

            return Json(message);
        }

        [HttpGet]
        public IActionResult GetRequest()
        {
            List<string> status_pending = new List<string>()
            {
                "Created",
                "Resubmit",
                "Pending",
                "Approved"
            };

            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            string emp_id = employees.Where(w => w.name_en.ToLower() == user.ToLower()).Select(s => s.emp_id).FirstOrDefault();
            
            List<LevelModel> levels = Level.GetLevelByEmpID(emp_id);
            List<int> level = levels.Where(w => w.emp_id == emp_id).Select(s => s.level).ToList();
            List<string> departments = levels.GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).ToList();

            List<RequestModel> requests = Requests.GetRequests();
            List<RequestModel> requests_pending = requests.Where(w => status_pending.Contains(w.status_request)).ToList();

            List<RequestModel> _requests = new List<RequestModel>();
            for (int i = 0; i < requests_pending.Count; i++)
            {              
                if (level.Count > 0)
                {
                    if (requests_pending[i].is_two_step_approve)
                    {
                        LeaveTypeModel leaveType = LeaveType.GetLeaveTypeByID(requests_pending[i].leave_type_id);
                        if (requests_pending[i].amount_leave_day >= leaveType.max_consecutive_days)
                        {
                            // Two Step Approve
                            bool chk_level = level.Any(a => a == requests_pending[i].level_step + 1);
                            if (chk_level)
                            {
                                string request_department = employees.Where(w => w.emp_id == requests_pending[i].emp_id).Select(s => s.department).FirstOrDefault();
                                bool chk_dep = departments.Contains(request_department);
                                if (chk_dep)
                                {
                                    _requests.Add(requests_pending[i]);
                                }
                            }
                        }
                        else
                        {
                            //One Step Approve
                            bool chk_level = level.Any(a => a == requests_pending[i].level_step + 1); //2
                            if (chk_level)
                            {
                                string request_department = employees.Where(w => w.emp_id == requests_pending[i].emp_id).Select(s => s.department).FirstOrDefault();
                                bool chk_dep = departments.Contains(request_department);
                                if (chk_dep)
                                {
                                    _requests.Add(requests_pending[i]);
                                }
                            }
                        }                      
                    }
                    else
                    {
                        if (requests_pending[i].level_step == 0)
                        {
                            bool chk_level = level.Any(a => a == requests_pending[i].level_step + 1);
                            if (chk_level)
                            {
                                string request_department = employees.Where(w => w.emp_id == requests_pending[i].emp_id).Select(s => s.department).FirstOrDefault();
                                bool chk_dep = departments.Contains(request_department);
                                if (chk_dep)
                                {
                                    _requests.Add(requests_pending[i]);
                                }
                            }
                        }
                        else if(requests_pending[i].level_step == 1)
                        {
                            bool chk_level = level.Any(a => a == requests_pending[i].level_step + 2);
                            if (chk_level)
                            {
                                string request_department = employees.Where(w => w.emp_id == requests_pending[i].emp_id).Select(s => s.department).FirstOrDefault();
                                bool chk_dep = departments.Contains(request_department);
                                if (chk_dep)
                                {
                                    _requests.Add(requests_pending[i]);
                                }
                            }
                        }
                    }
                }                
            }

            //Pending
            var pending = _requests.Select(s => new
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
                comment = s.comment,
                level_step = s.level_step,
                is_two_step_approve = s.is_two_step_approve
            }).ToList();

            // No Pending
            List<RequestModel> requests_no_pending = requests.Where(w => !status_pending.Contains(w.status_request)).ToList();
            var no_pending = requests_no_pending.Select(s => new
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
                comment = s.comment,
                level_step = s.level_step,
                is_two_step_approve = s.is_two_step_approve
            }).ToList();

            var data = new { pending = pending, no_pending = no_pending };
            return Json(data);
        }


        [HttpGet]
        public IActionResult GetData()
        {
            List<EmpModel> emps = Employee.GetEmps();
            return Ok(emps);
        }

        [HttpGet]
        public IActionResult GetRequestLog(string emp_id, string request_id)
        {
            List<RequestLogModel> requests_log = RequestLog.GetLogByRequestId(request_id);
            int current_action_level = requests_log.Max(m => m.action_by_level);

            RequestModel request = Requests.GetRequestByID(request_id);
            int current_level_step = request.level_step;
            bool is_two_step_approve = request.is_two_step_approve;
            requests_log = requests_log.Where(w=>w.new_level_step <= current_level_step).OrderBy(o => o.log_date).ToList();

            List<LevelModel> hierarchies = Level.GetHierarchyByEmpID(emp_id);
            int level = hierarchies.Min(m => m.level);

            List<ApprovedModel> approveds = new List<ApprovedModel>();
            for (int i = 0; i < requests_log.Count; i++)
            {
                if (requests_log[i].action_by_level > 0 && (requests_log[i].new_status == "Created" || requests_log[i].new_status == "Resubmit"))
                {
                    approveds.Add(new ApprovedModel()
                    {
                        emp_id = requests_log[i].action_by,
                        emp_name = requests_log[i].action_by_name,
                        current_level = requests_log[i].action_by_level,
                        date = requests_log[i].log_date,
                        status = requests_log[i].new_status,
                        is_two_step_approve = requests_log[i].is_two_step_approve,
                        next_level = requests_log[i].action_by_level + 1
                    });
                }
                else if (requests_log[i].action_by_level > 0 && requests_log[i].new_status != "Created" && requests_log[i].new_status != "Resubmit")
                {
                    approveds.Add(new ApprovedModel()
                    {
                        emp_id = requests_log[i].action_by,
                        emp_name = requests_log[i].action_by_name,
                        current_level = requests_log[i].action_by_level,
                        date = requests_log[i].log_date,
                        status = requests_log[i].new_status,
                        is_two_step_approve = requests_log[i].is_two_step_approve,
                        next_level = 3
                    });
                }
                else if (requests_log[i].action_by_level == 0)
                {
                    approveds.Add(new ApprovedModel()
                    {
                        emp_id = requests_log[i].action_by,
                        emp_name = requests_log[i].action_by_name,
                        current_level = requests_log[i].action_by_level,
                        date = requests_log[i].log_date,
                        status = requests_log[i].new_status,
                        is_two_step_approve = requests_log[i].is_two_step_approve,
                        next_level = requests_log[i].action_by_level + 1
                    });
                }
            }
            return Json(approveds);
        }

        [HttpPost]
        public IActionResult Rejected(string request_id, string comment)
        {
            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            CTLModels.EmployeeModel approver = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();

            List<RequestLogModel> requests_log = RequestLog.GetLogByRequestId(request_id);
            RequestLogModel last_request_log = requests_log.OrderByDescending(o => o.log_date).FirstOrDefault();

            RequestModel request = Requests.GetRequestByID(request_id);

            string new_status = "Rejected";
            int rejected_level = 0;
            if (request.level_step == 0) // Operation Only
            {
                if (request.is_two_step_approve)
                {
                    if (request.status_request == "Created" || request.status_request == "Resubmit")
                    {
                        rejected_level = 1;
                    }
                    if (request.status_request == "Pending")
                    {
                        rejected_level = 2;
                    }
                    if (request.status_request == "Approved")
                    {
                        rejected_level = 3;
                    }
                }
                else
                {
                    if (request.status_request == "Created" || request.status_request == "Resubmit")
                    {
                        rejected_level = 1;
                    }
                    if (request.status_request == "Approved")
                    {
                        rejected_level = 3;
                    }
                }
            }
            else if (request.level_step == 1) // Manager
            {
                if (request.status_request == "Created" || request.status_request == "Resubmit")
                {
                    rejected_level = 2;
                }
                if (request.status_request == "Pending")
                {
                    rejected_level = 2;
                }
                if (request.status_request == "Approved")
                {
                    rejected_level = 3;
                }
            }
            else if (request.level_step == 2) // Director
            {
                if (request.status_request == "Created")
                {
                    rejected_level = 3;
                }
                if (request.status_request == "Approved")
                {
                    rejected_level = 3;
                }

            }

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

                        //Update Old Request
                        request.status_request = new_status;
                        request.level_step = rejected_level;
                        request.end_request_time = new TimeSpan(request.end_request_time.Hours, request.end_request_time.Minutes, request.end_request_time.Seconds);
                        request.start_request_time = new TimeSpan(request.start_request_time.Hours, request.start_request_time.Minutes, request.start_request_time.Seconds);
                        request.comment = comment;
                        requestService.Update(request);

                        // Insert Request Log
                        RequestLogModel rsl = new RequestLogModel()
                        {
                            log_date = DateTime.Now,
                            comment = comment,
                            action_by = approver.emp_id,
                            action_by_level = rejected_level,
                            action_by_name = approver.name_en,
                            new_status = new_status,
                            new_level_step = rejected_level,
                            old_level_step = last_request_log.new_level_step,
                            old_status = last_request_log.new_status,
                            request_id = last_request_log.request_id,
                            is_two_step_approve = last_request_log.is_two_step_approve
                        };

                        requestLogService.Insert(rsl);

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
        public IActionResult Returned(string request_id, string comment)
        {
            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            CTLModels.EmployeeModel approver = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();

            List<RequestLogModel> requests_log = RequestLog.GetLogByRequestId(request_id);
            RequestLogModel last_request_log = requests_log.OrderByDescending(o => o.log_date).FirstOrDefault();

            RequestModel request = Requests.GetRequestByID(request_id);

            string new_status = "Returned";
            int returned_level = 0;
            if (request.level_step == 0) // Operation Only
            {
                if (request.is_two_step_approve)
                {
                    if (request.status_request == "Created" || request.status_request == "Resubmit")
                    {
                        returned_level = 1;
                    }
                    if (request.status_request == "Pending")
                    {
                        returned_level = 2;
                    }
                    if (request.status_request == "Approved")
                    {
                        returned_level = 3;
                    }
                }
                else
                {
                    if (request.status_request == "Created" || request.status_request == "Resubmit")
                    {
                        returned_level = 1;
                    }
                    if (request.status_request == "Approved")
                    {
                        returned_level = 3;
                    }
                }
            }
            else if (request.level_step == 1) // Manager
            {
                if (request.status_request == "Created" || request.status_request == "Resubmit")
                {
                    returned_level = 2;
                }
                if (request.status_request == "Pending")
                {
                    returned_level = 2;
                }
                if (request.status_request == "Approved")
                {
                    returned_level = 3;
                }
            }
            else if (request.level_step == 2) // Director
            {
                if (request.status_request == "Created" || request.status_request == "Resubmit")
                {
                    returned_level = 3;
                }
                if (request.status_request == "Approved")
                {
                    returned_level = 3;
                }

            }

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

                        //Update Old Request
                        request.status_request = new_status;
                        request.level_step = returned_level;
                        request.end_request_time = new TimeSpan(request.end_request_time.Hours, request.end_request_time.Minutes, request.end_request_time.Seconds);
                        request.start_request_time = new TimeSpan(request.start_request_time.Hours, request.start_request_time.Minutes, request.start_request_time.Seconds);
                        request.comment = comment;
                        requestService.Update(request);

                        // Insert Request Log
                        RequestLogModel rsl = new RequestLogModel()
                        {
                            log_date = DateTime.Now,
                            comment = comment,
                            action_by = approver.emp_id,
                            action_by_level = returned_level,
                            action_by_name = approver.name_en,
                            new_status = new_status,
                            new_level_step = returned_level,
                            old_level_step = last_request_log.new_level_step,
                            old_status = last_request_log.new_status,
                            request_id = last_request_log.request_id,
                            is_two_step_approve = last_request_log.is_two_step_approve
                        };

                        requestLogService.Insert(rsl);

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
        public IActionResult Approved(string request_id, string comment)
        {
            if (comment == null)
            {
                comment = "";
            }

            string user = HttpContext.Session.GetString("userId");
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            CTLModels.EmployeeModel approver = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();

            string message = Approver(request_id, comment , approver);
            return Json(message);
        }

        public string Approver(string request_id,string comment , CTLModels.EmployeeModel approver)
        {
            string message = "";
            int approve_level = -1;
            RequestModel request = Requests.GetRequestByID(request_id);
            int current_level_step = request.level_step;
            string request_emp_id = request.emp_id;
            string current_status = request.status_request;
            bool is_two_step_approve = request.is_two_step_approve;
            int level_step = request.level_step;
            string new_status = "";
            if (level_step == 0) // Operation Only
            {
                if (is_two_step_approve)
                {
                    if (current_status == "Created" || current_status == "Resubmit")
                    {
                        new_status = "Pending";
                        approve_level = 1;
                    }
                    if (current_status == "Pending")
                    {
                        new_status = "Approved";
                        approve_level = 2;
                    }
                    if (current_status == "Approved")
                    {
                        new_status = "Completed";
                        approve_level = 3;
                    }
                }
                else
                {
                    if (current_status == "Created" || current_status == "Resubmit")
                    {
                        new_status = "Approved";
                        approve_level = 1;
                    }
                    if (current_status == "Approved")
                    {
                        new_status = "Completed";
                        approve_level = 3;
                    }
                }
            }
            else if (level_step == 1) // Manager
            {
                if (current_status == "Created" || current_status == "Resubmit")
                {
                    new_status = "Approved";
                    approve_level = 2;
                }
                if (current_status == "Pending")
                {
                    new_status = "Approved";
                    approve_level = 2;
                }
                if (current_status == "Approved")
                {
                    new_status = "Completed";
                    approve_level = 3;
                }
            }
            else if (level_step == 2) // Director
            {
                if (current_status == "Created" || current_status == "Resubmit")
                {
                    new_status = "Completed";
                    approve_level = 3;
                }
                if (current_status == "Approved")
                {
                    new_status = "Completed";
                    approve_level = 3;
                }

            }

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

                        //Update Request
                        request.level_step = approve_level;
                        request.status_request = new_status;
                        request.end_request_time = new TimeSpan(request.end_request_time.Hours, request.end_request_time.Minutes, request.end_request_time.Seconds);
                        request.start_request_time = new TimeSpan(request.start_request_time.Hours, request.start_request_time.Minutes, request.start_request_time.Seconds);
                        request.comment = comment;
                        requestService.Update(request);

                        // Insert Request Log
                        RequestLogModel rsl = new RequestLogModel()
                        {
                            log_date = DateTime.Now,
                            comment = comment,
                            action_by = approver.emp_id,
                            action_by_level = approve_level,
                            action_by_name = approver.name_en,
                            new_status = new_status,
                            new_level_step = approve_level,
                            old_level_step = level_step,
                            old_status = current_status,
                            request_id = request_id,
                            is_two_step_approve = is_two_step_approve
                        };

                        requestLogService.Insert(rsl);

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
            return message;
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
