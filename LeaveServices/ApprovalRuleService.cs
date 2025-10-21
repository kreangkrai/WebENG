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
    public class ApprovalRuleService : IApprovalRule
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public ApprovalRuleService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<ApprovalRuleModel> GetApprovalRules()
        {
            List<ApprovalRuleModel> approvals = new List<ApprovalRuleModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT approval_rule_id
                                                  ,leave_type_id
                                                  ,min_consecutive_days
                                                  ,amount_step
                                                  ,is_active
                                                  ,created_at
                                                  ,created_by
                                                  ,updated_at
                                                  ,updated_by
                                              FROM [ELEAVE].[dbo].[approval_rule]");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ApprovalRuleModel approval = new ApprovalRuleModel()
                        {
                            approval_rule_id = dr["approval_rule_id"].ToString(),
                            leave_type_id = dr["leave_type_id"].ToString(),
                            min_consecutive_days = dr["min_consecutive_days"] != DBNull.Value ? Convert.ToDecimal(dr["min_consecutive_days"].ToString()) : 1,
                            amount_step = dr["amount_step"] != DBNull.Value ? Convert.ToInt32(dr["amount_step"].ToString()) : 0,
                            is_active = dr["is_active"] != DBNull.Value ? Convert.ToBoolean(dr["is_active"].ToString()) : false,
                            created_at = dr["created_at"] != DBNull.Value ? Convert.ToDateTime(dr["created_at"].ToString()) : DateTime.MinValue,
                            created_by = dr["created_by"].ToString(),
                            updated_at = dr["updated_at"] != DBNull.Value ? Convert.ToDateTime(dr["updated_at"].ToString()) : DateTime.MinValue,
                            updated_by = dr["updated_by"].ToString()
                        };
                        approvals.Add(approval);
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
            return approvals;
        }

        public string Insert(ApprovalRuleModel approval)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [ELEAVE].[dbo].[approval_rule]
                                               ([approval_rule_id]
                                               ,[leave_type_id]
                                               ,[min_consecutive_days]
                                               ,[amount_step]
                                               ,[is_active]
                                               ,[created_at]
                                               ,[created_by]
                                               ,[updated_at]
                                               ,[updated_by])
                                         VALUES
                                               (@approval_rule_id
                                               ,@leave_type_id
                                               ,@min_consecutive_days
                                               ,@amount_step
                                               ,@is_active
                                               ,@created_at
                                               ,@created_by
                                               ,@updated_at
                                               ,@updated_by)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@approval_rule_id", approval.approval_rule_id);
                command.Parameters.AddWithValue("@leave_type_id", approval.leave_type_id);
                command.Parameters.AddWithValue("@min_consecutive_days", approval.min_consecutive_days);
                command.Parameters.AddWithValue("@amount_step", approval.amount_step);
                command.Parameters.AddWithValue("@is_active", approval.is_active);
                command.Parameters.AddWithValue("@created_at", approval.created_at);
                command.Parameters.AddWithValue("@created_by", approval.created_by);
                command.Parameters.AddWithValue("@updated_at", approval.updated_at);
                command.Parameters.AddWithValue("@updated_by", approval.updated_by);
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

        public string Update(ApprovalRuleModel approval)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"UPDATE [ELEAVE].[dbo].[approval_rule]
                                                   SET [leave_type_id] = @leave_type_id
                                                      ,[min_consecutive_days] = @min_consecutive_days
                                                      ,[amount_step] = @amount_step
                                                      ,[is_active] = @is_active
                                                      ,[created_at] = @created_at
                                                      ,[created_by] = @created_by
                                                      ,[updated_at] = @updated_at
                                                      ,[updated_by] = @updated_by
                                                 WHERE [approval_rule_id] = @approval_rule_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@approval_rule_id", approval.approval_rule_id);
                command.Parameters.AddWithValue("@leave_type_id", approval.leave_type_id);
                command.Parameters.AddWithValue("@min_consecutive_days", approval.min_consecutive_days);
                command.Parameters.AddWithValue("@amount_step", approval.amount_step);
                command.Parameters.AddWithValue("@is_active", approval.is_active);
                command.Parameters.AddWithValue("@created_at", approval.created_at);
                command.Parameters.AddWithValue("@created_by", approval.created_by);
                command.Parameters.AddWithValue("@updated_at", approval.updated_at);
                command.Parameters.AddWithValue("@updated_by", approval.updated_by);
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
