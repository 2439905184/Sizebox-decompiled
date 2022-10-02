using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class LocalTriangleSphereCastCallback : ITriangleCallback, IDisposable
	{
		public IndexedMatrix m_ccdSphereFromTrans;

		public IndexedMatrix m_ccdSphereToTrans;

		public IndexedMatrix m_meshTransform;

		public float m_ccdSphereRadius;

		public float m_hitFraction;

		public virtual bool graphics()
		{
			return false;
		}

		public LocalTriangleSphereCastCallback()
		{
		}

		public LocalTriangleSphereCastCallback(ref IndexedMatrix from, ref IndexedMatrix to, float ccdSphereRadius, float hitFraction)
		{
			m_ccdSphereFromTrans = from;
			m_ccdSphereToTrans = to;
			m_ccdSphereRadius = ccdSphereRadius;
			m_hitFraction = hitFraction;
		}

		public void Initialize(ref IndexedMatrix from, ref IndexedMatrix to, float ccdSphereRadius, float hitFraction)
		{
			m_ccdSphereFromTrans = from;
			m_ccdSphereToTrans = to;
			m_ccdSphereRadius = ccdSphereRadius;
			m_hitFraction = hitFraction;
		}

		public virtual void Cleanup()
		{
		}

		public void ProcessTriangle(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			IndexedMatrix fromB = IndexedMatrix.Identity;
			CastResult castResult = BulletGlobals.CastResultPool.Get();
			castResult.m_fraction = m_hitFraction;
			SphereShape sphereShape = BulletGlobals.SphereShapePool.Get();
			sphereShape.Initialize(m_ccdSphereRadius);
			using (TriangleShape triangleShape = BulletGlobals.TriangleShapePool.Get())
			{
				triangleShape.Initialize(ref triangle[0], ref triangle[1], ref triangle[2]);
				VoronoiSimplexSolver voronoiSimplexSolver = BulletGlobals.VoronoiSimplexSolverPool.Get();
				SubSimplexConvexCast subSimplexConvexCast = BulletGlobals.SubSimplexConvexCastPool.Get();
				subSimplexConvexCast.Initialize(sphereShape, triangleShape, voronoiSimplexSolver);
				if (subSimplexConvexCast.CalcTimeOfImpact(ref m_ccdSphereFromTrans, ref m_ccdSphereToTrans, ref fromB, ref fromB, castResult) && m_hitFraction > castResult.m_fraction)
				{
					m_hitFraction = castResult.m_fraction;
				}
				BulletGlobals.SubSimplexConvexCastPool.Free(subSimplexConvexCast);
				BulletGlobals.VoronoiSimplexSolverPool.Free(voronoiSimplexSolver);
				BulletGlobals.SphereShapePool.Free(sphereShape);
				castResult.Cleanup();
			}
		}

		public void Dispose()
		{
			BulletGlobals.LocalTriangleSphereCastCallbackPool.Free(this);
		}
	}
}
