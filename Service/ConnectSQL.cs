using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebENG.Service
{
    public class ConnectSQL
    {
        public SqlConnection con;
        public SqlConnection OpenConnect()
        {
            con = new SqlConnection("Data Source = 192.168.15.12, 1433; Initial Catalog = MES; User Id = sa; Password = p@ssw0rd; Timeout = 120");
            //con = new SqlConnection(@"Data Source = 5550-18\MEESERVER; Initial Catalog = MES; User Id = sa; Password = P@ssw0rd; Timeout = 120;TrustServerCertificate=True;");
            //con = new SqlConnection(@"Data Source=DESKTOP-BMFLGER\SA;Initial Catalog=MES_TEST;Integrated Security=True");
          
            return con;
        }
        public SqlConnection OpenCTLConnect()
        {
            con = new SqlConnection("Data Source = 192.168.15.12, 1433; Initial Catalog = CTL; User Id = sa; Password = p@ssw0rd;TrustServerCertificate=True; Timeout = 120");
            //con = new SqlConnection(@"Data Source = 5550-18\MEESERVER; Initial Catalog = CTL; User Id = sa; Password = P@ssw0rd;TrustServerCertificate=True; Timeout = 120");
            return con;
        }
        public SqlConnection OpenLeaveConnect()
        {
            con = new SqlConnection("Data Source = 192.168.15.12, 1433; Initial Catalog = ELEAVE; User Id = sa; Password = p@ssw0rd;TrustServerCertificate=True; Timeout = 120");
            //con = new SqlConnection(@"Data Source = 5550-18\MEESERVER; Initial Catalog = ELEAVE; User Id = sa; Password = P@ssw0rd;TrustServerCertificate=True; Timeout = 120");
            return con;
        }
    }
}
