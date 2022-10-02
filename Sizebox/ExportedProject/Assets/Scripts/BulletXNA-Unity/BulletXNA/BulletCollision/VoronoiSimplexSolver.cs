using System.Collections;
using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class VoronoiSimplexSolver : ISimplexSolverInterface
	{
		public const int VORONOI_SIMPLEX_MAX_VERTS = 5;

		public const float VORONOI_DEFAULT_EQUAL_VERTEX_THRESHOLD = 0.0001f;

		public int m_numVertices;

		public IndexedVector3[] m_simplexVectorW = new IndexedVector3[5];

		public IndexedVector3[] m_simplexPointsP = new IndexedVector3[5];

		public IndexedVector3[] m_simplexPointsQ = new IndexedVector3[5];

		public IndexedVector3 m_cachedP1;

		public IndexedVector3 m_cachedP2;

		public IndexedVector3 m_cachedV;

		public IndexedVector3 m_lastW;

		public bool m_cachedValidClosest;

		public float m_equalVertexThreshold;

		public SubSimplexClosestResult m_cachedBC = new SubSimplexClosestResult();

		public bool m_needsUpdate;

		public VoronoiSimplexSolver()
		{
			m_equalVertexThreshold = 0.0001f;
		}

		public void SetEqualVertexThreshold(float threshold)
		{
			m_equalVertexThreshold = threshold;
		}

		public float GetEqualVertexThreshold()
		{
			return m_equalVertexThreshold;
		}

		public void RemoveVertex(int index)
		{
			m_numVertices--;
			m_simplexVectorW[index] = m_simplexVectorW[m_numVertices];
			m_simplexPointsP[index] = m_simplexPointsP[m_numVertices];
			m_simplexPointsQ[index] = m_simplexPointsQ[m_numVertices];
		}

		public void ReduceVertices(BitArray usedVerts)
		{
			if (NumVertices() >= 4 && !usedVerts.Get(3))
			{
				RemoveVertex(3);
			}
			if (NumVertices() >= 3 && !usedVerts.Get(2))
			{
				RemoveVertex(2);
			}
			if (NumVertices() >= 2 && !usedVerts.Get(1))
			{
				RemoveVertex(1);
			}
			if (NumVertices() >= 1 && !usedVerts.Get(0))
			{
				RemoveVertex(0);
			}
		}

		public bool UpdateClosestVectorAndPoints()
		{
			if (m_needsUpdate)
			{
				m_cachedBC.Reset();
				m_needsUpdate = false;
				switch (NumVertices())
				{
				case 0:
					m_cachedValidClosest = false;
					break;
				case 1:
					m_cachedP1 = m_simplexPointsP[0];
					m_cachedP2 = m_simplexPointsQ[0];
					IndexedVector3.Subtract(out m_cachedV, ref m_cachedP1, ref m_cachedP2);
					m_cachedBC.Reset();
					m_cachedBC.SetBarycentricCoordinates(1f, 0f, 0f, 0f);
					m_cachedValidClosest = m_cachedBC.IsValid();
					break;
				case 2:
				{
					IndexedVector3 value = m_simplexVectorW[0];
					IndexedVector3 value2 = m_simplexVectorW[1];
					IndexedVector3 value3 = IndexedVector3.Zero;
					IndexedVector3 b3 = IndexedVector3.Subtract(ref value3, ref value);
					IndexedVector3 a3 = IndexedVector3.Subtract(ref value2, ref value);
					float num = IndexedVector3.Dot(ref a3, ref b3);
					if (num > 0f)
					{
						float num2 = IndexedVector3.Dot(ref a3, ref a3);
						if (num < num2)
						{
							num /= num2;
							b3 -= num * a3;
							m_cachedBC.m_usedVertices.Set(0, true);
							m_cachedBC.m_usedVertices.Set(1, true);
						}
						else
						{
							num = 1f;
							b3 -= a3;
							m_cachedBC.m_usedVertices.Set(1, true);
						}
					}
					else
					{
						num = 0f;
						m_cachedBC.m_usedVertices.Set(0, true);
					}
					m_cachedBC.SetBarycentricCoordinates(1f - num, num, 0f, 0f);
					IndexedVector3 indexedVector = value + num * a3;
					m_cachedP1 = m_simplexPointsP[0] + num * (m_simplexPointsP[1] - m_simplexPointsP[0]);
					m_cachedP2 = m_simplexPointsQ[0] + num * (m_simplexPointsQ[1] - m_simplexPointsQ[0]);
					m_cachedV = m_cachedP1 - m_cachedP2;
					ReduceVertices(m_cachedBC.m_usedVertices);
					m_cachedValidClosest = m_cachedBC.IsValid();
					break;
				}
				case 3:
				{
					IndexedVector3 p2 = IndexedVector3.Zero;
					IndexedVector3 a2 = m_simplexVectorW[0];
					IndexedVector3 b2 = m_simplexVectorW[1];
					IndexedVector3 c2 = m_simplexVectorW[2];
					ClosestPtPointTriangle(ref p2, ref a2, ref b2, ref c2, ref m_cachedBC);
					m_cachedP1 = m_simplexPointsP[0] * m_cachedBC.m_barycentricCoords.X + m_simplexPointsP[1] * m_cachedBC.m_barycentricCoords.Y + m_simplexPointsP[2] * m_cachedBC.m_barycentricCoords.Z;
					m_cachedP2 = m_simplexPointsQ[0] * m_cachedBC.m_barycentricCoords.X + m_simplexPointsQ[1] * m_cachedBC.m_barycentricCoords.Y + m_simplexPointsQ[2] * m_cachedBC.m_barycentricCoords.Z;
					m_cachedV = m_cachedP1 - m_cachedP2;
					ReduceVertices(m_cachedBC.m_usedVertices);
					m_cachedValidClosest = m_cachedBC.IsValid();
					break;
				}
				case 4:
				{
					IndexedVector3 p = IndexedVector3.Zero;
					IndexedVector3 a = m_simplexVectorW[0];
					IndexedVector3 b = m_simplexVectorW[1];
					IndexedVector3 c = m_simplexVectorW[2];
					IndexedVector3 d = m_simplexVectorW[3];
					if (ClosestPtPointTetrahedron(ref p, ref a, ref b, ref c, ref d, ref m_cachedBC))
					{
						m_cachedP1 = m_simplexPointsP[0] * m_cachedBC.m_barycentricCoords.X + m_simplexPointsP[1] * m_cachedBC.m_barycentricCoords.Y + m_simplexPointsP[2] * m_cachedBC.m_barycentricCoords.Z + m_simplexPointsP[3] * m_cachedBC.m_barycentricCoords.W;
						m_cachedP2 = m_simplexPointsQ[0] * m_cachedBC.m_barycentricCoords.X + m_simplexPointsQ[1] * m_cachedBC.m_barycentricCoords.Y + m_simplexPointsQ[2] * m_cachedBC.m_barycentricCoords.Z + m_simplexPointsQ[3] * m_cachedBC.m_barycentricCoords.W;
						m_cachedV = m_cachedP1 - m_cachedP2;
						ReduceVertices(m_cachedBC.m_usedVertices);
						m_cachedValidClosest = m_cachedBC.IsValid();
					}
					else if (m_cachedBC.m_degenerate)
					{
						m_cachedValidClosest = false;
					}
					else
					{
						m_cachedValidClosest = true;
						m_cachedV = IndexedVector3.Zero;
					}
					break;
				}
				default:
					m_cachedValidClosest = false;
					break;
				}
			}
			return m_cachedValidClosest;
		}

		public bool ClosestPtPointTetrahedron(ref IndexedVector3 p, ref IndexedVector3 a, ref IndexedVector3 b, ref IndexedVector3 c, ref IndexedVector3 d, ref SubSimplexClosestResult finalResult)
		{
			finalResult.m_closestPointOnSimplex = p;
			finalResult.m_usedVertices.SetAll(true);
			int num = PointOutsideOfPlane(ref p, ref a, ref b, ref c, ref d);
			int num2 = PointOutsideOfPlane(ref p, ref a, ref c, ref d, ref b);
			int num3 = PointOutsideOfPlane(ref p, ref a, ref d, ref b, ref c);
			int num4 = PointOutsideOfPlane(ref p, ref b, ref d, ref c, ref a);
			if (num < 0 || num2 < 0 || num3 < 0 || num4 < 0)
			{
				finalResult.m_degenerate = true;
				return false;
			}
			if (num == 0 && num2 == 0 && num3 == 0 && num4 == 0)
			{
				return false;
			}
			SubSimplexClosestResult result = BulletGlobals.SubSimplexClosestResultPool.Get();
			result.Reset();
			float num5 = float.MaxValue;
			if (num != 0)
			{
				ClosestPtPointTriangle(ref p, ref a, ref b, ref c, ref result);
				IndexedVector3 closestPointOnSimplex = result.m_closestPointOnSimplex;
				float num6 = (closestPointOnSimplex - p).LengthSquared();
				if (num6 < num5)
				{
					num5 = num6;
					finalResult.m_closestPointOnSimplex = closestPointOnSimplex;
					finalResult.m_usedVertices.SetAll(false);
					finalResult.m_usedVertices.Set(0, result.m_usedVertices.Get(0));
					finalResult.m_usedVertices.Set(1, result.m_usedVertices.Get(1));
					finalResult.m_usedVertices.Set(2, result.m_usedVertices.Get(2));
					finalResult.SetBarycentricCoordinates(result.m_barycentricCoords.X, result.m_barycentricCoords.Y, result.m_barycentricCoords.Z, 0f);
				}
			}
			if (num2 != 0)
			{
				ClosestPtPointTriangle(ref p, ref a, ref c, ref d, ref result);
				IndexedVector3 closestPointOnSimplex2 = result.m_closestPointOnSimplex;
				float num7 = (closestPointOnSimplex2 - p).LengthSquared();
				if (num7 < num5)
				{
					num5 = num7;
					finalResult.m_closestPointOnSimplex = closestPointOnSimplex2;
					finalResult.m_usedVertices.SetAll(false);
					finalResult.m_usedVertices.Set(0, result.m_usedVertices.Get(0));
					finalResult.m_usedVertices.Set(2, result.m_usedVertices.Get(1));
					finalResult.m_usedVertices.Set(3, result.m_usedVertices.Get(2));
					finalResult.SetBarycentricCoordinates(result.m_barycentricCoords.X, 0f, result.m_barycentricCoords.Y, result.m_barycentricCoords.Z);
				}
			}
			if (num3 != 0)
			{
				ClosestPtPointTriangle(ref p, ref a, ref d, ref b, ref result);
				IndexedVector3 closestPointOnSimplex3 = result.m_closestPointOnSimplex;
				float num8 = (closestPointOnSimplex3 - p).LengthSquared();
				if (num8 < num5)
				{
					num5 = num8;
					finalResult.m_closestPointOnSimplex = closestPointOnSimplex3;
					finalResult.m_usedVertices.SetAll(false);
					finalResult.m_usedVertices.Set(0, result.m_usedVertices.Get(0));
					finalResult.m_usedVertices.Set(1, result.m_usedVertices.Get(2));
					finalResult.m_usedVertices.Set(3, result.m_usedVertices.Get(1));
					finalResult.SetBarycentricCoordinates(result.m_barycentricCoords.X, result.m_barycentricCoords.Z, 0f, result.m_barycentricCoords.Y);
				}
			}
			if (num4 != 0)
			{
				ClosestPtPointTriangle(ref p, ref b, ref d, ref c, ref result);
				IndexedVector3 closestPointOnSimplex4 = result.m_closestPointOnSimplex;
				float num9 = (closestPointOnSimplex4 - p).LengthSquared();
				if (num9 < num5)
				{
					num5 = num9;
					finalResult.m_closestPointOnSimplex = closestPointOnSimplex4;
					finalResult.m_usedVertices.SetAll(false);
					finalResult.m_usedVertices.Set(1, result.m_usedVertices.Get(0));
					finalResult.m_usedVertices.Set(2, result.m_usedVertices.Get(2));
					finalResult.m_usedVertices.Set(3, result.m_usedVertices.Get(1));
					finalResult.SetBarycentricCoordinates(0f, result.m_barycentricCoords.X, result.m_barycentricCoords.Z, result.m_barycentricCoords.Y);
				}
			}
			BulletGlobals.SubSimplexClosestResultPool.Free(result);
			if (finalResult.m_usedVertices.Get(0) && finalResult.m_usedVertices.Get(1) && finalResult.m_usedVertices.Get(2) && finalResult.m_usedVertices.Get(3))
			{
				return true;
			}
			return true;
		}

		public int PointOutsideOfPlane(ref IndexedVector3 p, ref IndexedVector3 a, ref IndexedVector3 b, ref IndexedVector3 c, ref IndexedVector3 d)
		{
			IndexedVector3 output;
			IndexedVector3.Subtract(out output, ref b, ref a);
			IndexedVector3 output2;
			IndexedVector3.Subtract(out output2, ref c, ref a);
			IndexedVector3 r;
			IndexedVector3.Cross(out r, ref output, ref output2);
			IndexedVector3 output3;
			IndexedVector3.Subtract(out output3, ref p, ref a);
			IndexedVector3 output4;
			IndexedVector3.Subtract(out output4, ref d, ref a);
			float num = IndexedVector3.Dot(ref output3, ref r);
			float num2 = IndexedVector3.Dot(ref output4, ref r);
			if (num2 * num2 < 9.999999E-09f)
			{
				return -1;
			}
			if (!(num * num2 < 0f))
			{
				return 0;
			}
			return 1;
		}

		public bool ClosestPtPointTriangle(ref IndexedVector3 p, ref IndexedVector3 a, ref IndexedVector3 b, ref IndexedVector3 c, ref SubSimplexClosestResult result)
		{
			result.m_usedVertices.SetAll(false);
			IndexedVector3 output;
			IndexedVector3.Subtract(out output, ref b, ref a);
			IndexedVector3 output2;
			IndexedVector3.Subtract(out output2, ref c, ref a);
			IndexedVector3 output3;
			IndexedVector3.Subtract(out output3, ref p, ref a);
			float num = IndexedVector3.Dot(ref output, ref output3);
			float num2 = IndexedVector3.Dot(ref output2, ref output3);
			if (num <= 0f && num2 <= 0f)
			{
				result.m_closestPointOnSimplex = a;
				result.m_usedVertices.Set(0, true);
				result.SetBarycentricCoordinates(1f, 0f, 0f, 0f);
				return true;
			}
			IndexedVector3 output4;
			IndexedVector3.Subtract(out output4, ref p, ref b);
			float num3 = IndexedVector3.Dot(ref output, ref output4);
			float num4 = IndexedVector3.Dot(ref output2, ref output4);
			if (num3 >= 0f && num4 <= num3)
			{
				result.m_closestPointOnSimplex = b;
				result.m_usedVertices.Set(1, true);
				result.SetBarycentricCoordinates(0f, 1f, 0f, 0f);
				return true;
			}
			float num5 = num * num4 - num3 * num2;
			if (num5 <= 0f && num >= 0f && num3 <= 0f)
			{
				float num6 = num / (num - num3);
				result.m_closestPointOnSimplex = a + num6 * output;
				result.m_usedVertices.Set(0, true);
				result.m_usedVertices.Set(1, true);
				result.SetBarycentricCoordinates(1f - num6, num6, 0f, 0f);
				return true;
			}
			IndexedVector3 output5;
			IndexedVector3.Subtract(out output5, ref p, ref c);
			float num7 = IndexedVector3.Dot(ref output, ref output5);
			float num8 = IndexedVector3.Dot(ref output2, ref output5);
			if (num8 >= 0f && num7 <= num8)
			{
				result.m_closestPointOnSimplex = c;
				result.m_usedVertices.Set(2, true);
				result.SetBarycentricCoordinates(0f, 0f, 1f, 0f);
				return true;
			}
			float num9 = num7 * num2 - num * num8;
			if (num9 <= 0f && num2 >= 0f && num8 <= 0f)
			{
				float num10 = num2 / (num2 - num8);
				result.m_closestPointOnSimplex = a + num10 * output2;
				result.m_usedVertices.Set(0, true);
				result.m_usedVertices.Set(2, true);
				result.SetBarycentricCoordinates(1f - num10, 0f, num10, 0f);
				return true;
			}
			float num11 = num3 * num8 - num7 * num4;
			if (num11 <= 0f && num4 - num3 >= 0f && num7 - num8 >= 0f)
			{
				float num12 = (num4 - num3) / (num4 - num3 + (num7 - num8));
				result.m_closestPointOnSimplex = b + num12 * (c - b);
				result.m_usedVertices.Set(1, true);
				result.m_usedVertices.Set(2, true);
				result.SetBarycentricCoordinates(0f, 1f - num12, num12, 0f);
				return true;
			}
			float num13 = 1f / (num11 + num9 + num5);
			float num14 = num9 * num13;
			float num15 = num5 * num13;
			result.m_closestPointOnSimplex = a + output * num14 + output2 * num15;
			result.m_usedVertices.Set(0, true);
			result.m_usedVertices.Set(1, true);
			result.m_usedVertices.Set(2, true);
			result.SetBarycentricCoordinates(1f - num14 - num15, num14, num15, 0f);
			return true;
		}

		public void Reset()
		{
			m_cachedValidClosest = false;
			m_numVertices = 0;
			m_needsUpdate = true;
			m_lastW = MathUtil.MAX_VECTOR;
			m_cachedBC.Reset();
		}

		public void AddVertex(ref IndexedVector3 w, ref IndexedVector3 p, ref IndexedVector3 q)
		{
			m_lastW = w;
			m_needsUpdate = true;
			m_simplexVectorW[m_numVertices] = w;
			m_simplexPointsP[m_numVertices] = p;
			m_simplexPointsQ[m_numVertices] = q;
			m_numVertices++;
		}

		public bool Closest(out IndexedVector3 v)
		{
			bool result = UpdateClosestVectorAndPoints();
			v = m_cachedV;
			return result;
		}

		public float MaxVertex()
		{
			int num = NumVertices();
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				float num3 = m_simplexVectorW[i].LengthSquared();
				if (num2 < num3)
				{
					num2 = num3;
				}
			}
			return num2;
		}

		public bool FullSimplex()
		{
			return m_numVertices == 4;
		}

		public int GetSimplex(IList<IndexedVector3> pBuf, IList<IndexedVector3> qBuf, IList<IndexedVector3> yBuf)
		{
			for (int i = 0; i < NumVertices(); i++)
			{
				yBuf[i] = m_simplexVectorW[i];
				pBuf[i] = m_simplexPointsP[i];
				qBuf[i] = m_simplexPointsQ[i];
			}
			return NumVertices();
		}

		public bool InSimplex(ref IndexedVector3 w)
		{
			bool result = false;
			int num = NumVertices();
			for (int i = 0; i < num; i++)
			{
				if ((w - m_simplexVectorW[i]).LengthSquared() <= m_equalVertexThreshold)
				{
					result = true;
				}
			}
			if (w == m_lastW)
			{
				return true;
			}
			return result;
		}

		public void BackupClosest(ref IndexedVector3 v)
		{
			v = m_cachedV;
		}

		public bool EmptySimplex()
		{
			return NumVertices() == 0;
		}

		public void ComputePoints(out IndexedVector3 p1, out IndexedVector3 p2)
		{
			UpdateClosestVectorAndPoints();
			p1 = m_cachedP1;
			p2 = m_cachedP2;
		}

		public int NumVertices()
		{
			return m_numVertices;
		}
	}
}
