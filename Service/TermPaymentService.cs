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
    public class TermPaymentService : ITermPayment
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public TermPaymentService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public Term_PaymentModel GetByJob(string job)
        {
            Term_PaymentModel term_Payment = new Term_PaymentModel();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader dr = null;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string command = string.Format($@"SELECT * FROM Term_Payment WHERE job_id='{job}'");
                List<InvoiceModel> invoices = new List<InvoiceModel>();
                cmd = new SqlCommand(command, con);
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        term_Payment = new Term_PaymentModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            down_payment = dr["down_payment"] != DBNull.Value ? Convert.ToInt32(dr["down_payment"]) : 0,
                            document_submit = dr["down_payment"] != DBNull.Value ? Convert.ToInt32(dr["down_payment"]) : 0,
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
                            after_hmc = dr["after_hmc"] != DBNull.Value ? Convert.ToInt32(dr["after_hmc"]) : 0,
                            complete = dr["complete"] != DBNull.Value ? Convert.ToInt32(dr["complete"]) : 0,
                        };
                    }
                }
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return term_Payment;
        }

        public string Insert(Term_PaymentModel term_Payment)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    INSERT INTO Term_Payment(job_id,
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
                                            finished,
                                            complete,
                                            after_hmc
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
                                            @finished,
                                            @complete,
                                            @after_hmc
                                            )");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", term_Payment.job_id);
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
                    cmd.Parameters.AddWithValue("@complete", term_Payment.complete);
                    cmd.Parameters.AddWithValue("@after_hmc", term_Payment.after_hmc);
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

        public string Update(Term_PaymentModel term_Payment)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
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
                        finished = @finished,
                        complete = @complete
                        after_hmc = @after_hmc
                    WHERE job_id = @job_id");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.Parameters.AddWithValue("@job_id", term_Payment.job_id);
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
                    cmd.Parameters.AddWithValue("@complete", term_Payment.complete);
                    cmd.Parameters.AddWithValue("@after_hmc", term_Payment.after_hmc);
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
