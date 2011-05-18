using System;
using System.Data;

namespace SqlBulkUpsert
{
    public static class IDataReaderEx
    {
        public static T GetTypedValue<T>(this IDataRecord reader, string columnName)
        {
            if (null == reader) throw new ArgumentNullException("reader");
            if (null == columnName) throw new ArgumentNullException("columnName");
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal)) return default(T);
            return (T)reader.GetValue(ordinal);
        }
    }
}