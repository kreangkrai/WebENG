using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class ForecastModel
    {
        public string job_id { get; set; }
        public string job_name { get; set; }
        public string responsible { get; set; }
        public string responsible_department { get; set; }
        public double job_in_hand { get; set; }
        public double department_in_hand { get; set; }
        public string job_type { get; set; }
        public string department { get; set; }
        public string payment_id { get; set; }
        public string milestone { get; set; }
        public string forecast_month { get; set; }
        public int percent { get; set; }
        public double forecast_portion_amount { get; set; }
        public double forecast_portion_department_amount { get; set; }
        public string forecast_remark { get; set; }
        public double invoice_department_portion { get; set; }
        public double total_invoice { get; set; }
        public DateTime actual_invoice_date { get; set; }

    }
    public class ForecastPaymentModel
    {
        public double[] actual_amount { get; set; } = new double[14] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public double[] forecast_amount { get; set; } = new double[14] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public double[] acc_actual_amount { get; set; } = new double[14] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public double[] acc_forecast_amount { get; set; } = new double[14] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public string[] month_label { get; set; }
    }
}
