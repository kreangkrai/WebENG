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
    public class SkillService : ISkill
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public SkillService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<EngSkillModel> GetSkills()
        {
            List<EngSkillModel> skills = new List<EngSkillModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT [No], Skill_ID, Skill_Name, Skill_Description FROM Eng_Skill");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EngSkillModel skill = new EngSkillModel()
                        {
                            no = dr["No"] != DBNull.Value ? Convert.ToInt32(dr["No"]) : 0,
                            skill_id = dr["Skill_ID"] != DBNull.Value ? dr["Skill_ID"].ToString() : "",
                            skill_name = dr["Skill_Name"] != DBNull.Value ? dr["Skill_Name"].ToString() : "",
                            skill_description = dr["Skill_Description"] != DBNull.Value ? dr["Skill_Description"].ToString() : "",
                        };
                        skills.Add(skill);
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
            return skills;
        }

        public int GetLastSkillID()
        {
            int id = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT TOP 1 Skill_ID FROM Eng_Skill ORDER BY Skill_ID DESC");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        id = dr["Skill_ID"] != DBNull.Value ? Convert.ToInt32(dr["Skill_ID"].ToString().Substring(3)) : 0;
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

        public string CreateSkill(EngSkillModel skill)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    INSERT INTO Eng_Skill(Skill_ID, Skill_Name, Skill_Description)
                    VALUES(@Skill_ID, @Skill_Name, @Skill_Description)");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@Skill_ID", skill.skill_id);
                    cmd.Parameters.AddWithValue("@Skill_Name", skill.skill_name);
                    cmd.Parameters.AddWithValue("@Skill_Description", skill.skill_description);
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

        public string EditSkill(EngSkillModel skill)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    UPDATE Eng_Skill 
                    SET
                        Skill_Name = @Skill_Name,
                        Skill_Description = @Skill_Description
                    WHERE Skill_ID = @Skill_ID");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@Skill_Name", skill.skill_name);
                    cmd.Parameters.AddWithValue("@Skill_Description", skill.skill_description);
                    cmd.Parameters.AddWithValue("@Skill_ID", skill.skill_id);
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

        public string DeleteSkill(EngSkillModel skill)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"DELETE FROM Eng_Skill WHERE Skill_ID = @Skill_ID");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Skill_ID", skill.skill_id);
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
