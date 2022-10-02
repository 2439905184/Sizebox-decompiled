using System;
using System.Diagnostics;

namespace BulletXNA
{
	public class BasicProfileManager : IProfileManager
	{
		private CProfileNode m_root;

		private CProfileNode m_currentNode;

		private int m_frameCounter;

		private long m_resetTime;

		public Stopwatch m_stopwatch;

		public BasicProfileManager()
		{
			m_root = new CProfileNode("Root", null, this);
			m_currentNode = m_root;
			m_stopwatch = new Stopwatch();
		}

		public void Start_Profile(string name)
		{
			if (name != m_currentNode.Get_Name())
			{
				m_currentNode = m_currentNode.Get_Sub_Node(name);
			}
			m_currentNode.Call();
		}

		public void Stop_Profile()
		{
			if (m_currentNode.Return())
			{
				m_currentNode = m_currentNode.Get_Parent();
			}
		}

		public void CleanupMemory()
		{
			throw new NotImplementedException();
		}

		public void Reset()
		{
			m_stopwatch.Reset();
			m_stopwatch.Start();
			m_root.Reset();
			m_root.Call();
			m_frameCounter = 0;
			m_resetTime = m_stopwatch.ElapsedTicks;
		}

		public void Increment_Frame_Counter()
		{
			m_frameCounter++;
		}

		public int Get_Frame_Count_Since_Reset()
		{
			long elapsedTicks = m_stopwatch.ElapsedTicks;
			elapsedTicks -= m_resetTime;
			return (int)((float)elapsedTicks / Profile_Get_Tick_Rate());
		}

		public float Profile_Get_Tick_Rate()
		{
			return 1000f;
		}

		public float Get_Time_Since_Reset()
		{
			long elapsedTicks = m_stopwatch.ElapsedTicks;
			elapsedTicks -= m_resetTime;
			return (float)elapsedTicks / Profile_Get_Tick_Rate();
		}

		public void DumpRecursive(IProfileIterator profileIterator, int spacing)
		{
		}

		public void DumpAll()
		{
		}

		public IProfileIterator getIterator()
		{
			return new BasicProfileIterator(m_root);
		}
	}
}
