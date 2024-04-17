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
        List<SummaryInvoiceModel> GetsSummaryAccENGInvoice(int year);
        List<SummaryInvoiceModel> GetsENGInvoice(int year);
    }
}
