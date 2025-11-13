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
    public class PositionService : IPosition
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public PositionService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public string delete(string emp_id)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"DELETE FROM [dbo].[position]
                                                        WHERE [emp_id] = @emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@emp_id", emp_id);
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

        public List<PositionModel> GetPositions()
        {
            List<PositionModel> positions = new List<PositionModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"
                                                SELECT [position_id]
                                                      ,[position].[emp_id]
	                                                  ,employees.name_en as emp_name
                                                      ,[level]
                                                      ,[department]
                                                      ,[is_active]
                                                  FROM [dbo].[position]
                                                  LEFT JOIN employees ON position.emp_id = employees.emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        PositionModel position = new PositionModel()
                        {
                            position_id = Int32.Parse(dr["position_id"].ToString()),
                            emd_name = dr["emd_name"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            level = dr["level"].ToString(),
                            department = dr["department"].ToString(),
                            is_active = dr["is_active"] != DBNull.Value ? Convert.ToBoolean(dr["is_active"].ToString()) : false,
                        };
                        positions.Add(position);
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
            return positions;
        }

        public string insert(List<PositionModel> positions)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [dbo].[position]
                                                       ([emp_id]
                                                       ,[level]
                                                       ,[department]
                                                       ,[is_active])
                                                 VALUES
                                                       (@emp_id
                                                       ,@level
                                                       ,@department
                                                       ,@is_active)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.Add("@emp_id", SqlDbType.Text);
                command.Parameters.Add("@level", SqlDbType.Text);
                command.Parameters.Add("@department", SqlDbType.Text);
                command.Parameters.Add("@is_active", SqlDbType.Bit);
                for(int i = 0; i < positions.Count; i++)
                {
                    command.Parameters[0].Value = positions[i].emp_id;
                    command.Parameters[1].Value = positions[i].level;
                    command.Parameters[2].Value = positions[i].department;
                    command.Parameters[3].Value = positions[i].is_active;
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
    }
}
