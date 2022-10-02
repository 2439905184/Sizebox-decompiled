using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SubSimplexConvexCast : IConvexCast
	{
		private ISimplexSolverInterface m_simplexSolver;

		private ConvexShape m_convexA;

		private ConvexShape m_convexB;

		private static readonly int MAX_ITERATIONS = 32;

		public SubSimplexConvexCast()
		{
		}

		public void Initialize(ConvexShape shapeA, ConvexShape shapeB, ISimplexSolverInterface simplexSolver)
		{
			m_convexA = shapeA;
			m_convexB = shapeB;
			m_simplexSolver = simplexSolver;
		}

		public SubSimplexConvexCast(ConvexShape shapeA, ConvexShape shapeB, ISimplexSolverInterface simplexSolver)
		{
			m_convexA = shapeA;
			m_convexB = shapeB;
			m_simplexSolver = simplexSolver;
		}

		public virtual bool CalcTimeOfImpact(ref IndexedMatrix fromA, ref IndexedMatrix toA, ref IndexedMatrix fromB, ref IndexedMatrix toB, CastResult result)
		{
			m_simplexSolver.Reset();
			IndexedVector3 indexedVector = toA._origin - fromA._origin;
			IndexedVector3 indexedVector2 = toB._origin - fromB._origin;
			float num = 0f;
			IndexedMatrix indexedMatrix = fromA;
			IndexedMatrix indexedMatrix2 = fromB;
			IndexedVector3 indexedVector3 = indexedVector - indexedVector2;
			IndexedVector3 indexedVector4 = fromA * m_convexA.LocalGetSupportingVertex(-indexedVector3 * fromA._basis);
			IndexedVector3 indexedVector5 = fromB * m_convexB.LocalGetSupportingVertex(indexedVector3 * fromB._basis);
			IndexedVector3 v = indexedVector4 - indexedVector5;
			int mAX_ITERATIONS = MAX_ITERATIONS;
			IndexedVector3 v2 = IndexedVector3.Zero;
			float num2 = v.LengthSquared();
			float num3 = 0.0001f;
			while (num2 > num3 && mAX_ITERATIONS-- > 0)
			{
				indexedVector4 = indexedMatrix * m_convexA.LocalGetSupportingVertex(-v * indexedMatrix._basis);
				indexedVector5 = indexedMatrix2 * m_convexB.LocalGetSupportingVertex(v * indexedMatrix2._basis);
				IndexedVector3 w = indexedVector4 - indexedVector5;
				float num4 = IndexedVector3.Dot(v, w);
				if (num > 1f)
				{
					return false;
				}
				if (num4 > 0f)
				{
					float num5 = IndexedVector3.Dot(v, indexedVector3);
					if (num5 >= -1.4210855E-14f)
					{
						return false;
					}
					num -= num4 / num5;
					indexedMatrix._origin = MathUtil.Interpolate3(fromA._origin, toA._origin, num);
					indexedMatrix2._origin = MathUtil.Interpolate3(fromB._origin, toB._origin, num);
					w = indexedVector4 - indexedVector5;
					v2 = v;
				}
				if (!m_simplexSolver.InSimplex(ref w))
				{
					m_simplexSolver.AddVertex(ref w, ref indexedVector4, ref indexedVector5);
				}
				num2 = ((!m_simplexSolver.Closest(out v)) ? 0f : v.LengthSquared());
			}
			result.m_fraction = num;
			if (v2.LengthSquared() >= 1.4210855E-14f)
			{
				result.m_normal = IndexedVector3.Normalize(v2);
			}
			else
			{
				result.m_normal = IndexedVector3.Zero;
			}
			if (IndexedVector3.Dot(result.m_normal, indexedVector3) >= 0f - result.m_allowedPenetration)
			{
				return false;
			}
			IndexedVector3 p;
			IndexedVector3 p2;
			m_simplexSolver.ComputePoints(out p, out p2);
			result.m_hitPoint = p2;
			return true;
		}
	}
}
