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
    public class DeviceGroupService : IDeviceGroup
    {
        ConnectSQL connect = null;
        SqlConnection con = null;

        public DeviceGroupService()
        {
            connect = new ConnectSQL();
            con = connect.OpenHRConnect();
        }

        public DeviceGroupModel GetDataByDevice(string device,string groupname)
        {
            DeviceGroupModel device_group = new DeviceGroupModel();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT * FROM device_group WHERE device = @device AND groupname = @groupname");
                SqlCommand command = new SqlCommand(strCmd, con);
                command.Parameters.AddWithValue("@device", device);
                command.Parameters.AddWithValue("@groupname", groupname);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        device_group = new DeviceGroupModel()
                        {
                            device = dr["device"].ToString(),
                            groupname = dr["groupname"].ToString(),
                            starttime = TimeSpan.Parse(dr["starttime"].ToString())
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
            return device_group;
        }

        public List<DeviceGroupModel> GetDevicesGroup()
        {
            List<DeviceGroupModel> device_groups = new List<DeviceGroupModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT * FROM device_group");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DeviceGroupModel device_group = new DeviceGroupModel()
                        {
                            device = dr["device"].ToString(),
                            groupname = dr["groupname"].ToString(),
                            starttime = TimeSpan.Parse(dr["starttime"].ToString())
                        };
                        device_groups.Add(device_group);
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
            return device_groups;
        }
    }
}
