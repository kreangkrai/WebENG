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

        public string Delete(string leave_id)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"DELETE FROM [dbo].[leave_entitlement_rule] WHERE leave_type_id = @leave_type_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@leave_type_id", leave_id);
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
                                                          ,[position]
                                                          ,[leave_type_id]
                                                          ,[start_age]
                                                          ,[before_age]
                                                          ,[days_per_year]
                                                          ,[hours_per_year]
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
                            position = dr["position"].ToString(),
                            leave_type_id = dr["leave_type_id"].ToString(),
                            start_age = Convert.ToDecimal(dr["start_age"].ToString()),
                            before_age = Convert.ToDecimal(dr["before_age"].ToString()),
                            days_per_year = dr["days_per_year"] != DBNull.Value ? Convert.ToInt32(dr["days_per_year"].ToString()) : 1,
                            hours_per_year = dr["hours_per_year"] != DBNull.Value ? Convert.ToInt32(dr["hours_per_year"].ToString()) : 1
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

        public List<LeaveEntitlementRuleModel> GetLeaveEntitlementRulesByLeaveID(string leave_type_id)
        {
            List<LeaveEntitlementRuleModel> leaves = new List<LeaveEntitlementRuleModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [entitlement_rule_id]
                                                          ,[position]
                                                          ,[leave_type_id]
                                                          ,[start_age]
                                                          ,[before_age]
                                                          ,[days_per_year]
                                                          ,[hours_per_year]
                                                      FROM [dbo].[leave_entitlement_rule]
                                                      WHERE [leave_type_id] = @leave_type_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@leave_type_id", leave_type_id);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        LeaveEntitlementRuleModel leave = new LeaveEntitlementRuleModel()
                        {
                            entitlement_rule_id = dr["entitlement_rule_id"].ToString(),
                            position = dr["position"].ToString(),
                            leave_type_id = dr["leave_type_id"].ToString(),
                            start_age = Convert.ToDecimal(dr["start_age"].ToString()),
                            before_age = Convert.ToDecimal(dr["before_age"].ToString()),
                            days_per_year = dr["days_per_year"] != DBNull.Value ? Convert.ToInt32(dr["days_per_year"].ToString()) : 1,
                            hours_per_year = dr["hours_per_year"] != DBNull.Value ? Convert.ToInt32(dr["hours_per_year"].ToString()) : 1
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

        public string Insert(List<LeaveEntitlementRuleModel> leaves)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [ELEAVE].[dbo].[leave_entitlement_rule]
                                                   ([entitlement_rule_id]
                                                   ,[position]
                                                   ,[leave_type_id]
                                                   ,[start_age]
                                                   ,[before_age]
                                                   ,[days_per_year]
                                                   ,[hours_per_year])
                                             VALUES
                                                   (@entitlement_rule_id
                                                   ,@position
                                                   ,@leave_type_id
                                                   ,@start_age
                                                   ,@before_age
                                                   ,@days_per_year
                                                   ,@hours_per_year)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.Add("@entitlement_rule_id",SqlDbType.Text);
                command.Parameters.Add("@position", SqlDbType.Text);
                command.Parameters.Add("@leave_type_id", SqlDbType.Text);
                command.Parameters.Add("@start_age", SqlDbType.Decimal);
                command.Parameters.Add("@before_age", SqlDbType.Decimal);
                command.Parameters.Add("@days_per_year", SqlDbType.Int);
                command.Parameters.Add("@hours_per_year", SqlDbType.Int);
                for(int i = 0; i < leaves.Count; i++)
                {
                    command.Parameters[0].Value = leaves[i].entitlement_rule_id;
                    command.Parameters[1].Value = leaves[i].position;
                    command.Parameters[2].Value = leaves[i].leave_type_id;
                    command.Parameters[3].Value = leaves[i].start_age;
                    command.Parameters[4].Value = leaves[i].before_age;
                    command.Parameters[5].Value = leaves[i].days_per_year;
                    command.Parameters[6].Value = leaves[i].hours_per_year;
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
