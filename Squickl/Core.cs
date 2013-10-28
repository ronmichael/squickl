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


public partial class Squickl : System.IDisposable
{



    public static Exception LastError { get; set; }


    /// <summary>
    /// When true (default) all exceptions will bubble up to caller.
    /// Otherwise exceptions are caught but available from LastError property;
    /// </summary>
    public static bool RaiseException
    {

        get {
            string config = ConfigurationManager.AppSettings["Squickl_RaiseExceptions"];
            if (!String.IsNullOrEmpty(config)) return Convert.ToBoolean(config);
            else return true;
        }

    }

    

    public static string SqlConnectionString(string name = "")
    {

        
        if (name.Length == 0)
            name = ConfigurationManager.AppSettings["Squickl_DefaultConnection"];
        

        if (ConfigurationManager.ConnectionStrings[name] != null)
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        else if (ConfigurationManager.ConnectionStrings.Count>0)
            return ConfigurationManager.ConnectionStrings[0].ConnectionString;
        else
            return "";

    }


    public static string Provider(string name = "")
    {

        string provider = "";

        if (String.IsNullOrEmpty(name)) name = ConfigurationManager.AppSettings["Squickl_DefaultConnection"];

        if (!String.IsNullOrEmpty(name) && ConfigurationManager.ConnectionStrings[name] != null) provider = ConfigurationManager.ConnectionStrings[name].ProviderName;

        if (String.IsNullOrEmpty(provider) && ConfigurationManager.ConnectionStrings.Count > 0) provider = ConfigurationManager.ConnectionStrings[0].ProviderName;

        if (String.IsNullOrEmpty(provider)) provider = "System.Data.SqlClient";

        return provider;

    }






    private static void HandleError(Exception err)
    {
        if (RaiseException) throw err;
        else LastError = err;

    }



}


