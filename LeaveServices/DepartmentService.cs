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
    public class DepartmentService : IDepartment
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public DepartmentService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<DepartmentModel> GetDepartments()
        {
            List<DepartmentModel> departments = new List<DepartmentModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [id]
                                                  ,[departments].[department]
                                                  ,[department_name]
                                                  ,[approver_level]
                                                  ,[manager_id]
	                                              ,emp.name_en as manager_name
                                                  ,[is_active]
                                              FROM [dbo].[departments]
                                              LEFT JOIN [CTL].dbo.[Employees] emp ON [departments].manager_id = emp.emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DepartmentModel department = new DepartmentModel()
                        {
                            id = Int32.Parse(dr["id"].ToString()),
                            department = dr["department"].ToString(),
                            department_name = dr["department_name"].ToString(),
                            approver_level = dr["approver_level"] != DBNull.Value ? Convert.ToInt32(dr["approver_level"].ToString()) : 0,
                            manager_id = dr["manager_id"].ToString(),
                            manager_name = dr["manager_name"].ToString(),
                            is_active = dr["is_active"] != DBNull.Value ? Convert.ToBoolean(dr["is_active"].ToString()): false
                        };
                        departments.Add(department);
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
            return departments;
        }

        public string Inserts(List<DepartmentModel> departments)
        {

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string strCmd = string.Format($@"INSERT INTO [dbo].[departments]
                                                   ([department]
                                                   ,[department_name]
                                                   ,[approver_level]
                                                   ,[manager_id]
                                                   ,[is_active])
                                             VALUES
                                                   (@department
                                                   ,@department_name
                                                   ,@approver_level
                                                   ,@manager_id
                                                   ,@is_active)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.Add("@department", SqlDbType.Text);
                command.Parameters.Add("@department_name", SqlDbType.Text);
                command.Parameters.Add("@approver_level", SqlDbType.Int);
                command.Parameters.Add("@manager_id", SqlDbType.Text);
                command.Parameters.Add("@is_active", SqlDbType.Bit);
                for (int i = 0; i < departments.Count; i++)
                {
                    command.Parameters[0].Value = departments[i].department;
                    command.Parameters[1].Value = departments[i].department_name;
                    command.Parameters[2].Value = departments[i].approver_level;
                    command.Parameters[3].Value = departments[i].manager_id;
                    command.Parameters[4].Value = departments[i].is_active;
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
                string strCmd = string.Format($@"DELETE FROM [dbo].[departments]
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
