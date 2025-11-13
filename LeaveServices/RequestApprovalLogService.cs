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
    public class RequestApprovalLogService : IRequestApprovalLog
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public RequestApprovalLogService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<RequestApprovalLogModel> GetApprovalLogByEmpID(string emp_id)
        {
            List<RequestApprovalLogModel> requests = new List<RequestApprovalLogModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [log_id]
                                                  ,[request_id]
	                                              ,[request_emp_id]
                                                  ,[action_by]
                                                  ,[action_date]
                                                  ,[action_type]
                                                  ,[action_level]
                                                  ,[comment]
                                              FROM [ELEAVE].[dbo].[request_approval_log] WHERE [request_emp_id] = @request_emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_emp_id", emp_id);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RequestApprovalLogModel request = new RequestApprovalLogModel()
                        {
                            request_id = dr["request_id"].ToString(),
                            request_emp_id = dr["emp_id"].ToString(),
                            action_by = dr["action_by"].ToString(),
                            action_date = dr["action_date"] != DBNull.Value ? Convert.ToDateTime(dr["action_date"].ToString()) : DateTime.MinValue,
                            action_type = dr["action_type"].ToString(),
                            action_level = dr["action_level"].ToString(),
                            comment = dr["comment"].ToString(),
                            log_id = Int32.Parse(dr["log_id"].ToString())
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

        public string Insert(RequestApprovalLogModel request)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [ELEAVE].[dbo].[request_approval_log]
                                                   ([request_id]
                                                   ,[request_emp_id]
                                                   ,[action_by]
                                                   ,[action_date]
                                                   ,[action_type]
                                                   ,[action_level]
                                                   ,[comment])
                                             VALUES
                                                   (@request_id
                                                   ,@request_emp_id
                                                   ,@action_by
                                                   ,@action_date
                                                   ,@action_type
                                                   ,@action_level
                                                   ,@comment)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_id", request.request_id);
                command.Parameters.AddWithValue("@request_emp_id", request.request_emp_id);
                command.Parameters.AddWithValue("@action_by", request.action_by);
                command.Parameters.AddWithValue("@action_date", request.action_date);
                command.Parameters.AddWithValue("@action_type", request.action_type);
                command.Parameters.AddWithValue("@action_level", request.action_level);
                command.Parameters.AddWithValue("@comment", request.comment);

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
