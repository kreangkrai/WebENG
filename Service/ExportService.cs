using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;
using OfficeOpenXml;
namespace WebENG.Service
{
    public class ExportService : IExport
    {
        public Stream ExportData(FileInfo path, List<JobModel> jobs)
        {
            Stream stream = new MemoryStream();
            if (path.Exists)
            {
                using (ExcelPackage p = new ExcelPackage(path))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets["Job In Hand"];

                    int startRows = 3;
                    for (int i = 0; i < jobs.Count; i++)
                    {
                        double cost = jobs[i].job_summary.Count > 0 ? jobs[i].job_summary[0].cost : 0;
                        double _eng_cost = jobs[i].job_summary.Count > 0 ? jobs[i].job_summary.Sum(s => s.totalEngCost) : 0;
                        worksheet.Cells["A" + (i + startRows)].Value = (i+1);
                        worksheet.Cells["B" + (i + startRows)].Value = jobs[i].job_id;
                        worksheet.Cells["C" + (i + startRows)].Value = jobs[i].job_name;
                        worksheet.Cells["D" + (i + startRows)].Value = jobs[i].customer;
                        worksheet.Cells["E" + (i + startRows)].Value = cost == 0 ? "NA" : ((int)(Math.Ceiling((_eng_cost / cost) * 100))).ToString();
                        worksheet.Cells["G" + (i + startRows)].Value = jobs[i].status;
                        worksheet.Cells["H" + (i + startRows)].Value = jobs[i].term_payment.down_payment * 0.01;
                        worksheet.Cells["I" + (i + startRows)].Value = jobs[i].term_payment.document_submit * 0.01;
                        worksheet.Cells["J" + (i + startRows)].Value = jobs[i].term_payment.instrument_vendor * 0.01;
                        worksheet.Cells["K" + (i + startRows)].Value = jobs[i].term_payment.instrument_delivered_ctl * 0.01;
                        worksheet.Cells["L" + (i + startRows)].Value = jobs[i].term_payment.system_delivered_ctl * 0.01;
                        worksheet.Cells["M" + (i + startRows)].Value = jobs[i].term_payment.fat * 0.01;
                        worksheet.Cells["N" + (i + startRows)].Value = jobs[i].term_payment.delivery_instrument * 0.01;
                        worksheet.Cells["O" + (i + startRows)].Value = jobs[i].term_payment.delivery_system * 0.01;
                        worksheet.Cells["P" + (i + startRows)].Value = jobs[i].term_payment.progress_work * 0.01;
                        worksheet.Cells["Q" + (i + startRows)].Value = jobs[i].term_payment.installation_work_complete * 0.01;
                        worksheet.Cells["R" + (i + startRows)].Value = jobs[i].term_payment.commissioning * 0.01;
                        worksheet.Cells["S" + (i + startRows)].Value = jobs[i].term_payment.startup * 0.01;
                        worksheet.Cells["T" + (i + startRows)].Value = jobs[i].term_payment.as_built * 0.01;
                        worksheet.Cells["U" + (i + startRows)].Value = jobs[i].term_payment.warranty * 0.01;
                        worksheet.Cells["V" + (i + startRows)].Value = jobs[i].term_payment.finished * 0.01;
                        worksheet.Cells["W" + (i + startRows)].Value = jobs[i].job_eng_in_hand;
                        worksheet.Cells["X" + (i + startRows)].Value = jobs[i].eng_invoice;
                        worksheet.Cells["Y" + (i + startRows)].Value = (jobs[i].eng_invoice / jobs[i].job_eng_in_hand);
                        worksheet.Cells["Z" + (i + startRows)].Value = jobs[i].job_date.ToString("dd/MM/yyyy");
                    }
                    p.SaveAs(stream);
                    stream.Position = 0;
                }
            }
            return stream;
        }

        public Stream ExportIdleTime(FileInfo path, List<EngineerIdleTimeModel> idles)
        {
            Stream stream = new MemoryStream();
            if (path.Exists)
            {
                using (ExcelPackage p = new ExcelPackage(path))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets["Idle"];

                    int startRows = 2;
                    for (int i = 0; i < idles.Count; i++)
                    {
                        worksheet.Cells["A" + (i + startRows)].Value = (i + 1);
                        worksheet.Cells["B" + (i + startRows)].Value = idles[i].userName;
                        worksheet.Cells["C" + (i + startRows)].Value = idles[i].idle;
                        worksheet.Cells["D" + (i + startRows)].Value = idles[i].normal;
                        worksheet.Cells["E" + (i + startRows)].Value = idles[i].ot1_5;
                        worksheet.Cells["F" + (i + startRows)].Value = idles[i].ot3_0;
                        worksheet.Cells["G" + (i + startRows)].Value = idles[i].leave;
                        worksheet.Cells["H" + (i + startRows)].Value = idles[i].workingHours;
                        worksheet.Cells["I" + (i + startRows)].Value = idles[i].normal + idles[i].leave + idles[i].ot1_5 + idles[i].ot3_0;
                    }
                    p.SaveAs(stream);
                    stream.Position = 0;
                }
            }
            return stream;
        }

        public Stream ExportSummaryJobInHand(FileInfo path, List<SummaryJobInHandModel> all, List<SummaryJobInHandModel> projects, List<SummaryJobInHandModel> services)
        {
            Stream stream = new MemoryStream();
            if (path.Exists)
            {
                using (ExcelPackage p = new ExcelPackage(path))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets["data"];

                    int startRows = 5;
                    for (int i = 0; i < all.Count; i++)
                    {
                        worksheet.Cells["E" + (i + startRows)].Value = all[i].target_month;
                        worksheet.Cells["F" + (i + startRows)].Value = all[i].job_eng_in_hand;
                    }

                    startRows = 20;
                    for (int i = 0; i < projects.Count; i++)
                    {
                        worksheet.Cells["E" + (i + startRows)].Value = projects[i].target_month;
                        worksheet.Cells["F" + (i + startRows)].Value = projects[i].job_eng_in_hand;
                    }

                    startRows = 35;
                    for (int i = 0; i < services.Count; i++)
                    {
                        worksheet.Cells["E" + (i + startRows)].Value = services[i].target_month;
                        worksheet.Cells["F" + (i + startRows)].Value = services[i].job_eng_in_hand;
                    }

                    p.SaveAs(stream);
                    stream.Position = 0;
                }
            }
            return stream;
        }

        public Stream ExportSummarySaleTurnOver(FileInfo path, List<SummaryInvoiceModel> acc_invoices,List<SummaryInvoiceModel> invoices)
        {
            Stream stream = new MemoryStream();
            if (path.Exists)
            {
                using (ExcelPackage p = new ExcelPackage(path))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets["data"];

                    int startRows = 5;
                    for (int i = 0; i < invoices.Count; i++)
                    {
                        worksheet.Cells["E" + (i + startRows)].Value = invoices[i].target_month;
                        worksheet.Cells["F" + (i + startRows)].Value = invoices[i].invoice;
                    }

                    startRows = 20;
                    for (int i = 0; i < acc_invoices.Count; i++)
                    {
                        worksheet.Cells["E" + (i + startRows)].Value = acc_invoices[i].target_month;
                        worksheet.Cells["F" + (i + startRows)].Value = acc_invoices[i].invoice;
                    }
                    p.SaveAs(stream);
                    stream.Position = 0;
                }
            }
            return stream;
        }
    }
}
