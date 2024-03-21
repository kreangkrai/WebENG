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
						Jobs.down_payment,
						Jobs.document_submit,
						Jobs.instrument_delivered_ctl,
						Jobs.system_delivered_ctl,
						Jobs.fat,
						Jobs.delivery_instrument,
						Jobs.delivery_system,
						Jobs.progress_work,
						Jobs.commissioning,
						Jobs.as_built,
						Jobs.job_in_hand,
						Jobs.invoice,
						Jobs.due_date,
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
                            down_payment = dr["down_payment"] != DBNull.Value ? Convert.ToInt32(dr["down_payment"]) : 0,
                            document_submit = dr["document_submit"] != DBNull.Value ? Convert.ToInt32(dr["document_submit"]) : 0,
                            instrument_delivered_ctl = dr["instrument_delivered_ctl"] != DBNull.Value ? Convert.ToInt32(dr["instrument_delivered_ctl"]) : 0,
                            system_delivered_ctl = dr["system_delivered_ctl"] != DBNull.Value ? Convert.ToInt32(dr["system_delivered_ctl"]) : 0,
                            fat = dr["fat"] != DBNull.Value ? Convert.ToInt32(dr["fat"]) : 0,
                            delivery_instrument = dr["delivery_instrument"] != DBNull.Value ? Convert.ToInt32(dr["delivery_instrument"]) : 0,
                            delivery_system = dr["delivery_system"] != DBNull.Value ? Convert.ToInt32(dr["delivery_system"]) : 0,
                            progress_work = dr["progress_work"] != DBNull.Value ? Convert.ToInt32(dr["progress_work"]) : 0,
                            commissioning = dr["commissioning"] != DBNull.Value ? Convert.ToInt32(dr["commissioning"]) : 0,
                            as_built = dr["as_built"] != DBNull.Value ? Convert.ToInt32(dr["as_built"]) : 0,
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0.0,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0.0,
                            due_date = dr["due_date"] != DBNull.Value ? Convert.ToDateTime(dr["due_date"].ToString()) : DateTime.MinValue,
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
                        WHERE WorkingHours.job_id <> 'J999999'
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
                    WHERE Jobs.job_id <> 'J999999'
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
                        Jobs(job_id, job_name, quotation_no, cost,process_id,
                             system_id, md_rate, pd_rate, status,
                             down_payment,document_submit,instrument_delivered_ctl,system_delivered_ctl,
                             fat,delivery_instrument,delivery_system,progress_work,
                             commissioning,as_built,job_in_hand,invoice,due_date
                        )
                        VALUES(@job_id, @job_name, @quotation_no, @cost,@process_id,
                               @system_id, @md_rate, @pd_rate, @status,
                               @down_payment,@document_submit,@instrument_delivered_ctl,@system_delivered_ctl,
                               @fat,@delivery_instrument,@delivery_system,@progress_work,
                               @commissioning,@as_built,@job_in_hand,@invoice,@due_date
                        )");
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
                    cmd.Parameters.AddWithValue("@down_payment", job.down_payment);
                    cmd.Parameters.AddWithValue("@document_submit", job.document_submit);
                    cmd.Parameters.AddWithValue("@instrument_delivered_ctl", job.instrument_delivered_ctl);
                    cmd.Parameters.AddWithValue("@system_delivered_ctl", job.system_delivered_ctl);
                    cmd.Parameters.AddWithValue("@fat", job.fat);
                    cmd.Parameters.AddWithValue("@delivery_instrument", job.delivery_instrument);
                    cmd.Parameters.AddWithValue("@delivery_system", job.delivery_system);
                    cmd.Parameters.AddWithValue("@progress_work", job.progress_work);
                    cmd.Parameters.AddWithValue("@commissioning", job.commissioning);
                    cmd.Parameters.AddWithValue("@as_built", job.as_built);
                    cmd.Parameters.AddWithValue("@job_in_hand", job.job_in_hand);
                    cmd.Parameters.AddWithValue("@invoice", job.invoice);
                    cmd.Parameters.AddWithValue("@due_date", job.due_date);
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
                if (job.finished_date.Day == new DateTime(1, 1, 1).Day)
                {
                    job.finished_date = new DateTime(1900, 1, 1);
                }
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
                        down_payment = @down_payment,
                        document_submit = @document_submit,
                        instrument_delivered_ctl = @instrument_delivered_ctl,
                        system_delivered_ctl = @system_delivered_ctl,
                        fat = @fat,
                        delivery_instrument = @delivery_instrument,
                        delivery_system = @delivery_system,
                        progress_work = @progress_work,
                        commissioning = @commissioning,
                        as_built = @as_built,
                        job_in_hand = @job_in_hand,
                        invoice = @invoice,
                        due_date = @due_date,
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
                    cmd.Parameters.AddWithValue("@down_payment", job.down_payment);
                    cmd.Parameters.AddWithValue("@document_submit", job.document_submit);
                    cmd.Parameters.AddWithValue("@instrument_delivered_ctl", job.instrument_delivered_ctl);
                    cmd.Parameters.AddWithValue("@system_delivered_ctl", job.system_delivered_ctl);
                    cmd.Parameters.AddWithValue("@fat", job.fat);
                    cmd.Parameters.AddWithValue("@delivery_instrument", job.delivery_instrument);
                    cmd.Parameters.AddWithValue("@delivery_system", job.delivery_system);
                    cmd.Parameters.AddWithValue("@progress_work", job.progress_work);
                    cmd.Parameters.AddWithValue("@commissioning", job.commissioning);
                    cmd.Parameters.AddWithValue("@as_built", job.as_built);
                    cmd.Parameters.AddWithValue("@job_in_hand", job.job_in_hand);
                    cmd.Parameters.AddWithValue("@invoice", job.invoice);
                    cmd.Parameters.AddWithValue("@due_date", job.due_date);
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
            try
            {
                string string_command = string.Format($@"SELECT Jobs.process_id FROM Jobs WHERE job_id ='{job}'");
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

            List<EngProcessModel> new_processes = new List<EngProcessModel>();
            if (processes.Count > 0)
            {
                List<EngProcessModel> _processes = GetProcesses();

                int count = processes[0].process_id.Count(c => c == ',');

                for (int i = 0; i <= count; i++)
                {
                    string process = processes[0].process_id.Split(',')[i];
                    new_processes.Add(new EngProcessModel()
                    {
                        process_id = process,
                        process_name = _processes.Where(w => w.process_id == process).Select(s => s.process_name).FirstOrDefault()
                    }) ;
                }
            }

            return new_processes;
        }

        public List<EngSystemModel> GetSystemByJob(string job)
        {
            List<EngSystemModel> systems = new List<EngSystemModel>();
            try
            {
                string string_command = string.Format($@"SELECT Jobs.system_id FROM Jobs WHERE job_id ='{job}'");
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

            List<EngSystemModel> new_systems = new List<EngSystemModel>();
            if (systems.Count > 0)
            {
                List<EngSystemModel> _systems = GetSystems();

                int count = systems[0].system_id.Count(c => c == ',');

                for (int i = 0; i <= count; i++)
                {
                    string system = systems[0].system_id.Split(',')[i];
                    new_systems.Add(new EngSystemModel()
                    {
                        system_id = system,
                        system_name = _systems.Where(w => w.system_id == system).Select(s => s.system_name).FirstOrDefault()
                    });
                }
            }

            return new_systems;
        }

        public List<EngProcessModel> GetProcesses()
        {
            List<EngProcessModel> processes = new List<EngProcessModel>();
            try
            {
                string string_command = string.Format($@"SELECT process_id,process_name FROM Eng_Process");
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

        public List<EngSystemModel> GetSystems()
        {
            List<EngSystemModel> systems = new List<EngSystemModel>();
            try
            {
                string string_command = string.Format($@"SELECT system_id,system_name FROM Eng_System");
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
