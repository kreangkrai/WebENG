using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class SummaryENGJobInHandModel
    {
        public string month { get; set; }
        public double job_eng_in_hand { get; set; }
        public double target_month { get; set; }
    }
    public class JobENGInhandModel
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

    public class SummaryCISJobInHandModel
    {
        public string month { get; set; }
        public double job_cis_in_hand { get; set; }
        public double target_month { get; set; }
    }
    public class JobCISInhandModel
    {
        public string job_id { get; set; }
        public string customer_name { get; set; }
        public string job_name { get; set; }
        public string job_type { get; set; }
        public double job_in_hand { get; set; }
        public double job_cis_in_hand { get; set; }
        public double invoice { get; set; }
        public double invoice_cis { get; set; }
        public double percent_cis_cost { get; set; }
        public double percent_invoice { get; set; }
        public double remaining_percent_invoice { get; set; }
        public double remaining_amount { get; set; }
    }
    public class SummaryAISJobInHandModel
    {
        public string month { get; set; }
        public double job_ais_in_hand { get; set; }
        public double target_month { get; set; }
    }
    public class JobAISInhandModel
    {
        public string job_id { get; set; }
        public string customer_name { get; set; }
        public string job_name { get; set; }
        public string job_type { get; set; }
        public double job_in_hand { get; set; }
        public double job_ais_in_hand { get; set; }
        public double invoice { get; set; }
        public double invoice_ais { get; set; }
        public double percent_ais_cost { get; set; }
        public double percent_invoice { get; set; }
        public double remaining_percent_invoice { get; set; }
        public double remaining_amount { get; set; }
    }
}
