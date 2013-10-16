using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;


/// <summary>
/// SmartReader.Tools contains various static functions related to database access
/// that don't require instantiating a SmartReader object
/// </summary>
public partial class Squickl : System.IDisposable
{



    /// <summary>
    /// Executes a SQL statement and reads the resultset into a DataTable
    /// </summary>
    /// <param name="sqlcmd">SQL command string.</param>
    /// <param name="addBlank">Add a blank row to the end?</param>
    /// <returns></returns>
    public static DataTable ReadTable(string sqlcmd)
    {

        DataTable outp = new DataTable();

        try
        {

            DbProviderFactory dbf = DbProviderFactories.GetFactory(Provider());
            using (DbConnection con = dbf.CreateConnection())
            {
                con.ConnectionString = SqlConnectionString();
                con.Open();
                using (DbDataAdapter da = dbf.CreateDataAdapter())
                {
                    DbCommand cmd = dbf.CreateCommand();
                    cmd.CommandText = sqlcmd;
                    cmd.Connection = con;

                    da.SelectCommand = cmd;
                    da.Fill(outp);


                }
            }

        }
        catch (Exception err)
        {
            HandleError(err);
        }


        return outp;
    }


    /// <summary>
    /// Executes a single SQL statement
    /// </summary>
    /// <param name="statement"></param>
    /// <returns></returns>
    public static bool Exec(string sqlcmd)
    {

        try
        {

            DbProviderFactory dbf = DbProviderFactories.GetFactory(Provider());
            using (DbConnection con = dbf.CreateConnection())
            {
                con.ConnectionString = SqlConnectionString();
                con.Open();
                using (DbDataAdapter da = dbf.CreateDataAdapter())
                {
                    DbCommand cmd = dbf.CreateCommand();
                    cmd.CommandText = sqlcmd;
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }

        }
        catch (Exception err)
        {
            HandleError(err);
            return false;
        }

    }




    public static bool Exists(string sqlcmd)
    {

        // Exists doesn't exist in SQL CE

        string results = Lookup("if exists (" + sqlcmd + ") select '1' else select '0'");

        return (results == "1");

        // should return error or exception if neither 1 or 0 is returned...

    }




    public static int Lookup(string statement, int ErrorValue)
    {
        string results = Lookup(statement, "");

        try
        {
            if (results.Length > 0) return Convert.ToInt32(results);
        }
        catch { }

        return ErrorValue;
    }


    public static string Lookup(string sql)
    {
        return Lookup(sql, "");
    }



    public static string Lookup(string sqlcmd, string defValue)
    {
        string ret = defValue;

        try
        {

            DbProviderFactory dbf = DbProviderFactories.GetFactory(Provider());
            using (DbConnection con = dbf.CreateConnection())
            {
                con.ConnectionString = SqlConnectionString();
                con.Open();
                using (DbDataAdapter da = dbf.CreateDataAdapter())
                {
                    DbCommand cmd = dbf.CreateCommand();
                    cmd.CommandText = sqlcmd;
                    cmd.Connection = con;
                    ret = cmd.ExecuteScalar().ToString();
                }
            }

        }
        catch (Exception err)
        {
            HandleError(err);
        }

        return ret;
    }







}


