using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class TriangleConvexcastCallback : ITriangleCallback
	{
		public ConvexShape m_convexShape;

		public IndexedMatrix m_convexShapeFrom;

		public IndexedMatrix m_convexShapeTo;

		public IndexedMatrix m_triangleToWorld;

		public float m_hitFraction;

		public float m_triangleCollisionMargin;

		public float m_allowedPenetration;

		public TriangleConvexcastCallback()
		{
		}

		public TriangleConvexcastCallback(ConvexShape convexShape, ref IndexedMatrix convexShapeFrom, ref IndexedMatrix convexShapeTo, ref IndexedMatrix triangleToWorld, float triangleCollisionMargin)
		{
			m_convexShape = convexShape;
			m_convexShapeFrom = convexShapeFrom;
			m_convexShapeTo = convexShapeTo;
			m_triangleToWorld = triangleToWorld;
			m_triangleCollisionMargin = triangleCollisionMargin;
		}

		public virtual void Initialize(ConvexShape convexShape, ref IndexedMatrix convexShapeFrom, ref IndexedMatrix convexShapeTo, ref IndexedMatrix triangleToWorld, float triangleCollisionMargin)
		{
			m_convexShape = convexShape;
			m_convexShapeFrom = convexShapeFrom;
			m_convexShapeTo = convexShapeTo;
			m_triangleToWorld = triangleToWorld;
			m_triangleCollisionMargin = triangleCollisionMargin;
		}

		public virtual bool graphics()
		{
			return false;
		}

		public virtual void ProcessTriangle(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			TriangleShape triangleShape = new TriangleShape(ref triangle[0], ref triangle[1], ref triangle[2]);
			triangleShape.SetMargin(m_triangleCollisionMargin);
			VoronoiSimplexSolver voronoiSimplexSolver = BulletGlobals.VoronoiSimplexSolverPool.Get();
			GjkEpaPenetrationDepthSolver penetrationDepthSolver = new GjkEpaPenetrationDepthSolver();
			ContinuousConvexCollision continuousConvexCollision = BulletGlobals.ContinuousConvexCollisionPool.Get();
			continuousConvexCollision.Initialize(m_convexShape, triangleShape, voronoiSimplexSolver, penetrationDepthSolver);
			CastResult castResult = BulletGlobals.CastResultPool.Get();
			castResult.m_fraction = 1f;
			if (continuousConvexCollision.CalcTimeOfImpact(ref m_convexShapeFrom, ref m_convexShapeTo, ref m_triangleToWorld, ref m_triangleToWorld, castResult) && castResult.m_normal.LengthSquared() > 0.0001f && castResult.m_fraction < m_hitFraction)
			{
				castResult.m_normal.Normalize();
				ReportHit(ref castResult.m_normal, ref castResult.m_hitPoint, castResult.m_fraction, partId, triangleIndex);
			}
			BulletGlobals.ContinuousConvexCollisionPool.Free(continuousConvexCollision);
			BulletGlobals.VoronoiSimplexSolverPool.Free(voronoiSimplexSolver);
			castResult.Cleanup();
		}

		public virtual void Cleanup()
		{
		}

		public abstract float ReportHit(ref IndexedVector3 hitNormalLocal, ref IndexedVector3 hitPointLocal, float hitFraction, int partId, int triangleIndex);
	}
}
