using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ConvexConvexAlgorithm : ActivatingCollisionAlgorithm
	{
		private ISimplexSolverInterface m_simplexSolver;

		private IConvexPenetrationDepthSolver m_pdSolver;

		private bool m_ownManifold;

		private PersistentManifold m_manifoldPtr;

		private bool m_lowLevelOfDetail;

		private int m_numPerturbationIterations;

		private int m_minimumPointsPerturbationThreshold;

		private bool disableCcd;

		private ObjectArray<IndexedVector3> m_vertices = new ObjectArray<IndexedVector3>();

		public ConvexConvexAlgorithm()
		{
		}

		public ConvexConvexAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1, ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver pdSolver, int numPerturbationIterations, int minimumPointsPerturbationThreshold)
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

		public void Initialize(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1, ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver pdSolver, int numPerturbationIterations, int minimumPointsPerturbationThreshold)
		{
			base.Initialize(ci, body0, body1);
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
				}
				m_ownManifold = false;
			}
			m_manifoldPtr = null;
			BulletGlobals.ConvexConvexAlgorithmPool.Free(this);
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
			if (convexShape.GetShapeType() == BroadphaseNativeTypes.CAPSULE_SHAPE_PROXYTYPE && convexShape2.GetShapeType() == BroadphaseNativeTypes.CAPSULE_SHAPE_PROXYTYPE)
			{
				CapsuleShape capsuleShape = convexShape as CapsuleShape;
				CapsuleShape capsuleShape2 = convexShape2 as CapsuleShape;
				float contactBreakingThreshold = m_manifoldPtr.GetContactBreakingThreshold();
				IndexedVector3 normalOnB;
				IndexedVector3 pointOnB;
				float num = CapsuleCapsuleDistance(out normalOnB, out pointOnB, capsuleShape.GetHalfHeight(), capsuleShape.GetRadius(), capsuleShape2.GetHalfHeight(), capsuleShape2.GetRadius(), capsuleShape.GetUpAxis(), capsuleShape2.GetUpAxis(), body0.GetWorldTransform(), body1.GetWorldTransform(), contactBreakingThreshold);
				if (num < contactBreakingThreshold)
				{
					resultOut.AddContactPoint(ref normalOnB, ref pointOnB, num);
				}
				resultOut.RefreshContactPoints();
				return;
			}
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
				if (convexShape.IsPolyhedral() && convexShape2.IsPolyhedral())
				{
					DummyResult dummyResult = default(DummyResult);
					PolyhedralConvexShape polyhedralConvexShape = convexShape as PolyhedralConvexShape;
					PolyhedralConvexShape polyhedralConvexShape2 = convexShape2 as PolyhedralConvexShape;
					if (polyhedralConvexShape.GetConvexPolyhedron() != null && polyhedralConvexShape2.GetConvexPolyhedron() != null)
					{
						float contactBreakingThreshold2 = m_manifoldPtr.GetContactBreakingThreshold();
						float num2 = float.MinValue;
						IndexedVector3 sep = new IndexedVector3(0f, 1f, 0f);
						bool flag = true;
						if (dispatchInfo.m_enableSatConvex)
						{
							flag = PolyhedralContactClipping.FindSeparatingAxis(polyhedralConvexShape.GetConvexPolyhedron(), polyhedralConvexShape2.GetConvexPolyhedron(), body0.GetWorldTransform(), body1.GetWorldTransform(), out sep);
						}
						else
						{
							gjkPairDetector.GetClosestPoints(ref input, dummyResult, dispatchInfo.m_debugDraw);
							float num3 = gjkPairDetector.GetCachedSeparatingAxis().LengthSquared();
							if (num3 > 1.1920929E-07f)
							{
								sep = gjkPairDetector.GetCachedSeparatingAxis() * (1f / num3);
								num2 = gjkPairDetector.GetCachedSeparatingDistance() - convexShape.GetMargin() - convexShape2.GetMargin();
								flag = gjkPairDetector.GetCachedSeparatingDistance() < convexShape.GetMargin() + convexShape2.GetMargin();
							}
						}
						if (flag)
						{
							PolyhedralContactClipping.ClipHullAgainstHull(sep, polyhedralConvexShape.GetConvexPolyhedron(), polyhedralConvexShape2.GetConvexPolyhedron(), body0.GetWorldTransform(), body1.GetWorldTransform(), num2 - contactBreakingThreshold2, contactBreakingThreshold2, resultOut);
						}
						if (m_ownManifold)
						{
							resultOut.RefreshContactPoints();
						}
						return;
					}
					if (polyhedralConvexShape.GetConvexPolyhedron() != null && polyhedralConvexShape2.GetShapeType() == BroadphaseNativeTypes.TRIANGLE_SHAPE_PROXYTYPE)
					{
						m_vertices.Clear();
						TriangleShape triangleShape = polyhedralConvexShape2 as TriangleShape;
						m_vertices.Add(body1.GetWorldTransform() * triangleShape.m_vertices1[0]);
						m_vertices.Add(body1.GetWorldTransform() * triangleShape.m_vertices1[1]);
						m_vertices.Add(body1.GetWorldTransform() * triangleShape.m_vertices1[2]);
						float contactBreakingThreshold3 = m_manifoldPtr.GetContactBreakingThreshold();
						IndexedVector3 separatingNormal = new IndexedVector3(0f, 1f, 0f);
						float num4 = float.MinValue;
						float maxDist = contactBreakingThreshold3;
						bool flag2 = false;
						gjkPairDetector.GetClosestPoints(ref input, dummyResult, dispatchInfo.m_debugDraw);
						float num5 = gjkPairDetector.GetCachedSeparatingAxis().LengthSquared();
						if (num5 > 1.1920929E-07f)
						{
							separatingNormal = gjkPairDetector.GetCachedSeparatingAxis() * (1f / num5);
							num4 = gjkPairDetector.GetCachedSeparatingDistance() - convexShape.GetMargin() - convexShape2.GetMargin();
							flag2 = true;
						}
						if (flag2)
						{
							PolyhedralContactClipping.ClipFaceAgainstHull(separatingNormal, polyhedralConvexShape.GetConvexPolyhedron(), body0.GetWorldTransform(), m_vertices, num4 - contactBreakingThreshold3, maxDist, resultOut);
						}
						if (m_ownManifold)
						{
							resultOut.RefreshContactPoints();
						}
						return;
					}
				}
				gjkPairDetector.GetClosestPoints(ref input, resultOut, dispatchInfo.getDebugDraw(), false);
				if (m_numPerturbationIterations > 0 && resultOut.GetPersistentManifold().GetNumContacts() < m_minimumPointsPerturbationThreshold)
				{
					IndexedVector3 n = gjkPairDetector.GetCachedSeparatingAxis();
					n.Normalize();
					IndexedVector3 p;
					IndexedVector3 q;
					TransformUtil.PlaneSpace1(ref n, out p, out q);
					bool flag3 = true;
					float angularMotionDisc = convexShape.GetAngularMotionDisc();
					float angularMotionDisc2 = convexShape2.GetAngularMotionDisc();
					float num6;
					if (angularMotionDisc < angularMotionDisc2)
					{
						num6 = BulletGlobals.gContactBreakingThreshold / angularMotionDisc;
						flag3 = true;
					}
					else
					{
						num6 = BulletGlobals.gContactBreakingThreshold / angularMotionDisc2;
						flag3 = false;
					}
					if (num6 > (float)Math.PI / 8f)
					{
						num6 = (float)Math.PI / 8f;
					}
					IndexedMatrix unPerturbedTransform = ((!flag3) ? input.m_transformB : input.m_transformA);
					for (int i = 0; i < m_numPerturbationIterations; i++)
					{
						if (p.LengthSquared() > 1.1920929E-07f)
						{
							IndexedQuaternion indexedQuaternion = new IndexedQuaternion(p, num6);
							float angle = (float)i * ((float)Math.PI * 2f / (float)m_numPerturbationIterations);
							IndexedQuaternion indexedQuaternion2 = new IndexedQuaternion(n, angle);
							if (flag3)
							{
								input.m_transformA._basis = new IndexedBasisMatrix(MathUtil.QuaternionInverse(indexedQuaternion2) * indexedQuaternion * indexedQuaternion2) * body0.GetWorldTransform()._basis;
								input.m_transformB = body1.GetWorldTransform();
								input.m_transformB = body1.GetWorldTransform();
								dispatchInfo.m_debugDraw.DrawTransform(ref input.m_transformA, 10f);
							}
							else
							{
								input.m_transformA = body0.GetWorldTransform();
								input.m_transformB._basis = new IndexedBasisMatrix(MathUtil.QuaternionInverse(indexedQuaternion2) * indexedQuaternion * indexedQuaternion2) * body1.GetWorldTransform()._basis;
								dispatchInfo.m_debugDraw.DrawTransform(ref input.m_transformB, 10f);
							}
							PerturbedContactResult output = new PerturbedContactResult(resultOut, ref input.m_transformA, ref input.m_transformB, ref unPerturbedTransform, flag3, dispatchInfo.getDebugDraw());
							gjkPairDetector.GetClosestPoints(ref input, output, dispatchInfo.getDebugDraw(), false);
						}
					}
				}
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
			if (disableCcd)
			{
				return 1f;
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

		public static void SegmentsClosestPoints(out IndexedVector3 ptsVector, out IndexedVector3 offsetA, out IndexedVector3 offsetB, out float tA, out float tB, ref IndexedVector3 translation, ref IndexedVector3 dirA, float hlenA, ref IndexedVector3 dirB, float hlenB)
		{
			float num = IndexedVector3.Dot(ref dirA, ref dirB);
			float num2 = IndexedVector3.Dot(ref dirA, ref translation);
			float num3 = IndexedVector3.Dot(ref dirB, ref translation);
			float num4 = 1f - num * num;
			if (MathUtil.FuzzyZero(num4))
			{
				tA = 0f;
			}
			else
			{
				tA = (num2 - num3 * num) / num4;
				if (tA < 0f - hlenA)
				{
					tA = 0f - hlenA;
				}
				else if (tA > hlenA)
				{
					tA = hlenA;
				}
			}
			tB = tA * num - num3;
			if (tB < 0f - hlenB)
			{
				tB = 0f - hlenB;
				tA = tB * num + num2;
				if (tA < 0f - hlenA)
				{
					tA = 0f - hlenA;
				}
				else if (tA > hlenA)
				{
					tA = hlenA;
				}
			}
			else if (tB > hlenB)
			{
				tB = hlenB;
				tA = tB * num + num2;
				if (tA < 0f - hlenA)
				{
					tA = 0f - hlenA;
				}
				else if (tA > hlenA)
				{
					tA = hlenA;
				}
			}
			offsetA = dirA * tA;
			offsetB = dirB * tB;
			ptsVector = translation - offsetA + offsetB;
		}

		public static float CapsuleCapsuleDistance(out IndexedVector3 normalOnB, out IndexedVector3 pointOnB, float capsuleLengthA, float capsuleRadiusA, float capsuleLengthB, float capsuleRadiusB, int capsuleAxisA, int capsuleAxisB, IndexedMatrix transformA, IndexedMatrix transformB, float distanceThreshold)
		{
			return CapsuleCapsuleDistance(out normalOnB, out pointOnB, capsuleLengthA, capsuleRadiusA, capsuleLengthB, capsuleRadiusB, capsuleAxisA, capsuleAxisB, ref transformA, ref transformB, distanceThreshold);
		}

		public static float CapsuleCapsuleDistance(out IndexedVector3 normalOnB, out IndexedVector3 pointOnB, float capsuleLengthA, float capsuleRadiusA, float capsuleLengthB, float capsuleRadiusB, int capsuleAxisA, int capsuleAxisB, ref IndexedMatrix transformA, ref IndexedMatrix transformB, float distanceThreshold)
		{
			IndexedVector3 dirA = transformA._basis.GetColumn(capsuleAxisA);
			IndexedVector3 origin = transformA._origin;
			IndexedVector3 dirB = transformB._basis.GetColumn(capsuleAxisB);
			IndexedVector3 origin2 = transformB._origin;
			IndexedVector3 translation = origin2 - origin;
			IndexedVector3 ptsVector;
			IndexedVector3 offsetA;
			IndexedVector3 offsetB;
			float tA;
			float tB;
			SegmentsClosestPoints(out ptsVector, out offsetA, out offsetB, out tA, out tB, ref translation, ref dirA, capsuleLengthA, ref dirB, capsuleLengthB);
			float num = ptsVector.Length() - capsuleRadiusA - capsuleRadiusB;
			if (num > distanceThreshold)
			{
				normalOnB = new IndexedVector3(0f, 1f, 0f);
				pointOnB = IndexedVector3.Zero;
				return num;
			}
			float num2 = ptsVector.LengthSquared();
			if (num2 <= 1.4210855E-14f)
			{
				IndexedVector3 q;
				TransformUtil.PlaneSpace1(ref dirA, out normalOnB, out q);
			}
			else
			{
				normalOnB = ptsVector * (0f - MathUtil.RecipSqrt(num2));
			}
			pointOnB = transformB._origin + offsetB + normalOnB * capsuleRadiusB;
			return num;
		}
	}
}
