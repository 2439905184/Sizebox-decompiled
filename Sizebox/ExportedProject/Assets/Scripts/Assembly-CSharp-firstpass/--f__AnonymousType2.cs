using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class _003C_003Ef__AnonymousType2<_003Creason_003Ej__TPar, _003CthreadId_003Ej__TPar>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003Creason_003Ej__TPar _003Creason_003Ei__Field;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003CthreadId_003Ej__TPar _003CthreadId_003Ei__Field;

	public _003Creason_003Ej__TPar reason
	{
		get
		{
			return _003Creason_003Ei__Field;
		}
	}

	public _003CthreadId_003Ej__TPar threadId
	{
		get
		{
			return _003CthreadId_003Ei__Field;
		}
	}

	[DebuggerHidden]
	public _003C_003Ef__AnonymousType2(_003Creason_003Ej__TPar reason, _003CthreadId_003Ej__TPar threadId)
	{
		_003Creason_003Ei__Field = reason;
		_003CthreadId_003Ei__Field = threadId;
	}

	[DebuggerHidden]
	public override bool Equals(object value)
	{
		_003C_003Ef__AnonymousType2<_003Creason_003Ej__TPar, _003CthreadId_003Ej__TPar> anon = value as _003C_003Ef__AnonymousType2<_003Creason_003Ej__TPar, _003CthreadId_003Ej__TPar>;
		if (anon != null && EqualityComparer<_003Creason_003Ej__TPar>.Default.Equals(_003Creason_003Ei__Field, anon._003Creason_003Ei__Field))
		{
			return EqualityComparer<_003CthreadId_003Ej__TPar>.Default.Equals(_003CthreadId_003Ei__Field, anon._003CthreadId_003Ei__Field);
		}
		return false;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (-1941369189 * -1521134295 + EqualityComparer<_003Creason_003Ej__TPar>.Default.GetHashCode(_003Creason_003Ei__Field)) * -1521134295 + EqualityComparer<_003CthreadId_003Ej__TPar>.Default.GetHashCode(_003CthreadId_003Ei__Field);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		object[] array = new object[2];
		_003Creason_003Ej__TPar val = _003Creason_003Ei__Field;
		ref _003Creason_003Ej__TPar reference = ref val;
		_003Creason_003Ej__TPar val2 = default(_003Creason_003Ej__TPar);
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
		_003CthreadId_003Ej__TPar val3 = _003CthreadId_003Ei__Field;
		ref _003CthreadId_003Ej__TPar reference2 = ref val3;
		_003CthreadId_003Ej__TPar val4 = default(_003CthreadId_003Ej__TPar);
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
		return string.Format(null, "{{ reason = {0}, threadId = {1} }}", array);
	}
}
