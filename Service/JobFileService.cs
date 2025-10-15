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
    public class JobFileService : IJobFile
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public JobFileService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public string CreateJobFile(string job_id)
        {
            try
            {
                //===item===
                // quatation
                // po
                // hand_over

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    INSERT INTO JobFile (job_id,quotation,po,hand_over)
                               VALUES (@job_id,@quotation,@po,@hand_over)");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job_id.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@quotation", "");
                    cmd.Parameters.AddWithValue("@po", "");
                    cmd.Parameters.AddWithValue("@hand_over", "");
                    cmd.ExecuteNonQuery();
                }
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

        public JobFileModel GetJobFile(string job_id)
        {
            JobFileModel job = new JobFileModel();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader dr = null;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string command = string.Format($@"SELECT job_id,quotation,po,hand_over FROM JobFile WHERE job_id = '{job_id}'");
                cmd = new SqlCommand(command, con);
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        job = new JobFileModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            quotation = dr["quotation"] != DBNull.Value ? dr["quotation"].ToString() : "",
                            po = dr["po"] != DBNull.Value ? dr["po"].ToString() : "",
                            hand_over = dr["hand_over"] != DBNull.Value ? dr["hand_over"].ToString() : "",
                        };
                    }
                }
                else
                {
                    job = new JobFileModel()
                    {
                        job_id = job_id,
                        quotation = "",
                        po = "",
                        hand_over = "",
                    };
                }
                dr.Close();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return job;
        }

        public string UpdateJobFileByItem(string job_id, string item, string link)
        {
            try
            {
                //===item===
                // quatation
                // po
                // hand_over

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    UPDATE JobFile
                    SET
                        {item} = @item
                    WHERE job_id = @job_id");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job_id);
                    cmd.Parameters.AddWithValue("@item", link);
                    cmd.ExecuteNonQuery();
                }
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
