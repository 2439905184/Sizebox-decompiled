using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class ErrorCollector
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Converter<Exception, string> _003C_003E9__6_0;

		internal string _003CGetErrorMessages_003Eb__6_0(Exception e)
		{
			return e.ToString();
		}
	}

	private List<Exception> _exceptions = new List<Exception>();

	public int Count
	{
		get
		{
			return _exceptions.Count;
		}
	}

	public void Add(Exception e)
	{
		_exceptions.Add(e);
	}

	public void Clear()
	{
		_exceptions.Clear();
	}

	public void GoBoom()
	{
		if (_exceptions.Count > 0)
		{
			throw new Exception(string.Join("  \n", GetErrorMessages()));
		}
	}

	public List<string> GetErrorMessages()
	{
		return _exceptions.ConvertAll(_003C_003Ec._003C_003E9__6_0 ?? (_003C_003Ec._003C_003E9__6_0 = _003C_003Ec._003C_003E9._003CGetErrorMessages_003Eb__6_0));
	}
}
