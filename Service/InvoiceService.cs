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
                string string_command = string.Format($@"SELECT job_id,invocie,invoice_date FROM Invoice WHERE job_id='{job}'");
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
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToInt32(dr["invoice"]) : 0,
                            invoice_date = dr["invoice_date"] != DBNull.Value ? Convert.ToDateTime(dr["invoice_date"].ToString()) : DateTime.MinValue,
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
                    INSERT INTO Invoice(job_id, invoice, invoice_date)
                    VALUES(@job_id, @invoice, @invoice_date)");
                    using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.AddWithValue("@job_id", invoices[i].job_id.Replace("-",String.Empty));
                        cmd.Parameters.AddWithValue("@invoice", invoices[i].invoice);
                        cmd.Parameters.AddWithValue("@invoice_date", invoices[i].invoice_date);
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
