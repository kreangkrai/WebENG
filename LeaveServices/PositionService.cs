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
    public class PositionService : IPosition
    {
        readonly CTLInterfaces.IEmployee Employee;
        ConnectSQL connect = null;
        SqlConnection con = null;
        public PositionService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
            Employee = new CTLServices.EmployeeService();
        }
        public string delete(string emp_id)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"DELETE FROM [dbo].[position]
                                                        WHERE [emp_id] = @emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@emp_id", emp_id);
                command.ExecuteNonQuery();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return "Success";
        }

        public List<LeavePositionModel> GetPositions()
        {
            List<CTLModels.EmpModel> emps = Employee.GetEmps();
            List<CTLModels.EmployeeModel> employees = Employee.GetEmployees();
            List<PositionModel> positions = new List<PositionModel>();
            List<LeavePositionModel> _positions = new List<LeavePositionModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"
                                                SELECT [position_id]
                                                      ,[position].[emp_id]
	                                                  ,employees.name_en as emp_name
                                                      ,[level]
                                                      ,[department]
                                                      ,[is_active]
                                                  FROM [dbo].[position]
                                                  LEFT JOIN employees ON position.emp_id = employees.emp_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        PositionModel position = new PositionModel()
                        {
                            position_id = Int32.Parse(dr["position_id"].ToString()),
                            emp_name = dr["emp_name"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            level = dr["level"].ToString(),
                            department = dr["department"].ToString(),
                            is_active = dr["is_active"] != DBNull.Value ? Convert.ToBoolean(dr["is_active"].ToString()) : false,
                            img = emps.Where(w=>w.emp_id == dr["emp_id"].ToString()).Select(x=>x.img).FirstOrDefault(),
                            position = employees.Where(w => w.emp_id == dr["emp_id"].ToString()).Select(x => x.position).FirstOrDefault(),
                        };
                        positions.Add(position);
                    }
                    dr.Close();

                    _positions = positions.GroupBy(g => g.emp_id).Select(s => new LeavePositionModel()
                    {
                        emp_id = s.Key,
                        emp_name = s.FirstOrDefault().emp_name,
                        img = s.FirstOrDefault().img,
                        position = s.FirstOrDefault().position,
                        is_active = s.FirstOrDefault().is_active,
                        is_director = s.Any(a=>a.level == "Director"),
                        is_auditor = s.Any(a => a.level == "Auditor"),
                        manager_departments = s.Where(x=>x.level == "Manager").Select(c=>c.department).ToList()
                    }).ToList();
                }
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return _positions;
        }

        public string insert(List<PositionModel> positions)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [dbo].[position]
                                                       ([emp_id]
                                                       ,[level]
                                                       ,[department]
                                                       ,[is_active])
                                                 VALUES
                                                       (@emp_id
                                                       ,@level
                                                       ,@department
                                                       ,@is_active)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.Add("@emp_id", SqlDbType.Text);
                command.Parameters.Add("@level", SqlDbType.Text);
                command.Parameters.Add("@department", SqlDbType.Text);
                command.Parameters.Add("@is_active", SqlDbType.Bit);
                for(int i = 0; i < positions.Count; i++)
                {
                    command.Parameters[0].Value = positions[i].emp_id;
                    command.Parameters[1].Value = positions[i].level;
                    command.Parameters[2].Value = positions[i].department;
                    command.Parameters[3].Value = positions[i].is_active;
                    command.ExecuteNonQuery();
                }

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return "Success";
        }
    }
}
