using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ConeShape : ConvexInternalShape
	{
		protected float m_sinAngle;

		protected float m_radius;

		protected float m_height;

		protected int[] m_coneIndices = new int[3];

		public ConeShape(float radius, float height)
		{
			m_radius = radius;
			m_height = height;
			m_shapeType = BroadphaseNativeTypes.CONE_SHAPE_PROXYTYPE;
			SetConeUpIndex(1);
			m_sinAngle = (float)((double)m_radius / Math.Sqrt(m_radius * m_radius + m_height * m_height));
		}

		public override IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			return ConeLocalSupport(ref vec);
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			return ConeLocalSupport(ref vec);
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			for (int i = 0; i < numVectors; i++)
			{
				IndexedVector3 v = vectors[i];
				supportVerticesOut[i] = new IndexedVector4(ConeLocalSupport(ref v), 0f);
			}
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			int i = m_coneIndices[1];
			int i2 = m_coneIndices[0];
			int i3 = m_coneIndices[2];
			m_height *= scaling[i] / m_localScaling[i];
			m_radius *= (scaling[i2] / m_localScaling[i2] + scaling[i3] / m_localScaling[i3]) / 2f;
			m_sinAngle = m_radius / (float)Math.Sqrt(m_radius * m_radius + m_height * m_height);
			base.SetLocalScaling(ref scaling);
		}

		public float GetRadius()
		{
			return m_radius;
		}

		public float GetHeight()
		{
			return m_height;
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			IndexedMatrix t = IndexedMatrix.Identity;
			IndexedVector3 aabbMin;
			IndexedVector3 aabbMax;
			GetAabb(ref t, out aabbMin, out aabbMax);
			IndexedVector3 indexedVector = (aabbMax - aabbMin) * 0.5f;
			float margin = GetMargin();
			float num = 2f * (indexedVector.X + margin);
			float num2 = 2f * (indexedVector.Y + margin);
			float num3 = 2f * (indexedVector.Z + margin);
			float num4 = num * num;
			float num5 = num2 * num2;
			float num6 = num3 * num3;
			float num7 = mass * 0.08333333f;
			inertia = num7 * new IndexedVector3(num5 + num6, num4 + num6, num4 + num5);
		}

		public override string GetName()
		{
			return "Cone";
		}

		public void SetConeUpIndex(int upIndex)
		{
			switch (upIndex)
			{
			case 0:
				m_coneIndices[0] = 1;
				m_coneIndices[1] = 0;
				m_coneIndices[2] = 2;
				break;
			case 1:
				m_coneIndices[0] = 0;
				m_coneIndices[1] = 1;
				m_coneIndices[2] = 2;
				break;
			case 2:
				m_coneIndices[0] = 0;
				m_coneIndices[1] = 2;
				m_coneIndices[2] = 1;
				break;
			}
		}

		public int GetConeUpIndex()
		{
			return m_coneIndices[1];
		}

		protected IndexedVector3 ConeLocalSupport(ref IndexedVector3 v)
		{
			float num = m_height * 0.5f;
			if (v[m_coneIndices[1]] > v.Length() * m_sinAngle)
			{
				IndexedVector3 result = default(IndexedVector3);
				result[m_coneIndices[0]] = 0f;
				result[m_coneIndices[1]] = num;
				result[m_coneIndices[2]] = 0f;
				return result;
			}
			float num2 = (float)Math.Sqrt(v[m_coneIndices[0]] * v[m_coneIndices[0]] + v[m_coneIndices[2]] * v[m_coneIndices[2]]);
			if (num2 > 1.1920929E-07f)
			{
				float num3 = m_radius / num2;
				IndexedVector3 result2 = default(IndexedVector3);
				result2[m_coneIndices[0]] = v[m_coneIndices[0]] * num3;
				result2[m_coneIndices[1]] = 0f - num;
				result2[m_coneIndices[2]] = v[m_coneIndices[2]] * num3;
				return result2;
			}
			IndexedVector3 result3 = default(IndexedVector3);
			result3[m_coneIndices[0]] = 0f;
			result3[m_coneIndices[1]] = 0f - num;
			result3[m_coneIndices[2]] = 0f;
			return result3;
		}
	}
}
