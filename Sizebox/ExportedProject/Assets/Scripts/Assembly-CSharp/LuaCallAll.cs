using MoonSharp.Interpreter;
using UnityEngine;

public class LuaCallAll : LuaInterval
{
	protected void LuaCallFunc()
	{
		if (IsEmpty())
		{
			return;
		}
		int num = 0;
		while (num < luaInterval.Count)
		{
			IntervalSt item = luaInterval[num];
			if (item.instance == null || item.Func == null)
			{
				luaInterval.Remove(item);
				if (IsEmpty())
				{
					break;
				}
				continue;
			}
			try
			{
				item.instance.Script.Call(item.Func, item.Table);
			}
			catch (ScriptRuntimeException ex)
			{
				Debug.LogError(ex.DecoratedMessage);
				item.instance.Exit(true);
				if (IsEmpty())
				{
					break;
				}
				continue;
			}
			num++;
		}
	}
}
