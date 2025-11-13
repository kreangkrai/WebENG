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
                string strCmd = string.Format($@"SELECT 
                                                department_id,
                                                department_name
                                                FROM departments");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DepartmentModel department = new DepartmentModel()
                        {
                            department_id = dr["department_id"].ToString(),
                            department_name = dr["department_name"].ToString()
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
                string strCmd = string.Format($@"INSERT INTO departments(department_id,
                                                    department_name)
                                                VALUES (@department_id,
                                                    @department_name)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@department_id", department.department_id);
                command.Parameters.AddWithValue("@department_name", department.department_name);
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
                string strCmd = string.Format($@"UPDATE departments SET
                                                    department_name = @department_name
                                                WHERE department_id = @department_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@department_id", department.department_id);
                command.Parameters.AddWithValue("@department_name", department.department_name);
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
