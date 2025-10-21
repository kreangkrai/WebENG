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
                                                          ,[leave_type_id]
                                                          ,[is_full_day]
                                                          ,[start_date]
                                                          ,[end_date]
                                                          ,[amount_leave_day]
                                                          ,[start_time]
                                                          ,[end_time]
                                                          ,[amount_leave_hour]
                                                          ,[path_file]
                                                          ,[request_date]
                                                          ,[manager_approver_status]
                                                          ,[manager_approver]
                                                          ,[manager_approve_date]
                                                          ,[admin_approver_status]
                                                          ,[admin_approver]
                                                          ,[admin_approve_date]
                                                          ,[decsription]
                                                          ,[status_request]
                                                      FROM [dbo].[request] WHERE [request_id] <> NULL");
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
                            is_full_day = dr["is_full_day"] != DBNull.Value ? Convert.ToBoolean(dr["is_full_day"].ToString()) : false,
                            start_date = dr["start_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_date"].ToString()) : DateTime.MinValue,
                            end_date = dr["end_date"] != DBNull.Value ? Convert.ToDateTime(dr["end_date"].ToString()) : DateTime.MinValue,
                            amount_leave_day = dr["amount_leave_day"] != DBNull.Value ? Convert.ToInt32(dr["amount_leave_day"].ToString()) : 0,
                            start_time = dr["start_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["start_time"].ToString()).Ticks) : new TimeSpan(0,0,0),
                            end_time = dr["end_time"] != DBNull.Value ? new TimeSpan(Convert.ToDateTime(dr["end_time"].ToString()).Ticks) : new TimeSpan(0, 0, 0),
                            amount_leave_hour = dr["amount_leave_hour"] != DBNull.Value ? Convert.ToDecimal(dr["amount_leave_hour"].ToString()) : 1,
                            path_file = dr["path_file"].ToString(),
                            request_date = dr["request_date"] != DBNull.Value ? Convert.ToDateTime(dr["request_date"].ToString()) : DateTime.MinValue,
                            manager_approver = dr["manager_approver"].ToString(),
                            manager_approve_date = dr["manager_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["manager_approve_date"].ToString()) : DateTime.MinValue,
                            manager_approver_status = dr["manager_approver_status"].ToString(),
                            admin_approver = dr["admin_approver"].ToString(),
                            admin_approve_date = dr["admin_approve_date"] != DBNull.Value ? Convert.ToDateTime(dr["admin_approve_date"].ToString()) : DateTime.MinValue,
                            admin_approver_status = dr["admin_approver_status"].ToString(),
                            decsription = dr["decsription"].ToString(),
                            status_request = dr["status_request"].ToString()
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
                                                   ,[start_date]
                                                   ,[end_date]
                                                   ,[amount_leave_day]
                                                   ,[start_time]
                                                   ,[end_time]
                                                   ,[amount_leave_hour]
                                                   ,[path_file]
                                                   ,[request_date]
                                                   ,[manager_approver_status]
                                                   ,[manager_approver]
                                                   ,[manager_approve_date]
                                                   ,[admin_approver_status]
                                                   ,[admin_approver]
                                                   ,[admin_approve_date]
                                                   ,[decsription]
                                                   ,[status_request])
                                             VALUES
                                                   (@request_id
                                                   ,@emp_id
                                                   ,@leave_type_id
                                                   ,@is_full_day
                                                   ,@start_date
                                                   ,@end_date
                                                   ,@amount_leave_day
                                                   ,@start_time
                                                   ,@end_time
                                                   ,@amount_leave_hour
                                                   ,@path_file
                                                   ,@request_date
                                                   ,@manager_approver_status
                                                   ,@manager_approver
                                                   ,@manager_approve_date
                                                   ,@admin_approver_status
                                                   ,@admin_approver
                                                   ,@admin_approve_date
                                                   ,@decsription
                                                   ,@status_request)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_id", request.request_id);
                command.Parameters.AddWithValue("@emp_id", request.emp_id);
                command.Parameters.AddWithValue("@leave_type_id", request.leave_type_id);
                command.Parameters.AddWithValue("@is_full_day", request.is_full_day);
                command.Parameters.AddWithValue("@start_date", request.start_date);
                command.Parameters.AddWithValue("@end_date", request.end_date);
                command.Parameters.AddWithValue("@amount_leave_day", request.amount_leave_day);
                command.Parameters.AddWithValue("@start_time", request.start_time);
                command.Parameters.AddWithValue("@end_time", request.end_time);
                command.Parameters.AddWithValue("@amount_leave_hour", request.amount_leave_hour);
                command.Parameters.AddWithValue("@path_file", request.path_file);
                command.Parameters.AddWithValue("@request_date", request.request_date);
                command.Parameters.AddWithValue("@manager_approver", request.manager_approver);
                command.Parameters.AddWithValue("@manager_approve_date", request.manager_approve_date);
                command.Parameters.AddWithValue("@manager_approver_status", request.manager_approver_status);
                command.Parameters.AddWithValue("@admin_approver", request.admin_approver);
                command.Parameters.AddWithValue("@admin_approve_date", request.admin_approve_date);
                command.Parameters.AddWithValue("@admin_approver_status", request.admin_approver_status);
                command.Parameters.AddWithValue("@decsription", request.decsription);
                command.Parameters.AddWithValue("@status_request", request.status_request);
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
                                                      ,[start_date] = @start_date
                                                      ,[end_date] = @end_date
                                                      ,[amount_leave_day] = @amount_leave_day
                                                      ,[start_time] = @start_time
                                                      ,[end_time] = @end_time
                                                      ,[amount_leave_hour] = @amount_leave_hour
                                                      ,[path_file] = @path_file
                                                      ,[request_date] = @request_date
                                                      ,[manager_approver_status] = @manager_approver_status
                                                      ,[manager_approver] = @manager_approver
                                                      ,[manager_approve_date] = @manager_approve_date
                                                      ,[admin_approver_status] = @admin_approver_status
                                                      ,[admin_approver] = @admin_approver
                                                      ,[admin_approve_date] = @admin_approve_date
                                                      ,[decsription] = @decsription
                                                      ,[status_request] = @status_request
                                                 WHERE [request_id] = @request_id AND  [emp_id] = @emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_id", request.request_id);
                command.Parameters.AddWithValue("@emp_id", request.emp_id);
                command.Parameters.AddWithValue("@leave_type_id", request.leave_type_id);
                command.Parameters.AddWithValue("@is_full_day", request.is_full_day);
                command.Parameters.AddWithValue("@start_date", request.start_date);
                command.Parameters.AddWithValue("@end_date", request.end_date);
                command.Parameters.AddWithValue("@amount_leave_day", request.amount_leave_day);
                command.Parameters.AddWithValue("@start_time", request.start_time);
                command.Parameters.AddWithValue("@end_time", request.end_time);
                command.Parameters.AddWithValue("@amount_leave_hour", request.amount_leave_hour);
                command.Parameters.AddWithValue("@path_file", request.path_file);
                command.Parameters.AddWithValue("@request_date", request.request_date);
                command.Parameters.AddWithValue("@manager_approver", request.manager_approver);
                command.Parameters.AddWithValue("@manager_approve_date", request.manager_approve_date);
                command.Parameters.AddWithValue("@manager_approver_status", request.manager_approver_status);
                command.Parameters.AddWithValue("@admin_approver", request.admin_approver);
                command.Parameters.AddWithValue("@admin_approve_date", request.admin_approve_date);
                command.Parameters.AddWithValue("@admin_approver_status", request.admin_approver_status);
                command.Parameters.AddWithValue("@decsription", request.decsription);
                command.Parameters.AddWithValue("@status_request", request.status_request);
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
    }
}
