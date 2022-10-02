using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GImpactBvh
	{
		protected BvhTree m_box_tree;

		protected IPrimitiveManagerBase m_primitive_manager;

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

		public GImpactBvh()
		{
			m_primitive_manager = null;
		}

		public GImpactBvh(IPrimitiveManagerBase primitive_manager)
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
			GIM_BVH_DATA_ARRAY gIM_BVH_DATA_ARRAY = new GIM_BVH_DATA_ARRAY();
			gIM_BVH_DATA_ARRAY.Capacity = m_primitive_manager.GetPrimitiveCount();
			for (int i = 0; i < gIM_BVH_DATA_ARRAY.Count; i++)
			{
				m_primitive_manager.GetPrimitiveBox(i, out gIM_BVH_DATA_ARRAY.GetRawArray()[i].m_bound);
				gIM_BVH_DATA_ARRAY.GetRawArray()[i].m_data = i;
			}
			m_box_tree.BuildTree(gIM_BVH_DATA_ARRAY);
		}

		public bool BoxQuery(ref AABB box, ObjectArray<int> collided_results)
		{
			int num = 0;
			int nodeCount = GetNodeCount();
			while (num < nodeCount)
			{
				AABB bound = default(AABB);
				GetNodeBound(num, out bound);
				bool flag = bound.HasCollision(ref box);
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
				AABB bound = default(AABB);
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
			return 0f;
		}

		public static void FindCollision(GImpactBvh boxset1, ref IndexedMatrix trans1, GImpactBvh boxset2, ref IndexedMatrix trans2, PairSet collision_pairs)
		{
			if (boxset1.GetNodeCount() != 0 && boxset2.GetNodeCount() != 0)
			{
				BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to = default(BT_BOX_BOX_TRANSFORM_CACHE);
				trans_cache_1to.CalcFromHomogenic(ref trans1, ref trans2);
				FindCollisionPairsRecursive(boxset1, boxset2, collision_pairs, trans_cache_1to, 0, 0, true);
			}
		}

		public static bool NodeCollision(GImpactBvh boxset0, GImpactBvh boxset1, ref BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0, int node0, int node1, bool complete_primitive_tests)
		{
			AABB bound;
			boxset0.GetNodeBound(node0, out bound);
			AABB bound2;
			boxset1.GetNodeBound(node1, out bound2);
			return bound.OverlappingTransCache(ref bound2, ref trans_cache_1to0, complete_primitive_tests);
		}

		public static void FindCollisionPairsRecursive(GImpactBvh boxset0, GImpactBvh boxset1, PairSet collision_pairs, BT_BOX_BOX_TRANSFORM_CACHE trans_cache_1to0, int node0, int node1, bool complete_primitive_tests)
		{
			if (!NodeCollision(boxset0, boxset1, ref trans_cache_1to0, node0, node1, complete_primitive_tests))
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
				FindCollisionPairsRecursive(boxset0, boxset1, collision_pairs, trans_cache_1to0, node0, boxset1.GetLeftNode(node1), false);
				FindCollisionPairsRecursive(boxset0, boxset1, collision_pairs, trans_cache_1to0, node0, boxset1.GetRightNode(node1), false);
			}
			else if (boxset1.IsLeafNode(node1))
			{
				FindCollisionPairsRecursive(boxset0, boxset1, collision_pairs, trans_cache_1to0, boxset0.GetLeftNode(node0), node1, false);
				FindCollisionPairsRecursive(boxset0, boxset1, collision_pairs, trans_cache_1to0, boxset0.GetRightNode(node0), node1, false);
			}
			else
			{
				FindCollisionPairsRecursive(boxset0, boxset1, collision_pairs, trans_cache_1to0, boxset0.GetLeftNode(node0), boxset1.GetLeftNode(node1), false);
				FindCollisionPairsRecursive(boxset0, boxset1, collision_pairs, trans_cache_1to0, boxset0.GetLeftNode(node0), boxset1.GetRightNode(node1), false);
				FindCollisionPairsRecursive(boxset0, boxset1, collision_pairs, trans_cache_1to0, boxset0.GetRightNode(node0), boxset1.GetLeftNode(node1), false);
				FindCollisionPairsRecursive(boxset0, boxset1, collision_pairs, trans_cache_1to0, boxset0.GetRightNode(node0), boxset1.GetRightNode(node1), false);
			}
		}
	}
}
