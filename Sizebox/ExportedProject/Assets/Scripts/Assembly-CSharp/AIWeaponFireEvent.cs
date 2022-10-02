using MoonSharp.Interpreter;

public class AIWeaponFireEvent : IEvent
{
	public EntityBase entity;

	public AIWeaponFireEvent(EntityBase entity)
	{
		this.entity = entity;
		code = EventCode.OnAIWeaponFire;
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
