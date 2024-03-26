using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IExport
    {
        Stream ExportData(FileInfo path, List<JobModel> jobs);
    }
}
