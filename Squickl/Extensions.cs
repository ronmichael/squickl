using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Web.UI.HtmlControls;


/// <summary>
/// These are a collection of extensions to common classes (strings, ints, etc)
/// to perform common SQL-related tasks with their data
/// </summary>
public static class SquicklExtensions
{

    public static string SqlClean(this string value)
    {
        if (String.IsNullOrEmpty(value)) return "";
        else return value.Replace("'", "''");
    }



    public static string SqlParam(this string value, string name = "")
    {
        string nv = value.SqlClean();
        if (nv.Length == 0) nv = "null";
        else nv = "'" + nv + "'";

        if (String.IsNullOrEmpty(name)) return nv;
        else return  "@" + name + "=" + nv;
    }


    public static string SqlParam(this bool value, string name = "")
    {
        return (value ? "1" : "0").SqlParam(name);
    }

    public static string SqlParam(this int value, string name = "")
    {
        return value.ToString().SqlParam(name);
    }

    public static string SqlParam(this long value, string name = "")
    {
        return value.ToString().SqlParam(name);
    }

    public static string SqlParam(this DateTime value, string name = "")
    {
        return value.ToString("M/d/yyyy h:mm tt").SqlParam(name);
    }

    public static string SqlParam(this HtmlInputHidden input, string name = "")
    {
        return input.Value.SqlParam(name);
    }

    public static string SqlParam(this HtmlInputText input, string name = "")
    {
        return input.Value.SqlParam(name);
    }

    public static string SqlParam(this HtmlTextArea input, string name = "")
    {
        return input.Value.SqlParam(name);
    }



    /// <summary>
    /// Takes a field name (MyField, My_Field) and makes it look pretty for a user (My Field)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FriendlyFieldName(this string value)
    {

        string f = value.Replace("_", " ");

        for (int x = 1; x < f.Length; x++)
        {
            char a = f[x - 1];
            char b = f[x];
            if (b >= 'A' && b <= 'Z' && a >= 'a' && a <= 'z')
                f = f.Insert(x, " ");

        }

        return f;

    }







}
