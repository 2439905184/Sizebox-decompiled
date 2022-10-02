using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class _003C_003Ef__AnonymousType9<_003CAttributes_003Ej__TPar, _003CDataType_003Ej__TPar>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003CAttributes_003Ej__TPar _003CAttributes_003Ei__Field;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003CDataType_003Ej__TPar _003CDataType_003Ei__Field;

	public _003CAttributes_003Ej__TPar Attributes
	{
		get
		{
			return _003CAttributes_003Ei__Field;
		}
	}

	public _003CDataType_003Ej__TPar DataType
	{
		get
		{
			return _003CDataType_003Ei__Field;
		}
	}

	[DebuggerHidden]
	public _003C_003Ef__AnonymousType9(_003CAttributes_003Ej__TPar Attributes, _003CDataType_003Ej__TPar DataType)
	{
		_003CAttributes_003Ei__Field = Attributes;
		_003CDataType_003Ei__Field = DataType;
	}

	[DebuggerHidden]
	public override bool Equals(object value)
	{
		_003C_003Ef__AnonymousType9<_003CAttributes_003Ej__TPar, _003CDataType_003Ej__TPar> anon = value as _003C_003Ef__AnonymousType9<_003CAttributes_003Ej__TPar, _003CDataType_003Ej__TPar>;
		if (anon != null && EqualityComparer<_003CAttributes_003Ej__TPar>.Default.Equals(_003CAttributes_003Ei__Field, anon._003CAttributes_003Ei__Field))
		{
			return EqualityComparer<_003CDataType_003Ej__TPar>.Default.Equals(_003CDataType_003Ei__Field, anon._003CDataType_003Ei__Field);
		}
		return false;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (-1924030281 * -1521134295 + EqualityComparer<_003CAttributes_003Ej__TPar>.Default.GetHashCode(_003CAttributes_003Ei__Field)) * -1521134295 + EqualityComparer<_003CDataType_003Ej__TPar>.Default.GetHashCode(_003CDataType_003Ei__Field);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		object[] array = new object[2];
		_003CAttributes_003Ej__TPar val = _003CAttributes_003Ei__Field;
		ref _003CAttributes_003Ej__TPar reference = ref val;
		_003CAttributes_003Ej__TPar val2 = default(_003CAttributes_003Ej__TPar);
		object obj;
		if (val2 == null)
		{
			val2 = reference;
			reference = ref val2;
			if (val2 == null)
			{
				obj = null;
				goto IL_0046;
			}
		}
		obj = reference.ToString();
		goto IL_0046;
		IL_0046:
		array[0] = obj;
		_003CDataType_003Ej__TPar val3 = _003CDataType_003Ei__Field;
		ref _003CDataType_003Ej__TPar reference2 = ref val3;
		_003CDataType_003Ej__TPar val4 = default(_003CDataType_003Ej__TPar);
		object obj2;
		if (val4 == null)
		{
			val4 = reference2;
			reference2 = ref val4;
			if (val4 == null)
			{
				obj2 = null;
				goto IL_0081;
			}
		}
		obj2 = reference2.ToString();
		goto IL_0081;
		IL_0081:
		array[1] = obj2;
		return string.Format(null, "{{ Attributes = {0}, DataType = {1} }}", array);
	}
}
