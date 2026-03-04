using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebENG.HRModel;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface ILeaveExport
    {
        Stream ExportDataAcc(FileInfo path, List<TimeAttendanceModel> datas, List<TimeInOutModel> calLatetimes,List<LeaveTypeModel> leaves,int year);
        Stream ExportData(FileInfo path, List<TimeAttendanceModel> datas, List<TimeInOutModel> calLatetimes, List<LeaveTypeModel> leaves, int year);
    }
}
