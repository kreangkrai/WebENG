using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface ISummaryInvoice
    {
        List<SummaryInvoiceModel> GetsSummaryENGInvoice(int year);
        List<SummaryInvoiceModel> GetsSummaryCISInvoice(int year);
        List<SummaryInvoiceModel> GetsSummaryAISInvoice(int year);
        List<SummaryInvoiceModel> GetsSummaryAccENGInvoice(int year);
        List<SummaryInvoiceModel> GetsSummaryAccCISInvoice(int year);
        List<SummaryInvoiceModel> GetsSummaryAccAISInvoice(int year);
        List<SummaryInvoiceModel> GetsENGInvoice(int year);
        List<SummaryInvoiceModel> GetsCISInvoice(int year);
        List<SummaryInvoiceModel> GetsAISInvoice(int year);
    }
}
