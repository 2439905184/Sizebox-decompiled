using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SphereTriangleDetector : IDiscreteCollisionDetectorInterface, IDisposable
	{
		private const float MAX_OVERLAP = 0f;

		private SphereShape m_sphere;

		private TriangleShape m_triangle;

		private float m_contactBreakingThreshold;

		public SphereTriangleDetector()
		{
		}

		public SphereTriangleDetector(SphereShape sphere, TriangleShape triangle, float contactBreakingThreshold)
		{
			m_sphere = sphere;
			m_triangle = triangle;
			m_contactBreakingThreshold = contactBreakingThreshold;
		}

		public void Initialize(SphereShape sphere, TriangleShape triangle, float contactBreakingThreshold)
		{
			m_sphere = sphere;
			m_triangle = triangle;
			m_contactBreakingThreshold = contactBreakingThreshold;
		}

		public bool Collide(ref IndexedVector3 sphereCenter, out IndexedVector3 point, out IndexedVector3 resultNormal, ref float depth, ref float timeOfImpact, float contactBreakingThreshold)
		{
			IndexedVector3[] vertexPtr = m_triangle.GetVertexPtr(0);
			float radius = m_sphere.GetRadius();
			float num = radius + contactBreakingThreshold;
			IndexedVector3 output;
			IndexedVector3.Subtract(out output, ref vertexPtr[1], ref vertexPtr[0]);
			IndexedVector3 output2;
			IndexedVector3.Subtract(out output2, ref vertexPtr[2], ref vertexPtr[0]);
			IndexedVector3 b = new IndexedVector3(output.Y * output2.Z - output.Z * output2.Y, output.Z * output2.X - output.X * output2.Z, output.X * output2.Y - output.Y * output2.X);
			b.Normalize();
			IndexedVector3 output3;
			IndexedVector3.Subtract(out output3, ref sphereCenter, ref vertexPtr[0]);
			float num2 = IndexedVector3.Dot(ref output3, ref b);
			if (num2 < 0f)
			{
				num2 *= -1f;
				b *= -1f;
			}
			bool flag = num2 < num;
			bool flag2 = false;
			IndexedVector3 indexedVector = IndexedVector3.Zero;
			if (flag)
			{
				if (FaceContains(ref sphereCenter, vertexPtr, ref b))
				{
					flag2 = true;
					indexedVector = sphereCenter - b * num2;
				}
				else
				{
					float num3 = num * num;
					for (int i = 0; i < m_triangle.GetNumEdges(); i++)
					{
						IndexedVector3 pa;
						IndexedVector3 pb;
						m_triangle.GetEdge(i, out pa, out pb);
						IndexedVector3 nearest;
						float num4 = SegmentSqrDistance(ref pa, ref pb, ref sphereCenter, out nearest);
						if (num4 < num3)
						{
							flag2 = true;
							indexedVector = nearest;
						}
					}
				}
			}
			if (flag2)
			{
				IndexedVector3 indexedVector2 = sphereCenter - indexedVector;
				float num5 = indexedVector2.LengthSquared();
				if (num5 < num * num)
				{
					if (num5 > 1.1920929E-07f)
					{
						float num6 = (float)Math.Sqrt(num5);
						resultNormal = indexedVector2;
						resultNormal.Normalize();
						point = indexedVector;
						depth = 0f - (radius - num6);
					}
					else
					{
						resultNormal = b;
						point = indexedVector;
						depth = 0f - radius;
					}
					return true;
				}
			}
			resultNormal = new IndexedVector3(0f, 1f, 0f);
			point = IndexedVector3.Zero;
			return false;
		}

		private bool PointInTriangle(IndexedVector3[] vertices, ref IndexedVector3 normal, ref IndexedVector3 p)
		{
			IndexedVector3 indexedVector = vertices[0];
			IndexedVector3 indexedVector2 = vertices[1];
			IndexedVector3 indexedVector3 = vertices[2];
			IndexedVector3 v = indexedVector2 - indexedVector;
			IndexedVector3 v2 = indexedVector3 - indexedVector2;
			IndexedVector3 v3 = indexedVector - indexedVector3;
			IndexedVector3 b = p - indexedVector;
			IndexedVector3 b2 = p - indexedVector2;
			IndexedVector3 b3 = p - indexedVector3;
			IndexedVector3 a = IndexedVector3.Cross(ref v, ref normal);
			IndexedVector3 a2 = IndexedVector3.Cross(ref v2, ref normal);
			IndexedVector3 a3 = IndexedVector3.Cross(ref v3, ref normal);
			float num = IndexedVector3.Dot(ref a, ref b);
			float num2 = IndexedVector3.Dot(ref a2, ref b2);
			float num3 = IndexedVector3.Dot(ref a3, ref b3);
			if ((num > 0f && num2 > 0f && num3 > 0f) || (num <= 0f && num2 <= 0f && num3 <= 0f))
			{
				return true;
			}
			return false;
		}

		private bool FaceContains(ref IndexedVector3 p, IndexedVector3[] vertices, ref IndexedVector3 normal)
		{
			return PointInTriangle(vertices, ref normal, ref p);
		}

		public void GetClosestPoints(ref ClosestPointInput input, IDiscreteCollisionDetectorInterfaceResult output, IDebugDraw debugDraw, bool swapResults)
		{
			IndexedMatrix t = input.m_transformA;
			IndexedMatrix transformB = input.m_transformB;
			float timeOfImpact = 1f;
			float depth = 0f;
			IndexedVector3 sphereCenter = transformB.InverseTimes(ref t)._origin;
			IndexedVector3 point;
			IndexedVector3 resultNormal;
			if (Collide(ref sphereCenter, out point, out resultNormal, ref depth, ref timeOfImpact, m_contactBreakingThreshold))
			{
				if (swapResults)
				{
					IndexedVector3 indexedVector = transformB._basis * resultNormal;
					IndexedVector3 normalOnBInWorld = -indexedVector;
					IndexedVector3 pointInWorld = transformB * point + indexedVector * depth;
					output.AddContactPoint(ref normalOnBInWorld, ref pointInWorld, depth);
				}
				else
				{
					IndexedVector3 normalOnBInWorld2 = transformB._basis * resultNormal;
					IndexedVector3 pointInWorld2 = transformB * point;
					output.AddContactPoint(ref normalOnBInWorld2, ref pointInWorld2, depth);
				}
			}
		}

		public static float SegmentSqrDistance(ref IndexedVector3 from, ref IndexedVector3 to, ref IndexedVector3 p, out IndexedVector3 nearest)
		{
			IndexedVector3 b = p - from;
			IndexedVector3 a = to - from;
			float num = IndexedVector3.Dot(ref a, ref b);
			if (num > 0f)
			{
				float num2 = IndexedVector3.Dot(ref a, ref a);
				if (num < num2)
				{
					num /= num2;
					b -= num * a;
				}
				else
				{
					num = 1f;
					b -= a;
				}
			}
			else
			{
				num = 0f;
			}
			nearest = from + num * a;
			return IndexedVector3.Dot(ref b, ref b);
		}

		public void Dispose()
		{
			BulletGlobals.SphereTriangleDetectorPool.Free(this);
		}
	}
}
