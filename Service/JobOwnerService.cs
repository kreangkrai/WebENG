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
    public class JobOwnerService : IJobOwner
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public JobOwnerService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public string DeleteByJobDepartment(string job_id, string job_department)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                job_id = job_id.Replace("-", String.Empty);
                string string_command = string.Format($@"DELETE FROM JobOwner WHERE job_id ='{job_id}' AND job_department='{job_department}'");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
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

        public List<JobOwnerModel> GetJobOwner()
        {
            List<JobOwnerModel> jobs = new List<JobOwnerModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string stringCommand = string.Format($@"SELECT job_id,job_department FROM JobOwner");
                SqlCommand cmd = new SqlCommand(stringCommand, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        JobOwnerModel job = new JobOwnerModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_department = dr["job_department"] != DBNull.Value ? dr["job_department"].ToString() : "",
                        };
                        jobs.Add(job);
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
            return jobs;
        }

        public List<JobOwnerModel> GetJobOwnerByJob(string job_id)
        {
            List<JobOwnerModel> jobs = new List<JobOwnerModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                job_id = job_id.Replace("-", String.Empty);
                string stringCommand = string.Format($@"SELECT job_id,job_department FROM JobOwner WHERE job_id = '{job_id}'");
                SqlCommand cmd = new SqlCommand(stringCommand, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        JobOwnerModel job = new JobOwnerModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_department = dr["job_department"] != DBNull.Value ? dr["job_department"].ToString() : "",
                        };
                        jobs.Add(job);
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
            return jobs;
        }

        public string Insert(string job_id, string job_department)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    IF NOT EXISTS ( SELECT 1 FROM JobOwner WHERE job_id = '{job_id}' AND job_department = '{job_department}' )
                        BEGIN
                            INSERT INTO 
                                JobOwner(job_id,
                                         job_department
                                )
                                VALUES(@job_id,
                                       @job_department
                                )
                        END");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job_id.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@job_department", job_department);
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
