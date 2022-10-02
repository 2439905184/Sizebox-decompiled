using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class TriangleShapeEx : TriangleShape
	{
		public TriangleShapeEx()
			: base(IndexedVector3.Zero, IndexedVector3.Zero, IndexedVector3.Zero)
		{
		}

		public TriangleShapeEx(ref IndexedVector3 p0, ref IndexedVector3 p1, ref IndexedVector3 p2)
			: base(ref p0, ref p1, ref p2)
		{
		}

		public TriangleShapeEx(TriangleShapeEx other)
			: base(ref other.m_vertices1[0], ref other.m_vertices1[1], ref other.m_vertices1[2])
		{
		}

		public virtual void GetAabb(ref IndexedMatrix t, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			IndexedVector3 V = t * m_vertices1[0];
			IndexedVector3 V2 = t * m_vertices1[1];
			IndexedVector3 V3 = t * m_vertices1[2];
			AABB aABB = new AABB(ref V, ref V2, ref V3, m_collisionMargin);
			aabbMin = aABB.m_min;
			aabbMax = aABB.m_max;
		}

		public void ApplyTransform(ref IndexedMatrix t)
		{
			IndexedVector3.Transform(m_vertices1, ref t, m_vertices1);
		}

		public void BuildTriPlane(out IndexedVector4 plane)
		{
			IndexedVector3 indexedVector = IndexedVector3.Cross(m_vertices1[1] - m_vertices1[0], m_vertices1[2] - m_vertices1[0]);
			indexedVector.Normalize();
			plane = new IndexedVector4(indexedVector, IndexedVector3.Dot(m_vertices1[0], indexedVector));
		}

		public bool OverlapTestConservative(TriangleShapeEx other)
		{
			float num = GetMargin() + other.GetMargin();
			IndexedVector4 plane;
			BuildTriPlane(out plane);
			IndexedVector4 plane2;
			other.BuildTriPlane(out plane2);
			float num2 = ClipPolygon.DistancePointPlane(ref plane, ref other.m_vertices1[0]) - num;
			float num3 = ClipPolygon.DistancePointPlane(ref plane, ref other.m_vertices1[1]) - num;
			float num4 = ClipPolygon.DistancePointPlane(ref plane, ref other.m_vertices1[2]) - num;
			if (num2 > 0f && num3 > 0f && num4 > 0f)
			{
				return false;
			}
			num2 = ClipPolygon.DistancePointPlane(ref plane2, ref m_vertices1[0]) - num;
			num3 = ClipPolygon.DistancePointPlane(ref plane2, ref m_vertices1[1]) - num;
			num4 = ClipPolygon.DistancePointPlane(ref plane2, ref m_vertices1[2]) - num;
			if (num2 > 0f && num3 > 0f && num4 > 0f)
			{
				return false;
			}
			return true;
		}
	}
}
