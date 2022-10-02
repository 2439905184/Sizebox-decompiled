using MoonSharp.Interpreter;

public class PlayerRaygunHitEvent : IEvent
{
	public EntityBase target;

	public float magnitude;

	public int firingMode;

	public float chargeValue;

	public PlayerRaygunHitEvent(EntityBase target, float magnitude, int firingMode, float chargeValue)
	{
		this.target = target;
		this.magnitude = magnitude;
		this.firingMode = firingMode;
		this.chargeValue = chargeValue;
		code = EventCode.OnPlayerRaygunHit;
	}

	public override DynValue GetLuaData()
	{
		if (data == null)
		{
			data = DynValue.NewPrimeTable();
			data.Table.Set("target", DynValue.FromObject(null, target ? target.GetLuaEntity() : null));
			data.Table.Set("magnitude", DynValue.FromObject(null, magnitude));
			data.Table.Set("firingMode", DynValue.FromObject(null, firingMode));
			data.Table.Set("chargeValue", DynValue.FromObject(null, chargeValue));
		}
		return data;
	}
}
