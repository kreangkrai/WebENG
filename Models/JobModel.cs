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
        public string sale_department { get; set; }
        public string sale { get; set; }
        public int cost { get; set; }
        public double md_rate { get; set; }
        public double pd_rate { get; set; }
        public double factor { get; set; }
        public double manpower { get; set; }
        public double cost_per_manpower { get; set; }
        public double ot_manpower { get; set; }
        public string status { get; set; } 
        public string quotation_no { get; set; }
        public string customer { get; set; }
        public string enduser { get; set; }
        public string sale_name { get; set; }
        public string department { get; set; }
        public string process { get; set; }
        public string system { get; set; }
        public int down_payment { get; set; }
        public int document_submit { get; set; }
        public int instrument_delivered_ctl { get; set; }
        public int system_delivered_ctl { get; set; }
        public int fat { get; set; }
        public int delivery_instrument { get; set; }
        public int delivery_system { get; set; }
        public int progress_work { get; set; }
        public int commissioning { get; set; }
        public int as_built { get; set; }
        public int finish { get; set; }
        public int job_in_hand { get; set; }
        public int invoice { get; set; }
        public DateTime due_date { get; set; }
        public DateTime finished_date { get; set; }
    }
}
