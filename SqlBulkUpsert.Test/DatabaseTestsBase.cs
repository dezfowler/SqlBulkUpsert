using NUnit.Framework;

namespace SqlBulkUpsert.Test
{
	public class DatabaseTestsBase
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			DatabaseHelper.RefreshSchema();
		}

		[TearDown]
		public void TearDown()
		{
			DatabaseHelper.ExecuteCommands("TRUNCATE TABLE [TestUpsert]");
		}
	}
}