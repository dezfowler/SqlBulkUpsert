using System;

namespace SqlBulkUpsert
{
    public static class GeneralExtensions
    {
        public static TResult Convert<T, TResult>(this T source, Converter<T, TResult> converter)
        {
            return converter(source);
        }
    }
}
