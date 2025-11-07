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
        readonly IEmployee Employee;
        readonly IRequest Request;
        public LeaveEntitlementRuleService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
            Employee = new EmployeeService();
            Request = new RequestService();
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

        public LeaveBalanceModel GetLeaveBalances(string emp_id, int year)
        {
            LeaveBalanceModel employees = new LeaveBalanceModel();
            employees.emp_id = emp_id;
            employees.year = year;

            EmployeeModel employee = Employee.GetEmployees().Where(w => w.emp_id == emp_id).FirstOrDefault();
            DateTime start_date = employee.start_date;
            string position = employee.position;

            //Get Request Leave
            List<RequestModel> requests = Request.GetRequestByEmpID(emp_id);
            requests = requests.Where(w => w.status_request == "Approved").ToList();

            List<LeaveEntitlementRuleModel> entitlements = GetLeaveEntitlementRules();
            if (position.Contains("Manager") || position.Contains("Director"))
            {
                entitlements = entitlements.Where(w=>w.position == "Manager").OrderBy(o => o.leave_type_id).ThenBy(t => t.start_age).ToList();
            }
            if (position.Contains("Operation"))
            {
                entitlements = entitlements.Where(w => w.position == "Operation").OrderBy(o => o.leave_type_id).ThenBy(t => t.start_age).ToList();
            }

            double Duration = (DateTime.Now - start_date).TotalDays;
            List<EntitlementBalanceModel> entitlements_balance = new List<EntitlementBalanceModel>();
            if (entitlements.Count > 0)
            {
                List<string> leaves_type_id = entitlements.GroupBy(g => g.leave_type_id).Select(s => s.FirstOrDefault().leave_type_id).ToList();
                for(int i = 0; i < leaves_type_id.Count; i++)
                {
                    
                    List<LeaveEntitlementRuleModel> entitles = entitlements.Where(w => w.leave_type_id == leaves_type_id[i]).ToList();
                    for(int j = 0; j < entitles.Count; j++)
                    {
                        if (Duration >= (double)(entitles[j].start_age * 365) && Duration <= (double)(entitles[j].before_age * 365))
                        {
                            double x_day = (double)requests.Where(w => w.leave_type_code == entitles[j].leave_type_code).Sum(x => x.amount_leave_day);
                            double x_hours = (double)requests.Where(w => w.leave_type_code == entitles[j].leave_type_code).Sum(x => x.amount_leave_hour);
                            double x_all = x_day + (x_hours / 24);
                            EntitlementBalanceModel entitle_b = new EntitlementBalanceModel()
                            {
                                amount = entitles[j].days_per_year,
                                balance = (decimal)(entitles[j].days_per_year - x_all),
                                leave_code = entitles[j].leave_type_code,
                                leave_type_id = entitles[j].leave_type_id,
                                leave_name_th = entitles[j].leave_name_th,
                                leave_name_en = entitles[j].leave_name_en
                            };
                            entitlements_balance.Add(entitle_b);
                            continue;
                        }
                    }
                }
            }
            employees.entitlements = entitlements_balance;
            return employees;
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
                                                          ,[leave_entitlement_rule].[leave_type_id]
														  ,Leave_type.leave_type_code
                                                          ,[start_age]
                                                          ,[before_age]
                                                          ,[days_per_year]
														  ,[is_prorated]
														  ,[prorated_amount_per_year]
														  ,[is_calculate_by_year]
														  ,Leave_type.leave_name_th
														  ,Leave_type.leave_name_en
                                                      FROM [dbo].[leave_entitlement_rule]
													  LEFT JOIN Leave_type ON [leave_entitlement_rule].leave_type_id = Leave_type.leave_type_id");
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
                            leave_type_code = dr["leave_type_code"].ToString(),
                            leave_name_th = dr["leave_name_th"].ToString(),
                            leave_name_en = dr["leave_name_en"].ToString(),
                            start_age = Convert.ToDecimal(dr["start_age"].ToString()),
                            before_age = Convert.ToDecimal(dr["before_age"].ToString()),
                            days_per_year = dr["days_per_year"] != DBNull.Value ? Convert.ToInt32(dr["days_per_year"].ToString()) : 0,
                            //hours_per_year = dr["hours_per_year"] != DBNull.Value ? Convert.ToInt32(dr["hours_per_year"].ToString()) : 0,
                            is_prorated = dr["is_prorated"] != DBNull.Value ? Convert.ToBoolean(dr["is_prorated"].ToString()) : false,
                            prorated_amount_per_year = dr["prorated_amount_per_year"] != DBNull.Value ? Convert.ToInt32(dr["prorated_amount_per_year"].ToString()) : 0,
                            is_calculate_by_year = dr["is_calculate_by_year"] != DBNull.Value ? Convert.ToBoolean(dr["is_calculate_by_year"].ToString()) : false
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
                                                          ,[leave_entitlement_rule].[leave_type_id]
														  ,Leave_type.leave_type_code
                                                          ,[start_age]
                                                          ,[before_age]
                                                          ,[days_per_year]
                                                          ,[is_prorated]
														  ,[prorated_amount_per_year]
														  ,[is_calculate_by_year]
														  ,Leave_type.leave_name_th
														  ,Leave_type.leave_name_en
                                                      FROM [dbo].[leave_entitlement_rule]
													  LEFT JOIN Leave_type ON [leave_entitlement_rule].leave_type_id = Leave_type.leave_type_id
                                                      WHERE [leave_entitlement_rule].[leave_type_id] = @leave_type_id");
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
                            leave_type_code = dr["leave_type_code"].ToString(),
                            leave_name_th = dr["leave_name_th"].ToString(),
                            leave_name_en = dr["leave_name_en"].ToString(),
                            start_age = Convert.ToDecimal(dr["start_age"].ToString()),
                            before_age = Convert.ToDecimal(dr["before_age"].ToString()),
                            days_per_year = dr["days_per_year"] != DBNull.Value ? Convert.ToInt32(dr["days_per_year"].ToString()) : 0,
                            //hours_per_year = dr["hours_per_year"] != DBNull.Value ? Convert.ToInt32(dr["hours_per_year"].ToString()) : 0,
                            is_prorated = dr["is_prorated"] != DBNull.Value ? Convert.ToBoolean(dr["is_prorated"].ToString()) : false,
                            prorated_amount_per_year = dr["prorated_amount_per_year"] != DBNull.Value ? Convert.ToInt32(dr["prorated_amount_per_year"].ToString()) : 0,
                            is_calculate_by_year = dr["is_calculate_by_year"] != DBNull.Value ? Convert.ToBoolean(dr["is_calculate_by_year"].ToString()) : false
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
                                                   ,[is_prorated]
                                                   ,[prorated_amount_per_year]
                                                   ,[is_calculate_by_year])
                                             VALUES
                                                   (@entitlement_rule_id
                                                   ,@position
                                                   ,@leave_type_id
                                                   ,@start_age
                                                   ,@before_age
                                                   ,@days_per_year
                                                   ,@is_prorated
                                                   ,@prorated_amount_per_year
                                                   ,@is_calculate_by_year)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.Add("@entitlement_rule_id",SqlDbType.Text);
                command.Parameters.Add("@position", SqlDbType.Text);
                command.Parameters.Add("@leave_type_id", SqlDbType.Text);
                command.Parameters.Add("@start_age", SqlDbType.Decimal);
                command.Parameters.Add("@before_age", SqlDbType.Decimal);
                command.Parameters.Add("@days_per_year", SqlDbType.Int);
                command.Parameters.Add("@is_prorated", SqlDbType.Bit);
                command.Parameters.Add("@prorated_amount_per_year", SqlDbType.Int);
                command.Parameters.Add("@is_calculate_by_year", SqlDbType.Bit);
                for (int i = 0; i < leaves.Count; i++)
                {
                    command.Parameters[0].Value = leaves[i].entitlement_rule_id;
                    command.Parameters[1].Value = leaves[i].position;
                    command.Parameters[2].Value = leaves[i].leave_type_id;
                    command.Parameters[3].Value = leaves[i].start_age;
                    command.Parameters[4].Value = leaves[i].before_age;
                    command.Parameters[5].Value = leaves[i].days_per_year;
                    command.Parameters[6].Value = leaves[i].is_prorated;
                    command.Parameters[7].Value = leaves[i].prorated_amount_per_year;
                    command.Parameters[8].Value = leaves[i].is_calculate_by_year;
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
