using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Reflection.PortableExecutable;
using WebENG.CTLInterfaces;
using WebENG.CTLModels;
using WebENG.Service;

namespace WebENG.CTLServices
{
    public class EmployeeService : IEmployee
    {
        ConnectSQL connect = null;
        SqlConnection con = null;
        public EmployeeService()
        {
            connect = new ConnectSQL();
            con = connect.OpenCTLConnect();
        }
        public List<EmployeeModel> GetEmployees()
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string strCmd = string.Format($@"SELECT emp_id,
                                                        name_en,
                                                        name_th,
                                                        department,
                                                        location,
                                                        start_date,
                                                        promote_manager_date,
                                                        position,
                                                        gender,
                                                        phone,
                                                        telephone,
                                                        email,
                                                        [group],
                                                        active,
                                                        team,
                                                        group_color
                                                  FROM Employees");
                SqlCommand command = new SqlCommand(strCmd, con);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EmployeeModel employee = new EmployeeModel()
                        {
                            emp_id = dr["emp_id"].ToString(),
                            name_en = dr["name_en"].ToString(),
                            name_th = dr["name_th"].ToString(),
                            department = dr["department"].ToString(),
                            location = dr["location"].ToString(),
                            start_date = dr["start_date"] != DBNull.Value ? Convert.ToDateTime(dr["start_date"].ToString()) : DateTime.MinValue,
                            promote_manager_date = dr["promote_manager_date"] != DBNull.Value ? Convert.ToDateTime(dr["promote_manager_date"].ToString()) : DateTime.MinValue,
                            position = dr["position"].ToString(),
                            gender = dr["gender"].ToString(),
                            phone = dr["phone"].ToString(),
                            telephone = dr["telephone"].ToString(),
                            email = dr["email"].ToString(),
                            group = dr["group"].ToString(),
                            active = dr["active"] != DBNull.Value ? Convert.ToBoolean(dr["active"].ToString()) : false,
                            team = dr["team"].ToString(),
                            group_color = dr["group_color"].ToString()
                        };
                        employees.Add(employee);
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
            return employees;
        }

        public List<EmpModel> GetEmps()
        {
            List<EmployeeModel> employees = GetEmployees();
            List<EmpModel> emps = new List<EmpModel>();
            string ldapPath = "LDAP://192.168.15.1";
            string username = "lor";     
            string password = "P@ssw0rd";
            try
            {              
                using (System.DirectoryServices.DirectoryEntry directoryEntry = new System.DirectoryServices.DirectoryEntry(ldapPath,username,password))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(directoryEntry))
                    {
                        searcher.Filter = "(&(objectClass=user)(thumbnailPhoto=*))";
                        searcher.PropertiesToLoad.Add("displayname");
                        searcher.PropertiesToLoad.Add("thumbnailPhoto");

                        SearchResultCollection results = searcher.FindAll();

                        foreach (SearchResult adsSearchResult in results)
                        {
                            if (adsSearchResult.Properties["displayname"].Count > 0)
                            {
                                string name = adsSearchResult.Properties["displayname"][0]?.ToString();
                                string photo = null;

                                if (adsSearchResult.Properties["thumbnailPhoto"].Count > 0)
                                {
                                    byte[] b = adsSearchResult.Properties["thumbnailPhoto"][0] as byte[];
                                    photo = Convert.ToBase64String(b);
                                }

                                EmpModel emp = new EmpModel()
                                {
                                    emp_id = employees.Where(w=>w.name_en.ToLower() == name.ToLower()).Select(s=>s.emp_id).FirstOrDefault(),
                                    name = name,
                                    img = photo
                                };

                                emps.Add(emp);
                            }
                        }
                    }
                }
            }
            catch
            {
                return emps;
            }
            return emps;
        }
    }
}
