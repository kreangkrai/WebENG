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
                                                  ,[department]
                                                  ,[department_name]
                                                  ,[approver_level]
                                                  ,[manager_id]
                                                  ,[is_active]
                                              FROM [dbo].[departments]");
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

        public string Insert(DepartmentModel department)
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
                command.Parameters.AddWithValue("@department", department.department);
                command.Parameters.AddWithValue("@department_name", department.department_name);
                command.Parameters.AddWithValue("@approver_level", department.approver_level);
                command.Parameters.AddWithValue("@manager_id", department.manager_id);
                command.Parameters.AddWithValue("@is_active", department.is_active);
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

        public string Update(DepartmentModel department)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"UPDATE [dbo].[departments]
                                                   SET [department_name] = @department_name
                                                      ,[approver_level] = @approver_level
                                                      ,[manager_id] = @manager_id
                                                      ,[is_active] = @is_active
                                                 WHERE [department] = @department");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@department", department.department);
                command.Parameters.AddWithValue("@department_name", department.department_name);
                command.Parameters.AddWithValue("@approver_level", department.approver_level);
                command.Parameters.AddWithValue("@manager_id", department.manager_id);
                command.Parameters.AddWithValue("@is_active", department.is_active);
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
