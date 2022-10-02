using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class QuantizedNodeTriangleCallback : IInternalTriangleIndexCallback
	{
		public const int MAX_NUM_PARTS_IN_BITS = 10;

		private IList<QuantizedBvhNode> m_triangleNodes;

		private QuantizedBvh m_optimizedTree;

		public virtual bool graphics()
		{
			return false;
		}

		public QuantizedNodeTriangleCallback(ObjectArray<QuantizedBvhNode> triangleNodes, QuantizedBvh tree)
		{
			m_triangleNodes = triangleNodes;
			m_optimizedTree = tree;
		}

		public virtual void InternalProcessTriangleIndex(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			QuantizedBvhNode quantizedBvhNode = new QuantizedBvhNode();
			IndexedVector3 output = MathUtil.MAX_VECTOR;
			IndexedVector3 output2 = MathUtil.MIN_VECTOR;
			MathUtil.VectorMin(ref triangle[0], ref output);
			MathUtil.VectorMax(ref triangle[0], ref output2);
			MathUtil.VectorMin(ref triangle[1], ref output);
			MathUtil.VectorMax(ref triangle[1], ref output2);
			MathUtil.VectorMin(ref triangle[2], ref output);
			MathUtil.VectorMax(ref triangle[2], ref output2);
			float num = 0.002f;
			float num2 = 0.001f;
			if (output2.X - output.X < num)
			{
				output2.X += num2;
				output.X -= num2;
			}
			if (output2.Y - output.Y < num)
			{
				output2.Y += num2;
				output.Y -= num2;
			}
			if (output2.Z - output.Z < num)
			{
				output2.Z += num2;
				output.Z -= num2;
			}
			m_optimizedTree.Quantize(out quantizedBvhNode.m_quantizedAabbMin, ref output, false);
			m_optimizedTree.Quantize(out quantizedBvhNode.m_quantizedAabbMax, ref output2, true);
			quantizedBvhNode.m_escapeIndexOrTriangleIndex = (partId << 21) | triangleIndex;
			m_triangleNodes.Add(quantizedBvhNode);
		}

		public virtual void Cleanup()
		{
		}
	}
}
