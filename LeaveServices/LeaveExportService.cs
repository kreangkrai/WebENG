using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebENG.HRModel;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;

namespace WebENG.LeaveServices
{
    public class LeaveExportService : ILeaveExport
    {
        public Stream ExportData(FileInfo path, List<TimeAttendanceModel> datas , List<TimeInOutModel> calLatetimes, List<LeaveTypeModel> leaves,int year)
        {
            Stream stream = new MemoryStream();
            if (path.Exists)
            {
                using (ExcelPackage p = new ExcelPackage(path))
                {
                    List<string> months = new List<string>
                    {
                        "มกราคม",
                        "กุมภาพันธ์",
                        "มีนาคม",
                        "เมษายน",
                        "พฤษภาคม",
                        "มิถุนายน",
                        "กรกฎาคม",
                        "สิงหาคม",
                        "กันยายน",
                        "ตุลาคม",
                        "พฤศจิกายน",
                        "ธันวาคม",
                        "สรุป"
                    };

                    Dictionary<string, Color> Colors = new Dictionary<string, Color>
                    {
                        { "color-brown",    Color.FromArgb(179, 154, 144) }, // #B39A90   น้ำตาลอ่อนนวล
                        { "color-sky",      Color.FromArgb(168, 221, 251) }, // #A8DDFB   ฟ้าอ่อนสดใส
                        { "color-yellow",   Color.FromArgb(255, 233, 167) }, // #FFE9A7   เหลืองพาสเทลอบอุ่น
                        { "color-green",    Color.FromArgb(150, 209, 153) }, // #96D199   เขียวมิ้นท์อ่อน
                        { "color-blue",     Color.FromArgb(126, 182, 240) }, // #7EB6F0   น้ำเงินอ่อนสะอาด
                        { "color-red",      Color.FromArgb(242, 156, 154) }, // #F29C9A   แดงชมพูอ่อน
                        { "color-pink",     Color.FromArgb(248, 183, 208) }, // #F8B7D0   ชมพูหวานพาสเทล
                        { "color-bluegrey", Color.FromArgb(199, 213, 220) }, // #C7D5DC   เทาอมฟ้าอ่อน สงบ
                        { "color-orange",   Color.FromArgb(255, 198, 176) }, // #FFC6B0   ส้มพีชอ่อนน่ารัก
                        { "color-purple",   Color.FromArgb(198, 143, 214) }  // #C68FD6   ม่วงลาเวนเดอร์อ่อน
                    };

                    for (int m = 0; m < months.Count; m++)
                    {
                        ExcelWorksheet worksheet = p.Workbook.Worksheets.Add(months[m]);
                        
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

                        int decimalValue = endColumn + 65 - 1;

                        char character = (char)decimalValue;

                        worksheet.Cells["A1"].Value = "สถิติการลาและการบันทึกเวลาเข้าออก";
                        worksheet.Cells["A1"].Style.Font.Size = 14;
                        worksheet.Cells["A1"].Style.Font.Bold = true;
                        worksheet.Cells[$"A1:{character}1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[$"A1:{character}1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(222, 151, 138));
                        if (m != months.Count - 1)
                        {
                            worksheet.Cells["A2"].Value = months[m] + " ปี " + year;
                        }
                        else
                        {
                            worksheet.Cells["A2"].Value = "ปี " + year;
                        }

                        worksheet.Cells["A2"].Style.Font.Size = 14;
                        worksheet.Cells[$"A2:{character}2"].Style.Font.Bold = true;
                        worksheet.Cells[$"A2:{character}2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[$"A2:{character}2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(222, 151, 138));

                        for (int col = startHeaderCols; col < startHeaderCols + headerCount; col++)
                        {
                            string header_name = headers[col - startHeaderCols];
                            var cellRange = worksheet.Cells[startHeaderRows, col, endsHeaderRows, col];
                            cellRange.Merge = true;
                            cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cellRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cellRange.Style.Font.Bold = true;
                            cellRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            string color_code = leaves.Where(w=>w.leave_name_th == header_name).Select(s=>s.color_code).FirstOrDefault();
                            if (color_code != null)
                            {
                                Color color_item = Colors[color_code];
                                cellRange.Style.Fill.BackgroundColor.SetColor(color_item);
                            }
                            else
                            {
                                cellRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            }
                            cellRange.Style.Font.Color.SetColor(Color.Black);
                            cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cellRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cellRange.Style.WrapText = true;
                            cellRange.AutoFitColumns();
                        }
                        worksheet.Cells[startHeaderRows, startHeaderCols].LoadFromArrays(new List<string[]> { headers });
                        int dataStartRow = endsHeaderRows + 1;
                        int lastRow = worksheet.Dimension.End.Row;

                        var fullRange = worksheet.Cells[startHeaderRows, startHeaderCols, lastRow, startHeaderCols + headerCount - 1];
                        fullRange.AutoFilter = true;

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
                                decimal sum_leaves = 0;
                                decimal sum_al = 0;
                                decimal balance_al = 0;
                                if (m != months.Count - 1)
                                {
                                    sum_leaves = datas[i].leaves.Sum(s => s.MonthlyAccumulated.Where(w => w.Month == m + 1).Select(x => x.AccumulatedAmount).FirstOrDefault());
                                    sum_al = datas[i].leaves.Where(w => w.leave_name_th == "ลาพักร้อน").Sum(s => s.MonthlyAccumulated.Where(a => a.Month == m + 1).Select(x => x.AccumulatedAmount).FirstOrDefault());
                                    balance_al = (decimal)datas[i].entitlement_al - sum_al;
                                }
                                else
                                {
                                    sum_leaves = datas[i].leaves.Sum(s => s.MonthlyAccumulated.Sum(x => x.MonthlyAmount));
                                    sum_al = datas[i].leaves.Where(w => w.leave_name_th == "ลาพักร้อน").Sum(s => s.MonthlyAccumulated.Sum(x => x.MonthlyAmount));
                                    balance_al = (decimal)datas[i].entitlement_al - sum_al;
                                }
                                if (col == 1)
                                {
                                    cellRange.Value = datas[i].emp_id;
                                    cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    if (datas[i].position != "Operation")
                                    {
                                        cellRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        cellRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(227, 237, 104));
                                    }
                                }
                                else if (col == 2)
                                {
                                    cellRange.Value = datas[i].name_th;
                                    cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    if (datas[i].position != "Operation")
                                    {
                                        cellRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        cellRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(227, 237, 104));
                                    }

                                }
                                else if (col == 3)
                                {
                                    cellRange.Value = datas[i].department;
                                    cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    if (datas[i].position != "Operation")
                                    {
                                        cellRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        cellRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(227, 237, 104));
                                    }
                                }
                                else if (col == 4)
                                {
                                    cellRange.Value = datas[i].entitlement_al;
                                }
                                else if (col == 5)
                                {
                                    cellRange.Value = balance_al;
                                    cellRange.Formula = $"IF({balance_al}<>0, {balance_al}, \"-\")";
                                    cellRange.Style.Font.Color.SetColor(Color.Red);
                                    cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    cellRange.Style.Font.Bold = true;
                                }
                                else if (col == 6)
                                {
                                    cellRange.Value = sum_leaves;
                                    cellRange.Formula = $"IF({sum_leaves}<>0, {sum_leaves}, \"-\")";
                                    cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                }

                                //Each Leaves
                                else if (col > 6 && col < startHeaderCols + headerCount - 3)
                                {
                                    string header_name = headers[col - startHeaderCols];
                                    LeaveTimeAttendanceModel leaves_ = datas[i].leaves.Where(w => w.leave_name_th == header_name).FirstOrDefault();
                                    if (leaves_ != null)
                                    {
                                        decimal sum = 0;
                                        if (m != months.Count - 1)
                                        {
                                            sum = leaves_.MonthlyAccumulated.Where(w => w.Month == m + 1).Select(s => s.AccumulatedAmount).FirstOrDefault();
                                        }
                                        else
                                        {
                                            sum = leaves_.MonthlyAccumulated.Sum(s => s.MonthlyAmount);
                                        }
                                        cellRange.Value = sum;
                                        cellRange.Formula = $"IF({sum}<>0, {sum}, \"-\")";
                                        cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    }
                                    else
                                    {
                                        cellRange.Value = "-";
                                        cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    }
                                }
                                else // For Face Scan
                                {
                                    if (months[m] == "สรุป")
                                    {
                                        TimeInOutModel timeInOut = calLatetimes.Where(w => w.emp_id == datas[i].emp_id).FirstOrDefault();
                                        if (col == startHeaderCols + headerCount - 3)
                                        {
                                            cellRange.Value = timeInOut.TotalCountLate;
                                            cellRange.Formula = $"IF({timeInOut.TotalCountLate}<>0, {timeInOut.TotalCountLate}, \"-\")";
                                            cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        }
                                        if (col == startHeaderCols + headerCount - 2)
                                        {
                                            cellRange.Value = timeInOut.TotalMinuteLate;
                                            cellRange.Formula = $"IF({timeInOut.TotalMinuteLate}<>0, {timeInOut.TotalMinuteLate}, \"-\")";
                                            cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        }
                                        if (col == startHeaderCols + headerCount - 1)
                                        {
                                            cellRange.Value = timeInOut.TotalCountForgotScan;
                                            cellRange.Formula = $"IF({timeInOut.TotalCountForgotScan}<>0, {timeInOut.TotalCountForgotScan}, \"-\")";
                                            cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        }
                                    }
                                    else
                                    {
                                        TimeInOutModel timeInOut = calLatetimes.Where(w => w.emp_id == datas[i].emp_id).FirstOrDefault();
                                        var time_InOut = timeInOut.months.Where(w => w.month == m + 1).FirstOrDefault();
                                        if (time_InOut != null)
                                        {
                                            if (col == startHeaderCols + headerCount - 3)
                                            {
                                                cellRange.Value = time_InOut.count_late;
                                                cellRange.Formula = $"IF({time_InOut.count_late}<>0, {time_InOut.count_late}, \"-\")";
                                                cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            }
                                            if (col == startHeaderCols + headerCount - 2)
                                            {
                                                cellRange.Value = time_InOut.minute_late;
                                                cellRange.Formula = $"IF({time_InOut.minute_late}<>0, {time_InOut.minute_late}, \"-\")";
                                                cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            }
                                            if (col == startHeaderCols + headerCount - 1)
                                            {
                                                cellRange.Value = time_InOut.count_forgot_scan;
                                                cellRange.Formula = $"IF({time_InOut.count_forgot_scan}<>0, {time_InOut.count_forgot_scan}, \"-\")";
                                                cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                            }
                                        }
                                    }
                                }
                                cellRange.AutoFitColumns();
                            }
                            startRows++;
                        }
                    }
                    p.Workbook.Worksheets.Delete(0);
                    p.SaveAs(stream);
                    stream.Position = 0;
                }
            }
            return stream;
        }
    }
}
