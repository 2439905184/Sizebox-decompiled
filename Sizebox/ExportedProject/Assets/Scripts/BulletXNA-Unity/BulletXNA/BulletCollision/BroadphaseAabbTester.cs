namespace BulletXNA.BulletCollision
{
	public struct BroadphaseAabbTester : ICollide
	{
		private IBroadphaseAabbCallback m_aabbCallback;

		public BroadphaseAabbTester(IBroadphaseAabbCallback orgCallback)
		{
			m_aabbCallback = orgCallback;
		}

		public void Process(DbvtNode leaf)
		{
			DbvtProxy proxy = leaf.data as DbvtProxy;
			m_aabbCallback.Process(proxy);
		}

		public void Process(DbvtNode n, DbvtNode n2)
		{
		}

		public void Process(DbvtNode n, float f)
		{
			Process(n);
		}

		public bool Descent(DbvtNode n)
		{
			return true;
		}

		public bool AllLeaves(DbvtNode n)
		{
			return true;
		}
	}
}
