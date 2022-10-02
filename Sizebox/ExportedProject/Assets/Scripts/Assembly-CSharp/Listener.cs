using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Listener
{
	public string interestCode;

	public IListener listener;

	public Listener(IListener listener, string interest)
	{
		interestCode = interest;
		this.listener = listener;
	}
}
