using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.HRModel
{
    public class EmployeeWorkModel
    {
        public DateTime date { get; set; }
        public string emp_id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public string location_face_scan { get; set; }
        public string location_trip_expense { get; set; }
        public string shift_time { get; set; }
        public TimeSpan start_time_face_scan { get; set; }
        public TimeSpan last_time_face_scan { get; set; }
        public TimeSpan actual_start_time_trip_expense { get; set; }
        public TimeSpan actual_last_time_trip_expense { get; set; }
        public TimeSpan start_time_trip_expense { get; set; }
        public TimeSpan last_time_trip_expense { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan last_time { get; set; }
        public TimeSpan adjust_start_time { get; set; }
        public TimeSpan adjust_last_time { get; set; }
        public TimeSpan working_hour { get; set; }
        public int late_time { get; set; }
        public TimeSpan start { get; set; }
        public TimeSpan stop { get; set; }
    }
    public class EmployeeTimeModel
    {
        public DateTime date { get; set; }
        public string emp_id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public string type { get; set; }
        public string location { get; set; }
        public DateTime start_time { get; set; }
        public DateTime last_time { get; set; }
    }
    public class EmployeeMonthTimeModel
    {
        public DateTime date { get; set; }
        public string emp_id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public string type { get; set; }
        public string location { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan last_time { get; set; }
    }
    public class EmployeeTimesModel
    {
        public DateTime date { get; set; }
        public string emp_id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public string type { get; set; }
        public string location { get; set; }
        public string personal_group { get; set; }
        public string sn { get; set; }
        public TimeSpan actual_start_time { get; set; }
        public TimeSpan actual_last_time { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan last_time { get; set; }
        public TimeSpan start { get; set; }
        public TimeSpan stop { get; set; }
    }
}
