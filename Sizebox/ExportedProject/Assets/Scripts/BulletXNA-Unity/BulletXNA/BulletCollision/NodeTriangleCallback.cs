using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class NodeTriangleCallback : IInternalTriangleIndexCallback
	{
		private IList<OptimizedBvhNode> m_triangleNodes;

		public virtual bool graphics()
		{
			return false;
		}

		public NodeTriangleCallback(ObjectArray<OptimizedBvhNode> triangleNodes)
		{
			m_triangleNodes = triangleNodes;
		}

		public virtual void InternalProcessTriangleIndex(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			OptimizedBvhNode optimizedBvhNode = new OptimizedBvhNode();
			IndexedVector3 output = MathUtil.MAX_VECTOR;
			IndexedVector3 output2 = MathUtil.MIN_VECTOR;
			MathUtil.VectorMax(ref triangle[0], ref output2);
			MathUtil.VectorMin(ref triangle[0], ref output);
			MathUtil.VectorMax(ref triangle[1], ref output2);
			MathUtil.VectorMin(ref triangle[1], ref output);
			MathUtil.VectorMax(ref triangle[2], ref output2);
			MathUtil.VectorMin(ref triangle[2], ref output);
			optimizedBvhNode.m_aabbMinOrg = output;
			optimizedBvhNode.m_aabbMaxOrg = output2;
			optimizedBvhNode.m_escapeIndex = -1;
			optimizedBvhNode.m_subPart = partId;
			optimizedBvhNode.m_triangleIndex = triangleIndex;
			m_triangleNodes.Add(optimizedBvhNode);
		}

		public virtual void Cleanup()
		{
		}
	}
}
