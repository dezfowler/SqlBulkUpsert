using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SqlBulkUpsert
{
	public class UpserterBase
	{
	    protected SqlTableSchema TargetTableSchema { get; private set; }

	    protected UpserterBase(SqlTableSchema targetTableSchema)
		{
			if (targetTableSchema == null) throw new ArgumentNullException("targetTableSchema");
			TargetTableSchema = targetTableSchema;
		}

		protected Dictionary<int, int> PerformUpsert(SqlConnection connection, ICollection<string> columnNames, IDataReader dataReader)
		{
			SqlTableSchema tempTableSchema = GetTempTableSchema(TargetTableSchema, columnNames);

			using(var upsert = new Upsert(connection, TargetTableSchema, tempTableSchema, columnNames))
			{
				return upsert.Execute(dataReader);
			}
		}

		private static SqlTableSchema GetTempTableSchema(SqlTableSchema targetTableSchema, ICollection<string> columnNames)
		{
			// only columns we're inserting
			var tempTableColumnList = targetTableSchema.Columns
				.Where(c => columnNames.Contains(c.Name))
				.Select(c =>
				{
					var tempCol = c.Clone();
					tempCol.Nullable = true;
					return tempCol;
				})
				.ToList();

			// Add surrogate identity for temp table
			tempTableColumnList.Insert(0, new IdentityColumn());

			// prepare temp table schema
			return new SqlTableSchema("#upsert", tempTableColumnList);
		}
	}
}