using MoonSharp.Interpreter;

public class PlayerTriggerPressEvent : IEvent
{
	public PlayerTriggerPressEvent()
	{
		code = EventCode.OnTriggerPress;
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
