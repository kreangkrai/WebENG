using WebENG.Interface;
using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Service;

namespace WebENG.Service
{
    public class WorkingHoursService : IWorkingHours
    {
        IHoliday Holiday;
        public WorkingHoursService()
        {
            Holiday = new HolidayService();
        }
        public List<WorkingHoursModel> GetWorkingHours()
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT
                        WorkingHours.ind,
                        WorkingHours.user_id,
                        Authen.name,
                        Authen.department,
                        WorkingHours.working_date,
                        WorkingHours.week_number,
                        WorkingHours.job_id,
					    Eng_Process.Process_ID as process_id,
                        Eng_Process.process_Name as process_name,
						Eng_System.System_ID as system_id,
                        Eng_System.system_Name as system_name,
                        Jobs.job_name,
                        WorkingHours.task_id,
                        Tasks.task_name,
                        WorkingHours.start_time,
                        WorkingHours.stop_time,
                        WorkingHours.lunch,
                        WorkingHours.dinner,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        WorkingHoursModel wh = new WorkingHoursModel()
                        {
                            index = dr["ind"].ToString(),
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            user_name = dr["name"] != DBNull.Value ? dr["name"].ToString() : "",
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            working_date = dr["working_date"] != DBNull.Value ? Convert.ToDateTime(dr["working_date"]) : default(DateTime),
                            week_number = dr["week_number"] != DBNull.Value ? Convert.ToInt32(dr["week_number"]) : default(Int32),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            process_id = dr["process_id"] != DBNull.Value ? dr["process_id"].ToString() : "",
                            process_name = dr["process_name"] != DBNull.Value ? dr["process_name"].ToString() : "",
                            system_id = dr["system_id"] != DBNull.Value ? dr["system_id"].ToString() : "",
                            system_name = dr["system_name"] != DBNull.Value ? dr["system_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            start_time = dr["start_time"] != DBNull.Value ? TimeSpan.Parse(dr["start_time"].ToString()) : default(TimeSpan),
                            stop_time = dr["stop_time"] != DBNull.Value ? TimeSpan.Parse(dr["stop_time"].ToString()) : default(TimeSpan),
                            lunch = dr["lunch"] != DBNull.Value ? Convert.ToBoolean(dr["lunch"].ToString()) : default(bool),
                            dinner = dr["dinner"] != DBNull.Value ? Convert.ToBoolean(dr["dinner"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return whs;
        }

        public List<WorkingHoursModel> GetWorkingHours(string user_name)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT
                        WorkingHours.ind,
                        WorkingHours.user_id,
                        Authen.name,
                        Authen.department,
                        WorkingHours.working_date,
                        WorkingHours.week_number,
                        WorkingHours.job_id,
					    Eng_Process.Process_ID as process_id,
                        Eng_Process.process_Name as process_name,
						Eng_System.System_ID as system_id,
                        Eng_System.system_Name as system_name,
                        Jobs.job_name,
                        WorkingHours.task_id,
                        Tasks.task_name,
                        WorkingHours.start_time,
                        WorkingHours.stop_time,
                        WorkingHours.lunch,
                        WorkingHours.dinner,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID
                    WHERE LOWER(Authen.name) = '{user_name}'");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        WorkingHoursModel wh = new WorkingHoursModel()
                        {
                            index = dr["ind"].ToString(),
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            user_name = dr["name"] != DBNull.Value ? dr["name"].ToString() : "",
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            working_date = dr["working_date"] != DBNull.Value ? Convert.ToDateTime(dr["working_date"]) : default(DateTime),
                            week_number = dr["week_number"] != DBNull.Value ? Convert.ToInt32(dr["week_number"]) : default(Int32),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            process_id = dr["process_id"] != DBNull.Value ? dr["process_id"].ToString() : "",
                            process_name = dr["process_name"] != DBNull.Value ? dr["process_name"].ToString() : "",
                            system_id = dr["system_id"] != DBNull.Value ? dr["system_id"].ToString() : "",
                            system_name = dr["system_name"] != DBNull.Value ? dr["system_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            start_time = dr["start_time"] != DBNull.Value ? TimeSpan.Parse(dr["start_time"].ToString()) : default(TimeSpan),
                            stop_time = dr["stop_time"] != DBNull.Value ? TimeSpan.Parse(dr["stop_time"].ToString()) : default(TimeSpan),
                            lunch = dr["lunch"] != DBNull.Value ? Convert.ToBoolean(dr["lunch"].ToString()) : default(bool),
                            dinner = dr["dinner"] != DBNull.Value ? Convert.ToBoolean(dr["dinner"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return whs;
        }

        public List<WorkingHoursModel> GetWorkingHours(string year, string month, string user_name)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT
                        WorkingHours.ind,
                        WorkingHours.user_id,
                        Authen.name,
                        Authen.department,
                        WorkingHours.working_date,
                        WorkingHours.week_number,
                        WorkingHours.job_id,
					    Eng_Process.Process_ID as process_id,
                        Eng_Process.process_Name as process_name,
						Eng_System.System_ID as system_id,
                        Eng_System.system_Name as system_name,
                        Jobs.job_name,
                        WorkingHours.task_id,
                        Tasks.task_name,
                        WorkingHours.start_time,
                        WorkingHours.stop_time,
                        WorkingHours.lunch,
                        WorkingHours.dinner,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID
                    WHERE WorkingHours.working_date like '{year}-{month}%' 
                    AND LOWER(Authen.name) ='{user_name}'");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        WorkingHoursModel wh = new WorkingHoursModel()
                        {
                            index = dr["ind"].ToString(),
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            user_name = dr["name"] != DBNull.Value ? dr["name"].ToString() : "",
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            working_date = dr["working_date"] != DBNull.Value ? Convert.ToDateTime(dr["working_date"]) : default(DateTime),
                            week_number = dr["week_number"] != DBNull.Value ? Convert.ToInt32(dr["week_number"]) : default(Int32),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            process_id = dr["process_id"] != DBNull.Value ? dr["process_id"].ToString() : "",
                            process_name = dr["process_name"] != DBNull.Value ? dr["process_name"].ToString() : "",
                            system_id = dr["system_id"] != DBNull.Value ? dr["system_id"].ToString() : "",
                            system_name = dr["system_name"] != DBNull.Value ? dr["system_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            start_time = dr["start_time"] != DBNull.Value ? TimeSpan.Parse(dr["start_time"].ToString()) : default(TimeSpan),
                            stop_time = dr["stop_time"] != DBNull.Value ? TimeSpan.Parse(dr["stop_time"].ToString()) : default(TimeSpan),
                            lunch = dr["lunch"] != DBNull.Value ? Convert.ToBoolean(dr["lunch"].ToString()) : default(bool),
                            dinner = dr["dinner"] != DBNull.Value ? Convert.ToBoolean(dr["dinner"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return whs;
        }

        public List<WorkingHoursModel> GetWorkingHours(string user_name, DateTime working_date)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT
                        WorkingHours.ind,
                        WorkingHours.user_id,
                        Authen.name,
                        Authen.department,
                        WorkingHours.working_date,
                        WorkingHours.week_number,
                        WorkingHours.job_id,
					    Eng_Process.Process_ID as process_id,
                        Eng_Process.process_Name as process_name,
						Eng_System.System_ID as system_id,
                        Eng_System.system_Name as system_name,
                        Jobs.job_name,
                        WorkingHours.task_id,
                        Tasks.task_name,
                        WorkingHours.start_time,
                        WorkingHours.stop_time,
                        WorkingHours.lunch,
                        WorkingHours.dinner,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID
                    WHERE LOWER(Authen.name) = '{user_name}'
                    AND WorkingHours.working_date LIKE '{working_date.ToString("yyyy-MM-dd")}'");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        WorkingHoursModel wh = new WorkingHoursModel()
                        {
                            index = dr["ind"].ToString(),
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            user_name = dr["name"] != DBNull.Value ? dr["name"].ToString() : "",
                            working_date = dr["working_date"] != DBNull.Value ? Convert.ToDateTime(dr["working_date"]) : default(DateTime),
                            week_number = dr["week_number"] != DBNull.Value ? Convert.ToInt32(dr["week_number"]) : default(Int32),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            process_id = dr["process_id"] != DBNull.Value ? dr["process_id"].ToString() : "",
                            process_name = dr["process_name"] != DBNull.Value ? dr["process_name"].ToString() : "",
                            system_id = dr["system_id"] != DBNull.Value ? dr["system_id"].ToString() : "",
                            system_name = dr["system_name"] != DBNull.Value ? dr["system_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            start_time = dr["start_time"] != DBNull.Value ? TimeSpan.Parse(dr["start_time"].ToString()) : default(TimeSpan),
                            stop_time = dr["stop_time"] != DBNull.Value ? TimeSpan.Parse(dr["stop_time"].ToString()) : default(TimeSpan),
                            lunch = dr["lunch"] != DBNull.Value ? Convert.ToBoolean(dr["lunch"].ToString()) : default(bool),
                            dinner = dr["dinner"] != DBNull.Value ? Convert.ToBoolean(dr["dinner"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return whs;
        }

        public string AddWorkingHours(WorkingHoursModel wh)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO WorkingHours(
                        ind,user_id, working_date, week_number, job_id,process_id,system_id, task_id, start_time, stop_time, lunch, dinner, note)
                    VALUES (
                        @ind,@user_id, @working_date, (SELECT DATEPART(ISO_WEEK,@working_date)), @job_id,@process_id,@system_id, @task_id, @start_time, @stop_time, @lunch, @dinner, @note)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@ind", wh.index);
                    cmd.Parameters.AddWithValue("@user_id", wh.user_id);
                    cmd.Parameters.AddWithValue("@working_date", wh.working_date);
                    cmd.Parameters.AddWithValue("@job_id", wh.job_id);
                    if (wh.job_id == "J999999")
                    {
                        cmd.Parameters.AddWithValue("@process_id", DBNull.Value);
                        cmd.Parameters.AddWithValue("@system_id", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@process_id", wh.process_id);
                        cmd.Parameters.AddWithValue("@system_id", wh.system_id);
                    }
                    
                    cmd.Parameters.AddWithValue("@task_id", wh.task_id);
                    cmd.Parameters.AddWithValue("@start_time", wh.start_time);
                    cmd.Parameters.AddWithValue("@stop_time", wh.stop_time);
                    cmd.Parameters.AddWithValue("@lunch", wh.lunch);
                    cmd.Parameters.AddWithValue("@dinner", wh.dinner);
                    cmd.Parameters.AddWithValue("@note", wh.note);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return "Success";
        }

        public string UpdateWorkingHours(WorkingHoursModel wh)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE WorkingHours 
                    SET
                        user_id = @user_id,
                        working_date = @working_date,
                        week_number = (SELECT DATEPART(ISO_WEEK,@working_date)),
                        job_id = @job_id,
                        process_id = @process_id,
                        system_id = @system_id,
                        task_id = @task_id,
                        start_time = @start_time,
                        stop_time = @stop_time,
                        lunch = @lunch,
                        dinner = @dinner,
                        note = @note
                    WHERE ind = @ind");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@user_id", wh.user_id);
                    cmd.Parameters.AddWithValue("@working_date", wh.working_date);
                    cmd.Parameters.AddWithValue("@job_id", wh.job_id);

                    if (wh.job_id == "J999999")
                    {
                        cmd.Parameters.AddWithValue("@process_id", DBNull.Value);
                        cmd.Parameters.AddWithValue("@system_id", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@process_id", wh.process_id);
                        cmd.Parameters.AddWithValue("@system_id", wh.system_id);
                    }
                    cmd.Parameters.AddWithValue("@task_id", wh.task_id);
                    cmd.Parameters.AddWithValue("@start_time", wh.start_time);
                    cmd.Parameters.AddWithValue("@stop_time", wh.stop_time);
                    cmd.Parameters.AddWithValue("@lunch", wh.lunch);
                    cmd.Parameters.AddWithValue("@dinner", wh.dinner);
                    cmd.Parameters.AddWithValue("@note", wh.note);
                    cmd.Parameters.AddWithValue("@ind", wh.index);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return "Success";
        }

        public string UpdateRestTime(WorkingHoursModel wh)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE WorkingHours 
                    SET lunch = @lunch,
                        dinner = @dinner
                    WHERE user_id = @user_id 
                        AND working_date = @working_date 
                        AND job_id = @job_id
                        AND task_id = @task_id
                        AND start_time = @start_time
                        AND stop_time = @stop_time");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@lunch", wh.lunch);
                    cmd.Parameters.AddWithValue("@dinner", wh.dinner);
                    cmd.Parameters.AddWithValue("@user_id", wh.user_id);
                    cmd.Parameters.AddWithValue("@working_date", wh.working_date);
                    cmd.Parameters.AddWithValue("@job_id", wh.job_id);
                    cmd.Parameters.AddWithValue("@task_id", wh.task_id);
                    cmd.Parameters.AddWithValue("@start_time", wh.start_time);
                    cmd.Parameters.AddWithValue("@stop_time", wh.stop_time);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Pass");
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return "Success";
        }

        public string DeleteWorkingHours(WorkingHoursModel wh)
        {
            try
            {
                string string_command = string.Format($@"
                    DELETE FROM WorkingHours
                    WHERE user_id = @user_id
                        AND ind = @ind");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@user_id", wh.user_id);
                    cmd.Parameters.AddWithValue("@ind", wh.index);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return "Success";
        }

        public List<JobWeeklyWorkingHoursModel> GetAllJobWorkingHours(int year, int week)
        {
            List<JobWeeklyWorkingHoursModel> whs = new List<JobWeeklyWorkingHoursModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT DISTINCT 
                        WorkingHours.job_id,
                        Jobs.job_name,
                        Quotation.customer,
                        {year} as year,
                        {week} as week_number,
                        ISNULL(hours,0) AS hours 
                    FROM WorkingHours
                    LEFT JOIN ( 
                        SELECT job_id, week_number, 
                               SUM(CASE 
                                    WHEN DATEDIFF(HOUR, start_time, stop_time) > 0
                                    THEN DATEDIFF(HOUR, start_time, stop_time)
		                            ELSE DATEDIFF(HOUR, start_time, stop_time) * -1
	                               END) AS hours 
                        FROM WorkingHours 
                        WHERE working_date LIKE '{year}%' AND week_number = {week}
                        GROUP BY job_id, week_number) AS a ON WorkingHours.job_id = a.job_id
                    LEFT JOIN Jobs on WorkingHours.job_id = Jobs.job_id
                    LEFT JOIN Quotation on Jobs.quotation_no = Quotation.quotation_no");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        JobWeeklyWorkingHoursModel wh = new JobWeeklyWorkingHoursModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                            year = dr["year"] != DBNull.Value ? Convert.ToInt32(dr["year"]) : 0,
                            week = dr["week_number"] != DBNull.Value ? Convert.ToInt32(dr["week_number"]) : 0,
                            hours = dr["hours"] != DBNull.Value ? Convert.ToInt32(dr["hours"]) : 0
                        };
                        whs.Add(wh);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return whs;
        }

        public List<EngWeeklyWorkingHoursModel> GetAllEngWorkingHours(int year, int week)
        {
            List<EngWeeklyWorkingHoursModel> whs = new List<EngWeeklyWorkingHoursModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT 
	                    DISTINCT user_id,
	                    {week} as week_number,
	                    {year} as year,
	                    ISNULL(a.hours, 0) AS hours
                    FROM (
	                    SELECT 
		                    user_id, 
		                    SUM(CASE 
				                    WHEN DATEDIFF(HOUR,start_time,stop_time) > 0
				                    THEN DATEDIFF(HOUR,start_time,stop_time)
				                    ELSE DATEDIFF(HOUR,start_time,stop_time) * -1
			                    END) AS hours
	                    FROM WorkingHours
	                    WHERE working_date LIKE '{year}%' AND week_number = {week}
	                    GROUP BY user_id) AS a
                    ");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EngWeeklyWorkingHoursModel wh = new EngWeeklyWorkingHoursModel()
                        {
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            year = dr["year"] != DBNull.Value ? Convert.ToInt32(dr["year"]) : 0,
                            week = dr["week_number"] != DBNull.Value ? Convert.ToInt32(dr["week_number"]) : 0,
                            hours = dr["hours"] != DBNull.Value ? Convert.ToInt32(dr["hours"]) : 0
                        };
                        whs.Add(wh);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return whs;
        }

        public List<WorkingHoursModel> GetWorkingHours(int year, int week)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT
                        WorkingHours.ind,
                        WorkingHours.user_id,
                        Authen.name,
                        Authen.department,
                        WorkingHours.working_date,
                        WorkingHours.week_number,
                        WorkingHours.job_id,
					    Eng_Process.Process_ID as process_id,
                        Eng_Process.process_Name as process_name,
						Eng_System.System_ID as system_id,
                        Eng_System.system_Name as system_name,
                        Jobs.job_name,
                        WorkingHours.task_id,
                        Tasks.task_name,
                        WorkingHours.start_time,
                        WorkingHours.stop_time,
                        WorkingHours.lunch,
                        WorkingHours.dinner,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID
                    WHERE WorkingHours.working_date LIKE '{year}%' AND WorkingHours.week_number = {week}");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        WorkingHoursModel wh = new WorkingHoursModel()
                        {
                            index = dr["ind"].ToString(),
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            user_name = dr["name"] != DBNull.Value ? dr["name"].ToString() : "",
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            working_date = dr["working_date"] != DBNull.Value ? Convert.ToDateTime(dr["working_date"]) : default(DateTime),
                            week_number = dr["week_number"] != DBNull.Value ? Convert.ToInt32(dr["week_number"]) : default(Int32),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            process_id = dr["process_id"] != DBNull.Value ? dr["process_id"].ToString() : "",
                            process_name = dr["process_name"] != DBNull.Value ? dr["process_name"].ToString() : "",
                            system_id = dr["system_id"] != DBNull.Value ? dr["system_id"].ToString() : "",
                            system_name = dr["system_name"] != DBNull.Value ? dr["system_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            start_time = dr["start_time"] != DBNull.Value ? TimeSpan.Parse(dr["start_time"].ToString()) : default(TimeSpan),
                            stop_time = dr["stop_time"] != DBNull.Value ? TimeSpan.Parse(dr["stop_time"].ToString()) : default(TimeSpan),
                            lunch = dr["lunch"] != DBNull.Value ? Convert.ToBoolean(dr["lunch"].ToString()) : default(bool),
                            dinner = dr["dinner"] != DBNull.Value ? Convert.ToBoolean(dr["dinner"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return whs;
        }

        public int GetLastWorkingHoursID()
        {
            int id = 0;
            try
            {
                string string_command = string.Format($@"SELECT TOP 1 ind FROM WorkingHours WHERE ind LIKE 'WH%' ORDER BY ind DESC");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        id = dr["ind"] != DBNull.Value ? Convert.ToInt32(dr["ind"].ToString().Substring(2)) : 0;
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return id;
        }

        public List<WorkingHoursModel> CalculateWorkingHours(string user_name, string month)
        {
            List<WorkingHoursModel> monthly = new List<WorkingHoursModel>();

            string day = "";
            int yy = Convert.ToInt32(month.Split("-")[0]);
            int mm = Convert.ToInt32(month.Split("-")[1]);

            List<WorkingHoursModel> whs = GetWorkingHours(yy.ToString(), mm.ToString().PadLeft(2, '0'), user_name);
            List<HolidayModel> holidays = Holiday.GetHolidays(yy.ToString());
 
            whs = whs.OrderBy(o => o.working_date).ToList();
            int days = DateTime.DaysInMonth(yy, mm);
            for (int i = 0; i < days; i++)
            {
                DateTime date = new DateTime(yy, mm, i + 1);
                List<WorkingHoursModel> whd = whs.Where(w => w.working_date == date).ToList();

                bool _isHoliday = holidays.Where(w => w.date == date).Count() > 0 ? true : false;
               
                // Check Holiday and Get day
                if (_isHoliday)
                {
                    day = "Holiday";
                }
                else
                {
                    day = date.DayOfWeek.ToString();
                }

                if (whd.Count > 0)
                {
                    TimeSpan substraction = new TimeSpan(8, 0, 0);
                    TimeSpan anHour = new TimeSpan(1, 0, 0);
                    TimeSpan noon = new TimeSpan(12, 0, 0);
                    TimeSpan after_noon = new TimeSpan(13, 0, 0);

                    TimeSpan morning = new TimeSpan(8, 30, 0);
                    TimeSpan evening = new TimeSpan(17, 30, 0);
                    TimeSpan end_evening = new TimeSpan(18, 30, 0);
                    TimeSpan leave = TimeSpan.Zero;
                    
                   
                    whd.OrderBy(o => o.start_time);
                                     
                    for (int j = 0; j < whd.Count; j++)
                    {
                        TimeSpan regular = new TimeSpan();
                        TimeSpan ot15 = new TimeSpan();
                        TimeSpan ot3 = new TimeSpan();
                        WorkingHoursModel wh = new WorkingHoursModel();
                        wh.working_date = whd[j].working_date;
                        wh.job_id = whd[j].job_id;
                        wh.job_name = whd[j].job_name;
                        wh.task_id = whd[j].task_id;
                        wh.task_name = whd[j].task_name;
                        wh.start_time = whd[j].start_time;
                        wh.stop_time = whd[j].stop_time;
                        wh.lunch = whd[j].lunch;
                        wh.dinner = whd[j].dinner;
                        wh.day = day;

                        bool isHoliday = holidays.Where(w => w.date == whd[0].working_date).Count() > 0 ? true : false;
                        bool isWeekend = (whd[0].working_date.DayOfWeek == DayOfWeek.Saturday || whd[0].working_date.DayOfWeek == DayOfWeek.Sunday) ? true : false;

                        if (isHoliday || isWeekend)
                        {
                            if (wh.stop_time == new TimeSpan(23, 59, 0))
                            {
                                ot15 += (wh.stop_time - wh.start_time).Add(new TimeSpan(0, 1, 0));
                            }
                            else
                            {
                                ot15 += wh.stop_time - wh.start_time;
                            }

                            if (wh.lunch)
                            {
                                ot15 -= anHour;
                            }

                            if (wh.dinner)
                            {
                                ot15 -= anHour;
                            }
                        }
                        else
                        {
                            if (wh.start_time < morning && wh.stop_time <= morning)
                            {
                                //Start before 08.30 and stop before 08.30
                                if (wh.task_name == "Traveling")
                                {
                                    regular = new TimeSpan(0, 0, 0);
                                }
                                else
                                {
                                    regular = wh.stop_time - wh.start_time;
                                }
                            }
                            else if (wh.start_time < morning && wh.stop_time > morning && wh.stop_time <= evening)
                            {
                                //Start before 08.30 and stop after 08.30
                                if (wh.task_name == "Traveling")
                                {
                                    regular += wh.stop_time - morning;
                                }
                            }
                            //Check Start and Stop Time to calculate hours
                            else if (wh.start_time < evening && wh.stop_time > evening)
                            {
                                //Start before 17.30 and stop after 17.30
                                regular += evening - wh.start_time;

                                //Check if task is not equal to travel
                                if (wh.task_id[0] != 'T')
                                {
                                    //Add hours to overtime 1.5 if task is not travel
                                    if (wh.stop_time == new TimeSpan(23, 59, 0))
                                    {
                                        ot15 += (wh.stop_time - evening).Add(new TimeSpan(0, 1, 0));
                                    }
                                    else
                                    {
                                        ot15 += wh.stop_time - evening;
                                    }
                                }
                                else
                                {
                                    if (wh.task_name == "Traveling")
                                    {
                                        //Add hours to regular if task is travel
                                        regular = evening - wh.start_time;
                                    }
                                }

                            }
                            else if (wh.start_time < evening && wh.stop_time <= evening)
                            {
                                //Start before 17.30 and stop before 17.30
                                // Leave
                                if (wh.task_name == "Leave")
                                {
                                    var leave_hours = wh.stop_time - wh.start_time;
                                    if (leave_hours.TotalHours >= 8)
                                    {
                                        leave = leave.Add(new TimeSpan(8, 0, 0));
                                    }
                                    else
                                    {
                                        leave = leave.Add(wh.stop_time - wh.start_time);
                                    }
                                }
                                regular += wh.stop_time - wh.start_time;

                            }
                            else
                            {
                                //Start and stop after 17.30
                                //Check if task is not equal to travel
                                if (wh.task_id[0] != 'T')
                                {
                                    //Add hours to overtime 1.5 if task is not travel        

                                    if (wh.stop_time == new TimeSpan(23, 59, 0))
                                    {
                                        ot15 += (wh.stop_time - wh.start_time).Add(new TimeSpan(0, 1, 0));
                                    }
                                    else
                                    {
                                        ot15 += wh.stop_time - wh.start_time;
                                    }
                                }
                                else
                                {
                                    //Add hours to regular if task is travel
                                    if (wh.task_name != "Traveling")
                                    {
                                        if (wh.stop_time == new TimeSpan(23, 59, 0))
                                        {
                                            regular += (wh.stop_time - wh.start_time).Add(new TimeSpan(0, 1, 0));
                                        }
                                        else
                                        {
                                            regular += wh.stop_time - wh.start_time;
                                        }
                                    }
                                }
                            }

                            if (wh.lunch && wh.start_time <= noon && wh.stop_time > after_noon && regular.Hours > 1)
                            {
                                regular -= after_noon - noon;
                            }

                            if (wh.dinner && wh.start_time <= evening && wh.stop_time > end_evening && ot15.Hours > 1)
                            {
                                ot15 -= end_evening - evening;
                            }

                            // Regular time <= 8
                            if (regular.TotalHours > 8)
                            {
                                regular = new TimeSpan(8, 0, 0);
                            }
                        }

                        if ((ot15 > substraction) && (isHoliday || isWeekend))
                        {
                            ot3 += ot15 - substraction;
                            ot15 -= ot15 - substraction;
                        }

                        List<WorkingHoursModel> lastDays = monthly.Where(w => w.working_date.Date == date.Date).ToList();
                        TimeSpan lastOT15 = TimeSpan.Zero;
                        TimeSpan lastNormal = TimeSpan.Zero;
                        for (int k = 0; k < lastDays.Count; k++)
                        {
                            lastOT15 += lastDays[k].ot1_5;
                            lastNormal += lastDays[k].normal;
                        }

                        bool isTaskT = wh.task_id[0] == 'T';
                        if (!isTaskT)
                        {
                            wh.normal = TimeSpan.Zero;
                            TimeSpan remain_ot15 = TimeSpan.Zero;
                            remain_ot15 = substraction - lastOT15;

                            if (isWeekend || isHoliday)
                            {
                                if ((lastOT15 + ot15).TotalHours >= 8)
                                {
                                    if (ot3.TotalHours > 0)
                                    {
                                        wh.ot1_5 = ot15 - (ot15 - remain_ot15);
                                        wh.ot3_0 = ot3;
                                    }
                                    else
                                    {
                                        wh.ot1_5 = ot15 - (ot15 - remain_ot15);
                                        wh.ot3_0 = ot15 - remain_ot15;
                                    }
                                    
                                }
                                else
                                {
                                    wh.ot1_5 = ot15;
                                    wh.ot3_0 = ot3;
                                }
                            }
                            else
                            {
                                if (lastNormal.TotalHours >= 8)
                                {
                                    wh.ot1_5 = regular;
                                    wh.ot3_0 = new TimeSpan(0, 0, 0);
                                }
                                else
                                {
                                    wh.normal = regular;
                                    wh.ot1_5 = ot15;
                                    wh.ot3_0 = new TimeSpan(0, 0, 0);
                                }
                            }
                        }
                        else
                        {
                            if (wh.task_name == "Leave")
                            {
                                wh.normal = TimeSpan.Zero;
                                wh.ot1_5 = TimeSpan.Zero;
                                wh.ot3_0 = TimeSpan.Zero;
                                wh.leave = leave;
                            }
                            if (wh.task_name == "Traveling")
                            {
                                if (regular.TotalHours >= 8)
                                {
                                    wh.normal = new TimeSpan(8, 0, 0);
                                }
                                else
                                {
                                    wh.normal = regular;
                                }

                                wh.ot1_5 = TimeSpan.Zero;
                                wh.ot3_0 = TimeSpan.Zero;
                            }
                        }
                        monthly.Add(wh);
                    }
                }
                else
                {
                    WorkingHoursModel wh = new WorkingHoursModel()
                    {
                        working_date = date,
                        day = day,
                        job_id = "",
                        job_name = "",
                        task_id = "",
                        task_name = "",
                        start_time = default(TimeSpan),
                        stop_time = default(TimeSpan),
                        lunch = false,
                        dinner = false,
                        normal = default(TimeSpan),
                        ot1_5 = default(TimeSpan),
                        ot3_0 = default(TimeSpan),
                        leave = default(TimeSpan)
                    };
                    monthly.Add(wh);
                }
            }
            return monthly;
        }

        public List<WorkingHoursSummaryModel> CalculateMonthlySummary(List<WorkingHoursModel> workings)
        {
            List<WorkingHoursSummaryModel> whs = new List<WorkingHoursSummaryModel>();
            string[] jobs = workings.Where(w => w.job_id != "").Select(s => s.job_id).Distinct().ToArray();
            for (int i = 0; i < jobs.Count(); i++)
            {
                WorkingHoursSummaryModel js = new WorkingHoursSummaryModel();
                js.job_id = jobs[i];
                js.job_name = workings.Where(w => w.job_id == jobs[i]).Select(s => s.job_name).FirstOrDefault();
                js.normal = Convert.ToInt32(workings.Where(s => s.job_id == jobs[i]).Sum(t => t.normal.TotalMinutes));
                js.ot1_5 = Convert.ToInt32(workings.Where(s => s.job_id == jobs[i]).Sum(t => t.ot1_5.TotalMinutes));
                js.ot3_0 = Convert.ToInt32(workings.Where(s => s.job_id == jobs[i]).Sum(t => t.ot3_0.TotalMinutes));
                js.leave = Convert.ToInt32(workings.Where(s => s.job_id == jobs[i]).Sum(t => t.leave.TotalMinutes));
                whs.Add(js);
            }
            return whs;
        }
    }
}
