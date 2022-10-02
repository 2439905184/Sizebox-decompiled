using System;

namespace BulletXNA.BulletCollision
{
	public class BroadphasePair : IComparable
	{
		public int m_index;

		public BroadphaseProxy m_pProxy0;

		public BroadphaseProxy m_pProxy1;

		public CollisionAlgorithm m_algorithm;

		public object m_internalInfo1;

		public int m_internalTmpValue;

		public BroadphasePair()
		{
			m_pProxy0 = null;
			m_pProxy1 = null;
			m_algorithm = null;
			m_internalInfo1 = null;
		}

		public BroadphasePair(ref BroadphasePair other)
		{
			m_pProxy0 = other.m_pProxy0;
			m_pProxy1 = other.m_pProxy1;
			m_algorithm = other.m_algorithm;
			m_internalInfo1 = other.m_internalInfo1;
		}

		public BroadphasePair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			if (proxy0.GetUid() < proxy1.GetUid())
			{
				m_pProxy0 = proxy0;
				m_pProxy1 = proxy1;
			}
			else
			{
				m_pProxy0 = proxy1;
				m_pProxy1 = proxy0;
			}
			m_algorithm = null;
			m_internalInfo1 = null;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is BroadphasePair)
			{
				BroadphasePair broadphasePair = (BroadphasePair)obj;
				if (m_pProxy0 == broadphasePair.m_pProxy0)
				{
					return m_pProxy1 == broadphasePair.m_pProxy1;
				}
				return false;
			}
			return false;
		}

		public static bool IsLessThen(BroadphasePair a, BroadphasePair b)
		{
			int num = ((a.m_pProxy0 != null) ? a.m_pProxy0.GetUid() : (-1));
			int num2 = ((b.m_pProxy0 != null) ? b.m_pProxy0.GetUid() : (-1));
			if (num > num2)
			{
				return true;
			}
			int num3 = ((a.m_pProxy1 != null) ? a.m_pProxy1.GetUid() : (-1));
			int num4 = ((b.m_pProxy1 != null) ? b.m_pProxy1.GetUid() : (-1));
			int num5 = ((a.m_algorithm != null) ? a.m_algorithm.colAgorithmId : 0);
			int num6 = ((b.m_algorithm != null) ? b.m_algorithm.colAgorithmId : 0);
			if (a.m_pProxy0 != b.m_pProxy0 || num3 <= num4)
			{
				if (a.m_pProxy0 == b.m_pProxy0 && a.m_pProxy1 == b.m_pProxy1)
				{
					return num5 > num6;
				}
				return false;
			}
			return true;
		}

		public int CompareTo(object obj)
		{
			if (!IsLessThen(this, (BroadphasePair)obj))
			{
				return 1;
			}
			return -1;
		}
	}
}
