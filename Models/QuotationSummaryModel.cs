using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class QuotationSummaryModel
    {
        public string quotation { get; set; }
        public string quotation_name { get; set; }
        public DateTime date { get; set; }
        public string customer { get; set; }
        public string quotation_type { get; set; }
        public string end_user { get; set; }
        public string sale_name { get; set; }
        public string sale_id { get; set; }
        public string sale_department { get; set; }
        public List<ENGQuotationSummaryModel> engineers { get; set; }

    }
    public class ENGQuotationSummaryModel
    {
        public string name { get; set; }
        public double total_manhour { get; set; }
    }
}
