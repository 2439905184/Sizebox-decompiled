using MoonSharp.Interpreter;
using UnityEngine;

public class LuaListener : IListener
{
	public DynValue instance;

	public DynValue function;

	public void OnNotify(IEvent e)
	{
		try
		{
			function.Function.Call(instance, e.GetLuaData());
		}
		catch (ScriptRuntimeException ex)
		{
			Debug.LogError(ex.DecoratedMessage);
		}
	}
}
