using System;
using System.Globalization;

namespace SqlBulkUpsert
{
	public class IdentityColumn : Column
	{
		public IdentityColumn()
		{
			Name = "_Surrogate";
			DataType = "int";
			Nullable = false;
			CanBeInserted = false;
			CanBeUpdated = false;
		}

		public override string ToColumnDefinitionString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{0}{1}", base.ToColumnDefinitionString(), " IDENTITY(0, 1)" );
		}
	}
}