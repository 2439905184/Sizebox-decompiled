using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class CenterCallback : IInternalTriangleIndexCallback
	{
		private bool first;

		private IndexedVector3 reference;

		private IndexedVector3 sum;

		private float volume;

		public CenterCallback()
		{
			first = true;
			reference = default(IndexedVector3);
			sum = default(IndexedVector3);
			volume = 0f;
		}

		public virtual bool graphics()
		{
			return false;
		}

		public virtual void InternalProcessTriangleIndex(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			if (first)
			{
				reference = triangle[0];
				first = false;
				return;
			}
			IndexedVector3 indexedVector = triangle[0] - reference;
			IndexedVector3 b = triangle[1] - reference;
			IndexedVector3 c = triangle[2] - reference;
			float num = Math.Abs(indexedVector.Triple(ref b, ref c));
			sum += 0.25f * num * (triangle[0] + triangle[1] + triangle[2] + reference);
			volume += num;
		}

		public IndexedVector3 GetCenter()
		{
			if (!(volume > 0f))
			{
				return reference;
			}
			return sum / volume;
		}

		public float GetVolume()
		{
			return volume * (1f / 6f);
		}

		public void Cleanup()
		{
		}
	}
}
