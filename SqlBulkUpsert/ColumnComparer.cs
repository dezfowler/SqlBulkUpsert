using System.Collections;
using System.Collections.Generic;

namespace SqlBulkUpsert
{
    public class ColumnComparer : EqualityComparer<Column>, IComparer
    {
        public int Compare(object x, object y)
        {
            return ((Column)x).Equals((Column)y) ? 0 : -1;
        }

        public override bool Equals(Column x, Column y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(Column obj)
        {
            return 0;
        }
    }
}