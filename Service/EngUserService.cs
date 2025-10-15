using WebENG.Interface;
using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Service;
using System.Data;

namespace WebENG.Service
{
    public class EngUserService : IEngUser
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        SqlConnection con_gps = null;
        public EngUserService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
            con_gps = connect.Open_db_gps_Connect();
        }
        public bool CheckAllowEditable(string user_name)
        {
            bool allow = false;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT allow_edit FROM EngineerUsers Where LOWER(user_name) = '{user_name}'");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        allow = dr["allow_edit"] != DBNull.Value ? Convert.ToBoolean(dr["allow_edit"]) : false;
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
            return allow;
        }

        public List<EngUserModel> GetUsers()
        {
            List<EngUserModel> users = new List<EngUserModel>();
            try
            {
                if (con_gps.State == ConnectionState.Closed)
                {
                    con_gps.Open();
                }
                string string_command = string.Format($@"SELECT * FROM Sale_User");
                SqlCommand cmd = new SqlCommand(string_command, con_gps);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EngUserModel user = new EngUserModel()
                        {
                            user_id = dr["Login"] != DBNull.Value ? dr["Login"].ToString() : "",
                            user_name = dr["Name"] != DBNull.Value ? dr["Name"].ToString().ToLower() : "",
                            department = dr["Department2"] != DBNull.Value ? dr["Department2"].ToString() : "",
                            allow_edit = false,
                            group = dr["Group"] != DBNull.Value ? dr["Group"].ToString().ToLower() : "",
                            active = dr["active"] != DBNull.Value ? Convert.ToBoolean(dr["active"].ToString()) : false,
                        };
                        users.Add(user);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (con_gps.State == ConnectionState.Open)
                {
                    con_gps.Close();
                }
            }
            return users.GroupBy(g => g.user_name).Select(s => s.FirstOrDefault()).ToList();
        }

        public List<EngUserModel> GetEngineerUsers()
        {
            List<EngUserModel> engineers = new List<EngUserModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    SELECT 
                        user_id,
                        user_name,
                        department,
                        role,
                        allow_edit
                    FROM EngineerUsers");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EngUserModel eng = new EngUserModel()
                        {
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            user_name = dr["user_name"] != DBNull.Value ? dr["user_name"].ToString() : "",
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            role = dr["role"] != DBNull.Value ? dr["role"].ToString() : "",
                            allow_edit = dr["allow_edit"] != DBNull.Value ? Convert.ToBoolean(dr["allow_edit"].ToString()) : false,
                        };
                        engineers.Add(eng);
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
            return engineers;
        }

        public EngUserModel GetEngineerUser(string user_name)
        {
            List<EngUserModel> engineers = new List<EngUserModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    SELECT 
                        user_id,
                        user_name,
                        department,
                        role,
                        allow_edit
                    FROM EngineerUsers
                    WHERE LOWER(user_name) = '{user_name}'");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EngUserModel eng = new EngUserModel()
                        {
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            user_name = dr["user_name"] != DBNull.Value ? dr["user_name"].ToString() : "",
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            role = dr["role"] != DBNull.Value ? dr["role"].ToString() : "",
                            allow_edit = dr["allow_edit"] != DBNull.Value ? Convert.ToBoolean(dr["allow_edit"].ToString()) : false,
                        };
                        engineers.Add(eng);
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
            EngUserModel engineer = engineers.Where(w => w.user_name.ToLower() == user_name).FirstOrDefault();
            return engineer;
        }

        public string CreateEngineerUser(EngUserModel engineer)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    INSERT INTO 
                        EngineerUsers(user_id, user_name, department, allow_edit)
                        VALUES(@user_id, @user_name, @department, @allow_edit)");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@user_id", engineer.user_id);
                    cmd.Parameters.AddWithValue("@user_name", engineer.user_name);
                    cmd.Parameters.AddWithValue("@department", engineer.department);
                    cmd.Parameters.AddWithValue("@allow_edit", engineer.allow_edit);
                    cmd.ExecuteNonQuery();
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

        public string UpdateEngineerUser(EngUserModel engineer)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    UPDATE EngineerUsers 
                    SET
                        user_name = @user_name,
                        department = @department,
                        allow_edit = @allow_edit
                    WHERE user_id = @user_id");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@user_id", engineer.user_id);
                    cmd.Parameters.AddWithValue("@user_name", engineer.user_name);
                    cmd.Parameters.AddWithValue("@department", engineer.department);
                    cmd.Parameters.AddWithValue("@allow_edit", engineer.allow_edit);
                    cmd.ExecuteNonQuery();
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
