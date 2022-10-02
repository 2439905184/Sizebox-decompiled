using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ManifoldResult : IDiscreteCollisionDetectorInterfaceResult
	{
		protected PersistentManifold m_manifoldPtr;

		protected IndexedMatrix m_rootTransA;

		protected IndexedMatrix m_rootTransB;

		protected CollisionObject m_body0;

		protected CollisionObject m_body1;

		protected int m_partId0;

		protected int m_partId1;

		protected int m_index0;

		protected int m_index1;

		public ManifoldResult()
		{
		}

		public ManifoldResult(CollisionObject body0, CollisionObject body1)
		{
			Initialise(body0, body1);
		}

		public void Initialise(CollisionObject body0, CollisionObject body1)
		{
			m_body0 = body0;
			m_body1 = body1;
			m_rootTransA = body0.GetWorldTransform();
			m_rootTransB = body1.GetWorldTransform();
			m_manifoldPtr = null;
		}

		public void SetPersistentManifold(PersistentManifold manifoldPtr)
		{
			m_manifoldPtr = manifoldPtr;
		}

		public PersistentManifold GetPersistentManifold()
		{
			return m_manifoldPtr;
		}

		public virtual void SetShapeIdentifiersA(int partId0, int index0)
		{
			m_partId0 = partId0;
			m_index0 = index0;
		}

		public virtual void SetShapeIdentifiersB(int partId1, int index1)
		{
			m_partId1 = partId1;
			m_index1 = index1;
		}

		public CollisionObject GetBody0Internal()
		{
			return m_body0;
		}

		public CollisionObject GetBody1Internal()
		{
			return m_body1;
		}

		public virtual void AddContactPoint(IndexedVector3 normalOnBInWorld, IndexedVector3 pointInWorld, float depth)
		{
			AddContactPoint(ref normalOnBInWorld, ref pointInWorld, depth);
		}

		public virtual void AddContactPoint(ref IndexedVector3 normalOnBInWorld, ref IndexedVector3 pointInWorld, float depth)
		{
			if (!(depth > m_manifoldPtr.GetContactBreakingThreshold()))
			{
				bool flag = m_manifoldPtr.GetBody0() != m_body0;
				IndexedVector3 v = pointInWorld + normalOnBInWorld * depth;
				IndexedVector3 o;
				IndexedVector3 o2;
				if (flag)
				{
					MathUtil.InverseTransform(ref m_rootTransB, ref v, out o);
					MathUtil.InverseTransform(ref m_rootTransA, ref pointInWorld, out o2);
				}
				else
				{
					MathUtil.InverseTransform(ref m_rootTransA, ref v, out o);
					MathUtil.InverseTransform(ref m_rootTransB, ref pointInWorld, out o2);
				}
				ManifoldPoint manifoldPoint = BulletGlobals.ManifoldPointPool.Get();
				manifoldPoint.Initialise(ref o, ref o2, ref normalOnBInWorld, depth);
				manifoldPoint.SetPositionWorldOnA(ref v);
				manifoldPoint.SetPositionWorldOnB(ref pointInWorld);
				int num = m_manifoldPtr.GetCacheEntry(manifoldPoint);
				manifoldPoint.SetCombinedFriction(CalculateCombinedFriction(m_body0, m_body1));
				manifoldPoint.SetCombinedRestitution(CalculateCombinedRestitution(m_body0, m_body1));
				if (flag)
				{
					manifoldPoint.m_partId0 = m_partId1;
					manifoldPoint.m_partId1 = m_partId0;
					manifoldPoint.m_index0 = m_index1;
					manifoldPoint.m_index1 = m_index0;
				}
				else
				{
					manifoldPoint.m_partId0 = m_partId0;
					manifoldPoint.m_partId1 = m_partId1;
					manifoldPoint.m_index0 = m_index0;
					manifoldPoint.m_index1 = m_index1;
				}
				if (num >= 0)
				{
					m_manifoldPtr.ReplaceContactPoint(manifoldPoint, num);
				}
				else
				{
					num = m_manifoldPtr.AddManifoldPoint(manifoldPoint);
				}
				if (BulletGlobals.gContactAddedCallback != null && ((m_body0.GetCollisionFlags() & CollisionFlags.CF_CUSTOM_MATERIAL_CALLBACK) != 0 || (m_body1.GetCollisionFlags() & CollisionFlags.CF_CUSTOM_MATERIAL_CALLBACK) != 0))
				{
					CollisionObject colObj = (flag ? m_body1 : m_body0);
					CollisionObject colObj2 = (flag ? m_body0 : m_body1);
					ManifoldPoint cp = m_manifoldPtr.GetContactPoint(num);
					BulletGlobals.gContactAddedCallback.Callback(ref cp, colObj, manifoldPoint.m_partId0, manifoldPoint.m_index0, colObj2, manifoldPoint.m_partId1, manifoldPoint.m_index1);
				}
			}
		}

		public void RefreshContactPoints()
		{
			if (m_manifoldPtr.GetNumContacts() != 0)
			{
				if (m_manifoldPtr.GetBody0() != m_body0)
				{
					m_manifoldPtr.RefreshContactPoints(ref m_rootTransB, ref m_rootTransA);
				}
				else
				{
					m_manifoldPtr.RefreshContactPoints(ref m_rootTransA, ref m_rootTransB);
				}
			}
		}

		private float CalculateCombinedFriction(CollisionObject body0, CollisionObject body1)
		{
			float num = body0.GetFriction() * body1.GetFriction();
			if (num < -10f)
			{
				num = -10f;
			}
			if (num > 10f)
			{
				num = 10f;
			}
			return num;
		}

		private float CalculateCombinedRestitution(CollisionObject body0, CollisionObject body1)
		{
			return body0.GetRestitution() * body1.GetRestitution();
		}
	}
}
