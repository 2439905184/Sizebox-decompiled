using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class QuantizedBvhTree
	{
		protected int m_num_nodes;

		protected GIM_QUANTIZED_BVH_NODE_ARRAY m_node_array = new GIM_QUANTIZED_BVH_NODE_ARRAY();

		protected AABB m_global_bound;

		protected IndexedVector3 m_bvhQuantization;

		protected void CalcQuantization(GIM_BVH_DATA_ARRAY primitive_boxes)
		{
			CalcQuantization(primitive_boxes, 1f);
		}

		protected void CalcQuantization(GIM_BVH_DATA_ARRAY primitive_boxes, float boundMargin)
		{
			AABB aABB = default(AABB);
			aABB.Invalidate();
			int count = primitive_boxes.Count;
			for (int i = 0; i < count; i++)
			{
				aABB.Merge(ref primitive_boxes.GetRawArray()[i].m_bound);
			}
			GImpactQuantization.CalcQuantizationParameters(out m_global_bound.m_min, out m_global_bound.m_max, out m_bvhQuantization, ref aABB.m_min, ref aABB.m_max, boundMargin);
		}

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
			m_node_array.GetRawArray()[num_nodes].SetEscapeIndex(m_num_nodes - num_nodes);
		}

		public QuantizedBvhTree()
		{
			m_num_nodes = 0;
		}

		public void BuildTree(GIM_BVH_DATA_ARRAY primitive_boxes)
		{
			CalcQuantization(primitive_boxes);
			m_num_nodes = 0;
			m_node_array.Resize(primitive_boxes.Count * 2);
			BuildSubTree(primitive_boxes, 0, primitive_boxes.Count);
		}

		public void QuantizePoint(out UShortVector3 quantizedpoint, ref IndexedVector3 point)
		{
			GImpactQuantization.QuantizeClamp(out quantizedpoint, ref point, ref m_global_bound.m_min, ref m_global_bound.m_max, ref m_bvhQuantization);
		}

		public bool TestQuantizedBoxOverlap(int node_index, ref UShortVector3 quantizedMin, ref UShortVector3 quantizedMax)
		{
			return m_node_array[node_index].TestQuantizedBoxOverlapp(ref quantizedMin, ref quantizedMax);
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
			bound.m_min = GImpactQuantization.Unquantize(ref m_node_array.GetRawArray()[nodeindex].m_quantizedAabbMin, ref m_global_bound.m_min, ref m_bvhQuantization);
			bound.m_max = GImpactQuantization.Unquantize(ref m_node_array.GetRawArray()[nodeindex].m_quantizedAabbMax, ref m_global_bound.m_min, ref m_bvhQuantization);
		}

		public void SetNodeBound(int nodeindex, ref AABB bound)
		{
			GImpactQuantization.QuantizeClamp(out m_node_array.GetRawArray()[nodeindex].m_quantizedAabbMin, ref bound.m_min, ref m_global_bound.m_min, ref m_global_bound.m_max, ref m_bvhQuantization);
			GImpactQuantization.QuantizeClamp(out m_node_array.GetRawArray()[nodeindex].m_quantizedAabbMax, ref bound.m_max, ref m_global_bound.m_min, ref m_global_bound.m_max, ref m_bvhQuantization);
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
