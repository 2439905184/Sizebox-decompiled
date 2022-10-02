namespace BulletXNA.BulletCollision
{
	public class DefaultCollisionConfiguration : ICollisionConfiguration
	{
		protected int m_persistentManifoldPoolSize;

		protected bool m_useEpaPenetrationAlgorithm;

		protected VoronoiSimplexSolver m_simplexSolver;

		protected IConvexPenetrationDepthSolver m_pdSolver;

		private CollisionAlgorithmCreateFunc m_convexConvexCreateFunc;

		private CollisionAlgorithmCreateFunc m_convexConcaveCreateFunc;

		private CollisionAlgorithmCreateFunc m_swappedConvexConcaveCreateFunc;

		private CollisionAlgorithmCreateFunc m_compoundCreateFunc;

		private CollisionAlgorithmCreateFunc m_swappedCompoundCreateFunc;

		private CollisionAlgorithmCreateFunc m_emptyCreateFunc;

		private CollisionAlgorithmCreateFunc m_sphereSphereCF;

		private CollisionAlgorithmCreateFunc m_sphereBoxCF;

		private CollisionAlgorithmCreateFunc m_boxSphereCF;

		private CollisionAlgorithmCreateFunc m_boxBoxCF;

		private CollisionAlgorithmCreateFunc m_sphereTriangleCF;

		private CollisionAlgorithmCreateFunc m_triangleSphereCF;

		private CollisionAlgorithmCreateFunc m_planeConvexCF;

		private CollisionAlgorithmCreateFunc m_convexPlaneCF;

		private CollisionAlgorithmCreateFunc m_convexAlgo2DCF;

		public DefaultCollisionConfiguration()
			: this(new DefaultCollisionConstructionInfo())
		{
		}

		public DefaultCollisionConfiguration(DefaultCollisionConstructionInfo constructionInfo)
		{
			m_simplexSolver = BulletGlobals.VoronoiSimplexSolverPool.Get();
			m_pdSolver = new MinkowskiPenetrationDepthSolver();
			m_useEpaPenetrationAlgorithm = true;
			m_convexConvexCreateFunc = new ConvexConvexCreateFunc(m_simplexSolver, m_pdSolver);
			m_convexConcaveCreateFunc = new ConvexConcaveCreateFunc();
			m_swappedConvexConcaveCreateFunc = new SwappedConvexConcaveCreateFunc();
			m_compoundCreateFunc = new CompoundCreateFunc();
			m_swappedCompoundCreateFunc = new SwappedCompoundCreateFunc();
			m_emptyCreateFunc = new EmptyCreateFunc();
			m_sphereSphereCF = new SphereSphereCreateFunc();
			m_sphereBoxCF = new SphereBoxCreateFunc();
			m_boxSphereCF = new SwappedSphereBoxCreateFunc();
			m_convexAlgo2DCF = new Convex2dConvex2dCreateFunc(m_simplexSolver, m_pdSolver);
			m_sphereTriangleCF = new SphereTriangleCreateFunc();
			m_triangleSphereCF = new SphereTriangleCreateFunc();
			m_triangleSphereCF.m_swapped = true;
			m_boxBoxCF = new BoxBoxCreateFunc();
			m_convexPlaneCF = new ConvexPlaneCreateFunc();
			m_planeConvexCF = new ConvexPlaneCreateFunc();
			m_planeConvexCF.m_swapped = true;
		}

		public virtual void Cleanup()
		{
		}

		public virtual VoronoiSimplexSolver GetSimplexSolver()
		{
			return m_simplexSolver;
		}

		public virtual CollisionAlgorithmCreateFunc GetCollisionAlgorithmCreateFunc(BroadphaseNativeTypes proxyType0, BroadphaseNativeTypes proxyType1)
		{
			if (proxyType0 == BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE && proxyType1 == BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE)
			{
				return m_sphereSphereCF;
			}
			if (proxyType0 == BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE && proxyType1 == BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE)
			{
				return m_sphereBoxCF;
			}
			if (proxyType0 == BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE && proxyType1 == BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE)
			{
				return m_boxSphereCF;
			}
			if (proxyType0 == BroadphaseNativeTypes.CONVEX_2D_SHAPE_PROXYTYPE && proxyType1 == BroadphaseNativeTypes.CONVEX_2D_SHAPE_PROXYTYPE)
			{
				return m_convexAlgo2DCF;
			}
			if (proxyType0 == BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE && proxyType1 == BroadphaseNativeTypes.TRIANGLE_SHAPE_PROXYTYPE)
			{
				return m_sphereTriangleCF;
			}
			if (proxyType0 == BroadphaseNativeTypes.TRIANGLE_SHAPE_PROXYTYPE && proxyType1 == BroadphaseNativeTypes.SPHERE_SHAPE_PROXYTYPE)
			{
				return m_triangleSphereCF;
			}
			if (proxyType0 == BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE && proxyType1 == BroadphaseNativeTypes.BOX_SHAPE_PROXYTYPE)
			{
				return m_boxBoxCF;
			}
			if (BroadphaseProxy.IsConvex(proxyType0) && proxyType1 == BroadphaseNativeTypes.STATIC_PLANE_PROXYTYPE)
			{
				return m_convexPlaneCF;
			}
			if (BroadphaseProxy.IsConvex(proxyType1) && proxyType0 == BroadphaseNativeTypes.STATIC_PLANE_PROXYTYPE)
			{
				return m_planeConvexCF;
			}
			if (BroadphaseProxy.IsConvex(proxyType0) && BroadphaseProxy.IsConvex(proxyType1))
			{
				return m_convexConvexCreateFunc;
			}
			if (BroadphaseProxy.IsConvex(proxyType0) && BroadphaseProxy.IsConcave(proxyType1))
			{
				return m_convexConcaveCreateFunc;
			}
			if (BroadphaseProxy.IsConvex(proxyType1) && BroadphaseProxy.IsConcave(proxyType0))
			{
				return m_swappedConvexConcaveCreateFunc;
			}
			if (BroadphaseProxy.IsCompound(proxyType0))
			{
				return m_compoundCreateFunc;
			}
			if (BroadphaseProxy.IsCompound(proxyType1))
			{
				return m_swappedCompoundCreateFunc;
			}
			return m_emptyCreateFunc;
		}

		public void SetConvexConvexMultipointIterations()
		{
			SetConvexConvexMultipointIterations(3, 3);
		}

		public void SetConvexConvexMultipointIterations(int numPerturbationIterations, int minimumPointsPerturbationThreshold)
		{
			ConvexConvexCreateFunc convexConvexCreateFunc = (ConvexConvexCreateFunc)m_convexConvexCreateFunc;
			convexConvexCreateFunc.m_numPerturbationIterations = numPerturbationIterations;
			convexConvexCreateFunc.m_minimumPointsPerturbationThreshold = minimumPointsPerturbationThreshold;
		}

		public void SetPlaneConvexMultipointIterations()
		{
			SetPlaneConvexMultipointIterations(3, 3);
		}

		public void SetPlaneConvexMultipointIterations(int numPerturbationIterations, int minimumPointsPerturbationThreshold)
		{
			ConvexPlaneCreateFunc convexPlaneCreateFunc = (ConvexPlaneCreateFunc)m_convexPlaneCF;
			convexPlaneCreateFunc.m_numPerturbationIterations = numPerturbationIterations;
			convexPlaneCreateFunc.m_minimumPointsPerturbationThreshold = minimumPointsPerturbationThreshold;
			ConvexPlaneCreateFunc convexPlaneCreateFunc2 = (ConvexPlaneCreateFunc)m_planeConvexCF;
			convexPlaneCreateFunc2.m_numPerturbationIterations = numPerturbationIterations;
			convexPlaneCreateFunc2.m_minimumPointsPerturbationThreshold = minimumPointsPerturbationThreshold;
		}
	}
}
