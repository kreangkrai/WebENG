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
    public class NotificationService : INotification
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public NotificationService()
        {
            connect = new ConnectSQL();
            con = connect.OpenLeaveConnect();
        }
        public List<NotificationModel> GetNotificationsByEMPID(string emp_id)
        {
            List<NotificationModel> notifications = new List<NotificationModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand(@"SELECT [notification_id]
                                                          ,[notification_type]
                                                          ,[emp_id]
                                                          ,[notification_issue]
                                                          ,[notification_date]
                                                          ,[notification_description]
                                                          ,[status]
                                                      FROM [ELEAVE].[dbo].[Notification] WHERE [emp_id] = @emp_id", con);
                cmd.Parameters.AddWithValue("@emp_id", emp_id);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        NotificationModel notification = new NotificationModel()
                        {
                            notification_id = Convert.ToInt32(dr["notification_id"].ToString()),
                            notification_type = dr["notification_type"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            notification_issue = dr["notification_issue"].ToString(),
                            notification_description = dr["notification_description"].ToString(),
                            notification_date = Convert.ToDateTime(dr["notification_date"].ToString()),
                            status = dr["status"].ToString()
                        };
                        notifications.Add(notification);
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
            return notifications;
        }

        public string Insert(NotificationModel notification)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                                            INSERT INTO [ELEAVE].[dbo].[Notification]
                                                   ([notification_type]
                                                   ,[emp_id]
                                                   ,[notification_issue]
                                                   ,[notification_date]
                                                   ,[notification_description]
                                                   ,[status])
                                             VALUES
                                                   (@notification_type
                                                   ,@emp_id
                                                   ,@notification_issue
                                                   ,@notification_date
                                                   ,@notification_description
                                                   ,@status)");
                SqlCommand command = new SqlCommand(string_command, con);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@notification_type", notification.notification_type);
                command.Parameters.AddWithValue("@emp_id", notification.emp_id);
                command.Parameters.AddWithValue("@notification_issue", notification.notification_issue);
                command.Parameters.AddWithValue("@notification_date", notification.notification_date);
                command.Parameters.AddWithValue("@notification_description", notification.notification_description);
                command.Parameters.AddWithValue("@status", notification.status);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
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

        public string Update(NotificationModel notification)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"UPDATE [ELEAVE].[dbo].[notification]
                                                           SET [notification_type] = @notification_type
                                                              ,[notification_issue] = @notification_issue
                                                              ,[notification_date] = @notification_date
                                                              ,[notification_description] = @notification_description
                                                              ,[status] = @status
                                                         WHERE [notification_id] = @notification_id AND [emp_id] = @emp_id");
                SqlCommand command = new SqlCommand(string_command, con);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@notification_type", notification.notification_type);
                command.Parameters.AddWithValue("@emp_id", notification.emp_id);
                command.Parameters.AddWithValue("@notification_issue", notification.notification_issue);
                command.Parameters.AddWithValue("@notification_date", notification.notification_date);
                command.Parameters.AddWithValue("@notification_description", notification.notification_description);
                command.Parameters.AddWithValue("@status", notification.status);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
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

        public string UpdateStatus(string emp_id,string status)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"UPDATE [ELEAVE].[dbo].[notification]
                                                           SET [status] = @status
                                                         WHERE [emp_id] = @emp_id");
                SqlCommand command = new SqlCommand(string_command, con);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@emp_id", emp_id);
                command.Parameters.AddWithValue("@status", status);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
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
