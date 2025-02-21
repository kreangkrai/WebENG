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
        Stream ExportSummaryJobInHand(FileInfo path, List<SummaryJobInHandModel> all, List<SummaryJobInHandModel> projects, List<SummaryJobInHandModel> services);
        Stream ExportSummarySaleTurnOver(FileInfo path, List<SummaryInvoiceModel> acc_invoices,List<SummaryInvoiceModel> invoices);
        Stream ExportIdleTime(FileInfo path, List<EngineerIdleTimeModel> idles);
        Stream ExportServiceReport(FileInfo path, List<DailyActivityModel> reports);
        Stream ExportJob(FileInfo path, List<JobModel> jobs);
        Stream ExportQuotationSummary(FileInfo path, List<QuotationSummaryModel> quotations);
    }
}
