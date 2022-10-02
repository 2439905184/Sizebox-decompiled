using Lua;
using MoonSharp.Interpreter;
using UnityEngine;

public class StepEvent : IEvent
{
	public EntityBase entity;

	public UnityEngine.Vector3 position;

	public float magnitude;

	public int foot;

	public StepEvent(EntityBase entity, UnityEngine.Vector3 position, float magnitude, int foot)
	{
		this.entity = entity;
		this.position = position;
		this.magnitude = magnitude;
		this.foot = foot;
		code = EventCode.OnStep;
	}

	public override DynValue GetLuaData()
	{
		if (data == null)
		{
			data = DynValue.NewPrimeTable();
			data.Table.Set("gts", DynValue.FromObject(null, entity.GetLuaEntity()));
			data.Table.Set("entity", DynValue.FromObject(null, entity.GetLuaEntity()));
			data.Table.Set("position", DynValue.FromObject(null, new Lua.Vector3(position)));
			data.Table.Set("magnitude", DynValue.FromObject(null, magnitude));
			data.Table.Set("foot", DynValue.FromObject(null, foot));
		}
		return data;
	}
}
