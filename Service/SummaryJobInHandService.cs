using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Interface;
using WebENG.Models;

namespace WebENG.Service
{
    public class SummaryJobInHandService : ISummaryJobInHand
    {
        readonly ITarget Target;
        public SummaryJobInHandService()
        {
            Target = new TargetService();
        }
        public List<SummaryJobInHandModel> GetsAccJobInHand(int year,string type)
        {
            List<SummaryJobInHandModel> jobsSummaries = new List<SummaryJobInHandModel>();
            List<string> months = new List<string>();
            int count = 0;
            for (DateTime date = new DateTime (year,1,1); date <= DateTime.Now; date = date.AddMonths(1))
            {
                if(count == 12)
                {
                    break;
                }
                months.Add(date.ToString("MMM"));
                count++;
            }
            List<SummaryJobInHandModel> getSummaries = GetsSummaryJobInHand(year, type);
            List<TargetModel> targets_project = Target.getData(year, "Project");
            List<TargetModel> targets_service = Target.getData(year, "Service").Union(targets_project).ToList();
            List<TargetModel> targets = targets_service.GroupBy(g => g.month).Select(s => new TargetModel()
            {
                month = s.Key,
                target = s.Sum(k=>k.target)
            }).ToList();
            double acc = 0;
            double acc_target = 0;
            for(int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_eng_in_hand).FirstOrDefault();
                var target = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault();
                acc += job_in_hand;
                acc_target += target;
                jobsSummaries.Add(new SummaryJobInHandModel()
                {
                    job_eng_in_hand = acc,
                    month = months[i],
                    target_month = acc_target
                });               
            }
            return jobsSummaries;
        }

        public List<JobInhandModel> GetsJobBackLog(int year)
        {
            List<JobInhandModel> jobsSummaries = new List<JobInhandModel>();
            try
            {
                string stringCommand = string.Format($@"
                    select Jobs.job_id,
		                Jobs.job_name,
		                Jobs.customer_name,
		                Jobs.job_type,
		                Jobs.job_in_hand,
		                Jobs.job_eng_in_hand,
		                case when Invoice.invoice is null then 0 else Invoice.invoice end as invoice,	
	                    CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_eng_in_hand) as decimal(18,2)) as invoice_eng,
	                    (100.0 - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * 100) as decimal(18,2))) as remaining_percent_invoice,
	                    CAST((job_in_hand - case when Invoice.invoice is null then 0 else Invoice.invoice end) as decimal(18,2)) as remaining_amount
                from Jobs
                LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(invoice_date,'yyyy') = '{year}' GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
                where (status <>'STA999' AND (FORMAT(job_date ,'yyyy') < '{year}' OR job_date is null)) OR (FORMAT(finished_date,'yyyy') = '{year}' AND FORMAT(job_date ,'yyyy') < '{year}')");

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
                        JobInhandModel jobSummary = new JobInhandModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            customer_name = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0,
                            job_eng_in_hand = dr["job_eng_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_eng_in_hand"]) : 0,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0,
                            invoice_eng = dr["invoice_eng"] != DBNull.Value ? Convert.ToDouble(dr["invoice_eng"]) : 0,
                            remaining_percent_invoice = dr["remaining_percent_invoice"] != DBNull.Value ? Convert.ToDouble(dr["remaining_percent_invoice"]) : 0,
                            remaining_amount = dr["remaining_amount"] != DBNull.Value ? Convert.ToDouble(dr["remaining_amount"]) : 0
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

        public List<JobInhandModel> GetsJobInhand(int year)
        {
            List<JobInhandModel> jobsSummaries = new List<JobInhandModel>();
            try
            {
                string stringCommand = string.Format($@"
                    select Jobs.job_id,
	                    customer_name,
	                    job_name,
	                    job_type,
	                    job_in_hand,
	                    job_eng_in_hand,
	                    case when Invoice.invoice is null then 0 else Invoice.invoice end as invoice,	
	                    CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_eng_in_hand) as decimal(18,2)) as invoice_eng,
	                    (100.0 - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * 100) as decimal(18,2))) as remaining_percent_invoice,
	                    CAST((job_in_hand - case when Invoice.invoice is null then 0 else Invoice.invoice end) as decimal(18,2)) as remaining_amount
                    from Jobs 
                    LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(invoice_date,'yyyy') = '{year}' GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
                    where FORMAT(job_date,'yyyy') = '{year}'");
                
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
                        JobInhandModel jobSummary = new JobInhandModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            customer_name = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0,
                            job_eng_in_hand = dr["job_eng_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_eng_in_hand"]) : 0,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0,
                            invoice_eng = dr["invoice_eng"] != DBNull.Value ? Convert.ToDouble(dr["invoice_eng"]) : 0,
                            remaining_percent_invoice = dr["remaining_percent_invoice"] != DBNull.Value ? Convert.ToDouble(dr["remaining_percent_invoice"]) : 0,
                            remaining_amount = dr["remaining_amount"] != DBNull.Value ? Convert.ToDouble(dr["remaining_amount"]) : 0
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

        public List<SummaryJobInHandModel> GetsProjectJobInHand(int year, string type)
        {
            List<SummaryJobInHandModel> jobsSummaries = new List<SummaryJobInHandModel>();
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
            List<SummaryJobInHandModel> getSummaries = GetsSummaryJobInHand(year, type);
            List<TargetModel> targets = Target.getData(year, "Project");
            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_eng_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryJobInHandModel()
                {
                    job_eng_in_hand = job_in_hand,
                    month = months[i],
                    target_month = targets.Where(w=>Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s=>s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryJobInHandModel> GetsServiceJobInHand(int year, string type)
        {
            List<SummaryJobInHandModel> jobsSummaries = new List<SummaryJobInHandModel>();
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
            List<SummaryJobInHandModel> getSummaries = GetsSummaryJobInHand(year, type);
            List<TargetModel> targets = Target.getData(year, "Service");
            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_eng_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryJobInHandModel()
                {
                    job_eng_in_hand = job_in_hand,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryJobInHandModel> GetsSummaryJobInHand(int year,string type)
        {
            List<SummaryJobInHandModel> jobsSummaries = new List<SummaryJobInHandModel>();
            try
            {
                string stringCommand = "";
                if (type == "ALL")
                {
                    stringCommand = string.Format($@"
                    select t1.Month,
	                    SUM(case when t1.job_eng_in_hand is null then 0.00 else FORMAT((t1.job_eng_in_hand / 1000000),'N2') end) as job_eng_in_hand 
                    from 
	                    (
		                    select job_eng_in_hand,FORMAT(job_date,'MMM') as Month 
		                    from jobs where FORMAT(job_date,'yyyy') = '{year}'
	                    ) as t1
                    group by t1.Month");
                }
                else
                {
                    stringCommand = string.Format($@"
                    select t1.Month,
	                    SUM(case when t1.job_eng_in_hand is null then 0.00 else FORMAT((t1.job_eng_in_hand / 1000000),'N2') end) as job_eng_in_hand 
                    from 
	                    (
		                    select job_eng_in_hand,FORMAT(job_date,'MMM') as Month 
		                    from jobs where FORMAT(job_date,'yyyy') = '{year}' AND job_type = '{type}'
	                    ) as t1
                    group by t1.Month");
                }
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
                        SummaryJobInHandModel jobSummary = new SummaryJobInHandModel()
                        {
                            month = dr["month"] != DBNull.Value ? dr["month"].ToString() : "",
                            job_eng_in_hand = dr["job_eng_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_eng_in_hand"]) : 0
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
    }
}
