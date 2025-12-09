using WebENG.Interface;
using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Service;
using System.Data;

namespace WebENG.Service
{
    public class WorkingHoursService : IWorkingHours
    {
        IHoliday Holiday;
        ConnectSQL connect = null;
        SqlConnection con = null;
        public WorkingHoursService()
        {
            Holiday = new HolidayService();
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<WorkingHoursModel> GetWorkingHours()
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
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
                        WorkingHours.lunch_full,
                        WorkingHours.lunch_half,
                        WorkingHours.dinner_full,
                        WorkingHours.dinner_half,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID
                    Where WorkingHours.job_id NOT LIKE 'Q%' AND WorkingHours.job_id <> 'J999999'");
                SqlCommand cmd = new SqlCommand(string_command, con);
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
                            lunch_full = dr["lunch_full"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_full"].ToString()) : default(bool),
                            lunch_half = dr["lunch_half"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_half"].ToString()) : default(bool),
                            dinner_full = dr["dinner_full"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_full"].ToString()) : default(bool),
                            dinner_half = dr["dinner_half"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_half"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
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
            return whs;
        }

        public List<WorkingHoursModel> GetWorkingHours(string user_name)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
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
                        WorkingHours.lunch_full,
                        WorkingHours.lunch_half,
                        WorkingHours.dinner_full,
                        WorkingHours.dinner_half,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID
                    WHERE LOWER(Authen.name) = '{user_name}'");
                SqlCommand cmd = new SqlCommand(string_command, con);
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
                            lunch_full = dr["lunch_full"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_full"].ToString()) : default(bool),
                            lunch_half = dr["lunch_half"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_half"].ToString()) : default(bool),
                            dinner_full = dr["dinner_full"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_full"].ToString()) : default(bool),
                            dinner_half = dr["dinner_half"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_half"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
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
            return whs;
        }

        public List<WorkingDayModel> GetWorkingHours(string year, string month, string user_name)
        {
            List<WorkingDayModel> wd = new List<WorkingDayModel>();
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
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
                        WorkingHours.lunch_full,
                        WorkingHours.lunch_half,
                        WorkingHours.dinner_full,
                        WorkingHours.dinner_half,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID
                    WHERE WorkingHours.working_date like '{year}-{month}%' 
                    AND LOWER(Authen.name) ='{user_name}'");
                SqlCommand cmd = new SqlCommand(string_command, con);
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
                            lunch_full = dr["lunch_full"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_full"].ToString()) : default(bool),
                            lunch_half = dr["lunch_half"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_half"].ToString()) : default(bool),
                            dinner_full = dr["dinner_full"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_full"].ToString()) : default(bool),
                            dinner_half = dr["dinner_half"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_half"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
                    }
                    dr.Close();

                    wd = whs.GroupBy(g => g.working_date).Select(s => new WorkingDayModel()
                    {
                        date = s.Key,
                        workings = whs.Where(w=>w.working_date.Date == s.Key.Date).ToList()
                    }).ToList();
                }
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return wd;
        }

        public List<WorkingDayModel> GetAllWorkingHours()
        {
            List<WorkingDayModel> wd = new List<WorkingDayModel>();
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
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
                        WorkingHours.lunch_full,
                        WorkingHours.lunch_half,
                        WorkingHours.dinner_full,
                        WorkingHours.dinner_half,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID");
                SqlCommand cmd = new SqlCommand(string_command, con);
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
                            lunch_full = dr["lunch_full"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_full"].ToString()) : default(bool),
                            lunch_half = dr["lunch_half"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_half"].ToString()) : default(bool),
                            dinner_full = dr["dinner_full"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_full"].ToString()) : default(bool),
                            dinner_half = dr["dinner_half"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_half"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
                    }
                    dr.Close();

                    wd = whs.GroupBy(g => g.working_date).Select(s => new WorkingDayModel()
                    {
                        date = s.Key,
                        workings = whs.Where(w => w.working_date.Date == s.Key.Date).ToList()
                    }).ToList();
                }
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return wd;
        }
        public List<WorkingHoursModel> GetWorkingHours(string user_name, DateTime working_date)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
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
                        WorkingHours.lunch_full,
                        WorkingHours.lunch_half,
                        WorkingHours.dinner_full,
                        WorkingHours.dinner_half,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID
                    WHERE LOWER(Authen.name) = '{user_name}'
                    AND WorkingHours.working_date LIKE '{working_date.ToString("yyyy-MM-dd")}'");
                SqlCommand cmd = new SqlCommand(string_command, con);
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
                            lunch_full = dr["lunch_full"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_full"].ToString()) : default(bool),
                            lunch_half = dr["lunch_half"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_half"].ToString()) : default(bool),
                            dinner_full = dr["dinner_full"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_full"].ToString()) : default(bool),
                            dinner_half = dr["dinner_half"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_half"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
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
            return whs;
        }

        public string AddWorkingHours(WorkingHoursModel wh)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    INSERT INTO WorkingHours(
                        ind,user_id, working_date, week_number, job_id,process_id,system_id, task_id, start_time, stop_time, lunch_full,lunch_half, dinner_full,dinner_half, note)
                    VALUES (
                        @ind,@user_id, @working_date, (SELECT DATEPART(ISO_WEEK,@working_date)), @job_id,@process_id,@system_id, @task_id, @start_time, @stop_time, @lunch_full,@lunch_half, @dinner_full,@dinner_half, @note)");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
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
                        if (wh.process_id == null)
                        {
                            cmd.Parameters.AddWithValue("@process_id", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@process_id", wh.process_id);
                        }

                        if (wh.system_id == null)
                        {
                            cmd.Parameters.AddWithValue("@system_id", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@system_id", wh.system_id);
                        }
                        
                    }
                    
                    cmd.Parameters.AddWithValue("@task_id", wh.task_id);
                    cmd.Parameters.AddWithValue("@start_time", wh.start_time);
                    cmd.Parameters.AddWithValue("@stop_time", wh.stop_time);
                    cmd.Parameters.AddWithValue("@lunch_full", wh.lunch_full);
                    cmd.Parameters.AddWithValue("@lunch_half", wh.lunch_half);
                    cmd.Parameters.AddWithValue("@dinner_full", wh.dinner_full);
                    cmd.Parameters.AddWithValue("@dinner_half", wh.dinner_half);
                    cmd.Parameters.AddWithValue("@note", wh.note);
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

        public string UpdateWorkingHours(WorkingHoursModel wh)
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
                        user_id = @user_id,
                        working_date = @working_date,
                        week_number = (SELECT DATEPART(ISO_WEEK,@working_date)),
                        job_id = @job_id,
                        process_id = @process_id,
                        system_id = @system_id,
                        task_id = @task_id,
                        start_time = @start_time,
                        stop_time = @stop_time,
                        lunch_full = @lunch_full,
                        lunch_half = @lunch_half,
                        dinner_full = @dinner_full,
                        dinner_half = @dinner_half,
                        note = @note
                    WHERE ind = @ind");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
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
                        if (wh.process_id == null)
                        {
                            cmd.Parameters.AddWithValue("@process_id", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@process_id", wh.process_id);
                        }

                        if (wh.system_id == null)
                        {
                            cmd.Parameters.AddWithValue("@system_id", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@system_id", wh.system_id);
                        }
                    }
                    cmd.Parameters.AddWithValue("@task_id", wh.task_id);
                    cmd.Parameters.AddWithValue("@start_time", wh.start_time);
                    cmd.Parameters.AddWithValue("@stop_time", wh.stop_time);
                    cmd.Parameters.AddWithValue("@lunch_full", wh.lunch_full);
                    cmd.Parameters.AddWithValue("@lunch_half", wh.lunch_half);
                    cmd.Parameters.AddWithValue("@dinner_full", wh.dinner_full);
                    cmd.Parameters.AddWithValue("@dinner_half", wh.dinner_half);
                    cmd.Parameters.AddWithValue("@note", wh.note);
                    cmd.Parameters.AddWithValue("@ind", wh.index);
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

        public string UpdateRestTime(WorkingHoursModel wh)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    UPDATE WorkingHours 
                    SET lunch_full = @lunch_full,
                        lunch_half = @lunch_half,
                        dinner_full = @dinner_full,
                        dinner_half = @dinner_half
                    WHERE user_id = @user_id 
                        AND working_date = @working_date 
                        AND job_id = @job_id
                        AND task_id = @task_id
                        AND start_time = @start_time
                        AND stop_time = @stop_time");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@lunch_full", wh.lunch_full);
                    cmd.Parameters.AddWithValue("@lunch_half", wh.lunch_half);
                    cmd.Parameters.AddWithValue("@dinner_full", wh.dinner_full);
                    cmd.Parameters.AddWithValue("@dinner_half", wh.dinner_half);
                    cmd.Parameters.AddWithValue("@user_id", wh.user_id);
                    cmd.Parameters.AddWithValue("@working_date", wh.working_date);
                    cmd.Parameters.AddWithValue("@job_id", wh.job_id);
                    cmd.Parameters.AddWithValue("@task_id", wh.task_id);
                    cmd.Parameters.AddWithValue("@start_time", wh.start_time);
                    cmd.Parameters.AddWithValue("@stop_time", wh.stop_time);
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
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return "Success";
        }

        public string DeleteWorkingHours(WorkingHoursModel wh)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    DELETE FROM WorkingHours
                    WHERE user_id = @user_id
                        AND ind = @ind");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@user_id", wh.user_id);
                    cmd.Parameters.AddWithValue("@ind", wh.index);
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

        public List<JobWeeklyWorkingHoursModel> GetAllJobWorkingHours(int year, int week)
        {
            List<JobWeeklyWorkingHoursModel> whs = new List<JobWeeklyWorkingHoursModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
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
                SqlCommand cmd = new SqlCommand(string_command, con);
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
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return whs;
        }

        public List<EngWeeklyWorkingHoursModel> GetAllEngWorkingHours(int year, int week)
        {
            List<EngWeeklyWorkingHoursModel> whs = new List<EngWeeklyWorkingHoursModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
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
                SqlCommand cmd = new SqlCommand(string_command, con);
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
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return whs;
        }

        public List<WorkingHoursModel> GetWorkingHours(int year, int week)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
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
                        WorkingHours.lunch_full,
                        WorkingHours.lunch_half,
                        WorkingHours.dinner_full,
                        WorkingHours.dinner_half,
                        WorkingHours.note
                    FROM WorkingHours
                        LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                        LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                        LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
						LEFT JOIN Eng_Process ON WorkingHours.process_id = Eng_Process.Process_ID
						LEFT JOIN Eng_System ON WorkingHours.system_id = Eng_System.System_ID
                    WHERE WorkingHours.working_date LIKE '{year}%' AND WorkingHours.week_number = {week}");
                SqlCommand cmd = new SqlCommand(string_command, con);
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
                            lunch_full = dr["lunch_full"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_full"].ToString()) : default(bool),
                            lunch_half = dr["lunch_half"] != DBNull.Value ? Convert.ToBoolean(dr["lunch_half"].ToString()) : default(bool),
                            dinner_full = dr["dinner_full"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_full"].ToString()) : default(bool),
                            dinner_half = dr["dinner_half"] != DBNull.Value ? Convert.ToBoolean(dr["dinner_half"].ToString()) : default(bool),
                            note = dr["note"] != DBNull.Value ? dr["note"].ToString() : "",
                        };
                        whs.Add(wh);
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
            return whs;
        }

        public int GetLastWorkingHoursID()
        {
            int id = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT TOP 1 ind FROM WorkingHours WHERE ind LIKE 'WH%' ORDER BY ind DESC");
                SqlCommand cmd = new SqlCommand(string_command, con);
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
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return id;
        }

        //public List<WorkingHoursModel> CalculateWorkingHours_OLD(string user_name, string month)
        //{
        //    List<WorkingHoursModel> monthly = new List<WorkingHoursModel>();

        //    string day = "";
        //    int yy = Convert.ToInt32(month.Split("-")[0]);
        //    int mm = Convert.ToInt32(month.Split("-")[1]);

        //    List<WorkingHoursModel> whs = GetWorkingHours(yy.ToString(), mm.ToString().PadLeft(2, '0'), user_name);
        //    List<HolidayModel> holidays = Holiday.GetHolidays(yy.ToString());

        //    whs = whs.OrderBy(o => o.working_date).ToList();
        //    int days = DateTime.DaysInMonth(yy, mm);
        //    for (int i = 0; i < days; i++)
        //    {
        //        DateTime date = new DateTime(yy, mm, i + 1);
        //        List<WorkingHoursModel> whd = whs.Where(w => w.working_date == date).ToList();

        //        bool _isHoliday = holidays.Where(w => w.date == date).Count() > 0 ? true : false;

        //        // Check Holiday and Get day
        //        if (_isHoliday)
        //        {
        //            day = "Holiday";
        //        }
        //        else
        //        {
        //            day = date.DayOfWeek.ToString();
        //        }

        //        if (whd.Count > 0)
        //        {
        //            TimeSpan substraction = new TimeSpan(8, 0, 0);
        //            TimeSpan anHour = new TimeSpan(1, 0, 0);
        //            TimeSpan anHalf = new TimeSpan(0, 30, 0);
        //            TimeSpan noon = new TimeSpan(12, 0, 0);
        //            TimeSpan after_noon_full = new TimeSpan(13, 0, 0);
        //            TimeSpan after_noon_half = new TimeSpan(12, 30, 0);

        //            TimeSpan morning = new TimeSpan(8, 30, 0);
        //            TimeSpan evening = new TimeSpan(17, 30, 0);
        //            TimeSpan end_evening_full = new TimeSpan(18, 30, 0);
        //            TimeSpan end_evening_half = new TimeSpan(18, 0, 0);
        //            TimeSpan leave = TimeSpan.Zero;


        //            whd.OrderBy(o => o.start_time);

        //            for (int j = 0; j < whd.Count; j++)
        //            {
        //                TimeSpan regular = new TimeSpan();
        //                TimeSpan ot15 = new TimeSpan();
        //                TimeSpan ot3 = new TimeSpan();
        //                WorkingHoursModel wh = new WorkingHoursModel();
        //                wh.working_date = whd[j].working_date;
        //                wh.job_id = whd[j].job_id;
        //                wh.job_name = whd[j].job_name;
        //                wh.task_id = whd[j].task_id;
        //                wh.task_name = whd[j].task_name;
        //                wh.start_time = whd[j].start_time;
        //                wh.stop_time = whd[j].stop_time;
        //                wh.lunch_full = whd[j].lunch_full;
        //                wh.lunch_half = whd[j].lunch_half;
        //                wh.dinner_full = whd[j].dinner_full;
        //                wh.dinner_half = whd[j].dinner_half;
        //                wh.day = day;

        //                bool isHoliday = holidays.Where(w => w.date == whd[0].working_date).Count() > 0 ? true : false;
        //                bool isWeekend = (whd[0].working_date.DayOfWeek == DayOfWeek.Saturday || whd[0].working_date.DayOfWeek == DayOfWeek.Sunday) ? true : false;

        //                if (isHoliday || isWeekend)
        //                {
        //                    if (wh.stop_time == new TimeSpan(23, 59, 0))
        //                    {
        //                        ot15 += (wh.stop_time - wh.start_time).Add(new TimeSpan(0, 1, 0));
        //                    }
        //                    else
        //                    {
        //                        ot15 += wh.stop_time - wh.start_time;
        //                    }

        //                    if (wh.lunch_full)
        //                    {
        //                        ot15 -= anHour;
        //                    }
        //                    if (wh.lunch_half)
        //                    {
        //                        ot15 -= anHalf;
        //                    }

        //                    if (wh.dinner_full)
        //                    {
        //                        ot15 -= anHour;
        //                    }
        //                    if (wh.dinner_half)
        //                    {
        //                        ot15 -= anHalf;
        //                    }
        //                }
        //                else
        //                {
        //                    if (wh.start_time < morning && wh.stop_time <= morning)
        //                    {
        //                        //Start before 08.30 and stop before 08.30
        //                        if (wh.task_name == "Traveling")
        //                        {
        //                            regular += new TimeSpan(0, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            regular += wh.stop_time - wh.start_time;
        //                        }
        //                    }
        //                    else if (wh.start_time < morning && wh.stop_time > morning && wh.stop_time <= evening)
        //                    {
        //                        //Start before 08.30 and stop after 08.30
        //                        if (wh.task_name == "Traveling")
        //                        {
        //                            regular += wh.stop_time - morning;
        //                        }
        //                    }
        //                    //Check Start and Stop Time to calculate hours
        //                    else if (wh.start_time < evening && wh.stop_time > evening)
        //                    {
        //                        //Start before 17.30 and stop after 17.30
        //                        regular += evening - wh.start_time;

        //                        //Check if task is not equal to travel
        //                        if (wh.task_id[0] != 'T')
        //                        {
        //                            //Add hours to overtime 1.5 if task is not travel
        //                            if (wh.stop_time == new TimeSpan(23, 59, 0))
        //                            {
        //                                ot15 += (wh.stop_time - evening).Add(new TimeSpan(0, 1, 0));
        //                            }
        //                            else
        //                            {
        //                                ot15 += wh.stop_time - evening;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (wh.task_name == "Traveling")
        //                            {
        //                                //Add hours to regular if task is travel
        //                                regular = evening - wh.start_time;
        //                            }
        //                        }

        //                    }
        //                    else if (wh.start_time < evening && wh.stop_time <= evening)
        //                    {
        //                        //Start before 17.30 and stop before 17.30
        //                        // Leave
        //                        if (wh.task_name == "Leave")
        //                        {
        //                            var leave_hours = wh.stop_time - wh.start_time;
        //                            if (leave_hours.TotalHours >= 8)
        //                            {
        //                                leave = leave.Add(new TimeSpan(8, 0, 0));
        //                            }
        //                            else
        //                            {
        //                                leave = leave.Add(wh.stop_time - wh.start_time);
        //                            }
        //                        }
        //                        regular += wh.stop_time - wh.start_time;

        //                    }
        //                    else
        //                    {
        //                        //Start and stop after 17.30
        //                        //Check if task is not equal to travel
        //                        if (wh.task_id[0] != 'T')
        //                        {
        //                            //Add hours to overtime 1.5 if task is not travel        

        //                            if (wh.stop_time == new TimeSpan(23, 59, 0))
        //                            {
        //                                ot15 += (wh.stop_time - wh.start_time).Add(new TimeSpan(0, 1, 0));
        //                            }
        //                            else
        //                            {
        //                                ot15 += wh.stop_time - wh.start_time;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //Add hours to regular if task is travel
        //                            if (wh.task_name != "Traveling")
        //                            {
        //                                if (wh.stop_time == new TimeSpan(23, 59, 0))
        //                                {
        //                                    regular += (wh.stop_time - wh.start_time).Add(new TimeSpan(0, 1, 0));
        //                                }
        //                                else
        //                                {
        //                                    regular += wh.stop_time - wh.start_time;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    if (wh.lunch_full && wh.start_time <= noon && wh.stop_time > after_noon_full && regular.Hours > 1)
        //                    {
        //                        regular -= after_noon_full - noon;
        //                    }

        //                    if (wh.lunch_half && wh.start_time <= noon && wh.stop_time > after_noon_half && regular.Hours > 1)
        //                    {
        //                        regular -= after_noon_half - noon;
        //                    }

        //                    if (wh.dinner_full && wh.start_time <= evening && wh.stop_time > end_evening_full && ot15.Hours > 1)
        //                    {
        //                        ot15 -= end_evening_full - evening;
        //                    }

        //                    if (wh.dinner_half && wh.start_time <= evening && wh.stop_time > end_evening_half && ot15.Hours > 1)
        //                    {
        //                        ot15 -= end_evening_half - evening;
        //                    }

        //                    // Regular time <= 8
        //                    if (regular.TotalHours > 8)
        //                    {
        //                        regular = new TimeSpan(8, 0, 0);
        //                    }
        //                }

        //                if ((ot15 > substraction) && (isHoliday || isWeekend))
        //                {
        //                    ot3 += ot15 - substraction;
        //                    ot15 -= ot15 - substraction;
        //                }

        //                List<WorkingHoursModel> lastDays = monthly.Where(w => w.working_date.Date == date.Date).ToList();
        //                TimeSpan lastOT15 = TimeSpan.Zero;
        //                TimeSpan lastNormal = TimeSpan.Zero;
        //                for (int k = 0; k < lastDays.Count; k++)
        //                {
        //                    lastOT15 += lastDays[k].ot1_5;
        //                    lastNormal += lastDays[k].normal;
        //                }

        //                bool isTaskT = wh.task_id[0] == 'T';
        //                if (!isTaskT)
        //                {
        //                    wh.normal = TimeSpan.Zero;
        //                    TimeSpan remain_ot15 = TimeSpan.Zero;
        //                    remain_ot15 = substraction - lastOT15;

        //                    if (isWeekend || isHoliday)
        //                    {
        //                        if ((lastOT15 + ot15).TotalHours >= 8)
        //                        {
        //                            if (ot3.TotalHours > 0)
        //                            {
        //                                wh.ot1_5 = ot15 - (ot15 - remain_ot15);
        //                                wh.ot3_0 = ot3;
        //                            }
        //                            else
        //                            {
        //                                wh.ot1_5 = ot15 - (ot15 - remain_ot15);
        //                                wh.ot3_0 = ot15 - remain_ot15;
        //                            }

        //                        }
        //                        else
        //                        {
        //                            wh.ot1_5 = ot15;
        //                            wh.ot3_0 = ot3;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (lastNormal.TotalHours >= 8)
        //                        {
        //                            if (regular.TotalHours > 0)
        //                            {
        //                                wh.ot1_5 = regular;
        //                                wh.ot3_0 = new TimeSpan(0, 0, 0);
        //                            }
        //                            else
        //                            {
        //                                wh.ot1_5 = ot15;
        //                                wh.ot3_0 = new TimeSpan(0, 0, 0);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            wh.normal = regular;
        //                            wh.ot1_5 = ot15;
        //                            wh.ot3_0 = new TimeSpan(0, 0, 0);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (wh.task_name == "Leave")
        //                    {
        //                        wh.normal = TimeSpan.Zero;
        //                        wh.ot1_5 = TimeSpan.Zero;
        //                        wh.ot3_0 = TimeSpan.Zero;
        //                        wh.leave = leave;
        //                    }
        //                    if (wh.task_name == "Traveling")
        //                    {
        //                        if (regular.TotalHours >= 8)
        //                        {
        //                            wh.normal = new TimeSpan(8, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            wh.normal = regular;
        //                        }

        //                        wh.ot1_5 = TimeSpan.Zero;
        //                        wh.ot3_0 = TimeSpan.Zero;
        //                    }
        //                }
        //                monthly.Add(wh);
        //            }
        //        }
        //        else
        //        {
        //            WorkingHoursModel wh = new WorkingHoursModel()
        //            {
        //                working_date = date,
        //                day = day,
        //                job_id = "",
        //                job_name = "",
        //                task_id = "",
        //                task_name = "",
        //                start_time = default(TimeSpan),
        //                stop_time = default(TimeSpan),
        //                lunch_full = false,
        //                lunch_half = false,
        //                dinner_full = false,
        //                dinner_half = false,
        //                normal = default(TimeSpan),
        //                ot1_5 = default(TimeSpan),
        //                ot3_0 = default(TimeSpan),
        //                leave = default(TimeSpan)
        //            };
        //            monthly.Add(wh);
        //        }
        //    }
        //    return monthly;
        //}
        public List<WorkingHoursModel> CalculateWorkingHours_O(string user_name, string month)
        {
            List<WorkingHoursModel> monthly = new List<WorkingHoursModel>();

            string day = "";
            int yy = Convert.ToInt32(month.Split("-")[0]);
            int mm = Convert.ToInt32(month.Split("-")[1]);

            List<WorkingDayModel> whs = GetWorkingHours(yy.ToString(), mm.ToString().PadLeft(2, '0'), user_name);
            List<HolidayModel> holidays = Holiday.GetHolidays(yy.ToString());
            WorkingHoursModel wh = new WorkingHoursModel();
            TimeSpan working_date = new TimeSpan(0, 0, 0);
            for (DateTime date = new DateTime(yy, mm, 1); date <= new DateTime(yy, mm, DateTime.DaysInMonth(yy, mm)); date = date.AddDays(1))
            {
                day = date.DayOfWeek.ToString();
                List<WorkingDayModel> _wd = whs.Where(w => w.date.Date == date).ToList();
                //_wd = _wd.Where(w => w.date == new DateTime(2025, 10, 15)).ToList();
                bool isHoliday = holidays.Where(w => w.date.Date == date.Date).Count() > 0 ? true : false;
                bool isWeekend = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? true : false;
                if (isHoliday)
                {
                    day = "Holiday";
                }
                if (_wd.Count > 0)
                {
                    for (int i = 0; i < _wd.Count; i++)
                    {
                        _wd[i].workings = _wd[i].workings.OrderBy(o => o.start_time).ToList();

                        bool chk_after_office = false;
                        working_date = new TimeSpan(0, 0, 0);

                        TimeSpan regular = new TimeSpan(0, 0, 0);
                        TimeSpan ot15 = new TimeSpan(0, 0, 0);
                        TimeSpan ot3 = new TimeSpan(0, 0, 0);
                        TimeSpan leave = new TimeSpan(0, 0, 0);

                        // Last index Customer of Work Day
                        int last_customer_index = _wd[i].workings.Count-1;
                        int first_customer_index = 0;
                        bool all_traveling = _wd[i].workings.All(a => a.task_name == "Traveling");
                        if (all_traveling)
                        {
                            last_customer_index = -1;
                            first_customer_index = -1;
                        }
                        else
                        {
                            for (int k = _wd[i].workings.Count - 1; k >= 0; k--)
                            {
                                if (_wd[i].workings[k].task_name != "Traveling")
                                {
                                    break;
                                }
                                else
                                {
                                    last_customer_index--;
                                }
                            }

                            for (int k = 0; k < _wd[i].workings.Count; k++)
                            {
                                if (_wd[i].workings[k].task_name != "Traveling")
                                {
                                    break;
                                }
                                else
                                {
                                    first_customer_index++;
                                }
                            }
                        }

                        for (int j = 0; j < _wd[i].workings.Count; j++)
                        {
                            chk_after_office = false;
                            regular = new TimeSpan(0, 0, 0);
                            ot15 = new TimeSpan(0, 0, 0);
                            ot3 = new TimeSpan(0, 0, 0);
                            leave = new TimeSpan(0, 0, 0);
                            //_wd[i].workings = _wd[i].workings.OrderByDescending(o => o.start_time).ToList();
                            // Check Holiday and Get day
                            if (isHoliday || isWeekend)
                            {
                                if (_wd[i].workings[j].task_name == "Traveling")
                                {
                                    ot15 = default(TimeSpan);
                                    ot3 = default(TimeSpan);

                                    if (_wd[i].workings[j].lunch_full)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].lunch_half)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(0, 30, 0);
                                        }
                                    }

                                    if (_wd[i].workings[j].dinner_full)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_half)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(0, 30, 0);
                                        }
                                    }

                                    wh = new WorkingHoursModel()
                                    {
                                        working_date = _wd[i].date,
                                        job_id = _wd[i].workings[j].job_id,
                                        job_name = _wd[i].workings[j].job_name,
                                        task_id = _wd[i].workings[j].task_id,
                                        task_name = _wd[i].workings[j].task_name,
                                        start_time = _wd[i].workings[j].start_time,
                                        stop_time = _wd[i].workings[j].stop_time,
                                        lunch_full = _wd[i].workings[j].lunch_full,
                                        lunch_half = _wd[i].workings[j].lunch_half,
                                        dinner_full = _wd[i].workings[j].dinner_full,
                                        dinner_half = _wd[i].workings[j].dinner_half,
                                        day = day,
                                        normal = regular,
                                        ot1_5 = ot15,
                                        ot3_0 = ot3,
                                        leave = leave
                                    };
                                    monthly.Add(wh);
                                    regular = new TimeSpan(0, 0, 0);
                                    continue;
                                }
                                else
                                {
                                    if (_wd[i].workings[j].stop_time == new TimeSpan(23, 59, 0))
                                    {
                                        ot15 += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time).Add(new TimeSpan(0, 1, 0));
                                    }
                                    else
                                    {
                                        ot15 += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                    }

                                }
                                if (_wd[i].workings[j].lunch_full)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].lunch_half)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(0, 30, 0);
                                    }
                                }

                                if (_wd[i].workings[j].dinner_full)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].dinner_half)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(0, 30, 0);
                                    }
                                }


                                if (ot15 >= new TimeSpan(8, 0, 0))
                                {
                                    ot3 = ot15 - new TimeSpan(8, 0, 0);
                                }

                                if (ot3 > new TimeSpan(0, 0, 0))
                                {
                                    if (j == 0)
                                    {
                                        ot15 = new TimeSpan(8, 0, 0);
                                    }
                                    else
                                    {
                                        ot15 = default(TimeSpan);
                                    }
                                }

                                //Check Sum OT 1.5
                                TimeSpan sum_ot15 = monthly.Where(w => w.working_date.Date == date.Date && w.task_name != "Traveling").ToList().Aggregate(
                                    TimeSpan.Zero, (sum_ot, next_ot) => sum_ot + next_ot.ot1_5) + ot15;
                                if (sum_ot15 > new TimeSpan(8, 0, 0))
                                {
                                    ot15 = new TimeSpan(8, 0, 0) - (sum_ot15 - ot15);
                                    ot3 = sum_ot15 - new TimeSpan(8, 0, 0);
                                }

                                wh = new WorkingHoursModel()
                                {
                                    working_date = _wd[i].date,
                                    job_id = _wd[i].workings[j].job_id,
                                    job_name = _wd[i].workings[j].job_name,
                                    task_id = _wd[i].workings[j].task_id,
                                    task_name = _wd[i].workings[j].task_name,
                                    start_time = _wd[i].workings[j].start_time,
                                    stop_time = _wd[i].workings[j].stop_time,
                                    lunch_full = _wd[i].workings[j].lunch_full,
                                    lunch_half = _wd[i].workings[j].lunch_half,
                                    dinner_full = _wd[i].workings[j].dinner_full,
                                    dinner_half = _wd[i].workings[j].dinner_half,
                                    day = day,
                                    normal = default(TimeSpan),
                                    ot1_5 = ot15,
                                    ot3_0 = ot3,
                                    leave = default(TimeSpan)
                                };
                                monthly.Add(wh);
                            }
                            else  // Regular day
                            {
                                day = _wd[i].workings[j].working_date.DayOfWeek.ToString();

                                // Last Traveling exclude working hours
                                                                                           
                                if (_wd[i].workings[j].task_name == "Traveling" && j > last_customer_index)
                                //if (_wd[i].workings[j].task_name == "Traveling")
                                {
                                    if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time >= new TimeSpan (8,30,0))
                                    {
                                        _wd[i].workings[j].start_time = new TimeSpan(8, 30, 0);
                                    }
                                    else if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time < new TimeSpan(8, 30, 0))
                                    {
                                        _wd[i].workings[j].start_time = new TimeSpan(8, 30, 0);
                                        _wd[i].workings[j].stop_time = new TimeSpan(8, 30, 0);
                                    }

                                    regular = (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                    ot15 = default(TimeSpan);
                                    ot3 = default(TimeSpan);

                                    if (_wd[i].workings[j].lunch_full)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].lunch_half)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(0, 30, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_full)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_half)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(0, 30, 0);
                                        }
                                    }

                                    wh = new WorkingHoursModel()
                                    {
                                        working_date = _wd[i].date,
                                        job_id = _wd[i].workings[j].job_id,
                                        job_name = _wd[i].workings[j].job_name,
                                        task_id = _wd[i].workings[j].task_id,
                                        task_name = _wd[i].workings[j].task_name,
                                        start_time = _wd[i].workings[j].start_time,
                                        stop_time = _wd[i].workings[j].stop_time,
                                        lunch_full = _wd[i].workings[j].lunch_full,
                                        lunch_half = _wd[i].workings[j].lunch_half,
                                        dinner_full = _wd[i].workings[j].dinner_full,
                                        dinner_half = _wd[i].workings[j].dinner_half,
                                        day = day,
                                        normal = regular,
                                        ot1_5 = ot15,
                                        ot3_0 = ot3,
                                        leave = leave
                                    };
                                    monthly.Add(wh);
                                    regular = new TimeSpan(0, 0, 0);
                                    continue;
                                }                                
                                else if (_wd[i].workings[j].task_name == "Leave")
                                {
                                    regular = default(TimeSpan);
                                    ot15 = default(TimeSpan);
                                    ot3 = default(TimeSpan);
                                    leave = (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                    if (leave > new TimeSpan(8, 0, 0))
                                    {
                                        leave = new TimeSpan(8, 0, 0);
                                    }

                                    wh = new WorkingHoursModel()
                                    {
                                        working_date = _wd[i].date,
                                        job_id = _wd[i].workings[j].job_id,
                                        job_name = _wd[i].workings[j].job_name,
                                        task_id = _wd[i].workings[j].task_id,
                                        task_name = _wd[i].workings[j].task_name,
                                        start_time = _wd[i].workings[j].start_time,
                                        stop_time = _wd[i].workings[j].stop_time,
                                        lunch_full = _wd[i].workings[j].lunch_full,
                                        lunch_half = _wd[i].workings[j].lunch_half,
                                        dinner_full = _wd[i].workings[j].dinner_full,
                                        dinner_half = _wd[i].workings[j].dinner_half,
                                        day = day,
                                        normal = regular,
                                        ot1_5 = ot15,
                                        ot3_0 = ot3,
                                        leave = leave
                                    };
                                    monthly.Add(wh);
                                    regular = new TimeSpan(0, 0, 0);
                                    continue;
                                }
                                else
                                {
                                    if (_wd[i].workings[j].task_id.Contains("O") || _wd[i].workings[j].task_id.Contains("H")) //Office,Home
                                    {
                                        if (_wd[i].workings[j].start_time <= new TimeSpan(17, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(17, 30, 0))
                                        {
                                            ot15 += _wd[i].workings[j].stop_time - new TimeSpan(17, 30, 0);
                                            regular = new TimeSpan(17, 30, 0) - _wd[i].workings[j].start_time;
                                            chk_after_office = true;

                                            if (_wd[i].workings[j].dinner_full)
                                            {
                                                if (ot15 != default(TimeSpan))
                                                {
                                                    ot15 -= new TimeSpan(1, 0, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].dinner_half)
                                            {
                                                if (ot15 != default(TimeSpan))
                                                {
                                                    ot15 -= new TimeSpan(0, 30, 0);
                                                }
                                            }
                                        }
                                        if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time < new TimeSpan(8, 30, 0))
                                        {
                                            ot15 += _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                            regular = new TimeSpan(0, 0, 0);
                                            chk_after_office = true;
                                        }

                                        if (_wd[i].workings[j].start_time > new TimeSpan(17, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(17, 30, 0))
                                        {
                                            ot15 += _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                            regular = new TimeSpan(0, 0, 0);
                                            chk_after_office = true;
                                        }
                                    }
                                    else if (_wd[i].workings[j].task_name == "Traveling")
                                    {
                                        if (first_customer_index != -1 && first_customer_index > j)
                                        {
                                            if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time >= new TimeSpan(8, 30, 0))
                                            {
                                                _wd[i].workings[j].start_time = new TimeSpan(8, 30, 0);
                                            }
                                            else if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time < new TimeSpan(8, 30, 0))
                                            {
                                                _wd[i].workings[j].start_time = new TimeSpan(8, 30, 0);
                                                _wd[i].workings[j].stop_time = new TimeSpan(8, 30, 0);
                                            }
                                        }
                                    }

                                    if (!chk_after_office)
                                    {
                                        if (_wd[i].workings[j].stop_time == new TimeSpan(23, 59, 0))
                                        {
                                            regular += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time).Add(new TimeSpan(0, 1, 0));
                                        }
                                        else
                                        {
                                            regular += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                        }
                                    }
                                    
                                }

                                if (!chk_after_office)
                                {
                                    if (_wd[i].workings[j].lunch_full)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].lunch_half)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(0, 30, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_full)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_half)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(0, 30, 0);
                                        }
                                    }
                                }

                                //Check Sum Regular
                                //TimeSpan sum_regular = monthly.Where(w => w.working_date.Date == date.Date && w.task_name != "Traveling" && w.task_name != "Leave").ToList().Aggregate(
                                  // TimeSpan.Zero, (sum_reg, next_reg) => sum_reg + next_reg.normal) + regular;
                                TimeSpan sum_regular = monthly.Where(w => w.working_date.Date == date.Date && w.task_name != "Leave").ToList().Aggregate(
                                    TimeSpan.Zero, (sum_reg, next_reg) => sum_reg + next_reg.normal) + regular;
                                if (sum_regular > new TimeSpan(8, 0, 0))
                                {
                                    regular = new TimeSpan(8, 0, 0) - (sum_regular - regular);
                                    if (!chk_after_office)
                                    {
                                        ot15 = sum_regular - new TimeSpan(8, 0, 0);
                                    }
                                }

                                wh = new WorkingHoursModel()
                                {
                                    working_date = _wd[i].date,
                                    job_id = _wd[i].workings[j].job_id,
                                    job_name = _wd[i].workings[j].job_name,
                                    task_id = _wd[i].workings[j].task_id,
                                    task_name = _wd[i].workings[j].task_name,
                                    start_time = _wd[i].workings[j].start_time,
                                    stop_time = _wd[i].workings[j].stop_time,
                                    lunch_full = _wd[i].workings[j].lunch_full,
                                    lunch_half = _wd[i].workings[j].lunch_half,
                                    dinner_full = _wd[i].workings[j].dinner_full,
                                    dinner_half = _wd[i].workings[j].dinner_half,
                                    day = day,
                                    normal = regular,
                                    ot1_5 = ot15,
                                    ot3_0 = ot3,
                                    leave = leave
                                };
                                monthly.Add(wh);
                            }
                        }
                    }
                }
                else
                {
                    wh = new WorkingHoursModel()
                    {
                        working_date = date,
                        day = day,
                        job_id = "",
                        job_name = "",
                        task_id = "",
                        task_name = "",
                        start_time = default(TimeSpan),
                        stop_time = default(TimeSpan),
                        lunch_full = false,
                        lunch_half = false,
                        dinner_full = false,
                        dinner_half = false,
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

        public List<WorkingHoursModel> CalculateWorkingHours_OLD2(string user_name, string month)
        {
            List<WorkingHoursModel> monthly = new List<WorkingHoursModel>();

            string day = "";
            int yy = Convert.ToInt32(month.Split("-")[0]);
            int mm = Convert.ToInt32(month.Split("-")[1]);

            List<WorkingDayModel> whs = GetWorkingHours(yy.ToString(), mm.ToString().PadLeft(2, '0'), user_name);
            List<HolidayModel> holidays = Holiday.GetHolidays(yy.ToString());
            WorkingHoursModel wh = new WorkingHoursModel();
            TimeSpan working_date = new TimeSpan(0, 0, 0);
            for (DateTime date = new DateTime(yy, mm, 1); date <= new DateTime(yy, mm, DateTime.DaysInMonth(yy, mm)); date = date.AddDays(1))
            {
                day = date.DayOfWeek.ToString();
                List<WorkingDayModel> _wd = whs.Where(w => w.date.Date == date).ToList();
                //_wd = _wd.Where(w => w.date == new DateTime(2025, 10, 15)).ToList();
                bool isHoliday = holidays.Where(w => w.date.Date == date.Date).Count() > 0 ? true : false;
                bool isWeekend = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? true : false;
                if (isHoliday)
                {
                    day = "Holiday";
                }
                if (_wd.Count > 0)
                {
                    for (int i = 0; i < _wd.Count; i++)
                    {
                        _wd[i].workings = _wd[i].workings.OrderBy(o => o.start_time).ToList();

                        working_date = new TimeSpan(0, 0, 0);

                        TimeSpan regular = new TimeSpan(0, 0, 0);
                        TimeSpan ot15 = new TimeSpan(0, 0, 0);
                        TimeSpan ot3 = new TimeSpan(0, 0, 0);
                        TimeSpan leave = new TimeSpan(0, 0, 0);

                        for (int j = 0; j < _wd[i].workings.Count; j++)
                        {
                            regular = new TimeSpan(0, 0, 0);
                            ot15 = new TimeSpan(0, 0, 0);
                            ot3 = new TimeSpan(0, 0, 0);
                            leave = new TimeSpan(0, 0, 0);
                            // Check Holiday and Get day
                            if (isHoliday || isWeekend)
                            {
                                if (_wd[i].workings[j].task_name == "Traveling")
                                {
                                    ot15 = default(TimeSpan);
                                    ot3 = default(TimeSpan);

                                    if (_wd[i].workings[j].lunch_full)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].lunch_half)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(0, 30, 0);
                                        }
                                    }

                                    if (_wd[i].workings[j].dinner_full)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_half)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(0, 30, 0);
                                        }
                                    }

                                    wh = new WorkingHoursModel()
                                    {
                                        working_date = _wd[i].date,
                                        job_id = _wd[i].workings[j].job_id,
                                        job_name = _wd[i].workings[j].job_name,
                                        task_id = _wd[i].workings[j].task_id,
                                        task_name = _wd[i].workings[j].task_name,
                                        start_time = _wd[i].workings[j].start_time,
                                        stop_time = _wd[i].workings[j].stop_time,
                                        lunch_full = _wd[i].workings[j].lunch_full,
                                        lunch_half = _wd[i].workings[j].lunch_half,
                                        dinner_full = _wd[i].workings[j].dinner_full,
                                        dinner_half = _wd[i].workings[j].dinner_half,
                                        day = day,
                                        normal = regular,
                                        ot1_5 = ot15,
                                        ot3_0 = ot3,
                                        leave = leave
                                    };
                                    monthly.Add(wh);
                                    regular = new TimeSpan(0, 0, 0);
                                    continue;
                                }
                                else
                                {
                                    if (_wd[i].workings[j].stop_time == new TimeSpan(23, 59, 0))
                                    {
                                        ot15 += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time).Add(new TimeSpan(0, 1, 0));
                                    }
                                    else
                                    {
                                        ot15 += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                    }

                                }
                                if (_wd[i].workings[j].lunch_full)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].lunch_half)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(0, 30, 0);
                                    }
                                }

                                if (_wd[i].workings[j].dinner_full)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(1, 0, 0);
                                    }
                                }
                                if (_wd[i].workings[j].dinner_half)
                                {
                                    if (ot15 != default(TimeSpan))
                                    {
                                        ot15 -= new TimeSpan(0, 30, 0);
                                    }
                                }


                                if (ot15 >= new TimeSpan(8, 0, 0))
                                {
                                    ot3 = ot15 - new TimeSpan(8, 0, 0);
                                }

                                if (ot3 > new TimeSpan(0, 0, 0))
                                {
                                    if (j == 0)
                                    {
                                        ot15 = new TimeSpan(8, 0, 0);
                                    }
                                    else
                                    {
                                        ot15 = default(TimeSpan);
                                    }
                                }

                                //Check Sum OT 1.5
                                TimeSpan sum_ot15 = monthly.Where(w => w.working_date.Date == date.Date && w.task_name != "Traveling").ToList().Aggregate(
                                    TimeSpan.Zero, (sum_ot, next_ot) => sum_ot + next_ot.ot1_5) + ot15;
                                if (sum_ot15 > new TimeSpan(8, 0, 0))
                                {
                                    ot15 = new TimeSpan(8, 0, 0) - (sum_ot15 - ot15);
                                    ot3 = sum_ot15 - new TimeSpan(8, 0, 0);
                                }

                                wh = new WorkingHoursModel()
                                {
                                    working_date = _wd[i].date,
                                    job_id = _wd[i].workings[j].job_id,
                                    job_name = _wd[i].workings[j].job_name,
                                    task_id = _wd[i].workings[j].task_id,
                                    task_name = _wd[i].workings[j].task_name,
                                    start_time = _wd[i].workings[j].start_time,
                                    stop_time = _wd[i].workings[j].stop_time,
                                    lunch_full = _wd[i].workings[j].lunch_full,
                                    lunch_half = _wd[i].workings[j].lunch_half,
                                    dinner_full = _wd[i].workings[j].dinner_full,
                                    dinner_half = _wd[i].workings[j].dinner_half,
                                    day = day,
                                    normal = default(TimeSpan),
                                    ot1_5 = ot15,
                                    ot3_0 = ot3,
                                    leave = default(TimeSpan)
                                };
                                monthly.Add(wh);
                            }
                            else  // Regular day
                            {
                                day = _wd[i].workings[j].working_date.DayOfWeek.ToString();

                                if (_wd[i].workings[j].task_name == "Traveling")
                                {                                 
                                    regular = (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                    ot15 = default(TimeSpan);
                                    ot3 = default(TimeSpan);

                                    if (_wd[i].workings[j].lunch_full)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].lunch_half)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(0, 30, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_full)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_half)
                                    {
                                        if (regular != default(TimeSpan))
                                        {
                                            regular -= new TimeSpan(0, 30, 0);
                                        }
                                    }

                                    wh = new WorkingHoursModel()
                                    {
                                        working_date = _wd[i].date,
                                        job_id = _wd[i].workings[j].job_id,
                                        job_name = _wd[i].workings[j].job_name,
                                        task_id = _wd[i].workings[j].task_id,
                                        task_name = _wd[i].workings[j].task_name,
                                        start_time = _wd[i].workings[j].start_time,
                                        stop_time = _wd[i].workings[j].stop_time,
                                        lunch_full = _wd[i].workings[j].lunch_full,
                                        lunch_half = _wd[i].workings[j].lunch_half,
                                        dinner_full = _wd[i].workings[j].dinner_full,
                                        dinner_half = _wd[i].workings[j].dinner_half,
                                        day = day,
                                        normal = regular,
                                        ot1_5 = ot15,
                                        ot3_0 = ot3,
                                        leave = leave
                                    };
                                    monthly.Add(wh);
                                    regular = new TimeSpan(0, 0, 0);
                                    continue;
                                }
                                else if (_wd[i].workings[j].task_name == "Leave")
                                {
                                    regular = default(TimeSpan);
                                    ot15 = default(TimeSpan);
                                    ot3 = default(TimeSpan);
                                    leave = (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                    if (leave > new TimeSpan(8, 0, 0))
                                    {
                                        leave = new TimeSpan(8, 0, 0);
                                    }

                                    wh = new WorkingHoursModel()
                                    {
                                        working_date = _wd[i].date,
                                        job_id = _wd[i].workings[j].job_id,
                                        job_name = _wd[i].workings[j].job_name,
                                        task_id = _wd[i].workings[j].task_id,
                                        task_name = _wd[i].workings[j].task_name,
                                        start_time = _wd[i].workings[j].start_time,
                                        stop_time = _wd[i].workings[j].stop_time,
                                        lunch_full = _wd[i].workings[j].lunch_full,
                                        lunch_half = _wd[i].workings[j].lunch_half,
                                        dinner_full = _wd[i].workings[j].dinner_full,
                                        dinner_half = _wd[i].workings[j].dinner_half,
                                        day = day,
                                        normal = regular,
                                        ot1_5 = ot15,
                                        ot3_0 = ot3,
                                        leave = leave
                                    };
                                    monthly.Add(wh);
                                    regular = new TimeSpan(0, 0, 0);
                                    continue;
                                }
                                else
                                {
                                    if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time < new TimeSpan(8, 30, 0))
                                    {
                                        ot15 += _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                        regular = default(TimeSpan);
                                        ot3 = default(TimeSpan);
                                    }
                                    else if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time <= new TimeSpan(17, 30, 0))
                                    {
                                        ot15 += new TimeSpan(8, 30, 0) - _wd[i].workings[j].start_time;
                                        regular = _wd[i].workings[j].stop_time - new TimeSpan(8, 30, 0);
                                        ot3 = default(TimeSpan);
                                        if (_wd[i].workings[j].lunch_full)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].lunch_half)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(0, 30, 0);
                                            }
                                        }
                                    }
                                    else if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(17, 30, 0))
                                    {
                                        ot15 += (new TimeSpan(8, 30, 0) - _wd[i].workings[j].start_time) + (_wd[i].workings[j].stop_time - new TimeSpan(17,30,0));
                                        regular = new TimeSpan(17, 30, 0) - new TimeSpan(8, 30, 0);
                                        ot3 = default(TimeSpan);
                                        if (_wd[i].workings[j].lunch_full)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].lunch_half)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(0, 30, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].dinner_full)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].dinner_half)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(0, 30, 0);
                                            }
                                        }
                                    }
                                    else if (_wd[i].workings[j].start_time >= new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time <= new TimeSpan(17, 30, 0))
                                    {
                                        ot15 += default(TimeSpan);
                                        regular = _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                        ot3 = default(TimeSpan);
                                        if (_wd[i].workings[j].lunch_full)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].lunch_half)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(0, 30, 0);
                                            }
                                        }
                                    }
                                    else if (_wd[i].workings[j].start_time >= new TimeSpan(8, 30, 0) && _wd[i].workings[j].start_time <= new TimeSpan(17, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(17, 30, 0))
                                    {
                                        ot15 += _wd[i].workings[j].stop_time - new TimeSpan(17, 30, 0);
                                        regular = new TimeSpan(17, 30, 0) - _wd[i].workings[j].start_time;
                                        ot3 = default(TimeSpan);
                                        if (_wd[i].workings[j].lunch_full)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].lunch_half)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(0, 30, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].dinner_full)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].dinner_half)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(0, 30, 0);
                                            }
                                        }
                                    }
                                    else if (_wd[i].workings[j].start_time >= new TimeSpan(17, 30, 0))
                                    {
                                        ot15 += _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                        regular = default(TimeSpan);
                                        ot3 = default(TimeSpan);
                                        if (_wd[i].workings[j].dinner_full)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].dinner_half)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(0, 30, 0);
                                            }
                                        }
                                    }

                                    wh = new WorkingHoursModel()
                                    {
                                        working_date = _wd[i].date,
                                        job_id = _wd[i].workings[j].job_id,
                                        job_name = _wd[i].workings[j].job_name,
                                        task_id = _wd[i].workings[j].task_id,
                                        task_name = _wd[i].workings[j].task_name,
                                        start_time = _wd[i].workings[j].start_time,
                                        stop_time = _wd[i].workings[j].stop_time,
                                        lunch_full = _wd[i].workings[j].lunch_full,
                                        lunch_half = _wd[i].workings[j].lunch_half,
                                        dinner_full = _wd[i].workings[j].dinner_full,
                                        dinner_half = _wd[i].workings[j].dinner_half,
                                        day = day,
                                        normal = regular,
                                        ot1_5 = ot15,
                                        ot3_0 = ot3,
                                        leave = leave
                                    };
                                    monthly.Add(wh);
                                }
                            }
                        }
                    }
                }
                else
                {
                    wh = new WorkingHoursModel()
                    {
                        working_date = date,
                        day = day,
                        job_id = "",
                        job_name = "",
                        task_id = "",
                        task_name = "",
                        start_time = default(TimeSpan),
                        stop_time = default(TimeSpan),
                        lunch_full = false,
                        lunch_half = false,
                        dinner_full = false,
                        dinner_half = false,
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

        public List<WorkingHoursModel> CalculateWorkingHours(string user_name, string month)
        {
            List<WorkingHoursModel> monthly = new List<WorkingHoursModel>();

            string day = "";
            int yy = Convert.ToInt32(month.Split("-")[0]);
            int mm = Convert.ToInt32(month.Split("-")[1]);

            List<WorkingDayModel> whs = GetWorkingHours(yy.ToString(), mm.ToString().PadLeft(2, '0'), user_name);
            List<HolidayModel> holidays = Holiday.GetHolidays(yy.ToString());
            WorkingHoursModel wh = new WorkingHoursModel();
            TimeSpan working_date = new TimeSpan(0, 0, 0);
            for (DateTime date = new DateTime(yy, mm, 1); date <= new DateTime(yy, mm, DateTime.DaysInMonth(yy, mm)); date = date.AddDays(1))
            {
                //if (date == new DateTime(2025, 10, 30))
                {
                    day = date.DayOfWeek.ToString();
                    List<WorkingDayModel> _wd = whs.Where(w => w.date.Date == date).ToList();
                    //_wd = _wd.Where(w => w.date == new DateTime(2025, 10, 15)).ToList();
                    bool isHoliday = holidays.Where(w => w.date.Date == date.Date).Count() > 0 ? true : false;
                    bool isWeekend = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? true : false;
                    if (isHoliday)
                    {
                        day = "Holiday";
                    }
                    if (_wd.Count > 0)
                    {
                        for (int i = 0; i < _wd.Count; i++)
                        {
                            _wd[i].workings = _wd[i].workings.OrderBy(o => o.start_time).ToList();

                            working_date = new TimeSpan(0, 0, 0);

                            TimeSpan regular = new TimeSpan(0, 0, 0);
                            TimeSpan ot15 = new TimeSpan(0, 0, 0);
                            TimeSpan ot3 = new TimeSpan(0, 0, 0);
                            TimeSpan leave = new TimeSpan(0, 0, 0);

                            //Check Total Time 8 Hours
                            TimeSpan wd_time = new TimeSpan(0, 0, 0);
                            for (int j = 0; j < _wd[i].workings.Count; j++)
                            {
                                if (_wd[i].workings[j].task_name != "Traveling" && _wd[i].workings[j].task_name != "Leave")
                                {
                                    wd_time += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                    if (_wd[i].workings[j].lunch_full)
                                    {
                                        if (wd_time != default(TimeSpan))
                                        {
                                            wd_time -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].lunch_half)
                                    {
                                        if (wd_time != default(TimeSpan))
                                        {
                                            wd_time -= new TimeSpan(0, 30, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_full)
                                    {
                                        if (wd_time != default(TimeSpan))
                                        {
                                            wd_time -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_half)
                                    {
                                        if (wd_time != default(TimeSpan))
                                        {
                                            wd_time -= new TimeSpan(0, 30, 0);
                                        }
                                    }
                                }
                            }
                            ///
                            for (int j = 0; j < _wd[i].workings.Count; j++)
                            {
                                regular = new TimeSpan(0, 0, 0);
                                ot15 = new TimeSpan(0, 0, 0);
                                ot3 = new TimeSpan(0, 0, 0);
                                leave = new TimeSpan(0, 0, 0);
                                // Check Holiday and Get day
                                if (isHoliday || isWeekend)
                                {
                                    if (_wd[i].workings[j].task_name == "Traveling")
                                    {
                                        ot15 = default(TimeSpan);
                                        ot3 = default(TimeSpan);

                                        if (_wd[i].workings[j].lunch_full)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].lunch_half)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(0, 30, 0);
                                            }
                                        }

                                        if (_wd[i].workings[j].dinner_full)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].dinner_half)
                                        {
                                            if (ot15 != default(TimeSpan))
                                            {
                                                ot15 -= new TimeSpan(0, 30, 0);
                                            }
                                        }

                                        wh = new WorkingHoursModel()
                                        {
                                            working_date = _wd[i].date,
                                            job_id = _wd[i].workings[j].job_id,
                                            job_name = _wd[i].workings[j].job_name,
                                            task_id = _wd[i].workings[j].task_id,
                                            task_name = _wd[i].workings[j].task_name,
                                            start_time = _wd[i].workings[j].start_time,
                                            stop_time = _wd[i].workings[j].stop_time,
                                            lunch_full = _wd[i].workings[j].lunch_full,
                                            lunch_half = _wd[i].workings[j].lunch_half,
                                            dinner_full = _wd[i].workings[j].dinner_full,
                                            dinner_half = _wd[i].workings[j].dinner_half,
                                            day = day,
                                            normal = regular,
                                            ot1_5 = ot15,
                                            ot3_0 = ot3,
                                            leave = leave
                                        };
                                        monthly.Add(wh);
                                        regular = new TimeSpan(0, 0, 0);
                                        continue;
                                    }
                                    else
                                    {
                                        if (_wd[i].workings[j].stop_time == new TimeSpan(23, 59, 0))
                                        {
                                            ot15 += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time).Add(new TimeSpan(0, 1, 0));
                                        }
                                        else
                                        {
                                            ot15 += (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                        }

                                    }
                                    if (_wd[i].workings[j].lunch_full)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].lunch_half)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(0, 30, 0);
                                        }
                                    }

                                    if (_wd[i].workings[j].dinner_full)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(1, 0, 0);
                                        }
                                    }
                                    if (_wd[i].workings[j].dinner_half)
                                    {
                                        if (ot15 != default(TimeSpan))
                                        {
                                            ot15 -= new TimeSpan(0, 30, 0);
                                        }
                                    }


                                    if (ot15 >= new TimeSpan(8, 0, 0))
                                    {
                                        ot3 = ot15 - new TimeSpan(8, 0, 0);
                                    }

                                    if (ot3 > new TimeSpan(0, 0, 0))
                                    {
                                        if (j == 0)
                                        {
                                            ot15 = new TimeSpan(8, 0, 0);
                                        }
                                        else
                                        {
                                            ot15 = default(TimeSpan);
                                        }
                                    }

                                    //Check Sum OT 1.5
                                    TimeSpan sum_ot15 = monthly.Where(w => w.working_date.Date == date.Date && w.task_name != "Traveling").ToList().Aggregate(
                                        TimeSpan.Zero, (sum_ot, next_ot) => sum_ot + next_ot.ot1_5) + ot15;
                                    if (sum_ot15 > new TimeSpan(8, 0, 0))
                                    {
                                        ot15 = new TimeSpan(8, 0, 0) - (sum_ot15 - ot15);
                                        ot3 = sum_ot15 - new TimeSpan(8, 0, 0);
                                    }

                                    wh = new WorkingHoursModel()
                                    {
                                        working_date = _wd[i].date,
                                        job_id = _wd[i].workings[j].job_id,
                                        job_name = _wd[i].workings[j].job_name,
                                        task_id = _wd[i].workings[j].task_id,
                                        task_name = _wd[i].workings[j].task_name,
                                        start_time = _wd[i].workings[j].start_time,
                                        stop_time = _wd[i].workings[j].stop_time,
                                        lunch_full = _wd[i].workings[j].lunch_full,
                                        lunch_half = _wd[i].workings[j].lunch_half,
                                        dinner_full = _wd[i].workings[j].dinner_full,
                                        dinner_half = _wd[i].workings[j].dinner_half,
                                        day = day,
                                        normal = default(TimeSpan),
                                        ot1_5 = ot15,
                                        ot3_0 = ot3,
                                        leave = default(TimeSpan)
                                    };
                                    monthly.Add(wh);
                                }
                                else  // Regular day
                                {
                                    day = _wd[i].workings[j].working_date.DayOfWeek.ToString();

                                    if (_wd[i].workings[j].task_name == "Traveling")
                                    {
                                        regular = (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                        ot15 = default(TimeSpan);
                                        ot3 = default(TimeSpan);

                                        if (_wd[i].workings[j].lunch_full)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].lunch_half)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(0, 30, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].dinner_full)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(1, 0, 0);
                                            }
                                        }
                                        if (_wd[i].workings[j].dinner_half)
                                        {
                                            if (regular != default(TimeSpan))
                                            {
                                                regular -= new TimeSpan(0, 30, 0);
                                            }
                                        }

                                        wh = new WorkingHoursModel()
                                        {
                                            working_date = _wd[i].date,
                                            job_id = _wd[i].workings[j].job_id,
                                            job_name = _wd[i].workings[j].job_name,
                                            task_id = _wd[i].workings[j].task_id,
                                            task_name = _wd[i].workings[j].task_name,
                                            start_time = _wd[i].workings[j].start_time,
                                            stop_time = _wd[i].workings[j].stop_time,
                                            lunch_full = _wd[i].workings[j].lunch_full,
                                            lunch_half = _wd[i].workings[j].lunch_half,
                                            dinner_full = _wd[i].workings[j].dinner_full,
                                            dinner_half = _wd[i].workings[j].dinner_half,
                                            day = day,
                                            normal = regular,
                                            ot1_5 = wd_time >= new TimeSpan(8, 0, 0) ? ot15 : default(TimeSpan),
                                            ot3_0 = wd_time >= new TimeSpan(8, 0, 0) ? ot3 : default(TimeSpan),
                                            leave = leave
                                        };
                                        monthly.Add(wh);
                                        regular = new TimeSpan(0, 0, 0);
                                        continue;
                                    }
                                    else if (_wd[i].workings[j].task_name == "Leave")
                                    {
                                        regular = default(TimeSpan);
                                        ot15 = default(TimeSpan);
                                        ot3 = default(TimeSpan);
                                        leave = (_wd[i].workings[j].stop_time - _wd[i].workings[j].start_time);
                                        if (leave > new TimeSpan(8, 0, 0))
                                        {
                                            leave = new TimeSpan(8, 0, 0);
                                        }

                                        wh = new WorkingHoursModel()
                                        {
                                            working_date = _wd[i].date,
                                            job_id = _wd[i].workings[j].job_id,
                                            job_name = _wd[i].workings[j].job_name,
                                            task_id = _wd[i].workings[j].task_id,
                                            task_name = _wd[i].workings[j].task_name,
                                            start_time = _wd[i].workings[j].start_time,
                                            stop_time = _wd[i].workings[j].stop_time,
                                            lunch_full = _wd[i].workings[j].lunch_full,
                                            lunch_half = _wd[i].workings[j].lunch_half,
                                            dinner_full = _wd[i].workings[j].dinner_full,
                                            dinner_half = _wd[i].workings[j].dinner_half,
                                            day = day,
                                            normal = regular,
                                            ot1_5 = wd_time >= new TimeSpan(8, 0, 0) ? ot15 : default(TimeSpan),
                                            ot3_0 = wd_time >= new TimeSpan(8, 0, 0) ? ot3 : default(TimeSpan),
                                            leave = leave
                                        };
                                        monthly.Add(wh);
                                        regular = new TimeSpan(0, 0, 0);
                                        continue;
                                    }
                                    else
                                    {
                                        if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time < new TimeSpan(8, 30, 0))
                                        {
                                            ot15 += _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                            regular = default(TimeSpan);
                                            ot3 = default(TimeSpan);
                                        }
                                        else if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time <= new TimeSpan(17, 30, 0))
                                        {
                                            ot15 += new TimeSpan(8, 30, 0) - _wd[i].workings[j].start_time;
                                            regular = _wd[i].workings[j].stop_time - new TimeSpan(8, 30, 0);
                                            ot3 = default(TimeSpan);
                                            if (_wd[i].workings[j].lunch_full)
                                            {
                                                if (regular != default(TimeSpan))
                                                {
                                                    regular -= new TimeSpan(1, 0, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].lunch_half)
                                            {
                                                if (regular != default(TimeSpan))
                                                {
                                                    regular -= new TimeSpan(0, 30, 0);
                                                }
                                            }
                                        }
                                        else if (_wd[i].workings[j].start_time < new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(17, 30, 0))
                                        {
                                            ot15 += (new TimeSpan(8, 30, 0) - _wd[i].workings[j].start_time) + (_wd[i].workings[j].stop_time - new TimeSpan(17, 30, 0));
                                            regular = new TimeSpan(17, 30, 0) - new TimeSpan(8, 30, 0);
                                            ot3 = default(TimeSpan);
                                            if (_wd[i].workings[j].lunch_full)
                                            {
                                                if (regular != default(TimeSpan))
                                                {
                                                    regular -= new TimeSpan(1, 0, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].lunch_half)
                                            {
                                                if (regular != default(TimeSpan))
                                                {
                                                    regular -= new TimeSpan(0, 30, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].dinner_full)
                                            {
                                                if (ot15 != default(TimeSpan))
                                                {
                                                    ot15 -= new TimeSpan(1, 0, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].dinner_half)
                                            {
                                                if (ot15 != default(TimeSpan))
                                                {
                                                    ot15 -= new TimeSpan(0, 30, 0);
                                                }
                                            }
                                        }
                                        else if (_wd[i].workings[j].start_time >= new TimeSpan(8, 30, 0) && _wd[i].workings[j].stop_time <= new TimeSpan(17, 30, 0))
                                        {
                                            ot15 += default(TimeSpan);
                                            regular = _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                            ot3 = default(TimeSpan);
                                            if (_wd[i].workings[j].lunch_full)
                                            {
                                                if (regular != default(TimeSpan))
                                                {
                                                    regular -= new TimeSpan(1, 0, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].lunch_half)
                                            {
                                                if (regular != default(TimeSpan))
                                                {
                                                    regular -= new TimeSpan(0, 30, 0);
                                                }
                                            }
                                        }
                                        else if (_wd[i].workings[j].start_time >= new TimeSpan(8, 30, 0) && _wd[i].workings[j].start_time <= new TimeSpan(17, 30, 0) && _wd[i].workings[j].stop_time > new TimeSpan(17, 30, 0))
                                        {
                                            ot15 += _wd[i].workings[j].stop_time - new TimeSpan(17, 30, 0);
                                            regular = new TimeSpan(17, 30, 0) - _wd[i].workings[j].start_time;
                                            ot3 = default(TimeSpan);
                                            if (_wd[i].workings[j].lunch_full)
                                            {
                                                if (regular != default(TimeSpan))
                                                {
                                                    regular -= new TimeSpan(1, 0, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].lunch_half)
                                            {
                                                if (regular != default(TimeSpan))
                                                {
                                                    regular -= new TimeSpan(0, 30, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].dinner_full)
                                            {
                                                if (ot15 != default(TimeSpan))
                                                {
                                                    ot15 -= new TimeSpan(1, 0, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].dinner_half)
                                            {
                                                if (ot15 != default(TimeSpan))
                                                {
                                                    ot15 -= new TimeSpan(0, 30, 0);
                                                }
                                            }
                                        }
                                        else if (_wd[i].workings[j].start_time >= new TimeSpan(17, 30, 0))
                                        {
                                            ot15 += _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                            regular = _wd[i].workings[j].stop_time - _wd[i].workings[j].start_time;
                                            //regular = default(TimeSpan);
                                            ot3 = default(TimeSpan);
                                            if (_wd[i].workings[j].dinner_full)
                                            {
                                                if (regular != default(TimeSpan))
                                                {
                                                    regular -= new TimeSpan(1, 0, 0);
                                                }
                                            }
                                            if (_wd[i].workings[j].dinner_half)
                                            {
                                                if (ot15 != default(TimeSpan))
                                                {
                                                    ot15 -= new TimeSpan(0, 30, 0);
                                                }
                                            }
                                        }

                                        wh = new WorkingHoursModel()
                                        {
                                            working_date = _wd[i].date,
                                            job_id = _wd[i].workings[j].job_id,
                                            job_name = _wd[i].workings[j].job_name,
                                            task_id = _wd[i].workings[j].task_id,
                                            task_name = _wd[i].workings[j].task_name,
                                            start_time = _wd[i].workings[j].start_time,
                                            stop_time = _wd[i].workings[j].stop_time,
                                            lunch_full = _wd[i].workings[j].lunch_full,
                                            lunch_half = _wd[i].workings[j].lunch_half,
                                            dinner_full = _wd[i].workings[j].dinner_full,
                                            dinner_half = _wd[i].workings[j].dinner_half,
                                            day = day,
                                            normal = regular,
                                            ot1_5 = wd_time >= new TimeSpan(8, 0, 0) ? ot15 : default(TimeSpan),
                                            ot3_0 = wd_time >= new TimeSpan(8, 0, 0) ? ot3 : default(TimeSpan),
                                            leave = leave
                                        };
                                        monthly.Add(wh);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        wh = new WorkingHoursModel()
                        {
                            working_date = date,
                            day = day,
                            job_id = "",
                            job_name = "",
                            task_id = "",
                            task_name = "",
                            start_time = default(TimeSpan),
                            stop_time = default(TimeSpan),
                            lunch_full = false,
                            lunch_half = false,
                            dinner_full = false,
                            dinner_half = false,
                            normal = default(TimeSpan),
                            ot1_5 = default(TimeSpan),
                            ot3_0 = default(TimeSpan),
                            leave = default(TimeSpan)
                        };
                        monthly.Add(wh);
                    }
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

        public WorkingHoursModel GetWorkingHourByLeave(string user_id, string working_date)
        {
            WorkingHoursModel wh = new WorkingHoursModel();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT * FROM WorkingHours WHERE user_id = @user_id AND working_date = @working_date AND job_id = 'J999999'");
                SqlCommand cmd = new SqlCommand(string_command, con);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                cmd.Parameters.AddWithValue("@working_date", working_date);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        wh = new WorkingHoursModel()
                        {
                            index = dr["ind"].ToString(),
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            working_date = dr["working_date"] != DBNull.Value ? Convert.ToDateTime(dr["working_date"]) : default(DateTime),
                            week_number = dr["week_number"] != DBNull.Value ? Convert.ToInt32(dr["week_number"]) : default(Int32),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                        };
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
            return wh;
        }
    }
}
