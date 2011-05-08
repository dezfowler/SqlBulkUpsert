using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SqlBulkUpsert
{
	/// <summary>
	/// Upserts to a target table from an enumerable of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TypedUpserter<T> : UpserterBase
	{
		private readonly Dictionary<string, Func<T, object>> _columnMappings;
		private readonly Action<T, int> _identUpdater;

		public TypedUpserter(SqlTableSchema targetTableSchema, Dictionary<string, Func<T, object>> columnMappings, Action<T,int> identUpdater)
			: base(targetTableSchema)
		{
			if (columnMappings == null) throw new ArgumentNullException("columnMappings");
			_columnMappings = columnMappings;
			_identUpdater = identUpdater;
		}

		public void Upsert(SqlConnection connection, IEnumerable<T> items)
		{
			var itemList = new List<T>(items);
			var inserts = PerformUpsert(connection, _columnMappings.Keys, new TypedDataReader(_columnMappings, items));
			if(_identUpdater != null)
			{
				foreach (var insert in inserts)
				{
					var item = itemList[insert.Key];
					_identUpdater(item, insert.Value);
				}
			}
		}

		class TypedDataReader : IDataReader
		{
			private readonly IEnumerator<T> _items;
			private readonly Dictionary<string, int> _mappingLookup;
			private readonly Func<T, object>[] _mappingFuncs;

			public TypedDataReader(Dictionary<string, Func<T, object>> columnMappings, IEnumerable<T> items)
			{
				if (columnMappings == null) throw new ArgumentNullException("columnMappings");
				if (items == null) throw new ArgumentNullException("items");

				_items = items.GetEnumerator();

				_mappingLookup = columnMappings.Keys
					.Select((s, i) => new { Key = s, Value = i })
					.ToDictionary(x => x.Key, x => x.Value);

				_mappingFuncs = columnMappings.Values.ToArray();
			}

			public void Dispose()
			{

			}

			public object GetValue(int i)
			{
				return _mappingFuncs[i](_items.Current);
			}

			public int GetOrdinal(string name)
			{
				return _mappingLookup[name];
			}

			public int FieldCount
			{
				get { return _mappingFuncs.Length; }
			}

			public bool Read()
			{
				return _items.MoveNext();
			}

			#region Not implemented - satisfying interface only
			public string GetName(int i)
			{
				throw new NotImplementedException();
			}

			public string GetDataTypeName(int i)
			{
				throw new NotImplementedException();
			}

			public Type GetFieldType(int i)
			{
				throw new NotImplementedException();
			}

			public int GetValues(object[] values)
			{
				throw new NotImplementedException();
			}

			public bool GetBoolean(int i)
			{
				throw new NotImplementedException();
			}

			public byte GetByte(int i)
			{
				throw new NotImplementedException();
			}

			public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
			{
				throw new NotImplementedException();
			}

			public char GetChar(int i)
			{
				throw new NotImplementedException();
			}

			public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
			{
				throw new NotImplementedException();
			}

			public Guid GetGuid(int i)
			{
				throw new NotImplementedException();
			}

			public short GetInt16(int i)
			{
				throw new NotImplementedException();
			}

			public int GetInt32(int i)
			{
				throw new NotImplementedException();
			}

			public long GetInt64(int i)
			{
				throw new NotImplementedException();
			}

			public float GetFloat(int i)
			{
				throw new NotImplementedException();
			}

			public double GetDouble(int i)
			{
				throw new NotImplementedException();
			}

			public string GetString(int i)
			{
				throw new NotImplementedException();
			}

			public decimal GetDecimal(int i)
			{
				throw new NotImplementedException();
			}

			public DateTime GetDateTime(int i)
			{
				throw new NotImplementedException();
			}

			public IDataReader GetData(int i)
			{
				throw new NotImplementedException();
			}

			public bool IsDBNull(int i)
			{
				throw new NotImplementedException();
			}

			object IDataRecord.this[int i]
			{
				get { throw new NotImplementedException(); }
			}

			object IDataRecord.this[string name]
			{
				get { throw new NotImplementedException(); }
			}

			public void Close()
			{
				throw new NotImplementedException();
			}

			public DataTable GetSchemaTable()
			{
				throw new NotImplementedException();
			}

			public bool NextResult()
			{
				throw new NotImplementedException();
			}

			public int Depth
			{
				get { throw new NotImplementedException(); }
			}

			public bool IsClosed
			{
				get { throw new NotImplementedException(); }
			}

			public int RecordsAffected
			{
				get { throw new NotImplementedException(); }
			}
			#endregion
		}
	}
	
}