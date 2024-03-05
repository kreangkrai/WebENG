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
        public DateTime finished_date { get; set; }
    }
}
