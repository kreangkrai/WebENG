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
    public class AccessoryService : IAccessory
    {
        public List<UserModel> getAllUser()
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                SqlCommand cmd = new SqlCommand(@"select DISTINCT t1.Login,
                                                t1.Department2,
                                                t1.[Group],
                                                LOWER(t1.Name) as Name,
                                                case when t2.Role ='Admin' then 'Admin' else t2.Role end as Role from [gps_sale_tracking].[dbo].[Sale_User] as t1
                                                 left join[MES].[dbo].[User] as t2 ON LOWER(t1.Name) = LOWER(t2.Fullname) order by t1.Login", ConnectSQL.Open_db_gps_Connect());
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UserModel u = new UserModel()
                        {
                            name = dr["Login"].ToString(),
                            fullname = dr["Name"].ToString().ToLower(),
                            department = dr["Department2"].ToString(),
                            groups = dr["Group"].ToString(),
                            role = dr["Role"].ToString()
                        };
                        users.Add(u);
                    }
                    dr.Close();
                }
            }
            finally
            {
                if (ConnectSQL.con_db_gps.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.Close_db_gps_Connect();
                }
            }
            return users;
        }
    }
}
