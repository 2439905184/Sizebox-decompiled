namespace BulletXNA.BulletCollision
{
	public class GIM_BVH_TREE_NODE
	{
		public AABB m_bound = default(AABB);

		protected int m_escapeIndexOrDataIndex;

		public GIM_BVH_TREE_NODE()
		{
			m_escapeIndexOrDataIndex = 0;
		}

		public bool IsLeafNode()
		{
			return m_escapeIndexOrDataIndex >= 0;
		}

		public int GetEscapeIndex()
		{
			return -m_escapeIndexOrDataIndex;
		}

		public void SetEscapeIndex(int index)
		{
			m_escapeIndexOrDataIndex = -index;
		}

		public int GetDataIndex()
		{
			return m_escapeIndexOrDataIndex;
		}

		public void SetDataIndex(int index)
		{
			m_escapeIndexOrDataIndex = index;
		}
	}
}
