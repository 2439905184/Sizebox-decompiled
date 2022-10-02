using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CylinderShape : ConvexInternalShape
	{
		protected int m_upAxis;

		public CylinderShape(IndexedVector3 halfExtents)
			: this(ref halfExtents)
		{
		}

		public CylinderShape(ref IndexedVector3 halfExtents)
		{
			m_upAxis = 1;
			SetSafeMargin(ref halfExtents);
			m_shapeType = BroadphaseNativeTypes.CYLINDER_SHAPE_PROXYTYPE;
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			m_implicitShapeDimensions = halfExtents * m_localScaling - indexedVector;
		}

		public virtual IndexedVector3 GetHalfExtentsWithMargin()
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			return halfExtentsWithoutMargin + indexedVector;
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			IndexedVector3 halfExtentsWithMargin = GetHalfExtentsWithMargin();
			float num = mass / 12f;
			float num2 = mass / 4f;
			float num3 = mass / 2f;
			int i;
			int i2;
			switch (m_upAxis)
			{
			case 0:
				i = 1;
				i2 = 0;
				break;
			case 2:
				i = 0;
				i2 = 2;
				break;
			default:
				i = 0;
				i2 = 1;
				break;
			}
			float num4 = halfExtentsWithMargin[i] * halfExtentsWithMargin[i];
			float num5 = 4f * halfExtentsWithMargin[i2] * halfExtentsWithMargin[i2];
			float num6 = num * num5 + num2 * num4;
			float num7 = num3 * num4;
			switch (m_upAxis)
			{
			case 0:
				inertia = new IndexedVector3(num7, num6, num6);
				break;
			case 2:
				inertia = new IndexedVector3(num6, num6, num7);
				break;
			default:
				inertia = new IndexedVector3(num6, num7, num6);
				break;
			}
		}

		public override void SetMargin(float collisionMargin)
		{
			IndexedVector3 indexedVector = new IndexedVector3(GetMargin());
			IndexedVector3 indexedVector2 = m_implicitShapeDimensions + indexedVector;
			base.SetMargin(collisionMargin);
			IndexedVector3 indexedVector3 = new IndexedVector3(GetMargin());
			m_implicitShapeDimensions = indexedVector2 - indexedVector3;
		}

		public IndexedVector3 GetHalfExtentsWithoutMargin()
		{
			return m_implicitShapeDimensions;
		}

		public override void GetAabb(ref IndexedMatrix trans, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			AabbUtil2.TransformAabb(GetHalfExtentsWithoutMargin(), GetMargin(), ref trans, out aabbMin, out aabbMax);
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec)
		{
			return CylinderLocalSupportX(GetHalfExtentsWithoutMargin(), vec);
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			IndexedVector3 halfExtentsWithoutMargin = GetHalfExtentsWithoutMargin();
			for (int i = 0; i < numVectors; i++)
			{
				supportVerticesOut[i] = new IndexedVector4(CylinderLocalSupportY(halfExtentsWithoutMargin, vectors[i]), 0f);
			}
		}

		public override IndexedVector3 LocalGetSupportingVertex(ref IndexedVector3 vec)
		{
			IndexedVector3 result = LocalGetSupportingVertexWithoutMargin(ref vec);
			if (GetMargin() != 0f)
			{
				IndexedVector3 indexedVector = vec;
				if (indexedVector.LengthSquared() < 1.4210855E-14f)
				{
					indexedVector = new IndexedVector3(-1f);
				}
				indexedVector.Normalize();
				result += GetMargin() * indexedVector;
			}
			return result;
		}

		public int GetUpAxis()
		{
			return m_upAxis;
		}

		public virtual float GetRadius()
		{
			return GetHalfExtentsWithMargin().X;
		}

		public override string GetName()
		{
			return "CylinderY";
		}

		public IndexedVector3 CylinderLocalSupportX(IndexedVector3 halfExtents, IndexedVector3 v)
		{
			return CylinderLocalSupportX(ref halfExtents, ref v);
		}

		public IndexedVector3 CylinderLocalSupportX(ref IndexedVector3 halfExtents, ref IndexedVector3 v)
		{
			return CylinderLocalSupport(ref halfExtents, ref v, 0, 1, 0, 2);
		}

		public IndexedVector3 CylinderLocalSupportY(IndexedVector3 halfExtents, IndexedVector3 v)
		{
			return CylinderLocalSupportY(ref halfExtents, ref v);
		}

		public IndexedVector3 CylinderLocalSupportY(ref IndexedVector3 halfExtents, ref IndexedVector3 v)
		{
			return CylinderLocalSupport(ref halfExtents, ref v, 1, 0, 1, 2);
		}

		public IndexedVector3 CylinderLocalSupportZ(IndexedVector3 halfExtents, IndexedVector3 v)
		{
			return CylinderLocalSupportZ(ref halfExtents, ref v);
		}

		public IndexedVector3 CylinderLocalSupportZ(ref IndexedVector3 halfExtents, ref IndexedVector3 v)
		{
			return CylinderLocalSupport(ref halfExtents, ref v, 2, 0, 2, 1);
		}

		private IndexedVector3 CylinderLocalSupport(ref IndexedVector3 halfExtents, ref IndexedVector3 v, int cylinderUpAxis, int XX, int YY, int ZZ)
		{
			float num = halfExtents[XX];
			float num2 = halfExtents[cylinderUpAxis];
			IndexedVector3 result = default(IndexedVector3);
			float num3 = 1f;
			float num4 = v[XX];
			float num7 = v[YY];
			float num5 = v[ZZ];
			float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
			if ((double)num6 != 0.0)
			{
				num3 = num / num6;
				result[XX] = v[XX] * num3;
				result[YY] = ((v[YY] < 0f) ? (0f - num2) : num2);
				result[ZZ] = v[ZZ] * num3;
				return result;
			}
			result[XX] = num;
			result[YY] = ((v[YY] < 0f) ? (0f - num2) : num2);
			result[ZZ] = 0f;
			return result;
		}
	}
}
