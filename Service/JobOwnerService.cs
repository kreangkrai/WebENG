using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;

namespace WebENG.Service
{
    public class JobOwnerService : IJobOwner
    {
        public string DeleteByJobDepartment(string job_id, string job_department)
        {
            try
            {
                job_id = job_id.Replace("-", String.Empty);
                string string_command = string.Format($@"DELETE FROM JobOwner WHERE job_id ='{job_id}' AND job_department='{job_department}'");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
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

        public List<JobOwnerModel> GetJobOwner()
        {
            List<JobOwnerModel> jobs = new List<JobOwnerModel>();
            try
            {
                string stringCommand = string.Format($@"SELECT job_id,job_department FROM JobOwner");
                SqlCommand cmd = new SqlCommand(stringCommand, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
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
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return jobs;
        }

        public List<JobOwnerModel> GetJobOwnerByJob(string job_id)
        {
            List<JobOwnerModel> jobs = new List<JobOwnerModel>();
            try
            {
                job_id = job_id.Replace("-", String.Empty);
                string stringCommand = string.Format($@"SELECT job_id,job_department FROM JobOwner WHERE job_id = '{job_id}'");
                SqlCommand cmd = new SqlCommand(stringCommand, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
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
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return jobs;
        }

        public string Insert(string job_id, string job_department)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO 
                        JobOwner(job_id,
                                 job_department
                        )
                        VALUES(@job_id,
                               @job_department
                        )");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job_id.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@job_department", job_department);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
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
