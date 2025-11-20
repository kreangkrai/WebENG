using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class CheckerModel
    {
        public int id { get; set; }
        public string checker_id { get; set; }
        public string checker_name { get; set; }
        public int checker_level { get; set; }
        public bool is_active { get; set; }
    }
}
