using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.CTLModels;
using WebENG.LeaveInterfaces;

namespace WebENG.LeaveServices
{
    public class LeaveService : ILeave
    {
        public double CalculateLeaveDays(EmployeeModel emp, int targetYear, int min_leave_staft, int max_leave_staft, int min_leave_manager, int max_leave_manager)
        {
            int hireYear = emp.start_date.Year;
            DateTime promoteDate = emp.promote_manager_date;
            int promoteYear = promoteDate.Year;
            bool isManager = emp.position.Contains("Manager") || emp.position.Contains("Director");

            if (isManager && promoteDate != DateTime.MinValue)
            {
                bool isFirstYearEmployeeAndPromoted = (hireYear == promoteYear) && (targetYear == promoteYear);

                if (isFirstYearEmployeeAndPromoted)
                {
                    // กรณีพิเศษ: เข้า + เลื่อน + target = ปีเดียวกัน
                    if (promoteDate.Day <= 15 && promoteDate.Month == 1)
                        return min_leave_manager; // เต็มปีทันที
                    else
                        return CalculateProratedManagerLeave(promoteDate, min_leave_manager);
                }
                else
                {

                    if (targetYear == promoteYear)
                    {
                        // เลื่อนปีนี้ แต่ไม่ใช่ปีแรกที่เข้าทำงาน
                        if (emp.promote_manager_date.Day <= 15 && emp.promote_manager_date.Month == 1)
                        {
                            int yearsAsStaff = promoteYear - hireYear - 1;
                            double leaveAsStaff = Math.Round((double)Math.Min(min_leave_staft + (yearsAsStaff), max_leave_staft), 2);
                            if (leaveAsStaff >= max_leave_staft)
                            {
                                return min_leave_manager + 1;
                            }
                            return min_leave_manager; // ปีแรกเข้าเต็ม (แต่เลื่อนปีนี้ → ยังเป็น Staff)
                        }
                        else
                        {
                            int yearsAsStaff = promoteYear - hireYear;
                            if (yearsAsStaff == 0)
                                return CalculateProratedStaffLeave(emp.start_date, min_leave_staft);
                            else
                                return Math.Round((double)Math.Min(min_leave_staft + (yearsAsStaff), max_leave_staft), 2);
                        }
                    }
                    else
                    {
                        // ปีถัดไปหลังเลื่อน → ใช้สูตร Manager ปกติ
                        int yearsAsStaff = promoteYear - hireYear;
                        int staffLeave = Math.Min(min_leave_staft + (yearsAsStaff), max_leave_staft);
                        bool reachedStaffMax = staffLeave >= max_leave_staft;
                        int yearsAsManager = targetYear - promoteYear;
                        int baseManagerLeave = reachedStaffMax ? (min_leave_manager + 1) : min_leave_manager;
                        int total = 0;
                        if (emp.promote_manager_date.Day <= 15 && emp.promote_manager_date.Month == 1)
                             total = baseManagerLeave + (yearsAsManager);
                        else
                             total = baseManagerLeave + (yearsAsManager - 1);
                        return Math.Round((double)Math.Min(total, max_leave_manager), 2);
                    }
                }
            }
            else
            {
                // Staff ปกติ
                if (targetYear == hireYear)
                {
                    if (emp.start_date.Day <= 15 && emp.start_date.Month == 1)
                        return min_leave_staft;
                    else
                        return CalculateProratedStaffLeave(emp.start_date, min_leave_staft);
                }
                else
                {
                    return Math.Round((double)Math.Min(min_leave_staft + (targetYear - hireYear), max_leave_staft), 2);
                }
            }
        }
        public double CalculateLeaveDaysOLD(EmployeeModel emp, int targetYear, int min_leave_staft, int max_leave_staft, int min_leave_manager, int max_leave_manager)
        {
            int hireYear = emp.start_date.Year;
            DateTime promoteDate = emp.promote_manager_date;
            int promoteYear = promoteDate.Year;

            bool isManager = emp.position.Contains("Manager") || emp.position.Contains("Director");

            if (isManager && promoteDate != DateTime.MinValue)
            {
                bool isFirstYearEmployeeAndPromoted = (hireYear == promoteYear) && (targetYear == promoteYear);

                if (isFirstYearEmployeeAndPromoted)
                {
                    // กรณีพิเศษ: เข้า + เลื่อน + target = ปีเดียวกัน → Prorate Manager
                    return CalculateProratedManagerLeave(promoteDate, min_leave_manager);
                }
                else if (targetYear == promoteYear)
                {
                    // เลื่อนปีนี้ แต่ไม่ใช่ปีแรกที่เข้าทำงาน → ใช้ Staff เต็ม
                    int yearsAsStaff = promoteYear - hireYear;
                    if (yearsAsStaff == 0)
                        return CalculateProratedStaffLeave(emp.start_date, min_leave_staft);
                    else
                        return Math.Round((double)Math.Min(min_leave_staft + (yearsAsStaff - 1), max_leave_staft), 2);
                }
                else
                {
                    // ปีถัดไป → ใช้สูตร Manager ปกติ
                    int yearsAsStaff = promoteYear - hireYear;
                    int staffLeave = Math.Min(min_leave_staft + (yearsAsStaff - 1), max_leave_staft);
                    bool reachedStaffMax = staffLeave >= max_leave_staft;

                    int yearsAsManager = targetYear - promoteYear;
                    int baseManagerLeave = reachedStaffMax ? (min_leave_manager + 1) : min_leave_manager;
                    int total = baseManagerLeave + (yearsAsManager - 1);
                    return Math.Round((double)Math.Min(total, max_leave_manager), 2);
                }
            }
            else
            {
                // Staff ปกติ
                if (targetYear == hireYear)
                    return CalculateProratedStaffLeave(emp.start_date, min_leave_staft);
                else
                    return Math.Round((double)Math.Min(min_leave_staft + (targetYear - hireYear - 1), max_leave_staft), 2);
            }
        }

        // Prorated ปีแรก (Staff)
        private double CalculateProratedStaffLeave(DateTime hireDate, int min_leave)
        {
            int months = GetCountedMonths(hireDate);
            double leave = (months * min_leave) / 12.0;
            return Math.Round(leave, 2);
        }

        // Prorated ปีแรก (Manager)
        private double CalculateProratedManagerLeave(DateTime promotionDate, int min_leave)
        {
            int months = GetCountedMonths(promotionDate);
            double leave = (months * min_leave) / 12.0;
            return Math.Round(leave, 2);
        }

        private int GetCountedMonths(DateTime startDate)
        {
            int startMonth = startDate.Month;
            bool countFirst = startDate.Day <= 15;
            int count = countFirst ? 1 : 0;

            for (int m = startMonth + 1; m <= 12; m++)
                count++;

            return count;
        }      
    }
}

