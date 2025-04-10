using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IJobOwner
    {
        List<JobOwnerModel> GetJobOwner();
        List<JobOwnerModel> GetJobOwnerByJob(string job_id);
        string DeleteByJobDepartment(string job_id, string job_department);
        string Insert(string job_id, string job_department);
    }
}
