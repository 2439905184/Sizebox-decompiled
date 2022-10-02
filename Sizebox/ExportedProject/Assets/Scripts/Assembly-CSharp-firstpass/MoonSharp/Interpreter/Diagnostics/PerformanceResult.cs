using System;

namespace MoonSharp.Interpreter.Diagnostics
{
	public class PerformanceResult
	{
		public string Name { get; internal set; }

		public long Counter { get; internal set; }

		public int Instances { get; internal set; }

		public bool Global { get; internal set; }

		public PerformanceCounterType Type { get; internal set; }

		public override string ToString()
		{
			return string.Format("{0}{1} : {2} times / {3} {4}", Name, Global ? "(g)" : "", Instances, Counter, PerformanceCounterTypeToString(Type));
		}

		public static string PerformanceCounterTypeToString(PerformanceCounterType Type)
		{
			switch (Type)
			{
			case PerformanceCounterType.MemoryBytes:
				return "bytes";
			case PerformanceCounterType.TimeMilliseconds:
				return "ms";
			default:
				throw new InvalidOperationException("PerformanceCounterType has invalid value " + Type);
			}
		}
	}
}
