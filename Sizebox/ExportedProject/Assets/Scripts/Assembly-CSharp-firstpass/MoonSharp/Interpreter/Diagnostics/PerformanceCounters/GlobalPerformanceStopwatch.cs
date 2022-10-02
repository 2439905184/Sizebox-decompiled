using System;
using System.Diagnostics;

namespace MoonSharp.Interpreter.Diagnostics.PerformanceCounters
{
	internal class GlobalPerformanceStopwatch : IPerformanceStopwatch
	{
		private class GlobalPerformanceStopwatch_StopwatchObject : IDisposable
		{
			private Stopwatch m_Stopwatch;

			private GlobalPerformanceStopwatch m_Parent;

			public GlobalPerformanceStopwatch_StopwatchObject(GlobalPerformanceStopwatch parent)
			{
				m_Parent = parent;
				m_Stopwatch = Stopwatch.StartNew();
			}

			public void Dispose()
			{
				m_Stopwatch.Stop();
				m_Parent.SignalStopwatchTerminated(m_Stopwatch);
			}
		}

		private int m_Count;

		private long m_Elapsed;

		private PerformanceCounter m_Counter;

		public GlobalPerformanceStopwatch(PerformanceCounter perfcounter)
		{
			m_Counter = perfcounter;
		}

		private void SignalStopwatchTerminated(Stopwatch sw)
		{
			m_Elapsed += sw.ElapsedMilliseconds;
			m_Count++;
		}

		public IDisposable Start()
		{
			return new GlobalPerformanceStopwatch_StopwatchObject(this);
		}

		public PerformanceResult GetResult()
		{
			return new PerformanceResult
			{
				Type = PerformanceCounterType.TimeMilliseconds,
				Global = false,
				Name = m_Counter.ToString(),
				Instances = m_Count,
				Counter = m_Elapsed
			};
		}
	}
}
