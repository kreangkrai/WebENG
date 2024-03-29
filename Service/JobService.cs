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
            SqlCommand cmd = new SqlCommand();
            SqlDataReader dr = null;
            try
            {
                string command_invoice = string.Format($@"SELECT job_id,invoice,invoice_date FROM Invoice");
                List<InvoiceModel> invoices = new List<InvoiceModel>();
                cmd = new SqlCommand(command_invoice, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        InvoiceModel invoice = new InvoiceModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0.0,
                            invoice_date = dr["invoice_date"] != DBNull.Value ? Convert.ToDateTime(dr["invoice_date"].ToString()) : DateTime.MinValue,
                        };
                        invoices.Add(invoice);
                    }
                }

                string string_command = string.Format($@"
                    SELECT
                        Jobs.job_id,
                        Jobs.job_name,
                        Jobs.customer_name as customer,
                        Jobs.job_date,
                        Jobs.cost,
                        Jobs.process_id,
                        Jobs.system_id,
                        Jobs.md_rate,
                        Jobs.pd_rate,
                        Jobs.status,
						Term_Payment.down_payment,
						Term_Payment.document_submit,
						Term_Payment.instrument_vendor,
						Term_Payment.instrument_delivered_ctl,
						Term_Payment.system_delivered_ctl,
						Term_Payment.fat,
						Term_Payment.delivery_instrument,
						Term_Payment.delivery_system,
						Term_Payment.progress_work,
						Term_Payment.installation_work_complete,
						Term_Payment.commissioning,
						Term_Payment.startup,
						Term_Payment.as_built,
						Term_Payment.warranty,
						Term_Payment.finished,
						Jobs.job_in_hand,
						Jobs.due_date,
                        Jobs.quotation_no,
                        Jobs.job_type,
                        Jobs.finished_date,
                        Quotation.customer,
	                    Quotation.enduser,
	                    Quotation.sale_name,
	                    Quotation.department
                    FROM Jobs
                    LEFT JOIN Quotation ON Jobs.quotation_no = Quotation.quotation_no
					LEFT JOIN Term_Payment ON Term_Payment.job_id = Jobs.job_id
                    ORDER BY Jobs.job_id");
                cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        List<InvoiceModel> _invoices = new List<InvoiceModel>();
                        _invoices = invoices.Where(w => w.job_id == dr["job_id"].ToString()).ToList();

                        Term_PaymentModel term_Payment = new Term_PaymentModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            down_payment = dr["down_payment"] != DBNull.Value ? Convert.ToInt32(dr["down_payment"]) : 0,
                            document_submit = dr["document_submit"] != DBNull.Value ? Convert.ToInt32(dr["document_submit"]) : 0,
                            instrument_vendor = dr["instrument_vendor"] != DBNull.Value ? Convert.ToInt32(dr["instrument_vendor"]) : 0,
                            instrument_delivered_ctl = dr["instrument_delivered_ctl"] != DBNull.Value ? Convert.ToInt32(dr["instrument_delivered_ctl"]) : 0,
                            system_delivered_ctl = dr["system_delivered_ctl"] != DBNull.Value ? Convert.ToInt32(dr["system_delivered_ctl"]) : 0,
                            fat = dr["fat"] != DBNull.Value ? Convert.ToInt32(dr["fat"]) : 0,
                            delivery_instrument = dr["delivery_instrument"] != DBNull.Value ? Convert.ToInt32(dr["delivery_instrument"]) : 0,
                            delivery_system = dr["delivery_system"] != DBNull.Value ? Convert.ToInt32(dr["delivery_system"]) : 0,
                            progress_work = dr["progress_work"] != DBNull.Value ? Convert.ToInt32(dr["progress_work"]) : 0,
                            installation_work_complete = dr["installation_work_complete"] != DBNull.Value ? Convert.ToInt32(dr["installation_work_complete"]) : 0,
                            commissioning = dr["commissioning"] != DBNull.Value ? Convert.ToInt32(dr["commissioning"]) : 0,
                            startup = dr["startup"] != DBNull.Value ? Convert.ToInt32(dr["startup"]) : 0,
                            as_built = dr["as_built"] != DBNull.Value ? Convert.ToInt32(dr["as_built"]) : 0,
                            warranty = dr["warranty"] != DBNull.Value ? Convert.ToInt32(dr["warranty"]) : 0,
                            finished = dr["finished"] != DBNull.Value ? Convert.ToInt32(dr["finished"]) : 0
                        };
                        JobModel job = new JobModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",                           
                            job_date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
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
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0.0,
                            invoices = _invoices,
                            due_date = dr["due_date"] != DBNull.Value ? Convert.ToDateTime(dr["due_date"].ToString()) : DateTime.MinValue,
                            quotation_no = dr["quotation_no"] != DBNull.Value ? dr["quotation_no"].ToString() : "",
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                            enduser = dr["enduser"] != DBNull.Value ? dr["enduser"].ToString() : "",
                            sale_name = dr["sale_name"] != DBNull.Value ? dr["sale_name"].ToString() : "",
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            finished_date = dr["finished_date"] != DBNull.Value ? Convert.ToDateTime(dr["finished_date"].ToString()) : DateTime.MinValue
                        };
                        job.factor = job.md_rate + job.pd_rate;
                        job.term_payment = term_Payment;
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
                        Jobs.status,
                        Jobs.process_id as process,
						Jobs.system_id as system
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
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "1",
                            process = dr["process"] != DBNull.Value ? dr["process"].ToString() : "",
                            system = dr["system"] != DBNull.Value ? dr["system"].ToString() : ""
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
                        Jobs(job_id, job_name,job_date,customer_name,quotation_no,job_type, cost,process_id,
                             system_id, md_rate, pd_rate, status,
                             job_in_hand,due_date
                        )
                        VALUES(@job_id, @job_name,@job_date,@customer_name, @quotation_no,@job_type, @cost,@process_id,
                               @system_id, @md_rate, @pd_rate, @status,
                               @job_in_hand,@due_date
                        )");
                using (SqlCommand cmd = new SqlCommand(string_command,ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job.job_id.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@job_name", job.job_name);
                    cmd.Parameters.AddWithValue("@job_date", job.job_date);
                    cmd.Parameters.AddWithValue("@customer_name", job.customer);
                    cmd.Parameters.AddWithValue("@quotation_no", job.quotation_no);
                    cmd.Parameters.AddWithValue("@job_type", job.job_type);
                    cmd.Parameters.AddWithValue("@cost", job.cost);
                    cmd.Parameters.AddWithValue("@process_id", job.process);
                    cmd.Parameters.AddWithValue("@system_id", job.system);
                    cmd.Parameters.AddWithValue("@md_rate", job.md_rate);
                    cmd.Parameters.AddWithValue("@pd_rate", job.pd_rate);
                    cmd.Parameters.AddWithValue("@status", job.status);
                    cmd.Parameters.AddWithValue("@job_in_hand", job.job_in_hand);
                    cmd.Parameters.AddWithValue("@due_date", job.due_date);
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
                        job_date = @job_date,
                        customer_name = @customer_name,
                        quotation_no = @quotation_no,
                        job_type = @job_type,
                        cost = @cost,
                        process_id = @process_id,
                        system_id = @system_id,
                        md_rate = @md_rate,
                        pd_rate = @pd_rate,
                        status = @status,
                        job_in_hand = @job_in_hand,
                        due_date = @due_date,
                        finished_date = @finished_date
                    WHERE job_id = @job_id");
                using (SqlCommand cmd = new SqlCommand(string_command,ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job.job_id.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@job_name", job.job_name);
                    cmd.Parameters.AddWithValue("@job_date", job.job_date);
                    cmd.Parameters.AddWithValue("@customer_name", job.customer);
                    cmd.Parameters.AddWithValue("@quotation_no", job.quotation_no);
                    cmd.Parameters.AddWithValue("@job_type", job.job_type);
                    cmd.Parameters.AddWithValue("@cost", job.cost);
                    cmd.Parameters.AddWithValue("@process_id", job.process);
                    cmd.Parameters.AddWithValue("@system_id", job.system);
                    cmd.Parameters.AddWithValue("@md_rate", job.md_rate);
                    cmd.Parameters.AddWithValue("@pd_rate", job.pd_rate);
                    cmd.Parameters.AddWithValue("@status", job.status);
                    cmd.Parameters.AddWithValue("@job_in_hand", job.job_in_hand);
                    cmd.Parameters.AddWithValue("@due_date", job.due_date);
                    cmd.Parameters.AddWithValue("@finished_date", job.finished_date);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }catch(Exception ex)
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

        public List<JobQuotationModel> GetJobQuotations(string year)
        {
            List<JobQuotationModel> quots = new List<JobQuotationModel>();
            try
            {
                string string_command = string.Format($@"
                    SELECT
                        quotation_no,
                        customer,
                        product_type
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
                            type = dr["product_type"] != DBNull.Value ? dr["product_type"].ToString() : ""
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

        public string CreateTermPayment(Term_PaymentModel term_Payment)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO 
                        Term_Payment(job_id,
                             down_payment,
                             document_submit,
                             instrument_vendor,
                             instrument_delivered_ctl,
                             system_delivered_ctl,
                             fat,
                             delivery_instrument,
                             delivery_system,
                             progress_work,
                             installation_work_complete,               
                             commissioning,
                             startup,
                             as_built,
                             warranty,
                             finished
                        )
                        VALUES(@job_id,
                                @down_payment,
                                @document_submit,
                                @instrument_vendor,
                                @instrument_delivered_ctl,
                                @system_delivered_ctl,
                                @fat,
                                @delivery_instrument,
                                @delivery_system,
                                @progress_work,
                                @installation_work_complete,
                                @commissioning,
                                @startup,
                                @as_built,
                                @warranty,
                                @finished
                        )");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", term_Payment.job_id.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@down_payment", term_Payment.down_payment);
                    cmd.Parameters.AddWithValue("@document_submit", term_Payment.document_submit);
                    cmd.Parameters.AddWithValue("@instrument_vendor", term_Payment.instrument_vendor);
                    cmd.Parameters.AddWithValue("@instrument_delivered_ctl", term_Payment.instrument_delivered_ctl);
                    cmd.Parameters.AddWithValue("@system_delivered_ctl", term_Payment.system_delivered_ctl);
                    cmd.Parameters.AddWithValue("@fat", term_Payment.fat);
                    cmd.Parameters.AddWithValue("@delivery_instrument", term_Payment.delivery_instrument);
                    cmd.Parameters.AddWithValue("@delivery_system", term_Payment.delivery_system);
                    cmd.Parameters.AddWithValue("@progress_work", term_Payment.progress_work);
                    cmd.Parameters.AddWithValue("@installation_work_complete", term_Payment.installation_work_complete);
                    cmd.Parameters.AddWithValue("@commissioning", term_Payment.commissioning);
                    cmd.Parameters.AddWithValue("@startup", term_Payment.startup);
                    cmd.Parameters.AddWithValue("@as_built", term_Payment.as_built);
                    cmd.Parameters.AddWithValue("@warranty", term_Payment.warranty);
                    cmd.Parameters.AddWithValue("@finished", term_Payment.finished);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }catch(Exception ex)
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

        public string UpdateTermPayment(Term_PaymentModel term_Payment)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE Term_Payment 
                    SET
                        down_payment = @down_payment,
                        document_submit = @document_submit,
                        instrument_vendor = @instrument_vendor,
                        instrument_delivered_ctl = @instrument_delivered_ctl,
                        system_delivered_ctl = @system_delivered_ctl,
                        fat = @fat,
                        delivery_instrument = @delivery_instrument,
                        delivery_system = @delivery_system,
                        progress_work = @progress_work,
                        installation_work_complete = @installation_work_complete,
                        commissioning = @commissioning,
                        startup = @startup,
                        as_built = @as_built,
                        warranty = @warranty,
                        finished = @finished
                    WHERE job_id = @job_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", term_Payment.job_id.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@down_payment", term_Payment.down_payment);
                    cmd.Parameters.AddWithValue("@document_submit", term_Payment.document_submit);
                    cmd.Parameters.AddWithValue("@instrument_vendor", term_Payment.instrument_vendor);
                    cmd.Parameters.AddWithValue("@instrument_delivered_ctl", term_Payment.instrument_delivered_ctl);
                    cmd.Parameters.AddWithValue("@system_delivered_ctl", term_Payment.system_delivered_ctl);
                    cmd.Parameters.AddWithValue("@fat", term_Payment.fat);
                    cmd.Parameters.AddWithValue("@delivery_instrument", term_Payment.delivery_instrument);
                    cmd.Parameters.AddWithValue("@delivery_system", term_Payment.delivery_system);
                    cmd.Parameters.AddWithValue("@progress_work", term_Payment.progress_work);
                    cmd.Parameters.AddWithValue("@installation_work_complete", term_Payment.installation_work_complete);
                    cmd.Parameters.AddWithValue("@commissioning", term_Payment.commissioning);
                    cmd.Parameters.AddWithValue("@startup", term_Payment.startup);
                    cmd.Parameters.AddWithValue("@as_built", term_Payment.as_built);
                    cmd.Parameters.AddWithValue("@warranty", term_Payment.warranty);
                    cmd.Parameters.AddWithValue("@finished", term_Payment.finished);
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
