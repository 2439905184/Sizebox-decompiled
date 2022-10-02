namespace BulletXNA.BulletCollision
{
	public class RemovePairCallback : IOverlapCallback
	{
		private BroadphaseProxy m_obsoleteProxy;

		public RemovePairCallback(BroadphaseProxy obsoleteProxy)
		{
			m_obsoleteProxy = obsoleteProxy;
		}

		public virtual bool ProcessOverlap(BroadphasePair pair)
		{
			if (pair != null)
			{
				if (pair.m_pProxy0 != m_obsoleteProxy)
				{
					return pair.m_pProxy1 == m_obsoleteProxy;
				}
				return true;
			}
			return false;
		}
	}
}
