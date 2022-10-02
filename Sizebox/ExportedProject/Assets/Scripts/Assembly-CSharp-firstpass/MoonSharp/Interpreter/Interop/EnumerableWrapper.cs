using System.Collections;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop
{
	internal class EnumerableWrapper : IUserDataType
	{
		private IEnumerator m_Enumerator;

		private Script m_Script;

		private DynValue m_Prev = DynValue.Nil;

		private bool m_HasTurnOnce;

		private EnumerableWrapper(Script script, IEnumerator enumerator)
		{
			m_Script = script;
			m_Enumerator = enumerator;
		}

		public void Reset()
		{
			if (m_HasTurnOnce)
			{
				m_Enumerator.Reset();
			}
			m_HasTurnOnce = true;
		}

		private DynValue GetNext(DynValue prev)
		{
			if (prev.IsNil())
			{
				Reset();
			}
			while (m_Enumerator.MoveNext())
			{
				DynValue dynValue = ClrToScriptConversions.ObjectToDynValue(m_Script, m_Enumerator.Current);
				if (!dynValue.IsNil())
				{
					return dynValue;
				}
			}
			return DynValue.Nil;
		}

		private DynValue LuaIteratorCallback(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			m_Prev = GetNext(m_Prev);
			return m_Prev;
		}

		internal static DynValue ConvertIterator(Script script, IEnumerator enumerator)
		{
			EnumerableWrapper o = new EnumerableWrapper(script, enumerator);
			return DynValue.NewTuple(UserData.Create(o), DynValue.Nil, DynValue.Nil);
		}

		internal static DynValue ConvertTable(Table table)
		{
			return ConvertIterator(table.OwnerScript, table.Values.GetEnumerator());
		}

		public DynValue Index(Script script, DynValue index, bool isDirectIndexing)
		{
			if (index.Type == DataType.String)
			{
				switch (index.String)
				{
				case "Current":
				case "current":
					return DynValue.FromObject(script, m_Enumerator.Current);
				case "MoveNext":
				case "moveNext":
				case "move_next":
					return DynValue.NewCallback(_003CIndex_003Eb__10_0);
				case "Reset":
				case "reset":
					return DynValue.NewCallback(_003CIndex_003Eb__10_1);
				}
			}
			return null;
		}

		public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
		{
			return false;
		}

		public DynValue MetaIndex(Script script, string metaname)
		{
			if (metaname == "__call")
			{
				return DynValue.NewCallback(LuaIteratorCallback);
			}
			return null;
		}

		[CompilerGenerated]
		private DynValue _003CIndex_003Eb__10_0(ScriptExecutionContext ctx, CallbackArguments args)
		{
			return DynValue.NewBoolean(m_Enumerator.MoveNext());
		}

		[CompilerGenerated]
		private DynValue _003CIndex_003Eb__10_1(ScriptExecutionContext ctx, CallbackArguments args)
		{
			Reset();
			return DynValue.Nil;
		}
	}
}
