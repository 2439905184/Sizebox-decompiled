using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GImpactQuantizedBvh
	{
		protected QuantizedBvhTree m_box_tree = new QuantizedBvhTree();

		protected IPrimitiveManagerBase m_primitive_manager;

		private static bool QuantizedNodeCollision(GImpactQuantizedBvh boxset0, GImpactQuantizedBvh boxset1, BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0, int node0, int node1, bool complete_primitive_tests)
		{
			AABB bound;
			boxset0.GetNodeBound(node0, out bound);
			AABB bound2;
			boxset1.GetNodeBound(node1, out bound2);
			return bound.OverlappingTransCache(ref bound2, ref trans_cache_1to0, complete_primitive_tests);
		}

		protected void Refit()
		{
			int nodeCount = GetNodeCount();
			while (nodeCount-- != 0)
			{
				if (IsLeafNode(nodeCount))
				{
					AABB primbox;
					m_primitive_manager.GetPrimitiveBox(GetNodeData(nodeCount), out primbox);
					SetNodeBound(nodeCount, ref primbox);
					continue;
				}
				AABB bound = default(AABB);
				bound.Invalidate();
				int leftNode = GetLeftNode(nodeCount);
				AABB bound2;
				if (leftNode != 0)
				{
					GetNodeBound(leftNode, out bound2);
					bound.Merge(ref bound2);
				}
				leftNode = GetRightNode(nodeCount);
				if (leftNode != 0)
				{
					GetNodeBound(leftNode, out bound2);
					bound.Merge(ref bound2);
				}
				SetNodeBound(nodeCount, ref bound);
			}
		}

		public GImpactQuantizedBvh()
		{
		}

		public GImpactQuantizedBvh(IPrimitiveManagerBase primitive_manager)
		{
			m_primitive_manager = primitive_manager;
		}

		public AABB GetGlobalBox()
		{
			AABB bound;
			GetNodeBound(0, out bound);
			return bound;
		}

		public void SetPrimitiveManager(IPrimitiveManagerBase primitive_manager)
		{
			m_primitive_manager = primitive_manager;
		}

		public IPrimitiveManagerBase GetPrimitiveManager()
		{
			return m_primitive_manager;
		}

		public void Update()
		{
			Refit();
		}

		public void BuildSet()
		{
			int primitiveCount = m_primitive_manager.GetPrimitiveCount();
			GIM_BVH_DATA_ARRAY gIM_BVH_DATA_ARRAY = new GIM_BVH_DATA_ARRAY(primitiveCount);
			gIM_BVH_DATA_ARRAY.Resize(primitiveCount);
			GIM_BVH_DATA[] rawArray = gIM_BVH_DATA_ARRAY.GetRawArray();
			for (int i = 0; i < primitiveCount; i++)
			{
				m_primitive_manager.GetPrimitiveBox(i, out rawArray[i].m_bound);
				rawArray[i].m_data = i;
			}
			m_box_tree.BuildTree(gIM_BVH_DATA_ARRAY);
		}

		public bool BoxQuery(ref AABB box, ObjectArray<int> collided_results)
		{
			int num = 0;
			int nodeCount = GetNodeCount();
			UShortVector3 quantizedpoint;
			m_box_tree.QuantizePoint(out quantizedpoint, ref box.m_min);
			UShortVector3 quantizedpoint2;
			m_box_tree.QuantizePoint(out quantizedpoint2, ref box.m_max);
			while (num < nodeCount)
			{
				bool flag = m_box_tree.TestQuantizedBoxOverlap(num, ref quantizedpoint, ref quantizedpoint2);
				bool flag2 = IsLeafNode(num);
				if (flag2 && flag)
				{
					collided_results.Add(GetNodeData(num));
				}
				num = ((!flag && !flag2) ? (num + GetEscapeNodeIndex(num)) : (num + 1));
			}
			if (collided_results.Count > 0)
			{
				return true;
			}
			return false;
		}

		public bool BoxQueryTrans(ref AABB box, ref IndexedMatrix transform, ObjectArray<int> collided_results)
		{
			AABB box2 = box;
			box2.ApplyTransform(ref transform);
			return BoxQuery(ref box2, collided_results);
		}

		public bool RayQuery(ref IndexedVector3 ray_dir, ref IndexedVector3 ray_origin, ObjectArray<int> collided_results)
		{
			int num = 0;
			int nodeCount = GetNodeCount();
			while (num < nodeCount)
			{
				AABB bound;
				GetNodeBound(num, out bound);
				bool flag = bound.CollideRay(ref ray_origin, ref ray_dir);
				bool flag2 = IsLeafNode(num);
				if (flag2 && flag)
				{
					collided_results.Add(GetNodeData(num));
				}
				num = ((!flag && !flag2) ? (num + GetEscapeNodeIndex(num)) : (num + 1));
			}
			if (collided_results.Count > 0)
			{
				return true;
			}
			return false;
		}

		public bool HasHierarchy()
		{
			return true;
		}

		public bool IsTrimesh()
		{
			return m_primitive_manager.IsTrimesh();
		}

		public int GetNodeCount()
		{
			return m_box_tree.GetNodeCount();
		}

		public bool IsLeafNode(int nodeindex)
		{
			return m_box_tree.IsLeafNode(nodeindex);
		}

		public int GetNodeData(int nodeindex)
		{
			return m_box_tree.GetNodeData(nodeindex);
		}

		public void GetNodeBound(int nodeindex, out AABB bound)
		{
			m_box_tree.GetNodeBound(nodeindex, out bound);
		}

		public void SetNodeBound(int nodeindex, ref AABB bound)
		{
			m_box_tree.SetNodeBound(nodeindex, ref bound);
		}

		public int GetLeftNode(int nodeindex)
		{
			return m_box_tree.GetLeftNode(nodeindex);
		}

		public int GetRightNode(int nodeindex)
		{
			return m_box_tree.GetRightNode(nodeindex);
		}

		public int GetEscapeNodeIndex(int nodeindex)
		{
			return m_box_tree.GetEscapeNodeIndex(nodeindex);
		}

		public void GetNodeTriangle(int nodeindex, PrimitiveTriangle triangle)
		{
			m_primitive_manager.GetPrimitiveTriangle(GetNodeData(nodeindex), triangle);
		}

		public static float GetAverageTreeCollisionTime()
		{
			return 1f;
		}

		public static void FindQuantizedCollisionPairsRecursive(GImpactQuantizedBvh boxset0, GImpactQuantizedBvh boxset1, PairSet collision_pairs, ref BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0, int node0, int node1, bool complete_primitive_tests)
		{
			if (!QuantizedNodeCollision(boxset0, boxset1, trans_cache_1to0, node0, node1, complete_primitive_tests))
			{
				return;
			}
			if (boxset0.IsLeafNode(node0))
			{
				if (boxset1.IsLeafNode(node1))
				{
					collision_pairs.PushPair(boxset0.GetNodeData(node0), boxset1.GetNodeData(node1));
					return;
				}
				FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to0, node0, boxset1.GetLeftNode(node1), false);
				FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to0, node0, boxset1.GetRightNode(node1), false);
			}
			else if (boxset1.IsLeafNode(node1))
			{
				FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to0, boxset0.GetLeftNode(node0), node1, false);
				FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to0, boxset0.GetRightNode(node0), node1, false);
			}
			else
			{
				FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to0, boxset0.GetLeftNode(node0), boxset1.GetLeftNode(node1), false);
				FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to0, boxset0.GetLeftNode(node0), boxset1.GetRightNode(node1), false);
				FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to0, boxset0.GetRightNode(node0), boxset1.GetLeftNode(node1), false);
				FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to0, boxset0.GetRightNode(node0), boxset1.GetRightNode(node1), false);
			}
		}

		public static void FindCollision(GImpactQuantizedBvh boxset0, ref IndexedMatrix trans0, GImpactQuantizedBvh boxset1, ref IndexedMatrix trans1, PairSet collision_pairs)
		{
			if (boxset0.GetNodeCount() != 0 && boxset1.GetNodeCount() != 0)
			{
				BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to = default(BT_BOX_BOX_TRANSFORM_CACHE);
				trans_cache_1to.CalcFromHomogenic(ref trans0, ref trans1);
				FindQuantizedCollisionPairsRecursive(boxset0, boxset1, collision_pairs, ref trans_cache_1to, 0, 0, true);
			}
		}
	}
}
