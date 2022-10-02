namespace BulletXNA
{
	public class CProfileNode
	{
		protected string m_name;

		protected int m_totalCalls;

		protected float m_totalTime;

		protected long m_startTime;

		protected int m_recursionCounter;

		protected CProfileNode m_parent;

		protected CProfileNode m_child;

		protected CProfileNode m_sibling;

		protected BasicProfileManager m_profileManager;

		public CProfileNode(string name, CProfileNode parent, BasicProfileManager profileManager)
		{
			m_name = name;
			m_parent = parent;
			m_profileManager = profileManager;
			Reset();
		}

		public void Cleanup()
		{
			m_child = null;
			m_sibling = null;
		}

		public CProfileNode Get_Sub_Node(string name)
		{
			for (CProfileNode cProfileNode = m_child; cProfileNode != null; cProfileNode = cProfileNode.m_sibling)
			{
				if (cProfileNode.m_name == name)
				{
					return cProfileNode;
				}
			}
			CProfileNode cProfileNode2 = new CProfileNode(name, this, m_profileManager);
			cProfileNode2.m_sibling = m_child;
			m_child = cProfileNode2;
			return cProfileNode2;
		}

		public CProfileNode Get_Parent()
		{
			return m_parent;
		}

		public CProfileNode Get_Sibling()
		{
			return m_sibling;
		}

		public CProfileNode Get_Child()
		{
			return m_child;
		}

		public void CleanupMemory()
		{
		}

		public void Reset()
		{
			m_totalCalls = 0;
			m_totalTime = 0f;
			if (m_child != null)
			{
				m_child.Reset();
			}
			if (m_sibling != null)
			{
				m_sibling.Reset();
			}
		}

		public void Call()
		{
			m_totalCalls++;
			if (m_recursionCounter++ == 0)
			{
				m_startTime = m_profileManager.m_stopwatch.ElapsedTicks;
			}
		}

		public bool Return()
		{
			if (--m_recursionCounter == 0 && m_totalCalls != 0)
			{
				long elapsedTicks = m_profileManager.m_stopwatch.ElapsedTicks;
				elapsedTicks -= m_startTime;
				m_totalTime += (float)elapsedTicks / m_profileManager.Profile_Get_Tick_Rate();
			}
			return m_recursionCounter == 0;
		}

		public string Get_Name()
		{
			return m_name;
		}

		public int Get_Total_Calls()
		{
			return m_totalCalls;
		}

		public float Get_Total_Time()
		{
			return m_totalTime;
		}
	}
}
