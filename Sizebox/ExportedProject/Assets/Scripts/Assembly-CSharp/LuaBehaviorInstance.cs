using System;
using MoonSharp.Interpreter;
using UnityEngine;

[Serializable]
public class LuaBehaviorInstance : IBehaviorInstance
{
	public string behaviorFilename;

	public string behaviorName;

	public Table Instance;

	public Script Script;

	private ushort _refCount;

	private DynValue _start;

	private DynValue _update;

	private DynValue _lazyUpdate;

	private DynValue _fixedUpdate;

	private DynValue _lazyFixedUpdate;

	private DynValue _exit;

	public ushort referenceCount
	{
		get
		{
			return _refCount;
		}
		set
		{
			if (value == _refCount + 1)
			{
				_refCount = value;
			}
			else if (_refCount > 0 && value == _refCount - 1)
			{
				_refCount = value;
				if (autoFinish)
				{
					Exit();
				}
			}
		}
	}

	private bool autoFinish
	{
		get
		{
			return _refCount == 0;
		}
	}

	public LuaBehaviorInstance(Table i, Script s)
	{
		Instance = i;
		Script = s;
		_start = GetMethod("Start" + 3u);
		if (object.Equals(_start, DynValue.Nil))
		{
			_start = GetMethod("Start");
		}
		_update = GetMethod("Update" + 3u);
		if (object.Equals(_update, DynValue.Nil))
		{
			_update = GetMethod("Update");
		}
		_fixedUpdate = GetMethod("FixedUpdate" + 3u);
		if (object.Equals(_fixedUpdate, DynValue.Nil))
		{
			_fixedUpdate = GetMethod("FixedUpdate");
		}
		_lazyUpdate = GetMethod("LazyUpdate" + 3u);
		if (object.Equals(_lazyUpdate, DynValue.Nil))
		{
			_lazyUpdate = GetMethod("LazyUpdate");
		}
		_lazyFixedUpdate = GetMethod("LazyFixedUpdate" + 3u);
		if (object.Equals(_lazyFixedUpdate, DynValue.Nil))
		{
			_lazyFixedUpdate = GetMethod("LazyFixedUpdate");
		}
		_exit = GetMethod("Exit" + 3u);
		if (object.Equals(_exit, DynValue.Nil))
		{
			_exit = GetMethod("Exit");
		}
	}

	private DynValue GetMethod(string methodName)
	{
		DynValue dynValue = Instance.Get(methodName);
		if (dynValue.IsNil())
		{
			dynValue = Instance.MetaTable.Get(methodName);
			if (dynValue.IsNil())
			{
				dynValue = Instance.MetaTable.Get("__index").Table.MetaTable.Get(methodName);
			}
		}
		return dynValue;
	}

	public void Start()
	{
		if (!_update.IsNil())
		{
			LuaUpdate.instance.AddInterval(this, _update);
		}
		if (!_fixedUpdate.IsNil())
		{
			LuaFixedUpdate.instance.AddInterval(this, _fixedUpdate);
		}
		if (!_lazyUpdate.IsNil())
		{
			LuaLazyUpdate.instance.AddInterval(this, _lazyUpdate);
		}
		if (!_lazyFixedUpdate.IsNil())
		{
			LuaLazyFixedUpdate.instance.AddInterval(this, _lazyFixedUpdate);
		}
		RunMethod(_start);
	}

	public void Exit(bool abort = false)
	{
		if (abort)
		{
			_exit = DynValue.Nil;
		}
		else
		{
			RunMethod(_exit);
		}
		Abort();
	}

	private void Abort()
	{
		if (referenceCount > 0)
		{
			referenceCount++;
		}
		if (!_update.IsNil() && (bool)LuaUpdate.instance)
		{
			LuaUpdate.instance.DelInterval(this, _update);
		}
		if (!_fixedUpdate.IsNil() && (bool)LuaFixedUpdate.instance)
		{
			LuaFixedUpdate.instance.DelInterval(this, _fixedUpdate);
		}
		if (!_lazyUpdate.IsNil() && (bool)LuaLazyUpdate.instance)
		{
			LuaLazyUpdate.instance.DelInterval(this, _lazyUpdate);
		}
		if (!_lazyFixedUpdate.IsNil() && (bool)LuaLazyFixedUpdate.instance)
		{
			LuaLazyFixedUpdate.instance.DelInterval(this, _lazyFixedUpdate);
		}
	}

	public bool AutoFinish()
	{
		return autoFinish;
	}

	private bool RunMethod(DynValue method, bool abort = true)
	{
		if (method.IsNil())
		{
			return true;
		}
		try
		{
			Script.Call(method, Instance);
		}
		catch (ScriptRuntimeException ex)
		{
			Debug.LogError(ex.DecoratedMessage);
			if (abort)
			{
				Exit(true);
			}
			return false;
		}
		return true;
	}
}
