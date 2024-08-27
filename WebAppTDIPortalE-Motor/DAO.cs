using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebAppTDIPortalE_Motor
{
    public class DAO<T>
    {
        public DAO()
        {

        }

        public DataTable RetrieveDataTablebySP(string spName, string[] spParam, object[] objParamValue)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection01"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(spName, sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    for (int i = 0; i < spParam.Length; i++)
                    {
                        cmd.Parameters.Add(new SqlParameter(spParam[i], objParamValue[i]));
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public List<T> RetrieveDataBySP(string spName, string[] spParam, object[] objParamValue)
        {
            List<T> lst = new List<T>();
            try
            {
                string Connection = ConfigurationManager.ConnectionStrings["DefaultConnection01"].ToString();
                using (SqlConnection con = new SqlConnection(Connection))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3000;
                    for (int i = 0; i < spParam.Length; i++)
                    {
                        cmd.Parameters.Add(new SqlParameter(spParam[i], objParamValue[i]));
                    }
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            T objRet = (T)PopulateData(reader, typeof(T));
                            lst.Add(objRet);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
                //lst = null;
            }
            return lst;
        }

        public object PopulateData(IDataRecord dr, Type t)
        {
            object o = Activator.CreateInstance(t);
            bool isPopulate = false;
            Type objType = o.GetType();


            foreach (PropertyInfo p in t.GetProperties())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string getField = dr.GetName(i).ToUpper();
                    string getProp = p.Name.ToUpper();
                    if (getField == getProp)
                    {

                        if (!dr.IsDBNull(i))
                        {
                            try
                            {
                                p.SetValue(o, dr[p.Name], null);
                                isPopulate = true;
                            }
                            catch (Exception e)
                            {
                                isPopulate = false;
                                throw e;
                            }
                           
                        }
                        else
                        {
                            //p.SetValue(o, null, null);
                            //isPopulate = true;
                        }
                    }

                }
            }
            if (!isPopulate)
            {
                o = null;
            }
            return o;

        }

        public int UpdateDataBySP(string spName, string[] spParam, object[] objParamValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection01"].ToString()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(spName, con);
                    for (int i = 0; i < spParam.Length; i++)
                    {
                        cmd.Parameters.Add(new SqlParameter(spParam[i], objParamValue[i]));
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }

        }

        public int UpdateDataBySP(string strsql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection01"].ToString()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(strsql, con);
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

        public List<T> RetrieveDataBySQL(string queryString)
        {

            List<T> listData = new List<T>();
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection01"].ToString()))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandText = queryString;

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            T objRet = (T)PopulateData(rdr, typeof(T));

                            listData.Add(objRet);
                        }
                    }

                }
            }

            return listData;
        }

        public DataSet RetrieveDataTablebySP(string queryString)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection01"].ConnectionString))
            {
                //using (SqlCommand cmd = conn.CreateCommand())
                //{
                //    if (conn.State == ConnectionState.Closed)
                //        conn.Open();

                //    cmd.CommandText = queryString;
                //    cmd.ExecuteReader();
                //    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                //    {
                //        da.Fill(ds);
                //    }
                //}
                using (SqlCommand cmd = new SqlCommand(queryString, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }

            return ds;
        }

    }
}