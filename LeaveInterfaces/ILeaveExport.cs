using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveModels;

namespace WebENG.LeaveInterfaces
{
    public interface ILeaveExport
    {
        Stream ExportData(FileInfo path, List<TimeAttendanceModel> datas, List<LeaveTypeModel> leaves,int year);
    }
}
