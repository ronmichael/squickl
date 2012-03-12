﻿using System;
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
        // have to deal with \' issue in mysql...
        return value.Replace("'", "''");

    }


    /// <summary>
    /// Prepare a boolean value for use in a SQL string - converts it to a 1 or a 0
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string SqlParam(this bool value)
    {
        
        return (value ? "1" : "0");


    }


    /// <summary>
    /// Prepare a string value for use in a SQL string - first trim it,
    /// then if it's empty return null otherwise return it in single quotes
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string SqlParam(this string value)
    {
        if (value == null) value = "";

        string nv = value.Trim().SqlClean();
        if (nv.Length == 0) return "null";
        else return "'" + nv + "'";

    }




}
