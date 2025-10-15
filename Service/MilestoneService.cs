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
    public class MilestoneService : IMilestone
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public MilestoneService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<MilestoneModel> GetMilestones()
        {
            List<MilestoneModel> milestones = new List<MilestoneModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT [No], Milestone_ID, Milestone_Name FROM Milestones");
                SqlCommand command = new SqlCommand(string_command, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        MilestoneModel ms = new MilestoneModel()
                        {
                            no = dr["No"] != DBNull.Value ? Convert.ToInt32(dr["no"]) : 0,
                            milestone_id = dr["Milestone_ID"] != DBNull.Value ? dr["Milestone_ID"].ToString() : "",
                            milestone_name = dr["Milestone_Name"] != DBNull.Value ? dr["Milestone_Name"].ToString() : ""
                        };
                        milestones.Add(ms);
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
            return milestones;
        }

        public int GetLastMilestoneID()
        {
            int id = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT TOP 1 Milestone_ID FROM Milestones ORDER BY Milestone_ID DESC");
                SqlCommand command = new SqlCommand(string_command, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        id = dr["Milestone_ID"] != DBNull.Value ? Convert.ToInt32(dr["Milestone_ID"].ToString().Substring(1)) : 0;
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

        public string CreateMilestone(MilestoneModel ms)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                INSERT INTO Milestones (
                    Milestone_ID,
                    Milestone_Name )
                VALUES (
                    @Milestone_ID,
                    @Milestone_Name
                )");
                SqlCommand command = new SqlCommand(string_command, con);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Milestone_ID", ms.milestone_id);
                command.Parameters.AddWithValue("@Milestone_Name", ms.milestone_name);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
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

        public string EditMilestone(MilestoneModel ms)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                UPDATE Milestones SET
                    Milestone_Name = @Milestone_Name,
                WHERE Milestone_ID = @Milestone_ID
                ");
                SqlCommand command = new SqlCommand(string_command, con);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Milestone_ID", ms.milestone_id);
                command.Parameters.AddWithValue("@Milestone_Name", ms.milestone_name);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
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
