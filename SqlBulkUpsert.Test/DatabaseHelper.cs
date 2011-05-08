using System.Data.SqlClient;
using SqlBulkUpsert.Test.Properties;

namespace SqlBulkUpsert.Test
{
	internal static class DatabaseHelper
	{
		public static void RefreshSchema()
		{
			ExecuteCommands(Resources.DatabaseSchemaRefresh);
		}

		/// <summary>
		/// Execute some SQL against the database
		/// </summary>
		/// <param name="sqlCommandText">SQL containing one or multiple command separated by \r\nGO\r\n</param>
		public static void ExecuteCommands(string sqlCommandText)
		{
			using (var connection = CreateAndOpenConnection())
			{
				connection.ExecuteCommands(sqlCommandText);
			}
		}

		public static SqlConnection CreateAndOpenConnection()
		{
			var sqlConnection = new SqlConnection(@"Data Source=.\SQLSERVER2008;Initial Catalog=SqlBulkUpsertTestDb;Integrated Security=SSPI;");
			sqlConnection.Open();
			return sqlConnection;
		}

		public static object ExecuteScalar(this SqlConnection connection, string sqlCommandText)
		{
			using(var cmd = new SqlCommand(sqlCommandText, connection))
			{
				return cmd.ExecuteScalar();
			}
		}
	}
}