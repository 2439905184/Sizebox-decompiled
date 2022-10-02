using System;
using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class MultiSphereShape : ConvexInternalAabbCachingShape
	{
		private IList<IndexedVector3> m_localPositionArray = new List<IndexedVector3>();

		private IList<float> m_radiArray = new List<float>();

		private IndexedVector3 m_inertiaHalfExtents;

		public MultiSphereShape(IList<IndexedVector3> positions, IList<float> radi, int numSpheres)
		{
			m_shapeType = BroadphaseNativeTypes.MULTI_SPHERE_SHAPE_PROXYTYPE;
			for (int i = 0; i < numSpheres; i++)
			{
				m_localPositionArray.Add(positions[i]);
				m_radiArray.Add(radi[i]);
			}
			RecalcLocalAabb();
		}

		public override void CalculateLocalInertia(float mass, out IndexedVector3 inertia)
		{
			IndexedVector3 aabbMin;
			IndexedVector3 aabbMax;
			GetCachedLocalAabb(out aabbMin, out aabbMax);
			IndexedVector3 indexedVector = (aabbMax - aabbMin) * 0.5f;
			float num = 2f * indexedVector.X;
			float num2 = 2f * indexedVector.Y;
			float num3 = 2f * indexedVector.Z;
			inertia = new IndexedVector3(mass / 12f * (num2 * num2 + num3 * num3), mass / 12f * (num * num + num3 * num3), mass / 12f * (num * num + num2 * num2));
		}

		public override IndexedVector3 LocalGetSupportingVertexWithoutMargin(ref IndexedVector3 vec0)
		{
			IndexedVector3 result = IndexedVector3.Zero;
			float num = float.MinValue;
			IndexedVector3 indexedVector = vec0;
			float num2 = indexedVector.LengthSquared();
			if (num2 < 1.4210855E-14f)
			{
				indexedVector = new IndexedVector3(1f, 0f, 0f);
			}
			else
			{
				float num3 = 1f / (float)Math.Sqrt(num2);
				indexedVector *= num3;
			}
			IndexedVector3 zero = IndexedVector3.Zero;
			int count = m_localPositionArray.Count;
			for (int i = 0; i < count; i++)
			{
				IndexedVector3 indexedVector2 = m_localPositionArray[i];
				float num4 = m_radiArray[i];
				zero = indexedVector2 + indexedVector * m_localScaling * num4 - indexedVector * GetMargin();
				float num5 = IndexedVector3.Dot(indexedVector, zero);
				if (num5 > num)
				{
					num = num5;
					result = zero;
				}
			}
			return result;
		}

		public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(IndexedVector3[] vectors, IndexedVector4[] supportVerticesOut, int numVectors)
		{
			for (int i = 0; i < numVectors; i++)
			{
				float num = float.MinValue;
				IndexedVector3 indexedVector = vectors[i];
				IndexedVector3 zero = IndexedVector3.Zero;
				int count = m_localPositionArray.Count;
				for (int j = 0; j < count; j++)
				{
					IndexedVector3 indexedVector2 = m_localPositionArray[j];
					float num2 = m_radiArray[j];
					zero = indexedVector2 + indexedVector * m_localScaling * num2 - indexedVector * GetMargin();
					float num3 = IndexedVector3.Dot(indexedVector, zero);
					if (num3 > num)
					{
						num = num3;
						supportVerticesOut[i] = new IndexedVector4(zero, 0f);
					}
				}
			}
		}

		public int GetSphereCount()
		{
			return m_localPositionArray.Count;
		}

		public IndexedVector3 GetSpherePosition(int index)
		{
			return m_localPositionArray[index];
		}

		public float GetSphereRadius(int index)
		{
			return m_radiArray[index];
		}

		public override string GetName()
		{
			return "MultiSphere";
		}
	}
}
