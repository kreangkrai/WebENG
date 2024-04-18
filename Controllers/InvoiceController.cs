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
    public class InvoiceController : Controller
    {
        readonly IAccessory Accessory;
        readonly ISummaryInvoice SummaryInvoice;
        readonly ISummaryJobInHand SummaryJobInHand;
        readonly IExport Export;
        protected readonly IHostingEnvironment _hostingEnvironment;
        public InvoiceController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Accessory = new AccessoryService();
            SummaryInvoice = new SummayInvoiceService();
            SummaryJobInHand = new SummaryJobInHandService();
            Export = new ExportService();
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
        public JsonResult GetENGInvoice(int year)
        {
            List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryENGInvoice(year);
            return Json(invoices);
        }

        [HttpGet]
        public JsonResult GetAccENGInvoice(int year)
        {
            List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryAccENGInvoice(year);
            return Json(invoices);
        }

        [HttpGet]
        public JsonResult GetSummaryQuarter(int year)
        {
            List<QuarterModel> datas = SummaryJobInHand.GetsSummaryQuarter(year);

            List<SummaryQuarterModel> quarters = new List<SummaryQuarterModel>();

            double backlog_volume = datas.Where(w => w.type == "backlog" && w.quarter == 0 && w.job_date.Year < year).Select(s => s.job_eng_in_hand).Sum() - datas.Where(w => w.type == "backlog" && w.quarter == 0).Select(s => s.backlog_invoice_eng).Sum();
            int backlog = datas.Where(w => w.type == "backlog" && w.job_type != "").Select(s => s.job_id).Count();
            double backlog_invoice_volume = datas.Where(w => w.type == "backlog").Select(s => s.invoice_eng).Sum();
            int backlog_complete = datas.Where(w => w.type == "backlog" && w.finished_date.Year == year && w.status == "Finished").Select(s => s.job_id).Count();
            double backlog_pending_volume = backlog_volume - backlog_invoice_volume;
            int backlog_incomplete = backlog - backlog_complete;

            quarters.Add(new SummaryQuarterModel()
            {
                quarter = "Backlog",
                job_in_hand_volume = backlog_volume,
                job_in_hand = backlog,
                invoice_volume = backlog_invoice_volume,
                invoice = backlog_complete,
                pending_volume = backlog_pending_volume,
                pending = backlog_incomplete
            });

            double q1_volume = datas.Where(w => w.type == "now" && w.quarter == 1).Select(s => s.job_eng_in_hand).Sum();
            int q1 = datas.Where(w => w.type == "now" && w.quarter == 1 && w.job_type != "").Select(s => s.job_id).Count();
            double q1_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 1).Select(s => s.invoice_eng).Sum();
            int q1_complete = datas.Where(w => w.type == "now" && w.quarter == 1 && w.finished_date.Year == year && w.status == "Finished").Select(s => s.job_id).Count();
            double q1_pending_volume = q1_volume - q1_invoice_volume;
            int q1_incomplete = q1 - q1_complete;

            quarters.Add(new SummaryQuarterModel()
            {
                quarter = $"Quarter 1' {year}",
                job_in_hand_volume = q1_volume,
                job_in_hand = q1,
                invoice_volume = q1_invoice_volume,
                invoice = q1_complete,
                pending_volume = q1_pending_volume,
                pending = q1_incomplete
            });

            double q2_volume = datas.Where(w => w.type == "now" && w.quarter == 2).Select(s => s.job_eng_in_hand).Sum();
            int q2 = datas.Where(w => w.type == "now" && w.quarter == 2 && w.job_type != "").Select(s => s.job_id).Count();
            double q2_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 2).Select(s => s.invoice_eng).Sum();
            int q2_complete = datas.Where(w => w.type == "now" && w.quarter == 2 && w.finished_date.Year == year && w.status == "Finished").Select(s => s.job_id).Count();
            double q2_pending_volume = q2_volume - q2_invoice_volume;
            int q2_incomplete = q2 - q2_complete;

            quarters.Add(new SummaryQuarterModel()
            {
                quarter = $"Quarter 2' {year}",
                job_in_hand_volume = q2_volume,
                job_in_hand = q2,
                invoice_volume = q2_invoice_volume,
                invoice = q2_complete,
                pending_volume = q2_pending_volume,
                pending = q2_incomplete
            });

            double q3_volume = datas.Where(w => w.type == "now" && w.quarter == 3).Select(s => s.job_eng_in_hand).Sum();
            int q3 = datas.Where(w => w.type == "now" && w.quarter == 3 && w.job_type != "").Select(s => s.job_id).Count();
            double q3_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 3).Select(s => s.invoice_eng).Sum();
            int q3_complete = datas.Where(w => w.type == "now" && w.quarter == 3 && w.finished_date.Year == year && w.status == "Finished").Select(s => s.job_id).Count();
            double q3_pending_volume = q3_volume - q3_invoice_volume;
            int q3_incomplete = q3 - q3_complete;

            quarters.Add(new SummaryQuarterModel()
            {
                quarter = $"Quarter 3' {year}",
                job_in_hand_volume = q3_volume,
                job_in_hand = q3,
                invoice_volume = q3_invoice_volume,
                invoice = q3_complete,
                pending_volume = q3_pending_volume,
                pending = q3_incomplete
            });

            double q4_volume = datas.Where(w => w.type == "now" && w.quarter == 4).Select(s => s.job_eng_in_hand).Sum();
            int q4 = datas.Where(w => w.type == "now" && w.quarter == 4 && w.job_type != "").Select(s => s.job_id).Count();
            double q4_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 4).Select(s => s.invoice_eng).Sum();
            int q4_complete = datas.Where(w => w.type == "now" && w.quarter == 4 && w.finished_date.Year == year && w.status == "Finished").Select(s => s.job_id).Count();
            double q4_pending_volume = q4_volume - q4_invoice_volume;
            int q4_incomplete = q4 - q4_complete;

            quarters.Add(new SummaryQuarterModel()
            {
                quarter = $"Quarter 4' {year}",
                job_in_hand_volume = q4_volume,
                job_in_hand = q4,
                invoice_volume = q4_invoice_volume,
                invoice = q4_complete,
                pending_volume = q4_pending_volume,
                pending = q4_incomplete
            });

            quarters.Add(new SummaryQuarterModel()
            {
                quarter = "Summary",
                job_in_hand_volume = backlog_volume + q1_volume + q2_volume + q3_volume + q4_volume,
                job_in_hand = backlog + q1 + q2 + q3 + q4,
                invoice_volume = backlog_invoice_volume + q1_invoice_volume + q2_invoice_volume + q3_invoice_volume + q4_invoice_volume,
                invoice = backlog_complete + q1_complete + q2_complete + q3_complete + q4_complete,
                pending_volume = backlog_pending_volume + q1_pending_volume + q2_pending_volume + q3_pending_volume + q4_pending_volume,
                pending = backlog_incomplete + q1_incomplete + q2_incomplete + q3_incomplete + q4_incomplete
            });
            return Json(quarters);
        }

        public IActionResult ExportSummarySaleTurnOver(int year)
        {
            List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryENGInvoice(year);
            List<SummaryInvoiceModel> acc_invoices = SummaryInvoice.GetsSummaryAccENGInvoice(year);
            //Download Excel
            var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "summary_sale_turnover.xlsx"));
            var stream = Export.ExportSummarySaleTurnOver(templateFileInfo, acc_invoices,invoices);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary_sale_turnover_" + year + ".xlsx");
        }
    }
}
