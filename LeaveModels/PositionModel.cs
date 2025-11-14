using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class PositionModel
    {
        public int position_id { get; set; }
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string img { get; set; }
        public string level { get; set; }
        public string position { get; set; }
        public string department { get; set; }
        public bool is_active { get; set; }
    }
    public class LeavePositionModel
    {
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string img { get; set; }
        public string position { get; set; }
        public List<string> manager_departments { get; set; }
        public bool is_director { get; set; }
        public bool is_auditor { get; set; }
        public bool is_active { get; set; }
    }
}
