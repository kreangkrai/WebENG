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
        public string emd_name { get; set; }
        public string level { get; set; }
        public string department { get; set; }
        public bool is_active { get; set; }
    }    
}
