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
        public int current_level { get; set; }
        public int next_level { get; set; }
        public string status { get; set; }
        public DateTime date { get; set; }
        public bool is_two_step_approve { get; set; }
    }
}
