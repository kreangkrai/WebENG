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
    public class QuotationSummaryService : IQuotationSummary
    {
        private IJob Job;
        ConnectSQL connect = null;
        SqlConnection con = null;
        public QuotationSummaryService()
        {
            Job = new JobService();
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<QuotationSummaryModel> GetQuotationSummaries()
        {
            List<JobSummaryModel> jobs = Job.GetJobsSummary();
            jobs = jobs.Where(w => w.user_id != "").ToList();

            List<QuotationSummaryModel> quotations = new List<QuotationSummaryModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT job_id,
	                                                        job_name,
	                                                        job_date,
	                                                        customer_name,
	                                                        enduser,
	                                                        sale,
	                                                        sale_department,
	                                                        job_type 
                                                        FROM Jobs WHERE job_id LIKE 'Q%'");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        QuotationSummaryModel quotation = new QuotationSummaryModel()
                        {
                            quotation = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            quotation_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
                            quotation_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            customer = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
                            end_user = dr["enduser"] != DBNull.Value ? dr["enduser"].ToString() : "",
                            sale_department = dr["sale_department"] != DBNull.Value ? dr["sale_department"].ToString() : "",
                            sale_name = dr["sale"] != DBNull.Value ? dr["sale"].ToString() : ""
                        };
                        quotation.engineers = jobs.Where(w => w.jobId == quotation.quotation).Select(s => new ENGQuotationSummaryModel()
                        {
                            name = s.user_id,
                            total_manhour = s.totalManhour
                        }).ToList();

                        quotations.Add(quotation);
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
            return quotations;
        }
    }
}
