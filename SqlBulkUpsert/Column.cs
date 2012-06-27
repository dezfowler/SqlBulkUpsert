using System;
using System.Data;
using System.Globalization;

namespace SqlBulkUpsert
{
	public class Column : IEquatable<Column>
	{
		public Column()
		{
			CanBeInserted = true;
			CanBeUpdated = true;
		}

		public string Name { get; set; }
		
		public int OrdinalPosition { get; set; }
		
		public bool Nullable { get; set; }
		
		public string DataType { get; set; }
		
		public bool CanBeInserted { get; set; }
		
		public bool CanBeUpdated { get; set; }

		public virtual Column Clone()
		{
			return CopyTo(new Column());
		}

		public virtual Column CopyTo(Column column)
		{
			if (null == column) throw new ArgumentNullException("column");

			column.Name = Name;
			column.OrdinalPosition = OrdinalPosition;
			column.Nullable = Nullable;
			column.DataType = DataType;
			column.CanBeInserted = CanBeInserted;
			column.CanBeUpdated = CanBeUpdated;

			return column;
		}

		public virtual bool Equals(Column other)
		{
			if (null == other) return false;
			
			return
				 Name == other.Name &&
				 OrdinalPosition == other.OrdinalPosition &&
				 Nullable == other.Nullable &&
				 DataType == other.DataType;
		}

		public string ToSelectListString()
		{
			return String.Format(CultureInfo.InvariantCulture, "[{0}]", Name);
		}

		public virtual string ToColumnDefinitionString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", ToSelectListString(), ToFullDataTypeString(), Nullable ? "NULL" : "NOT NULL");
		}

		public virtual string ToFullDataTypeString()
		{
			return DataType;
		}

		public static Column CreateFromReader(IDataReader sqlDataReader)
		{
			if (null == sqlDataReader) throw new ArgumentNullException("sqlDataReader");

			var dataType = (string)sqlDataReader["DATA_TYPE"];
			Column column;
			switch (dataType)
			{
				case "bigint":
				case "numeric":
				case "bit":
				case "smallint":
				case "decimal":
				case "smallmoney":
				case "int":
				case "tinyint":
				case "money":
				case "float":
				case "real":
					column = new NumericColumn();
					break;
				case "date":
				case "datetimeoffset":
				case "datetime2":
				case "smalldatetime":
				case "datetime":
				case "time":
					column = new DateColumn();
					break;
				case "char":
				case "varchar":
				case "text":
				case "nchar":
				case "nvarchar":
				case "ntext":
				case "binary":
				case "varbinary":
				case "image":
					column = new TextColumn();
					break;
				default:
					column = new Column();
					break;
			}

			column.Populate(sqlDataReader);
			return column;
		}

		protected virtual void Populate(IDataReader sqlDataReader)
		{
			if (null == sqlDataReader) throw new ArgumentNullException("sqlDataReader");

			Name = (string)sqlDataReader["COLUMN_NAME"];
			OrdinalPosition = (int)sqlDataReader["ORDINAL_POSITION"];
			Nullable = ((string)sqlDataReader["IS_NULLABLE"]) == "YES";
			DataType = (string)sqlDataReader["DATA_TYPE"];
		}
	}
}