using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision.CollisionDispatch
{
	public class InternalEdgeUtility
	{
		public class ConnectivityProcessor : ITriangleCallback
		{
			public int m_partIdA;

			public int m_triangleIndexA;

			public IndexedVector3[] m_triangleVerticesA;

			public TriangleInfoMap m_triangleInfoMap;

			public virtual void ProcessTriangle(IndexedVector3[] triangle, int partId, int triangleIndex)
			{
				if (m_partIdA == partId && m_triangleIndexA == triangleIndex)
				{
					return;
				}
				int num = 0;
				int[] array = new int[3] { -1, -1, -1 };
				int[] array2 = new int[3] { -1, -1, -1 };
				float num2 = IndexedVector3.Cross(triangle[1] - triangle[0], triangle[2] - triangle[0]).LengthSquared();
				if (num2 < m_triangleInfoMap.m_equalVertexThreshold)
				{
					return;
				}
				float num3 = IndexedVector3.Cross(m_triangleVerticesA[1] - m_triangleVerticesA[0], m_triangleVerticesA[2] - m_triangleVerticesA[0]).LengthSquared();
				if (num3 < m_triangleInfoMap.m_equalVertexThreshold)
				{
					return;
				}
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						if ((m_triangleVerticesA[i] - triangle[j]).LengthSquared() < m_triangleInfoMap.m_equalVertexThreshold)
						{
							array[num] = i;
							array2[num] = j;
							num++;
							if (num >= 3)
							{
								return;
							}
						}
					}
					if (num >= 3)
					{
						return;
					}
				}
				switch (num)
				{
				case 2:
				{
					if (array[0] == 0 && array[1] == 2)
					{
						array[0] = 2;
						array[1] = 0;
						int num4 = array2[1];
						array2[1] = array2[0];
						array2[0] = num4;
					}
					int hash = GetHash(m_partIdA, m_triangleIndexA);
					TriangleInfo triangleInfo = null;
					if (m_triangleInfoMap.ContainsKey(hash))
					{
						triangleInfo = m_triangleInfoMap[hash];
					}
					else
					{
						triangleInfo = new TriangleInfo();
						m_triangleInfoMap[hash] = triangleInfo;
					}
					int num5 = array[0] + array[1];
					int num6 = 3 - num5;
					IndexedVector3 v = new IndexedVector3(m_triangleVerticesA[array[1]] - m_triangleVerticesA[array[0]]);
					TriangleShape triangleShape = new TriangleShape(m_triangleVerticesA[0], m_triangleVerticesA[1], m_triangleVerticesA[2]);
					int num7 = 3 - (array2[0] + array2[1]);
					TriangleShape triangleShape2 = new TriangleShape(triangle[array2[1]], triangle[array2[0]], triangle[num7]);
					IndexedVector3 normal;
					triangleShape.CalcNormal(out normal);
					IndexedVector3 normal2;
					triangleShape2.CalcNormal(out normal2);
					v.Normalize();
					IndexedVector3 normalA = IndexedVector3.Normalize(IndexedVector3.Cross(v, normal));
					IndexedVector3 b = m_triangleVerticesA[num6] - m_triangleVerticesA[array[0]];
					if (IndexedVector3.Dot(normalA, b) < 0f)
					{
						normalA *= -1f;
					}
					IndexedVector3 normalB = IndexedVector3.Cross(v, normal2).Normalized();
					IndexedVector3 b2 = triangle[num7] - triangle[array2[0]];
					if (IndexedVector3.Dot(normalB, b2) < 0f)
					{
						normalB *= -1f;
					}
					float num8 = 0f;
					float num9 = 0f;
					IndexedVector3 indexedVector = IndexedVector3.Cross(normalA, normalB);
					float num10 = indexedVector.LengthSquared();
					float num11 = 0f;
					bool flag = false;
					if (num10 < m_triangleInfoMap.m_planarEpsilon)
					{
						num8 = 0f;
						num9 = 0f;
					}
					else
					{
						indexedVector.Normalize();
						IndexedVector3 edgeA = IndexedVector3.Cross(indexedVector, normalA);
						edgeA.Normalize();
						num8 = GetAngle(ref edgeA, ref normalA, ref normalB);
						num9 = (float)Math.PI - num8;
						float num12 = IndexedVector3.Dot(normal, normalB);
						flag = num12 < 0f;
						num11 = (flag ? num9 : (0f - num9));
						IndexedQuaternion q = new IndexedQuaternion(indexedVector, 0f - num11);
						IndexedMatrix.CreateFromQuaternion(q);
						IndexedVector3 indexedVector5 = new IndexedBasisMatrix(q) * normal;
					}
					switch (num5)
					{
					case 1:
					{
						IndexedVector3 axis2 = m_triangleVerticesA[0] - m_triangleVerticesA[1];
						IndexedQuaternion rotation2 = new IndexedQuaternion(axis2, 0f - num11);
						IndexedVector3 indexedVector3 = MathUtil.QuatRotate(rotation2, normal);
						float num13 = IndexedVector3.Dot(indexedVector3, normal2);
						if (num13 < 0f)
						{
							indexedVector3 *= -1f;
							triangleInfo.m_flags |= 8;
						}
						if ((indexedVector3 - normal2).Length() > 0.0001f)
						{
							Console.WriteLine("warning: normals not identical");
						}
						triangleInfo.m_edgeV0V1Angle = 0f - num11;
						if (flag)
						{
							triangleInfo.m_flags |= 1;
						}
						break;
					}
					case 2:
					{
						IndexedVector3 axis3 = m_triangleVerticesA[2] - m_triangleVerticesA[0];
						IndexedQuaternion rotation3 = new IndexedQuaternion(axis3, 0f - num11);
						IndexedVector3 indexedVector4 = MathUtil.QuatRotate(rotation3, normal);
						if (IndexedVector3.Dot(indexedVector4, normal2) < 0f)
						{
							indexedVector4 *= -1f;
							triangleInfo.m_flags |= 32;
						}
						if ((double)(indexedVector4 - normal2).Length() > 0.0001)
						{
							Console.WriteLine("warning: normals not identical");
						}
						triangleInfo.m_edgeV2V0Angle = 0f - num11;
						if (flag)
						{
							triangleInfo.m_flags |= 4;
						}
						break;
					}
					case 3:
					{
						IndexedVector3 axis = m_triangleVerticesA[1] - m_triangleVerticesA[2];
						IndexedQuaternion rotation = new IndexedQuaternion(axis, 0f - num11);
						IndexedVector3 indexedVector2 = MathUtil.QuatRotate(rotation, normal);
						if (IndexedVector3.Dot(indexedVector2, normal2) < 0f)
						{
							triangleInfo.m_flags |= 16;
							indexedVector2 *= -1f;
						}
						if ((double)(indexedVector2 - normal2).Length() > 0.0001)
						{
							Console.WriteLine("warning: normals not identical");
						}
						triangleInfo.m_edgeV1V2Angle = 0f - num11;
						if (flag)
						{
							triangleInfo.m_flags |= 2;
						}
						break;
					}
					}
					break;
				}
				case 0:
				case 1:
					break;
				}
			}

			public virtual bool graphics()
			{
				return false;
			}

			public virtual void Cleanup()
			{
			}
		}

		public enum InternalEdgeAdjustFlags
		{
			BT_TRIANGLE_CONVEX_BACKFACE_NONE = 0,
			BT_TRIANGLE_CONVEX_BACKFACE_MODE = 1,
			BT_TRIANGLE_CONCAVE_DOUBLE_SIDED = 2,
			BT_TRIANGLE_CONVEX_DOUBLE_SIDED = 4
		}

		public static void DebugDrawLine(IndexedVector3 from, IndexedVector3 to, IndexedVector3 color)
		{
			if (BulletGlobals.gDebugDraw != null)
			{
				BulletGlobals.gDebugDraw.DrawLine(ref from, ref to, ref color);
			}
		}

		public static void DebugDrawLine(ref IndexedVector3 from, ref IndexedVector3 to, ref IndexedVector3 color)
		{
			if (BulletGlobals.gDebugDraw != null)
			{
				BulletGlobals.gDebugDraw.DrawLine(ref from, ref to, ref color);
			}
		}

		public static int GetHash(int partId, int triangleIndex)
		{
			return (partId << 21) | triangleIndex;
		}

		public static float GetAngle(ref IndexedVector3 edgeA, ref IndexedVector3 normalA, ref IndexedVector3 normalB)
		{
			IndexedVector3 b = edgeA;
			IndexedVector3 b2 = normalA;
			IndexedVector3 a = normalB;
			return (float)Math.Atan2(IndexedVector3.Dot(ref a, ref b), IndexedVector3.Dot(ref a, ref b2));
		}

		public static void GenerateInternalEdgeInfo(BvhTriangleMeshShape trimeshShape, TriangleInfoMap triangleInfoMap)
		{
			if (trimeshShape.GetTriangleInfoMap() != null)
			{
				return;
			}
			trimeshShape.SetTriangleInfoMap(triangleInfoMap);
			StridingMeshInterface meshInterface = trimeshShape.GetMeshInterface();
			IndexedVector3 scaling = meshInterface.GetScaling();
			for (int i = 0; i < meshInterface.GetNumSubParts(); i++)
			{
				object vertexbase = null;
				int numverts = 0;
				PHY_ScalarType type = PHY_ScalarType.PHY_INTEGER;
				int stride = 0;
				object indexbase = null;
				int indexstride = 0;
				int numfaces = 0;
				PHY_ScalarType indicestype = PHY_ScalarType.PHY_INTEGER;
				IndexedVector3[] array = new IndexedVector3[3];
				meshInterface.GetLockedReadOnlyVertexIndexBase(out vertexbase, out numverts, out type, out stride, out indexbase, out indexstride, out numfaces, out indicestype, i);
				PHY_ScalarType pHY_ScalarType = indicestype;
				if (pHY_ScalarType != PHY_ScalarType.PHY_INTEGER)
				{
					continue;
				}
				int[] rawArray = ((ObjectArray<int>)indexbase).GetRawArray();
				IndexedVector3 aabbMin;
				IndexedVector3 aabbMax;
				if (vertexbase is ObjectArray<IndexedVector3>)
				{
					IndexedVector3[] rawArray2 = (vertexbase as ObjectArray<IndexedVector3>).GetRawArray();
					for (int j = 0; j < numfaces; j++)
					{
						int num = rawArray[j];
						int num2 = rawArray[j + 1];
						int num3 = rawArray[j + 2];
						array[0] = new IndexedVector3(rawArray2[num]) * scaling;
						array[1] = new IndexedVector3(rawArray2[num2]) * scaling;
						array[2] = new IndexedVector3(rawArray2[num3]) * scaling;
						ProcessResult(array, out aabbMin, out aabbMax, trimeshShape, i, j, triangleInfoMap);
					}
				}
				else if (vertexbase is ObjectArray<float>)
				{
					float[] rawArray3 = (vertexbase as ObjectArray<float>).GetRawArray();
					for (int k = 0; k < numfaces; k++)
					{
						array[0] = new IndexedVector3(rawArray3[rawArray[k]], rawArray3[rawArray[k] + 1], rawArray3[rawArray[k] + 2]) * scaling;
						array[1] = new IndexedVector3(rawArray3[rawArray[k + 1]], rawArray3[rawArray[k + 1] + 1], rawArray3[rawArray[k + 1] + 2]) * scaling;
						array[2] = new IndexedVector3(rawArray3[rawArray[k + 2]], rawArray3[rawArray[k + 2] + 1], rawArray3[rawArray[k + 2] + 2]) * scaling;
						ProcessResult(array, out aabbMin, out aabbMax, trimeshShape, i, k, triangleInfoMap);
					}
				}
			}
		}

		private static void ProcessResult(IndexedVector3[] triangleVerts, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax, BvhTriangleMeshShape trimeshShape, int partId, int triangleIndex, TriangleInfoMap triangleInfoMap)
		{
			aabbMin = MathUtil.MAX_VECTOR;
			aabbMax = MathUtil.MIN_VECTOR;
			aabbMin.SetMin(ref triangleVerts[0]);
			aabbMax.SetMax(ref triangleVerts[0]);
			aabbMin.SetMin(ref triangleVerts[1]);
			aabbMax.SetMax(ref triangleVerts[1]);
			aabbMin.SetMin(ref triangleVerts[2]);
			aabbMax.SetMax(ref triangleVerts[2]);
			ConnectivityProcessor connectivityProcessor = new ConnectivityProcessor();
			connectivityProcessor.m_partIdA = partId;
			connectivityProcessor.m_triangleIndexA = triangleIndex;
			connectivityProcessor.m_triangleVerticesA = triangleVerts;
			connectivityProcessor.m_triangleInfoMap = triangleInfoMap;
			trimeshShape.ProcessAllTriangles(connectivityProcessor, ref aabbMin, ref aabbMax);
		}

		public static void NearestPointInLineSegment(ref IndexedVector3 point, ref IndexedVector3 line0, ref IndexedVector3 line1, out IndexedVector3 nearestPoint)
		{
			IndexedVector3 indexedVector = line1 - line0;
			if (MathUtil.FuzzyZero(indexedVector.LengthSquared()))
			{
				nearestPoint = line0;
				return;
			}
			float num = IndexedVector3.Dot(point - line0, indexedVector) / IndexedVector3.Dot(indexedVector, indexedVector);
			if (num < 0f)
			{
				num = 0f;
			}
			else if (num > 1f)
			{
				num = 1f;
			}
			nearestPoint = line0 + indexedVector * num;
		}

		public static bool ClampNormal(IndexedVector3 edge, IndexedVector3 tri_normal_org, IndexedVector3 localContactNormalOnB, float correctedEdgeAngle, out IndexedVector3 clampedLocalNormal)
		{
			return ClampNormal(ref edge, ref tri_normal_org, ref localContactNormalOnB, correctedEdgeAngle, out clampedLocalNormal);
		}

		public static bool ClampNormal(ref IndexedVector3 edge, ref IndexedVector3 tri_normal_org, ref IndexedVector3 localContactNormalOnB, float correctedEdgeAngle, out IndexedVector3 clampedLocalNormal)
		{
			IndexedVector3 normalA = tri_normal_org;
			IndexedVector3 edgeA = IndexedVector3.Cross(edge, normalA).Normalized();
			float angle = GetAngle(ref edgeA, ref normalA, ref localContactNormalOnB);
			if (correctedEdgeAngle < 0f && angle < correctedEdgeAngle)
			{
				float angle2 = correctedEdgeAngle - angle;
				IndexedQuaternion q = new IndexedQuaternion(edge, angle2);
				clampedLocalNormal = new IndexedBasisMatrix(q) * localContactNormalOnB;
				return true;
			}
			if (correctedEdgeAngle >= 0f && angle > correctedEdgeAngle)
			{
				float angle3 = correctedEdgeAngle - angle;
				IndexedQuaternion q2 = new IndexedQuaternion(edge, angle3);
				clampedLocalNormal = new IndexedBasisMatrix(q2) * localContactNormalOnB;
				return true;
			}
			clampedLocalNormal = IndexedVector3.Zero;
			return false;
		}

		public static void AdjustInternalEdgeContacts(ManifoldPoint cp, CollisionObject colObj0, CollisionObject colObj1, int partId0, int index0, InternalEdgeAdjustFlags normalAdjustFlags)
		{
			if (colObj0.GetCollisionShape().GetShapeType() != BroadphaseNativeTypes.TRIANGLE_SHAPE_PROXYTYPE)
			{
				return;
			}
			BvhTriangleMeshShape bvhTriangleMeshShape = null;
			if (colObj0.GetRootCollisionShape().GetShapeType() != BroadphaseNativeTypes.SCALED_TRIANGLE_MESH_SHAPE_PROXYTYPE)
			{
				bvhTriangleMeshShape = (BvhTriangleMeshShape)colObj0.GetRootCollisionShape();
			}
			TriangleInfoMap triangleInfoMap = bvhTriangleMeshShape.GetTriangleInfoMap();
			if (triangleInfoMap == null)
			{
				return;
			}
			int hash = GetHash(partId0, index0);
			TriangleInfo value;
			if (!triangleInfoMap.TryGetValue(hash, out value))
			{
				return;
			}
			float num = (((normalAdjustFlags & InternalEdgeAdjustFlags.BT_TRIANGLE_CONVEX_BACKFACE_MODE) == 0) ? 1f : (-1f));
			TriangleShape triangleShape = colObj0.GetCollisionShape() as TriangleShape;
			IndexedVector3 vtx;
			triangleShape.GetVertex(0, out vtx);
			IndexedVector3 vtx2;
			triangleShape.GetVertex(1, out vtx2);
			IndexedVector3 vtx3;
			triangleShape.GetVertex(2, out vtx3);
			IndexedVector3 indexedVector9 = (vtx + vtx2 + vtx3) * (1f / 3f);
			IndexedVector3 color = new IndexedVector3(1f, 0f, 0f);
			IndexedVector3 color2 = new IndexedVector3(0f, 1f, 0f);
			IndexedVector3 color3 = new IndexedVector3(0f, 0f, 1f);
			IndexedVector3 color4 = new IndexedVector3(1f, 1f, 1f);
			IndexedVector3 color5 = new IndexedVector3(0f, 0f, 0f);
			IndexedVector3 normal;
			triangleShape.CalcNormal(out normal);
			IndexedVector3 nearestPoint;
			NearestPointInLineSegment(ref cp.m_localPointB, ref vtx, ref vtx2, out nearestPoint);
			IndexedVector3 point = cp.m_localPointB;
			IndexedMatrix worldTransform = colObj0.GetWorldTransform();
			DebugDrawLine(worldTransform * nearestPoint, worldTransform * cp.m_localPointB, color);
			bool flag = false;
			int num2 = 0;
			int num3 = 0;
			IndexedVector3 v = colObj0.GetWorldTransform()._basis.Transpose() * cp.m_normalWorldOnB;
			v.Normalize();
			int num4 = -1;
			float num5 = 1E+18f;
			if (Math.Abs(value.m_edgeV0V1Angle) < triangleInfoMap.m_maxEdgeAngleThreshold)
			{
				NearestPointInLineSegment(ref cp.m_localPointB, ref vtx, ref vtx2, out nearestPoint);
				float num6 = (point - nearestPoint).Length();
				if (num6 < num5)
				{
					num4 = 0;
					num5 = num6;
				}
			}
			if (Math.Abs(value.m_edgeV1V2Angle) < triangleInfoMap.m_maxEdgeAngleThreshold)
			{
				NearestPointInLineSegment(ref cp.m_localPointB, ref vtx2, ref vtx3, out nearestPoint);
				float num7 = (point - nearestPoint).Length();
				if (num7 < num5)
				{
					num4 = 1;
					num5 = num7;
				}
			}
			if (Math.Abs(value.m_edgeV2V0Angle) < triangleInfoMap.m_maxEdgeAngleThreshold)
			{
				NearestPointInLineSegment(ref cp.m_localPointB, ref vtx3, ref vtx, out nearestPoint);
				float num8 = (point - nearestPoint).Length();
				if (num8 < num5)
				{
					num4 = 2;
					num5 = num8;
				}
			}
			IndexedVector3 indexedVector = normal * new IndexedVector3(0.1f, 0.1f, 0.1f);
			DebugDrawLine(worldTransform * vtx + indexedVector, worldTransform * vtx2 + indexedVector, color);
			if (Math.Abs(value.m_edgeV0V1Angle) < triangleInfoMap.m_maxEdgeAngleThreshold)
			{
				DebugDrawLine(worldTransform * point, worldTransform * (point + cp.m_normalWorldOnB * 10f), color5);
				float num9 = (point - nearestPoint).Length();
				if (num9 < triangleInfoMap.m_edgeDistanceThreshold && num4 == 0)
				{
					IndexedVector3 indexedVector2 = vtx - vtx2;
					flag = true;
					if (value.m_edgeV0V1Angle == 0f)
					{
						num2++;
					}
					else
					{
						float num10 = ((((uint)value.m_flags & (true ? 1u : 0u)) != 0) ? 1f : (-1f));
						DebugDrawLine(worldTransform * nearestPoint, worldTransform * (nearestPoint + num10 * normal * 10f), color4);
						IndexedVector3 v2 = num10 * normal;
						IndexedQuaternion rotation = new IndexedQuaternion(indexedVector2, value.m_edgeV0V1Angle);
						IndexedVector3 indexedVector3 = MathUtil.QuatRotate(ref rotation, ref normal);
						if (((uint)value.m_flags & 8u) != 0)
						{
							indexedVector3 *= -1f;
						}
						IndexedVector3 v3 = num10 * indexedVector3;
						float num11 = v.Dot(ref v2);
						float num12 = v.Dot(ref v3);
						bool flag2 = num11 < triangleInfoMap.m_convexEpsilon && num12 < triangleInfoMap.m_convexEpsilon;
						DebugDrawLine(cp.GetPositionWorldOnB(), cp.GetPositionWorldOnB() + worldTransform._basis * (v3 * 20f), color);
						if (flag2)
						{
							num2++;
						}
						else
						{
							num3++;
							IndexedVector3 clampedLocalNormal;
							if (ClampNormal(indexedVector2, num10 * normal, v, value.m_edgeV0V1Angle, out clampedLocalNormal) && ((normalAdjustFlags & InternalEdgeAdjustFlags.BT_TRIANGLE_CONVEX_DOUBLE_SIDED) != 0 || clampedLocalNormal.Dot(num * normal) > 0f))
							{
								IndexedVector3 normalWorldOnB = colObj0.GetWorldTransform()._basis * clampedLocalNormal;
								cp.m_normalWorldOnB = normalWorldOnB;
								cp.m_positionWorldOnB = cp.m_positionWorldOnA - cp.m_normalWorldOnB * cp.m_distance1;
								cp.m_localPointB = colObj0.GetWorldTransform().InvXform(cp.m_positionWorldOnB);
							}
						}
					}
				}
			}
			NearestPointInLineSegment(ref point, ref vtx2, ref vtx3, out nearestPoint);
			DebugDrawLine(worldTransform * nearestPoint, worldTransform * cp.m_localPointB, color2);
			DebugDrawLine(worldTransform * vtx2 + indexedVector, worldTransform * vtx3 + indexedVector, color2);
			if (Math.Abs(value.m_edgeV1V2Angle) < triangleInfoMap.m_maxEdgeAngleThreshold)
			{
				DebugDrawLine(worldTransform * point, worldTransform * (point + cp.m_normalWorldOnB * 10f), color5);
				float num13 = (point - nearestPoint).Length();
				if (num13 < triangleInfoMap.m_edgeDistanceThreshold && num4 == 1)
				{
					flag = true;
					DebugDrawLine(worldTransform * nearestPoint, worldTransform * (nearestPoint + normal * 10f), color4);
					IndexedVector3 indexedVector4 = vtx2 - vtx3;
					flag = true;
					if (value.m_edgeV1V2Angle == 0f)
					{
						num2++;
					}
					else
					{
						float num14 = ((((uint)value.m_flags & 2u) != 0) ? 1f : (-1f));
						DebugDrawLine(worldTransform * nearestPoint, worldTransform * (nearestPoint + num14 * normal * 10f), color4);
						IndexedVector3 v4 = num14 * normal;
						IndexedQuaternion rotation2 = new IndexedQuaternion(indexedVector4, value.m_edgeV1V2Angle);
						IndexedVector3 indexedVector5 = MathUtil.QuatRotate(ref rotation2, ref normal);
						if (((uint)value.m_flags & 0x10u) != 0)
						{
							indexedVector5 *= -1f;
						}
						IndexedVector3 v5 = num14 * indexedVector5;
						DebugDrawLine(cp.GetPositionWorldOnB(), cp.GetPositionWorldOnB() + worldTransform._basis * (v5 * 20f), color);
						float num15 = v.Dot(ref v4);
						float num16 = v.Dot(ref v5);
						if (num15 < triangleInfoMap.m_convexEpsilon && num16 < triangleInfoMap.m_convexEpsilon)
						{
							num2++;
						}
						else
						{
							num3++;
							IndexedVector3 localContactNormalOnB = colObj0.GetWorldTransform()._basis.Transpose() * cp.m_normalWorldOnB;
							IndexedVector3 clampedLocalNormal2;
							if (ClampNormal(indexedVector4, num14 * normal, localContactNormalOnB, value.m_edgeV1V2Angle, out clampedLocalNormal2) && ((normalAdjustFlags & InternalEdgeAdjustFlags.BT_TRIANGLE_CONVEX_DOUBLE_SIDED) != 0 || clampedLocalNormal2.Dot(num * normal) > 0f))
							{
								IndexedVector3 normalWorldOnB2 = colObj0.GetWorldTransform()._basis * clampedLocalNormal2;
								cp.m_normalWorldOnB = normalWorldOnB2;
								cp.m_positionWorldOnB = cp.m_positionWorldOnA - cp.m_normalWorldOnB * cp.m_distance1;
								cp.m_localPointB = colObj0.GetWorldTransform().InvXform(cp.m_positionWorldOnB);
							}
						}
					}
				}
			}
			NearestPointInLineSegment(ref point, ref vtx3, ref vtx, out nearestPoint);
			DebugDrawLine(worldTransform * nearestPoint, worldTransform * cp.m_localPointB, color3);
			DebugDrawLine(worldTransform * vtx3 + indexedVector, worldTransform * vtx + indexedVector, color3);
			if (Math.Abs(value.m_edgeV2V0Angle) < triangleInfoMap.m_maxEdgeAngleThreshold)
			{
				DebugDrawLine(worldTransform * point, worldTransform * (point + cp.m_normalWorldOnB * 10f), color5);
				float num17 = (point - nearestPoint).Length();
				if (num17 < triangleInfoMap.m_edgeDistanceThreshold && num4 == 2)
				{
					flag = true;
					DebugDrawLine(worldTransform * nearestPoint, worldTransform * (nearestPoint + normal * 10f), color4);
					IndexedVector3 indexedVector6 = vtx3 - vtx;
					if (value.m_edgeV2V0Angle == 0f)
					{
						num2++;
					}
					else
					{
						float num18 = ((((uint)value.m_flags & 4u) != 0) ? 1f : (-1f));
						DebugDrawLine(worldTransform * nearestPoint, worldTransform * (nearestPoint + num18 * normal * 10f), color4);
						IndexedVector3 v6 = num18 * normal;
						IndexedQuaternion rotation3 = new IndexedQuaternion(indexedVector6, value.m_edgeV2V0Angle);
						IndexedVector3 indexedVector7 = MathUtil.QuatRotate(ref rotation3, ref normal);
						if (((uint)value.m_flags & 0x20u) != 0)
						{
							indexedVector7 *= -1f;
						}
						IndexedVector3 v7 = num18 * indexedVector7;
						DebugDrawLine(cp.GetPositionWorldOnB(), cp.GetPositionWorldOnB() + worldTransform._basis * (v7 * 20f), color);
						float num19 = v.Dot(ref v6);
						float num20 = v.Dot(ref v7);
						if (num19 < triangleInfoMap.m_convexEpsilon && num20 < triangleInfoMap.m_convexEpsilon)
						{
							num2++;
						}
						else
						{
							num3++;
							IndexedVector3 localContactNormalOnB2 = colObj0.GetWorldTransform()._basis.Transpose() * cp.m_normalWorldOnB;
							IndexedVector3 clampedLocalNormal3;
							if (ClampNormal(indexedVector6, num18 * normal, localContactNormalOnB2, value.m_edgeV2V0Angle, out clampedLocalNormal3) && ((normalAdjustFlags & InternalEdgeAdjustFlags.BT_TRIANGLE_CONVEX_DOUBLE_SIDED) != 0 || clampedLocalNormal3.Dot(num * normal) > 0f))
							{
								IndexedVector3 normalWorldOnB3 = colObj0.GetWorldTransform()._basis * clampedLocalNormal3;
								cp.m_normalWorldOnB = normalWorldOnB3;
								cp.m_positionWorldOnB = cp.m_positionWorldOnA - cp.m_normalWorldOnB * cp.m_distance1;
								cp.m_localPointB = colObj0.GetWorldTransform().InvXform(cp.m_positionWorldOnB);
							}
						}
					}
				}
			}
			DebugDrawLine(color: new IndexedVector3(0f, 1f, 1f), from: cp.GetPositionWorldOnB(), to: cp.GetPositionWorldOnB() + cp.m_normalWorldOnB * 10f);
			if (!flag || num2 <= 0)
			{
				return;
			}
			if ((normalAdjustFlags & InternalEdgeAdjustFlags.BT_TRIANGLE_CONCAVE_DOUBLE_SIDED) != 0)
			{
				if (normal.Dot(ref v) < 0f)
				{
					normal *= -1f;
				}
				cp.m_normalWorldOnB = colObj0.GetWorldTransform()._basis * normal;
			}
			else
			{
				IndexedVector3 indexedVector8 = normal * num;
				float num21 = indexedVector8.Dot(ref v);
				if (num21 < 0f)
				{
					return;
				}
				cp.m_normalWorldOnB = colObj0.GetWorldTransform()._basis * indexedVector8;
			}
			cp.m_positionWorldOnB = cp.m_positionWorldOnA - cp.m_normalWorldOnB * cp.m_distance1;
			cp.m_localPointB = colObj0.GetWorldTransform().InvXform(cp.m_positionWorldOnB);
		}
	}
}
