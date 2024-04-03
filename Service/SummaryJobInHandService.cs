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

            double acc = 0;
            for(int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_eng_in_hand).FirstOrDefault();
                acc += job_in_hand;
                jobsSummaries.Add(new SummaryJobInHandModel()
                {
                    job_eng_in_hand = acc,
                    month = months[i]
                });               
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

            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_eng_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryJobInHandModel()
                {
                    job_eng_in_hand = job_in_hand,
                    month = months[i]
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

            for (int i = 0; i < months.Count; i++)
            {
                var job_in_hand = getSummaries.Where(w => w.month == months[i]).Select(s => s.job_eng_in_hand).FirstOrDefault();
                jobsSummaries.Add(new SummaryJobInHandModel()
                {
                    job_eng_in_hand = job_in_hand,
                    month = months[i]
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
