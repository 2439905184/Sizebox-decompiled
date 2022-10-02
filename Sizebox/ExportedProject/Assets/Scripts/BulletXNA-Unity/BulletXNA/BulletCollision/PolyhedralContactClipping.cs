using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public static class PolyhedralContactClipping
	{
		public static int gActualSATPairTests = 0;

		public static int gExpectedNbTests = 0;

		public static int gActualNbTests = 0;

		public static bool gUseInternalObject = true;

		public static void BoxSupport(ref IndexedVector3 extents, ref IndexedVector3 sv, out IndexedVector3 p)
		{
			p = new IndexedVector3((sv.X < 0f) ? (0f - extents.X) : extents.X, (sv.Y < 0f) ? (0f - extents.Y) : extents.Y, (sv.Z < 0f) ? (0f - extents.Z) : extents.Z);
		}

		public static void InverseTransformPoint3x3(out IndexedVector3 outVec, ref IndexedVector3 input, ref IndexedMatrix tr)
		{
			IndexedBasisMatrix basis = tr._basis;
			IndexedVector3 el = basis._el0;
			IndexedVector3 el2 = basis._el1;
			IndexedVector3 el3 = basis._el2;
			float x = el.X * input.X + el2.X * input.Y + el3.X * input.Z;
			float y = el.Y * input.X + el2.Y * input.Y + el3.Y * input.Z;
			float z = el.Z * input.X + el2.Z * input.Y + el3.Z * input.Z;
			outVec = new IndexedVector3(x, y, z);
		}

		public static bool TestInternalObjects(ref IndexedMatrix trans0, ref IndexedMatrix trans1, ref IndexedVector3 delta_c, ref IndexedVector3 axis, ConvexPolyhedron convex0, ConvexPolyhedron convex1, float dmin)
		{
			float num = delta_c.Dot(ref axis);
			IndexedVector3 outVec;
			InverseTransformPoint3x3(out outVec, ref axis, ref trans0);
			IndexedVector3 outVec2;
			InverseTransformPoint3x3(out outVec2, ref axis, ref trans1);
			IndexedVector3 p;
			BoxSupport(ref convex0.m_extents, ref outVec, out p);
			IndexedVector3 p2;
			BoxSupport(ref convex1.m_extents, ref outVec2, out p2);
			float num2 = p.X * outVec.X + p.Y * outVec.Y + p.Z * outVec.Z;
			float num3 = p2.X * outVec2.X + p2.Y * outVec2.Y + p2.Z * outVec2.Z;
			float num4 = ((num2 > convex0.m_radius) ? num2 : convex0.m_radius);
			float num5 = ((num3 > convex1.m_radius) ? num3 : convex1.m_radius);
			float num6 = num5 + num4;
			float num7 = num6 + num;
			float num8 = num6 - num;
			float num9 = ((num7 < num8) ? num7 : num8);
			if (num9 > dmin)
			{
				return false;
			}
			return true;
		}

		public static void ClipHullAgainstHull(IndexedVector3 separatingNormal, ConvexPolyhedron hullA, ConvexPolyhedron hullB, IndexedMatrix transA, IndexedMatrix transB, float minDist, float maxDist, IDiscreteCollisionDetectorInterfaceResult resultOut)
		{
			ClipHullAgainstHull(ref separatingNormal, hullA, hullB, ref transA, ref transB, minDist, maxDist, resultOut);
		}

		public static void ClipHullAgainstHull(ref IndexedVector3 separatingNormal1, ConvexPolyhedron hullA, ConvexPolyhedron hullB, ref IndexedMatrix transA, ref IndexedMatrix transB, float minDist, float maxDist, IDiscreteCollisionDetectorInterfaceResult resultOut)
		{
			IndexedVector3 separatingNormal2 = separatingNormal1.Normalized();
			IndexedVector3 indexedVector = transA * hullA.m_localCenter;
			IndexedVector3 indexedVector2 = transB * hullB.m_localCenter;
			IndexedVector3 indexedVector5 = indexedVector - indexedVector2;
			int num = -1;
			float num2 = float.MinValue;
			for (int i = 0; i < hullB.m_faces.Count; i++)
			{
				IndexedVector3 indexedVector3 = new IndexedVector3(hullB.m_faces[i].m_plane[0], hullB.m_faces[i].m_plane[1], hullB.m_faces[i].m_plane[2]);
				IndexedVector3 a = transB._basis * indexedVector3;
				float num3 = IndexedVector3.Dot(a, separatingNormal2);
				if (num3 > num2)
				{
					num2 = num3;
					num = i;
				}
			}
			ObjectArray<IndexedVector3> objectArray = new ObjectArray<IndexedVector3>();
			Face face = hullB.m_faces[num];
			int count = face.m_indices.Count;
			for (int j = 0; j < count; j++)
			{
				IndexedVector3 indexedVector4 = hullB.m_vertices[face.m_indices[j]];
				objectArray.Add(transB * indexedVector4);
			}
			if (num >= 0)
			{
				ClipFaceAgainstHull(ref separatingNormal2, hullA, ref transA, objectArray, minDist, maxDist, resultOut);
			}
		}

		public static void ClipFaceAgainstHull(IndexedVector3 separatingNormal, ConvexPolyhedron hullA, IndexedMatrix transA, ObjectArray<IndexedVector3> worldVertsB1, float minDist, float maxDist, IDiscreteCollisionDetectorInterfaceResult resultOut)
		{
			ClipFaceAgainstHull(ref separatingNormal, hullA, ref transA, worldVertsB1, minDist, maxDist, resultOut);
		}

		public static void ClipFaceAgainstHull(ref IndexedVector3 separatingNormal, ConvexPolyhedron hullA, ref IndexedMatrix transA, ObjectArray<IndexedVector3> worldVertsB1, float minDist, float maxDist, IDiscreteCollisionDetectorInterfaceResult resultOut)
		{
			ObjectArray<IndexedVector3> objectArray = new ObjectArray<IndexedVector3>();
			ObjectArray<IndexedVector3> objectArray2 = worldVertsB1;
			ObjectArray<IndexedVector3> objectArray3 = objectArray;
			objectArray3.Capacity = objectArray2.Count;
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < hullA.m_faces.Count; i++)
			{
				IndexedVector3 indexedVector = new IndexedVector3(hullA.m_faces[i].m_plane[0], hullA.m_faces[i].m_plane[1], hullA.m_faces[i].m_plane[2]);
				IndexedVector3 a = transA._basis * indexedVector;
				float num3 = IndexedVector3.Dot(a, separatingNormal);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
			if (num < 0)
			{
				return;
			}
			Face face = hullA.m_faces[num];
			int count2 = objectArray2.Count;
			int count = face.m_indices.Count;
			for (int j = 0; j < count; j++)
			{
				IndexedVector3 indexedVector2 = hullA.m_vertices[face.m_indices[j]];
				IndexedVector3 indexedVector3 = hullA.m_vertices[face.m_indices[(j + 1) % count]];
				IndexedVector3 indexedVector4 = indexedVector2 - indexedVector3;
				IndexedVector3 indexedVector5 = transA._basis * indexedVector4;
				IndexedVector3 v = transA._basis * new IndexedVector3(face.m_plane[0], face.m_plane[1], face.m_plane[2]);
				IndexedVector3 indexedVector6 = -indexedVector5.Cross(v);
				float num4 = 0f - (transA * indexedVector2).Dot(indexedVector6);
				IndexedVector3 planeNormalWS = indexedVector6;
				float planeEqWS = num4;
				ClipFace(objectArray2, objectArray3, ref planeNormalWS, planeEqWS);
				ObjectArray<IndexedVector3> objectArray4 = objectArray2;
				objectArray2 = objectArray3;
				objectArray3 = objectArray4;
				objectArray3.Clear();
			}
			IndexedVector3 indexedVector7 = new IndexedVector3(face.m_plane[0], face.m_plane[1], face.m_plane[2]);
			float num5 = face.m_plane[3];
			IndexedVector3 a2 = transA._basis * indexedVector7;
			float num6 = num5 - IndexedVector3.Dot(a2, transA._origin);
			for (int k = 0; k < objectArray2.Count; k++)
			{
				float num7 = IndexedVector3.Dot(a2, objectArray2[k]) + num6;
				if (num7 <= minDist)
				{
					num7 = minDist;
				}
				if (num7 <= maxDist && num7 >= minDist)
				{
					IndexedVector3 pointInWorld = objectArray2[k];
					resultOut.AddContactPoint(ref separatingNormal, ref pointInWorld, num7);
				}
			}
		}

		public static bool FindSeparatingAxis(ConvexPolyhedron hullA, ConvexPolyhedron hullB, IndexedMatrix transA, IndexedMatrix transB, out IndexedVector3 sep)
		{
			return FindSeparatingAxis(hullA, hullB, ref transA, ref transB, out sep);
		}

		public static bool FindSeparatingAxis(ConvexPolyhedron hullA, ConvexPolyhedron hullB, ref IndexedMatrix transA, ref IndexedMatrix transB, out IndexedVector3 sep)
		{
			gActualSATPairTests++;
			sep = new IndexedVector3(0f, 1f, 0f);
			IndexedVector3 indexedVector = transA * hullA.m_localCenter;
			IndexedVector3 indexedVector2 = transB * hullB.m_localCenter;
			IndexedVector3 delta_c = indexedVector - indexedVector2;
			float num = float.MaxValue;
			int num2 = 0;
			int count = hullA.m_faces.Count;
			for (int i = 0; i < count; i++)
			{
				IndexedVector3 indexedVector3 = new IndexedVector3(hullA.m_faces[i].m_plane[0], hullA.m_faces[i].m_plane[1], hullA.m_faces[i].m_plane[2]);
				IndexedVector3 axis = transA._basis * indexedVector3;
				num2++;
				gExpectedNbTests++;
				if (!gUseInternalObject || TestInternalObjects(ref transA, ref transB, ref delta_c, ref axis, hullA, hullB, num))
				{
					gActualNbTests++;
					float depth;
					if (!TestSepAxis(hullA, hullB, ref transA, ref transB, ref axis, out depth))
					{
						return false;
					}
					if (depth < num)
					{
						num = depth;
						sep = axis;
					}
				}
			}
			int count2 = hullB.m_faces.Count;
			for (int j = 0; j < count2; j++)
			{
				IndexedVector3 indexedVector4 = new IndexedVector3(hullB.m_faces[j].m_plane[0], hullB.m_faces[j].m_plane[1], hullB.m_faces[j].m_plane[2]);
				IndexedVector3 axis2 = transB._basis * indexedVector4;
				num2++;
				gExpectedNbTests++;
				if (!gUseInternalObject || TestInternalObjects(ref transA, ref transB, ref delta_c, ref axis2, hullA, hullB, num))
				{
					gActualNbTests++;
					float depth2;
					if (!TestSepAxis(hullA, hullB, ref transA, ref transB, ref axis2, out depth2))
					{
						return false;
					}
					if (depth2 < num)
					{
						num = depth2;
						sep = axis2;
					}
				}
			}
			int num3 = 0;
			for (int k = 0; k < hullA.m_uniqueEdges.Count; k++)
			{
				IndexedVector3 indexedVector5 = hullA.m_uniqueEdges[k];
				IndexedVector3 v = transA._basis * indexedVector5;
				for (int l = 0; l < hullB.m_uniqueEdges.Count; l++)
				{
					IndexedVector3 indexedVector6 = hullB.m_uniqueEdges[l];
					IndexedVector3 v2 = transB._basis * indexedVector6;
					IndexedVector3 v3 = IndexedVector3.Cross(v, v2);
					num3++;
					if (MathUtil.IsAlmostZero(ref v3))
					{
						continue;
					}
					v3.Normalize();
					gExpectedNbTests++;
					if (!gUseInternalObject || TestInternalObjects(ref transA, ref transB, ref delta_c, ref v3, hullA, hullB, num))
					{
						gActualNbTests++;
						float depth3;
						if (!TestSepAxis(hullA, hullB, ref transA, ref transB, ref v3, out depth3))
						{
							return false;
						}
						if (depth3 < num)
						{
							num = depth3;
							sep = v3;
						}
					}
				}
			}
			IndexedVector3 a = transB._origin - transA._origin;
			if (IndexedVector3.Dot(a, sep) > 0f)
			{
				sep = -sep;
			}
			return true;
		}

		public static void ClipFace(ObjectArray<IndexedVector3> pVtxIn, ObjectArray<IndexedVector3> ppVtxOut, ref IndexedVector3 planeNormalWS, float planeEqWS)
		{
			int count = pVtxIn.Count;
			if (count < 2)
			{
				return;
			}
			IndexedVector3 b = pVtxIn[pVtxIn.Count - 1];
			IndexedVector3 indexedVector = pVtxIn[0];
			float num = IndexedVector3.Dot(ref planeNormalWS, ref b);
			num += planeEqWS;
			for (int i = 0; i < count; i++)
			{
				indexedVector = pVtxIn[i];
				float num2 = IndexedVector3.Dot(ref planeNormalWS, ref indexedVector);
				num2 += planeEqWS;
				if (num < 0f)
				{
					if (num2 < 0f)
					{
						ppVtxOut.Add(indexedVector);
					}
					else
					{
						ppVtxOut.Add(MathUtil.Vector3Lerp(ref b, ref indexedVector, num * 1f / (num - num2)));
					}
				}
				else if (num2 < 0f)
				{
					ppVtxOut.Add(MathUtil.Vector3Lerp(ref b, ref indexedVector, num * 1f / (num - num2)));
					ppVtxOut.Add(indexedVector);
				}
				b = indexedVector;
				num = num2;
			}
		}

		public static bool TestSepAxis(ConvexPolyhedron hullA, ConvexPolyhedron hullB, ref IndexedMatrix transA, ref IndexedMatrix transB, ref IndexedVector3 sep_axis, out float depth)
		{
			float min;
			float max;
			hullA.Project(ref transA, ref sep_axis, out min, out max);
			float min2;
			float max2;
			hullB.Project(ref transB, ref sep_axis, out min2, out max2);
			if (max < min2 || max2 < min)
			{
				depth = 0f;
				return false;
			}
			float num = max - min2;
			float num2 = max2 - min;
			depth = ((num < num2) ? num : num2);
			return true;
		}
	}
}
