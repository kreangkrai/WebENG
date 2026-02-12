using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;

namespace WebENG.Service
{
    public class DailyReportService : IDailyReport
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public DailyReportService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<DailyActivityModel> GetDailyActivities(string emp_id, DateTime start_date, DateTime stop_date)
        {
            List<DailyActivityModel> dlrs = new List<DailyActivityModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@" SELECT
	                    ind,
						WorkingHours.emp_id,
                        working_date,
                        start_time,
                        stop_time,
                        WorkingHours.job_id,
                        Jobs.job_name,
                        WorkingHours.task_id,
                        Tasks.task_name,
                        activity,
                        problem,
                        solution,
                        tomorrow_plan,
	                    emp.name_en as user_name,
                        Quotation.customer,
                        WorkingHours.note
                    FROM WorkingHours 
                    LEFT JOIN CTL.dbo.Employees emp ON WorkingHours.emp_id = emp.emp_id
                    LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                    LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
                    LEFT JOIN Quotation ON Jobs.quotation_no = Quotation.quotation_no
                    WHERE WorkingHours.emp_id = '{emp_id}'
                    AND working_date BETWEEN '{start_date.ToString("yyyy-MM-dd")}' AND '{stop_date.ToString("yyyy-MM-dd")}'
                    ORDER BY working_date, start_time;");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DailyActivityModel dlr = new DailyActivityModel()
                        {
                            ind = dr["ind"].ToString(),
                            date = dr["working_date"] != DBNull.Value ? Convert.ToDateTime(dr["working_date"]) : default(DateTime),
                            start_time = dr["start_time"] != DBNull.Value ? TimeSpan.Parse(dr["start_time"].ToString()) : default(TimeSpan),
                            stop_time = dr["stop_time"] != DBNull.Value ? TimeSpan.Parse(dr["stop_time"].ToString()) : default(TimeSpan),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            emp_id = dr["emp_id"] != DBNull.Value ? dr["emp_id"].ToString() : "",
                            user_name = dr["user_name"] != DBNull.Value ? dr["user_name"].ToString() : "",
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                            activity = dr["activity"] != DBNull.Value ? dr["activity"].ToString() : "",
                            problem = dr["problem"] != DBNull.Value ? dr["problem"].ToString() : "",
                            solution = dr["solution"] != DBNull.Value ? dr["solution"].ToString() : "",
                            tomorrow_plan = dr["tomorrow_plan"] != DBNull.Value ? dr["tomorrow_plan"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                        };
                        dlrs.Add(dlr);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return dlrs;
        }

        public string EditDailyReport(DailyActivityModel dlr)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    UPDATE WorkingHours
                    SET
                        problem = @problem,
                        solution = @solution,
                        tomorrow_plan = @tomorrow_plan,
                        customer = @customer
                    WHERE ind = @ind");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@problem", dlr.problem);
                    cmd.Parameters.AddWithValue("@solution", dlr.solution);
                    cmd.Parameters.AddWithValue("@tomorrow_plan", dlr.tomorrow_plan);
                    cmd.Parameters.AddWithValue("@customer", dlr.customer);
                    cmd.Parameters.AddWithValue("@ind", dlr.ind);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return "Success";
        }

        public List<DailyActivityModel> GetDailyActivities(string emp_id, string month)
        {
            int yy = Convert.ToInt32(month.Split("-")[0]);
            int mm = Convert.ToInt32(month.Split("-")[1]);
            List<DailyActivityModel> dlrs = new List<DailyActivityModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    SELECT
	                    ind,
                        working_date,
                        start_time,
                        stop_time,
                        WorkingHours.job_id,
                        Jobs.job_name,
                        WorkingHours.task_id,
                        Tasks.task_name,
                        activity,
                        problem,
                        solution,
                        tomorrow_plan,
                        WorkingHours.emp_id,
	                    emp.name_en as user_name,
                        Quotation.customer,
                        WorkingHours.note
                    FROM WorkingHours 
                    LEFT JOIN CTL.dbo.Employees emp ON WorkingHours.emp_id = emp.emp_id
                    LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                    LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
                    LEFT JOIN Quotation ON Jobs.quotation_no = Quotation.quotation_no
                    WHERE  WorkingHours.emp_id = '{emp_id}'
                    AND YEAR(working_date) = {yy} AND MONTH(working_date) = {mm}
                    ORDER BY working_date, start_time;
                ");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DailyActivityModel dlr = new DailyActivityModel()
                        {
                            ind = dr["ind"].ToString(),
                            date = dr["working_date"] != DBNull.Value ? Convert.ToDateTime(dr["working_date"]) : default(DateTime),
                            start_time = dr["start_time"] != DBNull.Value ? TimeSpan.Parse(dr["start_time"].ToString()) : default(TimeSpan),
                            stop_time = dr["stop_time"] != DBNull.Value ? TimeSpan.Parse(dr["stop_time"].ToString()) : default(TimeSpan),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            emp_id = dr["emp_id"] != DBNull.Value ? dr["emp_id"].ToString() : "",
                            user_name = dr["user_name"] != DBNull.Value ? dr["user_name"].ToString() : "",
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                            activity = dr["activity"] != DBNull.Value ? dr["activity"].ToString() : "",
                            problem = dr["problem"] != DBNull.Value ? dr["problem"].ToString() : "",
                            solution = dr["solution"] != DBNull.Value ? dr["solution"].ToString() : "",
                            tomorrow_plan = dr["tomorrow_plan"] != DBNull.Value ? dr["tomorrow_plan"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                        };
                        dlrs.Add(dlr);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return dlrs;
        }

        public List<DailyActivityModel> GetServiceActivities(string job_id)
        {
            List<DailyActivityModel> dlrs = new List<DailyActivityModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    SELECT
	                    ind,
                        working_date,
                        start_time,
                        stop_time,
                        WorkingHours.job_id,
                        Jobs.job_name,
                        WorkingHours.task_id,
                        Tasks.task_name,
                        activity,
                        problem,
                        solution,
                        tomorrow_plan,
                        WorkingHours.emp_id,
	                    emp.name_en as user_name,
                        Quotation.customer,
                        WorkingHours.note
                    FROM WorkingHours 
                    LEFT JOIN CTL.dbo.Employees emp ON WorkingHours.emp_id = emp.emp_id
                    LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                    LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
                    LEFT JOIN Quotation ON Jobs.quotation_no = Quotation.quotation_no
                    WHERE WorkingHours.job_id = '{job_id}'                     
                    ORDER BY working_date, start_time");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DailyActivityModel dlr = new DailyActivityModel()
                        {
                            ind = dr["ind"].ToString(),
                            date = dr["working_date"] != DBNull.Value ? Convert.ToDateTime(dr["working_date"]) : default(DateTime),
                            start_time = dr["start_time"] != DBNull.Value ? TimeSpan.Parse(dr["start_time"].ToString()) : default(TimeSpan),
                            stop_time = dr["stop_time"] != DBNull.Value ? TimeSpan.Parse(dr["stop_time"].ToString()) : default(TimeSpan),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            emp_id = dr["emp_id"] != DBNull.Value ? dr["emp_id"].ToString() : "",
                            user_name = dr["user_name"] != DBNull.Value ? dr["user_name"].ToString() : "",
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                            activity = dr["activity"] != DBNull.Value ? dr["activity"].ToString() : "",
                            problem = dr["problem"] != DBNull.Value ? dr["problem"].ToString() : "",
                            solution = dr["solution"] != DBNull.Value ? dr["solution"].ToString() : "",
                            tomorrow_plan = dr["tomorrow_plan"] != DBNull.Value ? dr["tomorrow_plan"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                        };
                        dlrs.Add(dlr);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return dlrs;
        }
    }
}
