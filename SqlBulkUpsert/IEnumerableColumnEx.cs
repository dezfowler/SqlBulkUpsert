using System.Collections.Generic;
using System.Linq;

namespace SqlBulkUpsert
{
    public static class IEnumerableColumnEx
    {
        public static string ToSelectListString(this IEnumerable<Column> columns)
        {
            return string.Join(", ", columns.Select(c => c.ToSelectListString()).ToArray());
        }

        public static string ToColumnDefinitionListString(this IEnumerable<Column> columns)
        {
            return string.Join(", ", columns.Select(c => c.ToColumnDefinitionString()).ToArray());
        }
    }
}