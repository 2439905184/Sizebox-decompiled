using MoonSharp.Interpreter;

public class PlayerTriggerReleaseEvent : IEvent
{
	public PlayerTriggerReleaseEvent()
	{
		code = EventCode.OnTriggerRelease;
	}

	public override DynValue GetLuaData()
	{
		if (data == null)
		{
			data = DynValue.NewPrimeTable();
		}
		return data;
	}
}
