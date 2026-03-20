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
        //public List<InvoicesModel> GetInvoice(int year)
        //{
        //    List<InvoicesModel> invoices = new List<InvoicesModel>();
        //    try
        //    {
        //        if (con.State == ConnectionState.Closed)
        //        {
        //            con.Open();
        //        }

        //        string string_command = string.Format($@"SELECT
        //                                                j.job_id,
		      //                                          j.job_in_hand,
		      //                                          j.job_eng_in_hand,
		      //                                          j.job_ais_in_hand,
		      //                                          j.job_cis_in_hand,
		      //                                          j.responsible,
		      //                                          j.job_date,
		      //                                          e.department,
        //                                                i.invoice,
		      //                                          i.actual_date as invoice_date
        //                                            FROM [dbo].[Jobs] j
        //                                            LEFT JOIN CTL.[dbo].[Employees] e ON j.responsible = e.name_en
        //                                            LEFT JOIN [dbo].[Invoice] i ON j.job_id = i.job_id
        //                                            WHERE FORMAT(i.actual_date, 'yyyy') = @year and j.responsible IS NOT NULL");

        //        SqlCommand cmd = new SqlCommand(string_command, con);
        //        cmd.Parameters.AddWithValue("@year", year);
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                InvoicesModel inv = new InvoicesModel()
        //                {
        //                    job_id = dr["job_id"].ToString(),
        //                    job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"].ToString()) : 0,
        //                    job_eng_in_hand = dr["job_eng_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_eng_in_hand"].ToString()) : 0,
        //                    job_ais_in_hand = dr["job_ais_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_ais_in_hand"].ToString()) : 0,
        //                    job_cis_in_hand = dr["job_cis_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_cis_in_hand"].ToString()) : 0,
        //                    responsible = dr["responsible"].ToString(),
        //                    job_date = Convert.ToDateTime(dr["job_date"].ToString()),
        //                    department = dr["department"].ToString(),
        //                    invoice = dr["invoice"] != DBNull.Value ? Convert.ToDouble(dr["invoice"].ToString()) : 0,
        //                    invoice_date = Convert.ToDateTime(dr["invoice_date"].ToString()),
        //                };
        //                invoices.Add(inv);
        //            }
        //            dr.Close();
        //        }
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //        }
        //    }
        //    return invoices;
        //}

        public List<ForecastModel> GetForecasts(int year)
        {
            double mb = 1_000_000;
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
                                                       ForecastSplit AS (
                                                            SELECT
                                                                tp.job_id,
                                                                j.job_name,
                                                                j.responsible,
                                                                j.job_in_hand,
                                                                j.job_eng_in_hand,
                                                                j.job_cis_in_hand,
                                                                j.job_ais_in_hand,
                                                                j.job_type,
                                                                tp.payment_id,
                                                                tp.payment_name,
                                                                tp.[percent],
                                                                CONVERT(varchar(7), tp.forecast_month, 126) AS forecast_month,
                                                                tp.remark AS forecast_remark,
       
                                                                CASE WHEN ISNULL(j.job_in_hand, 0) > 0 THEN j.job_eng_in_hand / j.job_in_hand ELSE 0 END AS prop_CES,
                                                                CASE WHEN ISNULL(j.job_in_hand, 0) > 0 THEN j.job_cis_in_hand / j.job_in_hand ELSE 0 END AS prop_CIS,
                                                                CASE WHEN ISNULL(j.job_in_hand, 0) > 0 THEN j.job_ais_in_hand / j.job_in_hand ELSE 0 END AS prop_AES,

                                                                (tp.[percent] / 100.0 * ISNULL(j.job_in_hand, 0)) * 
                                                                 CASE WHEN ISNULL(j.job_in_hand, 0) > 0 THEN j.job_eng_in_hand / j.job_in_hand ELSE 0 END AS forecast_CES,
        
                                                                (tp.[percent] / 100.0 * ISNULL(j.job_in_hand, 0)) * 
                                                                 CASE WHEN ISNULL(j.job_in_hand, 0) > 0 THEN j.job_cis_in_hand / j.job_in_hand ELSE 0 END AS forecast_CIS,
        
                                                                (tp.[percent] / 100.0 * ISNULL(j.job_in_hand, 0)) * 
                                                                 CASE WHEN ISNULL(j.job_in_hand, 0) > 0 THEN j.job_ais_in_hand / j.job_in_hand ELSE 0 END AS forecast_AES
                                                            FROM [dbo].[Term_Payments] tp
                                                            INNER JOIN [dbo].[Jobs] j ON tp.job_id = j.job_id
                                                            WHERE tp.forecast_month >= '{year}-01-01'
                                                              AND tp.forecast_month < '{year+1}-01-01'
                                                        ),

                                                        ForecastUnpivot AS (
                                                            SELECT
                                                                job_id,
                                                                job_name,
                                                                responsible,
                                                                job_in_hand,
                                                                job_eng_in_hand AS in_hand,
                                                                job_type,
                                                                payment_id,
                                                                payment_name,
                                                                forecast_month,
                                                                [percent],
                                                                forecast_remark,
                                                                'CES' AS department,
                                                                forecast_CES AS forecast_amount,
                                                                NULL AS actual_amount
                                                            FROM ForecastSplit
                                                            --WHERE prop_CES > 0

                                                            UNION ALL

                                                            SELECT
                                                                job_id,
                                                                job_name,
                                                                responsible,
                                                                job_in_hand,
                                                                job_cis_in_hand AS in_hand,
                                                                job_type,
                                                                payment_id,
                                                                payment_name,
                                                                forecast_month,
                                                                [percent],
                                                                forecast_remark,
                                                                'CIS' AS department,
                                                                forecast_CIS AS forecast_amount,
                                                                NULL AS actual_amount
                                                            FROM ForecastSplit
                                                            --WHERE prop_CIS > 0

                                                            UNION ALL

                                                            SELECT
                                                                job_id,
                                                                job_name,
                                                                responsible,
                                                                job_in_hand,
                                                                job_ais_in_hand AS in_hand,
                                                                job_type,
                                                                payment_id,
                                                                payment_name,
                                                                forecast_month,
                                                                [percent],
                                                                forecast_remark,
                                                                'AES' AS department,
                                                                forecast_AES AS forecast_amount,
                                                                NULL AS actual_amount
                                                            FROM ForecastSplit
                                                            --WHERE prop_AES > 0
                                                            UNION ALL

                                                            SELECT
                                                                job_id,
                                                                job_name,
                                                                responsible,
                                                                job_in_hand,
                                                                job_in_hand AS in_hand,
                                                                job_type,
                                                                payment_id,
                                                                payment_name,
                                                                forecast_month,
                                                                [percent],
                                                                forecast_remark,
                                                                'PMD' AS department,
                                                                forecast_AES AS forecast_amount,
                                                                NULL AS actual_amount
                                                            FROM ForecastSplit
                                                        ),

                                                        Combined AS (
                                                            SELECT
                                                                f.job_id,
                                                                f.job_name,
                                                                f.responsible,
                                                                f.job_in_hand,
                                                                f.in_hand,
                                                                f.job_type,
                                                                f.department,
                                                                f.payment_id,
                                                                f.payment_name,
                                                                f.forecast_month,
                                                                f.[percent],
                                                                f.forecast_remark,
                                                                f.forecast_amount,
                                                                f.actual_amount
                                                            FROM Months m
                                                            LEFT JOIN ForecastUnpivot f ON m.month = f.forecast_month
                                                        )

                                                        SELECT
                                                            c.job_id,
                                                            c.job_name,
                                                            c.responsible,
                                                            emp.department AS responsible_department,
                                                            c.job_in_hand,
                                                            c.in_hand AS department_in_hand,
                                                            c.job_type,
                                                            c.department,
                                                            c.payment_id,
                                                            c.payment_name as milestone,
                                                            c.forecast_month,
                                                            c.[percent],
	                                                        c.[percent] / 100.0 * c.job_in_hand as forecast_portion_amount,
                                                            c.[percent] / 100.0 * c.in_hand as forecast_portion_department_amount,
                                                            c.forecast_remark
                                                        FROM Combined c
                                                        LEFT JOIN [CTL].dbo.Employees emp
                                                            ON c.responsible = emp.name_en
                                                        WHERE c.job_id IS NOT NULL
                                                        ORDER BY c.job_id, c.department, c.payment_id, c.payment_name;");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ForecastModel forecast = new ForecastModel()
                        {
                            job_id = dr["job_id"].ToString(),
                            job_name = dr["job_name"].ToString(),
                            responsible = dr["responsible"].ToString(),
                            responsible_department = dr["responsible_department"].ToString(),
                            department = dr["department"].ToString(),
                            job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"].ToString()) / mb : 0,
                            department_in_hand = dr["department_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["department_in_hand"].ToString()) / mb : 0,
                            job_type = dr["job_type"].ToString(),
                            payment_id = dr["payment_id"].ToString(),
                            milestone = dr["milestone"].ToString(),
                            forecast_month = dr["forecast_month"].ToString(),
                            percent = dr["percent"] != DBNull.Value ? Convert.ToInt32(dr["percent"].ToString()) : 0,
                            forecast_portion_amount = dr["forecast_portion_amount"] != DBNull.Value ? Convert.ToDouble(dr["forecast_portion_amount"].ToString()) / mb : 0,
                            forecast_portion_department_amount = dr["forecast_portion_department_amount"] != DBNull.Value ? Convert.ToDouble(dr["forecast_portion_department_amount"].ToString()) / mb : 0,
                            forecast_remark = dr["forecast_remark"].ToString(),
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

        //public double GetInvoice(int year , string department, string responsible)
        //{
        //    double total_invoice = 0;
        //    try
        //    {
        //        if (con.State == ConnectionState.Closed)
        //        {
        //            con.Open();
        //        }
        //        string string_command = "";
        //        if (department == "ALL" && responsible == "ALL")
        //        {
        //            string_command = string.Format($@"SELECT SUM (main.total_invoice) total_invoice FROM (
        //                                                        SELECT 
        //                                                            j.job_id,
        //                                                            j.job_name,
        //                                                            j.responsible,
        //                                                            e.department,
        //                                                            SUM(i.invoice) AS total_invoice
        //                                                        FROM 
        //                                                            [dbo].[Jobs] j
        //                                                        LEFT JOIN CTL.[dbo].[Employees] e ON j.responsible = e.name_en
        //                                                        LEFT JOIN [dbo].[Invoice] i ON j.job_id = i.job_id
        //                                                        WHERE FORMAT(i.actual_date,'yyyy') = @year
        //                                                        GROUP BY 
        //                                                            j.job_id, j.job_name, j.responsible, e.department
        //                                                        HAVING 
        //                                                            SUM(i.invoice) > 0 
	
	       //                                                         ) as main");
        //        }
        //        else if (responsible != "ALL")
        //        {
        //            string_command = string.Format($@"SELECT SUM (main.total_invoice) total_invoice FROM (
        //                                                        SELECT 
        //                                                            j.job_id,
        //                                                            j.job_name,
        //                                                            j.responsible,
        //                                                            e.department,
        //                                                            SUM(i.invoice) AS total_invoice
        //                                                        FROM 
        //                                                            [dbo].[Jobs] j
        //                                                        LEFT JOIN CTL.[dbo].[Employees] e ON j.responsible = e.name_en
        //                                                        LEFT JOIN [dbo].[Invoice] i ON j.job_id = i.job_id
        //                                                        WHERE FORMAT(i.actual_date,'yyyy') = @year AND LOWER(responsible) = @responsible
        //                                                        GROUP BY 
        //                                                            j.job_id, j.job_name, j.responsible, e.department
        //                                                        HAVING 
        //                                                            SUM(i.invoice) > 0 
	
	       //                                                         ) as main");               
        //        }

        //        else if (department != "ALL" && responsible == "ALL")
        //        {
        //            if (department == "CES")
        //            {
        //                string_command = string.Format($@"SELECT SUM (main.total_invoice) total_invoice FROM (
        //                                                    SELECT 
        //                                                        j.job_id,
        //                                                        j.job_name,
        //                                                        j.responsible,
        //                                                        e.department,
        //                                                        SUM(i.invoice) AS total_invoice
        //                                                    FROM 
        //                                                        [dbo].[Jobs] j
        //                                                    LEFT JOIN CTL.[dbo].[Employees] e ON j.responsible = e.name_en
        //                                                    LEFT JOIN [dbo].[Invoice] i ON j.job_id = i.job_id
        //                                                    WHERE FORMAT(i.actual_date,'yyyy') = '2025' AND e.department IN('CES-system','CES-Exp','CES-PMD','CES-QIR')
        //                                                    GROUP BY 
        //                                                        j.job_id, j.job_name, j.responsible, e.department
        //                                                    HAVING 
        //                                                        SUM(i.invoice) > 0 
	
	       //                                                     ) as main");
        //            }
        //            if (department == "CIS")
        //            {
        //                string_command = string.Format($@"
        //                                                    SELECT SUM (main.total_invoice) total_invoice FROM (
        //                                                    SELECT 
        //                                                        j.job_id,
        //                                                        j.job_name,
        //                                                        j.responsible,
        //                                                        e.department,
        //                                                        SUM(i.invoice) AS total_invoice
        //                                                    FROM 
        //                                                        [dbo].[Jobs] j
        //                                                    LEFT JOIN CTL.[dbo].[Employees] e ON j.responsible = e.name_en
        //                                                    LEFT JOIN [dbo].[Invoice] i ON j.job_id = i.job_id
        //                                                    WHERE FORMAT(i.actual_date,'yyyy') = '2025' AND e.department IN('CES-CIS')
        //                                                    GROUP BY 
        //                                                        j.job_id, j.job_name, j.responsible, e.department
        //                                                    HAVING 
        //                                                        SUM(i.invoice) > 0 
	
	       //                                                     ) as main");
        //            }
        //            if (department == "AES")
        //            {
        //                string_command = string.Format($@"
        //                                                SELECT SUM (main.total_invoice) total_invoice FROM (
        //                                                SELECT 
        //                                                    j.job_id,
        //                                                    j.job_name,
        //                                                    j.responsible,
        //                                                    e.department,
        //                                                    SUM(i.invoice) AS total_invoice
        //                                                FROM 
        //                                                    [dbo].[Jobs] j
        //                                                LEFT JOIN CTL.[dbo].[Employees] e ON j.responsible = e.name_en
        //                                                LEFT JOIN [dbo].[Invoice] i ON j.job_id = i.job_id
        //                                                WHERE FORMAT(i.actual_date,'yyyy') = '2025' AND e.department IN('AES')
        //                                                GROUP BY 
        //                                                    j.job_id, j.job_name, j.responsible, e.department
        //                                                HAVING 
        //                                                    SUM(i.invoice) > 0 
	
	       //                                                 ) as main");
        //            }
        //        }


        //        SqlCommand cmd = new SqlCommand(string_command, con);
        //        cmd.Parameters.AddWithValue("@year", year);
        //        cmd.Parameters.AddWithValue("@responsible", responsible);
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                total_invoice = dr["total_invoice"] != DBNull.Value ? Convert.ToDouble(dr["total_invoice"].ToString()) : 0;
        //            }
        //            dr.Close();
        //        }
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //        }
        //    }
        //    return total_invoice;
        //}

        //public double GetJonInHand(int year , string department, string responsible)
        //{
        //    double job_in_hand = 0;
        //    try
        //    {
        //        if (con.State == ConnectionState.Closed)
        //        {
        //            con.Open();
        //        }
        //        string string_command = "";
        //        if (department == "ALL" && responsible == "ALL")
        //        {
        //            string_command = string.Format($@"select 
	       //                                                   Sum(job_in_hand) as job_in_hand
        //                                                from jobs where FORMAT(job_date,'yyyy') = @year");
        //        }

        //        else if(responsible != "ALL")
        //        {
        //            string_command = string.Format($@"select 
	       //                                                   Sum(job_in_hand) as job_in_hand
        //                                                from jobs where FORMAT(job_date,'yyyy') = @year AND LOWER(responsible) = @responsible");
        //        }
        //        else if (department != "ALL" && responsible == "ALL")
        //        {
        //            if (department == "CES")
        //            {
        //                string_command = string.Format($@"select 
	       //                                                   Sum(job_eng_in_hand) as job_in_hand
        //                                                from jobs 
								//						where FORMAT(job_date,'yyyy') = @year");
        //            }
        //            if (department == "CIS")
        //            {
        //                string_command = string.Format($@"select 
	       //                                                   Sum(job_cis_in_hand) as job_in_hand
        //                                                from jobs 
								//						where FORMAT(job_date,'yyyy') = @year");
        //            }
        //            if (department == "AES")
        //            {
        //                string_command = string.Format($@"select 
	       //                                                   Sum(job_ais_in_hand) as job_in_hand
        //                                                from jobs 
								//						where FORMAT(job_date,'yyyy') = @year");
        //            }                   
        //        }
                

        //        SqlCommand cmd = new SqlCommand(string_command, con);
        //        cmd.Parameters.AddWithValue("@year", year);
        //        cmd.Parameters.AddWithValue("@responsible", responsible);
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                job_in_hand = dr["job_in_hand"] != DBNull.Value ? Convert.ToDouble(dr["job_in_hand"].ToString()) : 0;
        //            }
        //            dr.Close();
        //        }
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //        }
        //    }
        //    return job_in_hand;
        //}
    }
}
