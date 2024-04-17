using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;

namespace WebENG.Service
{
    public class JobStatusService : IJobStatus
    {
        readonly IJob Job;
        public JobStatusService()
        {
            Job = new JobService();
        }
        public List<JobModel> GetJobStatusALL()
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

                List<JobSummaryModel> jobSummaries = Job.GetJobsSummary();

                string string_command = string.Format($@"
                    SELECT
                        Jobs.job_id,
                        Jobs.job_name,
                        Jobs.job_date,
                        Jobs.job_type,
                        Jobs.customer_name,
                        Jobs.cost,
                        Jobs.process_id,
                        Jobs.system_id,
                        Jobs.md_rate,
                        Jobs.pd_rate,
                        Eng_Status.Status_Name as status,
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
                        Jobs.finished_date
                    FROM Jobs
                    LEFT JOIN Eng_Status ON Jobs.status = Eng_Status.Status_ID
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

                        List<JobSummaryModel> jobSummary = jobSummaries.Where(w => w.jobId == dr["job_id"].ToString()).ToList();

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
                            finished  = dr["finished"] != DBNull.Value ? Convert.ToInt32(dr["finished"]) : 0,
                        };

                        JobModel job = new JobModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            job_date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            customer = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
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
                            job_summary = jobSummary,
                            due_date = dr["due_date"] != DBNull.Value ? Convert.ToDateTime(dr["due_date"].ToString()) : DateTime.MinValue,
                            quotation_no = dr["quotation_no"] != DBNull.Value ? dr["quotation_no"].ToString() : "",
                            finished_date = dr["finished_date"] != DBNull.Value ? Convert.ToDateTime(dr["finished_date"].ToString()) : DateTime.MinValue
                        };
                        job.factor = job.md_rate + job.pd_rate;
                        job.term_payment = term_Payment;
                        jobs.Add(job);
                    }
                    dr.Close();
                }
            }
            catch
            {
                return null;
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

        public List<JobModel> GetJobStatusByUser(string user)
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

                List<JobSummaryModel> jobSummaries = Job.GetJobsSummary();

                string string_command = string.Format($@"
                    SELECT
                        Jobs.job_id,
                        Jobs.job_name,
                        Jobs.job_date,
                        Jobs.job_type,
                        Jobs.customer_name,
                        Jobs.cost,
                        Jobs.process_id,
                        Jobs.system_id,
                        Jobs.md_rate,
                        Jobs.pd_rate,
                        Eng_Status.Status_Name as status,
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
                        Jobs.finished_date
                    FROM Jobs
                    LEFT JOIN Eng_Status ON Jobs.status = Eng_Status.Status_ID
					LEFT JOIN Term_Payment ON Term_Payment.job_id = Jobs.job_id
                    WHERE Jobs.job_id IN (SELECT job_id FROM JobResponsible WHERE user_id = '{user}')
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

                        List<JobSummaryModel> jobSummary = jobSummaries.Where(w => w.jobId == dr["job_id"].ToString()).ToList();

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
                            finished = dr["finished"] != DBNull.Value ? Convert.ToInt32(dr["finished"]) : 0,
                        };

                        JobModel job = new JobModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            cost = dr["cost"] != DBNull.Value ? Convert.ToInt32(dr["cost"]) : 0,
                            job_date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            customer = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
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
                            job_summary = jobSummary,
                            due_date = dr["due_date"] != DBNull.Value ? Convert.ToDateTime(dr["due_date"].ToString()) : DateTime.MinValue,
                            quotation_no = dr["quotation_no"] != DBNull.Value ? dr["quotation_no"].ToString() : "",
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

        public string UpdateJobByProcessSystem(JobModel job)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE Jobs 
                    SET
                        process_id = @process,
                        system_id = @system
                    WHERE job_id = @job_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job.job_id);
                    cmd.Parameters.AddWithValue("@process", job.process);
                    cmd.Parameters.AddWithValue("@system", job.system);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
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

        public string UpdateJobStatus(string job, string status)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE Jobs 
                    SET
                        status = @status
                    WHERE job_id = @job_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job.Replace("-", String.Empty));
                    cmd.Parameters.AddWithValue("@status", status);
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
