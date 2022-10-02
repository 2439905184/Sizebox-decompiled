using System;

namespace BulletXNA
{
	public class BasicProfileIterator : IProfileIterator
	{
		private CProfileNode m_currentParent;

		private CProfileNode m_currentChild;

		public BasicProfileIterator(CProfileNode start)
		{
			m_currentParent = start;
			m_currentChild = start.Get_Child();
		}

		public void First()
		{
			m_currentChild = m_currentParent.Get_Child();
		}

		public void Next()
		{
			m_currentChild = m_currentChild.Get_Sibling();
		}

		public bool Is_Done()
		{
			return m_currentChild == null;
		}

		public bool Is_Root()
		{
			return m_currentParent.Get_Parent() == null;
		}

		public void Enter_Child(int index)
		{
			m_currentChild = m_currentParent.Get_Child();
			while (m_currentChild != null && index != 0)
			{
				index--;
				m_currentChild = m_currentChild.Get_Sibling();
			}
			if (m_currentChild != null)
			{
				m_currentParent = m_currentChild;
				m_currentChild = m_currentParent.Get_Child();
			}
		}

		public void Enter_Largest_Child()
		{
			throw new NotImplementedException();
		}

		public void Enter_Parent()
		{
			if (m_currentParent.Get_Parent() != null)
			{
				m_currentParent = m_currentParent.Get_Parent();
			}
			m_currentChild = m_currentParent.Get_Child();
		}

		public string Get_Current_Name()
		{
			return m_currentChild.Get_Name();
		}

		public int Get_Current_Total_Calls()
		{
			return m_currentChild.Get_Total_Calls();
		}

		public float Get_Current_Total_Time()
		{
			return m_currentChild.Get_Total_Time();
		}

		public string Get_Current_Parent_Name()
		{
			return m_currentParent.Get_Name();
		}

		public int Get_Current_Parent_Total_Calls()
		{
			return m_currentParent.Get_Total_Calls();
		}

		public float Get_Current_Parent_Total_Time()
		{
			return m_currentParent.Get_Total_Time();
		}
	}
}
