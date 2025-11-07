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
    public class LeaveTypeService : ILeaveType
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public LeaveTypeService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<LeaveTypeModel> GetLeaveTypes()
        {
            List<LeaveTypeModel> leaves = new List<LeaveTypeModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [leave_type_id]
                                                          ,[leave_type_code]
                                                          ,[leave_name_th]
                                                          ,[leave_name_en]
                                                          ,[description]
                                                          ,[min_request_hours]
                                                          ,[request_timing]
                                                          ,[is_consecutive]
                                                          ,[max_consecutive_days]
                                                          ,[gender_restriction]
                                                          ,[attachment_required]
                                                          ,[attachment_threshold_days]
                                                          ,[count_holidays_as_leave]
                                                          ,[is_two_step_approve]
                                                          ,[over_consecutive_days_for_two_step]
                                                          ,[is_unpaid]
                                                          ,[is_active]
                                                          ,[created_at]
                                                          ,emp1.name_en as created_by
                                                          ,[updated_at]
                                                          ,emp2.name_en as updated_by
                                                      FROM [dbo].[leave_type]
													  LEFT JOIN Employees emp1 ON [dbo].[leave_type].[created_by] = emp1.emp_id
													  LEFT JOIN Employees emp2 ON [dbo].[leave_type].[updated_by] = emp2.emp_id
													  WHERE [leave_type_id] IS NOT NULL");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        LeaveTypeModel leave = new LeaveTypeModel()
                        {
                            leave_type_id = dr["leave_type_id"].ToString(),
                            leave_type_code = dr["leave_type_code"].ToString(),
                            leave_name_en = dr["leave_name_en"].ToString(),
                            leave_name_th = dr["leave_name_th"].ToString(),
                            description = dr["description"].ToString(),
                            min_request_hours = dr["min_request_hours"] != DBNull.Value ? Convert.ToDecimal(dr["min_request_hours"].ToString()) : 0,
                            request_timing = dr["request_timing"].ToString(),
                            is_consecutive = dr["is_consecutive"] != DBNull.Value ? Convert.ToBoolean(dr["is_consecutive"].ToString()) : false,
                            max_consecutive_days = dr["max_consecutive_days"] != DBNull.Value ? Convert.ToDecimal(dr["max_consecutive_days"].ToString()) : 0,
                            gender_restriction = dr["gender_restriction"].ToString(),
                            attachment_required = dr["attachment_required"] != DBNull.Value ? Convert.ToBoolean(dr["attachment_required"].ToString()) : false,
                            attachment_threshold_days = dr["attachment_threshold_days"] != DBNull.Value ? Convert.ToDecimal(dr["attachment_threshold_days"].ToString()) : 0,
                            count_holidays_as_leave = dr["count_holidays_as_leave"] != DBNull.Value ? Convert.ToBoolean(dr["count_holidays_as_leave"].ToString()) : false,
                            is_two_step_approve = dr["is_two_step_approve"] != DBNull.Value ? Convert.ToBoolean(dr["is_two_step_approve"].ToString()) : false,
                            over_consecutive_days_for_two_step = dr["over_consecutive_days_for_two_step"] != DBNull.Value ? Convert.ToDecimal(dr["over_consecutive_days_for_two_step"].ToString()) : 0,
                            is_unpaid = dr["is_unpaid"] != DBNull.Value ? Convert.ToBoolean(dr["is_unpaid"].ToString()) : false,
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
        public LeaveTypeModel GetLeaveTypeByID(string leave_type_id)
        {
            LeaveTypeModel leave = new LeaveTypeModel();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [leave_type_id]
                                                          ,[leave_type_code]
                                                          ,[leave_name_th]
                                                          ,[leave_name_en]
                                                          ,[description]
                                                          ,[min_request_hours]
                                                          ,[request_timing]
                                                          ,[is_consecutive]
                                                          ,[max_consecutive_days]
                                                          ,[gender_restriction]
                                                          ,[attachment_required]
                                                          ,[attachment_threshold_days]
                                                          ,[count_holidays_as_leave]
                                                          ,[is_two_step_approve]
                                                          ,[over_consecutive_days_for_two_step]
                                                          ,[is_unpaid]
                                                          ,[is_active]
                                                          ,[created_at]
                                                          ,emp1.name_en as created_by
                                                          ,[updated_at]
                                                          ,emp2.name_en as updated_by
                                                      FROM [dbo].[leave_type]
													  LEFT JOIN Employees emp1 ON [dbo].[leave_type].[created_by] = emp1.emp_id
													  LEFT JOIN Employees emp2 ON [dbo].[leave_type].[updated_by] = emp2.emp_id
													  WHERE [leave_type_id] = @leave_type_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@leave_type_id", leave_type_id);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        leave = new LeaveTypeModel()
                        {
                            leave_type_id = dr["leave_type_id"].ToString(),
                            leave_type_code = dr["leave_type_code"].ToString(),
                            leave_name_en = dr["leave_name_en"].ToString(),
                            leave_name_th = dr["leave_name_th"].ToString(),
                            description = dr["description"].ToString(),
                            min_request_hours = dr["min_request_hours"] != DBNull.Value ? Convert.ToDecimal(dr["min_request_hours"].ToString()) : 0,
                            request_timing = dr["request_timing"].ToString(),
                            is_consecutive = dr["is_consecutive"] != DBNull.Value ? Convert.ToBoolean(dr["is_consecutive"].ToString()) : false,
                            max_consecutive_days = dr["max_consecutive_days"] != DBNull.Value ? Convert.ToDecimal(dr["max_consecutive_days"].ToString()) : 0,
                            gender_restriction = dr["gender_restriction"].ToString(),
                            attachment_required = dr["attachment_required"] != DBNull.Value ? Convert.ToBoolean(dr["attachment_required"].ToString()) : false,
                            attachment_threshold_days = dr["attachment_threshold_days"] != DBNull.Value ? Convert.ToDecimal(dr["attachment_threshold_days"].ToString()) : 0,
                            count_holidays_as_leave = dr["count_holidays_as_leave"] != DBNull.Value ? Convert.ToBoolean(dr["count_holidays_as_leave"].ToString()) : false,
                            is_two_step_approve = dr["is_two_step_approve"] != DBNull.Value ? Convert.ToBoolean(dr["is_two_step_approve"].ToString()) : false,
                            over_consecutive_days_for_two_step = dr["over_consecutive_days_for_two_step"] != DBNull.Value ? Convert.ToDecimal(dr["over_consecutive_days_for_two_step"].ToString()) : 0,
                            is_unpaid = dr["is_unpaid"] != DBNull.Value ? Convert.ToBoolean(dr["is_unpaid"].ToString()) : false,
                            is_active = dr["is_active"] != DBNull.Value ? Convert.ToBoolean(dr["is_active"].ToString()) : false,
                            created_at = dr["created_at"] != DBNull.Value ? Convert.ToDateTime(dr["created_at"].ToString()) : DateTime.MinValue,
                            created_by = dr["created_by"].ToString(),
                            updated_at = dr["updated_at"] != DBNull.Value ? Convert.ToDateTime(dr["updated_at"].ToString()) : DateTime.MinValue,
                            updated_by = dr["updated_by"].ToString()
                        };
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
            return leave;
        }
        public string Insert(LeaveTypeModel leave)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"INSERT INTO [ELEAVE].[dbo].[leave_type]
                                               ([leave_type_id]
                                               ,[leave_type_code]
                                               ,[leave_name_th]
                                               ,[leave_name_en]
                                               ,[description]
                                               ,[min_request_hours]
                                               ,[request_timing]
                                               ,[is_consecutive]
                                               ,[max_consecutive_days]
                                               ,[gender_restriction]
                                               ,[attachment_required]
                                               ,[attachment_threshold_days]
                                               ,[count_holidays_as_leave]
                                               ,[is_two_step_approve]
                                               ,[over_consecutive_days_for_two_step]
                                               ,[is_unpaid]
                                               ,[is_active]
                                               ,[created_at]
                                               ,[created_by]
                                               ,[updated_at]
                                               ,[updated_by])
                                         VALUES
                                               (@leave_type_id
                                               ,@leave_type_code
                                               ,@leave_name_th
                                               ,@leave_name_en
                                               ,@description
                                               ,@min_request_hours
                                               ,@request_timing
                                               ,@is_consecutive
                                               ,@max_consecutive_days
                                               ,@gender_restriction
                                               ,@attachment_required
                                               ,@attachment_threshold_days
                                               ,@count_holidays_as_leave
                                               ,@is_two_step_approve
                                               ,@over_consecutive_days_for_two_step
                                               ,@is_unpaid
                                               ,@is_active
                                               ,@created_at
                                               ,@created_by
                                               ,@updated_at
                                               ,@updated_by)");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@leave_type_id", leave.leave_type_id);
                command.Parameters.AddWithValue("@leave_type_code", leave.leave_type_code);
                command.Parameters.AddWithValue("@leave_name_th", leave.leave_name_th);
                command.Parameters.AddWithValue("@leave_name_en", leave.leave_name_en);
                command.Parameters.AddWithValue("@description", leave.description);
                command.Parameters.AddWithValue("@min_request_hours", leave.min_request_hours);
                command.Parameters.AddWithValue("@request_timing", leave.request_timing);
                command.Parameters.AddWithValue("@is_consecutive", leave.is_consecutive);
                command.Parameters.AddWithValue("@max_consecutive_days", leave.max_consecutive_days);
                command.Parameters.AddWithValue("@gender_restriction", leave.gender_restriction);
                command.Parameters.AddWithValue("@attachment_required", leave.attachment_required);
                command.Parameters.AddWithValue("@attachment_threshold_days", leave.attachment_threshold_days);
                command.Parameters.AddWithValue("@count_holidays_as_leave", leave.count_holidays_as_leave);
                command.Parameters.AddWithValue("@is_two_step_approve", leave.is_two_step_approve);
                command.Parameters.AddWithValue("@over_consecutive_days_for_two_step", leave.over_consecutive_days_for_two_step);
                command.Parameters.AddWithValue("@is_unpaid", leave.is_unpaid);
                command.Parameters.AddWithValue("@is_active", leave.is_active);
                command.Parameters.AddWithValue("@created_at", leave.created_at);
                command.Parameters.AddWithValue("@created_by", leave.created_by);
                command.Parameters.AddWithValue("@updated_at", leave.updated_at);
                command.Parameters.AddWithValue("@updated_by", leave.updated_by);
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
        public string Update(LeaveTypeModel leave)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"UPDATE [dbo].[leave_type]
                                                   SET [leave_type_code] = @leave_type_code
                                                      ,[leave_name_th] = @leave_name_th
                                                      ,[leave_name_en] = @leave_name_en
                                                      ,[description] = @description
                                                      ,[min_request_hours] = @min_request_hours
                                                      ,[request_timing] = @request_timing
                                                      ,[is_consecutive] = @is_consecutive
                                                      ,[max_consecutive_days] = @max_consecutive_days
                                                      ,[gender_restriction] = @gender_restriction
                                                      ,[attachment_required] = @attachment_required
                                                      ,[attachment_threshold_days] = @attachment_threshold_days
                                                      ,[count_holidays_as_leave] = @count_holidays_as_leave
                                                      ,[is_two_step_approve] = @is_two_step_approve
                                                      ,[over_consecutive_days_for_two_step] = @over_consecutive_days_for_two_step
                                                      ,[is_unpaid] = @is_unpaid
                                                      ,[is_active] = @is_active
                                                      ,[created_at] = @created_at
                                                      ,[created_by] = @created_by
                                                      ,[updated_at] = @updated_at
                                                      ,[updated_by] = @updated_by
                                                 WHERE [leave_type_id] = @leave_type_id");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@leave_type_id", leave.leave_type_id);
                command.Parameters.AddWithValue("@leave_type_code", leave.leave_type_code);
                command.Parameters.AddWithValue("@leave_name_th", leave.leave_name_th);
                command.Parameters.AddWithValue("@leave_name_en", leave.leave_name_en);
                command.Parameters.AddWithValue("@description", leave.description);
                command.Parameters.AddWithValue("@min_request_hours", leave.min_request_hours);
                command.Parameters.AddWithValue("@request_timing", leave.request_timing);
                command.Parameters.AddWithValue("@is_consecutive", leave.is_consecutive);
                command.Parameters.AddWithValue("@max_consecutive_days", leave.max_consecutive_days);
                command.Parameters.AddWithValue("@gender_restriction", leave.gender_restriction);
                command.Parameters.AddWithValue("@attachment_required", leave.attachment_required);
                command.Parameters.AddWithValue("@attachment_threshold_days", leave.attachment_threshold_days);
                command.Parameters.AddWithValue("@count_holidays_as_leave", leave.count_holidays_as_leave);
                command.Parameters.AddWithValue("@is_two_step_approve", leave.is_two_step_approve);
                command.Parameters.AddWithValue("@over_consecutive_days_for_two_step", leave.over_consecutive_days_for_two_step);
                command.Parameters.AddWithValue("@is_unpaid", leave.is_unpaid);
                command.Parameters.AddWithValue("@is_active", leave.is_active);
                command.Parameters.AddWithValue("@created_at", leave.created_at);
                command.Parameters.AddWithValue("@created_by", leave.created_by);
                command.Parameters.AddWithValue("@updated_at", leave.updated_at);
                command.Parameters.AddWithValue("@updated_by", leave.updated_by);
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
