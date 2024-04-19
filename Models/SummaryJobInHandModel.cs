using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class SummaryJobInHandModel
    {
        public string month { get; set; }
        public double job_eng_in_hand { get; set; }
        public double target_month { get; set; }
    }
    public class JobInhandModel
    {
        public string job_id { get; set; }
        public string customer_name { get; set; }
        public string job_name { get; set; }
        public string job_type { get; set; }
        public double job_in_hand { get; set; }
        public double job_eng_in_hand { get; set; }
        public double invoice { get; set; }
        public double invoice_eng { get; set; }
        public double percent_eng_cost { get; set; }
        public double percent_invoice { get; set; }
        public double remaining_percent_invoice { get; set; }       
        public double remaining_amount { get; set; }
    }
}
