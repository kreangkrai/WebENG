using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class ForecastModel
    {
        public string month { get; set; }
        public string job_id { get; set; }
        public string job_name { get; set; }
        public double job_in_hand { get; set; }
        public string payment_id { get; set; }
        public string payment_name { get; set; }
        public string forecast_month { get; set; }
        public int percent { get; set; }
        public double forecast_amount { get; set; }
        public string forecast_remark { get; set; }
        public string actual_month { get; set; }
        public double actual_amount { get; set; }
        public DateTime actual_date { get; set; }
        public string status { get; set; }
        public string actual_remark { get; set; }
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
