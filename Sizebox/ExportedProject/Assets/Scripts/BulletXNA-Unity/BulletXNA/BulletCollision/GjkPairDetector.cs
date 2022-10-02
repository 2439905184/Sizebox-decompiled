using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GjkPairDetector : IDiscreteCollisionDetectorInterface, IDisposable
	{
		public int m_lastUsedMethod;

		public int m_curIter;

		public int m_degenerateSimplex;

		public bool m_catchDegeneracies;

		private IndexedVector3 m_cachedSeparatingAxis;

		private IConvexPenetrationDepthSolver m_penetrationDepthSolver;

		private ISimplexSolverInterface m_simplexSolver;

		private ConvexShape m_minkowskiA;

		private ConvexShape m_minkowskiB;

		private BroadphaseNativeTypes m_shapeTypeA;

		private BroadphaseNativeTypes m_shapeTypeB;

		private float m_marginA;

		private float m_marginB;

		private bool m_ignoreMargin;

		private float m_cachedSeparatingDistance;

		private static readonly float REL_ERROR2 = 1E-06f;

		private static int gNumDeepPenetrationChecks = 0;

		private static int gNumGjkChecks = 0;

		public GjkPairDetector()
		{
		}

		public GjkPairDetector(ConvexShape objectA, ConvexShape objectB, ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver penetrationDepthSolver)
		{
			Initialize(objectA, objectB, simplexSolver, penetrationDepthSolver);
		}

		public void Initialize(ConvexShape objectA, ConvexShape objectB, ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver penetrationDepthSolver)
		{
			m_minkowskiA = objectA;
			m_minkowskiB = objectB;
			m_shapeTypeA = objectA.GetShapeType();
			m_shapeTypeB = objectB.GetShapeType();
			m_marginA = objectA.GetMargin();
			m_marginB = objectB.GetMargin();
			m_cachedSeparatingAxis = new IndexedVector3(0f, 1f, 0f);
			m_simplexSolver = simplexSolver;
			m_penetrationDepthSolver = penetrationDepthSolver;
			m_ignoreMargin = false;
			m_lastUsedMethod = -1;
			m_catchDegeneracies = true;
		}

		public GjkPairDetector(ConvexShape objectA, ConvexShape objectB, BroadphaseNativeTypes shapeTypeA, BroadphaseNativeTypes shapeTypeB, float marginA, float marginB, ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver penetrationDepthSolver)
		{
			Initialize(objectA, objectB, shapeTypeA, shapeTypeB, marginA, marginB, simplexSolver, penetrationDepthSolver);
		}

		public void Initialize(ConvexShape objectA, ConvexShape objectB, BroadphaseNativeTypes shapeTypeA, BroadphaseNativeTypes shapeTypeB, float marginA, float marginB, ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver penetrationDepthSolver)
		{
			m_minkowskiA = objectA;
			m_minkowskiB = objectB;
			m_shapeTypeA = shapeTypeA;
			m_shapeTypeB = shapeTypeB;
			m_marginA = marginA;
			m_marginB = marginB;
			m_cachedSeparatingAxis = new IndexedVector3(0f, 1f, 0f);
			m_simplexSolver = simplexSolver;
			m_penetrationDepthSolver = penetrationDepthSolver;
			m_ignoreMargin = false;
			m_lastUsedMethod = -1;
			m_catchDegeneracies = true;
		}

		public virtual void GetClosestPoints(ref ClosestPointInput input, IDiscreteCollisionDetectorInterfaceResult output, IDebugDraw debugDraw)
		{
			GetClosestPoints(ref input, output, debugDraw, false);
		}

		public virtual void GetClosestPoints(ref ClosestPointInput input, IDiscreteCollisionDetectorInterfaceResult output, IDebugDraw debugDraw, bool swapResults)
		{
			GetClosestPointsNonVirtual(ref input, output, debugDraw);
		}

		public void GetClosestPointsNonVirtual(ref ClosestPointInput input, IDiscreteCollisionDetectorInterfaceResult output, IDebugDraw debugDraw)
		{
			m_cachedSeparatingDistance = 0f;
			float num = 0f;
			IndexedVector3 normalOnBInWorld = IndexedVector3.Zero;
			IndexedVector3 p = IndexedVector3.Zero;
			IndexedVector3 p2 = IndexedVector3.Zero;
			IndexedMatrix transA = input.m_transformA;
			IndexedMatrix transB = input.m_transformB;
			IndexedVector3 value = (transA._origin + transB._origin) * 0.5f;
			IndexedVector3.Subtract(out transA._origin, ref transA._origin, ref value);
			IndexedVector3.Subtract(out transB._origin, ref transB._origin, ref value);
			bool flag = m_minkowskiA.IsConvex2d() && m_minkowskiB.IsConvex2d();
			float num2 = m_marginA;
			float num3 = m_marginB;
			gNumGjkChecks++;
			if (m_ignoreMargin)
			{
				num2 = 0f;
				num3 = 0f;
			}
			m_curIter = 0;
			int num4 = 1000;
			m_cachedSeparatingAxis = new IndexedVector3(0f, 1f, 0f);
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			m_degenerateSimplex = 0;
			m_lastUsedMethod = -1;
			float num5 = 1E+18f;
			float num6 = 0f;
			float num7 = num2 + num3;
			m_simplexSolver.Reset();
			int num8 = 0;
			while (true)
			{
				num8++;
				IndexedVector3 localDir = -m_cachedSeparatingAxis * input.m_transformA._basis;
				IndexedVector3 localDir2 = m_cachedSeparatingAxis * input.m_transformB._basis;
				IndexedVector3 indexedVector = m_minkowskiA.LocalGetSupportVertexWithoutMarginNonVirtual(ref localDir);
				IndexedVector3 indexedVector2 = m_minkowskiB.LocalGetSupportVertexWithoutMarginNonVirtual(ref localDir2);
				IndexedVector3 p3 = transA * indexedVector;
				IndexedVector3 q = transB * indexedVector2;
				if (flag)
				{
					p3.Z = 0f;
					q.Z = 0f;
				}
				IndexedVector3 b = new IndexedVector3(p3.X - q.X, p3.Y - q.Y, p3.Z - q.Z);
				num6 = IndexedVector3.Dot(ref m_cachedSeparatingAxis, ref b);
				if (num6 > 0f && num6 * num6 > num5 * input.m_maximumDistanceSquared)
				{
					m_degenerateSimplex = 10;
					flag3 = true;
					break;
				}
				if (m_simplexSolver.InSimplex(ref b))
				{
					m_degenerateSimplex = 1;
					flag3 = true;
					break;
				}
				float num9 = num5 - num6;
				float num10 = num5 * REL_ERROR2;
				if (num9 <= num10)
				{
					if (num9 <= 0f)
					{
						m_degenerateSimplex = 2;
					}
					else
					{
						m_degenerateSimplex = 11;
					}
					flag3 = true;
					break;
				}
				m_simplexSolver.AddVertex(ref b, ref p3, ref q);
				IndexedVector3 v;
				if (!m_simplexSolver.Closest(out v))
				{
					m_degenerateSimplex = 3;
					flag3 = true;
					break;
				}
				if (v.LengthSquared() < REL_ERROR2)
				{
					m_cachedSeparatingAxis = v;
					m_degenerateSimplex = 6;
					flag3 = true;
					break;
				}
				float num11 = num5;
				num5 = v.LengthSquared();
				if (num11 - num5 <= 1.1920929E-07f * num11)
				{
					flag3 = true;
					m_degenerateSimplex = 12;
					break;
				}
				m_cachedSeparatingAxis = v;
				if (m_curIter++ > num4)
				{
					break;
				}
				if (m_simplexSolver.FullSimplex())
				{
					m_degenerateSimplex = 13;
					break;
				}
			}
			if (flag3)
			{
				m_simplexSolver.ComputePoints(out p, out p2);
				normalOnBInWorld = m_cachedSeparatingAxis;
				float num12 = m_cachedSeparatingAxis.LengthSquared();
				if (num12 < 0.0001f)
				{
					m_degenerateSimplex = 5;
				}
				if (num12 > 1.4210855E-14f)
				{
					float num13 = 1f / (float)Math.Sqrt(num12);
					normalOnBInWorld *= num13;
					float num14 = (float)Math.Sqrt(num5);
					p -= m_cachedSeparatingAxis * (num2 / num14);
					p2 += m_cachedSeparatingAxis * (num3 / num14);
					num = 1f / num13 - num7;
					flag2 = true;
					m_lastUsedMethod = 1;
				}
				else
				{
					m_lastUsedMethod = 2;
				}
			}
			bool flag5 = m_catchDegeneracies && m_penetrationDepthSolver != null && m_degenerateSimplex > 0 && (double)(num + num7) < 0.01;
			if (flag4 && (!flag2 || flag5) && m_penetrationDepthSolver != null)
			{
				IndexedVector3 pa = IndexedVector3.Zero;
				IndexedVector3 pb = IndexedVector3.Zero;
				gNumDeepPenetrationChecks++;
				m_cachedSeparatingAxis = IndexedVector3.Zero;
				if (m_penetrationDepthSolver.CalcPenDepth(m_simplexSolver, m_minkowskiA, m_minkowskiB, ref transA, ref transB, ref m_cachedSeparatingAxis, ref pa, ref pb, debugDraw))
				{
					IndexedVector3 indexedVector3 = pb - pa;
					float num15 = indexedVector3.LengthSquared();
					if (num15 <= 1.4210855E-14f)
					{
						indexedVector3 = m_cachedSeparatingAxis;
						num15 = m_cachedSeparatingAxis.LengthSquared();
					}
					if (num15 > 1.4210855E-14f)
					{
						indexedVector3 /= (float)Math.Sqrt(num15);
						float num16 = 0f - (pa - pb).Length();
						if (!flag2 || num16 < num)
						{
							num = num16;
							p = pa;
							p2 = pb;
							normalOnBInWorld = indexedVector3;
							flag2 = true;
							m_lastUsedMethod = 3;
						}
						else
						{
							m_lastUsedMethod = 8;
						}
					}
					else
					{
						m_lastUsedMethod = 9;
					}
				}
				else if (m_cachedSeparatingAxis.LengthSquared() > 0f)
				{
					float num17 = (pa - pb).Length() - num7;
					if (!flag2 || num17 < num)
					{
						num = num17;
						p = pa;
						p2 = pb;
						p -= m_cachedSeparatingAxis * num2;
						p2 += m_cachedSeparatingAxis * num3;
						normalOnBInWorld = m_cachedSeparatingAxis;
						normalOnBInWorld.Normalize();
						flag2 = true;
						m_lastUsedMethod = 6;
					}
					else
					{
						m_lastUsedMethod = 5;
					}
				}
			}
			if (flag2 && (num < 0f || num * num < input.m_maximumDistanceSquared))
			{
				m_cachedSeparatingAxis = normalOnBInWorld;
				m_cachedSeparatingDistance = num;
				IndexedVector3 pointInWorld = p2 + value;
				output.AddContactPoint(ref normalOnBInWorld, ref pointInWorld, num);
			}
		}

		public void SetMinkowskiA(ConvexShape minkA)
		{
			m_minkowskiA = minkA;
		}

		public void SetMinkowskiB(ConvexShape minkB)
		{
			m_minkowskiB = minkB;
		}

		public void SetCachedSeperatingAxis(IndexedVector3 seperatingAxis)
		{
			SetCachedSeperatingAxis(ref seperatingAxis);
		}

		public void SetCachedSeperatingAxis(ref IndexedVector3 seperatingAxis)
		{
			m_cachedSeparatingAxis = seperatingAxis;
		}

		public IndexedVector3 GetCachedSeparatingAxis()
		{
			return m_cachedSeparatingAxis;
		}

		public float GetCachedSeparatingDistance()
		{
			return m_cachedSeparatingDistance;
		}

		public void SetPenetrationDepthSolver(IConvexPenetrationDepthSolver penetrationDepthSolver)
		{
			m_penetrationDepthSolver = penetrationDepthSolver;
		}

		public void SetIgnoreMargin(bool ignoreMargin)
		{
			m_ignoreMargin = ignoreMargin;
		}

		public virtual void Dispose()
		{
			BulletGlobals.GjkPairDetectorPool.Free(this);
		}
	}
}
