using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SqlBulkUpsert
{
    [Serializable]
    public class SqlBulkUpsertException : Exception
    {
        public SqlBulkUpsertException() 
        {
        }

        public SqlBulkUpsertException(string message) : base(message)
        {
        }

        public SqlBulkUpsertException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SqlBulkUpsertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
