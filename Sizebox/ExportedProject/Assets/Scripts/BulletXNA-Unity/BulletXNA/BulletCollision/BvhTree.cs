using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BvhTree
	{
		protected int m_num_nodes;

		protected GIM_BVH_TREE_NODE_ARRAY m_node_array = new GIM_BVH_TREE_NODE_ARRAY();

		protected int SortAndCalcSplittingIndex(GIM_BVH_DATA_ARRAY primitive_boxes, int startIndex, int endIndex, int splitAxis)
		{
			int num = startIndex;
			int num2 = endIndex - startIndex;
			float num3 = 0f;
			IndexedVector3 zero = IndexedVector3.Zero;
			for (int i = startIndex; i < endIndex; i++)
			{
				IndexedVector3 indexedVector = 0.5f * (primitive_boxes[i].m_bound.m_max + primitive_boxes[i].m_bound.m_min);
				zero += indexedVector;
			}
			num3 = (zero * (1f / (float)num2))[splitAxis];
			for (int i = startIndex; i < endIndex; i++)
			{
				if ((0.5f * (primitive_boxes[i].m_bound.m_max + primitive_boxes[i].m_bound.m_min))[splitAxis] > num3)
				{
					primitive_boxes.Swap(i, num);
					num++;
				}
			}
			int num4 = num2 / 3;
			if (num <= startIndex + num4 || num >= endIndex - 1 - num4)
			{
				num = startIndex + (num2 >> 1);
			}
			return num;
		}

		protected int CalcSplittingAxis(GIM_BVH_DATA_ARRAY primitive_boxes, int startIndex, int endIndex)
		{
			IndexedVector3 zero = IndexedVector3.Zero;
			IndexedVector3 a = IndexedVector3.Zero;
			int num = endIndex - startIndex;
			for (int i = startIndex; i < endIndex; i++)
			{
				IndexedVector3 indexedVector = 0.5f * (primitive_boxes[i].m_bound.m_max + primitive_boxes[i].m_bound.m_min);
				zero += indexedVector;
			}
			zero *= 1f / (float)num;
			for (int i = startIndex; i < endIndex; i++)
			{
				IndexedVector3 indexedVector2 = 0.5f * (primitive_boxes[i].m_bound.m_max + primitive_boxes[i].m_bound.m_min);
				IndexedVector3 indexedVector3 = indexedVector2 - zero;
				indexedVector3 *= indexedVector3;
				a += indexedVector3;
			}
			a *= 1f / ((float)num - 1f);
			return MathUtil.MaxAxis(ref a);
		}

		protected void BuildSubTree(GIM_BVH_DATA_ARRAY primitive_boxes, int startIndex, int endIndex)
		{
			int num_nodes = m_num_nodes;
			m_num_nodes++;
			if (endIndex - startIndex == 1)
			{
				SetNodeBound(num_nodes, ref primitive_boxes.GetRawArray()[startIndex].m_bound);
				m_node_array[num_nodes].SetDataIndex(primitive_boxes[startIndex].m_data);
				return;
			}
			int splitAxis = CalcSplittingAxis(primitive_boxes, startIndex, endIndex);
			splitAxis = SortAndCalcSplittingIndex(primitive_boxes, startIndex, endIndex, splitAxis);
			AABB bound = default(AABB);
			bound.Invalidate();
			for (int i = startIndex; i < endIndex; i++)
			{
				bound.Merge(ref primitive_boxes.GetRawArray()[i].m_bound);
			}
			SetNodeBound(num_nodes, ref bound);
			BuildSubTree(primitive_boxes, startIndex, splitAxis);
			BuildSubTree(primitive_boxes, splitAxis, endIndex);
			m_node_array[num_nodes].SetEscapeIndex(m_num_nodes - num_nodes);
		}

		public BvhTree()
		{
			m_num_nodes = 0;
		}

		public void BuildTree(GIM_BVH_DATA_ARRAY primitive_boxes)
		{
			m_num_nodes = 0;
			m_node_array.Capacity = primitive_boxes.Count * 2;
			BuildSubTree(primitive_boxes, 0, primitive_boxes.Count);
		}

		public void ClearNodes()
		{
			m_node_array.Clear();
			m_num_nodes = 0;
		}

		public int GetNodeCount()
		{
			return m_num_nodes;
		}

		public bool IsLeafNode(int nodeindex)
		{
			return m_node_array[nodeindex].IsLeafNode();
		}

		public int GetNodeData(int nodeindex)
		{
			return m_node_array[nodeindex].GetDataIndex();
		}

		public void GetNodeBound(int nodeindex, out AABB bound)
		{
			bound = m_node_array[nodeindex].m_bound;
		}

		public void SetNodeBound(int nodeindex, ref AABB bound)
		{
			m_node_array[nodeindex].m_bound = bound;
		}

		public int GetLeftNode(int nodeindex)
		{
			return nodeindex + 1;
		}

		public int GetRightNode(int nodeindex)
		{
			if (m_node_array[nodeindex + 1].IsLeafNode())
			{
				return nodeindex + 2;
			}
			return nodeindex + 1 + m_node_array[nodeindex + 1].GetEscapeIndex();
		}

		public int GetEscapeNodeIndex(int nodeindex)
		{
			return m_node_array[nodeindex].GetEscapeIndex();
		}
	}
}
