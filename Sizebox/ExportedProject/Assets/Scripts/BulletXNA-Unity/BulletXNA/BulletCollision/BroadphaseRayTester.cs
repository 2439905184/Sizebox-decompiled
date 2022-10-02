using System;

namespace BulletXNA.BulletCollision
{
	public class BroadphaseRayTester : ICollide, IDisposable
	{
		private BroadphaseRayCallback m_rayCallback;

		public BroadphaseRayTester()
		{
		}

		public BroadphaseRayTester(BroadphaseRayCallback orgCallback)
		{
			m_rayCallback = orgCallback;
		}

		public void Initialize(BroadphaseRayCallback orgCallback)
		{
			m_rayCallback = orgCallback;
		}

		public void Process(DbvtNode leaf)
		{
			DbvtProxy proxy = leaf.data as DbvtProxy;
			m_rayCallback.Process(proxy);
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

		public void Dispose()
		{
			BulletGlobals.BroadphaseRayTesterPool.Free(this);
		}
	}
}
