using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BvhSubtreeInfo
	{
		public UShortVector3 m_quantizedAabbMin;

		public UShortVector3 m_quantizedAabbMax;

		public int m_rootNodeIndex;

		public int m_subtreeSize;

		public void SetAabbFromQuantizeNode(QuantizedBvhNode quantizedNode)
		{
			m_quantizedAabbMin = quantizedNode.m_quantizedAabbMin;
			m_quantizedAabbMax = quantizedNode.m_quantizedAabbMax;
		}
	}
}
