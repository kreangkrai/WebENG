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
    public class SummayInvoiceService : ISummaryInvoice
    {
        readonly ITarget Target;
        ConnectSQL connect = null;
        SqlConnection con = null;
        public SummayInvoiceService()
        {
            Target = new TargetService();
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }

        public List<SummaryInvoiceModel> GetsAISInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = new List<SummaryInvoiceModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string stringCommand = string.Format($@"
                    SELECT month,SUM(invoice_ais) as invoice_ais FROM (
	                    select Invoice.job_id,
		                       Jobs.job_in_hand,
		                       Jobs.job_ais_in_hand,
		                       Invoice.invoice,
		                       CAST(((Invoice.invoice / Jobs.job_in_hand) * Jobs.job_ais_in_hand / 1000000) as DECIMAL(18,3)) as invoice_ais,
		                       FORMAT(actual_date,'MMM') as month
	                    FROM Invoice
	                    LEFT JOIN Jobs ON Jobs.job_id = Invoice.job_id
	                    WHERE FORMAT(actual_date,'yyyy') = '{year}'
	                ) as mian
	                GROUP BY month");

                SqlCommand cmd = new SqlCommand(stringCommand, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        SummaryInvoiceModel jobSummary = new SummaryInvoiceModel()
                        {
                            month = dr["month"] != DBNull.Value ? dr["month"].ToString() : "",
                            invoice = dr["invoice_ais"] != DBNull.Value ? Convert.ToDouble(dr["invoice_ais"]) : 0
                        };
                        jobsSummaries.Add(jobSummary);
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
            return jobsSummaries;
        }

        public List<SummaryInvoiceModel> GetsCISInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = new List<SummaryInvoiceModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string stringCommand = string.Format($@"
                    SELECT month,SUM(invoice_cis) as invoice_cis FROM (
	                    select Invoice.job_id,
		                       Jobs.job_in_hand,
		                       Jobs.job_cis_in_hand,
		                       Invoice.invoice,
		                       CAST(((Invoice.invoice / Jobs.job_in_hand) * Jobs.job_cis_in_hand / 1000000) as DECIMAL(18,3)) as invoice_cis,
		                       FORMAT(actual_date,'MMM') as month
	                    FROM Invoice
	                    LEFT JOIN Jobs ON Jobs.job_id = Invoice.job_id
	                    WHERE FORMAT(actual_date,'yyyy') = '{year}'
	                ) as mian
	                GROUP BY month");

                SqlCommand cmd = new SqlCommand(stringCommand, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        SummaryInvoiceModel jobSummary = new SummaryInvoiceModel()
                        {
                            month = dr["month"] != DBNull.Value ? dr["month"].ToString() : "",
                            invoice = dr["invoice_cis"] != DBNull.Value ? Convert.ToDouble(dr["invoice_cis"]) : 0
                        };
                        jobsSummaries.Add(jobSummary);
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
            return jobsSummaries;
        }

        public List<SummaryInvoiceModel> GetsENGInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = new List<SummaryInvoiceModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string stringCommand = string.Format($@"
                    SELECT month,SUM(invoice_eng) as invoice_eng FROM (
	                    select Invoice.job_id,
		                       Jobs.job_in_hand,
		                       Jobs.job_eng_in_hand,
		                       Invoice.invoice,
		                       CAST(((Invoice.invoice / Jobs.job_in_hand) * Jobs.job_eng_in_hand / 1000000) as DECIMAL(18,3)) as invoice_eng,
		                       FORMAT(actual_date,'MMM') as month
	                    FROM Invoice
	                    LEFT JOIN Jobs ON Jobs.job_id = Invoice.job_id
	                    WHERE FORMAT(actual_date,'yyyy') = '{year}'
	                ) as mian
	                GROUP BY month");

                SqlCommand cmd = new SqlCommand(stringCommand, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        SummaryInvoiceModel jobSummary = new SummaryInvoiceModel()
                        {
                            month = dr["month"] != DBNull.Value ? dr["month"].ToString() : "",
                            invoice = dr["invoice_eng"] != DBNull.Value ? Convert.ToDouble(dr["invoice_eng"]) : 0
                        };
                        jobsSummaries.Add(jobSummary);
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
            return jobsSummaries;
        }

        public List<SummaryInvoiceModel> GetsSummaryAccAISInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = GetsSummaryAISInvoice(year);
            List<SummaryInvoiceModel> acc_invoices = new List<SummaryInvoiceModel>();
            double acc = 0;
            double acc_target = 0;
            for (int i = 0; i < jobsSummaries.Count; i++)
            {
                acc += jobsSummaries[i].invoice;
                acc_target += jobsSummaries[i].target_month;
                acc_invoices.Add(new SummaryInvoiceModel()
                {
                    invoice = acc,
                    month = jobsSummaries[i].month,
                    target_month = acc_target
                });
            }
            return acc_invoices;
        }

        public List<SummaryInvoiceModel> GetsSummaryAccCISInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = GetsSummaryCISInvoice(year);
            List<SummaryInvoiceModel> acc_invoices = new List<SummaryInvoiceModel>();
            double acc = 0;
            double acc_target = 0;
            for (int i = 0; i < jobsSummaries.Count; i++)
            {
                acc += jobsSummaries[i].invoice;
                acc_target += jobsSummaries[i].target_month;
                acc_invoices.Add(new SummaryInvoiceModel()
                {
                    invoice = acc,
                    month = jobsSummaries[i].month,
                    target_month = acc_target
                });
            }
            return acc_invoices;
        }

        public List<SummaryInvoiceModel> GetsSummaryAccENGInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = GetsSummaryENGInvoice(year);
            List<SummaryInvoiceModel> acc_invoices = new List<SummaryInvoiceModel>();
            double acc = 0;
            double acc_target = 0;
            for (int i = 0; i < jobsSummaries.Count; i++)
            {
                acc += jobsSummaries[i].invoice;
                acc_target += jobsSummaries[i].target_month;
                acc_invoices.Add(new SummaryInvoiceModel()
                {
                    invoice = acc,
                    month = jobsSummaries[i].month,
                    target_month = acc_target
                });
            }
            return acc_invoices;
        }

        public List<SummaryInvoiceModel> GetsSummaryAISInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = new List<SummaryInvoiceModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryInvoiceModel> getSummaries = GetsAISInvoice(year);
            List<TargetModel> targets = Target.getData(year, "Invoice");
            for (int i = 0; i < months.Count; i++)
            {
                var invoice = getSummaries.Where(w => w.month == months[i]).Select(s => s.invoice).FirstOrDefault();
                jobsSummaries.Add(new SummaryInvoiceModel()
                {
                    invoice = invoice,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryInvoiceModel> GetsSummaryCISInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = new List<SummaryInvoiceModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryInvoiceModel> getSummaries = GetsCISInvoice(year);
            List<TargetModel> targets = Target.getData(year, "Invoice");
            for (int i = 0; i < months.Count; i++)
            {
                var invoice = getSummaries.Where(w => w.month == months[i]).Select(s => s.invoice).FirstOrDefault();
                jobsSummaries.Add(new SummaryInvoiceModel()
                {
                    invoice = invoice,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryInvoiceModel> GetsSummaryENGInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = new List<SummaryInvoiceModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryInvoiceModel> getSummaries = GetsENGInvoice(year);
            List<TargetModel> targets = Target.getData(year, "Invoice");
            for (int i = 0; i < months.Count; i++)
            {
                var invoice = getSummaries.Where(w => w.month == months[i]).Select(s => s.invoice).FirstOrDefault();
                jobsSummaries.Add(new SummaryInvoiceModel()
                {
                    invoice = invoice,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }
    }
}
