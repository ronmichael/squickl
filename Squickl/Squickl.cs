using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web;
using System.Xml;
using MySql.Data;
using MySql.Data.MySqlClient;   



public class Squickl : System.IDisposable
{


    #region Private properties

    private IDbConnection cn;
    private IDataReader dr;
    private IDbCommand cmd;
    private static Exception lastError;     
    private int rowsread = 0;

    public enum ProviderTypes { MSSQL = 1, MySQL = 2, MSSQLCompact };
  

    public static string SqlConnectionString(string name = "")
    {
        
        if (name.Length == 0)
            name = ConfigurationManager.AppSettings["Squickl_DefaultConnection"];

        if (ConfigurationManager.ConnectionStrings[name] != null)
            return ConfigurationManager.ConnectionStrings[name].ConnectionString.ToString().Replace("%temp%", Path.GetTempPath());
        else
            return "";

    }

    public static string ProviderName(string name = "")
    {
        if (name.Length == 0)
            name = ConfigurationManager.AppSettings["Squickl_DefaultConnection"];
        if (ConfigurationManager.ConnectionStrings[name] != null)
            return ConfigurationManager.ConnectionStrings[name].ProviderName;
        else
            return "";

    }

    public static ProviderTypes ProviderType(string name = "")
    {
        if (ProviderName(name).ToLower().Contains("mysql"))
            return ProviderTypes.MySQL;
        else if (ProviderName(name)=="System.Data.SqlServerCe")
            return ProviderTypes.MSSQLCompact;
        else
            return ProviderTypes.MSSQL;
    }


    #endregion




    #region Public properties

    /// <summary>
    /// Returns the # of rows read so far
    /// </summary>
    public int RowsRead
    {
        get { return rowsread; }
    }

    
    /// <summary>
    /// Returns the last error that was raised by object
    /// </summary>
    public static Exception LastError
    {
        get { return lastError; }
    }


    #endregion





    #region Static tools



    /// <summary>
    /// Executes a SQL statement and reads the resultset into a DataTable
    /// </summary>
    /// <param name="sqlcmd">SQL command string.</param>
    /// <param name="addBlank">Add a blank row to the end?</param>
    /// <returns></returns>
    public static DataTable ReadTable(string sqlcmd, string connectionname = "") 
    {


        DataTable outp = new DataTable();

  
        try
        {
            // IDbConnection conn;


            if (ProviderType(connectionname) == ProviderTypes.MSSQL)
            {

                using (SqlConnection con = new SqlConnection(SqlConnectionString(connectionname)))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd, con))
                    {
                        con.Open();
                        da.Fill(outp);
                    }
                }


            }
            else
            {

                using (MySqlConnection con = new MySqlConnection(SqlConnectionString(connectionname)))
                {

                    using (MySqlDataAdapter da = new MySqlDataAdapter(sqlcmd, con))
                    {
                        con.Open();
                        da.Fill(outp);
                    }


                }
            }

            

        }
        catch (Exception err)
        {
            lastError = err;
        }


        return outp;
    }


    /// <summary>
    /// Executes a single SQL statement
    /// </summary>
    /// <param name="statement"></param>
    /// <returns></returns>
    public static bool Exec(string sql, string connectionname = "")
    {
        try
        {
            if (ProviderType(connectionname) == ProviderTypes.MSSQL)
            {

                using (SqlConnection con = new SqlConnection(SqlConnectionString(connectionname)))
                {
                    using (SqlCommand oc = new SqlCommand(sql, con))
                    {
                        con.Open();
                        oc.ExecuteNonQuery();
                    }

                }


            }
            else
            {

                using (MySqlConnection con = new MySqlConnection(SqlConnectionString(connectionname)))
                {
                    using (MySqlCommand oc = new MySqlCommand(sql, con))
                    {
                        con.Open();
                        oc.ExecuteNonQuery();
                    }


                }
            }

            return true;

        }
        catch (Exception err)
        {
            lastError = err;
            return false;
        }

    }






    /// <summary>
    /// Executes a SQL statement and returns a single value
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public static string Lookup(string sql, string defValue = "", string connectionname = "")
    {

        string ret = defValue;

        try
        {

            if (ProviderType(connectionname) == ProviderTypes.MSSQL)
            {

                using (SqlConnection con = new SqlConnection(SqlConnectionString(connectionname)))
                {
                    using (SqlCommand oc = new SqlCommand(sql, con))
                    {
                        con.Open();
                        ret = oc.ExecuteScalar().ToString();
                    }

                }


            }
            else
            {

                using (MySqlConnection con = new MySqlConnection(SqlConnectionString(connectionname)))
                {
                    using(MySqlCommand oc = new MySqlCommand(sql, con))
                   
                    {
                        con.Open();
                        ret = oc.ExecuteScalar().ToString();
                       
                    }


                }
            }

        }
        catch (Exception err)
        {
            lastError = err;
        }

        return ret;
    }


    #endregion





    #region Contructor and deconstructor and disposal




    public Squickl(string CommandText, string connectionStringName = "")
    {

        if (ProviderType(connectionStringName) == ProviderTypes.MSSQL)
        {
            cn = new SqlConnection(SqlConnectionString(connectionStringName));
            cmd = new SqlCommand();
            cmd.CommandTimeout = 120;
        }
        else if (ProviderType(connectionStringName) == ProviderTypes.MSSQLCompact)
        {
            cn = new SqlCeConnection(SqlConnectionString(connectionStringName));
            cmd = new SqlCeCommand();
        }
        else
        {
            cn = new MySqlConnection(SqlConnectionString(connectionStringName));
            cmd = new MySqlCommand();
        }

        cn.Open();

        cmd.Connection = cn;
        cmd.CommandText = CommandText;
        
        dr = cmd.ExecuteReader();
    }

    ~Squickl()
    {
        Close();
    }


    /// <summary>
    /// Disposes of the Squickl
    /// </summary>
    public void Dispose()
    {
        Close();
    }


    /// <summary>
    /// Closes the Squickl
    /// </summary>
    public void Close()
    {
        try
        {
            cn.Dispose();
            cmd.Dispose();
        }
        catch { }
    }
    #endregion






    /// <summary>
    /// Read the next row in the query.
    /// </summary>
    /// <returns>true if read, false if end of data</returns>
    public bool Read()
    {
        bool output;
        output = dr.Read();
        if (output) rowsread++;
        return output;
    }

    
    /// <summary>
    /// Moves to the next result set
    /// </summary>
    /// <returns>True of there is another result set; false if none</returns>
    public bool NextResult()
    {
        rowsread = 0;
        return dr.NextResult();
    }



    /// <summary>
    /// Get column name by its number.
    /// </summary>
    /// <param name="ColumnNumber">Number of column to retrieve</param>
    /// <returns>A string</returns>
    public string ColumnName(int ColumnNumber)
    {
        return dr.GetName(ColumnNumber);
        
    }

    /// <summary>
    /// Returns true if there are rows to return.
    /// </summary>
    /// <returns>A boolean</returns>
    public bool HasRows
    {
        get {
           
            if (dr is MySqlDataReader)
                return ((MySqlDataReader)dr).HasRows;
            else if (dr is SqlCeDataReader)
                return ((SqlCeDataReader)dr).HasRows; // this may fail because CE doesn't always support it
            else
                return ((SqlDataReader)dr).HasRows;
        } 
        
    }

    /// <summary>
    /// Returns the number of fields in a recordset.
    /// </summary>
    /// <returns>An integer</returns>
    public int FieldCount
    {
        get { return dr.FieldCount; }
    }







    #region Get field functions

    /// <summary>
    /// Read a column value in as a DateTime object.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>DateTime object</returns>
    public DateTime GetDateTime(string ColumnName)
    {
        DateTime output = new DateTime(1900, 1, 1);
        int ord = dr.GetOrdinal(ColumnName);

        try
        {
            output = dr.GetDateTime(ord);
        }
        catch(Exception err)
        {
            lastError = err;
        }
        return output;
    }


    /// <summary>
    /// Read a column value in as a string formatted as a date.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A string</returns>
    public string GetDate(string ColumnName, string format = "M/d/yyyy")
    {
        string output = string.Empty;

        try
        {
            int ord = dr.GetOrdinal(ColumnName);

            if (dr.IsDBNull(ord)) output = "";
            else output = dr.GetDateTime(ord).ToString(format);
        }
        catch(Exception err)
        {
            lastError = err;
        }

        return output;
    }


   
    /// <summary>
    /// Read a column value in as a string formatted as a time.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A string</returns>
    public string GetTime(string ColumnName, string format = "h:mm tt")
    {
        string output = string.Empty;

        try
        {
            int ord = dr.GetOrdinal(ColumnName);

            if (dr.IsDBNull(ord)) output = "";
            else output = dr.GetDateTime(ord).ToString(format);
        }
        catch (Exception err)
        {
            lastError = err;
        }

        return output;
    }



    /// <summary>
    /// Read a column value in as a string formatted as money.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A string</returns>
    public string GetMoney(string ColumnName, int decimalPlaces = 2, bool withDollarSign = true)
    {
        string output = string.Empty;


        try
        {
           
            int ord = dr.GetOrdinal(ColumnName);

            if (dr.IsDBNull(ord)) output = "";
            else
            {
                string format = "#,0";
                if(decimalPlaces>1)
                    format += "." + "".PadRight(decimalPlaces,'0');

                if (dr is MySqlDataReader)
                    output = ((SqlDataReader)dr).GetSqlMoney(ord).ToDouble().ToString(format); // wonder if this is worth the extra work..  just use double?
                else
                    output = ((MySqlDataReader)dr).GetDouble(ord).ToString(format);

                if (withDollarSign)
                    output = "$" + output;
            }

        }
        catch (Exception err)
        {
            lastError = err;
        }


        return output;
    }

   


    /// <summary>
    /// Read a column value in as an integer.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>An integer</returns>
    public int GetInt(string ColumnName)
    {
        int output = 0;

        try
        {
            int ord = dr.GetOrdinal(ColumnName);

            if (dr.IsDBNull(ord)) output = 0;
            //else if (dr.GetDataTypeName(ord) == "tinyint") output = Convert.ToInt32(dr.GetByte(ord));
            else output = Convert.ToInt32(dr[ord].ToString());
            //else output = dr.GetInt32(ord);
        }
        catch (Exception err)
        {
            lastError = err;
        }

        return output;
    }

    /// <summary>
    /// Retrieve the data type name of a specified column.
    /// </summary>
    /// <param name="ColumnIndex">Number of the column</param>
    /// <returns>An string</returns>
    public string GetDataTypeName(int ColumnIndex)
    {
        string output = string.Empty;

        try
        {
            output = dr.GetDataTypeName(ColumnIndex);
        }
        catch (Exception err)
        {
            lastError = err;
        }

        return output;
    }




    /// <summary>
    /// Retrieve the data type name of a specified column.
    /// </summary>
    /// <param name="ColumnName">Name of the column</param>
    /// <returns>An string</returns>
    public string GetDataTypeName(string ColumnName)
    {
        return GetDataTypeName(dr.GetOrdinal(ColumnName));
    }

    /// <summary>
    /// Read a column value in as a double.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A double</returns>
    public double GetDouble(string ColumnName)
    {
        double output = 0;

        try
        {
            int ord = dr.GetOrdinal(ColumnName);

            if (dr.IsDBNull(ord)) output = 0;
            else output = Convert.ToDouble(dr[ord].ToString());
        }
        catch (Exception err)
        {
            lastError = err;
        }

        return output;
    }



    /// <summary>
    /// Read a column value in as a float.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A float</returns>
    public float GetFloat(string ColumnName)
    {
        float output = 0;

        try
        {
            int ord = dr.GetOrdinal(ColumnName);

            if (dr.IsDBNull(ord)) output = 0;
            else output = dr.GetFloat(ord);
        }
        catch (Exception err)
        {
            lastError = err;
        }

        return output;
    }
    /// <summary>
    /// Read a column value in as a decmal.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A float</returns>
    public decimal GetDecimal(string ColumnName)
    {
        decimal output = 0;

        try
        {
            int ord = dr.GetOrdinal(ColumnName);

            if (dr.IsDBNull(ord)) output = 0;
            else output = Convert.ToDecimal(dr[ord].ToString());
        }
        catch (Exception err)
        {
            lastError = err;
        }

        return output;
    }

    /// <summary>
    /// Read a column value in as a string, by its column number.
    /// </summary>
    /// <param name="ColumnIndex">Number of column to retrieve</param>
    /// <returns>A string</returns>
    public string GetString(int ColumnIndex)
    {
        string output = string.Empty;

        try
        {
            if (dr.IsDBNull(ColumnIndex)) output = "";
            else output = dr[ColumnIndex].ToString();
        }
        catch (Exception err)
        {
            lastError = err;
        }

        return output;
    }

    /// <summary>
    /// Read a column value in as a string, by its column name.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A string</returns>
    public string GetString(string ColumnName)
    {
        return GetString(dr.GetOrdinal(ColumnName));
    }

    public string GetString(string ColumnName, string DefaultValue)
    {
        string x = GetString(dr.GetOrdinal(ColumnName));
        if (x.Length == 0) return DefaultValue;
        else return x;

    }

    public string this[string ColumnName]
    {
        get
        {
            return GetString(ColumnName);
        }
    }

    public string this[int ColumnNumber]
    {
        get
        {
            return GetString(ColumnNumber);
        }
    }


    public DataTable GetTable()
    {
        // there is another way to do this using built in functions, 
        // but this way allows you to do multiple GetTables when there are multiple result sets

        var table = new DataTable();
        var fieldCount = dr.FieldCount;
        for (var i = 0; i < fieldCount; i++)
            table.Columns.Add(dr.GetName(i), dr.GetFieldType(i));

        table.BeginLoadData();
        var values = new object[fieldCount];
        while (dr.Read())
        {
            dr.GetValues(values);
            table.LoadDataRow(values, true);
        }
        table.EndLoadData();

        return table;

    }
    

    /// <summary>
    /// Access to the base DataReader
    /// </summary>
    public IDataReader DataReader
    {
        get { return dr; }
    }


    /// <summary>
    /// Identify whether the column value is null or empty
    /// </summary>
    /// <param name="ColumnName"></param>
    /// <returns></returns>
    public bool IsNullOrEmpty(string ColumnName)
    {
        return String.IsNullOrEmpty(GetString(dr.GetOrdinal(ColumnName)));
    }



    /// <summary>
    /// Gets a numeric field that represents minutes (ie 125)
    /// and converts it to an hour format (ie 2:05)
    /// </summary>
    /// <param name="ColumnName"></param>
    /// <returns></returns>
    public string GetStringOfMinutes(string ColumnName)
    {

        string output = "";


        string d = GetString(dr.GetOrdinal(ColumnName));

        if (!String.IsNullOrEmpty(d))
        {
            double x = Convert.ToDouble(d);
            output = Math.Floor(x / 60).ToString() + ":";
            x -= Math.Floor(x / 60) * 60;
            output += x.ToString().PadLeft(2, '0');
        }

        return output;


    }


    /// <summary>
    /// Read a column value in as a boolean
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A boolean</returns>
    public bool GetBoolean(string ColumnName)
    {
        bool output = false;

        try
        {
            int ord = dr.GetOrdinal(ColumnName);

            if (dr.IsDBNull(ord)) output = false;
            else output = dr.GetBoolean(ord);
        }
        catch (Exception err)
        {
            lastError = err;
        }

        return output;
    }


    /// <summary>
    /// Read a column value in and process as yes or no using the GetBoolean method.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A string</returns>
    public string GetYesNo(string ColumnName)
    {
        string output = "n/a";

        try
        {
            int ord = dr.GetOrdinal(ColumnName);

            if (dr.IsDBNull(ord)) output = "n/a";
            else if (dr.GetBoolean(ord)) output = "Yes";
            else output = "No";
        }
        catch
        { }

        return output;
    }


    #endregion


    
}


