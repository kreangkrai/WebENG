using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class EngineerScoreController : Controller
    {
        private IJobResponsible JobResponsible;
        private IWorkingHours WorkingHours;
        readonly IAccessory Accessory;
        private IHoliday Holiday;
        private IJob Job;
        readonly CTLInterfaces.IEmployee Employees;
        public EngineerScoreController()
        {
            Accessory = new AccessoryService();
            Employees = new CTLServices.EmployeeService();
            JobResponsible = new JobResponsibleService();
            WorkingHours = new WorkingHoursService();
            Holiday = new HolidayService();
            Job = new JobService();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");

                List<UserModel> users = Accessory.getAllUser();
                List<CTLModels.EmployeeModel> emps = Employees.GetEmployees();
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).FirstOrDefault();
                if (u == null)
                {
                    List<CTLModels.EmployeeModel> employees = Employees.GetEmployees();
                    CTLModels.EmployeeModel employee = employees.Where(w => w.name_en.ToLower() == user.ToLower()).FirstOrDefault();
                    u = new UserModel()
                    {
                        emp_id = employee.emp_id,
                        name = employee.name_en,
                        role = "User",
                        department = employee.department
                    };
                }
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);
                HttpContext.Session.SetString("Role", u.role);

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
        public JsonResult GetScores(string emp_id, string department)
        {

            List<EngineerScoreModel> scores = new List<EngineerScoreModel>();

            List<JobModel> jobs_ = Job.GetAllJobs();
            List<JobResponsibleModel> jr = JobResponsible.GetJobsResponsible();
            jr = jr.Where(w => w.department == department).ToList();

            List<JobsWorkingHoursModel> jwh = new List<JobsWorkingHoursModel>();
            List<WorkingHoursModel> workings = WorkingHours.GetWorkingHours();

            workings = workings.Where(w => w.department == department).ToList();

            List<string> jobs = workings.GroupBy(g => g.job_id).Select(s => s.FirstOrDefault().job_id).OrderBy(o => o).ToList();
            List<HolidayModel> holidays = Holiday.GetAllHolidays();

            DateTime start = new DateTime(2022, 1, 1);
            DateTime stop = DateTime.Now;

            List<WorkingHoursModel> whs = WorkingHours.CalculateWorkingHours("ALL", start, stop);
            whs = whs.Where(w => w.department == department && w.job_id != "" && w.job_id != "J999999").ToList();
            for (int i = 0; i < jobs.Count; i++)
            {

                List<CTLModels.EmployeeModel> emps = workings.Where(w => w.job_id == jobs[i]).GroupBy(g => g.emp_id).Select(s => new CTLModels.EmployeeModel()
                {
                    emp_id = s.Key,
                    department = s.FirstOrDefault().department
                }).ToList();

                JobsWorkingHoursModel sum_department_jwh = new JobsWorkingHoursModel();
                JobsWorkingHoursModel sum_individual_jwh = new JobsWorkingHoursModel();

                for (int j = 0; j < emps.Count; j++)
                {
                    int level = 1;
                    if (jr.Any(w => w.job_id == jobs[i] && w.emp_id == emps[j].emp_id))
                    {
                        level = jr.Where(w => w.job_id == jobs[i] && w.emp_id == emps[j].emp_id).FirstOrDefault().level;
                    }
                    List<WorkingHoursModel> workings_ = workings.Where(w => w.job_id == jobs[i] && w.emp_id == emps[j].emp_id && w.department == department).ToList();
                    List<WorkingDayModel> wd = workings_.GroupBy(g => g.working_date).Select(s => new WorkingDayModel()
                    {
                        date = s.Key,
                        workings = workings_.Where(w => w.working_date == s.Key).ToList()
                    }).ToList();

                    var _whs = whs.Where(w => w.job_id == jobs[i] && w.emp_id == emps[j].emp_id).ToList();
                    List<EngineerJobTimeModel> wh = _whs.GroupBy(g => g.job_id).Select(s => new EngineerJobTimeModel()
                    {
                        normal = s.Aggregate(TimeSpan.Zero, (sum_, x) => sum_ + x.normal).TotalHours,
                        ot1_5 = s.Aggregate(TimeSpan.Zero, (sum_, x) => sum_ + x.ot1_5).TotalHours,
                        ot3_0 = s.Aggregate(TimeSpan.Zero, (sum_, x) => sum_ + x.ot3_0).TotalHours,
                        job = s.Key,
                        job_name = s.FirstOrDefault().job_name
                    }).ToList();

                    string dep = emps[j].department;
                    int amount = 3200;
                    if (dep == "CES-CIS" || dep == "AES")
                    {
                        amount = 2400;
                    }

                    if (emps[j].emp_id == emp_id)
                    {
                        JobsWorkingHoursModel jwh_individual = new JobsWorkingHoursModel()
                        {
                            job_id = jobs[i],
                            normal = wh.Sum(s => s.normal) * level,
                            ot1_5 = wh.Sum(s => s.ot1_5) * level,
                            ot3_0 = wh.Sum(s => s.ot3_0) * level,
                            total = ((wh.Sum(s => s.normal) * level) +
                            (wh.Sum(s => s.ot1_5) * 1.5 * level) +
                            (wh.Sum(s => s.ot3_0) * 3.0 * level)),
                            total_amount = (wh.Sum(s => s.normal) / 8 * level * amount),
                            total_ot_amount = ((wh.Sum(s => s.normal) / 8 * level * amount) +
                            (wh.Sum(s => s.ot1_5) / 8 * level * amount * 1.5) +
                            (wh.Sum(s => s.ot3_0) / 8 * level * amount * 3.0)),
                        };
                        sum_individual_jwh.job_id = jwh_individual.job_id;
                        sum_individual_jwh.normal += jwh_individual.normal;
                        sum_individual_jwh.ot1_5 += jwh_individual.ot1_5;
                        sum_individual_jwh.ot3_0 += jwh_individual.ot3_0;
                        sum_individual_jwh.total += jwh_individual.total;
                        sum_individual_jwh.total_amount += jwh_individual.total_amount;
                        sum_individual_jwh.total_ot_amount += jwh_individual.total_ot_amount;
                    }

                    JobsWorkingHoursModel jwh_department = new JobsWorkingHoursModel()
                    {
                        job_id = jobs[i],
                        normal = wh.Sum(s => s.normal) * level,
                        ot1_5 = wh.Sum(s => s.ot1_5) * level,
                        ot3_0 = wh.Sum(s => s.ot3_0) * level,
                        total = ((wh.Sum(s => s.normal) * level) +
                        (wh.Sum(s => s.ot1_5) * 1.5 * level) +
                        (wh.Sum(s => s.ot3_0) * 3.0 * level)),
                        total_amount = (wh.Sum(s => s.normal) / 8 * level * amount),
                        total_ot_amount = ((wh.Sum(s => s.normal) / 8 * level * amount) +
                        (wh.Sum(s => s.ot1_5) / 8 * level * amount * 1.5) +
                        (wh.Sum(s => s.ot3_0) / 8 * level * amount * 3.0)),
                    };

                    sum_department_jwh.job_id = jwh_department.job_id;
                    sum_department_jwh.normal += jwh_department.normal;
                    sum_department_jwh.ot1_5 += jwh_department.ot1_5;
                    sum_department_jwh.ot3_0 += jwh_department.ot3_0;
                    sum_department_jwh.total += jwh_department.total;
                    sum_department_jwh.total_amount += jwh_department.total_amount;
                    sum_department_jwh.total_ot_amount += jwh_department.total_ot_amount;

                }
                jwh.Add(sum_department_jwh);

                JobModel job = jobs_.Where(w => w.job_id == sum_department_jwh.job_id).FirstOrDefault();
                if (job != null)
                {
                    double cost = 0;
                    if (department == "CES-System" || department == "CES-ENG" || department == "CES-QIR" || department == "CES-PMD" || department == "CES-Exp")
                    {
                        cost = job.eng_cost;
                    }
                    if (department == "CES-CIS")
                    {
                        cost = job.cis_cost;
                    }
                    if (department == "AES")
                    {
                        cost = job.ais_cost;
                    }

                    double totalDirectCost = job.eng_cost + job.cis_cost + job.ais_cost;

                    double totalManpowerHours = sum_department_jwh.total;

                    double workingHours = sum_individual_jwh.total;
                    double factor = job.md_rate * job.pd_rate;

                    double sc =
                        totalDirectCost
                        * (job.md_rate + job.pd_rate)
                        * (totalDirectCost / totalManpowerHours)
                        * (workingHours / totalManpowerHours);
                    sc = Math.Round(sc, 1);

                    EngineerScoreModel score = new EngineerScoreModel()
                    {
                        job_id = job.job_id,
                        emp_id = emp_id,
                        job_name = job.job_name,
                        job_status = job.status,
                        md_rate = job.md_rate,
                        factor = factor,
                        pd_rate = job.pd_rate,
                        cost = cost,
                        cost_per_tmp = (job.eng_cost + job.cis_cost + job.ais_cost) / totalManpowerHours,
                        customer = job.customer,
                        manpower = workingHours,
                        manpower_per_tmp = workingHours / totalManpowerHours,
                        score = sc,
                        total_manpower = totalManpowerHours,
                        remaining_cost = cost - jwh.Where(w => w.job_id == job.job_id).Select(s => s.total_ot_amount).FirstOrDefault()

                    };

                    scores.Add(score);
                }
            }
            return Json(scores);
        }

        [HttpGet]
        public JsonResult GetWorkingUser()
        {
            List<UserModel> users = Accessory.getWorkingUser();

            return Json(users);
        }
    }
}
