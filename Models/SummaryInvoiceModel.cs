using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class SummaryInvoiceModel
    {
        public string month { get; set; }
        public double invoice { get; set; }
        public double target_month { get; set; }
    }
}
