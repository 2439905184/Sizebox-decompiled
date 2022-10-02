using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class Box2dBox2dCollisionAlgorithm : ActivatingCollisionAlgorithm
	{
		private const int b2_maxManifoldPoints = 2;

		private bool m_ownManifold;

		private PersistentManifold m_manifoldPtr;

		public Box2dBox2dCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
			: base(ci)
		{
		}

		public Box2dBox2dCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
			: base(ci, body0, body1)
		{
			m_ownManifold = false;
			m_manifoldPtr = mf;
			if (m_manifoldPtr == null && m_dispatcher.NeedsCollision(body0, body1))
			{
				m_manifoldPtr = m_dispatcher.GetNewManifold(body0, body1);
				m_ownManifold = true;
			}
		}

		public override void Cleanup()
		{
			if (m_ownManifold && m_manifoldPtr != null)
			{
				m_dispatcher.ReleaseManifold(m_manifoldPtr);
			}
			base.Cleanup();
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			if (m_manifoldPtr != null)
			{
				Box2dShape polyA = (Box2dShape)body0.GetCollisionShape();
				Box2dShape polyB = (Box2dShape)body1.GetCollisionShape();
				resultOut.SetPersistentManifold(m_manifoldPtr);
				B2CollidePolygons(ref resultOut, polyA, body0.GetWorldTransform(), polyB, body1.GetWorldTransform());
				if (m_ownManifold)
				{
					resultOut.RefreshContactPoints();
				}
			}
		}

		public override float CalculateTimeOfImpact(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			return 1f;
		}

		public override void GetAllContactManifolds(PersistentManifoldArray manifoldArray)
		{
			if (m_manifoldPtr != null && m_ownManifold)
			{
				manifoldArray.Add(m_manifoldPtr);
			}
		}

		private void B2CollidePolygons(ref ManifoldResult manifold, Box2dShape polyA, IndexedMatrix xfA, Box2dShape polyB, IndexedMatrix xfB)
		{
			B2CollidePolygons(ref manifold, polyA, ref xfA, polyB, ref xfB);
		}

		private void B2CollidePolygons(ref ManifoldResult manifold, Box2dShape polyA, ref IndexedMatrix xfA, Box2dShape polyB, ref IndexedMatrix xfB)
		{
			int edgeIndex = 0;
			float num = FindMaxSeparation(ref edgeIndex, polyA, ref xfA, polyB, ref xfB);
			if (num > 0f)
			{
				return;
			}
			int edgeIndex2 = 0;
			float num2 = FindMaxSeparation(ref edgeIndex2, polyB, ref xfB, polyA, ref xfA);
			if (num2 > 0f)
			{
				return;
			}
			Box2dShape box2dShape;
			Box2dShape poly;
			IndexedMatrix xf;
			IndexedMatrix xf2;
			int num3;
			bool flag;
			if (num2 > 0.98f * num + 0.001f)
			{
				box2dShape = polyB;
				poly = polyA;
				xf = xfB;
				xf2 = xfA;
				num3 = edgeIndex2;
				flag = true;
			}
			else
			{
				box2dShape = polyA;
				poly = polyB;
				xf = xfA;
				xf2 = xfB;
				num3 = edgeIndex;
				flag = false;
			}
			ClipVertex[] array = new ClipVertex[2];
			FindIncidentEdge(array, box2dShape, ref xf, num3, poly, ref xf2);
			int vertexCount = box2dShape.GetVertexCount();
			IndexedVector3[] vertices = box2dShape.GetVertices();
			IndexedVector3 indexedVector = vertices[num3];
			IndexedVector3 indexedVector2 = ((num3 + 1 < vertexCount) ? vertices[num3 + 1] : vertices[0]);
			IndexedVector3 indexedVector5 = indexedVector2 - indexedVector;
			IndexedVector3 a = xf._basis * (indexedVector2 - indexedVector);
			a.Normalize();
			IndexedVector3 indexedVector3 = CrossS(ref a, 1f);
			indexedVector = xf * indexedVector;
			indexedVector2 = xf * indexedVector2;
			float num4 = indexedVector3.Dot(ref indexedVector);
			float offset = 0f - a.Dot(ref indexedVector);
			float offset2 = a.Dot(ref indexedVector2);
			ClipVertex[] array2 = new ClipVertex[2];
			array2[0].v = IndexedVector3.Zero;
			array2[1].v = IndexedVector3.Zero;
			ClipVertex[] array3 = new ClipVertex[2];
			array3[0].v = IndexedVector3.Zero;
			array3[1].v = IndexedVector3.Zero;
			int num5 = ClipSegmentToLine(array2, array, -a, offset);
			if (num5 < 2)
			{
				return;
			}
			num5 = ClipSegmentToLine(array3, array2, a, offset2);
			if (num5 < 2)
			{
				return;
			}
			IndexedVector3 indexedVector4 = (flag ? (-indexedVector3) : indexedVector3);
			int num6 = 0;
			for (int i = 0; i < 2; i++)
			{
				float num7 = indexedVector3.Dot(array3[i].v) - num4;
				if (num7 <= 0f)
				{
					manifold.AddContactPoint(-indexedVector4, array3[i].v, num7);
					num6++;
				}
			}
		}

		public static int ClipSegmentToLine(ClipVertex[] vOut, ClipVertex[] vIn, IndexedVector3 normal, float offset)
		{
			int num = 0;
			float num2 = normal.Dot(vIn[0].v) - offset;
			float num3 = normal.Dot(vIn[1].v) - offset;
			if (num2 <= 0f)
			{
				vOut[num++] = vIn[0];
			}
			if (num3 <= 0f)
			{
				vOut[num++] = vIn[1];
			}
			if (num2 * num3 < 0f)
			{
				float num4 = num2 / (num2 - num3);
				vOut[num].v = vIn[0].v + num4 * (vIn[1].v - vIn[0].v);
				if (num2 > 0f)
				{
					vOut[num].id = vIn[0].id;
				}
				else
				{
					vOut[num].id = vIn[1].id;
				}
				num++;
			}
			return num;
		}

		private static float EdgeSeparation(Box2dShape poly1, ref IndexedMatrix xf1, int edge1, Box2dShape poly2, ref IndexedMatrix xf2)
		{
			IndexedVector3[] vertices = poly1.GetVertices();
			IndexedVector3[] normals = poly1.GetNormals();
			int vertexCount = poly2.GetVertexCount();
			IndexedVector3[] vertices2 = poly2.GetVertices();
			IndexedVector3 indexedVector = xf1._basis * normals[edge1];
			IndexedVector3 v = xf1._basis.Transpose() * indexedVector;
			int num = 0;
			float num2 = 1E+18f;
			for (int i = 0; i < vertexCount; i++)
			{
				float num3 = vertices2[i].Dot(v);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
			IndexedVector3 indexedVector2 = xf1 * vertices[edge1];
			IndexedVector3 indexedVector3 = xf2 * vertices2[num];
			return (indexedVector3 - indexedVector2).Dot(indexedVector);
		}

		private static float FindMaxSeparation(ref int edgeIndex, Box2dShape poly1, ref IndexedMatrix xf1, Box2dShape poly2, ref IndexedMatrix xf2)
		{
			int vertexCount = poly1.GetVertexCount();
			IndexedVector3[] normals = poly1.GetNormals();
			IndexedVector3 indexedVector = xf2 * poly2.GetCentroid() - xf1 * poly1.GetCentroid();
			IndexedVector3 v = xf1._basis.Transpose() * indexedVector;
			int num = 0;
			float num2 = -1E+18f;
			for (int i = 0; i < vertexCount; i++)
			{
				float num3 = normals[i].Dot(ref v);
				if (num3 > num2)
				{
					num2 = num3;
					num = i;
				}
			}
			float num4 = EdgeSeparation(poly1, ref xf1, num, poly2, ref xf2);
			if (num4 > 0f)
			{
				return num4;
			}
			int num5 = ((num - 1 >= 0) ? (num - 1) : (vertexCount - 1));
			float num6 = EdgeSeparation(poly1, ref xf1, num5, poly2, ref xf2);
			if (num6 > 0f)
			{
				return num6;
			}
			int num7 = ((num + 1 < vertexCount) ? (num + 1) : 0);
			float num8 = EdgeSeparation(poly1, ref xf1, num7, poly2, ref xf2);
			if (num8 > 0f)
			{
				return num8;
			}
			int num9;
			int num10;
			float num11;
			if (num6 > num4 && num6 > num8)
			{
				num9 = -1;
				num10 = num5;
				num11 = num6;
			}
			else
			{
				if (!(num8 > num4))
				{
					edgeIndex = num;
					return num4;
				}
				num9 = 1;
				num10 = num7;
				num11 = num8;
			}
			while (true)
			{
				num = ((num9 != -1) ? ((num10 + 1 < vertexCount) ? (num10 + 1) : 0) : ((num10 - 1 >= 0) ? (num10 - 1) : (vertexCount - 1)));
				num4 = EdgeSeparation(poly1, ref xf1, num, poly2, ref xf2);
				if (num4 > 0f)
				{
					return num4;
				}
				if (!(num4 > num11))
				{
					break;
				}
				num10 = num;
				num11 = num4;
			}
			edgeIndex = num10;
			return num11;
		}

		private static void FindIncidentEdge(ClipVertex[] c, Box2dShape poly1, ref IndexedMatrix xf1, int edge1, Box2dShape poly2, ref IndexedMatrix xf2)
		{
			IndexedVector3[] normals = poly1.GetNormals();
			int vertexCount = poly2.GetVertexCount();
			IndexedVector3[] vertices = poly2.GetVertices();
			IndexedVector3[] normals2 = poly2.GetNormals();
			IndexedVector3 indexedVector = xf2._basis.Transpose() * (xf1._basis * normals[edge1]);
			int num = 0;
			float num2 = 1E+18f;
			for (int i = 0; i < vertexCount; i++)
			{
				float num3 = indexedVector.Dot(normals2[i]);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
			int num4 = num;
			int num5 = ((num4 + 1 < vertexCount) ? (num4 + 1) : 0);
			c[0].v = xf2 * vertices[num4];
			c[1].v = xf2 * vertices[num5];
		}

		public static IndexedVector3 CrossS(ref IndexedVector3 a, float s)
		{
			return new IndexedVector3(s * a.Y, (0f - s) * a.X, 0f);
		}
	}
}
