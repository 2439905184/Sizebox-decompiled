using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BT_QUANTIZED_BVH_NODE
	{
		public UShortVector3 m_quantizedAabbMin;

		public UShortVector3 m_quantizedAabbMax;

		public int m_escapeIndexOrDataIndex;

		public bool IsLeafNode()
		{
			return m_escapeIndexOrDataIndex >= 0;
		}

		public int GetEscapeIndex()
		{
			return -m_escapeIndexOrDataIndex;
		}

		public void SetEscapeIndex(int index)
		{
			m_escapeIndexOrDataIndex = -index;
		}

		public int GetDataIndex()
		{
			return m_escapeIndexOrDataIndex;
		}

		public void SetDataIndex(int index)
		{
			m_escapeIndexOrDataIndex = index;
		}

		public bool TestQuantizedBoxOverlapp(ref UShortVector3 quantizedMin, ref UShortVector3 quantizedMax)
		{
			if (m_quantizedAabbMin.X > quantizedMax.X || m_quantizedAabbMax.X < quantizedMin.X || m_quantizedAabbMin.Y > quantizedMax.Y || m_quantizedAabbMax.Y < quantizedMin.Y || m_quantizedAabbMin.Z > quantizedMax.Z || m_quantizedAabbMax.Z < quantizedMin.Z)
			{
				return false;
			}
			return true;
		}
	}
}
