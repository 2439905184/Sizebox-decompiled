using MoonSharp.Interpreter;

public class IEvent
{
	public string code;

	public DynValue data;

	public virtual DynValue GetLuaData()
	{
		if (data == null)
		{
			data = DynValue.Nil;
		}
		return data;
	}
}
