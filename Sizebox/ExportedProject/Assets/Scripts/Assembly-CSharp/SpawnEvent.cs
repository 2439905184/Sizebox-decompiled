using MoonSharp.Interpreter;

public class SpawnEvent : IEvent
{
	public EntityBase entity;

	public SpawnEvent(EntityBase entity)
	{
		this.entity = entity;
		code = EventCode.OnSpawn;
	}

	public override DynValue GetLuaData()
	{
		if (data == null)
		{
			data = DynValue.NewPrimeTable();
			data.Table.Set("entity", DynValue.FromObject(null, entity.GetLuaEntity()));
		}
		return data;
	}
}
