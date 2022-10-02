namespace MoonSharp.Interpreter.REPL
{
	public class ReplHistoryInterpreter : ReplInterpreter
	{
		private string[] m_History;

		private int m_Last = -1;

		private int m_Navi = -1;

		public ReplHistoryInterpreter(Script script, int historySize)
			: base(script)
		{
			m_History = new string[historySize];
		}

		public override DynValue Evaluate(string input)
		{
			m_Navi = -1;
			m_Last = (m_Last + 1) % m_History.Length;
			m_History[m_Last] = input;
			return base.Evaluate(input);
		}

		public string HistoryPrev()
		{
			if (m_Navi == -1)
			{
				m_Navi = m_Last;
			}
			else
			{
				m_Navi = (m_Navi - 1 + m_History.Length) % m_History.Length;
			}
			if (m_Navi >= 0)
			{
				return m_History[m_Navi];
			}
			return null;
		}

		public string HistoryNext()
		{
			if (m_Navi == -1)
			{
				return null;
			}
			m_Navi = (m_Navi + 1) % m_History.Length;
			if (m_Navi >= 0)
			{
				return m_History[m_Navi];
			}
			return null;
		}
	}
}
