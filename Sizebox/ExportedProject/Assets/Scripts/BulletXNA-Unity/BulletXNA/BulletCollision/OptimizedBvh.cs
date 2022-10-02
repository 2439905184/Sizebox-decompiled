using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class OptimizedBvh : QuantizedBvh
	{
		public override void Cleanup()
		{
			base.Cleanup();
		}

		public void Build(StridingMeshInterface triangles, bool useQuantizedAabbCompression, ref IndexedVector3 bvhAabbMin, ref IndexedVector3 bvhAabbMax)
		{
			int num = 0;
			m_useQuantization = useQuantizedAabbCompression;
			if (m_useQuantization)
			{
				SetQuantizationValues(ref bvhAabbMin, ref bvhAabbMax);
				QuantizedNodeTriangleCallback callback = new QuantizedNodeTriangleCallback(m_quantizedLeafNodes, this);
				triangles.InternalProcessAllTriangles(callback, ref m_bvhAabbMin, ref m_bvhAabbMax);
				num = m_quantizedLeafNodes.Count;
				m_quantizedContiguousNodes.Capacity = 2 * num;
			}
			else
			{
				NodeTriangleCallback callback2 = new NodeTriangleCallback(m_leafNodes);
				IndexedVector3 aabbMin = MathUtil.MIN_VECTOR;
				IndexedVector3 aabbMax = MathUtil.MAX_VECTOR;
				triangles.InternalProcessAllTriangles(callback2, ref aabbMin, ref aabbMax);
				num = m_leafNodes.Count;
				m_contiguousNodes.Capacity = 2 * num;
			}
			m_curNodeIndex = 0;
			BuildTree(0, num);
			if (m_useQuantization && m_SubtreeHeaders.Count == 0)
			{
				BvhSubtreeInfo bvhSubtreeInfo = new BvhSubtreeInfo();
				m_SubtreeHeaders.Add(bvhSubtreeInfo);
				bvhSubtreeInfo.SetAabbFromQuantizeNode(m_quantizedContiguousNodes[0]);
				bvhSubtreeInfo.m_rootNodeIndex = 0;
				bvhSubtreeInfo.m_subtreeSize = (m_quantizedContiguousNodes[0].IsLeafNode() ? 1 : m_quantizedContiguousNodes[0].GetEscapeIndex());
			}
			m_subtreeHeaderCount = m_SubtreeHeaders.Count;
			m_quantizedLeafNodes.Clear();
			m_leafNodes.Clear();
		}

		public void Refit(StridingMeshInterface meshInterface, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			if (m_useQuantization)
			{
				SetQuantizationValues(ref aabbMin, ref aabbMax);
				UpdateBvhNodes(meshInterface, 0, m_curNodeIndex, 0);
				for (int i = 0; i < m_SubtreeHeaders.Count; i++)
				{
					BvhSubtreeInfo bvhSubtreeInfo = m_SubtreeHeaders[i];
					bvhSubtreeInfo.SetAabbFromQuantizeNode(m_quantizedContiguousNodes[bvhSubtreeInfo.m_rootNodeIndex]);
				}
			}
		}

		public void RefitPartial(StridingMeshInterface meshInterface, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			if (!m_useQuantization)
			{
				return;
			}
			UShortVector3 result;
			Quantize(out result, ref aabbMin, false);
			UShortVector3 result2;
			Quantize(out result2, ref aabbMax, true);
			for (int i = 0; i < m_SubtreeHeaders.Count; i++)
			{
				BvhSubtreeInfo bvhSubtreeInfo = m_SubtreeHeaders[i];
				if (AabbUtil2.TestQuantizedAabbAgainstQuantizedAabb(ref result, ref result2, ref bvhSubtreeInfo.m_quantizedAabbMin, ref bvhSubtreeInfo.m_quantizedAabbMax))
				{
					UpdateBvhNodes(meshInterface, bvhSubtreeInfo.m_rootNodeIndex, bvhSubtreeInfo.m_rootNodeIndex + bvhSubtreeInfo.m_subtreeSize, i);
					bvhSubtreeInfo.SetAabbFromQuantizeNode(m_quantizedContiguousNodes[bvhSubtreeInfo.m_rootNodeIndex]);
				}
			}
		}

		public void UpdateBvhNodes(StridingMeshInterface meshInterface, int firstNode, int endNode, int index)
		{
			int num = -1;
			object vertexbase = null;
			int numverts = 0;
			PHY_ScalarType type = PHY_ScalarType.PHY_INTEGER;
			int stride = 0;
			object indexbase = null;
			int indexstride = 0;
			int numfaces = 0;
			PHY_ScalarType indicestype = PHY_ScalarType.PHY_INTEGER;
			IndexedVector3[] array = new IndexedVector3[3];
			IndexedVector3 scaling = meshInterface.GetScaling();
			for (int num2 = endNode - 1; num2 >= firstNode; num2--)
			{
				QuantizedBvhNode quantizedBvhNode = m_quantizedContiguousNodes[num2];
				if (quantizedBvhNode.IsLeafNode())
				{
					int partId = quantizedBvhNode.GetPartId();
					int triangleIndex = quantizedBvhNode.GetTriangleIndex();
					if (partId != num)
					{
						if (num >= 0)
						{
							meshInterface.UnLockReadOnlyVertexBase(num);
						}
						meshInterface.GetLockedReadOnlyVertexIndexBase(out vertexbase, out numverts, out type, out stride, out indexbase, out indexstride, out numfaces, out indicestype, partId);
						num = partId;
					}
					int num3 = triangleIndex * indexstride;
					int[] rawArray = (indexbase as ObjectArray<int>).GetRawArray();
					if (vertexbase is IList<IndexedVector3>)
					{
						IndexedVector3[] rawArray2 = (vertexbase as ObjectArray<IndexedVector3>).GetRawArray();
						for (int num4 = 2; num4 >= 0; num4--)
						{
							int num5 = rawArray[num3 + num4];
							if (type == PHY_ScalarType.PHY_FLOAT)
							{
								array[num4] = rawArray2[num5] * scaling;
							}
						}
					}
					else if (vertexbase is ObjectArray<float>)
					{
						float[] rawArray3 = (vertexbase as ObjectArray<float>).GetRawArray();
						for (int num6 = 2; num6 >= 0; num6--)
						{
							int num7 = rawArray[num3 + num6];
							if (type == PHY_ScalarType.PHY_FLOAT)
							{
								int num8 = num7 * stride;
								array[num6] = new IndexedVector3(rawArray3[num8] * scaling.X, rawArray3[num8 + 1] * scaling.Y, rawArray3[num8 + 2] * scaling.Z);
							}
						}
					}
					IndexedVector3 output = MathUtil.MAX_VECTOR;
					IndexedVector3 output2 = MathUtil.MIN_VECTOR;
					MathUtil.VectorMin(ref array[0], ref output);
					MathUtil.VectorMax(ref array[0], ref output2);
					MathUtil.VectorMin(ref array[1], ref output);
					MathUtil.VectorMax(ref array[1], ref output2);
					MathUtil.VectorMin(ref array[2], ref output);
					MathUtil.VectorMax(ref array[2], ref output2);
					Quantize(out quantizedBvhNode.m_quantizedAabbMin, ref output, false);
					Quantize(out quantizedBvhNode.m_quantizedAabbMax, ref output2, true);
				}
				else
				{
					QuantizedBvhNode quantizedBvhNode2 = m_quantizedContiguousNodes[num2 + 1];
					QuantizedBvhNode quantizedBvhNode3 = (quantizedBvhNode2.IsLeafNode() ? m_quantizedContiguousNodes[num2 + 2] : m_quantizedContiguousNodes[num2 + 1 + quantizedBvhNode2.GetEscapeIndex()]);
					quantizedBvhNode.m_quantizedAabbMin = quantizedBvhNode2.m_quantizedAabbMin;
					quantizedBvhNode.m_quantizedAabbMin.Min(ref quantizedBvhNode3.m_quantizedAabbMin);
					quantizedBvhNode.m_quantizedAabbMax = quantizedBvhNode2.m_quantizedAabbMax;
					quantizedBvhNode.m_quantizedAabbMax.Max(ref quantizedBvhNode3.m_quantizedAabbMax);
				}
			}
			if (num >= 0)
			{
				meshInterface.UnLockReadOnlyVertexBase(num);
			}
		}
	}
}
