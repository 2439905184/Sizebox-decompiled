using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ContinuousConvexCollision : IConvexCast
	{
		private ISimplexSolverInterface m_simplexSolver;

		private IConvexPenetrationDepthSolver m_penetrationDepthSolver;

		private ConvexShape m_convexA;

		private ConvexShape m_convexB1;

		private StaticPlaneShape m_planeShape;

		private static int MAX_ITERATIONS = 64;

		public ContinuousConvexCollision()
		{
		}

		public ContinuousConvexCollision(ConvexShape shapeA, ConvexShape shapeB)
		{
			m_convexA = shapeA;
			m_convexB1 = shapeB;
		}

		public ContinuousConvexCollision(ConvexShape shapeA, ConvexShape shapeB, ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver penetrationDepthSolver)
		{
			m_convexA = shapeA;
			m_convexB1 = shapeB;
			m_simplexSolver = simplexSolver;
			m_penetrationDepthSolver = penetrationDepthSolver;
		}

		public ContinuousConvexCollision(ConvexShape shapeA, StaticPlaneShape plane)
		{
			m_convexA = shapeA;
			m_planeShape = plane;
		}

		public virtual void Initialize(ConvexShape shapeA, ConvexShape shapeB)
		{
			m_convexA = shapeA;
			m_convexB1 = shapeB;
			m_simplexSolver = null;
			m_penetrationDepthSolver = null;
		}

		public virtual void Initialize(ConvexShape shapeA, ConvexShape shapeB, ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver penetrationDepthSolver)
		{
			m_convexA = shapeA;
			m_convexB1 = shapeB;
			m_simplexSolver = simplexSolver;
			m_penetrationDepthSolver = penetrationDepthSolver;
		}

		public void Initialize(ConvexShape shapeA, StaticPlaneShape plane)
		{
			m_convexA = shapeA;
			m_planeShape = plane;
			m_simplexSolver = null;
			m_penetrationDepthSolver = null;
		}

		public virtual bool CalcTimeOfImpact(ref IndexedMatrix fromA, ref IndexedMatrix toA, ref IndexedMatrix fromB, ref IndexedMatrix toB, CastResult result)
		{
			IndexedVector3 linVel;
			IndexedVector3 angVel;
			TransformUtil.CalculateVelocity(ref fromA, ref toA, 1f, out linVel, out angVel);
			IndexedVector3 linVel2;
			IndexedVector3 angVel2;
			TransformUtil.CalculateVelocity(ref fromB, ref toB, 1f, out linVel2, out angVel2);
			float angularMotionDisc = m_convexA.GetAngularMotionDisc();
			float num = ((m_convexB1 != null) ? m_convexB1.GetAngularMotionDisc() : 0f);
			float num2 = angVel.Length() * angularMotionDisc + angVel2.Length() * num;
			IndexedVector3 a = linVel2 - linVel;
			float num3 = a.Length();
			if (MathUtil.FuzzyZero(num3 + num2))
			{
				return false;
			}
			float num4 = 0f;
			new IndexedVector3(1f, 0f, 0f);
			int mAX_ITERATIONS = MAX_ITERATIONS;
			IndexedVector3 zero = IndexedVector3.Zero;
			bool flag = false;
			float num5 = num4;
			int num6 = 0;
			float num7 = 0.001f;
			PointCollector pointCollector = new PointCollector();
			ComputeClosestPoints(ref fromA, ref fromB, pointCollector);
			flag = pointCollector.m_hasResult;
			IndexedVector3 p = pointCollector.m_pointInWorld;
			if (flag)
			{
				float num8 = pointCollector.m_distance + result.m_allowedPenetration;
				zero = pointCollector.m_normalOnBInWorld;
				float num9 = IndexedVector3.Dot(a, zero);
				if (num9 + num2 <= 1.1920929E-07f)
				{
					return false;
				}
				while (num8 > num7)
				{
					if (result.m_debugDrawer != null)
					{
						IndexedVector3 color = new IndexedVector3(1f, 1f, 1f);
						result.m_debugDrawer.DrawSphere(ref p, 0.2f, ref color);
					}
					float num10 = 0f;
					num9 = IndexedVector3.Dot(a, zero);
					if (num9 + num2 <= 1.1920929E-07f)
					{
						return false;
					}
					num10 = num8 / (num9 + num2);
					num4 += num10;
					if (num4 > 1f || num4 < 0f)
					{
						return false;
					}
					if (num4 <= num5)
					{
						return false;
					}
					num5 = num4;
					IndexedMatrix predictedTransform = IndexedMatrix.Identity;
					IndexedMatrix predictedTransform2 = IndexedMatrix.Identity;
					IndexedMatrix identity = IndexedMatrix.Identity;
					TransformUtil.IntegrateTransform(ref fromA, ref linVel, ref angVel, num4, out predictedTransform);
					TransformUtil.IntegrateTransform(ref fromB, ref linVel2, ref angVel2, num4, out predictedTransform2);
					predictedTransform2.InverseTimes(ref predictedTransform);
					if (result.m_debugDrawer != null)
					{
						result.m_debugDrawer.DrawSphere(predictedTransform._origin, 0.2f, new IndexedVector3(1f, 0f, 0f));
					}
					result.DebugDraw(num4);
					PointCollector pointCollector2 = new PointCollector();
					ComputeClosestPoints(ref predictedTransform, ref predictedTransform2, pointCollector2);
					if (pointCollector2.m_hasResult)
					{
						num8 = pointCollector2.m_distance + result.m_allowedPenetration;
						p = pointCollector2.m_pointInWorld;
						zero = pointCollector2.m_normalOnBInWorld;
						num8 = pointCollector2.m_distance;
						num6++;
						if (num6 > mAX_ITERATIONS)
						{
							result.ReportFailure(-2, num6);
							return false;
						}
						continue;
					}
					result.ReportFailure(-1, num6);
					return false;
				}
				result.m_fraction = num4;
				result.m_normal = zero;
				result.m_hitPoint = p;
				return true;
			}
			return false;
		}

		public void ComputeClosestPoints(ref IndexedMatrix transA, ref IndexedMatrix transB, PointCollector pointCollector)
		{
			if (m_convexB1 != null)
			{
				m_simplexSolver.Reset();
				GjkPairDetector gjkPairDetector = new GjkPairDetector(m_convexA, m_convexB1, m_convexA.GetShapeType(), m_convexB1.GetShapeType(), m_convexA.GetMargin(), m_convexB1.GetMargin(), m_simplexSolver, m_penetrationDepthSolver);
				ClosestPointInput input = ClosestPointInput.Default();
				input.m_transformA = transA;
				input.m_transformB = transB;
				gjkPairDetector.GetClosestPoints(ref input, pointCollector, null);
				return;
			}
			ConvexShape convexA = m_convexA;
			StaticPlaneShape planeShape = m_planeShape;
			IndexedVector3 planeNormal = planeShape.GetPlaneNormal();
			float planeConstant = planeShape.GetPlaneConstant();
			IndexedMatrix indexedMatrix = transA;
			IndexedMatrix indexedMatrix2 = transB.Inverse() * indexedMatrix;
			IndexedVector3 indexedVector = convexA.LocalGetSupportingVertex((indexedMatrix.Inverse() * transB)._basis * -planeNormal);
			IndexedVector3 indexedVector2 = indexedMatrix2 * indexedVector;
			float num = IndexedVector3.Dot(planeNormal, indexedVector2) - planeConstant;
			IndexedVector3 indexedVector3 = indexedVector2 - num * planeNormal;
			IndexedVector3 pointInWorld = transB * indexedVector3;
			IndexedVector3 normalOnBInWorld = transB._basis * planeNormal;
			pointCollector.AddContactPoint(ref normalOnBInWorld, ref pointInWorld, num);
		}
	}
}
