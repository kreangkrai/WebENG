using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class JobModel
    {
        public string job_id { get; set; }
        public string job_name { get; set; }
        public DateTime job_date { get; set; }
        public string sale_department { get; set; }
        public string sale { get; set; }
        public double gp { get; set; }
        public double est_cost { get; set; }
        public double eng_cost { get; set; }
        public double cis_cost { get; set; }
        public double ais_cost { get; set; }
        public double total_cost { get; set; }
        public double remaining_cost { get; set; }
        public double md_rate { get; set; }
        public double pd_rate { get; set; }
        public double factor { get; set; }
        public double manpower { get; set; }
        public double cost_per_manpower { get; set; }
        public double ot_manpower { get; set; }
        public string status { get; set; } 
        public string quotation_no { get; set; }
        public string job_type { get; set; }
        public string customer { get; set; }
        public string enduser { get; set; }
        public string sale_name { get; set; }
        public string sale_id { get; set; }
        public string department { get; set; }
        public string process { get; set; }
        public string system { get; set; }
        public Term_PaymentModel term_payment { get; set; }
        public List<Term_PaymentsModel> term_payments { get; set; }
        public double job_in_hand { get; set; }
        public double job_eng_in_hand { get; set; }
        public double job_cis_in_hand { get; set; }
        public double job_ais_in_hand { get; set; }
        public List<InvoiceModel> invoices { get; set; } = new List<InvoiceModel>();
        public double eng_invoice { get; set; }
        public double cis_invoice { get; set; }
        public double ais_invoice { get; set; }
        public List<JobSummaryModel> job_summary { get; set; }
        public DateTime due_date { get; set; }
        public DateTime finished_date { get; set; }
        public int warranty_period { get; set; }
        public int bank_guarantee { get; set; }
        public DateTime bg_start { get; set; }
        public DateTime bg_finish { get; set; }
        public int retention { get; set; }
        public string responsible { get; set; }
        public string responsible_id { get; set; }
        public string note { get; set; }
    }

    public class InvoiceModel
    {
        public string job_id { get; set; }
        public double invoice { get; set; }
        public DateTime plan_date { get; set; }
        public DateTime actual_date { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public DateTime new_plan_date { get; set; }
        public string milestone { get; set; }
    }
    public class Term_PaymentModel
    {
        public string job_id { get; set; }
        public int down_payment { get; set; }
        public int document_submit { get; set; }
        public int instrument_vendor { get; set; }
        public int instrument_delivered_ctl { get; set; }
        public int system_delivered_ctl { get; set; }
        public int fat { get; set; }
        public int delivery_instrument { get; set; }
        public int delivery_system { get; set; }
        public int progress_work { get; set; }
        public List<Term_ProgressModel> progress_works { get; set; }
        public int installation_work_complete { get; set; }
        public int commissioning { get; set; }
        public int startup { get; set; }
        public int as_built { get; set; }
        public int warranty { get; set; }
        public int finished { get; set; }
        public int complete { get; set; }
        public int after_hmc { get; set; }
    }

    public class Term_ProgressModel
    {
        public string progress_name { get; set; }
        public int progress_value { get; set; }
    }
    public class Term_PaymentsModel
    {
        public string job_id { get; set; }
        public string payment_id { get; set; }
        public string payment_name { get; set; }
        public int percent { get; set; }
        public string forecast_month { get; set; }
        public string remark { get; set; }     
    }

    public class JobProcessSystemModel
    {
        public string job_id { get; set; }
        public List<EngProcessModel> processes { get; set; } = new List<EngProcessModel>();
        public List<EngSystemModel> systems { get; set; } = new List<EngSystemModel>();
    }
}
