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
    public class CheckerService : IChecker
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public CheckerService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<CheckerModel> GetCheckers()
        {
            List<CheckerModel> checkers = new List<CheckerModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [id]
                                                  ,[Checkers].[emp_id]
                                                  ,[level]
	                                              ,emp.name_en as emp_name
                                                  ,[is_active]
                                              FROM [dbo].[Checkers]
                                              LEFT JOIN [CTL].dbo.[Employees] emp ON [Checkers].emp_id = emp.emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        CheckerModel checker = new CheckerModel()
                        {
                            id = Int32.Parse(dr["id"].ToString()),
                            emp_id = dr["emp_id"].ToString(),
                            emp_name = dr["emp_name"].ToString(),
                            level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 0,
                            is_active = dr["is_active"] != DBNull.Value ? Convert.ToBoolean(dr["is_active"].ToString()) : false
                        };
                        checkers.Add(checker);
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
            return checkers;
        }

        public string Inserts(List<CheckerModel> checkers)
        {
            return Inserts(checkers, null);
        }
        public string Inserts(List<CheckerModel> checkers, SqlTransaction tran)
        {
            if (checkers == null || !checkers.Any()) return "Success";

            SqlConnection localCon = tran?.Connection ?? con;
            bool shouldClose = tran == null && localCon.State == ConnectionState.Closed;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string sql = string.Format($@"INSERT INTO [dbo].[Checkers]
                                                   ([emp_id]
                                                   ,[level]
                                                   ,[is_active])
                                             VALUES
                                                   (@emp_id
                                                   ,@level
                                                   ,@is_active)");

                using (SqlCommand command = new SqlCommand(sql, localCon, tran))
                {
                    var emp_id = command.Parameters.Add("@emp_id", SqlDbType.Text);
                    var level = command.Parameters.Add("@level", SqlDbType.Int);
                    var is_active = command.Parameters.Add("@is_active", SqlDbType.Bit);
                    foreach(var checker in checkers)
                    {
                        emp_id.Value = checker.emp_id ?? (object)DBNull.Value;
                        level.Value = checker.level;
                        is_active.Value = checker.is_active;
                        command.ExecuteNonQuery();
                    }
                }
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception("Insert failed: " + ex.Message, ex);
            }
            finally
            {
                if (shouldClose && localCon.State == ConnectionState.Open)
                    localCon.Close();
            }
        }

        public string Delete()
        {
            return Delete(null);
        }
        public string Delete(SqlTransaction tran)
        {
            SqlConnection localCon = tran?.Connection ?? con;
            bool shouldClose = tran == null && localCon.State == ConnectionState.Closed;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string sql = string.Format($@"DELETE FROM [dbo].[Checkers]");
                using (SqlCommand command = new SqlCommand(sql, localCon, tran))
                {
                    command.ExecuteNonQuery();
                }
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception("Delete failed: " + ex.Message, ex);
            }
            finally
            {
                if (shouldClose && localCon.State == ConnectionState.Open)
                    localCon.Close();
            }
        }
    }
}
