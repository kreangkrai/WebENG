using HRManagement.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebENG.HRInterface;
using WebENG.HRModel;
using WebENG.Service;

namespace WebENG.HRService
{
    public class HrService : IHr
    {
        private IDevice Device;
        private IDeviceGroup DeviceGroup;
        ConnectSQL connect = null;
        SqlConnection con = null;
        public HrService()
        {
            connect = new ConnectSQL();
            con = connect.OpenHRConnect();
            DeviceGroup = new DeviceGroupService();
            Device = new DeviceService();
        }

        public List<DataModel> GetDataByDate(DateTime start, DateTime stop)
        {
            List<DataModel> datas = new List<DataModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"WITH data_with_first AS (
                                                    SELECT 
                                                        CAST(datetime AS DATE) AS [date],
                                                        cn,
                                                        sn,
                                                        persongroup,
                                                        datetime,
                                                        FIRST_VALUE(sn) OVER (
                                                            PARTITION BY CAST(datetime AS DATE), cn 
                                                            ORDER BY datetime 
                                                            ROWS UNBOUNDED PRECEDING
                                                        ) AS first_sn,
                                                        FIRST_VALUE(persongroup) OVER (
                                                            PARTITION BY CAST(datetime AS DATE), cn 
                                                            ORDER BY datetime 
                                                            ROWS UNBOUNDED PRECEDING
                                                        ) AS first_persongroup
                                                    FROM hr
                                                    WHERE datetime >= '{start.ToString("yyyy-MM-dd")}' 
                                                      AND datetime <= '{stop.ToString("yyyy-MM-dd")}'
                                                )
                                                SELECT
                                                    [date],
                                                    cn,
                                                    first_sn AS sn,                   
                                                    first_persongroup AS persongroup, 
                                                    MIN(datetime) AS time_in,          
                                                    MAX(datetime) AS time_out          
                                                FROM data_with_first
                                                GROUP BY
                                                    [date],
                                                    cn,
                                                    first_sn,
                                                    first_persongroup
                                                ORDER BY [date], cn;");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DataModel data = new DataModel()
                        {
                            date = dr["date"] != DBNull.Value ? Convert.ToDateTime(dr["date"].ToString()) : DateTime.MinValue,
                            emp = dr["cn"].ToString(),
                            persongroup = dr["persongroup"].ToString(),
                            cn = dr["cn"].ToString(),
                            sn = dr["sn"].ToString(),
                            time_in = ((DateTime)dr["time_in"]).TimeOfDay,
                            time_out = ((DateTime)dr["time_out"]).TimeOfDay
                        };
                        datas.Add(data);
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
            return datas;


        }
    }
}
