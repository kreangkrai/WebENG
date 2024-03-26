using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IJobStatus
    {
        List<JobModel> GetJobStatusByUser(string user);
        List<JobModel> GetJobStatusALL();
        string UpdateJobStatus(string job,string status);
        string UpdateJobByProcessSystem(JobModel job);
    }
}
