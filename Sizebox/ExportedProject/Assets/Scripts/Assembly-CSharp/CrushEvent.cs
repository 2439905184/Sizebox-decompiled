using MoonSharp.Interpreter;

public class CrushEvent : IEvent
{
	public EntityBase victim;

	public EntityBase crusher;

	public CrushEvent(EntityBase victim, EntityBase crusher)
	{
		this.crusher = crusher;
		this.victim = victim;
		code = EventCode.OnCrush;
	}

	public override DynValue GetLuaData()
	{
		if (data == null)
		{
			data = DynValue.NewPrimeTable();
			data.Table.Set("victim", DynValue.FromObject(null, victim.GetLuaEntity()));
			data.Table.Set("crusher", DynValue.FromObject(null, crusher ? crusher.GetLuaEntity() : null));
		}
		return data;
	}
}
