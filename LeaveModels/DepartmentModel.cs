using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class DepartmentModel
    {
        public int id { get; set; }
        public string department { get; set; }
        public string department_name { get; set; }
        public int approver_level { get; set; }
        public string manager_id { get; set; }
        public string manager_name { get; set; }
        public bool is_active { get; set; }
    }
}
