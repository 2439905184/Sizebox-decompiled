using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class _003C_003Ef__AnonymousType3<_003Ccategory_003Ej__TPar, _003Coutput_003Ej__TPar>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003Ccategory_003Ej__TPar _003Ccategory_003Ei__Field;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003Coutput_003Ej__TPar _003Coutput_003Ei__Field;

	public _003Ccategory_003Ej__TPar category
	{
		get
		{
			return _003Ccategory_003Ei__Field;
		}
	}

	public _003Coutput_003Ej__TPar output
	{
		get
		{
			return _003Coutput_003Ei__Field;
		}
	}

	[DebuggerHidden]
	public _003C_003Ef__AnonymousType3(_003Ccategory_003Ej__TPar category, _003Coutput_003Ej__TPar output)
	{
		_003Ccategory_003Ei__Field = category;
		_003Coutput_003Ei__Field = output;
	}

	[DebuggerHidden]
	public override bool Equals(object value)
	{
		_003C_003Ef__AnonymousType3<_003Ccategory_003Ej__TPar, _003Coutput_003Ej__TPar> anon = value as _003C_003Ef__AnonymousType3<_003Ccategory_003Ej__TPar, _003Coutput_003Ej__TPar>;
		if (anon != null && EqualityComparer<_003Ccategory_003Ej__TPar>.Default.Equals(_003Ccategory_003Ei__Field, anon._003Ccategory_003Ei__Field))
		{
			return EqualityComparer<_003Coutput_003Ej__TPar>.Default.Equals(_003Coutput_003Ei__Field, anon._003Coutput_003Ei__Field);
		}
		return false;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (-398429101 * -1521134295 + EqualityComparer<_003Ccategory_003Ej__TPar>.Default.GetHashCode(_003Ccategory_003Ei__Field)) * -1521134295 + EqualityComparer<_003Coutput_003Ej__TPar>.Default.GetHashCode(_003Coutput_003Ei__Field);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		object[] array = new object[2];
		_003Ccategory_003Ej__TPar val = _003Ccategory_003Ei__Field;
		ref _003Ccategory_003Ej__TPar reference = ref val;
		_003Ccategory_003Ej__TPar val2 = default(_003Ccategory_003Ej__TPar);
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
		_003Coutput_003Ej__TPar val3 = _003Coutput_003Ei__Field;
		ref _003Coutput_003Ej__TPar reference2 = ref val3;
		_003Coutput_003Ej__TPar val4 = default(_003Coutput_003Ej__TPar);
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
		return string.Format(null, "{{ category = {0}, output = {1} }}", array);
	}
}
