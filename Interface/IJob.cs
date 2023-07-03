using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Interface
{
    interface IJob
    {
        List<JobModel> GetAllJobs();
        List<JobSummaryModel> GetJobsSummary();
        string CreateJob(JobModel job);
        string UpdateJob(JobModel job);
        List<JobQuotationModel> GetJobQuotations(string year);
    }
}
