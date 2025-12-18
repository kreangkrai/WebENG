using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.TripModels
{
    public class TripExpenseModel
    {
        public string passenger { get; set; }
        public string emp_id { get; set; }
        public DateTime date { get; set; }
        public DateTime trip { get; set; }
        public string status { get; set; }
        public string location { get; set; }
    }
}
