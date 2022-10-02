using MoonSharp.Interpreter;

public class AISMGHitEvent : IEvent
{
	public EntityBase shooter;

	public EntityBase target;

	public AISMGHitEvent(EntityBase shooter, EntityBase target)
	{
		this.shooter = shooter;
		this.target = target;
		code = EventCode.OnAIRaygunHit;
	}

	public override DynValue GetLuaData()
	{
		if (data == null)
		{
			data = DynValue.NewPrimeTable();
			data.Table.Set("shooter", DynValue.FromObject(null, shooter.GetLuaEntity()));
			data.Table.Set("target", DynValue.FromObject(null, target ? target.GetLuaEntity() : null));
		}
		return data;
	}
}
