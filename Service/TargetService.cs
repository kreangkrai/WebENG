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
    public class TargetService : ITarget
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public TargetService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public List<TargetModel> getData(int year, string type)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                List<TargetModel> targets = new List<TargetModel>();
                SqlCommand cmd = null;
                SqlDataReader dr = null;
                if (type == "Project")
                {
                    cmd = new SqlCommand($"select month,target from Target_Project WHERE month LIKE '{year}%'", con);
                }
                if (type == "Service")
                {
                    cmd = new SqlCommand($"select month,target from Target_Service WHERE month LIKE '{year}%'", con);
                }
                if (type == "Invoice")
                {
                    cmd = new SqlCommand($"select month,target from Target_Invoice_ENG WHERE month LIKE '{year}%'", con);
                }
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TargetModel p = new TargetModel()
                        {
                            month = dr["month"].ToString(),
                            target = dr["target"] != DBNull.Value ? Convert.ToDouble(dr["target"].ToString()) : 0,
                        };
                        targets.Add(p);
                    }
                    dr.Close();
                }
                return targets;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public string Insert(List<TargetModel> targets , string type)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                for (int i = 0; i < targets.Count; i++)
                {
                    string command = "";
                    if (type == "Project")
                    {
                        command = @"INSERT INTO Target_Project(month,target) VALUES (@month,@target)";
                    }
                    if (type == "Service")
                    {
                        command = @"INSERT INTO Target_Service(month,target) VALUES (@month,@target)";
                    }
                    if (type == "Invoice")
                    {
                        command = @"INSERT INTO Target_Invoice_ENG(month,target) VALUES (@month,@target)";
                    }
                    
                    using (SqlCommand cmd = new SqlCommand(command,con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@month", targets[i].month);
                        cmd.Parameters.AddWithValue("@target", targets[i].target);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
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
        public string Update(List<TargetModel> targets,string type)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                for (int i = 0; i < targets.Count; i++)
                {
                    string command = "";
                    if (type == "Project")
                    {
                        command = $@"UPDATE Target_Project SET target = '{targets[i].target}'
                                     WHERE month='{targets[i].month}'";
                    }
                    if (type == "Service")
                    {
                        command = $@"UPDATE Target_Service SET target = '{targets[i].target}'
                                     WHERE month='{targets[i].month}'";
                    }
                    if(type == "Invoice")
                    {
                        command = $@"UPDATE Target_Invoice_ENG SET target = '{targets[i].target}'
                                     WHERE month='{targets[i].month}'";
                    }
                    SqlDataReader reader;
                    SqlCommand cmd = new SqlCommand(command);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
            }
            catch
            {
                return "Failed";
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
