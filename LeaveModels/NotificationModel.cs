using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.LeaveModels
{
    public class NotificationModel
    {
        public int notification_id { get; set; }
        public string notification_type { get; set; }
        public string emp_id { get; set; }
        public string notification_issue { get; set; }
        public DateTime notification_date { get; set; }
        public string notification_description { get; set; }
        public string notification_path { get; set; }
        public string status { get; set; }
    }
}
