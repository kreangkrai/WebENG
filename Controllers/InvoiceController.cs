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
        public JsonResult GetInvoice(string department ,int year)
        {
            if (department == "ENG")
            {
                List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryENGInvoice(year);
                return Json(invoices);
            }
            if (department == "CIS")
            {
                List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryENGInvoice(year);
                return Json(invoices);
            }
            if (department == "ENG")
            {
                List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryENGInvoice(year);
                return Json(invoices);
            }
            return Json(null);
        }

        [HttpGet]
        public JsonResult GetAccInvoice(string department,int year)
        {
            if (department == "ENG")
            {
                List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryAccENGInvoice(year);
                return Json(invoices);
            }
            if (department == "CIS")
            {
                List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryAccCISInvoice(year);
                return Json(invoices);
            }
            if (department == "AIS")
            {
                List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryAccAISInvoice(year);
                return Json(invoices);
            }
            return Json(null);
        }

        [HttpGet]
        public JsonResult GetSummaryQuarter(string department,int year)
        {
            if (department == "ENG")
            {
                List<QuarterENGModel> datas = SummaryJobInHand.GetsSummaryENGQuarter(year);
                datas = datas.Where(w => w.job_id.Substring(0, 1).ToUpper() == "J").ToList();

                List<SummaryENGQuarterModel> quarters = new List<SummaryENGQuarterModel>();

                double backlog_volume = datas.Where(w => w.type == "backlog" && w.quarter == 0 && w.job_date.Year < year).Select(s => s.job_eng_in_hand).Sum() - datas.Where(w => w.type == "backlog" && w.quarter == 0).Select(s => s.backlog_invoice_eng).Sum();
                int backlog = datas.Where(w => w.type == "backlog" && w.job_type != "").Select(s => s.job_id).Count();
                double backlog_invoice_volume = datas.Where(w => w.type == "backlog").Select(s => s.invoice_eng).Sum();
                int backlog_complete = datas.Where(w => w.type == "backlog" && (w.status == "Warranty" || w.status == "Finished")).Select(s => s.job_id).Count();
                double backlog_pending_volume = backlog_volume - backlog_invoice_volume;
                int backlog_incomplete = backlog - backlog_complete;

                quarters.Add(new SummaryENGQuarterModel()
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
                int q1_complete = datas.Where(w => w.type == "now" && w.quarter == 1 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q1_pending_volume = q1_volume - q1_invoice_volume;
                int q1_incomplete = q1 - q1_complete;

                quarters.Add(new SummaryENGQuarterModel()
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
                int q2_complete = datas.Where(w => w.type == "now" && w.quarter == 2 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q2_pending_volume = q2_volume - q2_invoice_volume;
                int q2_incomplete = q2 - q2_complete;

                quarters.Add(new SummaryENGQuarterModel()
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
                int q3_complete = datas.Where(w => w.type == "now" && w.quarter == 3 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q3_pending_volume = q3_volume - q3_invoice_volume;
                int q3_incomplete = q3 - q3_complete;

                quarters.Add(new SummaryENGQuarterModel()
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
                int q4_complete = datas.Where(w => w.type == "now" && w.quarter == 4 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q4_pending_volume = q4_volume - q4_invoice_volume;
                int q4_incomplete = q4 - q4_complete;

                quarters.Add(new SummaryENGQuarterModel()
                {
                    quarter = $"Quarter 4' {year}",
                    job_in_hand_volume = q4_volume,
                    job_in_hand = q4,
                    invoice_volume = q4_invoice_volume,
                    invoice = q4_complete,
                    pending_volume = q4_pending_volume,
                    pending = q4_incomplete
                });

                quarters.Add(new SummaryENGQuarterModel()
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
            if (department == "CIS")
            {
                List<QuarterCISModel> datas = SummaryJobInHand.GetsSummaryCISQuarter(year);
                datas = datas.Where(w => w.job_id.Substring(0, 1).ToUpper() == "J").ToList();

                List<SummaryCISQuarterModel> quarters = new List<SummaryCISQuarterModel>();

                double backlog_volume = datas.Where(w => w.type == "backlog" && w.quarter == 0 && w.job_date.Year < year).Select(s => s.job_cis_in_hand).Sum() - datas.Where(w => w.type == "backlog" && w.quarter == 0).Select(s => s.backlog_invoice_cis).Sum();
                int backlog = datas.Where(w => w.type == "backlog" && w.job_type != "").Select(s => s.job_id).Count();
                double backlog_invoice_volume = datas.Where(w => w.type == "backlog").Select(s => s.invoice_cis).Sum();
                int backlog_complete = datas.Where(w => w.type == "backlog" && (w.status == "Warranty" || w.status == "Finished")).Select(s => s.job_id).Count();
                double backlog_pending_volume = backlog_volume - backlog_invoice_volume;
                int backlog_incomplete = backlog - backlog_complete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = "Backlog",
                    job_in_hand_volume = backlog_volume,
                    job_in_hand = backlog,
                    invoice_volume = backlog_invoice_volume,
                    invoice = backlog_complete,
                    pending_volume = backlog_pending_volume,
                    pending = backlog_incomplete
                });

                double q1_volume = datas.Where(w => w.type == "now" && w.quarter == 1).Select(s => s.job_cis_in_hand).Sum();
                int q1 = datas.Where(w => w.type == "now" && w.quarter == 1 && w.job_type != "").Select(s => s.job_id).Count();
                double q1_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 1).Select(s => s.invoice_cis).Sum();
                int q1_complete = datas.Where(w => w.type == "now" && w.quarter == 1 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q1_pending_volume = q1_volume - q1_invoice_volume;
                int q1_incomplete = q1 - q1_complete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = $"Quarter 1' {year}",
                    job_in_hand_volume = q1_volume,
                    job_in_hand = q1,
                    invoice_volume = q1_invoice_volume,
                    invoice = q1_complete,
                    pending_volume = q1_pending_volume,
                    pending = q1_incomplete
                });

                double q2_volume = datas.Where(w => w.type == "now" && w.quarter == 2).Select(s => s.job_cis_in_hand).Sum();
                int q2 = datas.Where(w => w.type == "now" && w.quarter == 2 && w.job_type != "").Select(s => s.job_id).Count();
                double q2_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 2).Select(s => s.invoice_cis).Sum();
                int q2_complete = datas.Where(w => w.type == "now" && w.quarter == 2 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q2_pending_volume = q2_volume - q2_invoice_volume;
                int q2_incomplete = q2 - q2_complete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = $"Quarter 2' {year}",
                    job_in_hand_volume = q2_volume,
                    job_in_hand = q2,
                    invoice_volume = q2_invoice_volume,
                    invoice = q2_complete,
                    pending_volume = q2_pending_volume,
                    pending = q2_incomplete
                });

                double q3_volume = datas.Where(w => w.type == "now" && w.quarter == 3).Select(s => s.job_cis_in_hand).Sum();
                int q3 = datas.Where(w => w.type == "now" && w.quarter == 3 && w.job_type != "").Select(s => s.job_id).Count();
                double q3_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 3).Select(s => s.invoice_cis).Sum();
                int q3_complete = datas.Where(w => w.type == "now" && w.quarter == 3 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q3_pending_volume = q3_volume - q3_invoice_volume;
                int q3_incomplete = q3 - q3_complete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = $"Quarter 3' {year}",
                    job_in_hand_volume = q3_volume,
                    job_in_hand = q3,
                    invoice_volume = q3_invoice_volume,
                    invoice = q3_complete,
                    pending_volume = q3_pending_volume,
                    pending = q3_incomplete
                });

                double q4_volume = datas.Where(w => w.type == "now" && w.quarter == 4).Select(s => s.job_cis_in_hand).Sum();
                int q4 = datas.Where(w => w.type == "now" && w.quarter == 4 && w.job_type != "").Select(s => s.job_id).Count();
                double q4_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 4).Select(s => s.invoice_cis).Sum();
                int q4_complete = datas.Where(w => w.type == "now" && w.quarter == 4 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q4_pending_volume = q4_volume - q4_invoice_volume;
                int q4_incomplete = q4 - q4_complete;

                quarters.Add(new SummaryCISQuarterModel()
                {
                    quarter = $"Quarter 4' {year}",
                    job_in_hand_volume = q4_volume,
                    job_in_hand = q4,
                    invoice_volume = q4_invoice_volume,
                    invoice = q4_complete,
                    pending_volume = q4_pending_volume,
                    pending = q4_incomplete
                });

                quarters.Add(new SummaryCISQuarterModel()
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
            if (department == "AIS")
            {
                List<QuarterAISModel> datas = SummaryJobInHand.GetsSummaryAISQuarter(year);
                datas = datas.Where(w => w.job_id.Substring(0, 1).ToUpper() == "J").ToList();

                List<SummaryAISQuarterModel> quarters = new List<SummaryAISQuarterModel>();

                double backlog_volume = datas.Where(w => w.type == "backlog" && w.quarter == 0 && w.job_date.Year < year).Select(s => s.job_ais_in_hand).Sum() - datas.Where(w => w.type == "backlog" && w.quarter == 0).Select(s => s.backlog_invoice_ais).Sum();
                int backlog = datas.Where(w => w.type == "backlog" && w.job_type != "").Select(s => s.job_id).Count();
                double backlog_invoice_volume = datas.Where(w => w.type == "backlog").Select(s => s.invoice_ais).Sum();
                int backlog_complete = datas.Where(w => w.type == "backlog" && (w.status == "Warranty" || w.status == "Finished")).Select(s => s.job_id).Count();
                double backlog_pending_volume = backlog_volume - backlog_invoice_volume;
                int backlog_incomplete = backlog - backlog_complete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = "Backlog",
                    job_in_hand_volume = backlog_volume,
                    job_in_hand = backlog,
                    invoice_volume = backlog_invoice_volume,
                    invoice = backlog_complete,
                    pending_volume = backlog_pending_volume,
                    pending = backlog_incomplete
                });

                double q1_volume = datas.Where(w => w.type == "now" && w.quarter == 1).Select(s => s.job_ais_in_hand).Sum();
                int q1 = datas.Where(w => w.type == "now" && w.quarter == 1 && w.job_type != "").Select(s => s.job_id).Count();
                double q1_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 1).Select(s => s.invoice_ais).Sum();
                int q1_complete = datas.Where(w => w.type == "now" && w.quarter == 1 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q1_pending_volume = q1_volume - q1_invoice_volume;
                int q1_incomplete = q1 - q1_complete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = $"Quarter 1' {year}",
                    job_in_hand_volume = q1_volume,
                    job_in_hand = q1,
                    invoice_volume = q1_invoice_volume,
                    invoice = q1_complete,
                    pending_volume = q1_pending_volume,
                    pending = q1_incomplete
                });

                double q2_volume = datas.Where(w => w.type == "now" && w.quarter == 2).Select(s => s.job_ais_in_hand).Sum();
                int q2 = datas.Where(w => w.type == "now" && w.quarter == 2 && w.job_type != "").Select(s => s.job_id).Count();
                double q2_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 2).Select(s => s.invoice_ais).Sum();
                int q2_complete = datas.Where(w => w.type == "now" && w.quarter == 2 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q2_pending_volume = q2_volume - q2_invoice_volume;
                int q2_incomplete = q2 - q2_complete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = $"Quarter 2' {year}",
                    job_in_hand_volume = q2_volume,
                    job_in_hand = q2,
                    invoice_volume = q2_invoice_volume,
                    invoice = q2_complete,
                    pending_volume = q2_pending_volume,
                    pending = q2_incomplete
                });

                double q3_volume = datas.Where(w => w.type == "now" && w.quarter == 3).Select(s => s.job_ais_in_hand).Sum();
                int q3 = datas.Where(w => w.type == "now" && w.quarter == 3 && w.job_type != "").Select(s => s.job_id).Count();
                double q3_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 3).Select(s => s.invoice_ais).Sum();
                int q3_complete = datas.Where(w => w.type == "now" && w.quarter == 3 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q3_pending_volume = q3_volume - q3_invoice_volume;
                int q3_incomplete = q3 - q3_complete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = $"Quarter 3' {year}",
                    job_in_hand_volume = q3_volume,
                    job_in_hand = q3,
                    invoice_volume = q3_invoice_volume,
                    invoice = q3_complete,
                    pending_volume = q3_pending_volume,
                    pending = q3_incomplete
                });

                double q4_volume = datas.Where(w => w.type == "now" && w.quarter == 4).Select(s => s.job_ais_in_hand).Sum();
                int q4 = datas.Where(w => w.type == "now" && w.quarter == 4 && w.job_type != "").Select(s => s.job_id).Count();
                double q4_invoice_volume = datas.Where(w => w.type == "now" && w.quarter == 4).Select(s => s.invoice_ais).Sum();
                int q4_complete = datas.Where(w => w.type == "now" && w.quarter == 4 && ((w.status == "Warranty" && w.finished_date.Year == year) || (w.finished_date.Year == year && w.status == "Finished"))).Select(s => s.job_id).Count();
                double q4_pending_volume = q4_volume - q4_invoice_volume;
                int q4_incomplete = q4 - q4_complete;

                quarters.Add(new SummaryAISQuarterModel()
                {
                    quarter = $"Quarter 4' {year}",
                    job_in_hand_volume = q4_volume,
                    job_in_hand = q4,
                    invoice_volume = q4_invoice_volume,
                    invoice = q4_complete,
                    pending_volume = q4_pending_volume,
                    pending = q4_incomplete
                });

                quarters.Add(new SummaryAISQuarterModel()
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
            return Json(null);
        }

        [HttpGet]
        public JsonResult GetOrderInTake(string department,int year)
        {
            if (department == "ENG")
            {
                List<OrderInTakeENGModel> orderInTakes = new List<OrderInTakeENGModel>();
                for (int y = 2019; y <= year; y++)
                {
                    OrderInTakeENGModel orderInTake = SummaryJobInHand.GetOrderENGInTake(y);
                    orderInTakes.Add(orderInTake);
                }
                return Json(orderInTakes);
            }
            if (department == "CIS")
            {
                List<OrderInTakeCISModel> orderInTakes = new List<OrderInTakeCISModel>();
                for (int y = 2019; y <= year; y++)
                {
                    OrderInTakeCISModel orderInTake = SummaryJobInHand.GetOrderCISInTake(y);
                    orderInTakes.Add(orderInTake);
                }
                return Json(orderInTakes);
            }
            if (department == "AIS")
            {
                List<OrderInTakeAISModel> orderInTakes = new List<OrderInTakeAISModel>();
                for (int y = 2019; y <= year; y++)
                {
                    OrderInTakeAISModel orderInTake = SummaryJobInHand.GetOrderAISInTake(y);
                    orderInTakes.Add(orderInTake);
                }
                return Json(orderInTakes);
            }
            return Json(null);
        }
        public IActionResult ExportSummarySaleTurnOver(string department,int year)
        {
            if (department == "ENG")
            {
                List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryENGInvoice(year);
                List<SummaryInvoiceModel> acc_invoices = SummaryInvoice.GetsSummaryAccENGInvoice(year);
                //Download Excel
                var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "summary_sale_turnover.xlsx"));
                var stream = Export.ExportSummarySaleTurnOver(templateFileInfo, acc_invoices, invoices);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary_sale_turnover_" + year + ".xlsx");
            }
            if (department == "CIS")
            {
                List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryCISInvoice(year);
                List<SummaryInvoiceModel> acc_invoices = SummaryInvoice.GetsSummaryAccCISInvoice(year);
                //Download Excel
                var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "summary_sale_turnover.xlsx"));
                var stream = Export.ExportSummarySaleTurnOver(templateFileInfo, acc_invoices, invoices);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary_sale_turnover_" + year + ".xlsx");
            }
            if (department == "AIS")
            {
                List<SummaryInvoiceModel> invoices = SummaryInvoice.GetsSummaryAISInvoice(year);
                List<SummaryInvoiceModel> acc_invoices = SummaryInvoice.GetsSummaryAccAISInvoice(year);
                //Download Excel
                var templateFileInfo = new FileInfo(Path.Combine(_hostingEnvironment.ContentRootPath, "./wwwroot/files", "summary_sale_turnover.xlsx"));
                var stream = Export.ExportSummarySaleTurnOver(templateFileInfo, acc_invoices, invoices);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary_sale_turnover_" + year + ".xlsx");
            }
            return null;
        }
    }
}
