using WebENG.Interface;
using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Service;

namespace WebENG.Service
{
    public class JobService : IJob
    {
        public List<JobModel> GetAllJobs()
        {
            List<JobModel> jobs = new List<JobModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT
                        Jobs.job_id,
                        Jobs.job_name,
                        Jobs.cost,
                        Jobs.process_id,
                        Jobs.system_id,
                        Jobs.md_rate,
                        Jobs.pd_rate,
                        Jobs.status,
                        Jobs.quotation_no,
                        Jobs.finished_date,
                        Quotation.customer,
	                    Quotation.enduser,
	                    Quotation.sale_name,
	                    Quotation.department
                    FROM Jobs
                    LEFT JOIN Quotation ON Jobs.quotation_no = Quotation.quotation_no
                    ORDER BY Jobs.job_id");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
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
                        JobModel job = new JobModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            cost = dr["cost"] != DBNull.Value ? Convert.ToInt32(dr["cost"]) : 0,
                            process = dr["process_id"] != DBNull.Value ? dr["process_id"].ToString() : "",
                            system = dr["system_id"] != DBNull.Value ? dr["system_id"].ToString() : "",
                            md_rate = dr["md_rate"] != DBNull.Value ? Convert.ToDouble(dr["md_rate"]) : 1,
                            pd_rate = dr["pd_rate"] != DBNull.Value ? Convert.ToDouble(dr["pd_rate"]) : 1,
                            factor = 0,
                            manpower = 0,
                            cost_per_manpower = 0,
                            ot_manpower = 0,
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "",
                            quotation_no = dr["quotation_no"] != DBNull.Value ? dr["quotation_no"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                            enduser = dr["enduser"] != DBNull.Value ? dr["enduser"].ToString() : "",
                            sale_name = dr["sale_name"] != DBNull.Value ? dr["sale_name"].ToString() : "",
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            finished_date = dr["finished_date"] != DBNull.Value ? Convert.ToDateTime(dr["finished_date"].ToString()) : DateTime.MinValue
                        };
                        job.factor = job.md_rate + job.pd_rate;
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

        public List<JobSummaryModel> GetJobsSummary()
        {
            List<JobSummaryModel> jobsSummaries = new List<JobSummaryModel>();
            try
            {
                string stringCommand = string.Format($@"
                    WITH T1 AS (
                        SELECT 
		                    WorkingHours.job_id, 
		                    SUM(case when lunch = 1
							 then case when dinner = 1 
								then 
									DATEDIFF(HOUR,start_time,stop_time) - 2
								else 
									DATEDIFF(HOUR,start_time,stop_time) - 1 
							 end
							 else case when dinner = 1 
								then 
									DATEDIFF(HOUR,start_time,stop_time) - 1 
								else 
									DATEDIFF(HOUR,start_time,stop_time)
							 end
						end ) AS total_manpower 
	                    FROM WorkingHours
	                    GROUP BY job_id
					)
                    SELECT
                        Jobs.job_id,
                        Jobs.job_name,
                        Quotation.customer,
                        Jobs.cost,
                        (Jobs.md_rate * Jobs.pd_rate) as factor,
                        T1.total_manpower,
                        Jobs.status
                    FROM Jobs
                    LEFT JOIN Quotation ON Jobs.quotation_no = Quotation.quotation_no
                    LEFT JOIN T1 ON Jobs.job_id = T1.job_id
                    ORDER BY Jobs.job_id");
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
                        JobSummaryModel jobSummary = new JobSummaryModel()
                        {
                            jobId = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            jobName = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                            cost = dr["cost"] != DBNull.Value ? Convert.ToInt32(dr["cost"]) : 0,
                            factor = dr["factor"] != DBNull.Value ? Convert.ToDouble(dr["factor"]) : 1,
                            totalManhour = dr["total_manpower"] != DBNull.Value ? Convert.ToInt32(dr["total_manpower"]) : 0,
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "1"
                        };
                        jobSummary.remainingCost = jobSummary.cost - ((jobSummary.totalManhour / 8) * 3200);
                        jobsSummaries.Add(jobSummary);
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
            return jobsSummaries;
        }

        public string CreateJob(JobModel job)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO 
                        Jobs(job_id, job_name, quotation_no, cost,process_id,system_id, md_rate, pd_rate, status)
                        VALUES(@job_id, @job_name, @quotation_no, @cost,@process_id,@system_id, @md_rate, @pd_rate, @status)");
                using (SqlCommand cmd = new SqlCommand(string_command,ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job.job_id.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@job_name", job.job_name);
                    cmd.Parameters.AddWithValue("@quotation_no", job.quotation_no);
                    cmd.Parameters.AddWithValue("@cost", job.cost);
                    cmd.Parameters.AddWithValue("@process_id", job.process);
                    cmd.Parameters.AddWithValue("@system_id", job.system);
                    cmd.Parameters.AddWithValue("@md_rate", job.md_rate);
                    cmd.Parameters.AddWithValue("@pd_rate", job.pd_rate);
                    cmd.Parameters.AddWithValue("@status", job.status);
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

        public string UpdateJob(JobModel job)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE Jobs 
                    SET
                        job_name = @job_name,
                        quotation_no = @quotation_no,
                        cost = @cost,
                        process_id = @process_id,
                        system_id = @system_id,
                        md_rate = @md_rate,
                        pd_rate = @pd_rate,
                        status = @status,
                        finished_date = @finished_date
                    WHERE job_id = @job_id");
                using (SqlCommand cmd = new SqlCommand(string_command,ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job.job_id.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@job_name", job.job_name);
                    cmd.Parameters.AddWithValue("@quotation_no", job.quotation_no);
                    cmd.Parameters.AddWithValue("@cost", job.cost);
                    cmd.Parameters.AddWithValue("@process_id", job.process);
                    cmd.Parameters.AddWithValue("@system_id", job.system);
                    cmd.Parameters.AddWithValue("@md_rate", job.md_rate);
                    cmd.Parameters.AddWithValue("@pd_rate", job.pd_rate);
                    cmd.Parameters.AddWithValue("@status", job.status);
                    cmd.Parameters.AddWithValue("@finished_date", job.finished_date);
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

        public List<JobQuotationModel> GetJobQuotations(string year)
        {
            List<JobQuotationModel> quots = new List<JobQuotationModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT
                        quotation_no,
                        customer
                    FROM Quotation
                    WHERE quotation_no Like 'Q{year}%' OR 
                    quotation_no Like 'Q{Convert.ToInt32(year) - 1}%'");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
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
                        JobQuotationModel quot = new JobQuotationModel()
                        {
                            quotation_no = dr["quotation_no"] != DBNull.Value ? dr["quotation_no"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                        };
                        quots.Add(quot);
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
            return quots;
        }

        public List<EngProcessModel> GetProcessByJob(string job)
        {
            List<EngProcessModel> processes = new List<EngProcessModel>();
            processes.Add(new EngProcessModel()
            {
                process_id = "PRO000",
                process_name = ""
            });
            try
            {
                string string_command = string.Format($@"
                    SELECT  job_id, Jobs.process_id,Eng_Process.Process_Name as process_name FROM Jobs
                    CROSS APPLY STRING_SPLIT(process_id,',')
                    LEFT JOIN Eng_Process ON Eng_Process.Process_ID = Jobs.process_id
                    WHERE job_id ='{job}'");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
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
                        EngProcessModel process = new EngProcessModel()
                        {
                            process_id = dr["process_id"] != DBNull.Value ? dr["process_id"].ToString() : "",
                            process_name = dr["process_name"] != DBNull.Value ? dr["process_name"].ToString() : ""
                        };
                        processes.Add(process);
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
            return processes;
        }

        public List<EngSystemModel> GetSystemByJob(string job)
        {
            List<EngSystemModel> systems = new List<EngSystemModel>();
            systems.Add(new EngSystemModel
            {
                system_id = "SYS000",
                system_name = ""
            });
            try
            {
                string string_command = string.Format($@"
                    SELECT  job_id, Jobs.system_id,Eng_System.System_Name as system_name FROM Jobs
                    CROSS APPLY STRING_SPLIT(system_id,',')
                    LEFT JOIN Eng_System ON Eng_System.System_ID = Jobs.system_id
                    WHERE job_id ='{job}'");
                SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
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
                        EngSystemModel system = new EngSystemModel()
                        {
                            system_id = dr["system_id"] != DBNull.Value ? dr["system_id"].ToString() : "",
                            system_name = dr["system_name"] != DBNull.Value ? dr["system_name"].ToString() : ""
                        };
                        systems.Add(system);
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
            return systems;
        }
    }
}
