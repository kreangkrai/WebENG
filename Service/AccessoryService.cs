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
        public List<UserModel> getAllUser()
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT name,department,role,user_id FROM Authen ORDER BY name", ConnectSQL.OpenConnect());
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UserModel u = new UserModel()
                        {
                            user_id = dr["user_id"].ToString(),
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
                if (ConnectSQL.con.State == ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return users;
        }

        public List<UserModel> getWorkingUser()
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT DISTINCT Authen.user_id,
				                                                    Authen.name,
				                                                    Authen.department,
				                                                    Authen.role 
                                                    FROM WorkingHours 
                                                    LEFT JOIN Authen ON Authen.user_id = WorkingHours.user_id 
                                                    ORDER BY Authen.name", ConnectSQL.OpenConnect());
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UserModel u = new UserModel()
                        {
                            user_id = dr["user_id"].ToString(),
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
                if (ConnectSQL.con.State == ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return users;
        }
    }
}
