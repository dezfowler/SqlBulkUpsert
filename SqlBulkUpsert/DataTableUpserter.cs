using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
			if (null == dataTable) throw new ArgumentNullException("dataTable");
			if (null == connection) throw new ArgumentNullException("connection");

			VerifyDataTable(dataTable, TargetTableSchema);

			var columnNames = dataTable.Columns.Cast<DataColumn>().Select(dc => dc.ColumnName).ToList();

			Dictionary<int, int> inserts;
			using (var dataTableReader = new DataTableReader(dataTable))
			{
				inserts = PerformUpsert(connection, columnNames, dataTableReader);
			}

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

		private static void VerifyDataTable(DataTable dataTable, SqlTableSchema targetTableSchema)
		{
			// Ensure all columns in DataTable are in actual table schema
			foreach (DataColumn dataColumn in dataTable.Columns)
			{
				if (!targetTableSchema.Columns.Any(c => c.Name == dataColumn.ColumnName)) 
					throw new SqlBulkUpsertException(String.Format(CultureInfo.CurrentCulture, "Column {0} does not appear in table {1}", dataColumn.ColumnName, dataTable.TableName));
			}

			// ensure primary key columns present in data table
			if (!targetTableSchema.PrimaryKeyColumns.All(c => dataTable.Columns.Contains(c.Name)))
				throw new SqlBulkUpsertException("Table primary key columns must feature in the DataTable");
		}
	}
}