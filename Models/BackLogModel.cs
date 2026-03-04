using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class BackLogModel
    {
        public string job_id { get; set; }
        public string job_type { get; set; }
        public string job_name { get; set; }
        public DateTime job_date { get; set; }
        public string customer_name { get; set; }
        public string sale_department { get; set; }
        public string sale { get; set; }
        public string status { get; set; }
        public string status_name { get; set; }
        public double job_in_hand { get; set; }
        public double job_eng_in_hand { get; set; }
        public double job_cis_in_hand { get; set; }
        public double job_ais_in_hand { get; set; }
        public double eng_percent { get; set; }
        public double cis_percent { get; set; }
        public double ais_percent { get; set; }
        public double eng_invoice { get; set; }
        public double cis_invoice { get; set; }
        public double ais_invoice { get; set; }
        public double eng_percent_invoice { get; set; }
        public double cis_percent_invoice { get; set; }
        public double ais_percent_invoice { get; set; }
        public double total_invoice { get; set; }
        public double remaining_in_hand { get; set; }
        public DateTime finished_date { get; set; }
        public string responsible { get; set; }
        public string responsible_department { get; set; }
        public string department { get; set; }
    }
}
