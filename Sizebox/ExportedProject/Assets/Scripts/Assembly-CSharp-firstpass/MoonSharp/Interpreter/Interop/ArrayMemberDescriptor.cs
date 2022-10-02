using System;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop
{
	public class ArrayMemberDescriptor : ObjectCallbackMemberDescriptor, IWireableDescriptor
	{
		private bool m_IsSetter;

		public ArrayMemberDescriptor(string name, bool isSetter, ParameterDescriptor[] indexerParams)
			: base(name, isSetter ? new Func<object, ScriptExecutionContext, CallbackArguments, object>(ArrayIndexerSet) : new Func<object, ScriptExecutionContext, CallbackArguments, object>(ArrayIndexerGet), indexerParams)
		{
			m_IsSetter = isSetter;
		}

		public ArrayMemberDescriptor(string name, bool isSetter)
			: base(name, isSetter ? new Func<object, ScriptExecutionContext, CallbackArguments, object>(ArrayIndexerSet) : new Func<object, ScriptExecutionContext, CallbackArguments, object>(ArrayIndexerGet))
		{
			m_IsSetter = isSetter;
		}

		public void PrepareForWiring(Table t)
		{
			t.Set("class", DynValue.NewString(GetType().FullName));
			t.Set("name", DynValue.NewString(base.Name));
			t.Set("setter", DynValue.NewBoolean(m_IsSetter));
			if (base.Parameters != null)
			{
				DynValue dynValue = DynValue.NewPrimeTable();
				t.Set("params", dynValue);
				int num = 0;
				ParameterDescriptor[] parameters = base.Parameters;
				foreach (ParameterDescriptor obj in parameters)
				{
					DynValue dynValue2 = DynValue.NewPrimeTable();
					dynValue.Table.Set(++num, dynValue2);
					obj.PrepareForWiring(dynValue2.Table);
				}
			}
		}

		private static int[] BuildArrayIndices(CallbackArguments args, int count)
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = args.AsInt(i, "userdata_array_indexer");
			}
			return array;
		}

		private static object ArrayIndexerSet(object arrayObj, ScriptExecutionContext ctx, CallbackArguments args)
		{
			Array array = (Array)arrayObj;
			int[] indices = BuildArrayIndices(args, args.Count - 1);
			DynValue value = args[args.Count - 1];
			Type elementType = array.GetType().GetElementType();
			object value2 = ScriptToClrConversions.DynValueToObjectOfType(value, elementType, null, false);
			array.SetValue(value2, indices);
			return DynValue.Void;
		}

		private static object ArrayIndexerGet(object arrayObj, ScriptExecutionContext ctx, CallbackArguments args)
		{
			Array obj = (Array)arrayObj;
			int[] indices = BuildArrayIndices(args, args.Count);
			return obj.GetValue(indices);
		}
	}
}
