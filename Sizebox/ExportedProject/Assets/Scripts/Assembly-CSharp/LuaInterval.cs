using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter;
using UnityEngine;

public class LuaInterval : MonoBehaviour
{
	[Serializable]
	public struct IntervalSt
	{
		public LuaBehaviorInstance instance;

		public DynValue Func;

		public Table Table;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public LuaBehaviorInstance script;

		public DynValue func;

		internal bool _003CDelInterval_003Eb__0(IntervalSt x)
		{
			if (x.instance == script)
			{
				return x.Func == func;
			}
			return false;
		}
	}

	public List<IntervalSt> luaInterval;

	public void AddInterval(LuaBehaviorInstance script, DynValue func)
	{
		IntervalSt intervalSt = default(IntervalSt);
		intervalSt.instance = script;
		intervalSt.Func = func;
		intervalSt.Table = script.Instance;
		IntervalSt item = intervalSt;
		luaInterval.Add(item);
		script.referenceCount++;
		if (!base.enabled)
		{
			base.enabled = true;
		}
	}

	public void DelInterval(LuaBehaviorInstance script, DynValue func)
	{
		_003C_003Ec__DisplayClass3_0 _003C_003Ec__DisplayClass3_ = new _003C_003Ec__DisplayClass3_0();
		_003C_003Ec__DisplayClass3_.script = script;
		_003C_003Ec__DisplayClass3_.func = func;
		try
		{
			IntervalSt item = luaInterval.Single(_003C_003Ec__DisplayClass3_._003CDelInterval_003Eb__0);
			luaInterval.Remove(item);
			_003C_003Ec__DisplayClass3_.script.referenceCount--;
		}
		catch (InvalidOperationException)
		{
			Debug.LogWarning("Lua interval removal attempted on function that doesn't exist");
		}
		IsEmpty();
	}

	protected virtual void Start()
	{
		luaInterval = new List<IntervalSt>();
	}

	protected bool IsEmpty()
	{
		if (luaInterval.Count == 0)
		{
			base.enabled = false;
			return true;
		}
		return false;
	}
}
