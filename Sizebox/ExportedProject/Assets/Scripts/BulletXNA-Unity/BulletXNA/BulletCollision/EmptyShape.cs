using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class EmptyShape : ConcaveShape
	{
		protected IndexedVector3 m_localScaling;

		public EmptyShape()
		{
			m_shapeType = BroadphaseNativeTypes.EMPTY_SHAPE_PROXYTYPE;
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			float margin = GetMargin();
			IndexedVector3 indexedVector = new IndexedVector3(margin);
			aabbMin = t._origin - indexedVector;
			aabbMax = t._origin + indexedVector;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			m_localScaling = scaling;
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return m_localScaling;
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			inertia = IndexedVector3.Zero;
		}

		public override string GetName()
		{
			return "Empty";
		}

		public override void ProcessAllTriangles(ITriangleCallback callback, ref IndexedVector3 vec1, ref IndexedVector3 vec2)
		{
		}
	}
}
