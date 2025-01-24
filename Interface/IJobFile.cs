using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IJobFile
    {
        JobFileModel GetJobFile(string job_id);
        string UpdateJobFileByItem(string job_id, string item, string link);
        string CreateJobFile(string job_id);
    }
}
