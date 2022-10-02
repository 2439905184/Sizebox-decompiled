namespace MoonSharp.Interpreter.Interop
{
	public class AnonWrapper
	{
	}
	public class AnonWrapper<T> : AnonWrapper
	{
		public T Value { get; set; }

		public AnonWrapper()
		{
		}

		public AnonWrapper(T o)
		{
			Value = o;
		}
	}
}
