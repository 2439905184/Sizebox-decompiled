using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class Convex2dConvex2dAlgorithm : ActivatingCollisionAlgorithm
	{
		private ISimplexSolverInterface m_simplexSolver;

		private IConvexPenetrationDepthSolver m_pdSolver;

		private bool m_ownManifold;

		private PersistentManifold m_manifoldPtr;

		private bool m_lowLevelOfDetail;

		private int m_numPerturbationIterations;

		private int m_minimumPointsPerturbationThreshold;

		public Convex2dConvex2dAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1, ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver pdSolver, int numPerturbationIterations, int minimumPointsPerturbationThreshold)
			: base(ci, body0, body1)
		{
			m_simplexSolver = simplexSolver;
			m_pdSolver = pdSolver;
			m_ownManifold = false;
			m_manifoldPtr = mf;
			m_lowLevelOfDetail = false;
			m_numPerturbationIterations = numPerturbationIterations;
			m_minimumPointsPerturbationThreshold = minimumPointsPerturbationThreshold;
		}

		public override void Cleanup()
		{
			if (m_ownManifold)
			{
				if (m_manifoldPtr != null)
				{
					m_dispatcher.ReleaseManifold(m_manifoldPtr);
					m_manifoldPtr = null;
				}
				m_ownManifold = false;
			}
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			if (m_manifoldPtr == null)
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(body0, body1);
				m_ownManifold = true;
			}
			resultOut.SetPersistentManifold(m_manifoldPtr);
			ConvexShape convexShape = body0.GetCollisionShape() as ConvexShape;
			ConvexShape convexShape2 = body1.GetCollisionShape() as ConvexShape;
			IndexedVector3 zero = IndexedVector3.Zero;
			IndexedVector3 zero2 = IndexedVector3.Zero;
			ClosestPointInput input = ClosestPointInput.Default();
			using (GjkPairDetector gjkPairDetector = BulletGlobals.GjkPairDetectorPool.Get())
			{
				gjkPairDetector.Initialize(convexShape, convexShape2, m_simplexSolver, m_pdSolver);
				gjkPairDetector.SetMinkowskiA(convexShape);
				gjkPairDetector.SetMinkowskiB(convexShape2);
				input.m_maximumDistanceSquared = convexShape.GetMargin() + convexShape2.GetMargin() + m_manifoldPtr.GetContactBreakingThreshold();
				input.m_maximumDistanceSquared *= input.m_maximumDistanceSquared;
				input.m_transformA = body0.GetWorldTransform();
				input.m_transformB = body1.GetWorldTransform();
				gjkPairDetector.GetClosestPoints(ref input, resultOut, dispatchInfo.getDebugDraw(), false);
			}
			if (m_ownManifold)
			{
				resultOut.RefreshContactPoints();
			}
		}

		public override float CalculateTimeOfImpact(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			float num = 1f;
			float num2 = (body0.GetInterpolationWorldTransform()._origin - body0.GetWorldTransform()._origin).LengthSquared();
			float num3 = (body1.GetInterpolationWorldTransform()._origin - body1.GetWorldTransform()._origin).LengthSquared();
			if (num2 < body0.GetCcdSquareMotionThreshold() && num3 < body1.GetCcdSquareMotionThreshold())
			{
				return num;
			}
			ConvexShape convexA = body0.GetCollisionShape() as ConvexShape;
			SphereShape sphereShape = BulletGlobals.SphereShapePool.Get();
			sphereShape.Initialize(body1.GetCcdSweptSphereRadius());
			CastResult castResult = BulletGlobals.CastResultPool.Get();
			VoronoiSimplexSolver voronoiSimplexSolver = BulletGlobals.VoronoiSimplexSolverPool.Get();
			using (GjkConvexCast gjkConvexCast = BulletGlobals.GjkConvexCastPool.Get())
			{
				gjkConvexCast.Initialize(convexA, sphereShape, voronoiSimplexSolver);
				if (gjkConvexCast.CalcTimeOfImpact(body0.GetWorldTransform(), body0.GetInterpolationWorldTransform(), body1.GetWorldTransform(), body1.GetInterpolationWorldTransform(), castResult))
				{
					if (body0.GetHitFraction() > castResult.m_fraction)
					{
						body0.SetHitFraction(castResult.m_fraction);
					}
					if (body1.GetHitFraction() > castResult.m_fraction)
					{
						body1.SetHitFraction(castResult.m_fraction);
					}
					if (num > castResult.m_fraction)
					{
						num = castResult.m_fraction;
					}
				}
				BulletGlobals.VoronoiSimplexSolverPool.Free(voronoiSimplexSolver);
				BulletGlobals.SphereShapePool.Free(sphereShape);
				castResult.Cleanup();
			}
			ConvexShape convexB = body1.GetCollisionShape() as ConvexShape;
			SphereShape sphereShape2 = BulletGlobals.SphereShapePool.Get();
			sphereShape2.Initialize(body0.GetCcdSweptSphereRadius());
			CastResult castResult2 = BulletGlobals.CastResultPool.Get();
			VoronoiSimplexSolver voronoiSimplexSolver2 = BulletGlobals.VoronoiSimplexSolverPool.Get();
			using (GjkConvexCast gjkConvexCast2 = BulletGlobals.GjkConvexCastPool.Get())
			{
				gjkConvexCast2.Initialize(sphereShape2, convexB, voronoiSimplexSolver2);
				if (gjkConvexCast2.CalcTimeOfImpact(body0.GetWorldTransform(), body0.GetInterpolationWorldTransform(), body1.GetWorldTransform(), body1.GetInterpolationWorldTransform(), castResult2))
				{
					if (body0.GetHitFraction() > castResult2.m_fraction)
					{
						body0.SetHitFraction(castResult2.m_fraction);
					}
					if (body1.GetHitFraction() > castResult2.m_fraction)
					{
						body1.SetHitFraction(castResult2.m_fraction);
					}
					if (num > castResult2.m_fraction)
					{
						num = castResult2.m_fraction;
					}
				}
				BulletGlobals.VoronoiSimplexSolverPool.Free(voronoiSimplexSolver2);
				BulletGlobals.SphereShapePool.Free(sphereShape2);
				castResult2.Cleanup();
				return num;
			}
		}

		public override void GetAllContactManifolds(PersistentManifoldArray manifoldArray)
		{
			if (m_manifoldPtr != null && m_ownManifold)
			{
				manifoldArray.Add(m_manifoldPtr);
			}
		}

		public void SetLowLevelOfDetail(bool useLowLevel)
		{
			m_lowLevelOfDetail = useLowLevel;
		}

		public PersistentManifold GetManifold()
		{
			return m_manifoldPtr;
		}
	}
}
