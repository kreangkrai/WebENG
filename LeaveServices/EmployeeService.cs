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
                string strCmd = string.Format($@"SELECT 
                                                emp1.emp_id,
                                                emp2.name,
                                                emp2.department,
                                                emp2.location,
                                                emp2.position,
                                                emp2.gender,
                                                emp2.email,
                                                emp2.phone,
                                                emp2.start_date,
                                                emp2.active,
                                                emp1.role
                                                FROM ELEAVE.dbo.Employees emp1
                                                LEFT JOIN CTL.dbo.Employees  emp2 ON  emp1.emp_id = emp2.emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EmployeeModel employee = new EmployeeModel()
                        {
                            emp_id = dr["emp_id"].ToString(),
                            name = dr["name"].ToString(),
                            department = dr["department"].ToString(),
                            location = dr["location"].ToString(),
                            position = dr["position"].ToString(),
                            email = dr["email"].ToString(),
                            phone = dr["phone"].ToString(),
                            gender = dr["gender"].ToString(),
                            start_date = dr["start_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_date"].ToString()) : DateTime.MinValue,
                            active = dr["active"] != DBNull.Value ? Convert.ToBoolean(dr["active"].ToString()) : false,
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
                                                    role)
                                                VALUES (@emp_id,
                                                    @role)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@emp_id", employee.emp_id);
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
                                                    role = @role
                                                WHERE emp_id = @emp_id)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@emp_id", employee.emp_id);
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
