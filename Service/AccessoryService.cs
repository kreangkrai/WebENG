using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;

namespace WebENG.Service
{
    public class AccessoryService : IAccessory
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public AccessoryService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<UserModel> getAllUser()
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand(@"SELECT [MES].dbo.Authen.name,
                                                    [MES].dbo.Authen.department,
                                                    role,
                                                    emp.emp_id
                                                    FROM [MES].dbo.Authen
                                                    LEFT JOIN [CTL].dbo.[Employees] emp ON [MES].dbo.Authen.name = emp.name_en
                                                    ORDER BY [MES].dbo.Authen.name", con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UserModel u = new UserModel()
                        {
                            emp_id = dr["emp_id"].ToString(),
                            name = dr["name"].ToString().ToLower(),
                            department = dr["department"].ToString(),
                            role = dr["role"].ToString()
                        };
                        users.Add(u);
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
            return users;
        }

        public List<UserModel> getWorkingUser()
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand(@"SELECT DISTINCT emp.emp_id,
				                                                    emp.name_en as name,
				                                                    emp.department
                                                    FROM WorkingHours 
                                                    LEFT JOIN CTL.dbo.Employees emp ON WorkingHours.emp_id  = emp.emp_id
                                                    ORDER BY emp.name_en", con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr["emp_id"] != DBNull.Value && dr["emp_id"].ToString() != "")
                        {
                            UserModel u = new UserModel()
                            {
                                emp_id = dr["emp_id"].ToString(),
                                name = dr["name"].ToString().ToLower(),
                                department = dr["department"].ToString(),
                            };
                            users.Add(u);
                        }
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
            return users;
        }

        public List<UserModel> getWorkingUser(DateTime start, DateTime stop)
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand($@"SELECT DISTINCT emp.emp_id,
				                                                    emp.name_en as name,
				                                                    emp.department
                                                    FROM WorkingHours 
                                                    LEFT JOIN CTL.dbo.Employees emp ON WorkingHours.emp_id  = emp.emp_id                                                   
                                                    Where working_date between '{start.ToString("yyyy-MM-dd")}' AND '{stop.ToString("yyyy-MM-dd")}'
                                                    ORDER BY emp.name_en", con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr["emp_id"] != DBNull.Value && dr["emp_id"].ToString() != "")
                        {
                            UserModel u = new UserModel()
                            {
                                emp_id = dr["emp_id"].ToString(),
                                name = dr["name"].ToString().ToLower(),
                                department = dr["department"].ToString(),
                            };
                            users.Add(u);
                        }
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
            return users;
        }
    }
}
