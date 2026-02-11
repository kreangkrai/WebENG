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
        List<JobProcessSystemModel> getsJobprocessSystemByUser(string user);
        List<JobSummaryModel> GetJobsSummary();
        string CreateJob(JobModel job);
        string CreateTermPayment(Term_PaymentModel term_Payment);
        string CreateTermPayments(List<Term_PaymentsModel> term_Payments);
        string UpdateJob(JobModel job);
        string UpdateTermPayment(Term_PaymentModel term_Payment);
        string UpdateTermPayments(string jobid,List<Term_PaymentsModel> term_Payments);
        List<JobQuotationModel> GetJobQuotations();

        List<EngProcessModel> GetProcesses();
        List<EngProcessModel> GetProcessByJob(string job);
        List<EngProcessModel> GetProcessByUser(string user);
        List<EngSystemModel> GetSystems();
        List<EngSystemModel> GetSystemByJob(string job);
        List<EngSystemModel> GetSystemByUser(string user);
        string UpdateFinish(string job);
        List<string> GetDueWarranty();
    }
}
