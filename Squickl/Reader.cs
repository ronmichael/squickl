using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web;
using System.Xml;
using System.Data.Common;
using System.Dynamic;

/// <summary>
/// This is the Reader component of Squickl, which you'd instantiate to walk through a dataset.
/// e.g. using Squickl reader = new Squickl("select * from data")) { while reader.Read() {  } }
/// </summary>
//public partial class Squickl : DynamicObject, System.IDisposable, IDynamicMetaObjectProvider
public partial class Squickl : DynamicObject, System.IDisposable, IDynamicMetaObjectProvider
{

    private DbDataReader dr;
    private DbConnection cn;
    private DbCommand cmd;
    private Column[] ColumnsCache;

    public int RowsRead { get; set; }

    public class Column
    {
        public string Name;
        public string RemoteType;
        public Type LocalType;
    }

    public DbDataReader DataReader
    {
        get { return dr; }
    }



    public Squickl(string CommandText)
    {
        string provider = Provider();

        DbProviderFactory dbf = DbProviderFactories.GetFactory(provider);
        cn = dbf.CreateConnection();
        cn.ConnectionString = SqlConnectionString();
        cn.Open();

        cmd = dbf.CreateCommand();
        cmd.CommandText = CommandText;
        cmd.Connection = cn;
        if (!provider.ToLower().Contains("sqlserverce")) // can't use this with CE
            cmd.CommandTimeout = 120; // should be configurable

        dr = cmd.ExecuteReader();

    }



    public bool HasRows
    {
        get { return dr.HasRows; }
    }


    public int FieldCount
    {
        get { return dr.FieldCount; }
    }




   
    public Column[] Columns
    {
        get
        {
           
            if (ColumnsCache == null)
            {
                ColumnsCache = new Column[dr.FieldCount];
                
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    Columns[i] = new Column();
                    Columns[i].Name = dr.GetName(i);
                    Columns[i].RemoteType = dr.GetDataTypeName(i).ToLower();
                    Columns[i].LocalType = dr.GetFieldType(i);
                }
                
            }
            return ColumnsCache;

        }
    }





    


    /// <summary>
    /// Read the next row in the query.
    /// </summary>
    /// <returns>true if read, false if end of data</returns>
    public bool Read()
    {
        bool output = dr.Read();
        if (output) RowsRead++;
        return output;
    }


    /// <summary>
    /// Moves to the next result set
    /// </summary>
    /// <returns>True of there is another result set; false if none</returns>
    public bool NextResult()
    {
        RowsRead = 0;
        ColumnsCache = null;
        return dr.NextResult();
    }


    public DataTable GetTable()
    {
        // there is a built-in way to do this but this way allows you to do multiple GetTables when there are multiple result sets

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



    public bool IsNullOrEmpty(string ColumnName)
    {
        return String.IsNullOrEmpty(GetString(ColumnName));
    }





    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        result = null;

        try { 
            int ord = dr.GetOrdinal(binder.Name);
            result = dr[ord];
            return true;
        }
        catch {}
    
        /*
        // Next check for Public properties via Reflection
        if (Instance != null)
        {
            try
            {
                return GetProperty(Instance, binder.Name, out result);
            }
            catch { }
        }
        */
        // failed to retrieve a property
        result = null;
        return false;
    }

    public string GetString(string ColumnName)
    {
        
    
        int idx = dr.GetOrdinal(ColumnName); 

        string output = string.Empty;
        
        if (!dr.IsDBNull(idx)) output = dr[idx].ToString();

        return output;

    }


    public string this[string columnName]
    {
        get { return GetString(columnName); }
    }




    public Guid GetGuid(String ColumnName)
    {
        return dr.GetGuid(dr.GetOrdinal(ColumnName));
    }



    /// <summary>
    /// Read a column value in as a DateTime object.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>DateTime object</returns>
    public DateTime GetDateTime(string ColumnName)
    {
        return dr.GetDateTime(dr.GetOrdinal(ColumnName));
    }


    /// <summary>
    /// Read a column value in as a string formatted as a date.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A string</returns>
    public string GetDate(string ColumnName)
    {
        string output = string.Empty;

        int ord = dr.GetOrdinal(ColumnName);

        if (!dr.IsDBNull(ord))
            output = dr.GetDateTime(ord).ToString("M/d/yyyy");

        return output;
    }



    /// <summary>
    /// Read a column value in as a string formatted as a time.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A string</returns>
    public string GetTime(string ColumnName)
    {
        string output = string.Empty;

        int ord = dr.GetOrdinal(ColumnName);

        if (!dr.IsDBNull(ord))
            output = dr.GetDateTime(ord).ToString("h:mm tt");

        return output;
    }



    /// <summary>
    /// Read a column value in as a string formatted as money
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A string</returns>
    public string GetMoney(string ColumnName, bool prefix = true)
    {
        string output = string.Empty;

        int ord = dr.GetOrdinal(ColumnName);

        if (!dr.IsDBNull(ord))
            output = (prefix ? "$" : "") + dr.GetDecimal(ord).ToString("#,0.00");

        return output;
    }




    /// <summary>
    /// Read a column value in as an long/bigint
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>An integer</returns>
    public long GetLong(string ColumnName)
    {
        long output = 0;

        int ord = dr.GetOrdinal(ColumnName);

        if (!dr.IsDBNull(ord))
            output = Convert.ToInt64(dr[ord].ToString());

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

        int ord = dr.GetOrdinal(ColumnName);

        if (!dr.IsDBNull(ord))
            output = Convert.ToInt32(dr[ord].ToString());

        return output;
    }



    /// <summary>
    /// Read a column value in as a double.
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A double</returns>
    public double GetDouble(string ColumnName)
    {
        double output = 0;

        int ord = dr.GetOrdinal(ColumnName);

        if (!dr.IsDBNull(ord))
            output = Convert.ToDouble(dr[ord].ToString());

        return output;
    }



    /// <summary>
    /// Read a column value in as a float -- should depricate this, shouldn't really use this in CSC
    /// </summary>
    /// <param name="ColumnName">Name of column to retrieve</param>
    /// <returns>A float</returns>
    public float GetFloat(string ColumnName)
    {
        float output = 0;

        int ord = dr.GetOrdinal(ColumnName);

        if (!dr.IsDBNull(ord))
            output = (float)Convert.ToDecimal(dr[ord].ToString());

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

        int ord = dr.GetOrdinal(ColumnName);

        if (!dr.IsDBNull(ord))
            output = Convert.ToDecimal(dr[ord].ToString());

        return output;
    }

    


    /// <summary>
    /// Gets a numeric field that represents minutes (ie 125)
    /// and converts it to an hour format (ie 2:05)
    /// </summary>
    /// <param name="ColumnName"></param>
    /// <returns></returns>
    public string GetStringOfMinutes(string ColumnName)
    {

        string output = String.Empty; 


        string d = GetString(ColumnName);

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
            HandleError(err);
        }


        return output;
    }


    /// <summary>
    /// Read a column value in and process as Yes or No using the GetBoolean method; null = n/a
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
        catch (Exception err)
        {
            HandleError(err);
        }


        return output;
    }









    ~Squickl()
    {
        Dispose();
    }

    public void Dispose()
    {
        try
        {
            cn.Dispose();
            cmd.Dispose();
        }
        catch { }
    }




}


