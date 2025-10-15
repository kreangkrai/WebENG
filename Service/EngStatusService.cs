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
    public class EngStatusService : IStatus
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public EngStatusService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public string CreateStatus(EngStatusModel status)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    INSERT INTO Eng_Status(Status_ID, Status_Name, Status_Description)
                    VALUES(@Status_ID, @Status_Name, @Status_Description)");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Status_ID", status.status_id);
                    cmd.Parameters.AddWithValue("@Status_Name", status.status_name);
                    cmd.Parameters.AddWithValue("@Status_Description", status.status_description);
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

        public string DeleteStatus(EngStatusModel status)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"DELETE FROM Eng_Status WHERE Status_ID = @Status_ID");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Status_ID", status.status_id);
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

        public string EditStatus(EngStatusModel status)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    UPDATE Eng_Status 
                    SET
                        Status_Name = @Status_Name,
                        Status_Description = @Status_Description
                    WHERE Status_ID = @Status_ID");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Status_Name", status.status_name);
                    cmd.Parameters.AddWithValue("@Status_Description", status.status_description);
                    cmd.Parameters.AddWithValue("@Status_ID", status.status_id);
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

        public int GetLastStatusID()
        {
            int id = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT TOP 1 Status_ID FROM Eng_Status ORDER BY Status_ID DESC");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        id = dr["Status_ID"] != DBNull.Value ? Convert.ToInt32(dr["Status_ID"].ToString().Substring(3)) : 0;
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

        public List<EngStatusModel> GetStatuses()
        {
            List<EngStatusModel> statuses = new List<EngStatusModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT [No], Status_ID, Status_Name, Status_Description FROM Eng_Status");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EngStatusModel status = new EngStatusModel()
                        {
                            no = dr["No"] != DBNull.Value ? Convert.ToInt32(dr["No"]) : 0,
                            status_id = dr["Status_ID"] != DBNull.Value ? dr["Status_ID"].ToString() : "",
                            status_name = dr["Status_Name"] != DBNull.Value ? dr["Status_Name"].ToString() : "",
                            status_description = dr["Status_Description"] != DBNull.Value ? dr["Status_Description"].ToString() : "",
                        };
                        statuses.Add(status);
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
            return statuses;
        }
    }
}
