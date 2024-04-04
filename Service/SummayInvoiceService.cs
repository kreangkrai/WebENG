using System;
using System.Collections.Generic;
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
        public SummayInvoiceService()
        {
            Target = new TargetService();
        }

        public List<SummaryInvoiceModel> GetsENGInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = new List<SummaryInvoiceModel>();
            try
            {
                string stringCommand = string.Format($@"
                    SELECT month,SUM(invoice_eng) as invoice_eng FROM (
	                    select Invoice.job_id,
		                       Jobs.job_in_hand,
		                       Jobs.job_eng_in_hand,
		                       Invoice.invoice,
		                       CAST(((Invoice.invoice / Jobs.job_in_hand) * Jobs.job_eng_in_hand / 1000000) as DECIMAL(18,3)) as invoice_eng,
		                       FORMAT(invoice_date,'MMM') as month
	                    FROM Invoice
	                    LEFT JOIN Jobs ON Jobs.job_id = Invoice.job_id
	                    WHERE FORMAT(invoice_date,'yyyy') = '{year}'
	                ) as mian
	                GROUP BY month");

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
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return jobsSummaries;
        }

        public List<SummaryInvoiceModel> GetsSummaryENGInvoice(int year)
        {
            List<SummaryInvoiceModel> jobsSummaries = new List<SummaryInvoiceModel>();
            List<string> months = new List<string>();
            int count = 0;
            for (DateTime date = new DateTime(year, 1, 1); date <= DateTime.Now; date = date.AddMonths(1))
            {
                if (count == 12)
                {
                    break;
                }
                months.Add(date.ToString("MMM"));
                count++;
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
