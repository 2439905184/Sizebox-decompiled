using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class _003C_003Ef__AnonymousType4<_003C_format_003Ej__TPar>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003C_format_003Ej__TPar _003C_format_003Ei__Field;

	public _003C_format_003Ej__TPar _format
	{
		get
		{
			return _003C_format_003Ei__Field;
		}
	}

	[DebuggerHidden]
	public _003C_003Ef__AnonymousType4(_003C_format_003Ej__TPar _format)
	{
		_003C_format_003Ei__Field = _format;
	}

	[DebuggerHidden]
	public override bool Equals(object value)
	{
		_003C_003Ef__AnonymousType4<_003C_format_003Ej__TPar> anon = value as _003C_003Ef__AnonymousType4<_003C_format_003Ej__TPar>;
		if (anon != null)
		{
			return EqualityComparer<_003C_format_003Ej__TPar>.Default.Equals(_003C_format_003Ei__Field, anon._003C_format_003Ei__Field);
		}
		return false;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return -1992492600 * -1521134295 + EqualityComparer<_003C_format_003Ej__TPar>.Default.GetHashCode(_003C_format_003Ei__Field);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		object[] array = new object[1];
		_003C_format_003Ej__TPar val = _003C_format_003Ei__Field;
		ref _003C_format_003Ej__TPar reference = ref val;
		_003C_format_003Ej__TPar val2 = default(_003C_format_003Ej__TPar);
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
		return string.Format(null, "{{ _format = {0} }}", array);
	}
}
