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
    public class DepartmentApprovalRuleService : IDepartmentApprovalRule
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public DepartmentApprovalRuleService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<DepartmentApprovalRuleModel> GetDepartmentApprovalRules()
        {
            List<DepartmentApprovalRuleModel> departments = new List<DepartmentApprovalRuleModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT 
                                                department_id,
                                                admin_id
                                                FROM department_approval_rule");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DepartmentApprovalRuleModel department = new DepartmentApprovalRuleModel()
                        {
                            department_id = dr["department_id"].ToString(),
                            admin_id = dr["admin_id"].ToString()
                        };
                        departments.Add(department);
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
            return departments;
        }

        public string Insert(DepartmentApprovalRuleModel department)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO department_approval_rule(department_id,
                                                    admin_id)
                                                VALUES (@department_id,
                                                    @admin_id)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@department_id", department.department_id);
                command.Parameters.AddWithValue("@admin_id", department.admin_id);
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

        public string Update(DepartmentApprovalRuleModel department)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"UPDATE department_approval_rule SET
                                                    admin_id = @admin_id
                                                WHERE department_id = @department_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@department_id", department.department_id);
                command.Parameters.AddWithValue("@admin_id", department.admin_id);
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
    }
}
