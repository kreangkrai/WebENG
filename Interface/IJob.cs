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
        string CreateTermPayment(Term_PaymentModel term_Payment);
        string UpdateJob(JobModel job);
        string UpdateTermPayment(Term_PaymentModel term_Payment);
        List<JobQuotationModel> GetJobQuotations(string year);

        List<EngProcessModel> GetProcesses();
        List<EngProcessModel> GetProcessByJob(string job);
        List<EngSystemModel> GetSystems();
        List<EngSystemModel> GetSystemByJob(string job);

    }
}
