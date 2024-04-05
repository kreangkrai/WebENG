using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class QuarterModel
    {
        public string job_id { get; set; }
        public DateTime job_date { get; set; }
        public int quarter { get; set; }
        public string type { get; set; }
        public double job_eng_in_hand { get; set; }
        public double invoice_eng { get; set; }
        public string status { get; set; }
        public DateTime finished_date { get; set; }
    }
    public class SummaryQuarterModel
    {
        public string quarter { get; set; }
        public double job_in_hand_volume { get; set; }
        public int job_in_hand { get; set; }
        public double invoice_volume { get; set; }
        public int invoice { get; set; }
        public double pending_volume { get; set; }
        public int pending { get; set; }
    }
}
