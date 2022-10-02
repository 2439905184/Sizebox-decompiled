using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class ConcaveShape : CollisionShape
	{
		protected float m_collisionMargin;

		public ConcaveShape()
		{
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public abstract void ProcessAllTriangles(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax);

		public override float GetMargin()
		{
			return m_collisionMargin;
		}

		public override void SetMargin(float collisionMargin)
		{
			m_collisionMargin = collisionMargin;
		}
	}
}
