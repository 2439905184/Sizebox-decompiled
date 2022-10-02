using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class StaticPlaneShape : ConcaveShape
	{
		protected IndexedVector3 m_localAabbMin;

		protected IndexedVector3 m_localAabbMax;

		protected IndexedVector3 m_planeNormal;

		protected float m_planeConstant;

		protected IndexedVector3 m_localScaling;

		public StaticPlaneShape(IndexedVector3 planeNormal, float planeConstant)
			: this(ref planeNormal, planeConstant)
		{
		}

		public StaticPlaneShape(ref IndexedVector3 planeNormal, float planeConstant)
		{
			m_shapeType = BroadphaseNativeTypes.STATIC_PLANE_PROXYTYPE;
			m_planeNormal = planeNormal;
			m_planeConstant = planeConstant;
			m_localScaling = IndexedVector3.Zero;
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			aabbMin = MathUtil.MIN_VECTOR;
			aabbMax = MathUtil.MAX_VECTOR;
		}

		public override void ProcessAllTriangles(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			float num = ((aabbMax - aabbMin) * 0.5f).Length();
			IndexedVector3 indexedVector = (aabbMax + aabbMin) * 0.5f;
			IndexedVector3 p;
			IndexedVector3 q;
			TransformUtil.PlaneSpace1(ref m_planeNormal, out p, out q);
			IndexedVector3 zero = IndexedVector3.Zero;
			IndexedVector3 zero2 = IndexedVector3.Zero;
			IndexedVector3 indexedVector2 = indexedVector - (IndexedVector3.Dot(m_planeNormal, indexedVector) - m_planeConstant) * m_planeNormal;
			IndexedVector3[] array = new IndexedVector3[3]
			{
				indexedVector2 + p * num + q * num,
				indexedVector2 + p * num - q * num,
				indexedVector2 - p * num - q * num
			};
			callback.ProcessTriangle(array, 0, 0);
			array[0] = indexedVector2 - p * num - q * num;
			array[1] = indexedVector2 - p * num + q * num;
			array[2] = indexedVector2 + p * num + q * num;
			callback.ProcessTriangle(array, 0, 1);
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			inertia = IndexedVector3.Zero;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			m_localScaling = scaling;
		}

		public override IndexedVector3 GetLocalScaling()
		{
			return m_localScaling;
		}

		public IndexedVector3 GetPlaneNormal()
		{
			return m_planeNormal;
		}

		public float GetPlaneConstant()
		{
			return m_planeConstant;
		}

		public override string GetName()
		{
			return "STATICPLANE";
		}
	}
}
