using WebENG.Interface;
using WebENG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebENG.TripModels;
using WebENG.TripInterface;
using System.Data;

namespace WebENG.Service
{
    public class NewTripExpenseService : INEWTripExpense
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public NewTripExpenseService()
        {
            connect = new ConnectSQL();
            con = connect.OpenNEWTRIPConnect();
        }
        public List<TripExpenseModel> GetData(DateTime start, DateTime stop)
        {
            stop = stop.AddDays(1);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            List<TripExpenseModel> datas = new List<TripExpenseModel>();
            SqlCommand cmd = new SqlCommand($@"select emp.name_en,* from (
                                                SELECT driver as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM Personal WHERE  status IN('START','STOP','CHECK IN','CONTINUE')

												union ALL
												SELECT driver as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM Company WHERE  status IN('START','STOP','CHECK IN','CONTINUE')

												UNION ALL

												SELECT passenger as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM [Public] WHERE  status IN('START','STOP','CHECK IN','CONTINUE')

												UNION ALL

												SELECT passenger as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM Passenger_Personal WHERE  status IN('START','STOP','CHECK IN','CONTINUE')

												UNION ALL

												SELECT passenger as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM Passenger_Company WHERE  status IN('START','STOP','CHECK IN','CONTINUE')
												) main
                                                LEFT JOIN [CTL].dbo.Employees emp ON main.emp_id = emp.emp_id
												where main.trip_date >= '{start.ToString("yyyy-MM-dd")}' AND main.trip_date <= '{stop.ToString("yyyy-MM-dd")}'");
            cmd.Connection = con;
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    TripExpenseModel data = new TripExpenseModel()
                    {
                        passenger = dr["name_en"].ToString(),
                        emp_id = dr["emp_id"].ToString(),
                        date = Convert.ToDateTime(dr["date"].ToString()),
                        trip = dr["trip_date"].ToString() != "" ? Convert.ToDateTime(dr["trip_date"].ToString()) : Convert.ToDateTime(dr["date"].ToString()),
                        status = dr["status"].ToString(),
                        location = ReplaceSymbol($"{dr["location"].ToString()}({dr["zipcode"].ToString()})")
                    };
                    datas.Add(data);
                }
                dr.Close();
            }
            con.Close();

            return datas;
        }

        public List<TripExpenseModel> GetDataByEmployee(string emp_id, DateTime start, DateTime stop)
        {
            stop = stop.AddDays(1);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            List<TripExpenseModel> datas = new List<TripExpenseModel>();
            SqlCommand cmd = new SqlCommand($@"select emp.name_en,* from (
                                                SELECT driver as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM Personal WHERE  status IN('START','STOP','CHECK IN','CONTINUE')

												union ALL
												SELECT driver as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM Company WHERE  status IN('START','STOP','CHECK IN','CONTINUE')

												UNION ALL

												SELECT passenger as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM [Public] WHERE  status IN('START','STOP','CHECK IN','CONTINUE')

												UNION ALL

												SELECT passenger as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM Passenger_Personal WHERE  status IN('START','STOP','CHECK IN','CONTINUE')

												UNION ALL

												SELECT passenger as emp_id,trip, date,status,location,zipcode, CASE
                                                    WHEN LEN(trip) = 14 AND ISNUMERIC(trip) = 1
                                                    THEN TRY_CONVERT(datetime,
                                                             SUBSTRING(trip, 1,4) + '-' +
                                                             SUBSTRING(trip, 5,2) + '-' +
                                                             SUBSTRING(trip, 7,2) + ' ' +
                                                             SUBSTRING(trip, 9,2) + ':' +
                                                             SUBSTRING(trip,11,2) + ':' +
                                                             SUBSTRING(trip,13,2)
                                                         )
                                                    ELSE NULL
                                                END AS trip_date FROM Passenger_Company WHERE  status IN('START','STOP','CHECK IN','CONTINUE')
												) main
                                                LEFT JOIN [CTL].dbo.Employees emp ON main.emp_id = emp.emp_id
												where main.emp_id = '{emp_id}' AND main.trip_date >= '{start.ToString("yyyy-MM-dd")}' AND main.trip_date <= '{stop.ToString("yyyy-MM-dd")}'");

            cmd.Connection = con;
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    TripExpenseModel data = new TripExpenseModel()
                    {
                        passenger = dr["name_en"].ToString(),
                        emp_id = dr["emp_id"].ToString(),
                        date = Convert.ToDateTime(dr["date"].ToString()),
                        trip = dr["trip_date"].ToString() != "" ? Convert.ToDateTime(dr["trip_date"].ToString()) : Convert.ToDateTime(dr["date"].ToString()),
                        status = dr["status"].ToString(),
                        location = ReplaceSymbol($"{dr["location"].ToString()}({dr["zipcode"].ToString()})")
                    };
                    datas.Add(data);
                }
                dr.Close();
            }
            con.Close();

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
