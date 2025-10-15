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
    public class HolidayService : IHoliday
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public HolidayService()
        {
            connect = new ConnectSQL();
            con = connect.OpenConnect();
        }
        public string CreateHoliday(HolidayModel model)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"
                INSERT INTO Holidays (
                    date,
                    name )
                VALUES (
                    @date,
                    @name
                )");
                SqlCommand command = new SqlCommand(string_command, con);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@date", model.date);
                command.Parameters.AddWithValue("@name", model.name);
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

        public List<HolidayModel> GetAllHolidays()
        {
            List<HolidayModel> holidays = new List<HolidayModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT * FROM Holidays");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        HolidayModel holiday = new HolidayModel()
                        {
                            no = dr["no"] != DBNull.Value ? Convert.ToInt32(dr["no"]) : default(Int32),
                            date = dr["date"] != DBNull.Value ? Convert.ToDateTime(dr["date"]) : default(DateTime),
                            name = dr["name"] != DBNull.Value ? dr["name"].ToString() : ""
                        };
                        holidays.Add(holiday);
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
            return holidays.OrderBy(o => o.date).ToList();
        }

        public List<HolidayModel> GetHolidays(string year)
        {
            List<HolidayModel> holidays = new List<HolidayModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string string_command = string.Format($@"SELECT * FROM Holidays WHERE date LIKE '{year}%'");
                SqlCommand cmd = new SqlCommand(string_command, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        HolidayModel holiday = new HolidayModel()
                        {
                            no = dr["no"] != DBNull.Value ? Convert.ToInt32(dr["no"]) : default(Int32),
                            date = dr["date"] != DBNull.Value ? Convert.ToDateTime(dr["date"]) : default(DateTime),
                            name = dr["name"] != DBNull.Value ? dr["name"].ToString() : ""
                        };
                        holidays.Add(holiday);
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
            return holidays.OrderBy(o => o.date).ToList();
        }
    }
}
