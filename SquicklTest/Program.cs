using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Dynamic;

namespace SquicklTest
{
    class Program
    {


        private static dynamic SqlDataReaderToExpando(Squickl reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var i = 0; i < reader.FieldCount; i++)
                expandoObject.Add(reader.Columns[i].Name, reader.DataReader[i]);

            return expandoObject;
        }

        private static IEnumerable<dynamic> GetDynamicSqlData( string sql)
        {
            using (Squickl sqlx = new Squickl(sql))
            {
                while (sqlx.Read())
                {
                    yield return SqlDataReaderToExpando(sqlx);
                }
            }
        }


        static void Main(string[] args)
        {


            // copy the database from the project into the temp folder and use it from there
            /*
            string db = Path.GetTempPath() + "\\test.sdf";
            File.Delete(db);
            File.Copy("squickl.sdf", db);
            Console.WriteLine("Using database in " + db);
            */

          

            // read from the color table
            Console.WriteLine("\r\n\r\nReader columns\r\n");


            using (Squickl sr = new Squickl("select * from colors order by name"))
            {
                
          
                foreach(Squickl.Column c in sr.Columns)
       
                {
                    Console.WriteLine(c.Name + " = " + c.RemoteType + "," + c.LocalType);
                }


                Console.WriteLine("\r\nReader data\r\n");

                // SQL CE: HasRows won't work; NextResult won't work (no multiple return sets)

                while (sr.Read())
                {
                    Console.WriteLine("Row " + sr.RowsRead + ": " + sr.GetString("name") + " or " + sr["name"]);
                   
                }


            }


            Console.WriteLine("\r\n\r\nDynamic query\r\n");

            foreach (dynamic sx in Squickl.Query("select * from colors"))
            {
                // column name properties are not case sensitive
                Console.WriteLine(sx.name + "," + sx.NAME.GetType() + " = " + sx.number + "," + sx.Number.GetType());

      
            }






            Console.WriteLine("\r\nExtensions\r\n");

            string xx = "turt'wax''le";
            Console.WriteLine("string " + xx + " : " + xx.SqlParam() + " : " + xx.SqlParam("x"));

            xx = "";
            Console.WriteLine("string " + xx + " : " + xx.SqlParam() + " : " + xx.SqlParam("x"));

            int x = 42;
            Console.WriteLine("int " + x.ToString() + " : " + x.SqlParam() + " : " + x.SqlParam("x"));

            long y = 5012;
            Console.WriteLine("long " + y.ToString() + " : " + y.SqlParam() + " : " + y.SqlParam("x"));

            bool a = false;
            Console.WriteLine("bool " + a.ToString() + " : " + a.SqlParam() + " : " + a.SqlParam("x"));

            a = true;
            Console.WriteLine("bool " + a.ToString() + " : " + a.SqlParam() + " : " + a.SqlParam("x"));

            DateTime d = DateTime.Now;
            Console.WriteLine("DateTime " + d.ToString() + " : " + d.SqlParam() + " : " + d.SqlParam("x"));
            


            // clean up
            
            Console.WriteLine("\r\nDone. Press a key to finish.");
            Console.ReadKey();

        }


    }
}
