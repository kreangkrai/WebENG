using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebENG.CTLInterfaces;
using WebENG.CTLModels;
using WebENG.Service;

namespace WebENG.CTLServices
{
    public class EmployeeService : IEmployee
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public EmployeeService()
        {
            connect = new ConnectSQL();
            con = connect.OpenCTLConnect();
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
                                                        name,
                                                        department,
                                                        location,
                                                        start_date,
                                                        position,
                                                        gender,
                                                        phone,
                                                        email,
                                                        [group],
                                                        active,
                                                        team,
                                                        group_color
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
                            name = dr["name"].ToString(),
                            department = dr[""].ToString(),
                            location = dr["location"].ToString(),
                            start_date = dr["start_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_date"].ToString()) : DateTime.MinValue,
                            position = dr["position"].ToString(),
                            gender = dr["gender"].ToString(),
                            phone = dr["phone"].ToString(),
                            email = dr["email"].ToString(),
                            group = dr["group"].ToString(),
                            active = dr["active"] != DBNull.Value ? Convert.ToBoolean(dr["active"].ToString()) : false,
                            team = dr["team"].ToString(),
                            group_color = dr["group_color"].ToString()
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
    }
}
