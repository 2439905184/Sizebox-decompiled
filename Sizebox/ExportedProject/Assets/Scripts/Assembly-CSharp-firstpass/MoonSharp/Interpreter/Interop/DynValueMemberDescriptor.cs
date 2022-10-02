using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace MoonSharp.Interpreter.Interop
{
	public class DynValueMemberDescriptor : IMemberDescriptor, IWireableDescriptor
	{
		private DynValue m_Value;

		public bool IsStatic
		{
			get
			{
				return true;
			}
		}

		public string Name { get; private set; }

		public MemberDescriptorAccess MemberAccess { get; private set; }

		public virtual DynValue Value
		{
			get
			{
				return m_Value;
			}
		}

		protected DynValueMemberDescriptor(string name, string serializedTableValue)
		{
			DynValue dynValue = new Script().CreateDynamicExpression(serializedTableValue).Evaluate();
			m_Value = dynValue.Table.Get(1);
			Name = name;
			MemberAccess = MemberDescriptorAccess.CanRead;
		}

		protected DynValueMemberDescriptor(string name)
		{
			MemberAccess = MemberDescriptorAccess.CanRead;
			m_Value = null;
			Name = name;
		}

		public DynValueMemberDescriptor(string name, DynValue value)
		{
			m_Value = value;
			Name = name;
			if (value.Type == DataType.ClrFunction)
			{
				MemberAccess = MemberDescriptorAccess.CanRead | MemberDescriptorAccess.CanExecute;
			}
			else
			{
				MemberAccess = MemberDescriptorAccess.CanRead;
			}
		}

		public DynValue GetValue(Script script, object obj)
		{
			return Value;
		}

		public void SetValue(Script script, object obj, DynValue value)
		{
			throw new ScriptRuntimeException("userdata '{0}' cannot be written to.", Name);
		}

		public void PrepareForWiring(Table t)
		{
			t.Set("class", DynValue.NewString(GetType().FullName));
			t.Set("name", DynValue.NewString(Name));
			switch (Value.Type)
			{
			case DataType.Nil:
			case DataType.Void:
			case DataType.Boolean:
			case DataType.Number:
			case DataType.String:
			case DataType.Tuple:
				t.Set("value", Value);
				break;
			case DataType.Table:
				if (Value.Table.OwnerScript == null)
				{
					t.Set("value", Value);
				}
				else
				{
					t.Set("error", DynValue.NewString("Wiring of non-prime table value members not supported."));
				}
				break;
			case DataType.UserData:
				if (Value.UserData.Object == null)
				{
					t.Set("type", DynValue.NewString("userdata"));
					t.Set("staticType", DynValue.NewString(Value.UserData.Descriptor.Type.FullName));
					t.Set("visibility", DynValue.NewString(Value.UserData.Descriptor.Type.GetClrVisibility()));
				}
				else
				{
					t.Set("error", DynValue.NewString("Wiring of non-static userdata value members not supported."));
				}
				break;
			default:
				t.Set("error", DynValue.NewString(string.Format("Wiring of '{0}' value members not supported.", Value.Type.ToErrorTypeString())));
				break;
			}
		}
	}
}
