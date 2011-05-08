using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using SqlBulkUpsert.Test.Properties;

namespace SqlBulkUpsert.Test
{
	[TestFixture]
	public class DataTableUpserterTests : DatabaseTestsBase
	{
		[Test]
		public void EndToEndInsert()
		{
			// Arrange
			using (var connection = DatabaseHelper.CreateAndOpenConnection())
			{
				var targetSchema = SqlTableSchema.LoadFromDatabase(connection, "TestUpsert", "ident");

				var upserter = new DataTableUpserter(targetSchema);
				upserter.RetrieveIdentity = true;

				var dataTable = new DataTable("TestUpsert");
				dataTable.Columns.Add("ident");
				dataTable.Columns.Add("key_part_1");
				dataTable.Columns.Add("key_part_2");
				dataTable.Columns.Add("nullable_text");
				dataTable.Columns.Add("nullable_number");

				for (int i = 1; i <= 10; i++)
				{
					dataTable.Rows.Add(DBNull.Value, "TEST", i, String.Format("some text here {0}", i), i * 11);
				}

				// Act
				IEnumerable<int> result = upserter.Upsert(connection, dataTable);
				var idents = new List<int>(result);

				// Assert
				Assert.AreEqual(10, idents.Count);

				foreach (DataRow row in dataTable.Rows)
				{
					Assert.AreEqual(row["key_part_2"], row["ident"]);
				}
			}
		}

		[Test]
		public void EndToEndInsertUpdate()
		{
			// Arrange
			using (var connection = DatabaseHelper.CreateAndOpenConnection())
			{
				connection.ExecuteCommands(Resources.PopulateTable);

				var targetSchema = SqlTableSchema.LoadFromDatabase(connection, "TestUpsert", "ident");

				var upserter = new DataTableUpserter(targetSchema);
				upserter.RetrieveIdentity = true;

				var dataTable = new DataTable("TestUpsert");
				dataTable.Columns.Add("ident");
				dataTable.Columns.Add("key_part_1");
				dataTable.Columns.Add("key_part_2");
				dataTable.Columns.Add("nullable_text");
				dataTable.Columns.Add("nullable_number");

				for (int i = 1; i <= 10; i++)
				{
					dataTable.Rows.Add(i, "TEST", i, String.Format("some text here {0}", i), i);
					dataTable.Rows.Add(DBNull.Value, "BLAH", i, String.Format("some text here {0}", i), i + 10);
				}

				// Act
				IEnumerable<int> result = upserter.Upsert(connection, dataTable);
				var idents = new List<int>(result);

				// Assert
				Assert.AreEqual(10, idents.Count);

				foreach (DataRow row in dataTable.Rows)
				{
					Assert.AreEqual(row["nullable_number"], row["ident"]);
				}

				Assert.AreEqual(20, connection.ExecuteScalar("SELECT COUNT(1) FROM [TestUpsert]"));
				Assert.AreEqual(10, connection.ExecuteScalar("SELECT COUNT(1) FROM [TestUpsert] WHERE [key_part_1] != 'BLAH'"));
				Assert.AreEqual(10, connection.ExecuteScalar("SELECT COUNT(1) FROM [TestUpsert] WHERE [key_part_1] = 'BLAH'"));
			}
		}
	}
}
