using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IJobResponsible
    {
        List<JobResponsibleModel> GetJobResponsible(string user_name);
        List<JobResponsibleModel> GetJobsResponsible();
        List<JobResponsibleModel> GetJobLists();
        List<QuotationResponsibleModel> GetQuotationResponsible(string user_name);
        List<JobResponsibleModel> GetAssignEngineers(string job_id);
        string AddJobResponsible(List<JobResponsibleModel> jr);
    }
}
