using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BvhTriangleMeshShape : TriangleMeshShape
	{
		private OptimizedBvh m_bvh;

		private bool m_useQuantizedAabbCompression;

		private bool m_ownsBvh;

		private TriangleInfoMap m_triangleInfoMap;

		public BvhTriangleMeshShape()
			: base(null)
		{
			m_bvh = null;
			m_ownsBvh = false;
			m_shapeType = BroadphaseNativeTypes.TRIANGLE_MESH_SHAPE_PROXYTYPE;
			m_triangleInfoMap = null;
		}

		public BvhTriangleMeshShape(StridingMeshInterface meshInterface, bool useQuantizedAabbCompression, bool buildBvh)
			: base(meshInterface)
		{
			m_bvh = null;
			m_ownsBvh = false;
			m_useQuantizedAabbCompression = useQuantizedAabbCompression;
			m_shapeType = BroadphaseNativeTypes.TRIANGLE_MESH_SHAPE_PROXYTYPE;
			if (buildBvh)
			{
				BuildOptimizedBvh();
			}
		}

		private void BuildOptimizedBvh()
		{
			if (m_ownsBvh)
			{
				m_bvh.Cleanup();
				m_bvh = null;
			}
			m_bvh = new OptimizedBvh();
			m_bvh.Build(m_meshInterface, m_useQuantizedAabbCompression, ref m_localAabbMin, ref m_localAabbMax);
			m_ownsBvh = true;
		}

		public BvhTriangleMeshShape(StridingMeshInterface meshInterface, bool useQuantizedAabbCompression, ref IndexedVector3 bvhAabbMin, ref IndexedVector3 bvhAabbMax, bool buildBvh)
			: base(meshInterface)
		{
			m_bvh = null;
			m_ownsBvh = false;
			m_useQuantizedAabbCompression = useQuantizedAabbCompression;
			m_shapeType = BroadphaseNativeTypes.TRIANGLE_MESH_SHAPE_PROXYTYPE;
			if (buildBvh)
			{
				m_bvh = new OptimizedBvh();
				m_bvh.Build(meshInterface, m_useQuantizedAabbCompression, ref bvhAabbMin, ref bvhAabbMax);
				m_ownsBvh = true;
			}
		}

		public bool GetOwnsBvh()
		{
			return m_ownsBvh;
		}

		public void PerformRaycast(ITriangleCallback callback, ref IndexedVector3 raySource, ref IndexedVector3 rayTarget)
		{
			if (m_bvh != null)
			{
				using (MyNodeOverlapCallback myNodeOverlapCallback = BulletGlobals.MyNodeOverlapCallbackPool.Get())
				{
					myNodeOverlapCallback.Initialize(callback, m_meshInterface);
					m_bvh.ReportRayOverlappingNodex(myNodeOverlapCallback, ref raySource, ref rayTarget);
				}
			}
		}

		public void PerformConvexCast(ITriangleCallback callback, ref IndexedVector3 boxSource, ref IndexedVector3 boxTarget, ref IndexedVector3 boxMin, ref IndexedVector3 boxMax)
		{
			if (m_bvh != null)
			{
				using (MyNodeOverlapCallback myNodeOverlapCallback = BulletGlobals.MyNodeOverlapCallbackPool.Get())
				{
					myNodeOverlapCallback.Initialize(callback, m_meshInterface);
					m_bvh.ReportBoxCastOverlappingNodex(myNodeOverlapCallback, ref boxSource, ref boxTarget, ref boxMin, ref boxMax);
				}
			}
		}

		public override void ProcessAllTriangles(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			if (m_bvh != null)
			{
				using (MyNodeOverlapCallback myNodeOverlapCallback = BulletGlobals.MyNodeOverlapCallbackPool.Get())
				{
					myNodeOverlapCallback.Initialize(callback, m_meshInterface);
					m_bvh.ReportAabbOverlappingNodex(myNodeOverlapCallback, ref aabbMin, ref aabbMax);
				}
			}
		}

		public void RefitTree(IndexedVector3 aabbMin, IndexedVector3 aabbMax)
		{
			RefitTree(ref aabbMin, ref aabbMax);
		}

		public void RefitTree(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			if (m_bvh != null)
			{
				m_bvh.Refit(m_meshInterface, ref aabbMin, ref aabbMax);
				RecalcLocalAabb();
			}
		}

		public void PartialRefitTree(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			if (m_bvh != null)
			{
				m_bvh.RefitPartial(m_meshInterface, ref aabbMin, ref aabbMax);
				MathUtil.VectorMin(ref aabbMin, ref m_localAabbMin);
				MathUtil.VectorMax(ref aabbMax, ref m_localAabbMax);
			}
		}

		public override string GetName()
		{
			return "BVHTRIANGLEMESH";
		}

		public override void SetLocalScaling(ref IndexedVector3 scaling)
		{
			if ((GetLocalScaling() - scaling).LengthSquared() > 1.1920929E-07f)
			{
				base.SetLocalScaling(ref scaling);
				BuildOptimizedBvh();
			}
		}

		public OptimizedBvh GetOptimizedBvh()
		{
			return m_bvh;
		}

		public void SetOptimizedBvh(OptimizedBvh bvh, ref IndexedVector3 scaling)
		{
			m_bvh = bvh;
			m_ownsBvh = false;
			if ((GetLocalScaling() - scaling).LengthSquared() > 1.1920929E-07f)
			{
				base.SetLocalScaling(ref scaling);
			}
		}

		public bool UsesQuantizedAabbCompression()
		{
			return m_useQuantizedAabbCompression;
		}

		public void SetTriangleInfoMap(TriangleInfoMap triangleInfoMap)
		{
			m_triangleInfoMap = triangleInfoMap;
		}

		public TriangleInfoMap GetTriangleInfoMap()
		{
			return m_triangleInfoMap;
		}
	}
}
