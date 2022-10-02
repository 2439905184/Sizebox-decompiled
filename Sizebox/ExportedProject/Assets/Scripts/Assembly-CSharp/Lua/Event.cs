using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public static class Event
	{
		public static void Send(string code, DynValue data)
		{
			EventManager.SendEvent(new IEvent
			{
				code = code,
				data = data
			});
		}

		public static Listener Register(DynValue instance, string code, DynValue function)
		{
			if (function.Type != DataType.Function)
			{
				Debug.LogError("To Listen for events you must provide a function: Event.Listen(event, function)");
				return null;
			}
			return EventManager.Register(new LuaListener
			{
				instance = instance,
				function = function
			}, code);
		}

		public static void Unregister(Listener listener)
		{
			EventManager.Unregister(listener);
		}
	}
}
