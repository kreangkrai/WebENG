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
                        worksheet.Cells["A" + (i + startRows)].Value = (i+1);
                        worksheet.Cells["B" + (i + startRows)].Value = jobs[i].job_id;
                        worksheet.Cells["C" + (i + startRows)].Value = jobs[i].job_name;
                        worksheet.Cells["D" + (i + startRows)].Value = jobs[i].customer;
                        worksheet.Cells["F" + (i + startRows)].Value = jobs[i].status;
                        worksheet.Cells["G" + (i + startRows)].Value = jobs[i].down_payment * 0.01;
                        worksheet.Cells["H" + (i + startRows)].Value = jobs[i].document_submit * 0.01;
                        worksheet.Cells["I" + (i + startRows)].Value = jobs[i].instrument_delivered_ctl * 0.01;
                        worksheet.Cells["J" + (i + startRows)].Value = jobs[i].system_delivered_ctl * 0.01;
                        worksheet.Cells["K" + (i + startRows)].Value = jobs[i].fat * 0.01;
                        worksheet.Cells["L" + (i + startRows)].Value = jobs[i].delivery_instrument * 0.01;
                        worksheet.Cells["M" + (i + startRows)].Value = jobs[i].delivery_system * 0.01;
                        worksheet.Cells["N" + (i + startRows)].Value = jobs[i].progress_work * 0.01;
                        worksheet.Cells["O" + (i + startRows)].Value = jobs[i].commissioning * 0.01;
                        worksheet.Cells["P" + (i + startRows)].Value = jobs[i].as_built * 0.01;
                        worksheet.Cells["Q" + (i + startRows)].Value = jobs[i].job_in_hand;
                        worksheet.Cells["R" + (i + startRows)].Value = jobs[i].invoice;
                        worksheet.Cells["S" + (i + startRows)].Value = (jobs[i].invoice / jobs[i].job_in_hand);
                    }
                    p.SaveAs(stream);
                    stream.Position = 0;
                }
            }
            return stream;
        }
    }
}
