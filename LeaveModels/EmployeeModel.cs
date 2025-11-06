using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class EmployeeModel
    {
        public string emp_id { get; set; }
        public string name_en { get; set; }
        public string name_th { get; set; }
        public string department { get; set; }
        public string location { get; set; }
        public DateTime start_date { get; set; }
        public DateTime promote_manager_date { get; set; }
        public string position { get; set; }
        public string gender { get; set; }
        public string phone { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public bool active { get; set; }
        public string role { get; set; }
    }
}
