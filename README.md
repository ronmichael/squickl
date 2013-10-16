Squickl 
=============================================================
Squickl is a .NET library that makes it a breeze to access just about any SQL data source.
It works well in desktop or web applications and is for developers who don't like, want or need
a data layer or ORM.
It's the descendant of SmartReader, a library I wrote and have been using for years in dozens of projects.


Change history
-------------------------------------------------------------
1.10 - October 25th, 2013
- Significant refactoring & cleanup
- Now uses DbProviderFactory for more universal database support; compatible with Glimpse
- SqlLookup renamed to Lookup; SqlExec renamed to Exec; added Exists
- New implementation of access to Columns and column definitions
- Can now set in .config whether you want exceptions to be raised or not; default is true

1.01 - March 25th, 2013
- Added support for SQL Server Compact Edition
- Minor code refactoring
- Added SquicklTest project for testing purposes

1.00 - February 29th, 2012
- Initial release


Setup
-------------------------------------------------------------
Squickl assumes that most of your data access is to one data source. So you start by setting up your default data source in your .config file and you're off.

	<appSettings>
		<add key="Squickl_DefaultConnection" value="mssql" />
		<add key="Squickl_RaiseExceptions" value="false" />
	</appSettings>
	<connectionStrings>
		<add name="mssql" providerName="System.Data.SqlClient" connectionString="Server=?;Database=?;Trusted_Connection=False;User ID=?;Password=?"/>
		<add name="mysql" providerName="MySql.Data.MySQLClient" connectionString="server=?;user=?;database=?;port=?;password=?;" />
	    <add name="mssqlce" providerName="System.Data.SqlServerCe.4.0" connectionString="Data Source=|DataDirectory|\data.sdf" />
	</connectionStrings>

Notes:

- Squickl_DefaultConnection: Name of connection string to use by default. If not set, Squickl will use the first connection string it finds.
- Squickl_RaiseExceptions: Defaults to false. When true, exceptions can arise from calling Squickl. When false all exceptions are trapped but available by looking at LastError property.



Examples
-------------------------------------------------------------
Lookup something:

	string what = Squickl.Lookup("select top name from messages where author='Bob'");

Get a DataTable:

	DataTable msgs = Squickl.ReadTable("select top 10 * from messages");

Need to access a different database than your default? Make sure it's in your .config and pass the name to a function:

	string what = Squickl.Lookup("select top name from messages", "mysql");

Need to walk through a dataset? It's similar to DataReader (in fact you can access the base DataReader) but with helper functions to make it simpler:

	using(Squickl sl = new Squickl("select when, name, subject, cost from messages; exec dbo.spu_Moredata; exec dbo.spu_Colors"))
	{
	
		while(sl.Read())
		{
			Console.WriteLine(sl.GetDate(when) + ": " + sl.GetString("subject"));
			Console.WriteLine("It cost " + sl.GetMoney("cost"));
		}
		
		sl.NextResult();
		sl.Read();
		Console.WriteLine(sl.GetString("data"));
		
		sl.NextResult();
		DataTable colors = sl.GetTable();
		
	}


Squickl also includes some extensions to the standard String to make lazy SQL programming easier.  .

SqlParam will make a string SQL safe - trim it, encapsulate it in single quotes, and double any single quotes inside so that they are handled properly. If the string is empty it'll return null. It'll even take a C# boolean and convert it to 1 or 0.

	string name = "Mc'Cormick";                   
	bool yn = true; 
	DateTime when = Convert.ToDateTime("2/1/2013 2:35pm"); 
	
	string cmd1 = "update msgs set " + name=" + name.SqlParam() + ", complete=" + yn.SqlParam() + ", when=" + when.SqlParam() + " where id=1";
	// cmd1 -> update msg set name='Mc''Cormick', complete=1, when='2/1/2013 2:35 pm' where id=1";
	
	string cmd2 = "exec spu_Update id=1, " + name.SqlParam("name") + ", " + yn.SqlParam("complete") + ", " + when.SqlParam("when");
	// cmd2 -> exec spu_Update id=1, @name='Mc''Cormick', @complete=1, @when='2/1/2013 2:35 pm'
	
	Squickl.Execute(cmd1);
	Squickl.Execute(cmd2);


Known issues
-------------------------------------------------------------
SqlClean in Extensions doesn't handle \' in MySQL strings.


To do
-------------------------------------------------------------
- Get functions should probably raise an exception rather than hide it in LastError
- Better handling of dynamic SQL CE filenames and MSSQL/MySQL connection strings 
- Cleaner get functions?


