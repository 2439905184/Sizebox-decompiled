using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct TypeTable
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<InternedString, string> _003C_003E9__2_0;

			internal string _003Cget_names_003Eb__2_0(InternedString x)
			{
				return x.ToString();
			}
		}

		public Dictionary<InternedString, Type> table;

		public IEnumerable<string> names
		{
			get
			{
				return table.Keys.Select(_003C_003Ec._003C_003E9__2_0 ?? (_003C_003Ec._003C_003E9__2_0 = _003C_003Ec._003C_003E9._003Cget_names_003Eb__2_0));
			}
		}

		public IEnumerable<InternedString> internedNames
		{
			get
			{
				return table.Keys;
			}
		}

		public void Initialize()
		{
			table = new Dictionary<InternedString, Type>();
		}

		public InternedString FindNameForType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			foreach (KeyValuePair<InternedString, Type> item in table)
			{
				if (item.Value == type)
				{
					return item.Key;
				}
			}
			return default(InternedString);
		}

		public void AddTypeRegistration(string name, Type type)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name cannot be null or empty", "name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			InternedString key = new InternedString(name);
			table[key] = type;
		}

		public Type LookupTypeRegistration(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name cannot be null or empty", "name");
			}
			InternedString key = new InternedString(name);
			Type value;
			if (table.TryGetValue(key, out value))
			{
				return value;
			}
			return null;
		}
	}
}
