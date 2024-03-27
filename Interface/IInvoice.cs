using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IInvoice
    {
        List<InvoiceModel> GetByJob(string job);
        string Insert(List<InvoiceModel> invoices);
        string Delete(string job);
    }
}
