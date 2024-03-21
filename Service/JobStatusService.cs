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
        public List<JobModel> GetJobStatusByUser(string user)
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
                        Eng_Status.Status_Name as status,
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
                    LEFT JOIN Eng_Status ON Jobs.status = Eng_Status.Status_ID
					WHERE Jobs.job_id IN (SELECT job_id FROM JobResponsible WHERE user_id = '{user}')
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
