using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class OrderInTakeENGModel
    {
        public int year { get; set; }
        public double job_eng_in_hand { get; set; }
        public double backlog { get; set; }
        public int target { get; set; }
    }
    public class OrderInTakeCISModel
    {
        public int year { get; set; }
        public double job_cis_in_hand { get; set; }
        public double backlog { get; set; }
        public int target { get; set; }
    }
    public class OrderInTakeAISModel
    {
        public int year { get; set; }
        public double job_ais_in_hand { get; set; }
        public double backlog { get; set; }
        public int target { get; set; }
    }
}
