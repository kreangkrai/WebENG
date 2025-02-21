using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using WebENG.Service;

namespace WebENG.Controllers
{
    public class QuotationSummaryController : Controller
    {
        readonly IAccessory Accessory;
        private IQuotationSummary QuotationSummary;
        readonly IExport Export;
        readonly IEngUser EngUser;
        protected readonly IHostingEnvironment _hostingEnvironment;
        public QuotationSummaryController(IHostingEnvironment hostingEnvironment)
        {
            Accessory = new AccessoryService();
            QuotationSummary = new QuotationSummaryService();
            Export = new ExportService();
            EngUser = new EngUserService();
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Login_ENG") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<UserModel> users = new List<UserModel>();
                users = Accessory.getAllUser();
                List<EngUserModel> all_users = EngUser.GetUsers();
              
                UserModel u = users.Where(w => w.name.ToLower() == user.ToLower()).Select(s => new UserModel { name = s.name, department = s.department, role = s.role, user_id = s.user_id }).FirstOrDefault();
                HttpContext.Session.SetString("Role", u.role);
                HttpContext.Session.SetString("Name", u.name);
                HttpContext.Session.SetString("Department", u.department);

                List<string> years = new List<string>();
                for(int i= DateTime.Now.Year;i>= DateTime.Now.Year - 10; i--)
                {
                    years.Add(i.ToString());
                }

                List<string> engineers = users.Select(s => s.user_id).ToList();
                List<string> departments = all_users.Where(w=>w.group == "sale").GroupBy(g => g.department).Select(s => s.FirstOrDefault().department).ToList();

                ViewBag.ListYear = years;
                ViewBag.ListEngineer = engineers;
                ViewBag.ListDepartment = departments;

                return View(u);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public List<QuotationSummaryModel> GetQuotationSummary(string mode, string value)
        {
            List<QuotationSummaryModel> quotations = QuotationSummary.GetQuotationSummaries();
            if (mode == "Year")
            {
                string year = value.Substring(2, 2);
                quotations = quotations.Where(w => w.quotation.Contains(year)).ToList();
            }
            if (mode == "Engineer")
            {
                string engineer = value;
                quotations = quotations.Where(w => w.engineers.Where(q => q.name == engineer).Count() > 0).ToList();
            }
            if (mode == "Department")
            {
                string department = value;
                quotations = quotations.Where(w => w.sale_department == department).ToList();
            }

            return quotations;
        }
        public IActionResult ExportQuotationSummary()
        {
            List<QuotationSummaryModel> quotations = QuotationSummary.GetQuotationSummaries();
            //Download Excel
            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "quotation_summary.xlsx"));
            var stream = Export.ExportQuotationSummary(templateFileInfo, quotations);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "quotation_summary_" + ".xlsx");
        }
    }
}
