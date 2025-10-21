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
    public class LeaveEntitlementRuleService : ILeaveEntitlementRule
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public LeaveEntitlementRuleService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<LeaveEntitlementRuleModel> GetLeaveEntitlementRules()
        {
            List<LeaveEntitlementRuleModel> leaves = new List<LeaveEntitlementRuleModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [entitlement_rule_id]
                                                          ,[leave_type_id]
                                                          ,[position_group_id]
                                                          ,[min_tenure_months]
                                                          ,[max_tenure_months]
                                                          ,[gender_filter]
                                                          ,[days_per_year]
                                                          ,[is_active]
                                                          ,[created_at]
                                                          ,[created_by]
                                                          ,[updated_at]
                                                          ,[updated_by]
                                                      FROM [dbo].[leave_entitlement_rule]");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        LeaveEntitlementRuleModel leave = new LeaveEntitlementRuleModel()
                        {
                            entitlement_rule_id = dr["entitlement_rule_id"].ToString(),
                            leave_type_id = dr["leave_type_id"].ToString(),
                            position_group_id = dr["position_group_id"].ToString(),
                            min_tenure_months = dr["min_tenure_months"] != DBNull.Value ? Convert.ToInt32(dr["min_tenure_months"].ToString()) : 1,
                            max_tenure_months = dr["max_tenure_months"] != DBNull.Value ? Convert.ToInt32(dr["max_tenure_months"].ToString()) : 1,
                            gender_filter = dr["gender_filter"].ToString(),
                            days_per_year = dr["days_per_year"] != DBNull.Value ? Convert.ToDecimal(dr["days_per_year"].ToString()) : 1,
                            is_active = dr["is_active"] != DBNull.Value ? Convert.ToBoolean(dr["is_active"].ToString()) : false,
                            created_at = dr["created_at"] != DBNull.Value ? Convert.ToDateTime(dr["created_at"].ToString()) : DateTime.MinValue,
                            created_by = dr["created_by"].ToString(),
                            updated_at = dr["updated_at"] != DBNull.Value ? Convert.ToDateTime(dr["updated_at"].ToString()) : DateTime.MinValue,
                            updated_by = dr["updated_by"].ToString()
                        };
                        leaves.Add(leave);
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
            return leaves;
        }

        public string Insert(LeaveEntitlementRuleModel leave)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [ELEAVE].[dbo].[leave_entitlement_rule]
                                                   ([entitlement_rule_id]
                                                   ,[leave_type_id]
                                                   ,[position_group_id]
                                                   ,[min_tenure_months]
                                                   ,[max_tenure_months]
                                                   ,[gender_filter]
                                                   ,[days_per_year]
                                                   ,[is_active]
                                                   ,[created_at]
                                                   ,[created_by]
                                                   ,[updated_at]
                                                   ,[updated_by])
                                             VALUES
                                                   (@entitlement_rule_id
                                                   ,@leave_type_id
                                                   ,@position_group_id
                                                   ,@min_tenure_months
                                                   ,@max_tenure_months
                                                   ,@gender_filter
                                                   ,@days_per_year
                                                   ,@is_active
                                                   ,@created_at
                                                   ,@created_by
                                                   ,@updated_at
                                                   ,@updated_by)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@entitlement_rule_id", leave.entitlement_rule_id);
                command.Parameters.AddWithValue("@leave_type_id", leave.leave_type_id);
                command.Parameters.AddWithValue("@position_group_id", leave.position_group_id);
                command.Parameters.AddWithValue("@min_tenure_months", leave.min_tenure_months);
                command.Parameters.AddWithValue("@max_tenure_months", leave.max_tenure_months);
                command.Parameters.AddWithValue("@gender_filter", leave.gender_filter);
                command.Parameters.AddWithValue("@days_per_year", leave.days_per_year);
                command.Parameters.AddWithValue("@is_active", leave.is_active);
                command.Parameters.AddWithValue("@created_at", leave.created_at);
                command.Parameters.AddWithValue("@created_by", leave.created_by);
                command.Parameters.AddWithValue("@updated_at", leave.updated_at);
                command.Parameters.AddWithValue("@updated_by", leave.updated_by);
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

        public string Update(LeaveEntitlementRuleModel leave)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"UPDATE [dbo].[leave_entitlement_rule]
                                                   SET [leave_type_id] = @leave_type_id
                                                      ,[position_group_id] = @position_group_id
                                                      ,[min_tenure_months] = @min_tenure_months
                                                      ,[max_tenure_months] = @max_tenure_months
                                                      ,[gender_filter] = @gender_filter
                                                      ,[days_per_year] = @days_per_year
                                                      ,[is_active] = @is_active
                                                      ,[created_at] = @created_at
                                                      ,[created_by] = @created_by
                                                      ,[updated_at] = @updated_at
                                                      ,[updated_by] = @updated_by
                                                 WHERE [entitlement_rule_id] = @entitlement_rule_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@entitlement_rule_id", leave.entitlement_rule_id);
                command.Parameters.AddWithValue("@leave_type_id", leave.leave_type_id);
                command.Parameters.AddWithValue("@position_group_id", leave.position_group_id);
                command.Parameters.AddWithValue("@min_tenure_months", leave.min_tenure_months);
                command.Parameters.AddWithValue("@max_tenure_months", leave.max_tenure_months);
                command.Parameters.AddWithValue("@gender_filter", leave.gender_filter);
                command.Parameters.AddWithValue("@days_per_year", leave.days_per_year);
                command.Parameters.AddWithValue("@is_active", leave.is_active);
                command.Parameters.AddWithValue("@created_at", leave.created_at);
                command.Parameters.AddWithValue("@created_by", leave.created_by);
                command.Parameters.AddWithValue("@updated_at", leave.updated_at);
                command.Parameters.AddWithValue("@updated_by", leave.updated_by);
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
