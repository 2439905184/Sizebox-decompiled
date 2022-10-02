using System.Collections.Generic;
using MoonSharp.Interpreter.DataStructs;

namespace MoonSharp.Interpreter
{
	public class CallbackArguments
	{
		private IList<DynValue> m_Args;

		private int m_Count;

		private bool m_LastIsTuple;

		public int Count
		{
			get
			{
				return m_Count;
			}
		}

		public bool IsMethodCall { get; private set; }

		public DynValue this[int index]
		{
			get
			{
				return RawGet(index, true) ?? DynValue.Void;
			}
		}

		public CallbackArguments(IList<DynValue> args, bool isMethodCall)
		{
			m_Args = args;
			if (m_Args.Count > 0)
			{
				DynValue dynValue = m_Args[m_Args.Count - 1];
				if (dynValue.Type == DataType.Tuple)
				{
					m_Count = dynValue.Tuple.Length - 1 + m_Args.Count;
					m_LastIsTuple = true;
				}
				else if (dynValue.Type == DataType.Void)
				{
					m_Count = m_Args.Count - 1;
				}
				else
				{
					m_Count = m_Args.Count;
				}
			}
			else
			{
				m_Count = 0;
			}
			IsMethodCall = isMethodCall;
		}

		public DynValue RawGet(int index, bool translateVoids)
		{
			if (index >= m_Count)
			{
				return null;
			}
			DynValue dynValue = ((m_LastIsTuple && index >= m_Args.Count - 1) ? m_Args[m_Args.Count - 1].Tuple[index - (m_Args.Count - 1)] : m_Args[index]);
			if (dynValue.Type == DataType.Tuple)
			{
				dynValue = ((dynValue.Tuple.Length == 0) ? DynValue.Nil : dynValue.Tuple[0]);
			}
			if (translateVoids && dynValue.Type == DataType.Void)
			{
				dynValue = DynValue.Nil;
			}
			return dynValue;
		}

		public DynValue[] GetArray(int skip = 0)
		{
			if (skip >= m_Count)
			{
				return new DynValue[0];
			}
			DynValue[] array = new DynValue[m_Count - skip];
			for (int i = skip; i < m_Count; i++)
			{
				array[i - skip] = this[i];
			}
			return array;
		}

		public DynValue AsType(int argNum, string funcName, DataType type, bool allowNil = false)
		{
			return this[argNum].CheckType(funcName, type, argNum, allowNil ? (TypeValidationFlags.AllowNil | TypeValidationFlags.AutoConvert) : TypeValidationFlags.AutoConvert);
		}

		public T AsUserData<T>(int argNum, string funcName, bool allowNil = false)
		{
			return this[argNum].CheckUserDataType<T>(funcName, argNum, allowNil ? TypeValidationFlags.AllowNil : TypeValidationFlags.None);
		}

		public int AsInt(int argNum, string funcName)
		{
			return (int)AsType(argNum, funcName, DataType.Number).Number;
		}

		public long AsLong(int argNum, string funcName)
		{
			return (long)AsType(argNum, funcName, DataType.Number).Number;
		}

		public string AsStringUsingMeta(ScriptExecutionContext executionContext, int argNum, string funcName)
		{
			if (this[argNum].Type == DataType.Table && this[argNum].Table.MetaTable != null && this[argNum].Table.MetaTable.RawGet("__tostring") != null)
			{
				DynValue dynValue = executionContext.GetScript().Call(this[argNum].Table.MetaTable.RawGet("__tostring"), this[argNum]);
				if (dynValue.Type != DataType.String)
				{
					throw new ScriptRuntimeException("'tostring' must return a string to '{0}'", funcName);
				}
				return dynValue.ToPrintString();
			}
			return this[argNum].ToPrintString();
		}

		public CallbackArguments SkipMethodCall()
		{
			if (IsMethodCall)
			{
				return new CallbackArguments(new Slice<DynValue>(m_Args, 1, m_Args.Count - 1, false), false);
			}
			return this;
		}
	}
}
