using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class UnionFind
	{
		private ObjectArray<Element> m_elements;

		private UnionFindElementSortPredicate m_sortPredicate = new UnionFindElementSortPredicate();

		public UnionFind()
		{
			m_elements = new ObjectArray<Element>();
		}

		public virtual void Cleanup()
		{
			Free();
		}

		public void sortIslands()
		{
			int count = m_elements.Count;
			Element[] rawArray = m_elements.GetRawArray();
			for (int i = 0; i < count; i++)
			{
				rawArray[i].m_id = Find(i);
			}
			m_elements.QuickSort(m_sortPredicate);
		}

		public void Reset(int N)
		{
			Allocate(N);
			Element[] rawArray = m_elements.GetRawArray();
			for (int i = 0; i < N; i++)
			{
				rawArray[i].m_id = i;
				rawArray[i].m_sz = 1;
			}
		}

		public int GetNumElements()
		{
			return m_elements.Count;
		}

		public bool IsRoot(int x)
		{
			return x == m_elements[x].m_id;
		}

		public Element GetElement(int index)
		{
			return m_elements[index];
		}

		public void SetElementSize(int index, int size)
		{
			m_elements.GetRawArray()[index].m_sz = size;
		}

		public void Allocate(int N)
		{
			m_elements.Resize(N, true);
		}

		public void Free()
		{
			m_elements.Clear();
		}

		public bool Find(int p, int q)
		{
			return Find(p) == Find(q);
		}

		public void Unite(int p, int q)
		{
			int num = Find(p);
			int num2 = Find(q);
			if (num != num2)
			{
				m_elements.GetRawArray()[num].m_id = num2;
				m_elements.GetRawArray()[num2].m_sz += m_elements[num].m_sz;
			}
		}

		public int Find(int x)
		{
			Element[] rawArray = m_elements.GetRawArray();
			while (x != rawArray[x].m_id)
			{
				Element element = rawArray[rawArray[x].m_id];
				rawArray[x].m_id = element.m_id;
				x = element.m_id;
			}
			return x;
		}
	}
}
