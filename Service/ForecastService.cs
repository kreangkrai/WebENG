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
    public class ForecastService : IForecast
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public ForecastService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }

        public double GetBacklog(int year)
        {
            double backlog = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"select SUM(main.remaining_amount) as backlog FROM (
				select 			
	                    CAST((( CASE WHEN job_in_hand IS NULL THEN 0 ELSE job_in_hand END ) - CAST(((case when Invoice.invoice is null then 0 else Invoice.invoice end / NULLIF(job_in_hand,0)) * (CASE WHEN job_in_hand IS NULL THEN 0 ELSE job_in_hand END)) as decimal(18,3))) as decimal(18,3)) as remaining_amount
                from Jobs
                LEFT JOIN (select job_id,SUM(invoice) as invoice from Invoice where FORMAT(actual_date,'yyyy') <= @year GROUP BY job_id) as invoice ON invoice.job_id = Jobs.job_id
                where FORMAT(job_date ,'yyyy') < @year OR job_date is null ) as main");
                SqlCommand cmd = new SqlCommand(string_command, con);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        backlog = dr["backlog"] != DBNull.Value ? Convert.ToDouble(dr["backlog"].ToString()) : 0;
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
            return backlog;
        }

        public List<ForecastModel> GetForecasts(int year)
        {
            List<ForecastModel> forecasts = new List<ForecastModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"WITH Months AS (
                                                        SELECT '{year}-01' AS month UNION ALL SELECT '{year}-02' UNION ALL SELECT '{year}-03' UNION ALL SELECT '{year}-04'
                                                        UNION ALL SELECT '{year}-05' UNION ALL SELECT '{year}-06' UNION ALL SELECT '{year}-07' UNION ALL SELECT '{year}-08'
                                                        UNION ALL SELECT '{year}-09' UNION ALL SELECT '{year}-10' UNION ALL SELECT '{year}-11' UNION ALL SELECT '{year}-12'
                                                    ),
                                                    Forecast AS (
                                                        SELECT 
                                                            j.job_id,
                                                            j.job_name,
		                                                    j.job_in_hand,
                                                            tp.payment_id,
                                                            tp.payment_name,
                                                            tp.[percent],
                                                            tp.forecast_month,
                                                            CONVERT(varchar(7), tp.forecast_month, 126) AS forecast_month_str,
                                                            (tp.[percent] / 100.0 * ISNULL(j.job_in_hand, 0)) AS forecast_amount,
                                                            tp.remark AS forecast_remark
                                                        FROM [dbo].[Term_Payments] tp
                                                        INNER JOIN [dbo].[Jobs] j ON tp.job_id = j.job_id
                                                        WHERE tp.forecast_month >= '{year}-01-01' 
                                                          AND tp.forecast_month < '{year + 1}-01'
                                                    ),
                                                    Actual AS (
                                                        SELECT 
                                                            j.job_id,
		                                                    j.job_in_hand,
                                                            i.milestone,
                                                            i.invoice AS actual_amount,
                                                            CONVERT(varchar(7), i.actual_date, 126) AS actual_month,
                                                            i.actual_date,
                                                            i.status,
                                                            i.remark AS actual_remark
                                                        FROM [dbo].[Invoice] i
                                                        INNER JOIN [dbo].[Jobs] j ON i.job_id = j.job_id
                                                        WHERE i.actual_date >= '{year}-01-01' AND i.actual_date < '{year+1}-01-01'
                                                    ),
                                                    Combined AS (
                                                        SELECT 
                                                            m.month,
                                                            f.job_id,
                                                            f.job_name,
		                                                    f.job_in_hand,
                                                            f.payment_id,
                                                            f.payment_name,
                                                            f.forecast_month_str AS forecast_month,
                                                            f.[percent],
                                                            f.forecast_amount,
                                                            f.forecast_remark,
                                                            a.actual_month,
                                                            a.actual_amount,
                                                            a.actual_date,
                                                            a.status,
                                                            a.actual_remark
                                                        FROM Months m
                                                        LEFT JOIN Forecast f ON m.month = f.forecast_month_str
                                                        LEFT JOIN Actual a ON f.job_id = a.job_id 
                                                                          AND f.payment_name = a.milestone 

                                                        UNION ALL

                                                        SELECT 
                                                            m.month,
                                                            a.job_id,
                                                            j.job_name,
		                                                    a.job_in_hand AS JobInHand,
                                                            t.payment_id AS payment_id,
                                                            a.milestone AS payment_name,
                                                            NULL AS forecast_month,
                                                            NULL AS [percent],
                                                            NULL AS forecast_amount,
                                                            NULL AS forecast_remark,
                                                            a.actual_month,
                                                            a.actual_amount,
                                                            a.actual_date,
                                                            a.status,
                                                            a.actual_remark
                                                        FROM Months m
                                                        INNER JOIN Actual a ON m.month = a.actual_month
                                                        INNER JOIN [dbo].[Jobs] j ON a.job_id = j.job_id
                                                        LEFT JOIN Forecast f ON a.job_id = f.job_id 
                                                                            AND a.milestone = f.payment_name 
	                                                    LEFT JOIN Term_Payments t 
                                                        ON t.job_id = a.job_id 
                                                       AND t.payment_name = a.milestone
                                                        WHERE f.job_id IS NULL
                                                    )
                                                    SELECT 
                                                        month,
                                                        job_id,
                                                        job_name,
	                                                    job_in_hand,
                                                        payment_id,
                                                        payment_name,
                                                        forecast_month, 
                                                        [percent],
                                                        forecast_amount,
                                                        forecast_remark,
                                                        actual_month,
                                                        actual_amount,
                                                        actual_date,
                                                        status,
                                                        actual_remark
                                                    FROM Combined
                                                    WHERE job_id IS NOT NULL
                                                    ORDER BY month, job_id, payment_id; ");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ForecastModel forecast = new ForecastModel()
                        {
                            month = dr["month"].ToString(),
                            job_id = dr["job_id"].ToString(),
                            job_name = dr["job_name"].ToString(),
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"].ToString()) : 0,
                            payment_id = dr["payment_id"].ToString(),
                            payment_name = dr["payment_name"].ToString(),
                            forecast_month = dr["forecast_month"].ToString(),
                            percent = dr["percent"] != DBNull.Value ? Convert.ToInt32(dr["percent"].ToString()) : 0,
                            forecast_amount = dr["forecast_amount"] != DBNull.Value ? Convert.ToDouble(dr["forecast_amount"].ToString()) : 0,
                            forecast_remark = dr["forecast_remark"].ToString(),
                            actual_month = dr["actual_month"].ToString(),
                            actual_amount = dr["actual_amount"] != DBNull.Value ? Convert.ToDouble(dr["actual_amount"].ToString()) : 0,
                            actual_date = dr["actual_date"] != DBNull.Value ? Convert.ToDateTime(dr["actual_date"].ToString()) : DateTime.MinValue,
                            actual_remark = dr["actual_remark"].ToString(),
                            status = dr["status"].ToString()
                        };
                        forecasts.Add(forecast);
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
            return forecasts;
        }

        public double GetInvoice(int year)
        {
            double total_invoice = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT 
                                                            COALESCE(SUM(i.invoice), 0) AS total_invoice
                                                        FROM 
                                                            Jobs j
                                                        LEFT JOIN 
                                                            Invoice i ON j.job_id = i.job_id
                                                        WHERE 
                                                            FORMAT(j.job_date, 'yyyy') = @year;");
                SqlCommand cmd = new SqlCommand(string_command, con);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        total_invoice = dr["total_invoice"] != DBNull.Value ? Convert.ToDouble(dr["total_invoice"].ToString()) : 0;
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
            return total_invoice;
        }

        public double GetJonInHand(int year)
        {
            double job_in_hand = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"select 
	                                                          Sum(job_in_hand) as job_in_hand
                                                        from jobs where FORMAT(job_date,'yyyy') = @year");
                SqlCommand cmd = new SqlCommand(string_command, con);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"].ToString()) : 0;
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
            return job_in_hand;
        }
    }
}
