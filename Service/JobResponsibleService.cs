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
    public class JobResponsibleService : IJobResponsible
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public JobResponsibleService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<JobResponsibleModel> GetJobResponsible(string emp_id)
        {
            List<JobResponsibleModel> jrs = new List<JobResponsibleModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT
	                    JobResponsible.job_id,
                        JobResponsible.emp_id,
	                    Jobs.job_name,
                        Jobs.customer_name as customer,
	                    JobResponsible.user_id,
	                    emp.name_en AS user_name,
	                    emp.department,
                        JobResponsible.levels as level,
	                    JobResponsible.role,
	                    JobResponsible.assign_by,
	                    JobResponsible.assign_date
                    FROM JobResponsible
                        LEFT JOIN Jobs ON JobResponsible.job_id = Jobs.job_id
                        LEFT JOIN CTL.dbo.Employees AS emp ON JobResponsible.emp_id = emp.emp_id
                    WHERE JobResponsible.emp_id = '{emp_id}' AND Jobs.status <> 'STA999'
                    ORDER BY JobResponsible.job_id");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        JobResponsibleModel jr = new JobResponsibleModel()
                        {
                            emp_id = dr["emp_id"].ToString(),
                            user_name = dr["user_name"] != DBNull.Value ? dr["user_name"].ToString() : "",
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                            level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 1,
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            role = dr["role"] != DBNull.Value ? dr["role"].ToString() : "",
                            assign_by = dr["assign_by"] != DBNull.Value ? dr["assign_by"].ToString() : "",
                            assign_date = dr["assign_date"] != DBNull.Value ? Convert.ToDateTime(dr["assign_date"]) : default(DateTime),
                        };
                        jrs.Add(jr);
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
            return jrs;
        }

        public List<JobResponsibleModel> GetJobLists()
        {
            List<JobResponsibleModel> jrs = new List<JobResponsibleModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT
	                    Jobs.job_id,
                        Jobs.job_name,
                        Jobs.customer_name as customer,
                        JobResponsible.emp_id,
                        emp.name_en as name
                    FROM Jobs
                    LEFT JOIN JobResponsible ON Jobs.job_id = JobResponsible.job_id
                    LEFT JOIN CTL.dbo.Employees emp ON JobResponsible.emp_id = emp.emp_id
                    ORDER BY Jobs.job_id, JobResponsible.emp_id");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        JobResponsibleModel jr = new JobResponsibleModel()
                        {
                            emp_id = dr["emp_id"].ToString(),
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                            user_name = dr["name"] != DBNull.Value ? dr["name"].ToString() : "",
                        };
                        jrs.Add(jr);
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
            return jrs;
        }

        public List<QuotationResponsibleModel> GetQuotationResponsible(string user_name)
        {
            List<QuotationResponsibleModel> qrs = new List<QuotationResponsibleModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                    SELECT 
	                    Proposal.quotation_no,
	                    Quotation.project_name,
                        Quotation.customer
                    FROM Proposal
	                    LEFT JOIN Quotation ON Proposal.quotation_no = Quotation.quotation_no
                    WHERE Proposal.engineer_in_charge LIKE '%{user_name}%'
                    ORDER BY Proposal.quotation_no");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        QuotationResponsibleModel qr = new QuotationResponsibleModel()
                        {
                            quotation_no = dr["quotation_no"] != DBNull.Value ? dr["quotation_no"].ToString() : "",
                            project_name = dr["project_name"] != DBNull.Value ? dr["project_name"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                        };
                        qrs.Add(qr);
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
            return qrs;
        }

        public List<JobResponsibleModel> GetAssignEngineers(string job_id)
        {
            List<JobResponsibleModel> jrs = new List<JobResponsibleModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT
	                    JobResponsible.job_id,
                        JobResponsible.emp_id,
	                    Jobs.job_name,
                        Jobs.customer_name as customer,
	                    JobResponsible.user_id,
	                    emp.name_en AS user_name,
	                    emp.department,
                        JobResponsible.levels as level,
	                    JobResponsible.role,
	                    JobResponsible.assign_by,
                        JobResponsible.assign_date
                    FROM JobResponsible
                        LEFT JOIN Jobs ON JobResponsible.job_id = Jobs.job_id
                        LEFT JOIN CTL.dbo.Employees AS emp ON JobResponsible.emp_id = emp.emp_id
                    WHERE  JobResponsible.job_id = '{job_id}'
                    ORDER BY JobResponsible.job_id");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        JobResponsibleModel jr = new JobResponsibleModel()
                        {
                            emp_id = dr["emp_id"].ToString(),
                            user_name = dr["user_name"] != DBNull.Value ? dr["user_name"].ToString() : "",
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                            level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 1,
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            role = dr["role"] != DBNull.Value ? dr["role"].ToString() : "",
                            assign_by = dr["assign_by"] != DBNull.Value ? dr["assign_by"].ToString() : "",
                            assign_date = dr["assign_date"] != DBNull.Value ? Convert.ToDateTime(dr["assign_date"].ToString()) : default(DateTime),
                        };
                        jrs.Add(jr);
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
            return jrs;
        }

        public string AddJobResponsible(List<JobResponsibleModel> jrs)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                for (int i = 0; i < jrs.Count; i++)
                {
                    string string_command = string.Format($@"
                    BEGIN
                        IF NOT EXISTS (
                            SELECT job_id, emp_id FROM JobResponsible WHERE emp_id = @emp_id AND job_id = @job_id
                        )
                        BEGIN
                            INSERT INTO JobResponsible(job_id, emp_id, role, levels, assign_by, assign_date)
                            VALUES(@job_id, @emp_id, @role, @levels, @assign_by, @assign_date)
                        END
                    END
                ");


                    using (SqlCommand cmd = new SqlCommand(string_command, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@job_id", jrs[i].job_id.Replace("-", String.Empty));
                        cmd.Parameters.AddWithValue("@emp_id", jrs[i].emp_id);
                        cmd.Parameters.AddWithValue("@role", jrs[i].role);
                        cmd.Parameters.AddWithValue("@levels", jrs[i].level);
                        cmd.Parameters.AddWithValue("@assign_by", jrs[i].assign_by);
                        cmd.Parameters.AddWithValue("@assign_date", jrs[i].assign_date);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
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

        public List<JobResponsibleModel> GetJobsResponsible()
        {
            List<JobResponsibleModel> jrs = new List<JobResponsibleModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT
	                    JobResponsible.job_id,
                        JobResponsible.emp_id,
	                    Jobs.job_name,
                        Jobs.customer_name as customer,
	                    emp.name_en AS user_name,
	                    emp.department,
                        JobResponsible.levels as level,
	                    JobResponsible.role,
	                    JobResponsible.assign_by,
	                    JobResponsible.assign_date
                    FROM JobResponsible
                        LEFT JOIN Jobs ON JobResponsible.job_id = Jobs.job_id
                        LEFT JOIN CTL.dbo.Employees AS emp ON JobResponsible.emp_id = emp.emp_id
                    WHERE Jobs.status <> 'STA999'
                    ORDER BY JobResponsible.job_id");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        JobResponsibleModel jr = new JobResponsibleModel()
                        {
                            emp_id = dr["emp_id"].ToString(),
                            user_name = dr["user_name"] != DBNull.Value ? dr["user_name"].ToString() : "",
                            job_id = dr["job_id"] != DBNull.Value ? dr["job_id"].ToString() : "",
                            job_name = dr["job_name"] != DBNull.Value ? dr["job_name"].ToString() : "",
                            customer = dr["customer"] != DBNull.Value ? dr["customer"].ToString() : "",
                            level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 1,
                            department = dr["department"] != DBNull.Value ? dr["department"].ToString() : "",
                            role = dr["role"] != DBNull.Value ? dr["role"].ToString() : "",
                            assign_by = dr["assign_by"] != DBNull.Value ? dr["assign_by"].ToString() : "",
                            assign_date = dr["assign_date"] != DBNull.Value ? Convert.ToDateTime(dr["assign_date"]) : default(DateTime),
                        };
                        jrs.Add(jr);
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
            return jrs;
        }
    }
}
