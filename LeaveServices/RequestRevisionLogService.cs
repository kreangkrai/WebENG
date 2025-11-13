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
    public class RequestRevisionLogService : IRequestRevisionLog
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public RequestRevisionLogService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<RequestRevisionLogModel> GetRevisionLogByEmpID(string emp_id)
        {
            List<RequestRevisionLogModel> requests = new List<RequestRevisionLogModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [log_id]
                                                      ,[request_id]
                                                      ,[request_emp_id]
                                                      ,[revised_by]
                                                      ,[revised_date]                                                     
                                                      ,[revision_level
                                                      ,[revision_type]
                                                      ,[comment]
                                                  FROM [ELEAVE].[dbo].[request_revision_log] WHERE [request_emp_id] = @request_emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_emp_id", emp_id);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RequestRevisionLogModel request = new RequestRevisionLogModel()
                        {
                            request_id = dr["request_id"].ToString(),
                            request_emp_id = dr["emp_id"].ToString(),
                            revised_by = dr["revised_by"].ToString(),
                            revised_date = dr["revised_date"] != DBNull.Value ? Convert.ToDateTime(dr["revised_date"].ToString()) : DateTime.MinValue,
                            revision_level = dr["revision_level"].ToString(),
                            revision_type = dr["revision_type"].ToString(),
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

        public string Insert(RequestRevisionLogModel request)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [ELEAVE].[dbo].[request_revision_log]
                                                           ([request_id]
                                                           ,[request_emp_id]
                                                           ,[revised_by]
                                                           ,[revised_date]                                                           
                                                           ,[revision_level]
                                                           ,[revision_type]
                                                           ,[comment])
                                                     VALUES
                                                           (@request_id
                                                           ,@request_emp_id
                                                           ,@revised_by
                                                           ,@revised_date
                                                           ,@revision_level
                                                           ,@revision_type
                                                           ,@comment");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@request_id", request.request_id);
                command.Parameters.AddWithValue("@request_emp_id", request.request_emp_id);
                command.Parameters.AddWithValue("@revised_by", request.revised_by);
                command.Parameters.AddWithValue("@revised_date", request.revised_date);
                command.Parameters.AddWithValue("@revision_level", request.revision_level);
                command.Parameters.AddWithValue("@revision_type", request.revision_type);
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
