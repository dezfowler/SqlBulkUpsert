using System;
using System.Data;

namespace SqlBulkUpsert
{
	public static class IDbConnectionEx
	{
		/// <summary>
		/// Execute multiple non-query commands against a connection.
		/// </summary>
		/// <param name="connection">The database connection to execute the commands on.</param>
		/// <param name="commands">The list of SQL Commands to execute.</param>
		public static void ExecuteCommands(this IDbConnection connection, string commands)
		{	
			if (null == connection) throw new ArgumentNullException("connection");
			if (null == commands) throw new ArgumentNullException("commands");
			
			using (var cmd = connection.CreateCommand())
			{
				var commandStrings = commands.Split(new[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var commandString in commandStrings)
				{
					cmd.CommandText = commandString;
					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}