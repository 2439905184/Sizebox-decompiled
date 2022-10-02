using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class TriangleRaycastCallback : ITriangleCallback
	{
		public IndexedVector3 m_from;

		public IndexedVector3 m_to;

		public EFlags m_flags;

		public float m_hitFraction;

		public TriangleRaycastCallback()
		{
		}

		public TriangleRaycastCallback(ref IndexedVector3 from, ref IndexedVector3 to, EFlags flags)
		{
			m_from = from;
			m_to = to;
			m_flags = flags;
			m_hitFraction = 1f;
		}

		public virtual void Initialize(ref IndexedVector3 from, ref IndexedVector3 to, EFlags flags)
		{
			m_from = from;
			m_to = to;
			m_flags = flags;
			m_hitFraction = 1f;
		}

		public virtual bool graphics()
		{
			return false;
		}

		public virtual void ProcessTriangle(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			IndexedVector3 indexedVector = triangle[1] - triangle[0];
			IndexedVector3 v = triangle[2] - triangle[0];
			IndexedVector3 b = indexedVector.Cross(ref v);
			float num = IndexedVector3.Dot(ref triangle[0], ref b);
			float num2 = IndexedVector3.Dot(ref b, ref m_from);
			num2 -= num;
			float num3 = IndexedVector3.Dot(ref b, ref m_to);
			num3 -= num;
			if (num2 * num3 >= 0f || ((m_flags & EFlags.kF_FilterBackfaces) != 0 && num2 > 0f))
			{
				return;
			}
			float num4 = num2 - num3;
			float num5 = num2 / num4;
			if (!(num5 < m_hitFraction))
			{
				return;
			}
			float num6 = b.LengthSquared();
			num6 *= -0.0001f;
			IndexedVector3 indexedVector2 = MathUtil.Interpolate3(ref m_from, ref m_to, num5);
			IndexedVector3 v2 = triangle[0] - indexedVector2;
			IndexedVector3 v3 = triangle[1] - indexedVector2;
			IndexedVector3 a = v2.Cross(ref v3);
			if (!(IndexedVector3.Dot(ref a, ref b) >= num6))
			{
				return;
			}
			IndexedVector3 v4 = triangle[2] - indexedVector2;
			IndexedVector3 a2 = v3.Cross(ref v4);
			if (!(IndexedVector3.Dot(ref a2, ref b) >= num6))
			{
				return;
			}
			IndexedVector3 a3 = v4.Cross(ref v2);
			if (IndexedVector3.Dot(ref a3, ref b) >= num6)
			{
				b.Normalize();
				if ((m_flags & EFlags.kF_KeepUnflippedNormal) == 0 && num2 <= 0f)
				{
					IndexedVector3 hitNormalLocal = -b;
					m_hitFraction = ReportHit(ref hitNormalLocal, num5, partId, triangleIndex);
				}
				else
				{
					m_hitFraction = ReportHit(ref b, num5, partId, triangleIndex);
				}
			}
		}

		public abstract float ReportHit(ref IndexedVector3 hitNormalLocal, float hitFraction, int partId, int triangleIndex);

		public virtual void Cleanup()
		{
		}
	}
}
