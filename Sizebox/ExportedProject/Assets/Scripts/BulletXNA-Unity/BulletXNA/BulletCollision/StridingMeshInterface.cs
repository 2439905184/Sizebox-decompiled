using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class StridingMeshInterface
	{
		protected IndexedVector3 m_scaling;

		public StridingMeshInterface()
		{
			m_scaling = new IndexedVector3(1f);
		}

		public virtual void Cleanup()
		{
		}

		public virtual void InternalProcessAllTriangles(IInternalTriangleIndexCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			int num = 0;
			int numSubParts = GetNumSubParts();
			object vertexbase = null;
			object indexbase = null;
			int indexstride = 3;
			PHY_ScalarType type = PHY_ScalarType.PHY_FLOAT;
			PHY_ScalarType indicestype = PHY_ScalarType.PHY_INTEGER;
			int stride = 0;
			int numverts = 0;
			int numfaces = 0;
			IndexedVector3[] array = new IndexedVector3[3];
			IndexedVector3 scaling = GetScaling();
			for (int i = 0; i < numSubParts; i++)
			{
				GetLockedReadOnlyVertexIndexBase(out vertexbase, out numverts, out type, out stride, out indexbase, out indexstride, out numfaces, out indicestype, i);
				num += numfaces * 3;
				PHY_ScalarType pHY_ScalarType = indicestype;
				if (pHY_ScalarType == PHY_ScalarType.PHY_INTEGER)
				{
					int[] rawArray = ((ObjectArray<int>)indexbase).GetRawArray();
					if (vertexbase is ObjectArray<IndexedVector3>)
					{
						IndexedVector3[] rawArray2 = (vertexbase as ObjectArray<IndexedVector3>).GetRawArray();
						for (int j = 0; j < numfaces; j++)
						{
							int num2 = j * indexstride;
							int num3 = rawArray[num2];
							int num4 = rawArray[num2 + 1];
							int num5 = rawArray[num2 + 2];
							array[0] = rawArray2[num3] * scaling;
							array[1] = rawArray2[num4] * scaling;
							array[2] = rawArray2[num5] * scaling;
							callback.InternalProcessTriangleIndex(array, i, j);
						}
					}
					else if (vertexbase is ObjectArray<float>)
					{
						float[] rawArray3 = (vertexbase as ObjectArray<float>).GetRawArray();
						for (int k = 0; k < numfaces; k++)
						{
							int num6 = k * indexstride;
							int num7 = rawArray[num6];
							array[0] = new IndexedVector3(rawArray3[num7 * stride], rawArray3[num7 * stride + 1], rawArray3[num7 * stride + 2]) * scaling;
							num7 = rawArray[num6 + 1];
							array[1] = new IndexedVector3(rawArray3[num7 * stride], rawArray3[num7 * stride + 1], rawArray3[num7 * stride + 2]) * scaling;
							num7 = rawArray[num6 + 2];
							array[2] = new IndexedVector3(rawArray3[num7 * stride], rawArray3[num7 * stride + 1], rawArray3[num7 * stride + 2]) * scaling;
							callback.InternalProcessTriangleIndex(array, i, k);
						}
					}
				}
				UnLockReadOnlyVertexBase(i);
			}
		}

		public void CalculateAabbBruteForce(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			AabbCalculationCallback aabbCalculationCallback = new AabbCalculationCallback();
			aabbMin = MathUtil.MIN_VECTOR;
			aabbMax = MathUtil.MAX_VECTOR;
			InternalProcessAllTriangles(aabbCalculationCallback, ref aabbMin, ref aabbMax);
			aabbMin = aabbCalculationCallback.m_aabbMin;
			aabbMax = aabbCalculationCallback.m_aabbMax;
		}

		public abstract void GetLockedVertexIndexBase(out object vertexbase, out int numverts, out PHY_ScalarType type, out int stride, out object indexbase, out int indexstride, out int numfaces, out PHY_ScalarType indicestype, int subpart);

		public abstract void GetLockedReadOnlyVertexIndexBase(out object vertexbase, out int numverts, out PHY_ScalarType type, out int stride, out object indexbase, out int indexstride, out int numfaces, out PHY_ScalarType indicestype, int subpart);

		public abstract void UnLockVertexBase(int subpart);

		public abstract void UnLockReadOnlyVertexBase(int subpart);

		public abstract int GetNumSubParts();

		public abstract void PreallocateVertices(int numverts);

		public abstract void PreallocateIndices(int numindices);

		public virtual bool HasPremadeAabb()
		{
			return false;
		}

		public virtual void SetPremadeAabb(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
		}

		public virtual void GetPremadeAabb(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			aabbMin = IndexedVector3.Zero;
			aabbMax = IndexedVector3.Zero;
		}

		public IndexedVector3 GetScaling()
		{
			return m_scaling;
		}

		public void SetScaling(ref IndexedVector3 scaling)
		{
			m_scaling = scaling;
		}
	}
}
