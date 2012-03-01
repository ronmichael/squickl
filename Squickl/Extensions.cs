using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public static class SquicklExtensions
{
    /// <summary>
    /// Cleans up a string so that it can be safely passed as a parameter to SQL
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string SqlClean(this string value)
    {
        return value.Replace("'", "''");

    }

    public static string SqlParam(this bool value)
    {
        
        return (value ? "1" : "0");


    }



    public static string SqlParam(this string value)
    {
        if (value == null) value = "";

        string nv = value.Replace("'", "''").Trim();
        if (nv.Length == 0) return "null";
        else return "'" + nv + "'";

    }




}
