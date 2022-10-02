using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class BroadphaseRayCallback : IBroadphaseAabbCallback
	{
		public IndexedVector3 m_rayDirectionInverse;

		public bool[] m_signs = new bool[3];

		public float m_lambda_max;

		public virtual void Cleanup()
		{
		}

		public abstract bool Process(BroadphaseProxy proxy);
	}
}
