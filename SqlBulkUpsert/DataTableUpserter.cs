using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SqlBulkUpsert
{
	/// <summary>
	/// Upserts to a target table from a DataTable
	/// </summary>
	public class DataTableUpserter : UpserterBase
	{
		public DataTableUpserter(SqlTableSchema targetTableSchema) : base(targetTableSchema) { }

		/// <summary>
		/// Update supplied DataTable with identity values of inserted records
		/// </summary>
		public bool RetrieveIdentity { get; set; }

		public IEnumerable<int> Upsert(SqlConnection connection, DataTable dataTable)
		{
			VerifyDataTable(dataTable, TargetTableSchema);

			var columnNames = dataTable.Columns.Cast<DataColumn>().Select(dc => dc.ColumnName).ToList();
			var dataTableReader = new DataTableReader(dataTable);
			
			Dictionary<int, int> inserts = PerformUpsert(connection, columnNames, dataTableReader);

			// return keys / update data table
			if (RetrieveIdentity)
			{
				var identityColumnName = TargetTableSchema.IdentityColumn.Name;
				foreach (var insert in inserts)
				{
					dataTable.Rows[insert.Key][identityColumnName] = insert.Value;
				}
			}

			return inserts.Values;
		}

		private void VerifyDataTable(DataTable dataTable, SqlTableSchema targetTableSchema)
		{
			// Ensure all columns in DataTable are in actual table schema
			foreach (DataColumn dataColumn in dataTable.Columns)
			{
				if (!targetTableSchema.Columns.Any(c => c.Name == dataColumn.ColumnName)) throw new Exception(String.Format("Column does not appear in table {0}", dataColumn.ColumnName));
			}

			// ensure primary key columns present in data table
			if (!targetTableSchema.PrimaryKeyColumns.All(c => dataTable.Columns.Contains(c.Name))) throw new Exception("Table primary key columns must feature in DataTable");
		}
	}
}