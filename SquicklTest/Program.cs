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

            string db = Path.GetTempPath() + "\\squickl.sdf";
            File.Delete(db);
            File.Copy("squickl.sdf", db);
            Console.WriteLine("Using database in " + db);



            // read from the color table

            using (Squickl sr = new Squickl("select * from colors"))
            {
                // Console.WriteLine("has rows? " + sr.HasRows); // won't work in CE

                while (sr.Read())
                {
                    Console.WriteLine(sr.GetString("name"));
                }
            }



            // clean up

            File.Delete(db);

            Console.WriteLine("Done. Press a key to finish.");

            Console.ReadKey();
        }


    }
}
