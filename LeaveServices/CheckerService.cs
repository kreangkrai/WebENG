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
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [dbo].[Checkers]
                                                   ([emp_id]
                                                   ,[level]
                                                   ,[is_active])
                                             VALUES
                                                   (@emp_id
                                                   ,@level
                                                   ,@is_active)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.Add("@emp_id", SqlDbType.Text);
                command.Parameters.Add("@level", SqlDbType.Int);
                command.Parameters.Add("@is_active", SqlDbType.Bit);
                for (int i = 0; i < checkers.Count; i++)
                {
                    command.Parameters[0].Value = checkers[i].emp_id;
                    command.Parameters[1].Value = checkers[i].level;
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
