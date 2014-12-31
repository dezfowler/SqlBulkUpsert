using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SqlBulkUpsert.Test
{
	[TestFixture]
	public class TypedUpserterTests : DatabaseTestsBase
	{
		[Test]
		public void EndToEnd()
		{
			using (var connection = DatabaseHelper.CreateAndOpenConnection())
			{
				// Arrange
				var targetSchema = SqlTableSchema.LoadFromDatabase(connection, "TestUpsert", "ident");

				var columnMappings = new Dictionary<string, Func<TestDto, object>>
				                    	{
				                    		{"ident", d => d.Ident},
				                    		{"key_part_1", d => d.KeyPart1},
				                    		{"key_part_2", d => d.KeyPart2},
				                    		{"nullable_text", d => d.Text},
				                    		{"nullable_number", d => d.Number},
				                    		{"nullable_datetimeoffset", d => d.Date},
				                    	};

				Action<TestDto, int> identUpdater = (d, i) => d.Ident = i;
            
				var upserter = new TypedUpserter<TestDto>(targetSchema, columnMappings, identUpdater);

				var items = new List<TestDto>();

				for (int i = 1; i <= 10; i++)
				{
					items.Add(
						new TestDto
							{
								Ident = null,
								KeyPart1 = "TEST",
								KeyPart2 = (short)i,
								Text = String.Format("some text here {0}", i),
								Number = i,
								Date = new DateTimeOffset(new DateTime(2010, 11, 14, 12, 0, 0), TimeSpan.FromHours(i))
							});
				}

				// Act
				upserter.Upsert(connection, items);

				// Assert
				foreach (var testDto in items)
				{
					Assert.AreEqual(testDto.Number, testDto.Ident);
				}
			}
		}

		public class TestDto
		{
			public int? Ident { get; set; }
			public string KeyPart1 { get; set; }
			public short KeyPart2 { get; set; }
			public string Text { get; set; }
			public int Number { get; set; }
			public DateTimeOffset Date { get; set; }
		}
	}
}