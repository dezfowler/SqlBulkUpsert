using System;

namespace SqlBulkUpsert
{
    public static class GeneralExtensions
    {
        public static TResult Convert<T, TResult>(this T source, Converter<T, TResult> converter)
        {
            if (null == converter) throw new ArgumentNullException("converter");
            return converter(source);
        }
    }
}
