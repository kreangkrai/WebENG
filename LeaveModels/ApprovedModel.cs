using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class ApprovedModel
    {
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public int level { get; set; }
        public string status { get; set; }
        public DateTime date { get; set; }
    }
}
