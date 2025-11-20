using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class ApproverModel
    {
        public int id { get; set; }
        public string approver_id { get; set; }
        public string approver_name { get; set; }
        public string department { get; set; }
        public int approver_level { get; set; }
        public bool is_active { get; set; }
    }
}
