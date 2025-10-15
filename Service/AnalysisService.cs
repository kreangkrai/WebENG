using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Service;
using WebENG.Interface;
using WebENG.Models;
using System.Data;

namespace WebENG.Service
{
    public class AnalysisService : IAnalysis
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public AnalysisService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<TaskRatioModel> GetTaskRatio(string job_id)
        {
            List<TaskRatioModel> trs = new List<TaskRatioModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                
                    SELECT 
	                    WorkingHours.job_id,
	                    Jobs.job_name,
	                    WorkingHours.task_id,
	                    Tasks.task_name,
	                    CAST(SUM(
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
	                    ) / 60.0 as decimal(18,1))as hours
                    FROM WorkingHours
                    LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
                    LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                    WHERE WorkingHours.job_id = '{job_id}'
                    GROUP BY WorkingHours.task_id, Tasks.task_name, WorkingHours.job_id, Jobs.job_name
                    ORDER BY hours desc");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TaskRatioModel tr = new TaskRatioModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            hours = dr["hours"] != DBNull.Value ? Convert.ToDouble(dr["hours"]) : 0,
                            percents = 0
                        };
                        trs.Add(tr);
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
            return trs;
        }

        public List<TaskDistributionModel> GetTaskDistribution(string job_id)
        {
            List<TaskDistributionModel> tds = new List<TaskDistributionModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    SELECT 
	                    WorkingHours.job_id,
	                    Jobs.job_name,
	                    WorkingHours.task_id,
	                    Tasks.task_name,
	                    CAST(SUM(
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
	                    ) / 60.0 as decimal(18,1))as hours
                    FROM WorkingHours
                    LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
                    LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                    WHERE WorkingHours.job_id = '{job_id}'
                    GROUP BY WorkingHours.task_id, Tasks.task_name, WorkingHours.job_id, Jobs.job_name
                    ORDER BY hours desc");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TaskDistributionModel td = new TaskDistributionModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            hours = dr["hours"] != DBNull.Value ? Convert.ToDouble(dr["hours"].ToString()) : 0,
                        };
                        tds.Add(td);
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
            return tds;
        }

        public List<ManpowerRatioModel> GetManpowerRatio(string job_id)
        {
            List<ManpowerRatioModel> mrs = new List<ManpowerRatioModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    SELECT 
	                    WorkingHours.user_id,
	                    Authen.name,
						Authen.user_id,
	                    WorkingHours.job_id,
	                    Jobs.job_name,
	                    WorkingHours.task_id,
	                    Tasks.task_name,
						JobResponsible.levels,
	                    CAST(SUM(
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
	                    ) / 60.0 as decimal(18,1))as hours
                    FROM WorkingHours
                    LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                    LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                    LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
					LEFT JOIN JobResponsible ON JobResponsible.user_id = WorkingHours.user_id AND JobResponsible.job_id = WorkingHours.job_id
                    Where WorkingHours.job_id = '{job_id}'                    
                    GROUP BY WorkingHours.user_id, Authen.name,Authen.user_id,JobResponsible.levels, WorkingHours.job_id, job_name, WorkingHours.task_id, Tasks.task_name
                    ORDER BY WorkingHours.user_id");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ManpowerRatioModel mr = new ManpowerRatioModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            user_name = dr["name"] != DBNull.Value ? dr["name"].ToString() : "",
                            hours = dr["hours"] != DBNull.Value ? Math.Truncate(Convert.ToDouble(dr["hours"])) * (dr["levels"] != DBNull.Value ? Convert.ToInt32(dr["levels"].ToString()) : 1) : 0,
                            percents = 0
                        };
                        mrs.Add(mr);
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
            return mrs;
        }

        public List<ManpowerDistributionModel>  GetManpowerDistribution(string job_id)
        {
            List<ManpowerDistributionModel> mds = new List<ManpowerDistributionModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    SELECT 
	                    WorkingHours.user_id,
	                    Authen.name,
	                    WorkingHours.job_id,
	                    Jobs.job_name,
	                    WorkingHours.task_id,
	                    Tasks.task_name,
	                    CAST(SUM(
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
	                    ) / 60.0 as decimal(18,1))as hours
                    FROM WorkingHours
                    LEFT JOIN Authen ON WorkingHours.user_id = Authen.user_id
                    LEFT JOIN Jobs ON WorkingHours.job_id = Jobs.job_id
                    LEFT JOIN Tasks ON WorkingHours.task_id = Tasks.task_id
                    WHERE WorkingHours.job_id = '{job_id}'
                    GROUP BY WorkingHours.user_id, Authen.name, WorkingHours.job_id, job_name, WorkingHours.task_id, Tasks.task_name
                    ORDER BY user_id");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ManpowerDistributionModel md = new ManpowerDistributionModel()
                        {
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            user_id = dr["user_id"] != DBNull.Value ? dr["user_id"].ToString() : "",
                            user_name = dr["name"] != DBNull.Value ? dr["name"].ToString() : "",
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                            hours = dr["hours"] != DBNull.Value ? Convert.ToDouble(dr["hours"].ToString()) : 0,
                        };
                        mds.Add(md);
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
            return mds;
        }
    }
}