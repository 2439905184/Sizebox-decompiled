namespace BulletXNA.BulletCollision
{
	public class RemovePairContainingProxy
	{
		private BroadphaseProxy m_targetProxy;

		public virtual void Cleanup()
		{
		}

		protected virtual bool ProcessOverlap(ref BroadphasePair pair)
		{
			SimpleBroadphaseProxy simpleBroadphaseProxy = (SimpleBroadphaseProxy)pair.m_pProxy0;
			SimpleBroadphaseProxy simpleBroadphaseProxy2 = (SimpleBroadphaseProxy)pair.m_pProxy1;
			if (m_targetProxy != simpleBroadphaseProxy)
			{
				return m_targetProxy == simpleBroadphaseProxy2;
			}
			return true;
		}
	}
}
