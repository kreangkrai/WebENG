using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;

namespace WebENG.Service
{
    public class InvoiceService : IInvoice
    {
        public string Delete(string job)
        {
            try
            {
                string string_command = string.Format($@"DELETE FROM Invoice WHERE job_id = @job_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@job_id", job);
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

        public List<InvoiceModel> GetByJob(string job)
        {
            List<InvoiceModel> invoices = new List<InvoiceModel>();
            try
            {
                string string_command = string.Format($@"SELECT job_id,
                                                                milestone,
                                                                milestone_order,
                                                                invoice,
                                                                plan_date,
                                                                actual_date,
                                                                status,
                                                                remark,
                                                                new_plan_date
                                                          FROM Invoice WHERE job_id='{job}'");
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
                        InvoiceModel invoice = new InvoiceModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            milestone = dr["milestone"] != DBNull.Value ? dr["milestone"].ToString() : "",
                            milestone_order = dr["milestone_order"] != DBNull.Value ? Convert.ToInt32(dr["milestone_order"].ToString()) : 0,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0.0,
                            plan_date = dr["plan_date"] != DBNull.Value ? Convert.ToDateTime(dr["plan_date"].ToString()) : DateTime.MinValue,
                            actual_date = dr["actual_date"] != DBNull.Value ? Convert.ToDateTime(dr["actual_date"].ToString()) : DateTime.MinValue,
                            new_plan_date = dr["new_plan_date"] != DBNull.Value ? Convert.ToDateTime(dr["new_plan_date"].ToString()) : DateTime.MinValue,
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "",
                            remark = dr["remark"] != DBNull.Value ? dr["remark"].ToString() : ""
                        };
                        invoices.Add(invoice);
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
            return invoices;
        }

        public string Insert(List<InvoiceModel> invoices)
        {
            try
            {
                for (int i = 0; i < invoices.Count; i++)
                {
                    string string_command = string.Format($@"
                    INSERT INTO Invoice(job_id,
                                        milestone,
                                        milestone_order,
                                        invoice,
                                        plan_date,
                                        actual_date,
                                        status,
                                        remark,
                                        new_plan_date)
                    VALUES(@job_id,
                            @milestone,
                            @milestone_order,
                            @invoice,
                            @plan_date,
                            @actual_date,
                            @status,
                            @remark,
                            @new_plan_date)");
                    using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.AddWithValue("@job_id", invoices[i].job_id.Replace("-",String.Empty));
                        cmd.Parameters.AddWithValue("@milestone", invoices[i].milestone);
                        cmd.Parameters.AddWithValue("@milestone_order", invoices[i].milestone_order);
                        cmd.Parameters.AddWithValue("@invoice", invoices[i].invoice);
                        cmd.Parameters.AddWithValue("@plan_date", invoices[i].plan_date);
                        cmd.Parameters.AddWithValue("@actual_date", invoices[i].actual_date);
                        cmd.Parameters.AddWithValue("@status", invoices[i].status);
                        cmd.Parameters.AddWithValue("@remark", invoices[i].remark);
                        cmd.Parameters.AddWithValue("@new_plan_date", invoices[i].new_plan_date);
                        if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                        {
                            ConnectSQL.CloseConnect();
                            ConnectSQL.OpenConnect();
                        }
                        cmd.ExecuteNonQuery();
                    }
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
