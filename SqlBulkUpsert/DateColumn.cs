using System;
using System.Data;
using System.Globalization;

namespace SqlBulkUpsert
{
	public class DateColumn : Column
	{
		public int? Precision { get; set; }

		public override bool Equals(Column other)
		{
			var dateColumn = other as DateColumn;
			if (dateColumn == null) return false;
			return
				 base.Equals(other) &&
				 Precision == dateColumn.Precision;
		}

		public override Column Clone()
		{
			return CopyTo(new DateColumn());
		}

		public override Column CopyTo(Column column)
		{
			var dateColumn = (DateColumn)column;
			dateColumn.Precision = Precision;
			return base.CopyTo(dateColumn);
		}

		protected override void Populate(IDataReader sqlDataReader)
		{
			base.Populate(sqlDataReader);
			Precision = sqlDataReader.GetTypedValue<short?>("DATETIME_PRECISION");
		}

		public override string ToFullDataTypeString()
		{
			switch (DataType)
			{
				case "datetimeoffset":
				case "datetime2":
					return String.Format(CultureInfo.InvariantCulture, "{0}({1})", DataType, Precision);
			}
			return base.ToFullDataTypeString();
		}
	}
}