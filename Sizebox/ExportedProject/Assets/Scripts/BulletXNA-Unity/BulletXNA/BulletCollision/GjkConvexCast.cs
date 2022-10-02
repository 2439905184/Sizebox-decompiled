using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GjkConvexCast : IConvexCast, IDisposable
	{
		public const int MAX_ITERATIONS = 32;

		private ISimplexSolverInterface m_simplexSolver;

		private ConvexShape m_convexA;

		private ConvexShape m_convexB;

		public GjkConvexCast()
		{
		}

		public GjkConvexCast(ConvexShape convexA, ConvexShape convexB, ISimplexSolverInterface simplexSolver)
		{
			m_convexA = convexA;
			m_convexB = convexB;
			m_simplexSolver = simplexSolver;
		}

		public void Initialize(ConvexShape convexA, ConvexShape convexB, ISimplexSolverInterface simplexSolver)
		{
			m_convexA = convexA;
			m_convexB = convexB;
			m_simplexSolver = simplexSolver;
		}

		public virtual bool CalcTimeOfImpact(IndexedMatrix fromA, IndexedMatrix toA, IndexedMatrix fromB, IndexedMatrix toB, CastResult result)
		{
			return CalcTimeOfImpact(ref fromA, ref toA, ref fromB, ref toB, result);
		}

		public virtual bool CalcTimeOfImpact(ref IndexedMatrix fromA, ref IndexedMatrix toA, ref IndexedMatrix fromB, ref IndexedMatrix toB, CastResult result)
		{
			m_simplexSolver.Reset();
			IndexedVector3 indexedVector = toA._origin - fromA._origin;
			IndexedVector3 indexedVector2 = toB._origin - fromB._origin;
			float num = 0.001f;
			float num2 = 0f;
			new IndexedVector3(1f, 0f, 0f);
			int num3 = 32;
			IndexedVector3 zero = IndexedVector3.Zero;
			bool flag = false;
			IndexedVector3 indexedVector3 = indexedVector - indexedVector2;
			float num4 = num2;
			int num5 = 0;
			IndexedMatrix identity = IndexedMatrix.Identity;
			PointCollector pointCollector = new PointCollector();
			using (GjkPairDetector gjkPairDetector = BulletGlobals.GjkPairDetectorPool.Get())
			{
				gjkPairDetector.Initialize(m_convexA, m_convexB, m_simplexSolver, null);
				ClosestPointInput input = ClosestPointInput.Default();
				input.m_transformA = fromA;
				input.m_transformB = fromB;
				gjkPairDetector.GetClosestPoints(ref input, pointCollector, null, false);
				flag = pointCollector.m_hasResult;
				IndexedVector3 pointInWorld = pointCollector.m_pointInWorld;
				if (flag)
				{
					float distance = pointCollector.m_distance;
					zero = pointCollector.m_normalOnBInWorld;
					while (distance > num)
					{
						num5++;
						if (num5 > num3)
						{
							return false;
						}
						float num6 = 0f;
						float num7 = IndexedVector3.Dot(indexedVector3, zero);
						num6 = distance / num7;
						num2 -= num6;
						if (num2 > 1f || num2 < 0f)
						{
							return false;
						}
						if (num2 <= num4)
						{
							return false;
						}
						num4 = num2;
						result.DebugDraw(num2);
						input.m_transformA._origin = MathUtil.Interpolate3(fromA._origin, toA._origin, num2);
						input.m_transformB._origin = MathUtil.Interpolate3(fromB._origin, toB._origin, num2);
						gjkPairDetector.GetClosestPoints(ref input, pointCollector, null, false);
						if (pointCollector.m_hasResult)
						{
							if (pointCollector.m_distance < 0f)
							{
								result.m_fraction = num4;
								zero = pointCollector.m_normalOnBInWorld;
								result.m_normal = zero;
								result.m_hitPoint = pointCollector.m_pointInWorld;
								return true;
							}
							pointInWorld = pointCollector.m_pointInWorld;
							zero = pointCollector.m_normalOnBInWorld;
							distance = pointCollector.m_distance;
							continue;
						}
						return false;
					}
					if (IndexedVector3.Dot(zero, indexedVector3) >= 0f - result.m_allowedPenetration)
					{
						return false;
					}
					result.m_fraction = num2;
					result.m_normal = zero;
					result.m_hitPoint = pointInWorld;
					return true;
				}
			}
			return false;
		}

		public void Dispose()
		{
			BulletGlobals.GjkConvexCastPool.Free(this);
		}
	}
}
