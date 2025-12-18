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
    public class DeviceService : IDevice
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public DeviceService()
        {
            connect = new ConnectSQL();
            con = connect.OpenHRConnect();
        }
        public List<DeviceModel> GetDevices()
        {
            List<DeviceModel> devices = new List<DeviceModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT [sn]
                                                  ,[device]
                                                  ,[name]
                                                  ,[sdatetime]
                                                  ,[comment]
                                                  ,[tiggertime]
                                                  ,[levels]
                                                  ,[active]
                                              FROM [hc].[dbo].[devices]");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DeviceModel device = new DeviceModel()
                        {
                            sn = dr["sn"].ToString(),
                            sdatetime = dr["sdatetime"] != DBNull.Value ? Convert.ToDateTime(dr["sdatetime"].ToString()) : DateTime.MinValue,
                            name = dr["name"].ToString(),
                            active = dr["active"] != DBNull.Value ? Convert.ToBoolean(dr["active"].ToString()) :false,
                            device = dr["device"].ToString(),
                            levels = dr["levels"].ToString()
                        };
                        devices.Add(device);
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
            return devices;
        }
    }
}
