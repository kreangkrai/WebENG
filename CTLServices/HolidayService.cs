using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebENG.CTLInterfaces;
using WebENG.CTLModels;
using WebENG.Service;

namespace WebENG.CTLServices
{
    public class HolidayService : IHoliday
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public HolidayService()
        {
            connect = new ConnectSQL();
            con = connect.OpenCTLConnect();
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
                string strCmd = string.Format($@"SELECT date,detail FROM Holiday Where date LIKE '{year}%'");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        HolidayModel holiday = new HolidayModel()
                        {
                            date = dr["date"] != DBNull.Value ? Convert.ToDateTime(dr["date"].ToString()) : DateTime.MinValue,
                            detail = dr["detail"].ToString()
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
            return holidays;
        }
    }
}
