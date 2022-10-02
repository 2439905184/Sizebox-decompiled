using MoonSharp.Interpreter;

public class ActionCompleteEvent : IEvent
{
	private Humanoid agent;

	private AgentAction action;

	public ActionCompleteEvent(Humanoid agent, AgentAction action)
	{
		this.agent = agent;
		this.action = action;
		code = EventCode.OnActionComplete;
	}

	public override DynValue GetLuaData()
	{
		if (data == null)
		{
			data = DynValue.NewPrimeTable();
			data.Table.Set("agent", DynValue.FromObject(null, agent.GetLuaEntity()));
			data.Table.Set("action", DynValue.FromObject(null, action.name));
		}
		return data;
	}
}
