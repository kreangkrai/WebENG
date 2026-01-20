using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class InvoicesModel
    {
        public string job_id { get; set; }
        public double job_in_hand { get; set; }
        public double job_eng_in_hand { get; set; }
        public double job_cis_in_hand { get; set; }
        public double job_ais_in_hand { get; set; }
        public string responsible { get; set; }
        public DateTime job_date { get; set; }
        public string department { get; set; }
        public double invoice { get; set; }
        public DateTime invoice_date { get; set; }
    }
}
