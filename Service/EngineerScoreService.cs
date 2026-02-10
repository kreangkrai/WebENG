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
    public class EngineerScoreService : IScore
    {
		ConnectSQL connect = null;
		SqlConnection con = null;
        public EngineerScoreService()
        {
			connect = new ConnectSQL();
			con = connect.OpenConnect();
		}
		public List<EngineerScoreModel> GetScores(string user_id, string department)
        {
			string cost = "";
			if (department == "CES-System" || department == "CES-QIR" || department == "CES-Exp" || department == "CES-PMD" || department == "CES-ENG")
            {
				cost = "eng_cost";
            }
			if (department == "CES-CIS")
			{
				cost = "cis_cost";
			}

			if (department == "AES")
			{
				cost = "ais_cost";
			}

			List<EngineerScoreModel> scores = new List<EngineerScoreModel>();

			if (cost != "")
			{				
				try
				{
					if (con.State == ConnectionState.Closed)
					{
						con.Open();
					}
					string string_command = string.Format($@"                                       
                with t2 as (
					SELECT t.job_id,CAST(SUM(t.total_manpower * t. level) AS decimal(18,1)) as total_manpower ,
						 CAST(SUM(t.total_manpower / 60.0 / 8.0 * t.level * 3200) AS decimal(18,1)) as total_used_cost FROM ( 
							SELECT main.user_id,
							main.job_id,
							main.total_manpower,
							CASE WHEN JobResponsible.levels IS NULL THEN 1 ELSE JobResponsible.levels END as level
							 FROM (
							SELECT 
								WorkingHours.job_id,
								WorkingHours.user_id,
								SUM(
									case when task_id = 'T001' then
										case when FORMAT(working_date,'ddd') NOT IN ('Sun','Sat') then
											case when lunch_full = 1 then 
												case when dinner_full = 1 then
													DATEDIFF(MINUTE,start_time,stop_time) - 120												
												else
													case when dinner_half = 1 then
														DATEDIFF(MINUTE,start_time,stop_time) - 90
													else
														DATEDIFF(MINUTE,start_time,stop_time) - 60
													end
												end
											else 
												case when dinner_full = 1 then
													DATEDIFF(MINUTE,start_time,stop_time) - 60										
												else
													case when dinner_half = 1 then
														DATEDIFF(MINUTE,start_time,stop_time) - 30
													else
														DATEDIFF(MINUTE,start_time,stop_time)
													end
												end
											end
										else
											0
										end
									else
										case when lunch_full = 1 then 
												case when dinner_full = 1 then
													DATEDIFF(MINUTE,start_time,stop_time) - 120
												else
													case when dinner_half = 1 then
														DATEDIFF(MINUTE,start_time,stop_time) - 90
													else
														DATEDIFF(MINUTE,start_time,stop_time) - 60
													end
												end
											else 
												case when dinner_full = 1 then
													DATEDIFF(MINUTE,start_time,stop_time) - 60										
												else 
													case when dinner_half = 1 then
														DATEDIFF(MINUTE,start_time,stop_time) - 30
													else
														DATEDIFF(MINUTE,start_time,stop_time)
													end
												end
											end
							end) AS total_manpower
								
							FROM WorkingHours						
							WHERE WorkingHours.job_id <> 'J999999'
							GROUP BY WorkingHours.job_id,WorkingHours.user_id
							) as main
							LEFT JOIN JobResponsible ON main.user_id = JobResponsible.user_id AND main.job_id = JobResponsible.job_id
							) as t
							GROUP BY t.job_id
					),
					t3 as (
					SELECT 
		                    WorkingHours.job_id, 
		                    SUM(
									case when task_id = 'T001' then
									case when FORMAT(working_date,'ddd') NOT IN ('Sun','Sat') then
										case when lunch_full = 1 then 
											case when dinner_full = 1 then
												DATEDIFF(MINUTE,start_time,stop_time) - 120
											else
												case when dinner_half = 1 then
													DATEDIFF(MINUTE,start_time,stop_time) - 90
												else
													DATEDIFF(MINUTE,start_time,stop_time) - 60
												end
											end
										else 
											case when dinner_full = 1 then
												DATEDIFF(MINUTE,start_time,stop_time) - 60									
											else 
												case when dinner_half = 1 then
													DATEDIFF(MINUTE,start_time,stop_time) - 30
												else
													DATEDIFF(MINUTE,start_time,stop_time)
												end
											end
										end
									else
										0
									end
								else
									case when lunch_full = 1 then 
											case when dinner_full = 1 then
												DATEDIFF(MINUTE,start_time,stop_time) - 120
											else
												case when dinner_half = 1 then
													DATEDIFF(MINUTE,start_time,stop_time) - 90
												else
													DATEDIFF(MINUTE,start_time,stop_time) - 60
												end
											end
									else 
											case when dinner_full = 1 then
												DATEDIFF(MINUTE,start_time,stop_time) - 60										
											else
												case when dinner_half = 1 then
													DATEDIFF(MINUTE,start_time,stop_time) - 30
												else
													DATEDIFF(MINUTE,start_time,stop_time)
												end
											end
										end
								end) AS working_hours 
	                    FROM WorkingHours
						WHERE user_id = '{user_id}'
	                    GROUP BY user_id,job_id
					)
					SELECT distinct
	                    t1.user_id AS user_id,
	                    t1.job_id AS job_id,
	                    Jobs.job_name AS job_name,
                        Jobs.customer_name as customer,
                        Jobs.status,
                        {cost},
	                    md_rate AS md_rate,
	                    pd_rate AS pd_rate,
	                    (md_rate * pd_rate) AS factor,						
	                    CAST((t2.total_manpower / 60.0 ) as decimal(18,1)) AS total_manpower,
	                    CAST(((CASE WHEN eng_cost IS NULL THEN 0 ELSE eng_cost END +
						 CASE WHEN cis_cost IS NULL THEN 0 ELSE cis_cost END + 
						 CASE WHEN ais_cost IS NULL THEN 0 ELSE ais_cost END) / (t2.total_manpower / 60.0 )) as decimal(18,1)) AS cost_per_tmp,
	                    CAST((t3.working_hours / 60.0 ) as decimal(18,1)) AS manpower,
	                    CAST((CAST((t3.working_hours / 60.0) AS FLOAT) / (CAST((t2.total_manpower / 60.0) AS FLOAT))) as decimal(18,1)) AS manpower_per_tmp,
	                    CAST(((CASE WHEN eng_cost IS NULL THEN 0 ELSE eng_cost END +
						 CASE WHEN cis_cost IS NULL THEN 0 ELSE cis_cost END + 
						 CASE WHEN ais_cost IS NULL THEN 0 ELSE ais_cost END) * (md_rate + pd_rate) * ((CASE WHEN eng_cost IS NULL THEN 0 ELSE eng_cost END +
						 CASE WHEN cis_cost IS NULL THEN 0 ELSE cis_cost END + 
						 CASE WHEN ais_cost IS NULL THEN 0 ELSE ais_cost END) / (t2.total_manpower / 60.0)) * (CAST((t3.working_hours / 60.0) AS FLOAT) / (CAST((t2.total_manpower / 60.0) AS FLOAT)))) as decimal(18,1)) AS score,
						CAST(((CASE WHEN eng_cost IS NULL THEN 0 ELSE eng_cost END +
						 CASE WHEN cis_cost IS NULL THEN 0 ELSE cis_cost END + 
						 CASE WHEN ais_cost IS NULL THEN 0 ELSE ais_cost END) - t2.total_used_cost) AS decimal(18,1)) as remaining_cost
                    FROM WorkingHours  As t1  
					LEFT JOIN Jobs ON t1.job_id = Jobs.job_id
                    LEFT JOIN Quotation ON Jobs.quotation_no = Quotation.quotation_no
                    INNER JOIN t2	                     
                    ON t1.job_id = t2.job_id
                    INNER JOIN t3					
                    ON t1.job_id = t3.job_id
                 WHERE t1.user_id = '{user_id}'");
					SqlCommand cmd = new SqlCommand(string_command, con);
					SqlDataReader dr = cmd.ExecuteReader();
					if (dr.HasRows)
					{
						while (dr.Read())
						{
							EngineerScoreModel score = new EngineerScoreModel()
							{
								job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
								job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
								job_status = dr["status"] != DBNull.Value ? dr["status"].ToString() : "",
								customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
								cost = dr[cost] != DBNull.Value ? Convert.ToInt32(dr[cost]) : 0,
								md_rate = dr["md_rate"] != DBNull.Value ? Convert.ToDouble(dr["md_rate"]) : 0,
								pd_rate = dr["pd_rate"] != DBNull.Value ? Convert.ToDouble(dr["pd_rate"]) : 0,
								factor = dr["factor"] != DBNull.Value ? Convert.ToDouble(dr["factor"]) : 0,
								total_manpower = dr["total_manpower"] != DBNull.Value ? Convert.ToDouble(dr["total_manpower"]) : 0,
								cost_per_tmp = dr["cost_per_tmp"] != DBNull.Value ? Convert.ToDouble(dr["cost_per_tmp"]) : 0,
								manpower = dr["manpower"] != DBNull.Value ? Convert.ToDouble(dr["manpower"]) : 0,
								manpower_per_tmp = dr["manpower_per_tmp"] != DBNull.Value ? Convert.ToDouble(dr["manpower_per_tmp"]) : 0,
								score = dr["score"] != DBNull.Value ? Convert.ToDouble(dr["score"]) : 0,
								remaining_cost = dr["remaining_cost"] != DBNull.Value ? Convert.ToDouble(dr["remaining_cost"]) : 0,
							};
							scores.Add(score);
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
			}
            return scores;
        }
    }
}
