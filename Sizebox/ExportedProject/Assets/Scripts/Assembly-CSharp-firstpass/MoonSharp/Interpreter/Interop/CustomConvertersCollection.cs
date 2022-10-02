using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MoonSharp.Interpreter.Interop
{
	public class CustomConvertersCollection
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass6_0<T>
		{
			public Func<Script, T, DynValue> converter;

			internal DynValue _003CSetClrToScriptCustomConversion_003Eb__0(Script s, object o)
			{
				return converter(s, (T)o);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass8_0
		{
			public Func<object, DynValue> converter;

			internal DynValue _003CSetClrToScriptCustomConversion_003Eb__0(Script s, object o)
			{
				return converter(o);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass9_0<T>
		{
			public Func<T, DynValue> converter;

			internal DynValue _003CSetClrToScriptCustomConversion_003Eb__0(object o)
			{
				return converter((T)o);
			}
		}

		private Dictionary<Type, Func<DynValue, object>>[] m_Script2Clr = new Dictionary<Type, Func<DynValue, object>>[11];

		private Dictionary<Type, Func<Script, object, DynValue>> m_Clr2Script = new Dictionary<Type, Func<Script, object, DynValue>>();

		internal CustomConvertersCollection()
		{
			for (int i = 0; i < m_Script2Clr.Length; i++)
			{
				m_Script2Clr[i] = new Dictionary<Type, Func<DynValue, object>>();
			}
		}

		public void SetScriptToClrCustomConversion(DataType scriptDataType, Type clrDataType, Func<DynValue, object> converter = null)
		{
			if ((int)scriptDataType > m_Script2Clr.Length)
			{
				throw new ArgumentException("scriptDataType");
			}
			Dictionary<Type, Func<DynValue, object>> dictionary = m_Script2Clr[(int)scriptDataType];
			if (converter == null)
			{
				if (dictionary.ContainsKey(clrDataType))
				{
					dictionary.Remove(clrDataType);
				}
			}
			else
			{
				dictionary[clrDataType] = converter;
			}
		}

		public Func<DynValue, object> GetScriptToClrCustomConversion(DataType scriptDataType, Type clrDataType)
		{
			if ((int)scriptDataType > m_Script2Clr.Length)
			{
				return null;
			}
			return m_Script2Clr[(int)scriptDataType].GetOrDefault(clrDataType);
		}

		public void SetClrToScriptCustomConversion(Type clrDataType, Func<Script, object, DynValue> converter = null)
		{
			if (converter == null)
			{
				if (m_Clr2Script.ContainsKey(clrDataType))
				{
					m_Clr2Script.Remove(clrDataType);
				}
			}
			else
			{
				m_Clr2Script[clrDataType] = converter;
			}
		}

		public void SetClrToScriptCustomConversion<T>(Func<Script, T, DynValue> converter = null)
		{
			_003C_003Ec__DisplayClass6_0<T> _003C_003Ec__DisplayClass6_ = new _003C_003Ec__DisplayClass6_0<T>();
			_003C_003Ec__DisplayClass6_.converter = converter;
			SetClrToScriptCustomConversion(typeof(T), _003C_003Ec__DisplayClass6_._003CSetClrToScriptCustomConversion_003Eb__0);
		}

		public Func<Script, object, DynValue> GetClrToScriptCustomConversion(Type clrDataType)
		{
			return m_Clr2Script.GetOrDefault(clrDataType);
		}

		[Obsolete("This method is deprecated. Use the overloads accepting functions with a Script argument.")]
		public void SetClrToScriptCustomConversion(Type clrDataType, Func<object, DynValue> converter = null)
		{
			_003C_003Ec__DisplayClass8_0 _003C_003Ec__DisplayClass8_ = new _003C_003Ec__DisplayClass8_0();
			_003C_003Ec__DisplayClass8_.converter = converter;
			SetClrToScriptCustomConversion(clrDataType, _003C_003Ec__DisplayClass8_._003CSetClrToScriptCustomConversion_003Eb__0);
		}

		[Obsolete("This method is deprecated. Use the overloads accepting functions with a Script argument.")]
		public void SetClrToScriptCustomConversion<T>(Func<T, DynValue> converter = null)
		{
			_003C_003Ec__DisplayClass9_0<T> _003C_003Ec__DisplayClass9_ = new _003C_003Ec__DisplayClass9_0<T>();
			_003C_003Ec__DisplayClass9_.converter = converter;
			SetClrToScriptCustomConversion(typeof(T), _003C_003Ec__DisplayClass9_._003CSetClrToScriptCustomConversion_003Eb__0);
		}

		public void Clear()
		{
			m_Clr2Script.Clear();
			for (int i = 0; i < m_Script2Clr.Length; i++)
			{
				m_Script2Clr[i].Clear();
			}
		}
	}
}
