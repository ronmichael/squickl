using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;


/// <summary>
/// SmartReader.Dynamicreader contains a new dynamic reader.
/// Thanks to:
/// http://blogs.msdn.com/b/davidebb/archive/2009/10/29/using-c-dynamic-to-simplify-ado-net-use.aspx
/// http://www.west-wind.com/weblog/posts/2012/Feb/08/Creating-a-dynamic-extensible-C-Expando-Object
/// http://phejndorf.wordpress.com/2011/10/30/using-c-dynamic-with-sqldatareader/
/// </summary>
public partial class Squickl : System.IDisposable
{

    private class DataRecordDynamicWrapper : DynamicObject
    {
        private IDataRecord _dataRecord;
        public DataRecordDynamicWrapper(IDataRecord dataRecord) { _dataRecord = dataRecord; }

        // always returns in a data type equivalent to sql...
        // referencing a field that doesn't exist will always raise an exception right now...

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // could you add a function to the returned object?
            //dynamic rx = _dataRecord[binder.Name];
            //rx.SqlDataType = (Func<string,string>) ((string name)=>{ return "test"; });
            //result = rx;

            result = _dataRecord[binder.Name];

            return result != null;
        }
    }



    public static dynamic Query1(string commandText)
    {
        DbProviderFactory dbf = DbProviderFactories.GetFactory(Provider());

        using (DbConnection cn = dbf.CreateConnection())
        {

            cn.ConnectionString = SqlConnectionString();
            cn.Open();

            using (DbCommand cmd = dbf.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Connection = cn;
                // if (!provider.ToLower().Contains("sqlserverce")) // can't use this with CE
                //   cmd.CommandTimeout = 120; // should be configurable

                using (DbDataReader dr = cmd.ExecuteReader())
                {

                    var result = new ExpandoObject() as IDictionary<string, Object>;

                    bool hasData = dr.Read();

                    result.Add("hasdata", hasData);

                    if (hasData)
                    {

                        for (int x = 0; x < dr.FieldCount; x++)
                        {
                            result.Add(dr.GetName(x), dr[x]);
                        }

                    }

                    return result;            

                }
            }
        }
    }


    public static IEnumerable<dynamic> Query( string commandText)
    {

        DbProviderFactory dbf = DbProviderFactories.GetFactory(Provider());

        using (DbConnection cn = dbf.CreateConnection())
        {

            cn.ConnectionString = SqlConnectionString();
            cn.Open();

            using (DbCommand cmd = dbf.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Connection = cn;
               // if (!provider.ToLower().Contains("sqlserverce")) // can't use this with CE
                 //   cmd.CommandTimeout = 120; // should be configurable

                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    foreach(IDataRecord record in dr)
                    {
                        yield return new DataRecordDynamicWrapper(record);
                    }


                }
            }
        }
    
    }




}


