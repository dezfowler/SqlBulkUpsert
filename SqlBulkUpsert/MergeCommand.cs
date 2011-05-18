using System;
using System.Globalization;
using System.Linq;
using SqlBulkUpsert.Properties;

namespace SqlBulkUpsert
{
	public class MergeCommand
	{
		private readonly SqlTableSchema _sourceTableSchema;
		private readonly SqlTableSchema _targetTableSchema;

		public MergeCommand(SqlTableSchema sourceTableSchema, SqlTableSchema targetTableSchema)
		{
			if (sourceTableSchema == null) throw new ArgumentNullException("sourceTableSchema");
			if (targetTableSchema == null) throw new ArgumentNullException("targetTableSchema");
			_sourceTableSchema = sourceTableSchema;
			_targetTableSchema = targetTableSchema;
		}


		public override string ToString()
		{
			return String.Format(
				CultureInfo.InvariantCulture,
                Resources.MergeStatement,
				_targetTableSchema.TableName,
				_sourceTableSchema.TableName,
				GetJoinCriteriaString(),
				GetSetString(),
				GetValuesList()
				);

		}
		private string GetJoinCriteriaString()
		{
			return _targetTableSchema.PrimaryKeyColumns
				.Select(c => String.Format(CultureInfo.InvariantCulture, "target.{0} = source.{0}", c.ToSelectListString()))
				.ToArray()
				.Convert(a => String.Join(" AND ", a));
		}

		private string GetSetString()
		{
			// exclude primary key and identity columns
			var columnsToBeSet = _sourceTableSchema.Columns
				.Join(_targetTableSchema.Columns, c => c.Name, c => c.Name, (s, t) => t)
				.Where(c => c.CanBeUpdated)
				.ToList();

			return columnsToBeSet
				.Select(c => String.Format(CultureInfo.InvariantCulture, "{0} = source.{0}", c.ToSelectListString()))
				.ToArray()
				.Convert(a => String.Join(", ", a));
		}

		private string GetValuesList()
		{
			return _sourceTableSchema.Columns
				.Join(_targetTableSchema.Columns, c => c.Name, c => c.Name, (s, t) => t)
				.Where(c => c.CanBeInserted)
				.ToSelectListString();
		}

	}
}