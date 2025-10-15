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
    public class SystemService : ISystem
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public SystemService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<EngSystemModel> GetSystems()
        {
            List<EngSystemModel> systems = new List<EngSystemModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT [No], System_ID, System_Name, System_Description FROM Eng_System");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EngSystemModel system = new EngSystemModel()
                        {
                            no = dr["No"] != DBNull.Value ? Convert.ToInt32(dr["No"]) : 0,
                            system_id = dr["System_ID"] != DBNull.Value ? dr["System_ID"].ToString() : "",
                            system_name = dr["System_Name"] != DBNull.Value ? dr["System_Name"].ToString() : "",
                            system_description = dr["System_Description"] != DBNull.Value ? dr["System_Description"].ToString() : "",
                        };
                        systems.Add(system);
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
            return systems;
        }

        public int GetLastSystemID()
        {
            int id = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT TOP 1 System_ID FROM Eng_System ORDER BY System_ID DESC");
                SqlCommand cmd = new SqlCommand(string_command,con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        id = dr["System_ID"] != DBNull.Value ? Convert.ToInt32(dr["System_ID"].ToString().Substring(3)) : 0;
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
            return id;
        }

        public string CreateSystem(EngSystemModel system)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    INSERT INTO Eng_System(System_ID, System_Name, System_Description)
                    VALUES(@System_ID, @System_Name, @System_Description)");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@System_ID", system.system_id);
                    cmd.Parameters.AddWithValue("@System_Name", system.system_name);
                    cmd.Parameters.AddWithValue("@System_Description", system.system_description);
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

        public string EditSystem(EngSystemModel system)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    UPDATE Eng_System
                    SET
                        System_Name = @System_Name,
                        System_Description = @System_Description
                    WHERE System_ID = @System_ID");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@System_Name", system.system_name);
                    cmd.Parameters.AddWithValue("@System_Description", system.system_description);
                    cmd.Parameters.AddWithValue("@System_ID", system.system_id);
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

        public string DeleteSystem(EngSystemModel system)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"DELETE FROM Eng_System WHERE System_ID = @System_ID");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@System_ID", system.system_id);
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
