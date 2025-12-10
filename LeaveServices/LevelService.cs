using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;
using WebENG.Service;

namespace WebENG.LeaveServices
{
    public class LevelService : ILevel
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public LevelService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }

        public int CalcuLevelStep(List<LevelModel> levels, RequestModel request, LeaveTypeModel leave)
        {

            bool hasOperation = levels.Any(x => x.level == 0);

            if (request.status_request == "Created" || request.status_request == "Resubmit")
            {
                return hasOperation ? 1 : levels.Min(x => x.level) + 1;
            }

            int current = request.level_step;
            bool isLongLeave = request.is_full_day ? request.amount_leave_day >= leave.max_consecutive_days : (decimal)((double)request.amount_leave_hour / 8.0) >= leave.max_consecutive_days;

            if (hasOperation)
            {
                if (!leave.is_two_step_approve || !isLongLeave)
                    return current + 2;
                else
                    return current + 1;
            }
            else
            {
                return current + 1;
            }
        }

        //public List<LevelModel> GetHierarchyByEmpID(string emp_id)
        //{
        //    List<LevelModel> levels = new List<LevelModel>();
        //    try
        //    {
        //        if (con.State == ConnectionState.Closed)
        //        {
        //            con.Open();
        //        }
        //        string strCmd = string.Format($@"WITH l0 AS (
        //                                            SELECT emp_id,
        //                                                   name_en as emp_name,
        //                                                   position,
        //                                                   department,
        //                                                   CASE WHEN position = 'Operation' OR position = '' THEN 0
        //						   WHEN position LIKE '%Manager%' THEN 1
        //						   WHEN position LIKE '%Director%' THEN 2
        //						   ELSE 0 END as level 
        //                                            FROM [CTL].dbo.Employees
        //                                            WHERE emp_id = @emp_id
        //                                        ),
        //                                        l1 AS (
        //                                            SELECT [ELEAVE].dbo.departments.emp_id,
        //                                                   name_en as emp_name,
        //                                                   position,
        //                                                   [ELEAVE].dbo.departments.department,
        //                                                   level
        //                                            FROM [ELEAVE].dbo.departments 
        //                                         LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.departments.emp_id = emp.emp_id
        //                                            WHERE [ELEAVE].dbo.departments.department = (SELECT department FROM l0)
        //                                        ),
        //                                        l2 AS (
        //                                            SELECT [ELEAVE].dbo.[Approvers].emp_id,
        //                                                   emp.name_en as emp_name,
        //                                                   'Director' as position,
        //                                                    [ELEAVE].dbo.[Approvers].department,
        //                                                    level
        //                                                    FROM [ELEAVE].dbo.[Approvers]
        //                                                    LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.[Approvers].emp_id = emp.emp_id
        //                                           WHERE [ELEAVE].dbo.[Approvers].department = (SELECT department FROM l0)
        //                                        ),
        //                                        l3 AS (
        //                                            SELECT [ELEAVE].dbo.[Checkers].emp_id,
        //                                                   emp.name_en as emp_name,
        //                                                   'Checker' as position,
        //                                                   emp.department,
        //                                                   level
        //                                                   FROM [ELEAVE].dbo.[Checkers]
        //                                                   LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.[Checkers].emp_id = emp.emp_id
        //                                        ),
        //                                        m AS (

        //                                        SELECT * FROM l0
        //                                        UNION ALL
        //                                        SELECT * FROM l1
        //                                        UNION ALL
        //                                        SELECT * FROM l2
        //                                        UNION ALL
        //                                        SELECT * FROM l3
        //                                        )
        //                                        SELECT DISTINCT * FROM m ORDER BY m.level");
        //        SqlCommand command = new SqlCommand(strCmd, con);
        //        command.Parameters.AddWithValue("@emp_id", emp_id);
        //        SqlDataReader dr = command.ExecuteReader();
        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                LevelModel level = new LevelModel()
        //                {
        //                    position = dr["position"].ToString(),
        //                    department = dr["department"].ToString(),
        //                    emp_id = dr["emp_id"].ToString(),
        //                    emp_name = dr["emp_name"].ToString(),
        //                    level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 0
        //                };
        //                levels.Add(level);
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
        //    return levels;
        //}

        public List<LevelModel> GetLevelByEmpID(string emp_id)
        {
            List<LevelModel> levels = new List<LevelModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"WITH l0 AS (SELECT emp_id,
                                                           name_en as emp_name_en,
														   name_th as emp_name_th,
                                                           position,
                                                           department,
                                                           CASE WHEN position = 'Operation' OR position = '' THEN 0
														   WHEN position LIKE '%Manager%' THEN 1
														   WHEN position LIKE '%Director%' THEN 2
														   ELSE 0 END as level ,
														   [CTL].dbo.[Employees].email
                                                    FROM [CTL].dbo.Employees
                                                    WHERE emp_id = @emp_id                                                   
                                                ),
                                                l1 AS (
                                                    SELECT [ELEAVE].dbo.departments.emp_id,
                                                           emp.name_en as emp_name_en,
														   emp.name_th as emp_name_th,
                                                           'Manager' as position,
                                                           [ELEAVE].dbo.departments.department,
                                                           level,
														   emp.email
                                                    FROM [ELEAVE].dbo.departments 
	                                                LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.departments.emp_id = emp.emp_id
													WHERE [ELEAVE].dbo.departments.emp_id = @emp_id    
                                                ),
                                                l2 AS (
                                                    SELECT [ELEAVE].dbo.[Approvers].emp_id,
                                                           emp.name_en as emp_name_en,
														   emp.name_th as emp_name_th,
                                                           'Director' as position,
                                                            [ELEAVE].dbo.[Approvers].department,
                                                            level,
															emp.email
                                                            FROM [ELEAVE].dbo.[Approvers]
                                                            LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.[Approvers].emp_id = emp.emp_id
															WHERE [ELEAVE].dbo.[Approvers].emp_id = @emp_id 
                                                ),
                                                l3 AS (
                                                    SELECT [ELEAVE].dbo.[Checkers].emp_id,
                                                           emp.name_en as emp_name_en,
														   emp.name_th as emp_name_th,
                                                           'Checker' as position,
                                                           emp.department,
                                                           level,
														   emp.email
                                                           FROM [ELEAVE].dbo.[Checkers]
                                                           LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.[Checkers].emp_id = emp.emp_id
                                                ),
                                                m AS (
                                                SELECT * FROM l0
                                                UNION ALL
                                                SELECT * FROM l1
                                                UNION ALL
                                                SELECT * FROM l2
                                                UNION ALL
                                                SELECT * FROM l3
                                                )
                                                SELECT DISTINCT * FROM m  ORDER BY m.level ");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@emp_id", emp_id);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        LevelModel level = new LevelModel()
                        {
                            position = dr["position"].ToString(),
                            department = dr["department"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            emp_name_en = dr["emp_name_en"].ToString(),
                            emp_name_th = dr["emp_name_th"].ToString(),
                            level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 0,
                            email = dr["email"].ToString()
                        };
                        levels.Add(level);
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
            return levels;
        }
    }
}
