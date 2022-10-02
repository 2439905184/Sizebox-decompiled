using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CapsuleShape : ConvexInternalShape
	{
		protected int m_upAxis;

		protected CapsuleShape()
		{
			m_shapeType = BroadphaseNativeTypes.CAPSULE_SHAPE_PROXYTYPE;
		}

		public CapsuleShape(float radius, float height)
			: this()
		{
			m_upAxis = 1;
			m_implicitShapeDimensions = new IndexedVector3(radius, 0.5f * height, radius);
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			IndexedMatrix identity = IndexedMatrix.Identity;
			float radius = GetRadius();
			IndexedVector3 indexedVector = new IndexedVector3(radius);
			float num = indexedVector[GetUpAxis()];
			indexedVector[GetUpAxis()] = num + GetHalfHeight();
			float num2 = 2f * (indexedVector.X + 0.04f);
			float num3 = 2f * (indexedVector.Y + 0.04f);
			float num4 = 2f * (indexedVector.Z + 0.04f);
			float num5 = num2 * num2;
			float num6 = num3 * num3;
			float num7 = num4 * num4;
			float num8 = mass * 0.08333333f;
			inertia = num8 * new IndexedVector3(num6 + num7, num5 + num7, num5 + num6);
		}

		public override void SetMargin(float collisionMargin)
		{
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			IndexedVector3 indexedVector2 = m_implicitShapeDimensions + indexedVector;
			base.SetMargin(collisionMargin);
			IndexedVector3 indexedVector3 = new IndexedVector3(GetMargin());
			m_implicitShapeDimensions = indexedVector2 - indexedVector3;
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			IndexedVector3 indexedVector2 = m_implicitShapeDimensions + indexedVector;
			IndexedVector3 indexedVector3 = indexedVector2 / m_localScaling;
			base.SetLocalScaling(ref scaling);
			m_implicitShapeDimensions = indexedVector3 * m_localScaling - indexedVector;
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec0)
		{
			IndexedVector3 result = IndexedVector3.Zero;
			float num = float.MinValue;
			IndexedVector3 indexedVector = vec0;
			float num2 = indexedVector.LengthSquared();
			if (num2 < 0.0001f)
			{
				indexedVector = new IndexedVector3(1f, 0f, 0f);
			}
			else
			{
				float num3 = 1f / (float)Math.Sqrt(num2);
				indexedVector *= num3;
			}
			float radius = GetRadius();
			IndexedVector3 zero = IndexedVector3.Zero;
			zero[GetUpAxis()] = GetHalfHeight();
			IndexedVector3 v = zero + indexedVector * radius - indexedVector * GetMargin();
			float num4 = indexedVector.Dot(ref v);
			if (num4 > num)
			{
				num = num4;
				result = v;
			}
			IndexedVector3 zero2 = IndexedVector3.Zero;
			zero2[GetUpAxis()] = 0f - GetHalfHeight();
			v = zero2 + indexedVector * radius - indexedVector * GetMargin();
			num4 = indexedVector.Dot(ref v);
			if (num4 > num)
			{
				num = num4;
				result = v;
			}
			return result;
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			float radius = GetRadius();
			for (int i = 0; i < numVectors; i++)
			{
				float num = float.MinValue;
				IndexedVector3 indexedVector = vectors[i];
				float num2 = 0f;
				IndexedVector3 zero = IndexedVector3.Zero;
				zero[GetUpAxis()] = GetHalfHeight();
				IndexedVector3 v = zero + indexedVector * radius - indexedVector * GetMargin();
				num2 = indexedVector.Dot(ref v);
				if (num2 > num)
				{
					num = num2;
					supportVerticesOut[i] = new IndexedVector4(v, 0f);
				}
				IndexedVector3 zero2 = IndexedVector3.Zero;
				zero2[GetUpAxis()] = 0f - GetHalfHeight();
				v = zero2 + indexedVector * radius - indexedVector * GetMargin();
				num2 = indexedVector.Dot(ref v);
				if (num2 > num)
				{
					num = num2;
					supportVerticesOut[i] = new IndexedVector4(v, 0f);
				}
			}
		}

		public override void GetAabb(ref IndexedMatrix trans, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			IndexedVector3 v = new IndexedVector3(GetRadius());
			v[m_upAxis] = GetRadius() + GetHalfHeight();
			v += new IndexedVector3(GetMargin());
			IndexedBasisMatrix indexedBasisMatrix = trans._basis.Absolute();
			IndexedVector3 origin = trans._origin;
			IndexedVector3 indexedVector = new IndexedVector3(indexedBasisMatrix._el0.Dot(ref v), indexedBasisMatrix._el1.Dot(ref v), indexedBasisMatrix._el2.Dot(ref v));
			aabbMin = origin - indexedVector;
			aabbMax = origin + indexedVector;
		}

		public override string GetName()
		{
			return "CapsuleShape";
		}

		public int GetUpAxis()
		{
			return m_upAxis;
		}

		public float GetRadius()
		{
			int i = (m_upAxis + 2) % 3;
			return m_implicitShapeDimensions[i];
		}

		public float GetHalfHeight()
		{
			return m_implicitShapeDimensions[m_upAxis];
		}
	}
}
