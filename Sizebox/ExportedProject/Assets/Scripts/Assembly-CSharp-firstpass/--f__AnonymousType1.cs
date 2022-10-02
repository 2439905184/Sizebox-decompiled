using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class _003C_003Ef__AnonymousType1<_003CexitCode_003Ej__TPar>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003CexitCode_003Ej__TPar _003CexitCode_003Ei__Field;

	public _003CexitCode_003Ej__TPar exitCode
	{
		get
		{
			return _003CexitCode_003Ei__Field;
		}
	}

	[DebuggerHidden]
	public _003C_003Ef__AnonymousType1(_003CexitCode_003Ej__TPar exitCode)
	{
		_003CexitCode_003Ei__Field = exitCode;
	}

	[DebuggerHidden]
	public override bool Equals(object value)
	{
		_003C_003Ef__AnonymousType1<_003CexitCode_003Ej__TPar> anon = value as _003C_003Ef__AnonymousType1<_003CexitCode_003Ej__TPar>;
		if (anon != null)
		{
			return EqualityComparer<_003CexitCode_003Ej__TPar>.Default.Equals(_003CexitCode_003Ei__Field, anon._003CexitCode_003Ei__Field);
		}
		return false;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return -165364249 * -1521134295 + EqualityComparer<_003CexitCode_003Ej__TPar>.Default.GetHashCode(_003CexitCode_003Ei__Field);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		object[] array = new object[1];
		_003CexitCode_003Ej__TPar val = _003CexitCode_003Ei__Field;
		ref _003CexitCode_003Ej__TPar reference = ref val;
		_003CexitCode_003Ej__TPar val2 = default(_003CexitCode_003Ej__TPar);
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
		return string.Format(null, "{{ exitCode = {0} }}", array);
	}
}
