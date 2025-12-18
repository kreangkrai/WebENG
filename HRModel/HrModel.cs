using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.HRModel
{
    public class HrModel
    {
        public int id { get; set; }
        public string employeeid { get; set; }
        public DateTime sdatetime { get; set; }
        public DateTime datetime { get; set; }
        public DateTime date { get; set; }
        public TimeSpan time { get; set; }
        public string devicename { get; set; }
        public string sn { get; set; }
        public string readername { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string personname { get; set; }
        public string persongroup { get; set; }
        public string cn { get; set; }
        public string autr { get; set; }
        public string autt { get; set; }
        public string levels { get; set; }
    }
}
