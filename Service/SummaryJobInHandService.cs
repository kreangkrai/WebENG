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
        readonly IJobStatus JobStatus;
        public SummaryJobInHandService()
        {
            Target = new TargetService();
            JobStatus = new JobStatusService();
        }

        public OrderInTakeAISModel GetOrderAISInTake(int year)
        {
            OrderInTakeAISModel orderInTake = new OrderInTakeAISModel();
            List<QuarterAISModel> jobs = new List<QuarterAISModel>();
            try
            {
                string stringCommand = string.Format($@"
                select  Jobs.job_id,
						job_date,
                        job_type,
						case when FORMAT(job_date,'yyyy') < {year} OR job_date is null then 'backlog' else 'now' end as type,
						CAST((Jobs.job_ais_in_hand / 1000000) as decimal(18,3)) as job_ais_in_hand,
                        CAST((((case when backlog_invoice.invoice is null then 0 else backlog_invoice.invoice end / NULLIF(job_in_hand,0)) * job_ais_in_hand) / 1000000) as decimal(18,3)) as backlog_invoice_ais,
					    Eng_Status.status_name as status,
				        Jobs.finished_date
				from Jobs
				LEFT JOIN Eng_Status ON Eng_Status.status_id = Jobs.status
                LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') < {year} GROUP BY job_id) as backlog_invoice ON backlog_invoice.job_id = Jobs.job_id");

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
                        QuarterAISModel job = new QuarterAISModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            type = dr["type"] != DBNull.Value ? dr["type"].ToString() : "",
                            job_ais_in_hand = dr["job_ais_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_ais_in_hand"]) : 0,
                            backlog_invoice_ais = dr["backlog_invoice_ais"] != DBNull.Value ? Convert.ToDouble(dr["backlog_invoice_ais"]) : 0,
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "",
                            finished_date = dr["finished_date"] != DBNull.Value ? Convert.ToDateTime(dr["finished_date"].ToString()) : DateTime.MinValue,
                        };
                        jobs.Add(job);
                    }
                    dr.Close();
                }

                double backlog_volume_ais = jobs.Where(w => w.type == "backlog" && w.job_date.Year < year).Select(s => s.job_ais_in_hand).Sum() - jobs.Where(w => w.type == "backlog").Select(s => s.backlog_invoice_ais).Sum();
                double job_ais_in_hand_volume = jobs.Where(w => w.type == "now").Select(s => s.job_ais_in_hand).Sum();
                orderInTake = new OrderInTakeAISModel()
                {
                    year = year,
                    backlog = backlog_volume_ais,
                    job_ais_in_hand = job_ais_in_hand_volume,
                    target = 200
                };
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return orderInTake;
        }

        public OrderInTakeCISModel GetOrderCISInTake(int year)
        {
            OrderInTakeCISModel orderInTake = new OrderInTakeCISModel();
            List<QuarterCISModel> jobs = new List<QuarterCISModel>();
            try
            {
                string stringCommand = string.Format($@"
                select  Jobs.job_id,
						job_date,
                        job_type,
						case when FORMAT(job_date,'yyyy') < {year} OR job_date is null then 'backlog' else 'now' end as type,
						CAST((Jobs.job_cis_in_hand / 1000000) as decimal(18,3)) as job_cis_in_hand,
                        CAST((((case when backlog_invoice.invoice is null then 0 else backlog_invoice.invoice end / NULLIF(job_in_hand,0)) * job_cis_in_hand) / 1000000) as decimal(18,3)) as backlog_invoice_cis,
					    Eng_Status.status_name as status,
				        Jobs.finished_date
				from Jobs
				LEFT JOIN Eng_Status ON Eng_Status.status_id = Jobs.status
                LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') < {year} GROUP BY job_id) as backlog_invoice ON backlog_invoice.job_id = Jobs.job_id");

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
                        QuarterCISModel job = new QuarterCISModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            type = dr["type"] != DBNull.Value ? dr["type"].ToString() : "",
                            job_cis_in_hand = dr["job_cis_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_cis_in_hand"]) : 0,
                            backlog_invoice_cis = dr["backlog_invoice_cis"] != DBNull.Value ? Convert.ToDouble(dr["backlog_invoice_cis"]) : 0,
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "",
                            finished_date = dr["finished_date"] != DBNull.Value ? Convert.ToDateTime(dr["finished_date"].ToString()) : DateTime.MinValue,
                        };
                        jobs.Add(job);
                    }
                    dr.Close();
                }

                double backlog_volume_cis = jobs.Where(w => w.type == "backlog" && w.job_date.Year < year).Select(s => s.job_cis_in_hand).Sum() - jobs.Where(w => w.type == "backlog").Select(s => s.backlog_invoice_cis).Sum();
                double job_cis_in_hand_volume = jobs.Where(w => w.type == "now").Select(s => s.job_cis_in_hand).Sum();
                orderInTake = new OrderInTakeCISModel()
                {
                    year = year,
                    backlog = backlog_volume_cis,
                    job_cis_in_hand = job_cis_in_hand_volume,
                    target = 200
                };
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return orderInTake;
        }

        public OrderInTakeENGModel GetOrderENGInTake(int year)
        {
            OrderInTakeENGModel orderInTake = new OrderInTakeENGModel();
            List<QuarterENGModel> jobs = new List<QuarterENGModel>();
            try
            {
                string stringCommand = string.Format($@"
                select  Jobs.job_id,
						job_date,
                        job_type,
						case when FORMAT(job_date,'yyyy') < {year} OR job_date is null then 'backlog' else 'now' end as type,
						CAST((Jobs.job_eng_in_hand / 1000000) as decimal(18,3)) as job_eng_in_hand,
                        CAST((((case when backlog_invoice.invoice is null then 0 else backlog_invoice.invoice end / NULLIF(job_in_hand,0)) * job_eng_in_hand) / 1000000) as decimal(18,3)) as backlog_invoice_eng,
					    Eng_Status.status_name as status,
				        Jobs.finished_date
				from Jobs
				LEFT JOIN Eng_Status ON Eng_Status.status_id = Jobs.status
                LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') < {year} GROUP BY job_id) as backlog_invoice ON backlog_invoice.job_id = Jobs.job_id");

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
                        QuarterENGModel job = new QuarterENGModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            type = dr["type"] != DBNull.Value ? dr["type"].ToString() : "",
                            job_eng_in_hand = dr["job_eng_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_eng_in_hand"]) : 0,
                            backlog_invoice_eng = dr["backlog_invoice_eng"] != DBNull.Value ? Convert.ToDouble(dr["backlog_invoice_eng"]) : 0,
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "",
                            finished_date = dr["finished_date"] != DBNull.Value ? Convert.ToDateTime(dr["finished_date"].ToString()) : DateTime.MinValue,
                        };
                        jobs.Add(job);
                    }
                    dr.Close();
                }

                double backlog_volume_eng = jobs.Where(w => w.type == "backlog" && w.job_date.Year < year).Select(s => s.job_eng_in_hand).Sum() - jobs.Where(w => w.type == "backlog").Select(s => s.backlog_invoice_eng).Sum();
                double job_eng_in_hand_volume = jobs.Where(w => w.type == "now").Select(s => s.job_eng_in_hand).Sum();
                orderInTake = new OrderInTakeENGModel()
                {
                    year = year,
                    backlog = backlog_volume_eng,
                    job_eng_in_hand = job_eng_in_hand_volume,
                    target = 200
                };
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return orderInTake;
        }
        public List<SummaryAISJobInHandModel> GetsAccAISJobInHand(int year, string type)
        {
            List<SummaryAISJobInHandModel> jobsSummaries = new List<SummaryAISJobInHandModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryAISJobInHandModel> getSummaries = GetsSummaryAISJobInHand(year, type);
            List<TargetModel> targets_project = Target.getData(year, "Project");
            List<TargetModel> targets_service = Target.getData(year, "Service").Union(targets_project).ToList();
            List<TargetModel> targets = targets_service.GroupBy(g => g.month).Select(s => new TargetModel()
            {
                month = s.Key,
                target = s.Sum(k => k.target)
            }).ToList();
            double acc_ais = 0;
            double acc_target = 0;
            for (int i = 0; i < months.Count; i++)
            {
                var job_ais_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_ais_in_hand).FirstOrDefault();
                var target = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault();
                acc_ais += job_ais_in_hand;
                acc_target += target;
                jobsSummaries.Add(new SummaryAISJobInHandModel()
                {
                    job_ais_in_hand = acc_ais,
                    month = months[i],
                    target_month = acc_target
                });
            }
            return jobsSummaries;
        }

        public List<SummaryCISJobInHandModel> GetsAccCISJobInHand(int year, string type)
        {
            List<SummaryCISJobInHandModel> jobsSummaries = new List<SummaryCISJobInHandModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryCISJobInHandModel> getSummaries = GetsSummaryCISJobInHand(year, type);
            List<TargetModel> targets_project = Target.getData(year, "Project");
            List<TargetModel> targets_service = Target.getData(year, "Service").Union(targets_project).ToList();
            List<TargetModel> targets = targets_service.GroupBy(g => g.month).Select(s => new TargetModel()
            {
                month = s.Key,
                target = s.Sum(k => k.target)
            }).ToList();
            double acc_cis = 0;
            double acc_target = 0;
            for (int i = 0; i < months.Count; i++)
            {
                var job_cis_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_cis_in_hand).FirstOrDefault();
                var target = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault();
                acc_cis += job_cis_in_hand;
                acc_target += target;
                jobsSummaries.Add(new SummaryCISJobInHandModel()
                {
                    job_cis_in_hand = acc_cis,
                    month = months[i],
                    target_month = acc_target
                });
            }
            return jobsSummaries;
        }

        public List<SummaryENGJobInHandModel> GetsAccENGJobInHand(int year, string type)
        {
            List<SummaryENGJobInHandModel> jobsSummaries = new List<SummaryENGJobInHandModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryENGJobInHandModel> getSummaries = GetsSummaryENGJobInHand(year, type);
            List<TargetModel> targets_project = Target.getData(year, "Project");
            List<TargetModel> targets_service = Target.getData(year, "Service").Union(targets_project).ToList();
            List<TargetModel> targets = targets_service.GroupBy(g => g.month).Select(s => new TargetModel()
            {
                month = s.Key,
                target = s.Sum(k => k.target)
            }).ToList();
            double acc_eng = 0;
            double acc_target = 0;
            for (int i = 0; i < months.Count; i++)
            {
                var job_eng_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_eng_in_hand).FirstOrDefault();
                var target = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault();
                acc_eng += job_eng_in_hand;
                acc_target += target;
                jobsSummaries.Add(new SummaryENGJobInHandModel()
                {
                    job_eng_in_hand = acc_eng,
                    month = months[i],
                    target_month = acc_target
                });
            }
            return jobsSummaries;
        }
        public List<JobAISInhandModel> GetsAISJobBackLog(int year)
        {
            List<JobAISInhandModel> jobsSummaries = new List<JobAISInhandModel>();
            try
            {
                string stringCommand = string.Format($@"
                    select Jobs.job_id,
		                Jobs.job_name,
		                Jobs.customer_name,
		                Jobs.job_type,
		                Jobs.job_in_hand,
		                Jobs.job_ais_in_hand,
		                case when Invoice.invoice is null then 0 else Invoice.invoice end as invoice,	
	                    CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_ais_in_hand) as decimal(18,2)) as invoice_ais,
	                    (100.0 - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * 100) as decimal(18,3))) as remaining_percent_invoice,
	                    CAST((( CASE WHEN job_ais_in_hand IS NULL THEN 0 ELSE job_ais_in_hand END ) - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * (CASE WHEN job_ais_in_hand IS NULL THEN 0 ELSE job_ais_in_hand END)) as decimal(18,3))) as decimal(18,3)) as remaining_amount
                from Jobs
                LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') <= '{year}' GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
                where FORMAT(job_date ,'yyyy') < '{year}' OR job_date is null");


                SqlCommand cmd = new SqlCommand(stringCommand, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    List<JobModel> _jobs = JobStatus.GetJobStatusALL();
                    while (dr.Read())
                    {
                        string job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "";
                        JobModel _job = _jobs.Where(w => w.job_id == job_id).FirstOrDefault();
                        double ais_cost = _job.job_summary.FirstOrDefault().ais_cost;
                        double percent_ais_cost = Math.Ceiling(_job.job_summary.Sum(s => s.totalCost) / ais_cost * 100);
                        double invoice_ais = _job.ais_invoice;
                        double percent_invoice_ais = Math.Ceiling(invoice_ais / _job.job_ais_in_hand * 100);
                        double remaining_amount = _job.job_in_hand - _job.invoices.Sum(s => s.invoice);
                        JobAISInhandModel jobSummary = new JobAISInhandModel()
                        {
                            job_id = job_id,
                            customer_name = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0,
                            job_ais_in_hand = dr["job_ais_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_ais_in_hand"]) : 0,
                            percent_ais_cost = percent_ais_cost,
                            percent_invoice = percent_invoice_ais,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0,
                            invoice_ais = dr["invoice_ais"] != DBNull.Value ? Convert.ToDouble(dr["invoice_ais"]) : 0,
                            remaining_percent_invoice = dr["remaining_percent_invoice"] != DBNull.Value ? Convert.ToDouble(dr["remaining_percent_invoice"]) : 0,
                            remaining_amount = Double.IsNaN(remaining_amount) ? 0 : remaining_amount
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

        public List<JobAISInhandModel> GetsAISJobInhand(int year)
        {
            List<JobAISInhandModel> jobsSummaries = new List<JobAISInhandModel>();
            try
            {
                string stringCommand = string.Format($@"
                    select Jobs.job_id,
	                    customer_name,
	                    job_name,
	                    job_type,
	                    job_in_hand,
	                    job_ais_in_hand,
	                    case when Invoice.invoice is null then 0 else Invoice.invoice end as invoice,	
	                    CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_ais_in_hand) as decimal(18,3)) as invoice_ais,
	                    (100.0 - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * 100) as decimal(18,3))) as remaining_percent_invoice,
	                    CAST((job_cis_in_hand - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_ais_in_hand) as decimal(18,3))) as decimal(18,3)) as remaining_amount
                    from Jobs 
                    LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') = '{year}' GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
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
                    List<JobModel> _jobs = JobStatus.GetJobStatusALL();
                    while (dr.Read())
                    {
                        string job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "";
                        JobModel _job = _jobs.Where(w => w.job_id == job_id).FirstOrDefault();
                        double ais_cost = _job.job_summary.FirstOrDefault().ais_cost;
                        double percent_ais_cost = Math.Ceiling(_job.job_summary.Sum(s => s.totalCost) / ais_cost * 100);
                        double invoice_ais = _job.ais_invoice;
                        double percent_invoice_ais = Math.Ceiling(invoice_ais / _job.job_ais_in_hand * 100);
                        double remaining_amount = _job.job_ais_in_hand - invoice_ais;
                        JobAISInhandModel jobSummary = new JobAISInhandModel()
                        {
                            job_id = job_id,
                            customer_name = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0,
                            job_ais_in_hand = dr["job_ais_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_ais_in_hand"]) : 0,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0,
                            invoice_ais = invoice_ais,
                            percent_ais_cost = percent_ais_cost,
                            percent_invoice = percent_invoice_ais,
                            remaining_percent_invoice = dr["remaining_percent_invoice"] != DBNull.Value ? Convert.ToDouble(dr["remaining_percent_invoice"]) : 0,
                            remaining_amount = Double.IsNaN(remaining_amount) ? 0 : remaining_amount
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

        public List<JobCISInhandModel> GetsCISJobBackLog(int year)
        {
            List<JobCISInhandModel> jobsSummaries = new List<JobCISInhandModel>();
            try
            {
                string stringCommand = string.Format($@"
                    select Jobs.job_id,
		                Jobs.job_name,
		                Jobs.customer_name,
		                Jobs.job_type,
		                Jobs.job_in_hand,
		                Jobs.job_cis_in_hand,
		                case when Invoice.invoice is null then 0 else Invoice.invoice end as invoice,	
	                    CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_cis_in_hand) as decimal(18,2)) as invoice_cis,
	                    (100.0 - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * 100) as decimal(18,3))) as remaining_percent_invoice,
	                    CAST((( CASE WHEN job_cis_in_hand IS NULL THEN 0 ELSE job_cis_in_hand END ) - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * (CASE WHEN job_cis_in_hand IS NULL THEN 0 ELSE job_cis_in_hand END)) as decimal(18,3))) as decimal(18,3)) as remaining_amount
                from Jobs
                LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') <= '{year}' GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
                where FORMAT(job_date ,'yyyy') < '{year}' OR job_date is null");


                SqlCommand cmd = new SqlCommand(stringCommand, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    List<JobModel> _jobs = JobStatus.GetJobStatusALL();
                    while (dr.Read())
                    {
                        string job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "";
                        JobModel _job = _jobs.Where(w => w.job_id == job_id).FirstOrDefault();
                        double cis_cost = _job.job_summary.FirstOrDefault().cis_cost;
                        double percent_cis_cost = Math.Ceiling(_job.job_summary.Sum(s => s.totalCost) / cis_cost * 100);
                        double invoice_cis = _job.cis_invoice;
                        double percent_invoice_cis = Math.Ceiling(invoice_cis / _job.job_cis_in_hand * 100);
                        double remaining_amount = _job.job_in_hand - _job.invoices.Sum(s => s.invoice);
                        JobCISInhandModel jobSummary = new JobCISInhandModel()
                        {
                            job_id = job_id,
                            customer_name = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0,
                            job_cis_in_hand = dr["job_cis_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_cis_in_hand"]) : 0,
                            percent_cis_cost = percent_cis_cost,
                            percent_invoice = percent_invoice_cis,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0,
                            invoice_cis = dr["invoice_cis"] != DBNull.Value ? Convert.ToDouble(dr["invoice_cis"]) : 0,
                            remaining_percent_invoice = dr["remaining_percent_invoice"] != DBNull.Value ? Convert.ToDouble(dr["remaining_percent_invoice"]) : 0,
                            remaining_amount = Double.IsNaN(remaining_amount) ? 0 : remaining_amount
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

        public List<JobCISInhandModel> GetsCISJobInhand(int year)
        {
            List<JobCISInhandModel> jobsSummaries = new List<JobCISInhandModel>();
            try
            {
                string stringCommand = string.Format($@"
                    select Jobs.job_id,
	                    customer_name,
	                    job_name,
	                    job_type,
	                    job_in_hand,
	                    job_cis_in_hand,
	                    case when Invoice.invoice is null then 0 else Invoice.invoice end as invoice,	
	                    CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_cis_in_hand) as decimal(18,3)) as invoice_cis,
	                    (100.0 - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * 100) as decimal(18,3))) as remaining_percent_invoice,
	                    CAST((job_cis_in_hand - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_cis_in_hand) as decimal(18,3))) as decimal(18,3)) as remaining_amount
                    from Jobs 
                    LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') = '{year}' GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
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
                    List<JobModel> _jobs = JobStatus.GetJobStatusALL();
                    while (dr.Read())
                    {
                        string job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "";
                        JobModel _job = _jobs.Where(w => w.job_id == job_id).FirstOrDefault();
                        double cis_cost = _job.job_summary.FirstOrDefault().cis_cost;
                        double percent_cis_cost = Math.Ceiling(_job.job_summary.Sum(s => s.totalCost) / cis_cost * 100);
                        double invoice_cis = _job.cis_invoice;
                        double percent_invoice_cis = Math.Ceiling(invoice_cis / _job.job_cis_in_hand * 100);
                        double remaining_amount = _job.job_cis_in_hand - invoice_cis;
                        JobCISInhandModel jobSummary = new JobCISInhandModel()
                        {
                            job_id = job_id,
                            customer_name = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0,
                            job_cis_in_hand = dr["job_cis_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_cis_in_hand"]) : 0,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0,
                            invoice_cis = invoice_cis,
                            percent_cis_cost = percent_cis_cost,
                            percent_invoice = percent_invoice_cis,
                            remaining_percent_invoice = dr["remaining_percent_invoice"] != DBNull.Value ? Convert.ToDouble(dr["remaining_percent_invoice"]) : 0,
                            remaining_amount = Double.IsNaN(remaining_amount) ? 0 : remaining_amount
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

        public List<JobENGInhandModel> GetsENGJobBackLog(int year)
        {
            List<JobENGInhandModel> jobsSummaries = new List<JobENGInhandModel>();
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
	                    (100.0 - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * 100) as decimal(18,3))) as remaining_percent_invoice,
	                    CAST((( CASE WHEN job_eng_in_hand IS NULL THEN 0 ELSE job_eng_in_hand END ) - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * (CASE WHEN job_eng_in_hand IS NULL THEN 0 ELSE job_eng_in_hand END)) as decimal(18,3))) as decimal(18,3)) as remaining_amount
                from Jobs
                LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') <= '{year}' GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
                where FORMAT(job_date ,'yyyy') < '{year}' OR job_date is null");


                SqlCommand cmd = new SqlCommand(stringCommand, ConnectSQL.OpenConnect());
                if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                    ConnectSQL.OpenConnect();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    List<JobModel> _jobs = JobStatus.GetJobStatusALL();
                    while (dr.Read())
                    {
                        string job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "";
                        JobModel _job = _jobs.Where(w => w.job_id == job_id).FirstOrDefault();
                        double eng_cost = _job.job_summary.FirstOrDefault().eng_cost;
                        double percent_eng_cost = Math.Ceiling(_job.job_summary.Sum(s => s.totalCost) / eng_cost * 100);
                        double invoice_eng = _job.eng_invoice;
                        double percent_invoice_eng = Math.Ceiling(invoice_eng / _job.job_eng_in_hand * 100);
                        double remaining_amount = _job.job_in_hand - _job.invoices.Sum(s => s.invoice);
                        JobENGInhandModel jobSummary = new JobENGInhandModel()
                        {
                            job_id = job_id,
                            customer_name = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0,
                            job_eng_in_hand = dr["job_eng_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_eng_in_hand"]) : 0,
                            percent_eng_cost = percent_eng_cost,
                            percent_invoice = percent_invoice_eng,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0,
                            invoice_eng = dr["invoice_eng"] != DBNull.Value ? Convert.ToDouble(dr["invoice_eng"]) : 0,
                            remaining_percent_invoice = dr["remaining_percent_invoice"] != DBNull.Value ? Convert.ToDouble(dr["remaining_percent_invoice"]) : 0,
                            remaining_amount = Double.IsNaN(remaining_amount) ? 0 : remaining_amount
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

        public List<JobENGInhandModel> GetsENGJobInhand(int year)
        {
            List<JobENGInhandModel> jobsSummaries = new List<JobENGInhandModel>();
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
	                    CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_eng_in_hand) as decimal(18,3)) as invoice_eng,
	                    (100.0 - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * 100) as decimal(18,3))) as remaining_percent_invoice,
	                    CAST((job_eng_in_hand - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_eng_in_hand) as decimal(18,3))) as decimal(18,3)) as remaining_amount
                    from Jobs 
                    LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') = '{year}' GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
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
                    List<JobModel> _jobs = JobStatus.GetJobStatusALL();
                    while (dr.Read())
                    {
                        string job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "";
                        JobModel _job = _jobs.Where(w => w.job_id == job_id).FirstOrDefault();
                        double eng_cost = _job.job_summary.FirstOrDefault().eng_cost;
                        double percent_eng_cost = Math.Ceiling(_job.job_summary.Sum(s => s.totalCost) / eng_cost * 100);
                        double invoice_eng = _job.eng_invoice;
                        double percent_invoice_eng = Math.Ceiling(invoice_eng / _job.job_eng_in_hand * 100);
                        double remaining_amount = _job.job_eng_in_hand - invoice_eng;
                        JobENGInhandModel jobSummary = new JobENGInhandModel()
                        {
                            job_id = job_id,
                            customer_name = dr["customer_name"] != DBNull.Value ? dr["customer_name"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"]) : 0,
                            job_eng_in_hand = dr["job_eng_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_eng_in_hand"]) : 0,
                            invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"]) : 0,
                            invoice_eng = invoice_eng,
                            percent_eng_cost = percent_eng_cost,
                            percent_invoice = percent_invoice_eng,
                            remaining_percent_invoice = dr["remaining_percent_invoice"] != DBNull.Value ? Convert.ToDouble(dr["remaining_percent_invoice"]) : 0,
                            remaining_amount = Double.IsNaN(remaining_amount) ? 0 : remaining_amount
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
       
        public List<SummaryAISJobInHandModel> GetsProjectAISJobInHand(int year, string type)
        {
            List<SummaryAISJobInHandModel> jobsSummaries = new List<SummaryAISJobInHandModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryAISJobInHandModel> getSummaries = GetsSummaryAISJobInHand(year, type);
            List<TargetModel> targets = Target.getData(year, "Project");
            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_ais_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryAISJobInHandModel()
                {
                    job_ais_in_hand = job_in_hand,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryCISJobInHandModel> GetsProjectCISJobInHand(int year, string type)
        {
            List<SummaryCISJobInHandModel> jobsSummaries = new List<SummaryCISJobInHandModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryCISJobInHandModel> getSummaries = GetsSummaryCISJobInHand(year, type);
            List<TargetModel> targets = Target.getData(year, "Project");
            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_cis_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryCISJobInHandModel()
                {
                    job_cis_in_hand = job_in_hand,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryENGJobInHandModel> GetsProjectENGJobInHand(int year, string type)
        {
            List<SummaryENGJobInHandModel> jobsSummaries = new List<SummaryENGJobInHandModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryENGJobInHandModel> getSummaries = GetsSummaryENGJobInHand(year, type);
            List<TargetModel> targets = Target.getData(year, "Project");
            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_eng_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryENGJobInHandModel()
                {
                    job_eng_in_hand = job_in_hand,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryAISJobInHandModel> GetsServiceAISJobInHand(int year, string type)
        {
            List<SummaryAISJobInHandModel> jobsSummaries = new List<SummaryAISJobInHandModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryAISJobInHandModel> getSummaries = GetsSummaryAISJobInHand(year, type);
            List<TargetModel> targets = Target.getData(year, "Service");
            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_ais_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryAISJobInHandModel()
                {
                    job_ais_in_hand = job_in_hand,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryCISJobInHandModel> GetsServiceCISJobInHand(int year, string type)
        {
            List<SummaryCISJobInHandModel> jobsSummaries = new List<SummaryCISJobInHandModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryCISJobInHandModel> getSummaries = GetsSummaryCISJobInHand(year, type);
            List<TargetModel> targets = Target.getData(year, "Service");
            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_cis_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryCISJobInHandModel()
                {
                    job_cis_in_hand = job_in_hand,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryENGJobInHandModel> GetsServiceENGJobInHand(int year, string type)
        {
            List<SummaryENGJobInHandModel> jobsSummaries = new List<SummaryENGJobInHandModel>();
            List<string> months = new List<string>();
            for (DateTime date = new DateTime(year, 1, 1); date <= new DateTime(year, 12, 1); date = date.AddMonths(1))
            {
                months.Add(date.ToString("MMM"));
            }
            List<SummaryENGJobInHandModel> getSummaries = GetsSummaryENGJobInHand(year, type);
            List<TargetModel> targets = Target.getData(year, "Service");
            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_eng_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryENGJobInHandModel()
                {
                    job_eng_in_hand = job_in_hand,
                    month = months[i],
                    target_month = targets.Where(w => Convert.ToDateTime(w.month).ToString("MMM") == months[i]).Select(s => s.target).FirstOrDefault()
                });
            }
            return jobsSummaries;
        }

        public List<SummaryAISJobInHandModel> GetsSummaryAISJobInHand(int year, string type)
        {
            List<SummaryAISJobInHandModel> jobsSummaries = new List<SummaryAISJobInHandModel>();
            try
            {
                string stringCommand = "";
                if (type == "ALL")
                {
                    stringCommand = string.Format($@"
                    select t1.Month,
	                    SUM(case when t1.job_ais_in_hand is null then 0.00 else CAST((t1.job_ais_in_hand / 1000000) as decimal(10,3)) end) as job_ais_in_hand
                    from 
	                    (
		                    select job_ais_in_hand,FORMAT(job_date,'MMM') as Month 
		                    from jobs where FORMAT(job_date,'yyyy') = '{year}'
	                    ) as t1
                    group by t1.Month");
                }
                else
                {
                    stringCommand = string.Format($@"
                    select t1.Month,
	                    SUM(case when t1.job_ais_in_hand is null then 0.00 else CAST((t1.job_ais_in_hand / 1000000) as decimal(10,3)) end) as job_ais_in_hand 
                    from 
	                    (
		                    select job_ais_in_hand,FORMAT(job_date,'MMM') as Month 
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
                        SummaryAISJobInHandModel jobSummary = new SummaryAISJobInHandModel()
                        {
                            month = dr["month"] != DBNull.Value ? dr["month"].ToString() : "",
                            job_ais_in_hand = dr["job_ais_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_ais_in_hand"]) : 0
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

        public List<QuarterAISModel> GetsSummaryAISQuarter(int year)
        {
            List<QuarterAISModel> jobsSummaries = new List<QuarterAISModel>();
            try
            {
                string stringCommand = string.Format($@"
                select  Jobs.job_id,
						job_date,
                        job_type,
                        CASE WHEN FORMAT(job_date,'yyyy') = '{year}' THEN
							CASE
								WHEN MONTH(job_date) >= 1 AND MONTH(job_date) <=3
								THEN
									1
								WHEN MONTH(job_date) >= 4 AND MONTH(job_date) <=6
								THEN
									2
								WHEN MONTH(job_date) >= 7 AND MONTH(job_date) <=9
								THEN
									3
								WHEN MONTH(job_date) >= 10 AND MONTH(job_date) <=12
								THEN
									4
								ELSE
									0
							END
						ELSE
							0						
						END as quarter,
						case when FORMAT(job_date,'yyyy') < {year} OR Jobs.job_date IS NULL then 'backlog' else 'now' end as type,
						CAST((Jobs.job_ais_in_hand / 1000000) as decimal(18,3)) as job_ais_in_hand,
						CAST((((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_ais_in_hand) / 1000000) as decimal(18,3)) as invoice_ais,
                        CAST((((case when backlog_invoice.invoice is null then 0 else backlog_invoice.invoice end / NULLIF(job_in_hand,0)) * job_ais_in_hand) / 1000000) as decimal(18,3)) as backlog_invoice_ais,
					    Eng_Status.status_name as status,
				        Jobs.finished_date
				from Jobs

				 LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') = {year} GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
                 LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') < {year} GROUP BY job_id) as backlog_invoice ON backlog_invoice.job_id = Jobs.job_id
				 LEFT JOIN Eng_Status ON Eng_Status.status_id = Jobs.status
                 WHERE job_type <> 'Department' and SUBSTRING(Jobs.job_id,1,1) <> 'Q'");

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
                        QuarterAISModel jobSummary = new QuarterAISModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
                            quarter = dr["quarter"] != DBNull.Value ? Convert.ToInt32(dr["quarter"].ToString()) : 0,
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            type = dr["type"] != DBNull.Value ? dr["type"].ToString() : "",
                            job_ais_in_hand = dr["job_ais_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_ais_in_hand"]) : 0,
                            invoice_ais = dr["invoice_ais"] != DBNull.Value ? Convert.ToDouble(dr["invoice_ais"]) : 0,
                            backlog_invoice_ais = dr["backlog_invoice_ais"] != DBNull.Value ? Convert.ToDouble(dr["backlog_invoice_ais"]) : 0,
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "",
                            finished_date = dr["finished_date"] != DBNull.Value ? Convert.ToDateTime(dr["finished_date"].ToString()) : DateTime.MinValue,
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

        public List<SummaryCISJobInHandModel> GetsSummaryCISJobInHand(int year, string type)
        {
            List<SummaryCISJobInHandModel> jobsSummaries = new List<SummaryCISJobInHandModel>();
            try
            {
                string stringCommand = "";
                if (type == "ALL")
                {
                    stringCommand = string.Format($@"
                    select t1.Month,
	                    SUM(case when t1.job_cis_in_hand is null then 0.00 else CAST((t1.job_cis_in_hand / 1000000) as decimal(10,3)) end) as job_cis_in_hand
                    from 
	                    (
		                    select job_cis_in_hand,FORMAT(job_date,'MMM') as Month 
		                    from jobs where FORMAT(job_date,'yyyy') = '{year}'
	                    ) as t1
                    group by t1.Month");
                }
                else
                {
                    stringCommand = string.Format($@"
                    select t1.Month,
	                    SUM(case when t1.job_cis_in_hand is null then 0.00 else CAST((t1.job_cis_in_hand / 1000000) as decimal(10,3)) end) as job_cis_in_hand 
                    from 
	                    (
		                    select job_cis_in_hand,FORMAT(job_date,'MMM') as Month 
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
                        SummaryCISJobInHandModel jobSummary = new SummaryCISJobInHandModel()
                        {
                            month = dr["month"] != DBNull.Value ? dr["month"].ToString() : "",
                            job_cis_in_hand = dr["job_cis_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_cis_in_hand"]) : 0
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

        public List<QuarterCISModel> GetsSummaryCISQuarter(int year)
        {
            List<QuarterCISModel> jobsSummaries = new List<QuarterCISModel>();
            try
            {
                string stringCommand = string.Format($@"
                select  Jobs.job_id,
						job_date,
                        job_type,
                        CASE WHEN FORMAT(job_date,'yyyy') = '{year}' THEN
							CASE
								WHEN MONTH(job_date) >= 1 AND MONTH(job_date) <=3
								THEN
									1
								WHEN MONTH(job_date) >= 4 AND MONTH(job_date) <=6
								THEN
									2
								WHEN MONTH(job_date) >= 7 AND MONTH(job_date) <=9
								THEN
									3
								WHEN MONTH(job_date) >= 10 AND MONTH(job_date) <=12
								THEN
									4
								ELSE
									0
							END
						ELSE
							0						
						END as quarter,
						case when FORMAT(job_date,'yyyy') < {year} OR Jobs.job_date IS NULL then 'backlog' else 'now' end as type,
						CAST((Jobs.job_cis_in_hand / 1000000) as decimal(18,3)) as job_cis_in_hand,
						CAST((((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_cis_in_hand) / 1000000) as decimal(18,3)) as invoice_cis,
                        CAST((((case when backlog_invoice.invoice is null then 0 else backlog_invoice.invoice end / NULLIF(job_in_hand,0)) * job_cis_in_hand) / 1000000) as decimal(18,3)) as backlog_invoice_cis,
					    Eng_Status.status_name as status,
				        Jobs.finished_date
				from Jobs

				 LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') = {year} GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
                 LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') < {year} GROUP BY job_id) as backlog_invoice ON backlog_invoice.job_id = Jobs.job_id
				 LEFT JOIN Eng_Status ON Eng_Status.status_id = Jobs.status
                 WHERE job_type <> 'Department' and SUBSTRING(Jobs.job_id,1,1) <> 'Q'");

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
                        QuarterCISModel jobSummary = new QuarterCISModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
                            quarter = dr["quarter"] != DBNull.Value ? Convert.ToInt32(dr["quarter"].ToString()) : 0,
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            type = dr["type"] != DBNull.Value ? dr["type"].ToString() : "",
                            job_cis_in_hand = dr["job_cis_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_cis_in_hand"]) : 0,
                            invoice_cis = dr["invoice_cis"] != DBNull.Value ? Convert.ToDouble(dr["invoice_cis"]) : 0,
                            backlog_invoice_cis = dr["backlog_invoice_cis"] != DBNull.Value ? Convert.ToDouble(dr["backlog_invoice_cis"]) : 0,
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "",
                            finished_date = dr["finished_date"] != DBNull.Value ? Convert.ToDateTime(dr["finished_date"].ToString()) : DateTime.MinValue,
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

        public List<QuarterENGModel> GetsSummaryENGQuarter(int year)
        {
            List<QuarterENGModel> jobsSummaries = new List<QuarterENGModel>();
            try
            {
                string stringCommand = string.Format($@"
                select  Jobs.job_id,
						job_date,
                        job_type,
                        CASE WHEN FORMAT(job_date,'yyyy') = '{year}' THEN
							CASE
								WHEN MONTH(job_date) >= 1 AND MONTH(job_date) <=3
								THEN
									1
								WHEN MONTH(job_date) >= 4 AND MONTH(job_date) <=6
								THEN
									2
								WHEN MONTH(job_date) >= 7 AND MONTH(job_date) <=9
								THEN
									3
								WHEN MONTH(job_date) >= 10 AND MONTH(job_date) <=12
								THEN
									4
								ELSE
									0
							END
						ELSE
							0						
						END as quarter,
						case when FORMAT(job_date,'yyyy') < {year} OR Jobs.job_date IS NULL then 'backlog' else 'now' end as type,
						CAST((Jobs.job_eng_in_hand / 1000000) as decimal(18,3)) as job_eng_in_hand,
						CAST((((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * job_eng_in_hand) / 1000000) as decimal(18,3)) as invoice_eng,
                        CAST((((case when backlog_invoice.invoice is null then 0 else backlog_invoice.invoice end / NULLIF(job_in_hand,0)) * job_eng_in_hand) / 1000000) as decimal(18,3)) as backlog_invoice_eng,
					    Eng_Status.status_name as status,
				        Jobs.finished_date
				from Jobs

				 LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') = {year} GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
                 LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') < {year} GROUP BY job_id) as backlog_invoice ON backlog_invoice.job_id = Jobs.job_id
				 LEFT JOIN Eng_Status ON Eng_Status.status_id = Jobs.status
                 WHERE job_type <> 'Department' and SUBSTRING(Jobs.job_id,1,1) <> 'Q'");

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
                        QuarterENGModel jobSummary = new QuarterENGModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_date = dr["job_date"] != DBNull.Value ? Convert.ToDateTime(dr["job_date"].ToString()) : DateTime.MinValue,
                            quarter = dr["quarter"] != DBNull.Value ? Convert.ToInt32(dr["quarter"].ToString()) : 0,
                            job_type = dr["job_type"] != DBNull.Value ? dr["job_type"].ToString() : "",
                            type = dr["type"] != DBNull.Value ? dr["type"].ToString() : "",
                            job_eng_in_hand = dr["job_eng_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_eng_in_hand"]) : 0,
                            invoice_eng = dr["invoice_eng"] != DBNull.Value ? Convert.ToDouble(dr["invoice_eng"]) : 0,
                            backlog_invoice_eng = dr["backlog_invoice_eng"] != DBNull.Value ? Convert.ToDouble(dr["backlog_invoice_eng"]) : 0,
                            status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "",
                            finished_date = dr["finished_date"] != DBNull.Value ? Convert.ToDateTime(dr["finished_date"].ToString()) : DateTime.MinValue,
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

        public List<SummaryENGJobInHandModel> GetsSummaryENGJobInHand(int year,string type)
        {
            List<SummaryENGJobInHandModel> jobsSummaries = new List<SummaryENGJobInHandModel>();
            try
            {
                string stringCommand = "";
                if (type == "ALL")
                {
                    stringCommand = string.Format($@"
                    select t1.Month,
	                    SUM(case when t1.job_eng_in_hand is null then 0.00 else CAST((t1.job_eng_in_hand / 1000000) as decimal(10,3)) end) as job_eng_in_hand
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
	                    SUM(case when t1.job_eng_in_hand is null then 0.00 else CAST((t1.job_eng_in_hand / 1000000) as decimal(10,3)) end) as job_eng_in_hand 
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
                        SummaryENGJobInHandModel jobSummary = new SummaryENGJobInHandModel()
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
