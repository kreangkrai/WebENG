using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class JobSummaryModel
    {
        public string emp_id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public int levels { get; set; }
        public string jobId { get; set; }
        public string jobName { get; set; }
        public string customer { get; set; }
        public string responsible { get; set; }
        public double eng_cost { get; set; }
        public double cis_cost { get; set; }
        public double ais_cost { get; set; }
        public double factor { get; set; }
        public double totalManhour { get; set; }
        public double totalOTManhour { get; set; }
        public double totalCost { get; set; }
        public double totalAmount { get; set; }
        public double totalOTAmount { get; set; }
        public string status { get; set; }
        public string process { get; set; }
        public string system { get; set; }
        public double remainingCost { get; set; }
        public double remainingOTCost { get; set; }
        public List<Term_PaymentsModel> term_payments { get; set; }
    }
}
