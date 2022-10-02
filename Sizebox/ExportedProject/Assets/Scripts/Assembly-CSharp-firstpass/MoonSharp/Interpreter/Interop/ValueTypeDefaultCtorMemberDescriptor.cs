using System;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop
{
	public class ValueTypeDefaultCtorMemberDescriptor : IOverloadableMemberDescriptor, IMemberDescriptor, IWireableDescriptor
	{
		public bool IsStatic
		{
			get
			{
				return true;
			}
		}

		public string Name { get; private set; }

		public Type ValueTypeDefaultCtor { get; private set; }

		public ParameterDescriptor[] Parameters { get; private set; }

		public Type ExtensionMethodType
		{
			get
			{
				return null;
			}
		}

		public Type VarArgsArrayType
		{
			get
			{
				return null;
			}
		}

		public Type VarArgsElementType
		{
			get
			{
				return null;
			}
		}

		public string SortDiscriminant
		{
			get
			{
				return "@.ctor";
			}
		}

		public MemberDescriptorAccess MemberAccess
		{
			get
			{
				return MemberDescriptorAccess.CanRead | MemberDescriptorAccess.CanExecute;
			}
		}

		public ValueTypeDefaultCtorMemberDescriptor(Type valueType)
		{
			if (!Framework.Do.IsValueType(valueType))
			{
				throw new ArgumentException("valueType is not a value type");
			}
			Name = "__new";
			Parameters = new ParameterDescriptor[0];
			ValueTypeDefaultCtor = valueType;
		}

		public DynValue Execute(Script script, object obj, ScriptExecutionContext context, CallbackArguments args)
		{
			this.CheckAccess(MemberDescriptorAccess.CanRead, obj);
			object obj2 = Activator.CreateInstance(ValueTypeDefaultCtor);
			return ClrToScriptConversions.ObjectToDynValue(script, obj2);
		}

		public DynValue GetValue(Script script, object obj)
		{
			this.CheckAccess(MemberDescriptorAccess.CanRead, obj);
			object obj2 = Activator.CreateInstance(ValueTypeDefaultCtor);
			return ClrToScriptConversions.ObjectToDynValue(script, obj2);
		}

		public void SetValue(Script script, object obj, DynValue value)
		{
			this.CheckAccess(MemberDescriptorAccess.CanWrite, obj);
		}

		public void PrepareForWiring(Table t)
		{
			t.Set("class", DynValue.NewString(GetType().FullName));
			t.Set("type", DynValue.NewString(ValueTypeDefaultCtor.FullName));
			t.Set("name", DynValue.NewString(Name));
		}
	}
}
