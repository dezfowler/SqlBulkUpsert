using System;
using System.Data;

namespace SqlBulkUpsert
{
	public class TextColumn : Column
	{
		public int? CharLength { get; set; }
		public int? ByteLength { get; set; }

		public override bool Equals(Column other)
		{
			var textOther = other as TextColumn;
			if (textOther == null) return false;
			return
				 base.Equals(other) &&
				 CharLength == textOther.CharLength &&
				 ByteLength == textOther.ByteLength;
		}

		public override Column Clone()
		{
			return CopyTo(new TextColumn());
		}

		public override Column CopyTo(Column column)
		{
			var textColumn = (TextColumn)column;
			textColumn.CharLength = CharLength;
			textColumn.ByteLength = ByteLength;
			return base.CopyTo(textColumn);
		}

		public override string ToFullDataTypeString()
		{
			switch (DataType)
			{
				case "char":
				case "varchar":
				case "nchar":
				case "nvarchar":
					return String.Format("{0}({1})", DataType, HandleMax(CharLength));

				case "binary":
				case "varbinary":
					return String.Format("{0}({1})", DataType, HandleMax(ByteLength));

				default:
					return base.ToFullDataTypeString();
			}
		}

		private static string HandleMax(int? val)
		{
			if (!val.HasValue) throw new Exception("Expected column length");
			var value = val.Value;
			return value == -1 ? "max" : value.ToString();
		}

		protected override void Populate(IDataReader sqlDataReader)
		{
			base.Populate(sqlDataReader);
			CharLength = sqlDataReader.GetTypedValue<int?>("CHARACTER_MAXIMUM_LENGTH");
			ByteLength = sqlDataReader.GetTypedValue<int?>("CHARACTER_OCTET_LENGTH");
		}
	}
}