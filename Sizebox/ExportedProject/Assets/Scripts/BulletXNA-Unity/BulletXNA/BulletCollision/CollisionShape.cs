using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class CollisionShape
	{
		protected BroadphaseNativeTypes m_shapeType;

		protected object m_userPointer;

		public static float gContactThresholdFactor = 0.02f;

		public CollisionShape()
		{
			m_shapeType = BroadphaseNativeTypes.INVALID_SHAPE_PROXYTYPE;
			m_userPointer = null;
		}

		public virtual void Cleanup()
		{
		}

		public virtual void GetAabb(IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			GetAabb(ref t, out aabbMin, out aabbMax);
		}

		public abstract void GetAabb(ref IndexedMatrix t, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax);

		public virtual void GetBoundingSphere(out IndexedVector3 center, out float radius)
		{
			IndexedMatrix t = IndexedMatrix.Identity;
			IndexedVector3 aabbMin;
			IndexedVector3 aabbMax;
			GetAabb(ref t, out aabbMin, out aabbMax);
			radius = (aabbMax - aabbMin).Length() * 0.5f;
			center = (aabbMin + aabbMax) * 0.5f;
		}

		public virtual float GetAngularMotionDisc()
		{
			IndexedVector3 center;
			float radius;
			GetBoundingSphere(out center, out radius);
			return radius + center.Length();
		}

		public virtual float GetContactBreakingThreshold(float defaultContactThreshold)
		{
			return GetAngularMotionDisc() * defaultContactThreshold;
		}

		public void CalculateTemporalAabb(ref IndexedMatrix curTrans, ref IndexedVector3 linvel, ref IndexedVector3 angvel, float timeStep, out IndexedVector3 temporalAabbMin, out IndexedVector3 temporalAabbMax)
		{
			GetAabb(ref curTrans, out temporalAabbMin, out temporalAabbMax);
			float num = temporalAabbMax.X;
			float num2 = temporalAabbMax.Y;
			float num3 = temporalAabbMax.Z;
			float num4 = temporalAabbMin.X;
			float num5 = temporalAabbMin.Y;
			float num6 = temporalAabbMin.Z;
			IndexedVector3 indexedVector = linvel * timeStep;
			if (indexedVector.X > 0f)
			{
				num += indexedVector.X;
			}
			else
			{
				num4 += indexedVector.X;
			}
			if (indexedVector.Y > 0f)
			{
				num2 += indexedVector.Y;
			}
			else
			{
				num5 += indexedVector.Y;
			}
			if (indexedVector.Z > 0f)
			{
				num3 += indexedVector.Z;
			}
			else
			{
				num6 += indexedVector.Z;
			}
			float x = angvel.Length() * GetAngularMotionDisc() * timeStep;
			IndexedVector3 indexedVector2 = new IndexedVector3(x);
			temporalAabbMin = new IndexedVector3(num4, num5, num6);
			temporalAabbMax = new IndexedVector3(num, num2, num3);
			temporalAabbMin -= indexedVector2;
			temporalAabbMax += indexedVector2;
		}

		public bool IsPolyhedral()
		{
			return BroadphaseProxy.IsPolyhedral(GetShapeType());
		}

		public bool IsConvex()
		{
			return BroadphaseProxy.IsConvex(GetShapeType());
		}

		public bool IsNonMoving()
		{
			return BroadphaseProxy.IsNonMoving(GetShapeType());
		}

		public bool IsConvex2d()
		{
			return BroadphaseProxy.IsConvex2d(GetShapeType());
		}

		public bool IsConcave()
		{
			return BroadphaseProxy.IsConcave(GetShapeType());
		}

		public bool IsCompound()
		{
			return BroadphaseProxy.IsCompound(GetShapeType());
		}

		public bool IsSoftBody()
		{
			return BroadphaseProxy.IsSoftBody(GetShapeType());
		}

		public bool IsInfinite()
		{
			return BroadphaseProxy.IsInfinite(GetShapeType());
		}

		public virtual void SetLocalScaling(ref IndexedVector3 scaling)
		{
		}

		public virtual IndexedVector3 GetLocalScaling()
		{
			return new IndexedVector3(1f);
		}

		public virtual void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			inertia = IndexedVector3.Zero;
		}

		public virtual string GetName()
		{
			return "Not-Defined";
		}

		public BroadphaseNativeTypes GetShapeType()
		{
			return m_shapeType;
		}

		public abstract void SetMargin(float margin);

		public abstract float GetMargin();

		public void SetUserPointer(object userPtr)
		{
			m_userPointer = userPtr;
		}

		public object GetUserPointer()
		{
			return m_userPointer;
		}
	}
}
