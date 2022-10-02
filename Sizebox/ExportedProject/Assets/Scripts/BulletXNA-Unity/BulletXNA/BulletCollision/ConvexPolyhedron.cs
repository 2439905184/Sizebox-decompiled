using System;
using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ConvexPolyhedron
	{
		public ObjectArray<IndexedVector3> m_vertices = new ObjectArray<IndexedVector3>();

		public ObjectArray<Face> m_faces = new ObjectArray<Face>();

		public ObjectArray<IndexedVector3> m_uniqueEdges = new ObjectArray<IndexedVector3>();

		public IndexedVector3 m_localCenter;

		public IndexedVector3 m_extents;

		public float m_radius;

		public IndexedVector3 mC;

		public IndexedVector3 mE;

		public virtual void Cleanup()
		{
		}

		public void Initialize()
		{
			Dictionary<InternalVertexPair, InternalEdge> dictionary = new Dictionary<InternalVertexPair, InternalEdge>();
			float num = 0f;
			m_localCenter = IndexedVector3.Zero;
			for (int i = 0; i < m_faces.Count; i++)
			{
				int count = m_faces[i].m_indices.Count;
				int num2 = count;
				for (int j = 0; j < num2; j++)
				{
					int index = (j + 1) % count;
					InternalVertexPair internalVertexPair = new InternalVertexPair(m_faces[i].m_indices[j], m_faces[i].m_indices[index]);
					InternalEdge internalEdge = dictionary[internalVertexPair];
					IndexedVector3 indexedVector = m_vertices[internalVertexPair.m_v1] - m_vertices[internalVertexPair.m_v0];
					indexedVector.Normalize();
					bool flag = false;
					for (int k = 0; k < m_uniqueEdges.Count; k++)
					{
						if (MathUtil.IsAlmostZero(m_uniqueEdges[k] - indexedVector) || MathUtil.IsAlmostZero(m_uniqueEdges[k] + indexedVector))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						m_uniqueEdges.Add(indexedVector);
					}
					if (internalEdge != null)
					{
						internalEdge.m_face1 = i;
						continue;
					}
					InternalEdge internalEdge2 = new InternalEdge();
					internalEdge2.m_face0 = i;
					dictionary[internalVertexPair] = internalEdge2;
				}
			}
			for (int l = 0; l < m_faces.Count; l++)
			{
				int count2 = m_faces[l].m_indices.Count;
				int num3 = count2 - 2;
				IndexedVector3 indexedVector2 = m_vertices[m_faces[l].m_indices[0]];
				for (int m = 1; m <= num3; m++)
				{
					int index2 = (m + 1) % count2;
					IndexedVector3 indexedVector3 = m_vertices[m_faces[l].m_indices[m]];
					IndexedVector3 indexedVector4 = m_vertices[m_faces[l].m_indices[index2]];
					float num4 = IndexedVector3.Cross(indexedVector2 - indexedVector3, indexedVector2 - indexedVector4).Length() * 0.5f;
					IndexedVector3 indexedVector5 = (indexedVector2 + indexedVector3 + indexedVector4) / 3f;
					m_localCenter += num4 * indexedVector5;
					num += num4;
				}
			}
			m_localCenter /= num;
			m_radius = float.MaxValue;
			for (int n = 0; n < m_faces.Count; n++)
			{
				IndexedVector3 v = new IndexedVector3(m_faces[n].m_plane[0], m_faces[n].m_plane[1], m_faces[n].m_plane[2]);
				float num5 = Math.Abs(m_localCenter.Dot(v) + m_faces[n].m_plane[3]);
				if (num5 < m_radius)
				{
					m_radius = num5;
				}
			}
			float num6 = float.MaxValue;
			float num7 = float.MaxValue;
			float num8 = float.MaxValue;
			float num9 = float.MinValue;
			float num10 = float.MinValue;
			float num11 = float.MinValue;
			for (int num12 = 0; num12 < m_vertices.Count; num12++)
			{
				IndexedVector3 indexedVector6 = m_vertices[num12];
				if (indexedVector6.X < num6)
				{
					num6 = indexedVector6.X;
				}
				if (indexedVector6.X > num9)
				{
					num9 = indexedVector6.X;
				}
				if (indexedVector6.Y < num7)
				{
					num7 = indexedVector6.Y;
				}
				if (indexedVector6.Y > num10)
				{
					num10 = indexedVector6.Y;
				}
				if (indexedVector6.Z < num8)
				{
					num8 = indexedVector6.Z;
				}
				if (indexedVector6.Z > num11)
				{
					num11 = indexedVector6.Z;
				}
			}
			mC = new IndexedVector3(num9 + num6, num10 + num7, num11 + num8);
			mE = new IndexedVector3(num9 - num6, num10 - num7, num11 - num8);
			float num13 = m_radius / (float)Math.Sqrt(3.0);
			int num14 = mE.MaxAxis();
			float num15 = (mE[num14] * 0.5f - num13) / 1024f;
			m_extents.X = (m_extents.Y = (m_extents.Z = num13));
			m_extents[num14] = mE[num14] * 0.5f;
			bool flag2 = false;
			for (int num16 = 0; num16 < 1024; num16++)
			{
				if (TestContainment())
				{
					flag2 = true;
					break;
				}
				m_extents[num14] -= num15;
			}
			if (!flag2)
			{
				m_extents.X = (m_extents.Y = (m_extents.Z = num13));
				return;
			}
			float num17 = (m_radius - num13) / 1024f;
			int num18 = (1 << num14) & 3;
			int i2 = (1 << num18) & 3;
			for (int num19 = 0; num19 < 1024; num19++)
			{
				float value = m_extents[num18];
				float value2 = m_extents[i2];
				m_extents[num18] += num17;
				m_extents[i2] += num17;
				if (!TestContainment())
				{
					m_extents[num18] = value;
					m_extents[i2] = value2;
					break;
				}
			}
		}

		public bool TestContainment()
		{
			for (int i = 0; i < 8; i++)
			{
				IndexedVector3 indexedVector = IndexedVector3.Zero;
				switch (i)
				{
				case 0:
					indexedVector = m_localCenter + new IndexedVector3(m_extents.X, m_extents.Y, m_extents.Z);
					break;
				case 1:
					indexedVector = m_localCenter + new IndexedVector3(m_extents.X, m_extents.Y, 0f - m_extents.Z);
					break;
				case 2:
					indexedVector = m_localCenter + new IndexedVector3(m_extents.X, 0f - m_extents.Y, m_extents.Z);
					break;
				case 3:
					indexedVector = m_localCenter + new IndexedVector3(m_extents.X, 0f - m_extents.Y, 0f - m_extents.Z);
					break;
				case 4:
					indexedVector = m_localCenter + new IndexedVector3(0f - m_extents.X, m_extents.Y, m_extents.Z);
					break;
				case 5:
					indexedVector = m_localCenter + new IndexedVector3(0f - m_extents.X, m_extents.Y, 0f - m_extents.Z);
					break;
				case 6:
					indexedVector = m_localCenter + new IndexedVector3(0f - m_extents.X, 0f - m_extents.Y, m_extents.Z);
					break;
				case 7:
					indexedVector = m_localCenter + new IndexedVector3(0f - m_extents.X, 0f - m_extents.Y, 0f - m_extents.Z);
					break;
				}
				for (int j = 0; j < m_faces.Count; j++)
				{
					IndexedVector3 v = new IndexedVector3(m_faces[j].m_plane[0], m_faces[j].m_plane[1], m_faces[j].m_plane[2]);
					float num = indexedVector.Dot(v) + m_faces[j].m_plane[3];
					if (num > 0f)
					{
						return false;
					}
				}
			}
			return true;
		}

		public void Project(ref IndexedMatrix trans, ref IndexedVector3 dir, out float min, out float max)
		{
			min = float.MaxValue;
			max = float.MinValue;
			int count = m_vertices.Count;
			for (int i = 0; i < count; i++)
			{
				IndexedVector3 a = trans * m_vertices[i];
				float num = IndexedVector3.Dot(a, dir);
				if (num < min)
				{
					min = num;
				}
				if (num > max)
				{
					max = num;
				}
			}
			if (min > max)
			{
				float num2 = min;
				min = max;
				max = num2;
			}
		}
	}
}
