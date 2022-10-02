namespace MoonSharp.Interpreter
{
	public enum DataType
	{
		Nil = 0,
		Void = 1,
		Boolean = 2,
		Number = 3,
		String = 4,
		Function = 5,
		Table = 6,
		Tuple = 7,
		UserData = 8,
		Thread = 9,
		ClrFunction = 10,
		TailCallRequest = 11,
		YieldRequest = 12
	}
}
