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
                                                  ,[checker_id]
                                                  ,[checker_level]
	                                              ,emp.name_en as checker_name
                                                  ,[is_active]
                                              FROM [dbo].[Checkers]
                                              LEFT JOIN [CTL].dbo.[Employees] emp ON [Checkers].checker_id = emp.emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        CheckerModel checker = new CheckerModel()
                        {
                            id = Int32.Parse(dr["id"].ToString()),
                            checker_id = dr["checker_id"].ToString(),
                            checker_name = dr["checker_name"].ToString(),
                            checker_level = dr["checker_level"] != DBNull.Value ? Convert.ToInt32(dr["checker_level"].ToString()) : 0,
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
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [dbo].[Checkers]
                                                   ([checker_id]
                                                   ,[checker_level]
                                                   ,[is_active])
                                             VALUES
                                                   (@checker_id
                                                   ,@checker_level
                                                   ,@is_active)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.Add("@checker_id", SqlDbType.Text);
                command.Parameters.Add("@checker_level", SqlDbType.Int);
                command.Parameters.Add("@is_active", SqlDbType.Bit);
                for (int i = 0; i < checkers.Count; i++)
                {
                    command.Parameters[0].Value = checkers[i].checker_id;
                    command.Parameters[1].Value = checkers[i].checker_level;
                    command.Parameters[2].Value = checkers[i].is_active;
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

        public string Delete()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"DELETE FROM [dbo].[Checkers]");
                SqlCommand command = new SqlCommand(strCmd, con);
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
