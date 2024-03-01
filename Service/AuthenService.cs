using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;

namespace WebENG.Service
{
    public class AuthenService : IAuthen
    {
        public List<AuthenModel> GetAuthens()
        {
            List<AuthenModel> authens = new List<AuthenModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SElECT user_id,name,department,role FROM Authen ORDER BY name");
                SqlCommand command = new SqlCommand(strCmd, connection);
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
                            role = dr["role"].ToString()
                        };
                        authens.Add(authen);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return authens;
        }

        public string Insert(AuthenModel authen)
        {
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string string_command = string.Format($@"
                IF (SELECT COUNT(name) FROM Authen WHERE name ='{authen.name}') = 0
                BEGIN
                    INSERT INTO Authen (
                        user_id,
                        name,
                        department,
                        role ) 
                    VALUES (
                        @user_id,
                        @name, 
                        @department,
                        @role
                    )
                END");
                SqlCommand command = new SqlCommand(string_command, connection);
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.AddWithValue("@user_id", authen.user_id);
                command.Parameters.AddWithValue("@name", authen.name.ToLower());
                command.Parameters.AddWithValue("@department", authen.department);
                command.Parameters.AddWithValue("@role", authen.role);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return "Success";
        }

        public string UpdateRole(AuthenModel authen)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE Authen 
                    SET
                        role = @role 
                    WHERE name = @name");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@role", authen.role);
                    cmd.Parameters.AddWithValue("@name", authen.name.ToLower());
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return "Success";
        }
    }
}
