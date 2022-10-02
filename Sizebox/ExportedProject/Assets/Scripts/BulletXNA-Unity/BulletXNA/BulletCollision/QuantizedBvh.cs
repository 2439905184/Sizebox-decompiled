using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class QuantizedBvh
	{
		public const int MAX_SUBTREE_SIZE_IN_BYTES = 128;

		public const int MAX_NUM_PARTS_IN_BITS = 10;

		public static int gStackDepth;

		public static int gMaxStackDepth;

		protected IndexedVector3 m_bvhAabbMin;

		protected IndexedVector3 m_bvhAabbMax;

		protected IndexedVector3 m_bvhQuantization;

		protected int m_bulletVersion;

		protected int m_curNodeIndex;

		protected bool m_useQuantization;

		protected ObjectArray<OptimizedBvhNode> m_leafNodes = new ObjectArray<OptimizedBvhNode>();

		protected ObjectArray<OptimizedBvhNode> m_contiguousNodes = new ObjectArray<OptimizedBvhNode>();

		protected ObjectArray<QuantizedBvhNode> m_quantizedLeafNodes = new ObjectArray<QuantizedBvhNode>();

		protected ObjectArray<QuantizedBvhNode> m_quantizedContiguousNodes = new ObjectArray<QuantizedBvhNode>();

		protected TraversalMode m_traversalMode;

		protected ObjectArray<BvhSubtreeInfo> m_SubtreeHeaders = new ObjectArray<BvhSubtreeInfo>();

		protected int m_subtreeHeaderCount;

		public int m_maxIterations;

		public QuantizedBvh()
		{
			m_bulletVersion = BulletGlobals.BT_BULLET_VERSION;
			m_useQuantization = false;
			m_traversalMode = TraversalMode.TRAVERSAL_STACKLESS;
			m_subtreeHeaderCount = 0;
			m_bvhAabbMin = new IndexedVector3(float.MinValue);
			m_bvhAabbMax = new IndexedVector3(float.MaxValue);
		}

		public virtual void Cleanup()
		{
		}

		protected void SetInternalNodeAabbMin(int nodeIndex, ref IndexedVector3 aabbMin)
		{
			if (m_useQuantization)
			{
				QuantizedBvhNode quantizedBvhNode = m_quantizedContiguousNodes[nodeIndex];
				Quantize(out quantizedBvhNode.m_quantizedAabbMin, ref aabbMin, false);
				m_quantizedContiguousNodes[nodeIndex] = quantizedBvhNode;
			}
			else
			{
				OptimizedBvhNode optimizedBvhNode = m_contiguousNodes[nodeIndex];
				optimizedBvhNode.m_aabbMinOrg = aabbMin;
				m_contiguousNodes[nodeIndex] = optimizedBvhNode;
			}
		}

		public void SetInternalNodeAabbMax(int nodeIndex, ref IndexedVector3 aabbMax)
		{
			if (m_useQuantization)
			{
				QuantizedBvhNode quantizedBvhNode = m_quantizedContiguousNodes[nodeIndex];
				Quantize(out quantizedBvhNode.m_quantizedAabbMax, ref aabbMax, true);
				m_quantizedContiguousNodes[nodeIndex] = quantizedBvhNode;
			}
			else
			{
				OptimizedBvhNode optimizedBvhNode = m_contiguousNodes[nodeIndex];
				optimizedBvhNode.m_aabbMaxOrg = aabbMax;
				m_contiguousNodes[nodeIndex] = optimizedBvhNode;
			}
		}

		public IndexedVector3 GetAabbMin(int nodeIndex)
		{
			if (m_useQuantization)
			{
				IndexedVector3 vecOut;
				UnQuantize(ref m_quantizedLeafNodes[nodeIndex].m_quantizedAabbMin, out vecOut);
				return vecOut;
			}
			return m_leafNodes[nodeIndex].m_aabbMinOrg;
		}

		public IndexedVector3 GetAabbMax(int nodeIndex)
		{
			if (m_useQuantization)
			{
				IndexedVector3 vecOut;
				UnQuantize(ref m_quantizedLeafNodes[nodeIndex].m_quantizedAabbMax, out vecOut);
				return vecOut;
			}
			return m_leafNodes[nodeIndex].m_aabbMaxOrg;
		}

		public void SetInternalNodeEscapeIndex(int nodeIndex, int escapeIndex)
		{
			if (m_useQuantization)
			{
				m_quantizedContiguousNodes[nodeIndex].m_escapeIndexOrTriangleIndex = -escapeIndex;
			}
			else
			{
				m_contiguousNodes[nodeIndex].m_escapeIndex = escapeIndex;
			}
		}

		public void MergeInternalNodeAabb(int nodeIndex, IndexedVector3 newAabbMin, IndexedVector3 newAabbMax)
		{
			MergeInternalNodeAabb(nodeIndex, ref newAabbMin, ref newAabbMax);
		}

		public void MergeInternalNodeAabb(int nodeIndex, ref IndexedVector3 newAabbMin, ref IndexedVector3 newAabbMax)
		{
			if (m_useQuantization)
			{
				UShortVector3 result;
				Quantize(out result, ref newAabbMin, false);
				UShortVector3 result2;
				Quantize(out result2, ref newAabbMax, true);
				QuantizedBvhNode quantizedBvhNode = m_quantizedContiguousNodes[nodeIndex];
				quantizedBvhNode.m_quantizedAabbMin.Min(ref result);
				quantizedBvhNode.m_quantizedAabbMax.Max(ref result2);
				m_quantizedContiguousNodes[nodeIndex] = quantizedBvhNode;
			}
			else
			{
				OptimizedBvhNode optimizedBvhNode = m_contiguousNodes[nodeIndex];
				MathUtil.VectorMin(ref newAabbMin, ref optimizedBvhNode.m_aabbMinOrg);
				MathUtil.VectorMax(ref newAabbMax, ref optimizedBvhNode.m_aabbMaxOrg);
				m_contiguousNodes[nodeIndex] = optimizedBvhNode;
			}
		}

		public void SwapLeafNodes(int firstIndex, int secondIndex)
		{
			if (m_useQuantization)
			{
				QuantizedBvhNode value = m_quantizedLeafNodes[firstIndex];
				m_quantizedLeafNodes[firstIndex] = m_quantizedLeafNodes[secondIndex];
				m_quantizedLeafNodes[secondIndex] = value;
			}
			else
			{
				OptimizedBvhNode value2 = m_leafNodes[firstIndex];
				m_leafNodes[firstIndex] = m_leafNodes[secondIndex];
				m_leafNodes[secondIndex] = value2;
			}
		}

		public void AssignInternalNodeFromLeafNode(int internalNode, int leafNodeIndex)
		{
			if (m_useQuantization)
			{
				m_quantizedContiguousNodes[internalNode] = m_quantizedLeafNodes[leafNodeIndex];
			}
			else
			{
				m_contiguousNodes[internalNode] = m_leafNodes[leafNodeIndex];
			}
		}

		protected void BuildTree(int startIndex, int endIndex)
		{
			int num = endIndex - startIndex;
			int curNodeIndex = m_curNodeIndex;
			if (num == 1)
			{
				AssignInternalNodeFromLeafNode(m_curNodeIndex, startIndex);
				m_curNodeIndex++;
				return;
			}
			int splitAxis = CalcSplittingAxis(startIndex, endIndex);
			int num2 = SortAndCalcSplittingIndex(startIndex, endIndex, splitAxis);
			int curNodeIndex2 = m_curNodeIndex;
			SetInternalNodeAabbMin(m_curNodeIndex, ref m_bvhAabbMax);
			SetInternalNodeAabbMax(m_curNodeIndex, ref m_bvhAabbMin);
			for (int i = startIndex; i < endIndex; i++)
			{
				MergeInternalNodeAabb(m_curNodeIndex, GetAabbMin(i), GetAabbMax(i));
			}
			int curNodeIndex3 = m_curNodeIndex;
			m_curNodeIndex++;
			int curNodeIndex4 = m_curNodeIndex;
			m_quantizedContiguousNodes[curNodeIndex3].m_leftChildIndex = curNodeIndex4;
			BuildTree(startIndex, num2);
			int curNodeIndex5 = m_curNodeIndex;
			m_quantizedContiguousNodes[curNodeIndex3].m_rightChildIndex = curNodeIndex5;
			BuildTree(num2, endIndex);
			int num3 = m_curNodeIndex - curNodeIndex;
			if (m_useQuantization && num3 > 128)
			{
				UpdateSubtreeHeaders(curNodeIndex4, curNodeIndex5);
			}
			SetInternalNodeEscapeIndex(curNodeIndex2, num3);
		}

		protected int CalcSplittingAxis(int startIndex, int endIndex)
		{
			IndexedVector3 zero = IndexedVector3.Zero;
			IndexedVector3 a = IndexedVector3.Zero;
			int num = endIndex - startIndex;
			for (int i = startIndex; i < endIndex; i++)
			{
				IndexedVector3 indexedVector = 0.5f * (GetAabbMax(i) + GetAabbMin(i));
				zero += indexedVector;
			}
			zero *= 1f / (float)num;
			for (int j = startIndex; j < endIndex; j++)
			{
				IndexedVector3 indexedVector2 = 0.5f * (GetAabbMax(j) + GetAabbMin(j));
				IndexedVector3 indexedVector3 = indexedVector2 - zero;
				indexedVector3 *= indexedVector3;
				a += indexedVector3;
			}
			a *= 1f / (float)(num - 1);
			return MathUtil.MaxAxis(ref a);
		}

		protected int SortAndCalcSplittingIndex(int startIndex, int endIndex, int splitAxis)
		{
			int num = startIndex;
			int num2 = endIndex - startIndex;
			IndexedVector3 zero = IndexedVector3.Zero;
			for (int i = startIndex; i < endIndex; i++)
			{
				IndexedVector3 indexedVector = 0.5f * (GetAabbMax(i) + GetAabbMin(i));
				zero += indexedVector;
			}
			float num3 = (zero * (1f / (float)num2))[splitAxis];
			for (int j = startIndex; j < endIndex; j++)
			{
				if ((0.5f * (GetAabbMax(j) + GetAabbMin(j)))[splitAxis] > num3)
				{
					SwapLeafNodes(j, num);
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

		protected void WalkStacklessTree(INodeOverlapCallback nodeCallback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			int num = 0;
			OptimizedBvhNode optimizedBvhNode = m_contiguousNodes[num];
			int num2 = 0;
			while (num < m_curNodeIndex)
			{
				num2++;
				bool flag = AabbUtil2.TestAabbAgainstAabb2(ref aabbMin, ref aabbMax, ref optimizedBvhNode.m_aabbMinOrg, ref optimizedBvhNode.m_aabbMaxOrg);
				bool flag2 = optimizedBvhNode.m_escapeIndex == -1;
				if (flag2 && flag)
				{
					nodeCallback.ProcessNode(optimizedBvhNode.m_subPart, optimizedBvhNode.m_triangleIndex);
				}
				if (flag || flag2)
				{
					num++;
				}
				else
				{
					int escapeIndex = optimizedBvhNode.m_escapeIndex;
					num += escapeIndex;
				}
				optimizedBvhNode = m_contiguousNodes[num];
			}
			if (m_maxIterations < num2)
			{
				m_maxIterations = num2;
			}
		}

		protected void WalkStacklessQuantizedTreeAgainstRay(INodeOverlapCallback nodeCallback, ref IndexedVector3 raySource, ref IndexedVector3 rayTarget, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, int startNodeIndex, int endNodeIndex)
		{
			int num = startNodeIndex;
			int num2 = 0;
			QuantizedBvhNode quantizedBvhNode = m_quantizedContiguousNodes[num];
			int num3 = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			float num4 = 1f;
			IndexedVector3 rayInvDirection = rayTarget - raySource;
			rayInvDirection.Normalize();
			num4 = IndexedVector3.Dot(rayInvDirection, rayTarget - raySource);
			rayInvDirection.X = (MathUtil.FuzzyZero(rayInvDirection.X) ? 1E+18f : (1f / rayInvDirection.X));
			rayInvDirection.Y = (MathUtil.FuzzyZero(rayInvDirection.Y) ? 1E+18f : (1f / rayInvDirection.Y));
			rayInvDirection.Z = (MathUtil.FuzzyZero(rayInvDirection.Z) ? 1E+18f : (1f / rayInvDirection.Z));
			bool raySign = rayInvDirection.X < 0f;
			bool raySign2 = rayInvDirection.Y < 0f;
			bool raySign3 = rayInvDirection.Z < 0f;
			IndexedVector3 output = raySource;
			IndexedVector3 output2 = raySource;
			MathUtil.VectorMin(ref rayTarget, ref output);
			MathUtil.VectorMax(ref rayTarget, ref output2);
			output += aabbMin;
			output2 += aabbMax;
			UShortVector3 result;
			QuantizeWithClamp(out result, ref output, false);
			UShortVector3 result2;
			QuantizeWithClamp(out result2, ref output2, true);
			while (num < endNodeIndex)
			{
				num2++;
				float tmin = 1f;
				flag3 = false;
				flag2 = AabbUtil2.TestQuantizedAabbAgainstQuantizedAabb(ref result, ref result2, ref quantizedBvhNode.m_quantizedAabbMin, ref quantizedBvhNode.m_quantizedAabbMax);
				flag = quantizedBvhNode.IsLeafNode();
				if (flag2)
				{
					IndexedVector3 vecOut;
					UnQuantize(ref quantizedBvhNode.m_quantizedAabbMin, out vecOut);
					IndexedVector3 vecOut2;
					UnQuantize(ref quantizedBvhNode.m_quantizedAabbMax, out vecOut2);
					vecOut -= aabbMax;
					vecOut2 -= aabbMin;
					BulletGlobals.StartProfile("btRayAabb2");
					flag3 = AabbUtil2.RayAabb2Alt(ref raySource, ref rayInvDirection, raySign, raySign2, raySign3, ref vecOut, ref vecOut2, out tmin, 0f, num4);
					BulletGlobals.StopProfile();
				}
				if (flag && flag3)
				{
					nodeCallback.ProcessNode(quantizedBvhNode.GetPartId(), quantizedBvhNode.GetTriangleIndex());
				}
				if (flag3 || flag)
				{
					num++;
				}
				else
				{
					num3 = quantizedBvhNode.GetEscapeIndex();
					num += num3;
				}
				quantizedBvhNode = m_quantizedContiguousNodes[num];
			}
			if (m_maxIterations < num2)
			{
				m_maxIterations = num2;
			}
		}

		protected void WalkStacklessQuantizedTree(INodeOverlapCallback nodeCallback, ref UShortVector3 quantizedQueryAabbMin, ref UShortVector3 quantizedQueryAabbMax, int startNodeIndex, int endNodeIndex)
		{
			int num = startNodeIndex;
			int num2 = 0;
			QuantizedBvhNode quantizedBvhNode = m_quantizedContiguousNodes[num];
			int num3 = 0;
			bool flag = false;
			while (num < endNodeIndex)
			{
				num2++;
				flag = AabbUtil2.TestQuantizedAabbAgainstQuantizedAabb(ref quantizedQueryAabbMin, ref quantizedQueryAabbMax, ref quantizedBvhNode.m_quantizedAabbMin, ref quantizedBvhNode.m_quantizedAabbMax);
				bool flag2 = quantizedBvhNode.IsLeafNode();
				if (flag2 && flag)
				{
					nodeCallback.ProcessNode(quantizedBvhNode.GetPartId(), quantizedBvhNode.GetTriangleIndex());
				}
				if (flag || flag2)
				{
					num++;
				}
				else
				{
					num3 = quantizedBvhNode.GetEscapeIndex();
					num += num3;
				}
				quantizedBvhNode = m_quantizedContiguousNodes[num];
			}
			if (m_maxIterations < num2)
			{
				m_maxIterations = num2;
			}
		}

		protected void WalkStacklessTreeAgainstRay(INodeOverlapCallback nodeCallback, ref IndexedVector3 raySource, ref IndexedVector3 rayTarget, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, int startNodeIndex, int endNodeIndex)
		{
			int num = 0;
			OptimizedBvhNode optimizedBvhNode = m_contiguousNodes[num];
			int num2 = 0;
			bool flag = false;
			bool flag2 = false;
			float num3 = 1f;
			IndexedVector3 output = raySource;
			IndexedVector3 output2 = raySource;
			MathUtil.VectorMin(ref rayTarget, ref output);
			MathUtil.VectorMax(ref rayTarget, ref output2);
			output += aabbMin;
			output2 += aabbMax;
			IndexedVector3 a = rayTarget - raySource;
			a.Normalize();
			num3 = IndexedVector3.Dot(a, rayTarget - raySource);
			IndexedVector3 rayInvDirection = new IndexedVector3(MathUtil.FuzzyZero(a.X) ? 1E+18f : (1f / a.X), MathUtil.FuzzyZero(a.Y) ? 1E+18f : (1f / a.Y), MathUtil.FuzzyZero(a.Z) ? 1E+18f : (1f / a.Z));
			bool raySign = rayInvDirection.X < 0f;
			bool raySign2 = rayInvDirection.Y < 0f;
			bool raySign3 = rayInvDirection.Z < 0f;
			while (num < m_curNodeIndex)
			{
				float tmin = 1f;
				num2++;
				IndexedVector3 minBounds = optimizedBvhNode.m_aabbMinOrg;
				IndexedVector3 maxBounds = optimizedBvhNode.m_aabbMaxOrg;
				minBounds -= aabbMax;
				maxBounds -= aabbMin;
				flag2 = AabbUtil2.TestAabbAgainstAabb2(ref output, ref output2, ref optimizedBvhNode.m_aabbMinOrg, ref optimizedBvhNode.m_aabbMaxOrg) && AabbUtil2.RayAabb2Alt(ref raySource, ref rayInvDirection, raySign, raySign2, raySign3, ref minBounds, ref maxBounds, out tmin, 0f, num3);
				bool flag3 = optimizedBvhNode.m_escapeIndex == -1;
				if (flag3 && flag2)
				{
					nodeCallback.ProcessNode(optimizedBvhNode.m_subPart, optimizedBvhNode.m_triangleIndex);
				}
				if (flag2 || flag3)
				{
					num++;
				}
				else
				{
					int escapeIndex = optimizedBvhNode.m_escapeIndex;
					num += escapeIndex;
				}
				optimizedBvhNode = m_contiguousNodes[num];
			}
			if (m_maxIterations < num2)
			{
				m_maxIterations = num2;
			}
		}

		protected void WalkStacklessQuantizedTreeCacheFriendly(INodeOverlapCallback nodeCallback, ref UShortVector3 quantizedQueryAabbMin, ref UShortVector3 quantizedQueryAabbMax)
		{
			for (int i = 0; i < m_SubtreeHeaders.Count; i++)
			{
				BvhSubtreeInfo bvhSubtreeInfo = m_SubtreeHeaders[i];
				if (AabbUtil2.TestQuantizedAabbAgainstQuantizedAabb(ref quantizedQueryAabbMin, ref quantizedQueryAabbMax, ref bvhSubtreeInfo.m_quantizedAabbMin, ref bvhSubtreeInfo.m_quantizedAabbMax))
				{
					WalkStacklessQuantizedTree(nodeCallback, ref quantizedQueryAabbMin, ref quantizedQueryAabbMax, bvhSubtreeInfo.m_rootNodeIndex, bvhSubtreeInfo.m_rootNodeIndex + bvhSubtreeInfo.m_subtreeSize);
				}
			}
		}

		protected void WalkRecursiveQuantizedTreeAgainstQueryAabb(ref QuantizedBvhNode currentNode, INodeOverlapCallback nodeCallback, ref UShortVector3 quantizedQueryAabbMin, ref UShortVector3 quantizedQueryAabbMax)
		{
			bool flag = false;
			flag = AabbUtil2.TestQuantizedAabbAgainstQuantizedAabb(ref quantizedQueryAabbMin, ref quantizedQueryAabbMax, ref currentNode.m_quantizedAabbMin, ref currentNode.m_quantizedAabbMax);
			bool flag2 = currentNode.IsLeafNode();
			if (!flag)
			{
				return;
			}
			if (flag2)
			{
				nodeCallback.ProcessNode(currentNode.GetPartId(), currentNode.GetTriangleIndex());
				return;
			}
			if (currentNode.m_leftChildIndex > -1 && currentNode.m_leftChildIndex < m_quantizedContiguousNodes.Count)
			{
				QuantizedBvhNode currentNode2 = m_quantizedContiguousNodes[currentNode.m_leftChildIndex];
				WalkRecursiveQuantizedTreeAgainstQueryAabb(ref currentNode2, nodeCallback, ref quantizedQueryAabbMin, ref quantizedQueryAabbMax);
			}
			if (currentNode.m_rightChildIndex > -1 && currentNode.m_rightChildIndex < m_quantizedContiguousNodes.Count)
			{
				QuantizedBvhNode currentNode3 = m_quantizedContiguousNodes[currentNode.m_rightChildIndex];
				WalkRecursiveQuantizedTreeAgainstQueryAabb(ref currentNode3, nodeCallback, ref quantizedQueryAabbMin, ref quantizedQueryAabbMax);
			}
		}

		protected void WalkRecursiveQuantizedTreeAgainstQuantizedTree(QuantizedBvhNode treeNodeA, QuantizedBvhNode treeNodeB, INodeOverlapCallback nodeCallback)
		{
		}

		protected void UpdateSubtreeHeaders(int leftChildNodexIndex, int rightChildNodexIndex)
		{
			QuantizedBvhNode quantizedBvhNode = m_quantizedContiguousNodes[leftChildNodexIndex];
			int subtreeSize = (quantizedBvhNode.IsLeafNode() ? 1 : quantizedBvhNode.GetEscapeIndex());
			QuantizedBvhNode quantizedBvhNode2 = m_quantizedContiguousNodes[rightChildNodexIndex];
			int subtreeSize2 = (quantizedBvhNode2.IsLeafNode() ? 1 : quantizedBvhNode2.GetEscapeIndex());
			if (quantizedBvhNode.GetEscapeIndex() <= 128)
			{
				BvhSubtreeInfo bvhSubtreeInfo = new BvhSubtreeInfo();
				m_SubtreeHeaders.Add(bvhSubtreeInfo);
				bvhSubtreeInfo.SetAabbFromQuantizeNode(quantizedBvhNode);
				bvhSubtreeInfo.m_rootNodeIndex = leftChildNodexIndex;
				bvhSubtreeInfo.m_subtreeSize = subtreeSize;
			}
			if (quantizedBvhNode2.GetEscapeIndex() <= 128)
			{
				BvhSubtreeInfo bvhSubtreeInfo2 = new BvhSubtreeInfo();
				m_SubtreeHeaders.Add(bvhSubtreeInfo2);
				bvhSubtreeInfo2.SetAabbFromQuantizeNode(quantizedBvhNode2);
				bvhSubtreeInfo2.m_rootNodeIndex = rightChildNodexIndex;
				bvhSubtreeInfo2.m_subtreeSize = subtreeSize2;
			}
			m_subtreeHeaderCount = m_SubtreeHeaders.Count;
		}

		public void SetQuantizationValues(ref IndexedVector3 bvhAabbMin, ref IndexedVector3 bvhAabbMax)
		{
			SetQuantizationValues(ref bvhAabbMin, ref bvhAabbMax, 1f);
		}

		public void SetQuantizationValues(ref IndexedVector3 bvhAabbMin, ref IndexedVector3 bvhAabbMax, float quantizationMargin)
		{
			IndexedVector3 indexedVector = new IndexedVector3(quantizationMargin);
			m_bvhAabbMin = bvhAabbMin - indexedVector;
			m_bvhAabbMax = bvhAabbMax + indexedVector;
			IndexedVector3 indexedVector2 = m_bvhAabbMax - m_bvhAabbMin;
			m_bvhQuantization = new IndexedVector3(65533f) / indexedVector2;
			m_useQuantization = true;
		}

		public ObjectArray<QuantizedBvhNode> GetLeafNodeArray()
		{
			return m_quantizedLeafNodes;
		}

		public void BuildInternal()
		{
			m_useQuantization = true;
			int num = 0;
			if (m_useQuantization)
			{
				num = m_quantizedLeafNodes.Count;
				m_quantizedContiguousNodes.Capacity = 2 * num;
			}
			m_curNodeIndex = 0;
			BuildTree(0, num);
			for (int i = 0; i < m_quantizedContiguousNodes.Count; i++)
			{
				QuantizedBvhNode quantizedBvhNode = m_quantizedContiguousNodes[i];
				Console.WriteLine(string.Format("QNode[{0}] Esc[{1}] min[{2},{3},{4}] max[{5},{6},{7}]", i, quantizedBvhNode.m_escapeIndexOrTriangleIndex, quantizedBvhNode.m_quantizedAabbMin.X, quantizedBvhNode.m_quantizedAabbMin.Y, quantizedBvhNode.m_quantizedAabbMin.Z, quantizedBvhNode.m_quantizedAabbMax.X, quantizedBvhNode.m_quantizedAabbMax.Y, quantizedBvhNode.m_quantizedAabbMax.Z));
			}
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

		public void ReportAabbOverlappingNodex(INodeOverlapCallback nodeCallback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			if (m_useQuantization)
			{
				UShortVector3 result;
				QuantizeWithClamp(out result, ref aabbMin, false);
				UShortVector3 result2;
				QuantizeWithClamp(out result2, ref aabbMax, true);
				switch (m_traversalMode)
				{
				case TraversalMode.TRAVERSAL_STACKLESS:
					if (m_useQuantization)
					{
						WalkStacklessQuantizedTree(nodeCallback, ref result, ref result2, 0, m_curNodeIndex);
					}
					else
					{
						WalkStacklessTree(nodeCallback, ref aabbMin, ref aabbMax);
					}
					break;
				case TraversalMode.TRAVERSAL_STACKLESS_CACHE_FRIENDLY:
					WalkStacklessQuantizedTreeCacheFriendly(nodeCallback, ref result, ref result2);
					break;
				case TraversalMode.TRAVERSAL_RECURSIVE:
				{
					QuantizedBvhNode currentNode = m_quantizedContiguousNodes[0];
					WalkRecursiveQuantizedTreeAgainstQueryAabb(ref currentNode, nodeCallback, ref result, ref result2);
					break;
				}
				}
			}
			else
			{
				WalkStacklessTree(nodeCallback, ref aabbMin, ref aabbMax);
			}
		}

		public void ReportRayOverlappingNodex(INodeOverlapCallback nodeCallback, ref IndexedVector3 raySource, ref IndexedVector3 rayTarget)
		{
			IndexedVector3 aabbMin = IndexedVector3.Zero;
			IndexedVector3 aabbMax = IndexedVector3.Zero;
			ReportBoxCastOverlappingNodex(nodeCallback, ref raySource, ref rayTarget, ref aabbMin, ref aabbMax);
		}

		public void ReportBoxCastOverlappingNodex(INodeOverlapCallback nodeCallback, ref IndexedVector3 raySource, ref IndexedVector3 rayTarget, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			if (m_useQuantization)
			{
				WalkStacklessQuantizedTreeAgainstRay(nodeCallback, ref raySource, ref rayTarget, ref aabbMin, ref aabbMax, 0, m_curNodeIndex);
			}
			else
			{
				WalkStacklessTreeAgainstRay(nodeCallback, ref raySource, ref rayTarget, ref aabbMin, ref aabbMax, 0, m_curNodeIndex);
			}
		}

		public void Quantize(out UShortVector3 result, ref IndexedVector3 point, bool isMax)
		{
			IndexedVector3 indexedVector = (point - m_bvhAabbMin) * m_bvhQuantization;
			result = default(UShortVector3);
			if (isMax)
			{
				result.X = (ushort)((ushort)(indexedVector.X + 1f) | 1u);
				result.Y = (ushort)((ushort)(indexedVector.Y + 1f) | 1u);
				result.Z = (ushort)((ushort)(indexedVector.Z + 1f) | 1u);
			}
			else
			{
				result.X = (ushort)((ushort)indexedVector.X & 0xFFFEu);
				result.Y = (ushort)((ushort)indexedVector.Y & 0xFFFEu);
				result.Z = (ushort)((ushort)indexedVector.Z & 0xFFFEu);
			}
		}

		public void QuantizeWithClamp(out UShortVector3 result, ref IndexedVector3 point2, bool isMax)
		{
			IndexedVector3 output = point2;
			MathUtil.VectorMax(ref m_bvhAabbMin, ref output);
			MathUtil.VectorMin(ref m_bvhAabbMax, ref output);
			Quantize(out result, ref output, isMax);
		}

		public void UnQuantize(ref UShortVector3 vecIn, out IndexedVector3 vecOut)
		{
			vecOut = new IndexedVector3((float)(int)vecIn.X / m_bvhQuantization.X, (float)(int)vecIn.Y / m_bvhQuantization.Y, (float)(int)vecIn.Z / m_bvhQuantization.Z);
			vecOut += m_bvhAabbMin;
		}

		private void SetTraversalMode(TraversalMode traversalMode)
		{
			m_traversalMode = traversalMode;
		}

		public ObjectArray<QuantizedBvhNode> GetQuantizedNodeArray()
		{
			return m_quantizedContiguousNodes;
		}

		public ObjectArray<BvhSubtreeInfo> GetSubtreeInfoArray()
		{
			return m_SubtreeHeaders;
		}

		public bool IsQuantized()
		{
			return m_useQuantization;
		}
	}
}
