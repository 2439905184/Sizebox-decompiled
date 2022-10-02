using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SphereShape : ConvexInternalShape
	{
		public SphereShape()
			: this(1f)
		{
		}

		public SphereShape(float radius)
		{
			m_shapeType = BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE;
			m_implicitShapeDimensions.X = radius;
			m_collisionMargin = radius;
		}

		public void Initialize(float radius)
		{
			m_shapeType = BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE;
			m_implicitShapeDimensions.X = radius;
			m_collisionMargin = radius;
		}

		public override IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			IndexedVector3 indexedVector = LocalGetSupportingVertexWithoutMargin(ref vec);
			IndexedVector3 indexedVector2 = vec;
			if (indexedVector2.LengthSquared() < 1.4210855E-14f)
			{
				indexedVector2 = new IndexedVector3(-1f);
			}
			indexedVector2.Normalize();
			return indexedVector + GetMargin() * indexedVector2;
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			return IndexedVector3.Zero;
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			for (int i = 0; i < numVectors; i++)
			{
				supportVerticesOut[i] = IndexedVector4.Zero;
			}
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			float x = 0.4f * mass * GetMargin() * GetMargin();
			inertia = new IndexedVector3(x);
		}

		public override void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			IndexedVector3 origin = t._origin;
			float margin = GetMargin();
			IndexedVector3 indexedVector = new IndexedVector3(margin);
			aabbMin = origin - indexedVector;
			aabbMax = origin + indexedVector;
		}

		public virtual float GetRadius()
		{
			return m_implicitShapeDimensions.X * m_localScaling.X;
		}

		public void SetUnscaledRadius(float radius)
		{
			m_implicitShapeDimensions.X = radius;
			SetMargin(radius);
		}

		public override string GetName()
		{
			return "SPHERE";
		}

		public override float GetMargin()
		{
			return GetRadius();
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}
	}
}
