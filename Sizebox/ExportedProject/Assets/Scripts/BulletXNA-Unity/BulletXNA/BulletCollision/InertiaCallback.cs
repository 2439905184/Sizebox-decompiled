using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class InertiaCallback : IInternalTriangleIndexCallback
	{
		private IndexedBasisMatrix m_sum;

		private IndexedVector3 m_center;

		public virtual bool graphics()
		{
			return false;
		}

		public InertiaCallback(ref IndexedVector3 center)
		{
			m_sum = default(IndexedBasisMatrix);
			m_center = center;
		}

		public virtual void InternalProcessTriangleIndex(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			IndexedBasisMatrix indexedBasisMatrix = default(IndexedBasisMatrix);
			IndexedVector3 indexedVector = triangle[0] - m_center;
			IndexedVector3 b = triangle[1] - m_center;
			IndexedVector3 c = triangle[2] - m_center;
			float num = (0f - Math.Abs(indexedVector.Triple(ref b, ref c))) * (1f / 6f);
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j <= i; j++)
				{
					int i2 = i;
					int j2 = j;
					float value = (indexedBasisMatrix[j, i] = num * (0.1f * (indexedVector[i] * indexedVector[j] + b[i] * b[j] + c[i] * c[j]) + 0.05f * (indexedVector[i] * b[j] + indexedVector[j] * b[i] + indexedVector[i] * c[j] + indexedVector[j] * c[i] + b[i] * c[j] + b[j] * c[i])));
					indexedBasisMatrix[i2, j2] = value;
				}
			}
			float num3 = 0f - indexedBasisMatrix._el0.X;
			float num4 = 0f - indexedBasisMatrix._el1.Y;
			float num5 = 0f - indexedBasisMatrix._el2.Z;
			indexedBasisMatrix[0, 0] = num4 + num5;
			indexedBasisMatrix[1, 1] = num5 + num3;
			indexedBasisMatrix[2, 2] = num3 + num4;
			m_sum._el0 += indexedBasisMatrix._el0;
			m_sum._el1 += indexedBasisMatrix._el1;
			m_sum._el2 += indexedBasisMatrix._el2;
		}

		public IndexedBasisMatrix GetInertia()
		{
			return m_sum;
		}

		public void Cleanup()
		{
		}
	}
}
