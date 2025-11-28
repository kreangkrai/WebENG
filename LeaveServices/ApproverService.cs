using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public string Delete(string department)
        {
            return Delete(department, null);
        }

        public string Delete(string department, SqlTransaction tran)
        {
            SqlConnection localCon = tran?.Connection ?? con;
            bool shouldClose = tran == null && localCon.State == ConnectionState.Closed;

            try
            {
                if (localCon.State == ConnectionState.Closed)
                    localCon.Open();

                string sql = "DELETE FROM [dbo].[Approvers] WHERE [department] = @department";
                using (SqlCommand cmd = new SqlCommand(sql, localCon, tran))
                {
                    cmd.Parameters.AddWithValue("@department", department);
                    cmd.ExecuteNonQuery();
                }
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception("Delete Approver failed: " + ex.Message, ex);
            }
            finally
            {
                if (shouldClose && localCon.State == ConnectionState.Open)
                    localCon.Close();
            }
        }

        public string Inserts(List<ApproverModel> approvers)
        {
            return Inserts(approvers, null);
        }

        public string Inserts(List<ApproverModel> approvers, SqlTransaction tran)
        {
            if (approvers == null || !approvers.Any()) return "Success";

            SqlConnection localCon = tran?.Connection ?? con;
            bool shouldClose = tran == null && localCon.State == ConnectionState.Closed;

            try
            {
                if (localCon.State == ConnectionState.Closed)
                    localCon.Open();

                string sql = @"INSERT INTO [dbo].[Approvers]
                               ([emp_id], [department], [level], [is_active])
                               VALUES
                               (@emp_id, @department, @level, @is_active)";

                using (SqlCommand cmd = new SqlCommand(sql, localCon, tran))
                {
                    var pEmpId = cmd.Parameters.Add("@emp_id", SqlDbType.NVarChar);
                    var pDept = cmd.Parameters.Add("@department", SqlDbType.NVarChar);
                    var pLevel = cmd.Parameters.Add("@level", SqlDbType.Int);
                    var pActive = cmd.Parameters.Add("@is_active", SqlDbType.Bit);

                    foreach (var a in approvers)
                    {
                        pEmpId.Value = a.emp_id ?? (object)DBNull.Value;
                        pDept.Value = a.department ?? (object)DBNull.Value;
                        pLevel.Value = a.level;
                        pActive.Value = a.is_active;

                        cmd.ExecuteNonQuery();
                    }
                }
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception("Insert Approver failed: " + ex.Message, ex);
            }
            finally
            {
                if (shouldClose && localCon.State == ConnectionState.Open)
                    localCon.Close();
            }
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
                                              ,[Approvers].[emp_id]
	                                          ,emp.name_en as emp_name
                                              ,[Approvers].[department]
                                              ,[level]
                                              ,[is_active]
                                          FROM [dbo].[Approvers]
                                          LEFT JOIN [CTL].dbo.[Employees] emp ON [Approvers].emp_id = emp.emp_id");
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
                            emp_id = dr["emp_id"].ToString(),
                            emp_name = dr["emp_name"].ToString(),
                            level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 0,
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
    }
}