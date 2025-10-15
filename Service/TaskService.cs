using WebENG.Interface;
using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Service;
using System.Data;

namespace WebENG.Service
{
    public class TaskService : ITask
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public TaskService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<TaskModel> GetAllTasks()
        {
            List<TaskModel> tasks = new List<TaskModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($"SELECT * FROM Tasks");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TaskModel task = new TaskModel()
                        {
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                        };
                        tasks.Add(task);
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
            return tasks;
        }

        public List<TaskModel> GetOfficeTasks()
        {
            List<TaskModel> tasks = new List<TaskModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($"SELECT * FROM Tasks WHERE Tasks.task_id LIKE 'O%'");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TaskModel task = new TaskModel()
                        {
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                        };
                        tasks.Add(task);
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
            return tasks;
        }

        public List<TaskModel> GetSiteTasks()
        {
            List<TaskModel> tasks = new List<TaskModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($"SELECT * FROM Tasks WHERE Tasks.task_id LIKE 'S%'");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TaskModel task = new TaskModel()
                        {
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                        };
                        tasks.Add(task);
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
            return tasks;
        }

        public List<TaskModel> GetOtherTasks()
        {
            List<TaskModel> tasks = new List<TaskModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($"SELECT * FROM Tasks WHERE Tasks.task_id LIKE 'T%'");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TaskModel task = new TaskModel()
                        {
                            task_id = dr["task_id"] != DBNull.Value ? dr["task_id"].ToString() : "",
                            task_name = dr["task_name"] != DBNull.Value ? dr["task_name"].ToString() : "",
                        };
                        tasks.Add(task);
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
            return tasks;
        }

        public string CreateTask(TaskModel task)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"INSERT INTO Tasks(task_id, task_name) VALUES(@task_id, @task_name)");
                using (SqlCommand cmd = new SqlCommand(string_command, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@task_id", task.task_id);
                    cmd.Parameters.AddWithValue("@task_name", task.task_name);
                    cmd.ExecuteNonQuery();
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

        public string UpdateTask(TaskModel task)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                UPDATE TASKS 
                SET
                    task_name = @task_name
                WHERE task_id = @task_id");
                using (SqlCommand cmd = new SqlCommand(string_command,con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@task_id", task.task_id);
                    cmd.Parameters.AddWithValue("@task_name", task.task_name);
                    cmd.ExecuteNonQuery();
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
