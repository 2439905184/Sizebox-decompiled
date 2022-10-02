using MoonSharp.Interpreter;
using UnityEngine;

public class LuaCallBatch : LuaInterval
{
	private int _luaIntervalIterate;

	private static int _luaBatchSize = 2;

	protected override void Start()
	{
		base.Start();
		_luaBatchSize = GlobalPreferences.LazyBatch.value;
	}

	protected void LuaCallFunc()
	{
		int num = ((_luaIntervalIterate + _luaBatchSize >= luaInterval.Count) ? (luaInterval.Count - _luaIntervalIterate) : _luaBatchSize);
		for (int i = 0; i < num; i++)
		{
			if (luaInterval.Count < _luaIntervalIterate)
			{
				_luaIntervalIterate = 0;
				break;
			}
			IntervalSt intervalSt = luaInterval[_luaIntervalIterate];
			if (intervalSt.instance == null || intervalSt.Func == null)
			{
				luaInterval.RemoveAt(_luaIntervalIterate);
				break;
			}
			try
			{
				intervalSt.instance.Script.Call(intervalSt.Func, intervalSt.Table);
			}
			catch (ScriptRuntimeException ex)
			{
				Debug.LogError(ex.DecoratedMessage);
				intervalSt.instance.Exit(true);
				break;
			}
			_luaIntervalIterate++;
		}
	}
}
