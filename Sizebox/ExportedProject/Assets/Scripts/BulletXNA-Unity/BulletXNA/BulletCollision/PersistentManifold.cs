using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class PersistentManifold : TypedObject, IComparable
	{
		private const int MANIFOLD_CACHE_SIZE = 4;

		private ManifoldPoint[] m_pointCache = new ManifoldPoint[4];

		private object m_body0;

		private object m_body1;

		private int m_cachedPoints;

		public int m_companionIdA;

		public int m_companionIdB;

		public int m_index1a;

		private float m_contactBreakingThreshold;

		private float m_contactProcessingThreshold;

		public static IContactDestroyedCallback gContactDestroyedCallback;

		public static IContactProcessedCallback gContactProcessedCallback;

		private int SortCachedPoints(ManifoldPoint pt)
		{
			int num = -1;
			float distance = pt.GetDistance();
			for (int i = 0; i < m_pointCache.Length; i++)
			{
				if (m_pointCache[i].GetDistance() < distance)
				{
					num = i;
					distance = m_pointCache[i].GetDistance();
				}
			}
			float x = 0f;
			float y = 0f;
			float z = 0f;
			float w = 0f;
			if (num != 0)
			{
				IndexedVector3 v = pt.GetLocalPointA() - m_pointCache[1].GetLocalPointA();
				IndexedVector3 v2 = m_pointCache[3].GetLocalPointA() - m_pointCache[2].GetLocalPointA();
				x = IndexedVector3.Cross(v, v2).LengthSquared();
			}
			if (num != 1)
			{
				IndexedVector3 v3 = pt.GetLocalPointA() - m_pointCache[0].GetLocalPointA();
				IndexedVector3 v4 = m_pointCache[3].GetLocalPointA() - m_pointCache[2].GetLocalPointA();
				y = IndexedVector3.Cross(v3, v4).LengthSquared();
			}
			if (num != 2)
			{
				IndexedVector3 v5 = pt.GetLocalPointA() - m_pointCache[0].GetLocalPointA();
				IndexedVector3 v6 = m_pointCache[3].GetLocalPointA() - m_pointCache[1].GetLocalPointA();
				z = IndexedVector3.Cross(v5, v6).LengthSquared();
			}
			if (num != 3)
			{
				IndexedVector3 v7 = pt.GetLocalPointA() - m_pointCache[0].GetLocalPointA();
				IndexedVector3 v8 = m_pointCache[2].GetLocalPointA() - m_pointCache[1].GetLocalPointA();
				w = IndexedVector3.Cross(v7, v8).LengthSquared();
			}
			IndexedVector4 a = new IndexedVector4(x, y, z, w);
			return MathUtil.ClosestAxis(ref a);
		}

		public int FindContactPoint(ref ManifoldPoint unUsed, int numUnused, ref ManifoldPoint pt)
		{
			return 0;
		}

		public PersistentManifold()
			: base(1025)
		{
		}

		public PersistentManifold(object body0, object body1, int foo, float contactBreakingThreshold, float contactProcessingThreshold)
			: base(1025)
		{
			m_body0 = body0;
			m_body1 = body1;
			m_contactBreakingThreshold = contactBreakingThreshold;
			m_contactProcessingThreshold = contactProcessingThreshold;
			m_cachedPoints = 0;
		}

		public void Initialise(object body0, object body1, int foo, float contactBreakingThreshold, float contactProcessingThreshold)
		{
			m_body0 = body0;
			m_body1 = body1;
			m_contactBreakingThreshold = contactBreakingThreshold;
			m_contactProcessingThreshold = contactProcessingThreshold;
			m_cachedPoints = 0;
		}

		public object GetBody0()
		{
			return m_body0;
		}

		public object GetBody1()
		{
			return m_body1;
		}

		public void SetBodies(object body0, object body1)
		{
			m_body0 = body0;
			m_body1 = body1;
		}

		public void ClearUserCache(ref ManifoldPoint pt)
		{
			object userPersistentData = pt.m_userPersistentData;
			if (userPersistentData != null)
			{
				if (pt.m_userPersistentData != null && gContactDestroyedCallback != null)
				{
					gContactDestroyedCallback.Callback(pt.m_userPersistentData);
					pt.m_userPersistentData = null;
				}
				DebugPersistency();
			}
			BulletGlobals.ManifoldPointPool.Free(pt);
			pt = null;
		}

		public void DebugPersistency()
		{
		}

		public int GetNumContacts()
		{
			return m_cachedPoints;
		}

		public ManifoldPoint GetContactPoint(int index)
		{
			return m_pointCache[index];
		}

		public float GetContactBreakingThreshold()
		{
			return m_contactBreakingThreshold;
		}

		public float GetContactProcessingThreshold()
		{
			return m_contactProcessingThreshold;
		}

		public int GetCacheEntry(ManifoldPoint newPoint)
		{
			float contactBreakingThreshold = GetContactBreakingThreshold();
			contactBreakingThreshold *= contactBreakingThreshold;
			int numContacts = GetNumContacts();
			int result = -1;
			for (int i = 0; i < numContacts; i++)
			{
				float num = (m_pointCache[i].GetLocalPointA() - newPoint.GetLocalPointA()).LengthSquared();
				if (num < contactBreakingThreshold)
				{
					contactBreakingThreshold = num;
					result = i;
				}
			}
			return result;
		}

		public int AddManifoldPoint(ManifoldPoint newPoint)
		{
			int num = GetNumContacts();
			if (num == 4)
			{
				num = SortCachedPoints(newPoint);
				ClearUserCache(ref m_pointCache[num]);
			}
			else
			{
				m_cachedPoints++;
			}
			if (num < 0)
			{
				num = 0;
			}
			m_pointCache[num] = newPoint;
			return num;
		}

		public void RemoveContactPoint(int index)
		{
			ClearUserCache(ref m_pointCache[index]);
			int num = GetNumContacts() - 1;
			if (index != num)
			{
				m_pointCache[index] = m_pointCache[num];
				m_pointCache[num] = null;
			}
			m_cachedPoints--;
		}

		public void ReplaceContactPoint(ManifoldPoint newPoint, int insertIndex)
		{
			int lifeTime = m_pointCache[insertIndex].GetLifeTime();
			float appliedImpulse = m_pointCache[insertIndex].m_appliedImpulse;
			float appliedImpulseLateral = m_pointCache[insertIndex].m_appliedImpulseLateral1;
			float appliedImpulseLateral2 = m_pointCache[insertIndex].m_appliedImpulseLateral2;
			object userPersistentData = m_pointCache[insertIndex].GetUserPersistentData();
			BulletGlobals.ManifoldPointPool.Free(m_pointCache[insertIndex]);
			m_pointCache[insertIndex] = newPoint;
			m_pointCache[insertIndex].SetUserPersistentData(userPersistentData);
			m_pointCache[insertIndex].SetAppliedImpulse(appliedImpulse);
			m_pointCache[insertIndex].SetAppliedImpulseLateral1(appliedImpulseLateral);
			m_pointCache[insertIndex].SetAppliedImpulseLateral2(appliedImpulseLateral2);
			m_pointCache[insertIndex].m_constraintRow[0].m_accumImpulse = appliedImpulse;
			m_pointCache[insertIndex].m_constraintRow[1].m_accumImpulse = appliedImpulseLateral;
			m_pointCache[insertIndex].m_constraintRow[2].m_accumImpulse = appliedImpulseLateral2;
			m_pointCache[insertIndex].SetLifeTime(lifeTime);
		}

		public bool ValidContactDistance(ManifoldPoint pt)
		{
			return pt.m_distance1 <= GetContactBreakingThreshold();
		}

		public void RefreshContactPoints(ref IndexedMatrix trA, ref IndexedMatrix trB)
		{
			int num = GetNumContacts() - 1;
			for (int num2 = num; num2 >= 0; num2--)
			{
				ManifoldPoint manifoldPoint = m_pointCache[num2];
				IndexedVector3 value = trA * manifoldPoint.GetLocalPointA();
				IndexedVector3 value2 = trB * manifoldPoint.GetLocalPointB();
				manifoldPoint.SetPositionWorldOnA(ref value);
				manifoldPoint.SetPositionWorldOnB(ref value2);
				manifoldPoint.SetDistance(IndexedVector3.Dot(manifoldPoint.GetPositionWorldOnA() - manifoldPoint.GetPositionWorldOnB(), manifoldPoint.GetNormalWorldOnB()));
				manifoldPoint.SetLifeTime(manifoldPoint.GetLifeTime() + 1);
				m_pointCache[num2] = manifoldPoint;
			}
			for (int num3 = num; num3 >= 0; num3--)
			{
				ManifoldPoint manifoldPoint2 = m_pointCache[num3];
				if (!ValidContactDistance(manifoldPoint2))
				{
					RemoveContactPoint(num3);
				}
				else
				{
					IndexedVector3 indexedVector = manifoldPoint2.GetPositionWorldOnA() - manifoldPoint2.GetNormalWorldOnB() * manifoldPoint2.GetDistance();
					IndexedVector3 indexedVector2 = manifoldPoint2.GetPositionWorldOnB() - indexedVector;
					float num4 = IndexedVector3.Dot(indexedVector2, indexedVector2);
					if (num4 > GetContactBreakingThreshold() * GetContactBreakingThreshold())
					{
						RemoveContactPoint(num3);
					}
					else if (gContactProcessedCallback != null)
					{
						gContactProcessedCallback.Callback(manifoldPoint2, m_body0, m_body1);
					}
				}
			}
			DebugPersistency();
		}

		public void ClearManifold()
		{
			for (int i = 0; i < m_cachedPoints; i++)
			{
				ClearUserCache(ref m_pointCache[i]);
			}
			m_cachedPoints = 0;
		}

		public int CompareTo(object obj)
		{
			if (obj is PersistentManifold)
			{
				PersistentManifold lhs = (PersistentManifold)obj;
				return getIslandId(this) - getIslandId(lhs);
			}
			throw new ArgumentException("Object is not a PersistentManifold");
		}

		private static int getIslandId(PersistentManifold lhs)
		{
			CollisionObject collisionObject = lhs.GetBody0() as CollisionObject;
			int islandTag = collisionObject.GetIslandTag();
			if (islandTag >= 0)
			{
				return collisionObject.GetIslandTag();
			}
			CollisionObject collisionObject2 = lhs.GetBody1() as CollisionObject;
			return collisionObject2.GetIslandTag();
		}
	}
}
