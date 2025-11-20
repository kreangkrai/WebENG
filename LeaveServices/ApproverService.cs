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
    public class ApproverService : IApprover
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public ApproverService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<ApproverModel> GetApprovers()
        {
            List<ApproverModel> approvers = new List<ApproverModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [id]
                                              ,[approver_id]
	                                          ,emp.name_en as approver_name
                                              ,[Approvers].[department]
                                              ,[approver_level]
                                              ,[is_active]
                                          FROM [dbo].[Approvers]
                                          LEFT JOIN [CTL].dbo.[Employees] emp ON [Approvers].approver_id = emp.emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ApproverModel approver = new ApproverModel()
                        {
                            id = Int32.Parse(dr["id"].ToString()),
                            department = dr["department"].ToString(),
                            approver_id = dr["approver_id"].ToString(),
                            approver_name = dr["approver_name"].ToString(),
                            approver_level = dr["approver_level"] != DBNull.Value ? Convert.ToInt32(dr["approver_level"].ToString()) : 0,
                            is_active = dr["is_active"] != DBNull.Value ? Convert.ToBoolean(dr["is_active"].ToString()) : false
                        };
                        approvers.Add(approver);
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
            return approvers;
        }

        public string Inserts(List<ApproverModel> approvers)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [dbo].[Approvers]
                                                                   ([approver_id]
                                                                   ,[department]
                                                                   ,[approver_level]
                                                                   ,[is_active])
                                                             VALUES
                                                                   (@approver_id
                                                                   ,@department
                                                                   ,@approver_level
                                                                   ,@is_active)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.Add("@approver_id", SqlDbType.Text);
                command.Parameters.Add("@department", SqlDbType.Text);
                command.Parameters.Add("@approver_level", SqlDbType.Int);
                command.Parameters.Add("@is_active", SqlDbType.Bit);
                for (int i = 0; i < approvers.Count; i++)
                {
                    command.Parameters[0].Value = approvers[i].approver_id;
                    command.Parameters[1].Value = approvers[i].department;
                    command.Parameters[2].Value = approvers[i].approver_level;
                    command.Parameters[3].Value = approvers[i].is_active;
                    command.ExecuteNonQuery();
                }
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

        public string Delete(string department)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"DELETE FROM [dbo].[Approvers]
                                                 WHERE [department] = @department");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@department", department);
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
