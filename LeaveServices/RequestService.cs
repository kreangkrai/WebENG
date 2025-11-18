using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;
using WebENG.Service;

namespace WebENG.LeaveServices
{
    public class RequestService : IRequest
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public RequestService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<RequestModel> GetRequests()
        {
            List<RequestModel> requests = new List<RequestModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [request_id]
                                                          ,[emp_id]
                                                          ,[request].[leave_type_id]
														  ,Leave_type.leave_type_code
                                                          ,[is_full_day]
                                                          ,[start_request_date]
                                                          ,[end_request_date]
                                                          ,[amount_leave_day]
                                                          ,[start_request_time]
                                                          ,[end_request_time]
                                                          ,[amount_leave_hour]
                                                          ,[path_file]
                                                          ,[request_date]
                                                          ,[manager_approver_status]
                                                          ,[manager_approver]
                                                          ,[manager_approve_date]
                                                          ,[director_approver_status]
                                                          ,[director_approver]
                                                          ,[director_approve_date]
                                                          ,[admin_approver_status]
                                                          ,[admin_approver]
                                                          ,[admin_approve_date]
                                                          ,[decsription]
                                                          ,[status_request]
														  ,Leave_type.leave_name_th
														  ,Leave_type.leave_name_en
                                                          ,Leave_type.color_code
                                                          ,[request].[is_two_step_approve]
                                                      FROM [dbo].[request] 
													  LEFT JOIN Leave_type ON [request].leave_type_id = Leave_type.leave_type_id
                                                      WHERE [request_id] IS NOT NULL");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RequestModel request = new RequestModel()
                        {
                            request_id = dr["request_id"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            leave_type_id = dr["leave_type_id"].ToString(),
                            leave_type_code = dr["leave_type_code"].ToString(),
                            leave_name_th = dr["leave_name_th"].ToString(),
                            leave_name_en = dr["leave_name_en"].ToString(),
                            is_full_day = dr["is_full_day"] != DBNull.Value ? Convert.ToBoolean(dr["is_full_day"].ToString()) : false,
                            start_request_date = dr["start_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_request_date"].ToString()) : DateTime.MinValue,
                            end_request_date = dr["end_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["end_request_date"].ToString()) : DateTime.MinValue,
                            amount_leave_day = dr["amount_leave_day"] != DBNull.Value ? Convert.ToInt32(dr["amount_leave_day"].ToString()) : 0,
                            start_request_time = dr["start_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["start_request_time"].ToString()).Ticks) : new TimeSpan(0,0,0),
                            end_request_time = dr["end_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["end_request_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            amount_leave_hour = dr["amount_leave_hour"] != DBNull.Value ? Convert.ToDecimal(dr["amount_leave_hour"].ToString()) : 1,
                            path_file = dr["path_file"].ToString(),
                            request_date = dr["request_date"] != DBNull.Value ? Convert.ToDateTime(dr["request_date"].ToString()) : DateTime.MinValue,
                            manager_approver = dr["manager_approver"].ToString(),
                            manager_approve_date = dr["manager_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["manager_approve_date"].ToString()) : DateTime.MinValue,
                            manager_approver_status = dr["manager_approver_status"].ToString(),
                            director_approver = dr["director_approver"].ToString(),
                            director_approve_date = dr["director_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["director_approve_date"].ToString()) : DateTime.MinValue,
                            director_approver_status = dr["director_approver_status"].ToString(),
                            admin_approver = dr["admin_approver"].ToString(),
                            admin_approve_date = dr["admin_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["admin_approve_date"].ToString()) : DateTime.MinValue,
                            admin_approver_status = dr["admin_approver_status"].ToString(),
                            decsription = dr["decsription"].ToString(),
                            status_request = dr["status_request"].ToString(),
                            is_two_step_approve = dr["is_two_step_approve"] != DBNull.Value ? Convert.ToBoolean(dr["is_two_step_approve"].ToString()) : false,
                            color_code = dr["color_code"].ToString()
                        };
                        requests.Add(request);
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
            return requests;
        }
        public RequestModel GetRequestByID(string request_id)
        {
            RequestModel request = new RequestModel();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [request_id]
                                                          ,[emp_id]
                                                          ,[request].[leave_type_id]
														  ,Leave_type.leave_type_code
                                                          ,[is_full_day]
                                                          ,[start_request_date]
                                                          ,[end_request_date]
                                                          ,[amount_leave_day]
                                                          ,[start_request_time]
                                                          ,[end_request_time]
                                                          ,[amount_leave_hour]
                                                          ,[path_file]
                                                          ,[request_date]
                                                          ,[manager_approver_status]
                                                          ,[manager_approver]
                                                          ,[manager_approve_date]
                                                          ,[director_approver_status]
                                                          ,[director_approver]
                                                          ,[director_approve_date]
                                                          ,[admin_approver_status]
                                                          ,[admin_approver]
                                                          ,[admin_approve_date]
                                                          ,[decsription]
                                                          ,[status_request]
														  ,Leave_type.leave_name_th
														  ,Leave_type.leave_name_en
                                                          ,Leave_type.color_code
                                                          ,[request].[is_two_step_approve]
                                                      FROM [dbo].[request] 
													  LEFT JOIN Leave_type ON [request].leave_type_id = Leave_type.leave_type_id
                                                      WHERE [request_id] = @request_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_id", request_id);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        request = new RequestModel()
                        {
                            request_id = dr["request_id"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            leave_type_id = dr["leave_type_id"].ToString(),
                            leave_type_code = dr["leave_type_code"].ToString(),
                            leave_name_th = dr["leave_name_th"].ToString(),
                            leave_name_en = dr["leave_name_en"].ToString(),
                            is_full_day = dr["is_full_day"] != DBNull.Value ? Convert.ToBoolean(dr["is_full_day"].ToString()) : false,
                            start_request_date = dr["start_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_request_date"].ToString()) : DateTime.MinValue,
                            end_request_date = dr["end_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["end_request_date"].ToString()) : DateTime.MinValue,
                            amount_leave_day = dr["amount_leave_day"] != DBNull.Value ? Convert.ToInt32(dr["amount_leave_day"].ToString()) : 0,
                            start_request_time = dr["start_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["start_request_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            end_request_time = dr["end_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["end_request_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            amount_leave_hour = dr["amount_leave_hour"] != DBNull.Value ? Convert.ToDecimal(dr["amount_leave_hour"].ToString()) : 1,
                            path_file = dr["path_file"].ToString(),
                            request_date = dr["request_date"] != DBNull.Value ? Convert.ToDateTime(dr["request_date"].ToString()) : DateTime.MinValue,
                            manager_approver = dr["manager_approver"].ToString(),
                            manager_approve_date = dr["manager_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["manager_approve_date"].ToString()) : DateTime.MinValue,
                            manager_approver_status = dr["manager_approver_status"].ToString(),
                            director_approver = dr["director_approver"].ToString(),
                            director_approve_date = dr["director_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["director_approve_date"].ToString()) : DateTime.MinValue,
                            director_approver_status = dr["director_approver_status"].ToString(),
                            admin_approver = dr["admin_approver"].ToString(),
                            admin_approve_date = dr["admin_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["admin_approve_date"].ToString()) : DateTime.MinValue,
                            admin_approver_status = dr["admin_approver_status"].ToString(),
                            decsription = dr["decsription"].ToString(),
                            status_request = dr["status_request"].ToString(),
                            is_two_step_approve = dr["is_two_step_approve"] != DBNull.Value ? Convert.ToBoolean(dr["is_two_step_approve"].ToString()) : false,
                            color_code = dr["color_code"].ToString()
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
            return request;
        }

        public string Insert(RequestModel request)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [dbo].[request]
                                                   ([request_id]
                                                   ,[emp_id]
                                                   ,[leave_type_id]
                                                   ,[is_full_day]
                                                   ,[start_request_date]
                                                   ,[end_request_date]
                                                   ,[amount_leave_day]
                                                   ,[start_request_time]
                                                   ,[end_request_time]
                                                   ,[amount_leave_hour]
                                                   ,[path_file]
                                                   ,[request_date]
                                                   ,[decsription]
                                                   ,[status_request]
                                                   ,[is_two_step_approve])
                                             VALUES
                                                   (@request_id
                                                   ,@emp_id
                                                   ,@leave_type_id
                                                   ,@is_full_day
                                                   ,@start_request_date
                                                   ,@end_request_date
                                                   ,@amount_leave_day
                                                   ,@start_request_time
                                                   ,@end_request_time
                                                   ,@amount_leave_hour
                                                   ,@path_file
                                                   ,@request_date
                                                   ,@decsription
                                                   ,@status_request
                                                   ,@is_two_step_approve)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_id", request.request_id);
                command.Parameters.AddWithValue("@emp_id", request.emp_id);
                command.Parameters.AddWithValue("@leave_type_id", request.leave_type_id);
                command.Parameters.AddWithValue("@is_full_day", request.is_full_day);
                command.Parameters.AddWithValue("@start_request_date", request.start_request_date);
                command.Parameters.AddWithValue("@end_request_date", request.end_request_date);
                command.Parameters.AddWithValue("@amount_leave_day", request.amount_leave_day);
                command.Parameters.AddWithValue("@start_request_time", request.start_request_time);
                command.Parameters.AddWithValue("@end_request_time", request.end_request_time);
                command.Parameters.AddWithValue("@amount_leave_hour", request.amount_leave_hour);
                command.Parameters.AddWithValue("@path_file", request.path_file);
                command.Parameters.AddWithValue("@request_date", request.request_date);
                command.Parameters.AddWithValue("@decsription", request.decsription);
                command.Parameters.AddWithValue("@status_request", request.status_request);
                command.Parameters.AddWithValue("@is_two_step_approve", request.is_two_step_approve);
                command.ExecuteNonQuery();
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

        public string Update(RequestModel request)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"UPDATE [dbo].[request]
                                                   SET [leave_type_id] = @leave_type_id
                                                      ,[is_full_day] = @is_full_day
                                                      ,[start_request_date] = @start_request_date
                                                      ,[end_request_date] = @end_request_date
                                                      ,[amount_leave_day] = @amount_leave_day
                                                      ,[start_request_time] = @start_request_time
                                                      ,[end_request_time] = @end_request_time
                                                      ,[amount_leave_hour] = @amount_leave_hour
                                                      ,[path_file] = @path_file
                                                      ,[request_date] = @request_date
                                                      ,[manager_approver_status] = @manager_approver_status
                                                      ,[manager_approver] = @manager_approver
                                                      ,[manager_approve_date] = @manager_approve_date
                                                      ,[director_approver_status] = @director_approver_status
                                                      ,[director_approver] = @director_approver
                                                      ,[director_approve_date] = @director_approve_date
                                                      ,[admin_approver_status] = @admin_approver_status
                                                      ,[admin_approver] = @admin_approver
                                                      ,[admin_approve_date] = @admin_approve_date
                                                      ,[decsription] = @decsription
                                                      ,[status_request] = @status_request
                                                      ,[is_two_step_approve] = @is_two_step_approve
                                                 WHERE [request_id] = @request_id AND  [emp_id] = @emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_id", request.request_id);
                command.Parameters.AddWithValue("@emp_id", request.emp_id);
                command.Parameters.AddWithValue("@leave_type_id", request.leave_type_id);
                command.Parameters.AddWithValue("@is_full_day", request.is_full_day);
                command.Parameters.AddWithValue("@start_request_date", request.start_request_date);
                command.Parameters.AddWithValue("@end_request_date", request.end_request_date);
                command.Parameters.AddWithValue("@amount_leave_day", request.amount_leave_day);
                command.Parameters.AddWithValue("@start_request_time", request.start_request_time);
                command.Parameters.AddWithValue("@end_request_time", request.end_request_time);
                command.Parameters.AddWithValue("@amount_leave_hour", request.amount_leave_hour);
                command.Parameters.AddWithValue("@path_file", request.path_file);
                command.Parameters.AddWithValue("@request_date", request.request_date);
                command.Parameters.AddWithValue("@manager_approver", request.manager_approver);
                command.Parameters.AddWithValue("@manager_approve_date", request.manager_approve_date);
                command.Parameters.AddWithValue("@manager_approver_status", request.manager_approver_status);
                command.Parameters.AddWithValue("@director_approver", request.director_approver);
                command.Parameters.AddWithValue("@director_approve_date", request.director_approve_date);
                command.Parameters.AddWithValue("@director_approver_status", request.director_approver_status);
                command.Parameters.AddWithValue("@admin_approver", request.admin_approver);
                command.Parameters.AddWithValue("@admin_approve_date", request.admin_approve_date);
                command.Parameters.AddWithValue("@admin_approver_status", request.admin_approver_status);
                command.Parameters.AddWithValue("@decsription", request.decsription);
                command.Parameters.AddWithValue("@status_request", request.status_request);
                command.Parameters.AddWithValue("@is_two_step_approve", request.is_two_step_approve);
                command.ExecuteNonQuery();
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

        public List<RequestModel> GetRequestByEmpID(string emp_id)
        {
            List<RequestModel> requests = new List<RequestModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [request_id]
                                                          ,[emp_id]
                                                          ,[request].[leave_type_id]
														  ,Leave_type.leave_type_code
                                                          ,[is_full_day]
                                                          ,[start_request_date]
                                                          ,[end_request_date]
                                                          ,[amount_leave_day]
                                                          ,[start_request_time]
                                                          ,[end_request_time]
                                                          ,[amount_leave_hour]
                                                          ,[path_file]
                                                          ,[request_date]
                                                          ,[manager_approver_status]
                                                          ,[manager_approver]
                                                          ,[manager_approve_date]
                                                          ,[director_approver_status]
                                                          ,[director_approver]
                                                          ,[director_approve_date]
                                                          ,[admin_approver_status]
                                                          ,[admin_approver]
                                                          ,[admin_approve_date]
                                                          ,[decsription]
                                                          ,[status_request]
														  ,Leave_type.leave_name_th
														  ,Leave_type.leave_name_en
                                                          ,Leave_type.color_code
                                                          ,[request].[is_two_step_approve]
                                                      FROM [dbo].[request] 
													  LEFT JOIN Leave_type ON [request].leave_type_id = Leave_type.leave_type_id
                                                      WHERE [emp_id] = @emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@emp_id", emp_id);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RequestModel request = new RequestModel()
                        {
                            request_id = dr["request_id"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            leave_type_id = dr["leave_type_id"].ToString(),
                            leave_type_code = dr["leave_type_code"].ToString(),
                            leave_name_th = dr["leave_name_th"].ToString(),
                            leave_name_en = dr["leave_name_en"].ToString(),
                            is_full_day = dr["is_full_day"] != DBNull.Value ? Convert.ToBoolean(dr["is_full_day"].ToString()) : false,
                            start_request_date = dr["start_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_request_date"].ToString()) : DateTime.MinValue,
                            end_request_date = dr["end_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["end_request_date"].ToString()) : DateTime.MinValue,
                            amount_leave_day = dr["amount_leave_day"] != DBNull.Value ? Convert.ToInt32(dr["amount_leave_day"].ToString()) : 0,
                            start_request_time = dr["start_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["start_request_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            end_request_time = dr["end_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["end_request_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            amount_leave_hour = dr["amount_leave_hour"] != DBNull.Value ? Convert.ToDecimal(dr["amount_leave_hour"].ToString()) : 1,
                            path_file = dr["path_file"].ToString(),
                            request_date = dr["request_date"] != DBNull.Value ? Convert.ToDateTime(dr["request_date"].ToString()) : DateTime.MinValue,
                            manager_approver = dr["manager_approver"].ToString(),
                            manager_approve_date = dr["manager_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["manager_approve_date"].ToString()) : DateTime.MinValue,
                            manager_approver_status = dr["manager_approver_status"].ToString(),
                            director_approver = dr["director_approver"].ToString(),
                            director_approve_date = dr["director_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["director_approve_date"].ToString()) : DateTime.MinValue,
                            director_approver_status = dr["director_approver_status"].ToString(),
                            admin_approver = dr["admin_approver"].ToString(),
                            admin_approve_date = dr["admin_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["admin_approve_date"].ToString()) : DateTime.MinValue,
                            admin_approver_status = dr["admin_approver_status"].ToString(),
                            decsription = dr["decsription"].ToString(),
                            status_request = dr["status_request"].ToString(),
                            is_two_step_approve = dr["is_two_step_approve"] != DBNull.Value ? Convert.ToBoolean(dr["is_two_step_approve"].ToString()) : false,
                            color_code = dr["color_code"].ToString()
                        };
                        requests.Add(request);
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
            return requests;
        }

        public List<RequestModel> GetRequestByDate(string date)
        {
            List<RequestModel> requests = new List<RequestModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [request_id]
                                                          ,[emp_id]
                                                          ,[request].[leave_type_id]
														  ,Leave_type.leave_type_code
                                                          ,[is_full_day]
                                                          ,[start_request_date]
                                                          ,[end_request_date]
                                                          ,[amount_leave_day]
                                                          ,[start_request_time]
                                                          ,[end_request_time]
                                                          ,[amount_leave_hour]
                                                          ,[path_file]
                                                          ,[request_date]
                                                          ,[manager_approver_status]
                                                          ,[manager_approver]
                                                          ,[manager_approve_date]
                                                          ,[director_approver_status]
                                                          ,[director_approver]
                                                          ,[director_approve_date]
                                                          ,[admin_approver_status]
                                                          ,[admin_approver]
                                                          ,[admin_approve_date]
                                                          ,[decsription]
                                                          ,[status_request]
														  ,Leave_type.leave_name_th
														  ,Leave_type.leave_name_en
                                                          ,Leave_type.color_code
                                                          ,[request].[is_two_step_approve]
                                                      FROM [dbo].[request] 
													  LEFT JOIN Leave_type ON [request].leave_type_id = Leave_type.leave_type_id
                                                      WHERE [start_request_date] = @start_request_date");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@start_request_date", date);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RequestModel request = new RequestModel()
                        {
                            request_id = dr["request_id"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            leave_type_id = dr["leave_type_id"].ToString(),
                            leave_type_code = dr["leave_type_code"].ToString(),
                            leave_name_th = dr["leave_name_th"].ToString(),
                            leave_name_en = dr["leave_name_en"].ToString(),
                            is_full_day = dr["is_full_day"] != DBNull.Value ? Convert.ToBoolean(dr["is_full_day"].ToString()) : false,
                            start_request_date = dr["start_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_request_date"].ToString()) : DateTime.MinValue,
                            end_request_date = dr["end_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["end_request_date"].ToString()) : DateTime.MinValue,
                            amount_leave_day = dr["amount_leave_day"] != DBNull.Value ? Convert.ToInt32(dr["amount_leave_day"].ToString()) : 0,
                            start_request_time = dr["start_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["start_request_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            end_request_time = dr["end_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["end_request_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            amount_leave_hour = dr["amount_leave_hour"] != DBNull.Value ? Convert.ToDecimal(dr["amount_leave_hour"].ToString()) : 1,
                            path_file = dr["path_file"].ToString(),
                            request_date = dr["request_date"] != DBNull.Value ? Convert.ToDateTime(dr["request_date"].ToString()) : DateTime.MinValue,
                            manager_approver = dr["manager_approver"].ToString(),
                            manager_approve_date = dr["manager_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["manager_approve_date"].ToString()) : DateTime.MinValue,
                            manager_approver_status = dr["manager_approver_status"].ToString(),
                            director_approver = dr["director_approver"].ToString(),
                            director_approve_date = dr["director_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["director_approve_date"].ToString()) : DateTime.MinValue,
                            director_approver_status = dr["director_approver_status"].ToString(),
                            admin_approver = dr["admin_approver"].ToString(),
                            admin_approve_date = dr["admin_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["admin_approve_date"].ToString()) : DateTime.MinValue,
                            admin_approver_status = dr["admin_approver_status"].ToString(),
                            decsription = dr["decsription"].ToString(),
                            status_request = dr["status_request"].ToString(),
                            is_two_step_approve = dr["is_two_step_approve"] != DBNull.Value ? Convert.ToBoolean(dr["is_two_step_approve"].ToString()) : false,
                            color_code = dr["color_code"].ToString()
                        };
                        requests.Add(request);
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
            return requests;
        }

        public List<RequestModel> GetRequestByMonth(string month)
        {
            List<RequestModel> requests = new List<RequestModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [request_id]
                                                          ,[emp_id]
                                                          ,[request].[leave_type_id]
														  ,Leave_type.leave_type_code
                                                          ,[is_full_day]
                                                          ,[start_request_date]
                                                          ,[end_request_date]
                                                          ,[amount_leave_day]
                                                          ,[start_request_time]
                                                          ,[end_request_time]
                                                          ,[amount_leave_hour]
                                                          ,[path_file]
                                                          ,[request_date]
                                                          ,[manager_approver_status]
                                                          ,[manager_approver]
                                                          ,[manager_approve_date]
                                                          ,[director_approver_status]
                                                          ,[director_approver]
                                                          ,[director_approve_date]
                                                          ,[admin_approver_status]
                                                          ,[admin_approver]
                                                          ,[admin_approve_date]
                                                          ,[decsription]
                                                          ,[status_request]
														  ,Leave_type.leave_name_th
														  ,Leave_type.leave_name_en
                                                          ,Leave_type.color_code
                                                          ,[request].[is_two_step_approve]
                                                      FROM [dbo].[request] 
													  LEFT JOIN Leave_type ON [request].leave_type_id = Leave_type.leave_type_id
                                                      WHERE [start_request_date] LIKE '{month}%'");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RequestModel request = new RequestModel()
                        {
                            request_id = dr["request_id"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            leave_type_id = dr["leave_type_id"].ToString(),
                            leave_type_code = dr["leave_type_code"].ToString(),
                            leave_name_th = dr["leave_name_th"].ToString(),
                            leave_name_en = dr["leave_name_en"].ToString(),
                            is_full_day = dr["is_full_day"] != DBNull.Value ? Convert.ToBoolean(dr["is_full_day"].ToString()) : false,
                            start_request_date = dr["start_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_request_date"].ToString()) : DateTime.MinValue,
                            end_request_date = dr["end_request_date"] != DBNull.Value ? Convert.ToDateTime(dr["end_request_date"].ToString()) : DateTime.MinValue,
                            amount_leave_day = dr["amount_leave_day"] != DBNull.Value ? Convert.ToInt32(dr["amount_leave_day"].ToString()) : 0,
                            start_request_time = dr["start_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["start_request_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            end_request_time = dr["end_request_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["end_request_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            amount_leave_hour = dr["amount_leave_hour"] != DBNull.Value ? Convert.ToDecimal(dr["amount_leave_hour"].ToString()) : 1,
                            path_file = dr["path_file"].ToString(),
                            request_date = dr["request_date"] != DBNull.Value ? Convert.ToDateTime(dr["request_date"].ToString()) : DateTime.MinValue,
                            manager_approver = dr["manager_approver"].ToString(),
                            manager_approve_date = dr["manager_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["manager_approve_date"].ToString()) : DateTime.MinValue,
                            manager_approver_status = dr["manager_approver_status"].ToString(),
                            director_approver = dr["director_approver"].ToString(),
                            director_approve_date = dr["director_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["director_approve_date"].ToString()) : DateTime.MinValue,
                            director_approver_status = dr["director_approver_status"].ToString(),
                            admin_approver = dr["admin_approver"].ToString(),
                            admin_approve_date = dr["admin_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["admin_approve_date"].ToString()) : DateTime.MinValue,
                            admin_approver_status = dr["admin_approver_status"].ToString(),
                            decsription = dr["decsription"].ToString(),
                            status_request = dr["status_request"].ToString(),
                            is_two_step_approve = dr["is_two_step_approve"] != DBNull.Value ? Convert.ToBoolean(dr["is_two_step_approve"].ToString()) : false,
                            color_code = dr["color_code"].ToString()
                        };
                        requests.Add(request);
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
            return requests;
        }
    }
}
