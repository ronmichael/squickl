Squickl 
=============================================================
SQL+Quick or Quick+SQL - is a .NET library that makes it a breeze to access MSSQL and
MySQL data sources for those of us who don't want to build data access layers.

Setup
-------------------------------------------------------------
Squickl assumes that most of your data access is to one data source. So you start by setting up your default data source in your .config file and you're off.

	<appSettings>
		<add key="Squickl_DefaultConnection" value="mssql" />
	</appSettings>
	<connectionStrings>
		<add name="mssql" connectionString="Server=?;Database=?;Trusted_Connection=False;User ID=?;Password=?"/>
		<add name="mysql" providerName="MySql.Data.MySQLClient" connectionString="server=?;user=?;database=?;port=?;password=?;" />
	</connectionStrings>


Examples
-------------------------------------------------------------
Lookup something:

	string what = Squickl.Lookup("select top name from messages where author='Bob'");

Get a DataSet:

	DataTable msgs = Squickl.ReadTable("select top 10 * from messages");

Need to access a different database than your default? Make sure it's in your .config and pass the name to a function:

	string what = Squickl.Lookup("select top name from messages", "mysql");

Need to walk through a dataset? It's similar to DataReader (in fact you can access the base DataReader) but with helper functions to make it simpler:

	using(Squickl sl = new Squickl("select when, name, subject, cost from messages"))
	{
		while(sl.Read())
		{
			Console.WriteLine(sl.GetDate(when) + ": " + sl.GetString("subject"));
			Console.WriteLine("It cost " + sl.GetMoney("cost"));
		}
	}


Squickl also includes some extensions to the standard String to make lazy SQL programming easier.  .

SqlParam will make a string SQL safe - trim it, encapsulate it in single quotes, and double any single quotes inside so that they are handled properly. If the string is empty it'll return null. It'll even take a C# boolean and convert it to 1 or 0.

	string name = "Mc'Cormick";
	bool yn = true;
	string cmd = "update msgs set name=" + name.SqlParam() + ", complete=" + yn.SqlParam() where id=1";
	Squickl.Execute(cmd);



The MIT License
-------------------------------------------------------------
Copyright (c) 2012 Ron Michael Zettlemoyer
				
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
