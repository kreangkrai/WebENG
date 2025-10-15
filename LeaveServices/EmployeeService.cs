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
    public class EmployeeService : IEmployee
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public EmployeeService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<EmployeeModel> GetEmployees()
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT emp_id,
                                                    name_th,
                                                    name_en,
                                                    role
                                                    FROM Employees");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EmployeeModel employee = new EmployeeModel()
                        {
                            emp_id = dr["emp_id"].ToString(),
                            name_th = dr["name_th"].ToString(),
                            name_en = dr["name_en"].ToString(),
                            role = dr["role"].ToString()
                        };
                        employees.Add(employee);
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
            return employees;
        }

        public string Insert(EmployeeModel employee)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO Employees(emp_id,
                                                    name_th,
                                                    name_en,
                                                    role)
                                                VALUES (@emp_id,
                                                    @name_th,
                                                    @name_en,
                                                    @role)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@emp_id", employee.emp_id);
                command.Parameters.AddWithValue("@name_th", employee.name_th);
                command.Parameters.AddWithValue("@name_en", employee.name_en);
                command.Parameters.AddWithValue("@role", employee.role);
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

        public string Update(EmployeeModel employee)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"UPDATE Employees SET
                                                    name_th = @name_th,
                                                    name_en = @name_en,
                                                    role = @role
                                                WHERE emp_id = @emp_id)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@emp_id", employee.emp_id);
                command.Parameters.AddWithValue("@name_th", employee.name_th);
                command.Parameters.AddWithValue("@name_en", employee.name_en);
                command.Parameters.AddWithValue("@role", employee.role);
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
