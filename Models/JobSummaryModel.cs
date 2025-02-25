using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class JobSummaryModel
    {
        public string user_id { get; set; }
        public int levels { get; set; }
        public string jobId { get; set; }
        public string jobName { get; set; }
        public string customer { get; set; }
        public string responsible { get; set; }
        public int cost { get; set; }
        public double factor { get; set; }
        public double totalManhour { get; set; }
        public int totalEngCost { get; set; }
        public string status { get; set; }
        public string process { get; set; }
        public string system { get; set; }
        public int remainingCost { get; set; }
    }
}
