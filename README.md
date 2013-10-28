Squickl 
=============================================================
Squickl is a .NET library that simplifies access to SQL data sources 
when you don't want or need an ORM. 


Installation
-------------------------------------------------------------
Squickl is available via NuGet (https://www.nuget.org/packages/Squickl) can can be installed from the Package Manager:

	PM> Install-Package Squickl


Examples
-------------------------------------------------------------
Execute a statement that returns no results:

	Squickl.Execute("update colors set name='Red' where name='Redd'");

Lookup a single value:

	string what = Squickl.Lookup("select top name from messages where author='Bob'");

Get a DataTable:

	DataTable msgs = Squickl.ReadTable("select top 10 * from messages");

Get a single dynamic record:

	dynamic record = Squickl.Query1("select top 10 * from colors");
	Console.WriteLine(sx.ColorName + ", " + sx.ColorNumber); // all the columns are properties of the dynamic object

Iterate over all records in a result set using dynamic foreach syntax:

	foreach (dynamic sx in Squickl.Query("select * from colors")) 
	{
		Console.WriteLine(sx.ColorName);
	}

Execute 3 queries; walk through the first two results using typical DataReader syntax, pull the third into a DataTable:

	using(var sl = new Squickl("select when, name, subject, cost from messages; exec dbo.spu_Moredata; exec dbo.spu_Colors"))
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

Need to access a different database than your default? Make sure it's in your .config and pass the name to a function:

	string what = Squickl.Lookup("select top name from messages", "mysql");


Extensions
-------------------------------------------------------------
Squickl also includes some extensions to standard data types to make lazy SQL programming easier.

SqlParam will make a string SQL safe - trim it, encapsulate it in single quotes, and double any single quotes inside so that they are handled properly. 
If the string is empty it'll return null. It'll even take a C# boolean and convert it to 1 or 0.

	string name = "Mc'Cormick";                   
	bool yn = true; 
	DateTime when = Convert.ToDateTime("2/1/2013 2:35pm"); 
	
	string cmd1 = "update msgs set " + name=" + name.SqlParam() + ", complete=" + yn.SqlParam() + ", when=" + when.SqlParam() + " where id=1";
	// cmd1 -> update msg set name='Mc''Cormick', complete=1, when='2/1/2013 2:35 pm' where id=1";
	
	string cmd2 = "exec spu_Update id=1, " + name.SqlParam("name") + ", " + yn.SqlParam("complete") + ", " + when.SqlParam("when");
	// cmd2 -> exec spu_Update id=1, @name='Mc''Cormick', @complete=1, @when='2/1/2013 2:35 pm'
	
	Squickl.Execute(cmd1);
	Squickl.Execute(cmd2);


Setup
-------------------------------------------------------------
Squickl assumes that most of your data access is to one data source. So you start by setting up your default data source in your .config file and you're off.

	<appSettings>
		<add key="Squickl_DefaultConnection" value="mssql" />
		<add key="Squickl_RaiseExceptions" value="false" />
	</appSettings>
	<connectionStrings>
		<clear />
		<add name="mssql" providerName="System.Data.SqlClient" connectionString="Server=?;Database=?;Trusted_Connection=False;User ID=?;Password=?"/>
		<add name="mysql" providerName="MySql.Data.MySQLClient" connectionString="server=?;user=?;database=?;port=?;password=?;" />
	    <add name="mssqlce" providerName="System.Data.SqlServerCe.4.0" connectionString="Data Source=|DataDirectory|\data.sdf" />
	</connectionStrings>

Notes:

- Squickl_DefaultConnection: Name of connection string to use by default. If not set, Squickl will use the first connection string it finds.
Be careful: The first connection string in your local config file may not be the actual first connection string that dotNet sees; 
there may be connection strings in machine.config. You may want to <clear /> your list first.
- Squickl_RaiseExceptions: Defaults to false. When true, exceptions can arise from calling Squickl. 
When false all exceptions are trapped but available by looking at LastError property.



Change history
-------------------------------------------------------------

1.12 - October 26th, 2013
- Added Query1 function to return one dynamic row; see new sample

1.11 - October 19th, 2013
- Added Query function for dynamic foreach reading; see new sample

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



Known issues
-------------------------------------------------------------
SqlClean in Extensions doesn't handle \' in MySQL strings.


