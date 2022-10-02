using MoonSharp.Interpreter;

namespace Lua
{
	public class EntityInitializedCallback
	{
		private DynValue script;

		private DynValue callback;

		public EntityInitializedCallback(DynValue script, DynValue callback)
		{
			this.script = script;
			this.callback = callback;
		}

		public void Call(EntityBase entity)
		{
			callback.Function.Call(script, entity.GetLuaEntity());
		}
	}
}
