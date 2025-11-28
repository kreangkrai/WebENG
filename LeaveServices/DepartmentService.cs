using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WebENG.LeaveInterfaces;
using WebENG.LeaveModels;
using WebENG.Service;

public class DepartmentService : IDepartment
{
    ConnectSQL connect = null;
    SqlConnection con = null;

    public DepartmentService()
    {
        connect = new ConnectSQL();
        con = connect.OpenLeaveConnect();
    }

    public string Delete(string department)
    {
        return Delete(department, null);
    }

    public string Delete(string department, SqlTransaction tran)
    {
        SqlConnection localCon = tran?.Connection ?? con;
        bool shouldClose = false;

        try
        {
            if (localCon.State == ConnectionState.Closed)
            {
                localCon.Open();
                shouldClose = true;
            }

            string sql = "DELETE FROM [dbo].[departments] WHERE [department] = @department";
            using (SqlCommand cmd = new SqlCommand(sql, localCon, tran))
            {
                cmd.Parameters.AddWithValue("@department", department);
                cmd.ExecuteNonQuery();
            }
            return "Success";
        }
        catch (Exception ex)
        {
            throw new Exception("Delete Department failed: " + ex.Message, ex);
        }
        finally
        {
            if (shouldClose && localCon.State == ConnectionState.Open)
                localCon.Close();
        }
    }
    public List<DepartmentModel> GetDepartments()
    {
        List<DepartmentModel> departments = new List<DepartmentModel>();
        try
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            string strCmd = string.Format($@"SELECT [id]
                                                  ,[departments].[department]
                                                  ,[department_name]
                                                  ,[level]
                                                  ,[departments].[emp_id]
	                                              ,emp.name_en as emp_name
                                                  ,[is_active]
                                              FROM [dbo].[departments]
                                              LEFT JOIN [CTL].dbo.[Employees] emp ON [departments].emp_id = emp.emp_id");
            SqlCommand command = new SqlCommand(strCmd, con);
            SqlDataReader dr = command.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    DepartmentModel department = new DepartmentModel()
                    {
                        id = Int32.Parse(dr["id"].ToString()),
                        department = dr["department"].ToString(),
                        department_name = dr["department_name"].ToString(),
                        level = dr["level"] != DBNull.Value ? Convert.ToInt32(dr["level"].ToString()) : 0,
                        emp_id = dr["emp_id"].ToString(),
                        emp_name = dr["emp_name"].ToString(),
                        is_active = dr["is_active"] != DBNull.Value ? Convert.ToBoolean(dr["is_active"].ToString()) : false
                    };
                    departments.Add(department);
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
        return departments;
    }

    public string Inserts(List<DepartmentModel> departments)
    {
        return Inserts(departments, null);
    }

    public string Inserts(List<DepartmentModel> departments, SqlTransaction tran)
    {
        if (departments == null || !departments.Any()) return "Success";

        SqlConnection localCon = tran?.Connection ?? con;
        bool shouldClose = false;

        try
        {
            if (localCon.State == ConnectionState.Closed)
            {
                localCon.Open();
                shouldClose = true;
            }

            string sql = @"INSERT INTO [dbo].[departments]
                           ([department],[department_name],[level],[emp_id],[is_active])
                           VALUES
                           (@department,@department_name,@level,@emp_id,@is_active)";

            using (SqlCommand cmd = new SqlCommand(sql, localCon, tran))
            {
                cmd.Parameters.Add("@department", SqlDbType.NVarChar);
                cmd.Parameters.Add("@department_name", SqlDbType.NVarChar);
                cmd.Parameters.Add("@level", SqlDbType.Int);
                cmd.Parameters.Add("@emp_id", SqlDbType.NVarChar);
                cmd.Parameters.Add("@is_active", SqlDbType.Bit);

                foreach (var d in departments)
                {
                    cmd.Parameters[0].Value = d.department ?? (object)DBNull.Value;
                    cmd.Parameters[1].Value = d.department_name ?? (object)DBNull.Value;
                    cmd.Parameters[2].Value = d.level;
                    cmd.Parameters[3].Value = d.emp_id ?? (object)DBNull.Value;
                    cmd.Parameters[4].Value = d.is_active;

                    cmd.ExecuteNonQuery();
                }
            }
            return "Success";
        }
        catch (Exception ex)
        {
            throw new Exception("Insert Department failed: " + ex.Message, ex);
        }
        finally
        {
            if (shouldClose && localCon.State == ConnectionState.Open)
                localCon.Close();
        }
    }
}
