using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;


namespace SquicklTest
{
    class Program
    {
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


            using (Squickl sr = new Squickl("select * from colors order by name"))
            {
                
                Console.WriteLine("Reader columns\r\n");

                sr.Read();

                foreach(Squickl.Column c in sr.Columns)
       
                {
                     
                    Console.WriteLine(c.Name + " = " + c.RemoteType  + "," + c.LocalType);
                }


                Console.WriteLine("\r\nReader data\r\n");

                // SQL CE: HasRows won't work; NextResult won't work (no multiple return sets)

                while (sr.Read())
                {
                    Console.WriteLine("Row " + sr.RowsRead + ": " + sr.GetString("name") + " or " + sr["name"]);
                }


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
