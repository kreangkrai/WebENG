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

        public List<LevelModel> GetHierarchyByEmpID(string emp_id)
        {
            List<LevelModel> levels = new List<LevelModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"WITH l0 AS (
                                                    SELECT emp_id,
                                                           name_en as emp_name,
                                                           position,
                                                           department,
                                                           CASE WHEN position = 'Operation' OR position = '' THEN 0 ELSE 1 END as level 
                                                    FROM [CTL].dbo.Employees
                                                    WHERE emp_id = @emp_id
                                                ),
                                                l1 AS (
                                                    SELECT [ELEAVE].dbo.departments.emp_id,
                                                           name_en as emp_name,
                                                           position,
                                                           [ELEAVE].dbo.departments.department,
                                                           level
                                                    FROM [ELEAVE].dbo.departments 
	                                                LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.departments.emp_id = emp.emp_id
                                                    WHERE [ELEAVE].dbo.departments.department = (SELECT department FROM l0)
                                                ),
                                                l2 AS (
                                                    SELECT [ELEAVE].dbo.[Approvers].emp_id,
                                                           emp.name_en as emp_name,
                                                           'Director' as position,
                                                            [ELEAVE].dbo.[Approvers].department,
                                                            level
                                                            FROM [ELEAVE].dbo.[Approvers]
                                                            LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.[Approvers].emp_id = emp.emp_id
			                                                WHERE [ELEAVE].dbo.[Approvers].department = (SELECT department FROM l0)
                                                ),
                                                l3 AS (
                                                    SELECT [ELEAVE].dbo.[Checkers].emp_id,
                                                           emp.name_en as emp_name,
                                                           'Checker' as position,
                                                           emp.department,
                                                           level
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
                                                SELECT DISTINCT * FROM m ORDER BY m.level");
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
                            emp_name = dr["emp_name"].ToString(),
                            level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 0
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

        public List<LevelModel> GetLevelByEmpID(string emp_id)
        {
            List<LevelModel> levels = new List<LevelModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"WITH main AS (
                                                    SELECT emp_id,
                                                    name_en as emp_name,
                                                    position,
                                                    department ,
                                                    CASE WHEN position = 'Operation' OR position = '' THEN 0 ELSE 1 END as level 
                                                    FROM [CTL].dbo.Employees
                                                    UNION ALL
                                                    SELECT [ELEAVE].dbo.[Approvers].emp_id,
                                                    emp.name_en as emp_name,
                                                    'Director' as position,
                                                    [ELEAVE].dbo.[Approvers].department,
                                                    level
                                                    FROM [ELEAVE].dbo.[Approvers]
                                                    LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.[Approvers].emp_id = emp.emp_id
                                                    UNION ALL 
                                                    SELECT [ELEAVE].dbo.[Checkers].emp_id,
                                                    emp.name_en as emp_name,
                                                    'Checker' as position,
                                                    emp.department,
                                                    level
                                                    FROM [ELEAVE].dbo.[Checkers]
                                                    LEFT JOIN [CTL].dbo.[Employees] emp ON [ELEAVE].dbo.[Checkers].emp_id = emp.emp_id
                                                )
                                                SELECT * FROM main WHERE main.emp_id = @emp_id ORDER BY main.emp_name");
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
                            emp_name = dr["emp_name"].ToString(),
                            level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 0
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
