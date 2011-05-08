using System;
using System.Data;

namespace SqlBulkUpsert
{
    public static class IDataReaderEx
    {
        public static T GetTypedValue<T>(this IDataReader reader, string columnName)
        {
            if (columnName == null) throw new ArgumentNullException("columnName");
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal)) return default(T);
            return (T)reader.GetValue(ordinal);
        }
    }
}