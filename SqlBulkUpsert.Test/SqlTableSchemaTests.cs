using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;

namespace SqlBulkUpsert.Test
{
 [TestFixture]
 public class SqlTableSchemaTests : DatabaseTestsBase
 {
	 [Test]
	 [ExpectedException(ExpectedMessage = "Table not found")]
	 public void RetrieveTableSchemaNotExist()
	 {
		 using (var connection = DatabaseHelper.CreateAndOpenConnection())
		 {
			 // Arrange

			 // Act
			 SqlTableSchema.LoadFromDatabase(connection, "DoesNotExist", null);

			 // Assert
		 }
	 }

	 [Test]
	 public void PopulateFromReader()
	 {
		 // Arrange
		 var columnDetail = new DataTable();
		 columnDetail.Columns.AddRange(new List<DataColumn>
									   {
										   new DataColumn
										   {
											   ColumnName = "COLUMN_NAME",
											   DataType = typeof(string),
										   },
										   new DataColumn
										   {
											   ColumnName = "ORDINAL_POSITION",
											   DataType = typeof(int),
										   },
										   new DataColumn
										   {
											   ColumnName = "IS_NULLABLE",
											   DataType = typeof(string),
										   },
										   new DataColumn
										   {
											   ColumnName = "DATA_TYPE",
											   DataType = typeof(string),
										   },
										   new DataColumn
										   {
											   ColumnName = "CHARACTER_MAXIMUM_LENGTH",
											   DataType = typeof(int),
											   AllowDBNull = true,
										   },
										   new DataColumn
										   {
											   ColumnName = "CHARACTER_OCTET_LENGTH",
											   DataType = typeof(int),
											   AllowDBNull = true,
										   },
										   new DataColumn
										   {
											   ColumnName = "NUMERIC_PRECISION",
											   DataType = typeof(byte),
											   AllowDBNull = true,
										   },
										   new DataColumn
										   {
											   ColumnName = "NUMERIC_PRECISION_RADIX",
											   DataType = typeof(short),
											   AllowDBNull = true,
										   },
										   new DataColumn
										   {
											   ColumnName = "NUMERIC_SCALE",
											   DataType = typeof(int),
											   AllowDBNull = true,
										   },
										   new DataColumn
										   {
											   ColumnName = "DATETIME_PRECISION",
											   DataType = typeof(short),
											   AllowDBNull = true,
										   },
									   }.ToArray());

		 columnDetail.Rows.Add("ident", 1, "NO", "int", DBNull.Value, DBNull.Value, (byte)10, (short)10, 0, DBNull.Value);
		 columnDetail.Rows.Add("key_part_1", 2, "NO", "nchar", 4, 8, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
		 columnDetail.Rows.Add("key_part_2", 3, "NO", "smallint", DBNull.Value, DBNull.Value, (byte)5, (short)10, 0, DBNull.Value);
		 columnDetail.Rows.Add("nullable_text", 4, "YES", "nvarchar", 50, 100, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
		 columnDetail.Rows.Add("nullable_number", 5, "YES", "int", DBNull.Value, DBNull.Value, (byte)10, (short)10, 0, DBNull.Value);
		 columnDetail.Rows.Add("nullable_datetimeoffset", 6, "YES", "datetimeoffset", DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, (short)7);
		 columnDetail.Rows.Add("nullable_money", 7, "YES", "money", DBNull.Value, DBNull.Value, (byte)19, (short)10, 4, DBNull.Value);
		 columnDetail.Rows.Add("nullable_varbinary", 8, "YES", "varbinary", -1, -1, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
		 columnDetail.Rows.Add("nullable_image", 9, "YES", "image", 2147483647, 2147483647, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
		 columnDetail.Rows.Add("nullable_xml", 10, "YES", "xml", -1, -1, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);

		 var keyDetail = new DataTable();
		 keyDetail.Columns.AddRange(new List<DataColumn>
									{
										new DataColumn
										{
											ColumnName = "COLUMN_NAME",
											DataType = typeof (string),
										},
									}.ToArray());

		 keyDetail.Rows.Add("key_part_1");
		 keyDetail.Rows.Add("key_part_2");

		 var dataTableReader = new DataTableReader(new[] { columnDetail, keyDetail });

		 #region Columns
		 var expectedColumns = new List<Column>
								   {
									   new NumericColumn
									   {
										   Name = "ident",
										   OrdinalPosition = 1,
										   Nullable = false,
										   DataType = "int",
										   Precision = 10,
										   Radix = 10,
										   Scale = 0,
									   },
									   new TextColumn
									   {
										   Name = "key_part_1",
										   OrdinalPosition = 2,
										   Nullable = false,
										   DataType = "nchar",
										   CharLength = 4,
										   ByteLength = 8,
									   },
									   new NumericColumn
									   {
										   Name = "key_part_2",
										   OrdinalPosition = 3,
										   Nullable = false,
										   DataType = "smallint",
										   Precision = 5,
										   Radix = 10,
										   Scale = 0,
									   },
									   new TextColumn
									   {
										   Name = "nullable_text",
										   OrdinalPosition = 4,
										   Nullable = true,
										   DataType = "nvarchar",
										   CharLength = 50,
										   ByteLength = 100,
									   },
									   new NumericColumn
									   {
										   Name = "nullable_number",
										   OrdinalPosition = 5,
										   Nullable = true,
										   DataType = "int",
										   Precision = 10,
										   Radix = 10,
										   Scale = 0,
									   },
									   new DateColumn
									   {
										   Name = "nullable_datetimeoffset",
										   OrdinalPosition = 6,
										   Nullable = true,
										   DataType = "datetimeoffset",
										   Precision = 7,
									   },
									   new NumericColumn
									   {
										   Name = "nullable_money",
										   OrdinalPosition = 7,
										   Nullable = true,
										   DataType = "money",
										   Precision = 19,
										   Radix = 10,
										   Scale = 4,
									   },
									   new TextColumn
									   {
										   Name = "nullable_varbinary",
										   OrdinalPosition = 8,
										   Nullable = true,
										   DataType = "varbinary",
										   CharLength = -1,
										   ByteLength = -1,
									   },
									   new TextColumn
									   {
										   Name = "nullable_image",
										   OrdinalPosition = 9,
										   Nullable = true,
										   DataType = "image",
										   CharLength = 2147483647,
										   ByteLength = 2147483647,
									   },
									   new Column
									   {
										   Name = "nullable_xml",
										   OrdinalPosition = 10,
										   Nullable = true,
										   DataType = "xml",
									   },
								   };

		 var expectedKeyColumns = new List<Column>
									  {
										  new TextColumn
										  {
											  Name = "key_part_1",
											  OrdinalPosition = 2,
											  Nullable = false,
											  DataType = "nchar",
											  CharLength = 4,
											  ByteLength = 8,
										  },
										  new NumericColumn
										  {
											  Name = "key_part_2",
											  OrdinalPosition = 3,
											  Nullable = false,
											  DataType = "smallint",
											  Precision = 5,
											  Radix = 10,
											  Scale = 0,
										  },
									  };
		 #endregion

		 // Act
		 var schema = SqlTableSchema.LoadFromReader("TestUpsert", dataTableReader, "ident");

		 // Assert 
		 Assert.AreEqual("TestUpsert", schema.TableName);
		 CollectionAssert.AreEqual(expectedColumns, schema.Columns, new ColumnComparer());
		 CollectionAssert.AreEqual(expectedKeyColumns, schema.PrimaryKeyColumns, new ColumnComparer());
	 }

	 [Test]
	 public void RetrieveTableSchema()
	 {
		 using (var connection = DatabaseHelper.CreateAndOpenConnection())
		 {
			 // Arrange
			 #region Columns
			 var expectedColumns = new List<Column>
								   {
									   new NumericColumn
									   {
										   Name = "ident",
										   OrdinalPosition = 1,
										   Nullable = false,
										   DataType = "int",
										   Precision = 10,
										   Radix = 10,
										   Scale = 0,
									   },
									   new TextColumn
									   {
										   Name = "key_part_1",
										   OrdinalPosition = 2,
										   Nullable = false,
										   DataType = "nchar",
										   CharLength = 4,
										   ByteLength = 8,
									   },
									   new NumericColumn
									   {
										   Name = "key_part_2",
										   OrdinalPosition = 3,
										   Nullable = false,
										   DataType = "smallint",
										   Precision = 5,
										   Radix = 10,
										   Scale = 0,
									   },
									   new TextColumn
									   {
										   Name = "nullable_text",
										   OrdinalPosition = 4,
										   Nullable = true,
										   DataType = "nvarchar",
										   CharLength = 50,
										   ByteLength = 100,
									   },
									   new NumericColumn
									   {
										   Name = "nullable_number",
										   OrdinalPosition = 5,
										   Nullable = true,
										   DataType = "int",
										   Precision = 10,
										   Radix = 10,
										   Scale = 0,
									   },
									   new DateColumn
									   {
										   Name = "nullable_datetimeoffset",
										   OrdinalPosition = 6,
										   Nullable = true,
										   DataType = "datetimeoffset",
										   Precision = 7,
									   },
									   new NumericColumn
									   {
										   Name = "nullable_money",
										   OrdinalPosition = 7,
										   Nullable = true,
										   DataType = "money",
										   Precision = 19,
										   Radix = 10,
										   Scale = 4,
									   },
									   new TextColumn
									   {
										   Name = "nullable_varbinary",
										   OrdinalPosition = 8,
										   Nullable = true,
										   DataType = "varbinary",
										   CharLength = -1,
										   ByteLength = -1,
									   },
									   new TextColumn
									   {
										   Name = "nullable_image",
										   OrdinalPosition = 9,
										   Nullable = true,
										   DataType = "image",
										   CharLength = 2147483647,
										   ByteLength = 2147483647,
									   },
									   new Column
									   {
										   Name = "nullable_xml",
										   OrdinalPosition = 10,
										   Nullable = true,
										   DataType = "xml",
									   },
								   };

			 var expectedKeyColumns = new List<Column>
									  {
										  new TextColumn
										  {
											  Name = "key_part_1",
											  OrdinalPosition = 2,
											  Nullable = false,
											  DataType = "nchar",
											  CharLength = 4,
											  ByteLength = 8,
										  },
										  new NumericColumn
										  {
											  Name = "key_part_2",
											  OrdinalPosition = 3,
											  Nullable = false,
											  DataType = "smallint",
											  Precision = 5,
											  Radix = 10,
											  Scale = 0,
										  },
									  };
			 #endregion

			 // Act
			 SqlTableSchema schema = SqlTableSchema.LoadFromDatabase(connection, "TestUpsert", "ident");

			 // Assert
			 Assert.AreEqual("TestUpsert", schema.TableName);
			 CollectionAssert.AreEqual(expectedColumns, schema.Columns, new ColumnComparer());
			 CollectionAssert.AreEqual(expectedKeyColumns, schema.PrimaryKeyColumns, new ColumnComparer());
		 }
	 }

	 [Test]
	 public void CheckCreateTableCommand()
	 {
		 // Arrange
		 var schema = new SqlTableSchema("TestUpsert", new List<Column>
													   {
														   new NumericColumn
														   {
															   Name = "first",
															   DataType = "int",
															   OrdinalPosition = 1,
															   Nullable = false,
														   },
														   new TextColumn
														   {
															   Name = "second",
															   DataType = "ntext",
															   OrdinalPosition = 2,
															   Nullable = true,
														   },
														   new DateColumn
														   {
															   Name = "third",
															   DataType = "datetime2",
															   OrdinalPosition = 3,
															   Precision = 4,
														   }
													   });

		 // Act
		 var cmdText = schema.ToCreateTableCommandText();

		 // Assert
		 Assert.AreEqual("CREATE TABLE [TestUpsert] ([first] int NOT NULL, [second] ntext NULL, [third] datetime2(4) NOT NULL)", cmdText);
	 }

	 [Test]
	 public void CheckDropTableCommand()
	 {
		 // Arrange
		 var schema = new SqlTableSchema("TestUpsert");

		 // Act
		 var cmdText = schema.ToDropTableCommandText();

		 // Assert
		 Assert.AreEqual("DROP TABLE [TestUpsert]", cmdText);
	 }

 }
}
