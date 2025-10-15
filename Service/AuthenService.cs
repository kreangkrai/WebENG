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
    public class AuthenService : IAuthen
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public AuthenService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<AuthenModel> GetAuthens()
        {
            List<AuthenModel> authens = new List<AuthenModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SElECT user_id,name,department,role,levels FROM Authen ORDER BY name");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        AuthenModel authen = new AuthenModel()
                        {
                            user_id = dr["user_id"].ToString(),
                            name = dr["name"].ToString(),
                            department = dr["department"].ToString(),
                            role = dr["role"].ToString(),
                            levels = dr["levels"] != DBNull.Value ? Convert.ToInt32(dr["levels"].ToString()): 1
                        };
                        authens.Add(authen);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (con.State== ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return authens;
        }

        public string Insert(AuthenModel authen)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                IF (SELECT COUNT(name) FROM Authen WHERE name ='{authen.name}') = 0
                BEGIN
                    INSERT INTO Authen (
                        user_id,
                        name,
                        department,
                        role,
                        levels) 
                    VALUES (
                        @user_id,
                        @name, 
                        @department,
                        @role,
                        @levels
                    )
                END");
                SqlCommand command = new SqlCommand(string_command, con);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@user_id", authen.user_id);
                command.Parameters.AddWithValue("@name", authen.name.ToLower());
                command.Parameters.AddWithValue("@department", authen.department);
                command.Parameters.AddWithValue("@role", authen.role);
                command.Parameters.AddWithValue("@levels", authen.levels);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return "Success";
        }

        public string UpdateLevel(AuthenModel authen)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    UPDATE Authen 
                    SET
                        levels = @levels
                    WHERE name = @name");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@levels", authen.levels);
                    cmd.Parameters.AddWithValue("@name", authen.name.ToLower());
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

        public string UpdateRole(AuthenModel authen)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    UPDATE Authen 
                    SET
                        role = @role
                    WHERE name = @name");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@role", authen.role);
                    cmd.Parameters.AddWithValue("@name", authen.name.ToLower());
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
