using System;
using System.Collections.Generic;

namespace BulletXNA.LinearMath
{
	public class HullLibrary
	{
		public const float PAPERWIDTH = 0.001f;

		public const float planetestepsilon = 0.001f;

		public const float EPSILON = 1E-06f;

		public IList<HullTriangle> m_tris = new List<HullTriangle>();

		public IList<int> m_vertexIndexMapping = new List<int>();

		public static int MaxDirFiltered(IList<IndexedVector3> p, int count, ref IndexedVector3 dir, IList<int> allow)
		{
			int num = -1;
			for (int i = 0; i < count; i++)
			{
				if (allow[i] != 0 && (num == -1 || IndexedVector3.Dot(p[i], dir) > IndexedVector3.Dot(p[num], dir)))
				{
					num = i;
				}
			}
			return num;
		}

		public static IndexedVector3 Orth(ref IndexedVector3 v)
		{
			IndexedVector3 v2 = IndexedVector3.Cross(v, new IndexedVector3(0f, 0f, 1f));
			IndexedVector3 v3 = IndexedVector3.Cross(v, new IndexedVector3(0f, 1f, 0f));
			if (v2.Length() > v3.Length())
			{
				return IndexedVector3.Normalize(v2);
			}
			return IndexedVector3.Normalize(v3);
		}

		public static int MaxDirSterId(IList<IndexedVector3> p, int count, IndexedVector3 dir, IList<int> allow)
		{
			return MaxDirSterId(p, count, ref dir, allow);
		}

		public static int MaxDirSterId(IList<IndexedVector3> p, int count, ref IndexedVector3 dir, IList<int> allow)
		{
			int num;
			for (num = -1; num == -1; num = -1)
			{
				num = MaxDirFiltered(p, count, ref dir, allow);
				if (allow[num] == 3)
				{
					return num;
				}
				IndexedVector3 indexedVector = Orth(ref dir);
				IndexedVector3 indexedVector2 = IndexedVector3.Cross(indexedVector, dir);
				int num2 = -1;
				for (float num3 = 0f; num3 <= 360f; num3 += 45f)
				{
					float num4 = (float)Math.Sin((float)Math.PI / 180f * num3);
					float num5 = (float)Math.Cos((float)Math.PI / 180f * num3);
					IndexedVector3 dir2 = dir + (indexedVector * num4 + indexedVector2 * num5) * 0.025f;
					int num6 = MaxDirFiltered(p, count, ref dir2, allow);
					if (num2 == num && num6 == num)
					{
						allow[num] = 3;
						return num;
					}
					if (num2 != -1 && num2 != num6)
					{
						int num7 = num2;
						for (float num8 = num3 - 40f; num8 <= num3; num8 += 5f)
						{
							float num9 = (float)Math.Sin((float)Math.PI / 180f * num8);
							float num10 = (float)Math.Cos((float)Math.PI / 180f * num8);
							IndexedVector3 dir3 = dir + (indexedVector * num9 + indexedVector2 * num10) * 0.025f;
							int num11 = MaxDirFiltered(p, count, ref dir3, allow);
							if (num7 == num && num11 == num)
							{
								allow[num] = 3;
								return num;
							}
							num7 = num11;
						}
					}
					num2 = num6;
				}
				allow[num] = 0;
			}
			return num;
		}

		public bool Above(IList<IndexedVector3> vertices, int3 t, IndexedVector3 p, float epsilon)
		{
			return Above(vertices, t, ref p, epsilon);
		}

		public bool Above(IList<IndexedVector3> vertices, int3 t, ref IndexedVector3 p, float epsilon)
		{
			IndexedVector3 a = TriNormal(vertices[t.At(0)], vertices[t.At(1)], vertices[t.At(2)]);
			return IndexedVector3.Dot(a, p - vertices[t.At(0)]) > epsilon;
		}

		public bool HasEdge(int3 t, int a, int b)
		{
			for (int i = 0; i < 3; i++)
			{
				int index = (i + 1) % 3;
				if (t.At(i) == a && t.At(index) == b)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasVert(int3 t, int v)
		{
			if (t.At(0) != v && t.At(1) != v)
			{
				return t.At(2) == v;
			}
			return true;
		}

		public int ShareEdge(int3 a, int3 b)
		{
			for (int i = 0; i < 3; i++)
			{
				int index = (i + 1) % 3;
				if (HasEdge(a, b.At(index), b.At(i)))
				{
					return 1;
				}
			}
			return 0;
		}

		public void B2bFix(HullTriangle s, HullTriangle t)
		{
			for (int i = 0; i < 3; i++)
			{
				int index = (i + 1) % 3;
				int index2 = (i + 2) % 3;
				int num = s.At(index);
				int num2 = s.At(index2);
				m_tris[s.Neib(num, num2)].Neib(num2, num, t.Neib(num2, num));
				m_tris[t.Neib(num2, num)].Neib(num, num2, s.Neib(num, num2));
			}
		}

		private void RemoveB2b(HullTriangle s, HullTriangle t)
		{
			B2bFix(s, t);
			DeAllocateTriangle(s);
			DeAllocateTriangle(t);
		}

		private void CheckIt(HullTriangle t)
		{
			for (int i = 0; i < 3; i++)
			{
				int index = (i + 1) % 3;
				int index2 = (i + 2) % 3;
				t.At(index);
				t.At(index2);
			}
		}

		public HullTriangle AllocateTriangle(int a, int b, int c)
		{
			HullTriangle hullTriangle = new HullTriangle(a, b, c);
			hullTriangle.id = m_tris.Count;
			m_tris.Add(hullTriangle);
			return hullTriangle;
		}

		public void DeAllocateTriangle(HullTriangle tri)
		{
			m_tris[tri.id] = null;
		}

		public void Extrude(HullTriangle t0, int v)
		{
			int count = m_tris.Count;
			HullTriangle hullTriangle = AllocateTriangle(v, t0.At(1), t0.At(2));
			hullTriangle.n = new int3(t0.n.At(0), count + 1, count + 2);
			m_tris[t0.n.At(0)].Neib(t0.At(1), t0.At(2), count);
			HullTriangle hullTriangle2 = AllocateTriangle(v, t0.At(2), t0.At(0));
			hullTriangle2.n = new int3(t0.n.At(1), count + 2, count);
			m_tris[t0.n.At(1)].Neib(t0.At(2), t0.At(0), count + 1);
			HullTriangle hullTriangle3 = AllocateTriangle(v, t0.At(0), t0.At(1));
			hullTriangle3.n = new int3(t0.n.At(2), count, count + 1);
			m_tris[t0.n.At(2)].Neib(t0.At(0), t0.At(1), count + 2);
			CheckIt(hullTriangle);
			CheckIt(hullTriangle2);
			CheckIt(hullTriangle3);
			if (HasVert(m_tris[hullTriangle.n.At(0)], v))
			{
				RemoveB2b(hullTriangle, m_tris[hullTriangle.n.At(0)]);
			}
			if (HasVert(m_tris[hullTriangle2.n.At(0)], v))
			{
				RemoveB2b(hullTriangle2, m_tris[hullTriangle2.n.At(0)]);
			}
			if (HasVert(m_tris[hullTriangle3.n.At(0)], v))
			{
				RemoveB2b(hullTriangle3, m_tris[hullTriangle3.n.At(0)]);
			}
			DeAllocateTriangle(t0);
		}

		public HullTriangle Extrudable(float epsilon)
		{
			HullTriangle hullTriangle = null;
			for (int i = 0; i < m_tris.Count; i++)
			{
				if (hullTriangle == null || (m_tris[i] != null && hullTriangle.rise < m_tris[i].rise))
				{
					hullTriangle = m_tris[i];
				}
			}
			if (!(hullTriangle.rise > epsilon))
			{
				return null;
			}
			return hullTriangle;
		}

		public int4 FindSimplex(IList<IndexedVector3> verts, int verts_count, IList<int> allow)
		{
			IndexedVector3[] array = new IndexedVector3[3]
			{
				new IndexedVector3(0.01f, 0.02f, 1f),
				default(IndexedVector3),
				default(IndexedVector3)
			};
			int num = MaxDirSterId(verts, verts_count, array[0], allow);
			int num2 = MaxDirSterId(verts, verts_count, -array[0], allow);
			array[0] = verts[num] - verts[num2];
			if (num == num2 || array[0] == IndexedVector3.Zero)
			{
				return new int4(-1, -1, -1, -1);
			}
			array[1] = IndexedVector3.Cross(new IndexedVector3(1f, 0.02f, 0f), array[0]);
			array[2] = IndexedVector3.Cross(new IndexedVector3(-0.02f, 1f, 0f), array[0]);
			if (array[1].Length() > array[2].Length())
			{
				array[1].Normalize();
			}
			else
			{
				array[1] = array[2];
				array[1].Normalize();
			}
			int num3 = MaxDirSterId(verts, verts_count, array[1], allow);
			if (num3 == num || num3 == num2)
			{
				num3 = MaxDirSterId(verts, verts_count, -array[1], allow);
			}
			if (num3 == num || num3 == num2)
			{
				return new int4(-1, -1, -1, -1);
			}
			array[1] = verts[num3] - verts[num];
			array[2] = IndexedVector3.Normalize(IndexedVector3.Cross(array[1], array[0]));
			int num4 = MaxDirSterId(verts, verts_count, array[2], allow);
			if (num4 == num || num4 == num2 || num4 == num3)
			{
				num4 = MaxDirSterId(verts, verts_count, -array[2], allow);
			}
			if (num4 == num || num4 == num2 || num4 == num3)
			{
				return new int4(-1, -1, -1, -1);
			}
			if (IndexedVector3.Dot(verts[num4] - verts[num], IndexedVector3.Cross(verts[num2] - verts[num], verts[num3] - verts[num])) < 0f)
			{
				int num5 = num3;
				num3 = num4;
				num4 = num5;
			}
			return new int4(num, num2, num3, num4);
		}

		public int CalcHullGen(IList<IndexedVector3> verts, int verts_count, int vlimit)
		{
			if (verts_count < 4)
			{
				return 0;
			}
			if (vlimit == 0)
			{
				vlimit = 1000000000;
			}
			IndexedVector3 output = MathUtil.MAX_VECTOR;
			IndexedVector3 output2 = MathUtil.MIN_VECTOR;
			IList<int> list = new List<int>(verts_count);
			IList<int> list2 = new List<int>(verts_count);
			for (int i = 0; i < verts_count; i++)
			{
				list2.Add(1);
				list.Add(0);
				MathUtil.VectorMin(verts[i], ref output);
				MathUtil.VectorMax(verts[i], ref output2);
			}
			float num = (output2 - output).Length() * 0.001f;
			int4 int5 = FindSimplex(verts, verts_count, list2);
			if (int5.x == -1)
			{
				return 0;
			}
			IndexedVector3 p = (verts[int5.At(0)] + verts[int5.At(1)] + verts[int5.At(2)] + verts[int5.At(3)]) / 4f;
			HullTriangle hullTriangle = AllocateTriangle(int5.At(2), int5.At(3), int5.At(1));
			hullTriangle.n = new int3(2, 3, 1);
			HullTriangle hullTriangle2 = AllocateTriangle(int5.At(3), int5.At(2), int5.At(0));
			hullTriangle2.n = new int3(3, 2, 0);
			HullTriangle hullTriangle3 = AllocateTriangle(int5.At(0), int5.At(1), int5.At(3));
			hullTriangle3.n = new int3(0, 1, 3);
			HullTriangle hullTriangle4 = AllocateTriangle(int5.At(1), int5.At(0), int5.At(2));
			hullTriangle4.n = new int3(1, 0, 2);
			int index = int5.At(0);
			int index2 = int5.At(1);
			int index3 = int5.At(2);
			int num3 = (list[int5.At(3)] = 1);
			int num5 = (list[index3] = num3);
			int value = (list[index2] = num5);
			list[index] = value;
			CheckIt(hullTriangle);
			CheckIt(hullTriangle2);
			CheckIt(hullTriangle3);
			CheckIt(hullTriangle4);
			for (int j = 0; j < m_tris.Count; j++)
			{
				HullTriangle hullTriangle5 = m_tris[j];
				IndexedVector3 indexedVector = TriNormal(verts[hullTriangle5.At(0)], verts[hullTriangle5.At(1)], verts[hullTriangle5.At(2)]);
				hullTriangle5.vmax = MaxDirSterId(verts, verts_count, indexedVector, list2);
				hullTriangle5.rise = IndexedVector3.Dot(indexedVector, verts[hullTriangle5.vmax] - verts[hullTriangle5.At(0)]);
			}
			HullTriangle hullTriangle6 = null;
			vlimit -= 4;
			while (vlimit > 0 && (hullTriangle6 = Extrudable(num)) != null)
			{
				int vmax = hullTriangle6.vmax;
				list[vmax] = 1;
				int count = m_tris.Count;
				while (count-- > 0)
				{
					if (m_tris[count] != null)
					{
						int3 t = m_tris[count];
						if (Above(verts, t, verts[vmax], 0.01f * num))
						{
							Extrude(m_tris[count], vmax);
						}
					}
				}
				count = m_tris.Count;
				while (count-- > 0)
				{
					if (m_tris[count] != null)
					{
						if (!HasVert(m_tris[count], vmax))
						{
							break;
						}
						int3 int6 = m_tris[count];
						if (Above(verts, int6, p, 0.01f * num) || IndexedVector3.Cross(verts[int6.At(1)] - verts[int6.At(0)], verts[int6.At(2)] - verts[int6.At(1)]).Length() < num * num * 0.1f)
						{
							HullTriangle t2 = m_tris[m_tris[count].n.At(0)];
							Extrude(t2, vmax);
							count = m_tris.Count;
						}
					}
				}
				count = m_tris.Count;
				while (count-- != 0)
				{
					HullTriangle hullTriangle7 = m_tris[count];
					if (hullTriangle7 != null)
					{
						if (hullTriangle7.vmax >= 0)
						{
							break;
						}
						IndexedVector3 indexedVector2 = TriNormal(verts[hullTriangle7.At(0)], verts[hullTriangle7.At(1)], verts[hullTriangle7.At(2)]);
						hullTriangle7.vmax = MaxDirSterId(verts, verts_count, indexedVector2, list2);
						if (list[hullTriangle7.vmax] != 0)
						{
							hullTriangle7.vmax = -1;
						}
						else
						{
							hullTriangle7.rise = IndexedVector3.Dot(indexedVector2, verts[hullTriangle7.vmax] - verts[hullTriangle7.At(0)]);
						}
					}
				}
				vlimit--;
			}
			return 1;
		}

		public int CalcHull(IList<IndexedVector3> verts, int verts_count, IList<int> tris_out, ref int tris_count, int vlimit)
		{
			if (CalcHullGen(verts, verts_count, vlimit) == 0)
			{
				return 0;
			}
			IList<int> list = new List<int>();
			for (int i = 0; i < m_tris.Count; i++)
			{
				if (m_tris[i] != null)
				{
					for (int j = 0; j < 3; j++)
					{
						list.Add(m_tris[i].At(j));
					}
					DeAllocateTriangle(m_tris[i]);
				}
			}
			tris_count = list.Count / 3;
			tris_out.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				tris_out.Add(list[i]);
			}
			m_tris.Clear();
			return 1;
		}

		public bool ComputeHull(int vcount, IList<IndexedVector3> vertices, PHullResult result, int vlimit)
		{
			int tris_count = 0;
			if (CalcHull(vertices, vcount, result.m_Indices, ref tris_count, vlimit) == 0)
			{
				return false;
			}
			result.mIndexCount = tris_count * 3;
			result.mFaceCount = tris_count;
			result.mVertices = vertices;
			result.mVcount = vcount;
			return true;
		}

		public void ReleaseHull(PHullResult result)
		{
			if (result.m_Indices.Count != 0)
			{
				result.m_Indices.Clear();
			}
			result.mVcount = 0;
			result.mIndexCount = 0;
			result.mVertices.Clear();
		}

		public HullError CreateConvexHull(HullDesc desc, HullResult result)
		{
			HullError result2 = HullError.QE_FAIL;
			PHullResult pHullResult = new PHullResult();
			int num = desc.mVcount;
			if (num < 8)
			{
				num = 8;
			}
			IList<IndexedVector3> list = new List<IndexedVector3>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(IndexedVector3.Zero);
			}
			IndexedVector3 scale = new IndexedVector3(1f);
			int vcount = 0;
			if (CleanupVertices(desc.mVcount, desc.mVertices, desc.mVertexStride, ref vcount, list, desc.mNormalEpsilon, ref scale))
			{
				for (int j = 0; j < vcount; j++)
				{
					IndexedVector3 value = list[j];
					value.X *= scale.X;
					value.Y *= scale.Y;
					value.Z *= scale.Z;
					list[j] = value;
				}
				if (ComputeHull(vcount, list, pHullResult, desc.mMaxVertices))
				{
					IList<IndexedVector3> list2 = new ObjectArray<IndexedVector3>(pHullResult.mVcount);
					BringOutYourDead(pHullResult.mVertices, pHullResult.mVcount, list2, ref vcount, pHullResult.m_Indices, pHullResult.mIndexCount);
					result2 = HullError.QE_OK;
					if (desc.HasHullFlag(HullFlag.QF_TRIANGLES))
					{
						result.mPolygons = false;
						result.mNumOutputVertices = vcount;
						result.m_OutputVertices.Clear();
						result.mNumFaces = pHullResult.mFaceCount;
						result.mNumIndices = pHullResult.mIndexCount;
						result.m_Indices.Clear();
						for (int k = 0; k < vcount; k++)
						{
							result.m_OutputVertices.Add(list2[k]);
						}
						if (desc.HasHullFlag(HullFlag.QF_REVERSE_ORDER))
						{
							IList<int> indices = pHullResult.m_Indices;
							IList<int> indices2 = result.m_Indices;
							for (int l = 0; l < pHullResult.mFaceCount; l++)
							{
								int num2 = l * 3;
								indices2.Add(indices[num2 + 2]);
								indices2.Add(indices[num2 + 1]);
								indices2.Add(indices[num2]);
							}
						}
						else
						{
							for (int m = 0; m < pHullResult.mIndexCount; m++)
							{
								result.m_Indices.Add(pHullResult.m_Indices[m]);
							}
						}
					}
					else
					{
						result.mPolygons = true;
						result.mNumOutputVertices = vcount;
						result.m_OutputVertices.Clear();
						result.mNumFaces = pHullResult.mFaceCount;
						result.mNumIndices = pHullResult.mIndexCount + pHullResult.mFaceCount;
						result.m_Indices.Clear();
						for (int n = 0; n < vcount; n++)
						{
							result.m_OutputVertices.Add(list2[n]);
						}
						IList<int> indices3 = pHullResult.m_Indices;
						IList<int> indices4 = result.m_Indices;
						for (int num3 = 0; num3 < pHullResult.mFaceCount; num3++)
						{
							int num4 = num3 * 3;
							indices4[0] = 3;
							if (desc.HasHullFlag(HullFlag.QF_REVERSE_ORDER))
							{
								indices4.Add(indices3[num4 + 2]);
								indices4.Add(indices3[num4 + 1]);
								indices4.Add(indices3[num4]);
							}
							else
							{
								indices4.Add(indices3[num4]);
								indices4.Add(indices3[num4 + 1]);
								indices4.Add(indices3[num4 + 2]);
							}
						}
					}
					ReleaseHull(pHullResult);
				}
			}
			return result2;
		}

		public HullError ReleaseResult(HullResult result)
		{
			if (result.m_OutputVertices.Count != 0)
			{
				result.mNumOutputVertices = 0;
				result.m_OutputVertices.Clear();
			}
			if (result.m_Indices.Count != 0)
			{
				result.mNumIndices = 0;
				result.m_Indices.Clear();
			}
			return HullError.QE_OK;
		}

		public static void AddPoint(ref int vcount, IList<IndexedVector3> p, float x, float y, float z)
		{
			IndexedVector3 indexedVector = p[vcount];
			vcount++;
		}

		public float GetDist(float px, float py, float pz, ref IndexedVector3 p2)
		{
			float num = px - p2.X;
			float num2 = py - p2.Y;
			float num3 = pz - p2.Z;
			return num * num + num2 * num2 + num3 * num3;
		}

		public bool CleanupVertices(int svcount, IList<IndexedVector3> svertices, int stride, ref int vcount, IList<IndexedVector3> vertices, float normalepsilon, ref IndexedVector3 scale)
		{
			if (svcount == 0)
			{
				return false;
			}
			m_vertexIndexMapping.Clear();
			vcount = 0;
			IndexedVector3 indexedVector = default(IndexedVector3);
			scale = new IndexedVector3(1f);
			IndexedVector3 output = MathUtil.MAX_VECTOR;
			IndexedVector3 output2 = MathUtil.MIN_VECTOR;
			for (int i = 0; i < svcount; i++)
			{
				IndexedVector3 input = svertices[i];
				MathUtil.VectorMin(ref input, ref output);
				MathUtil.VectorMax(ref input, ref output2);
				svertices[i] = input;
			}
			IndexedVector3 indexedVector2 = output2 - output;
			IndexedVector3 indexedVector3 = indexedVector2 * 0.5f;
			indexedVector3 += output;
			if (indexedVector2.X < 1E-06f || indexedVector2.Y < 1E-06f || indexedVector2.Z < 1E-06f || svcount < 3)
			{
				float num = float.MaxValue;
				if (indexedVector2.X > 1E-06f && indexedVector2.X < num)
				{
					num = indexedVector2.X;
				}
				if (indexedVector2.Y > 1E-06f && indexedVector2.Y < num)
				{
					num = indexedVector2.Y;
				}
				if (indexedVector2.Z > 1E-06f && indexedVector2.Z < num)
				{
					num = indexedVector2.Z;
				}
				if (num == float.MaxValue)
				{
					indexedVector2 = new IndexedVector3(0.01f);
				}
				else
				{
					if (indexedVector2.X < 1E-06f)
					{
						indexedVector2.X = num * 0.05f;
					}
					if (indexedVector2.Y < 1E-06f)
					{
						indexedVector2.Y = num * 0.05f;
					}
					if (indexedVector2.Z < 1E-06f)
					{
						indexedVector2.Z = num * 0.05f;
					}
				}
				float x = indexedVector3.X - indexedVector2.X;
				float x2 = indexedVector3.X + indexedVector2.X;
				float y = indexedVector3.Y - indexedVector2.Y;
				float y2 = indexedVector3.Y + indexedVector2.Y;
				float z = indexedVector3.Z - indexedVector2.Z;
				float z2 = indexedVector3.Z + indexedVector2.Z;
				AddPoint(ref vcount, vertices, x, y, z);
				AddPoint(ref vcount, vertices, x2, y, z);
				AddPoint(ref vcount, vertices, x2, y2, z);
				AddPoint(ref vcount, vertices, x, y2, z);
				AddPoint(ref vcount, vertices, x, y, z2);
				AddPoint(ref vcount, vertices, x2, y, z2);
				AddPoint(ref vcount, vertices, x2, y2, z2);
				AddPoint(ref vcount, vertices, x, y2, z2);
				return true;
			}
			if (scale.LengthSquared() > 0f)
			{
				scale = indexedVector2;
				indexedVector.X = 1f / indexedVector2.X;
				indexedVector.Y = 1f / indexedVector2.Y;
				indexedVector.Z = 1f / indexedVector2.Z;
				indexedVector3 *= indexedVector;
			}
			for (int j = 0; j < svcount; j++)
			{
				IndexedVector3 indexedVector4 = svertices[j];
				if (scale.LengthSquared() > 0f)
				{
					indexedVector4.X *= indexedVector.X;
					indexedVector4.Y *= indexedVector.Y;
					indexedVector4.Z *= indexedVector.Z;
				}
				int num2 = 0;
				for (num2 = 0; num2 < vcount; num2++)
				{
					IndexedVector3 indexedVector5 = vertices[num2];
					IndexedVector3 indexedVector6 = (indexedVector5 - indexedVector4).Absolute();
					if (indexedVector6.X < normalepsilon && indexedVector6.Y < normalepsilon && indexedVector6.Z < normalepsilon)
					{
						float num3 = (indexedVector4 - indexedVector3).LengthSquared();
						float num4 = (indexedVector5 - indexedVector3).LengthSquared();
						if (num3 > num4)
						{
							vertices[num2] = indexedVector4;
						}
						break;
					}
				}
				if (num2 == vcount)
				{
					vertices[vcount] = indexedVector4;
					vcount++;
				}
				m_vertexIndexMapping.Add(num2);
			}
			float[] array = new float[3]
			{
				float.MaxValue,
				float.MaxValue,
				float.MaxValue
			};
			float[] array2 = new float[3]
			{
				float.MinValue,
				float.MinValue,
				float.MinValue
			};
			for (int k = 0; k < vcount; k++)
			{
				IndexedVector3 indexedVector7 = vertices[k];
				if (indexedVector7.X < array[0])
				{
					array[0] = indexedVector7.X;
				}
				if (indexedVector7.X > array2[0])
				{
					array2[0] = indexedVector7.X;
				}
				if (indexedVector7.Y < array[1])
				{
					array[1] = indexedVector7.Y;
				}
				if (indexedVector7.Y > array2[1])
				{
					array2[1] = indexedVector7.Y;
				}
				if (indexedVector7.Z < array[2])
				{
					array[2] = indexedVector7.Z;
				}
				if (indexedVector7.Z > array2[2])
				{
					array2[2] = indexedVector7.Z;
				}
			}
			float num5 = array2[0] - array[0];
			float num6 = array2[1] - array[1];
			float num7 = array2[2] - array[2];
			if (num5 < 1E-06f || num6 < 1E-06f || num7 < 1E-06f || vcount < 3)
			{
				float num8 = num5 * 0.5f + array[0];
				float num9 = num6 * 0.5f + array[1];
				float num10 = num7 * 0.5f + array[2];
				float num11 = float.MaxValue;
				if (num5 >= 1E-06f && num5 < num11)
				{
					num11 = num5;
				}
				if (num6 >= 1E-06f && num6 < num11)
				{
					num11 = num6;
				}
				if (num7 >= 1E-06f && num7 < num11)
				{
					num11 = num7;
				}
				if (num11 == float.MaxValue)
				{
					num5 = (num6 = (num7 = 0.01f));
				}
				else
				{
					if (num5 < 1E-06f)
					{
						num5 = num11 * 0.05f;
					}
					if (num6 < 1E-06f)
					{
						num6 = num11 * 0.05f;
					}
					if (num7 < 1E-06f)
					{
						num7 = num11 * 0.05f;
					}
				}
				float x3 = num8 - num5;
				float x4 = num8 + num5;
				float y3 = num9 - num6;
				float y4 = num9 + num6;
				float z3 = num10 - num7;
				float z4 = num10 + num7;
				vcount = 0;
				AddPoint(ref vcount, vertices, x3, y3, z3);
				AddPoint(ref vcount, vertices, x4, y3, z3);
				AddPoint(ref vcount, vertices, x4, y4, z3);
				AddPoint(ref vcount, vertices, x3, y4, z3);
				AddPoint(ref vcount, vertices, x3, y3, z4);
				AddPoint(ref vcount, vertices, x4, y3, z4);
				AddPoint(ref vcount, vertices, x4, y4, z4);
				AddPoint(ref vcount, vertices, x3, y4, z4);
				return true;
			}
			return true;
		}

		public void BringOutYourDead(IList<IndexedVector3> verts, int vcount, IList<IndexedVector3> overts, ref int ocount, IList<int> indices, int indexcount)
		{
			IList<int> list = new ObjectArray<int>(m_vertexIndexMapping.Count);
			for (int i = 0; i < m_vertexIndexMapping.Count; i++)
			{
				list[i] = m_vertexIndexMapping[i];
			}
			IList<int> list2 = new ObjectArray<int>(vcount);
			ocount = 0;
			for (int j = 0; j < indexcount; j++)
			{
				int num = indices[j];
				if (list2[num] != 0)
				{
					indices[j] = list2[num] - 1;
					continue;
				}
				indices[j] = ocount;
				overts[ocount] = verts[num];
				for (int k = 0; k < m_vertexIndexMapping.Count; k++)
				{
					if (list[k] == num)
					{
						m_vertexIndexMapping[k] = ocount;
					}
				}
				ocount++;
				list2[num] = ocount;
			}
		}

		public static IndexedVector3 ThreePlaneIntersection(IndexedVector4 p0, IndexedVector4 p1, IndexedVector4 p2)
		{
			IndexedVector3 indexedVector = p0.ToVector3();
			IndexedVector3 indexedVector2 = p1.ToVector3();
			IndexedVector3 indexedVector3 = p2.ToVector3();
			IndexedVector3 indexedVector4 = IndexedVector3.Cross(indexedVector2, indexedVector3);
			IndexedVector3 indexedVector5 = IndexedVector3.Cross(indexedVector3, indexedVector);
			IndexedVector3 indexedVector6 = IndexedVector3.Cross(indexedVector, indexedVector2);
			float num = IndexedVector3.Dot(indexedVector, indexedVector4);
			num = -1f / num;
			indexedVector4 *= p0.W;
			indexedVector5 *= p1.W;
			indexedVector6 *= p2.W;
			IndexedVector3 indexedVector7 = indexedVector4;
			indexedVector7 += indexedVector5;
			indexedVector7 += indexedVector6;
			return indexedVector7 * num;
		}

		public static IndexedVector3 PlaneLineIntersection(ref IndexedVector4 plane, ref IndexedVector3 p0, ref IndexedVector3 p1)
		{
			IndexedVector3 indexedVector = p1 - p0;
			float num = IndexedVector3.Dot(plane.ToVector3(), indexedVector);
			float num2 = (0f - (plane.W + IndexedVector3.Dot(plane.ToVector3(), p0))) / num;
			return p0 + indexedVector * num2;
		}

		public static IndexedVector3 PlaneProject(ref IndexedVector4 plane, ref IndexedVector3 point)
		{
			return point - plane.ToVector3() * (IndexedVector3.Dot(point, plane.ToVector3()) + plane.W);
		}

		public static IndexedVector3 TriNormal(IndexedVector3 v0, IndexedVector3 v1, IndexedVector3 v2)
		{
			return TriNormal(ref v0, ref v1, ref v2);
		}

		public static IndexedVector3 TriNormal(ref IndexedVector3 v0, ref IndexedVector3 v1, ref IndexedVector3 v2)
		{
			IndexedVector3 indexedVector = IndexedVector3.Cross(v1 - v0, v2 - v1);
			float num = indexedVector.Length();
			if (num == 0f)
			{
				return new IndexedVector3(1f, 0f, 0f);
			}
			return indexedVector * (1f / num);
		}

		public static float DistanceBetweenLines(ref IndexedVector3 ustart, ref IndexedVector3 udir, ref IndexedVector3 vstart, ref IndexedVector3 vdir, ref IndexedVector3? upoint, ref IndexedVector3? vpoint)
		{
			IndexedVector3 indexedVector = IndexedVector3.Cross(udir, vdir);
			indexedVector.Normalize();
			float num = 0f - IndexedVector3.Dot(indexedVector, ustart);
			float num2 = 0f - IndexedVector3.Dot(indexedVector, vstart);
			float result = Math.Abs(num - num2);
			if (upoint.HasValue)
			{
				IndexedVector4 plane = new IndexedVector4(IndexedVector3.Cross(vdir, indexedVector).Normalized(), 0f);
				plane.W = 0f - IndexedVector3.Dot(plane.ToVector3(), vstart);
				IndexedVector3 p = ustart + udir;
				upoint = PlaneLineIntersection(ref plane, ref ustart, ref p);
			}
			if (vpoint.HasValue)
			{
				IndexedVector4 plane2 = new IndexedVector4(IndexedVector3.Cross(udir, indexedVector).Normalized(), 0f);
				plane2.W = 0f - IndexedVector3.Dot(plane2.ToVector3(), ustart);
				IndexedVector3 p2 = vstart + vdir;
				vpoint = PlaneLineIntersection(ref plane2, ref vstart, ref p2);
			}
			return result;
		}

		public static PlaneIntersectType PlaneTest(ref IndexedVector4 p, ref IndexedVector3 v)
		{
			float num = 0.0001f;
			float num2 = IndexedVector3.Dot(v, p.ToVector3()) + p.W;
			return (num2 > num) ? PlaneIntersectType.OVER : ((num2 < 0f - num) ? PlaneIntersectType.UNDER : PlaneIntersectType.COPLANAR);
		}

		public static PlaneIntersectType SplitTest(ConvexH convex, ref IndexedVector4 IndexedVector4)
		{
			PlaneIntersectType planeIntersectType = PlaneIntersectType.COPLANAR;
			for (int i = 0; i < convex.vertices.Count; i++)
			{
				IndexedVector3 v = convex.vertices[i];
				planeIntersectType |= PlaneTest(ref IndexedVector4, ref v);
			}
			return planeIntersectType;
		}
	}
}
