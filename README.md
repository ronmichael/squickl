Squickl - SQL+Quick or Quick+SQL - is a .NET library that makes it a breeze to access MSSQL and MySQL data sources for those of us who don't want to build data access layers.

Squickl assumes that most of your data access is to one data source. So you start by setting up your default data source in your .config file and you're off.

	<appSettings>
		<add key="Squickl_DefaultConnection" value="mssql" />
	</appSettings>
	<connectionStrings>
		<add name="mssql" connectionString="Server=?;Database=?;Trusted_Connection=False;User ID=?;Password=?"/>
		<add name="mysql" providerName="MySql.Data.MySQLClient" connectionString="server=?;user=?;database=?;port=?;password=?;" />
	</connectionStrings>


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



