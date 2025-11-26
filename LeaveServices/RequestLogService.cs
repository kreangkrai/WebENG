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
    public class RequestLogService : IRequestLog
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public RequestLogService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<RequestLogModel> GetLogByEmpID(string emp_id)
        {
            List<RequestLogModel> requests = new List<RequestLogModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [log_id]
                                                      ,[request_id]
                                                      ,[action_by]
                                                      ,[action_by_name]
                                                      ,[action_by_level]
                                                      ,[old_status]
                                                      ,[new_status]
                                                      ,[old_level_step]
                                                      ,[new_level_step]
                                                      ,[comment]
                                                      ,[log_date]
                                                  FROM [dbo].[request_log]
                                                  WHERE [action_by] = @action_by");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@action_by", emp_id);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RequestLogModel request = new RequestLogModel()
                        {
                            request_id = dr["request_id"].ToString(),
                            action_by = dr["action_by"].ToString(),
                            action_by_name = dr["action_by_name"].ToString(),
                            action_by_level = dr["action_by_level"] != DBNull.Value ? Convert.ToInt32(dr["action_by_level"].ToString()) : -1,
                            old_status = dr["old_status"].ToString(),
                            new_status = dr["new_status"].ToString(),
                            old_level_step = dr["old_level_step"] != DBNull.Value ? Convert.ToInt32(dr["old_level_step"].ToString()) : -1,
                            new_level_step = dr["new_level_step"] != DBNull.Value ? Convert.ToInt32(dr["new_level_step"].ToString()) : -1,
                            comment = dr["comment"].ToString(),
                            log_id = Int32.Parse(dr["log_id"].ToString()),
                            log_date = Convert.ToDateTime(dr["log_date"].ToString())
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

        public List<RequestLogModel> GetLogByRequestId(string request_id)
        {
            List<RequestLogModel> requests = new List<RequestLogModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [log_id]
                                                      ,[request_id]
                                                      ,[action_by]
                                                      ,[action_by_name]
                                                      ,[action_by_level]
                                                      ,[old_status]
                                                      ,[new_status]
                                                      ,[old_level_step]
                                                      ,[new_level_step]
                                                      ,[comment]
                                                      ,[log_date]
                                                  FROM [dbo].[request_log]
                                                  WHERE [request_id] = @request_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_id", request_id);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RequestLogModel request = new RequestLogModel()
                        {
                            request_id = dr["request_id"].ToString(),
                            action_by = dr["action_by"].ToString(),
                            action_by_name = dr["action_by_name"].ToString(),
                            action_by_level = dr["action_by_level"] != DBNull.Value ? Convert.ToInt32(dr["action_by_level"].ToString()) : -1,
                            old_status = dr["old_status"].ToString(),
                            new_status = dr["new_status"].ToString(),
                            old_level_step = dr["old_level_step"] != DBNull.Value ? Convert.ToInt32(dr["old_level_step"].ToString()) : -1,
                            new_level_step = dr["new_level_step"] != DBNull.Value ? Convert.ToInt32(dr["new_level_step"].ToString()) : -1,
                            comment = dr["comment"].ToString(),
                            log_id = Int32.Parse(dr["log_id"].ToString()),
                            log_date = Convert.ToDateTime(dr["log_date"].ToString())
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

        public string Insert(RequestLogModel request)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [dbo].[request_log]
                                                       ([request_id]
                                                       ,[action_by]
                                                       ,[action_by_name]
                                                       ,[action_by_level]
                                                       ,[old_status]
                                                       ,[new_status]
                                                       ,[old_level_step]
                                                       ,[new_level_step]
                                                       ,[comment]
                                                       ,[log_date])
                                                 VALUES
                                                       (@request_id
                                                       ,@action_by
                                                       ,@action_by_name
                                                       ,@action_by_level
                                                       ,@old_status
                                                       ,@new_status
                                                       ,@old_level_step
                                                       ,@new_level_step
                                                       ,@comment
                                                       ,@log_date)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_id", request.request_id);
                command.Parameters.AddWithValue("@action_by", request.action_by);
                command.Parameters.AddWithValue("@action_by_name", request.action_by_name);
                command.Parameters.AddWithValue("@action_by_level", request.action_by_level);
                command.Parameters.AddWithValue("@old_status", request.old_status);
                command.Parameters.AddWithValue("@new_status", request.new_status);
                command.Parameters.AddWithValue("@old_level_step", request.old_level_step);
                command.Parameters.AddWithValue("@new_level_step", request.new_level_step);
                command.Parameters.AddWithValue("@log_date", request.log_date);
                command.Parameters.AddWithValue("@comment", request.comment);
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
    }
}
