using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public partial class TimeAttendanceModel
    {
        public string emp_id { get; set; }
        public string department { get; set; }
        public double entitlement_al { get; set; }
        public string name_en { get; set; }
        public string name_th { get; set; }
        public string position { get; set; }
        public List<LeaveTimeAttendanceModel> leaves { get; set; }
    }

    public partial class LeaveTimeAttendanceModel
    {
        public string leave_type_code { get; set; }
        public string leave_name_en { get; set; }
        public string leave_name_th { get; set; }
        public List<MonthlyAccumulatedModel> MonthlyAccumulated { get; set; }
    }

    public partial class MonthlyAccumulatedModel
    {
        public int Month { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal AccumulatedAmount { get; set; }
    }
}
