using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SqlBulkUpsert
{
	/// <summary>
	/// A single upsert operation
	/// </summary>
	class Upsert : IDisposable
	{
		private SqlConnection _connection;
		private readonly SqlTableSchema _targetTableSchema;
		private readonly SqlTableSchema _tempTableSchema;
		private readonly ICollection<string> _columnNames;

		public Upsert(SqlConnection connection, SqlTableSchema targetTableSchema, SqlTableSchema tempTableSchema, ICollection<string> columnNames)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (targetTableSchema == null) throw new ArgumentNullException("targetTableSchema");
			if (tempTableSchema == null) throw new ArgumentNullException("tempTableSchema");
			if (columnNames == null) throw new ArgumentNullException("columnNames");
			_connection = connection;
			_targetTableSchema = targetTableSchema;
			_tempTableSchema = tempTableSchema;
			_columnNames = columnNames;
		}

		public void Dispose()
		{
			_connection = null;
		}

		private void CreateTempTable()
		{
			string createTempTableSql = _tempTableSchema.ToCreateTableCommandText();
			_connection.ExecuteCommands(createTempTableSql);
		}

		private void BulkLoadTempTable(IDataReader dataTableReader)
		{
			using (var copy = new SqlBulkCopy(_connection))
			{
				copy.DestinationTableName = "#upsert";
				foreach (var columnName in _columnNames)
				{
					copy.ColumnMappings.Add(columnName, columnName);
				}
				copy.WriteToServer(dataTableReader);
			}
		}

		private Dictionary<int, int> UpsertFromTempTable()
		{
			var inserts = new Dictionary<int, int>();

			using (var selectCommand = _connection.CreateCommand())
			{
				var mergeCommand = new MergeCommand(_tempTableSchema, _targetTableSchema);

				selectCommand.CommandText = mergeCommand.ToString();

				using (var reader = selectCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						var action = (string)reader["action"];
						if (action == "INSERT")
						{
							var ident = (int)reader["ident"];
							var surrogateKey = (int)reader["_Surrogate"];
							inserts.Add(surrogateKey, ident);
						}
					}
				}
			}
			return inserts;
		}

		private void DropTempTable()
		{
			string dropTempTableSql = _tempTableSchema.ToDropTableCommandText();
			_connection.ExecuteCommands(dropTempTableSql);
		}

		public Dictionary<int, int> Execute(IDataReader dataReader)
		{
			CreateTempTable();
			BulkLoadTempTable(dataReader);
			Dictionary<int, int> inserts = UpsertFromTempTable();
			DropTempTable();
			return inserts;
		}
	}
}