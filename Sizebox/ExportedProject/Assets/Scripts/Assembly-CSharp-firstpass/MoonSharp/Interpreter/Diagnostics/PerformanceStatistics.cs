using System;
using System.Text;
using MoonSharp.Interpreter.Diagnostics.PerformanceCounters;

namespace MoonSharp.Interpreter.Diagnostics
{
	public class PerformanceStatistics
	{
		private IPerformanceStopwatch[] m_Stopwatches = new IPerformanceStopwatch[4];

		private static IPerformanceStopwatch[] m_GlobalStopwatches = new IPerformanceStopwatch[4];

		private bool m_Enabled;

		public bool Enabled
		{
			get
			{
				return m_Enabled;
			}
			set
			{
				if (value && !m_Enabled)
				{
					if (m_GlobalStopwatches[3] == null)
					{
						m_GlobalStopwatches[3] = new GlobalPerformanceStopwatch(PerformanceCounter.AdaptersCompilation);
					}
					for (int i = 0; i < 4; i++)
					{
						m_Stopwatches[i] = m_GlobalStopwatches[i] ?? new PerformanceStopwatch((PerformanceCounter)i);
					}
				}
				else if (!value && m_Enabled)
				{
					m_Stopwatches = new IPerformanceStopwatch[4];
					m_GlobalStopwatches = new IPerformanceStopwatch[4];
				}
				m_Enabled = value;
			}
		}

		public PerformanceResult GetPerformanceCounterResult(PerformanceCounter pc)
		{
			IPerformanceStopwatch performanceStopwatch = m_Stopwatches[(int)pc];
			if (performanceStopwatch == null)
			{
				return null;
			}
			return performanceStopwatch.GetResult();
		}

		internal IDisposable StartStopwatch(PerformanceCounter pc)
		{
			IPerformanceStopwatch performanceStopwatch = m_Stopwatches[(int)pc];
			if (performanceStopwatch == null)
			{
				return null;
			}
			return performanceStopwatch.Start();
		}

		internal static IDisposable StartGlobalStopwatch(PerformanceCounter pc)
		{
			IPerformanceStopwatch performanceStopwatch = m_GlobalStopwatches[(int)pc];
			if (performanceStopwatch == null)
			{
				return null;
			}
			return performanceStopwatch.Start();
		}

		public string GetPerformanceLog()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 4; i++)
			{
				PerformanceResult performanceCounterResult = GetPerformanceCounterResult((PerformanceCounter)i);
				if (performanceCounterResult != null)
				{
					stringBuilder.AppendLine(performanceCounterResult.ToString());
				}
			}
			return stringBuilder.ToString();
		}
	}
}
