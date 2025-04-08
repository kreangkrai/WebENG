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
        Stream ExportSummaryENGJobInHand(FileInfo path, List<SummaryENGJobInHandModel> all, List<SummaryENGJobInHandModel> projects, List<SummaryENGJobInHandModel> services);
        Stream ExportSummaryCISJobInHand(FileInfo path, List<SummaryCISJobInHandModel> all, List<SummaryCISJobInHandModel> projects, List<SummaryCISJobInHandModel> services);
        Stream ExportSummaryAISJobInHand(FileInfo path, List<SummaryAISJobInHandModel> all, List<SummaryAISJobInHandModel> projects, List<SummaryAISJobInHandModel> services);
        Stream ExportSummarySaleTurnOver(FileInfo path, List<SummaryInvoiceModel> acc_invoices,List<SummaryInvoiceModel> invoices);
        Stream ExportIdleTime(FileInfo path, List<EngineerIdleTimeModel> idles);
        Stream ExportServiceReport(FileInfo path, List<DailyActivityModel> reports);
        Stream ExportJob(FileInfo path, List<JobModel> jobs);
        Stream ExportQuotationSummary(FileInfo path, List<QuotationSummaryModel> quotations);
    }
}
