using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Models
{
    public class Form_DailyReportModel
    {
        public string name { get; set; }
        public string month { get; set; }
        public List<DailyActivityModel> datas { get; set; }
    }
}
