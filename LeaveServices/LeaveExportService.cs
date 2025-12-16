using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;

namespace WebENG.LeaveServices
{
    public class LeaveExportService : ILeaveExport
    {
        public Stream ExportData(FileInfo path, List<TimeAttendanceModel> datas , List<LeaveTypeModel> leaves,int year)
        {
            Stream stream = new MemoryStream();
            if (path.Exists)
            {
                using (ExcelPackage p = new ExcelPackage(path))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets[0];

                    worksheet.Cells["A1"].Value = "สถิติการลาและการบันทึกเวลาเข้าออก";
                    worksheet.Cells["A2"].Value = "ปี " + year;


                    List<string> leaves_type = new List<string>()
                    {
                        "รหัสพนักงาน","ชื่อ-สกุล","แผนก","สิทธิการลาพักร้อน","พักร้อนคงเหลือ","รวมวันลาทั้งหมด"
                    };
                    var _leaves_type = leaves.GroupBy(g => g.leave_type_code).Select(s => s.FirstOrDefault()).Select(s => s.leave_name_th).ToList();
                    leaves_type.AddRange(_leaves_type);
                    leaves_type.Add("สาย(ครั้ง)");
                    leaves_type.Add("สาย(นาที)");
                    leaves_type.Add("ลืมสแกนหน้า(ครั้ง)");

                    var headers = leaves_type.ToArray();
                   
                    int startHeaderRows = 3;
                    int endsHeaderRows = 4;
                    int startHeaderCols = 1;
                    int headerCount = headers.Length;
                    int endColumn = startHeaderCols + headerCount - 1;
                    for (int col = startHeaderCols; col < startHeaderCols + headerCount; col++)
                    {
                        var cellRange = worksheet.Cells[startHeaderRows, col, endsHeaderRows, col];
                        cellRange.Merge = true;
                        cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cellRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cellRange.Style.Font.Bold = true;                        
                        cellRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cellRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(233, 235, 136));
                        cellRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cellRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cellRange.Style.WrapText = true;
                        cellRange.AutoFitColumns();
                    }
                    worksheet.Cells[startHeaderRows, startHeaderCols].LoadFromArrays(new List<string[]> { headers });
                    worksheet.Row(1).Height = 20;
                    worksheet.Row(2).Height = 20;
                    worksheet.Row(3).Height = 20; 
                    worksheet.Row(4).Height = 20;

                    int startRows = 5;

                    for (int i = 0; i < datas.Count; i++)
                    {
                        for (int col = startHeaderCols; col < startHeaderCols + headerCount; col++)
                        {
                            var cellRange = worksheet.Cells[startRows, col];
                            decimal sum_leaves = datas[i].leaves.Sum(s => s.MonthlyAccumulated.Sum(x => x.MonthlyAmount));
                            decimal sum_al = datas[i].leaves.Where(w=>w.leave_name_th == "ลาพักร้อน").Sum(s => s.MonthlyAccumulated.Sum(x => x.MonthlyAmount));
                            decimal balance_al = (decimal)datas[i].entitlement_al - sum_al;
                            if (col == 1)
                            {
                                cellRange.Value = datas[i].emp_id;
                            }
                            else if (col == 2)
                            {
                                cellRange.Value = datas[i].name_th;
                            }
                            else if(col == 3)
                            {
                                cellRange.Value = datas[i].department;
                            }
                            else if(col == 4)
                            {
                                cellRange.Value = datas[i].entitlement_al;
                            }
                            else if(col == 5)
                            {
                                cellRange.Value = balance_al;
                                cellRange.FormulaR1C1 = $"=IF({cellRange.Address}<>0, {cellRange.Address}, -)";
                                cellRange.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                cellRange.Style.Font.Bold = true;
                            }
                            else if(col == 6)
                            {
                                cellRange.Value = sum_leaves;
                                cellRange.FormulaR1C1 = $"=IF({cellRange.Address}<>0, {cellRange.Address}, -)";
                            }

                            //Each Leaves
                            else if(col > 6 && col < startHeaderCols + headerCount -3)
                            {
                                string header_name = headers[col- startHeaderCols];
                                LeaveTimeAttendanceModel leaves_ = datas[i].leaves.Where(w => w.leave_name_th == header_name).FirstOrDefault();
                                if (leaves_ != null)
                                {
                                    decimal sum = leaves_.MonthlyAccumulated.Sum(s => s.MonthlyAmount);
                                    cellRange.Value = sum;
                                }
                                else
                                {
                                    cellRange.Value = "-";
                                }
                            }
                            else // For Face Scan
                            {

                            }
                            cellRange.AutoFitColumns();
                        }
                        startRows++;
                    }
                    p.SaveAs(stream);
                    stream.Position = 0;
                }
            }
            return stream;
        }
    }
}
