using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebENG.Service;
using WebENG.TripInterface;
using WebENG.TripModels;

namespace HRManagement.Service
{
    public class TripExpenseService : ITripExpense
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public TripExpenseService()
        {
            connect = new ConnectSQL();
            con = connect.OpenTRIPConnect();
        }
        public List<TripExpenseModel> GetData(DateTime start, DateTime stop)
        {
            List<TripExpenseModel> datas = new List<TripExpenseModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand($@"SELECT DISTINCT main.Passenger as passenger,
                                                    main.emp_id,
                                                    main.trip,
                                                    main.Date as date,
                                                    main.Status,
                                                    main.Location as location
                                                     FROM (
	                                                    SELECT Passenger,
	                                                     Sale_User.emp_id,
                                                         trip,
	                                                     Date,
                                                         Status,
	                                                     Location,
	                                                     RANK() OVER(PARTITION BY SUBSTRING(date,1,11) ,Passenger ORDER BY Date ASC) as rank_start,
	                                                     RANK() OVER(PARTITION BY SUBSTRING(date,1,11) ,Passenger ORDER BY Date DESC) as rank_stop 
	                                                    from CTL_Master
	                                                    LEFT JOIN Sale_User ON Sale_User.Login = CTL_Master.Passenger
	                                                    where SUBSTRING(Date,1,11) BETWEEN '{start.ToString("yyyy-MM-dd")}' AND '{stop.ToString("yyyy-MM-dd")}' AND Status IN('START','STOP','GetIn','GetOut','ChkIn')
	                                                     ) as main

                                                     WHERE main.rank_start = 1 OR main.rank_stop = 1
                                                     ORDER BY main.Date,main.Passenger",con);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TripExpenseModel data = new TripExpenseModel()
                        {
                            passenger = dr["passenger"].ToString(),
                            emp_id = dr["emp_id"].ToString(),
                            date = Convert.ToDateTime(dr["date"].ToString()),
                            trip = dr["trip"].ToString() != "" ? Convert.ToDateTime(dr["trip"].ToString()) : Convert.ToDateTime(dr["date"].ToString()),
                            status = dr["Status"].ToString(),
                            location = ReplaceSymbol(dr["location"].ToString())
                        };
                        datas.Add(data);
                    }
                    dr.Close();
                }
                con.Close();

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return datas;
        }
        public string ReplaceSymbol(string str)
        {
            if (str.Trim() == "")
            {
                return "_";
            }
            else
            {
                Regex reg = new Regex("[^A-Za-z0-9]");
                str = reg.Replace(str, "_");
                string new_str = "";
                bool chk_ = false;
                string ch = "";
                for (int i = 0; i < str.Length; i++)
                {
                    bool b = str[i] == '_' ? true : false;

                    if (b)
                    {
                        if (!chk_)
                        {
                            ch += "_";
                            chk_ = true;
                        }
                    }
                    else
                    {
                        ch += str[i];
                        chk_ = false;
                    }

                    new_str += ch;
                    ch = "";

                }
                return new_str;
            }
        }
    }
}
