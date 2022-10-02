using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GJK : IDisposable
	{
		private IndexedVector3[] lastw = new IndexedVector3[4];

		private static uint[] imd3 = new uint[3] { 1u, 2u, 0u };

		private static IndexedVector3[] vt = new IndexedVector3[3];

		private static IndexedVector3[] dl = new IndexedVector3[3];

		private static bool inhere2 = false;

		private static uint[] imd3a = new uint[3] { 1u, 2u, 0u };

		private static IndexedVector3[] vta = new IndexedVector3[4];

		private static IndexedVector3[] dla = new IndexedVector3[3];

		private static bool inhere1 = false;

		public GjkEpaSolver2MinkowskiDiff m_shape;

		public IndexedVector3 m_ray;

		public float m_distance;

		public sSimplex[] m_simplices = new sSimplex[2];

		public sSV[] m_store = new sSV[4];

		public sSV[] m_free = new sSV[4];

		public uint m_nfree;

		public uint m_current;

		public sSimplex m_simplex = new sSimplex();

		public GJKStatus m_status;

		public void Initialise()
		{
			m_ray = IndexedVector3.Zero;
			m_nfree = 0u;
			m_status = GJKStatus.Failed;
			m_current = 0u;
			m_distance = 0f;
			for (int i = 0; i < m_simplices.Length; i++)
			{
				if (m_simplices[i] == null)
				{
					m_simplices[i] = new sSimplex();
				}
			}
			for (int j = 0; j < m_store.Length; j++)
			{
				if (m_store[j] == null)
				{
					m_store[j] = new sSV();
				}
			}
		}

		public GJKStatus Evaluate(GjkEpaSolver2MinkowskiDiff shapearg, ref IndexedVector3 guess)
		{
			uint num = 0u;
			float num2 = 0f;
			float num3 = 0f;
			uint num4 = 0u;
			m_free[0] = m_store[0];
			m_free[1] = m_store[1];
			m_free[2] = m_store[2];
			m_free[3] = m_store[3];
			m_nfree = 4u;
			m_current = 0u;
			m_status = GJKStatus.Valid;
			m_shape = shapearg;
			m_distance = 0f;
			m_simplices[0].rank = 0u;
			m_ray = guess;
			float num5 = m_ray.LengthSquared();
			IndexedVector3 v = ((num5 > 0f) ? (-m_ray) : new IndexedVector3(1f, 0f, 0f));
			AppendVertice(m_simplices[0], ref v);
			m_simplices[0].p[0] = 1f;
			m_ray = m_simplices[0].c[0].w;
			num2 = num5;
			lastw[0] = (lastw[1] = (lastw[2] = (lastw[3] = m_ray)));
			do
			{
				uint num6 = 1 - m_current;
				sSimplex sSimplex2 = m_simplices[m_current];
				sSimplex sSimplex3 = m_simplices[num6];
				float num7 = m_ray.Length();
				if (num7 < 0.0001f)
				{
					m_status = GJKStatus.Inside;
					break;
				}
				IndexedVector3 v2 = -m_ray;
				AppendVertice(sSimplex2, ref v2);
				IndexedVector3 b = sSimplex2.c[sSimplex2.rank - 1].w;
				bool flag = false;
				for (int i = 0; i < 4; i++)
				{
					if ((b - lastw[i]).LengthSquared() < 0.0001f)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					RemoveVertice(m_simplices[m_current]);
					break;
				}
				lastw[num4 = (num4 + 1) & 3u] = b;
				float val = IndexedVector3.Dot(ref m_ray, ref b) / num7;
				num3 = Math.Max(val, num3);
				if (num7 - num3 - 0.0001f * num7 <= 0f)
				{
					RemoveVertice(m_simplices[m_current]);
					break;
				}
				IndexedVector4 w = default(IndexedVector4);
				uint m = 0u;
				switch (sSimplex2.rank)
				{
				case 2u:
					num2 = ProjectOrigin(ref sSimplex2.c[0].w, ref sSimplex2.c[1].w, ref w, ref m);
					break;
				case 3u:
					num2 = ProjectOrigin(ref sSimplex2.c[0].w, ref sSimplex2.c[1].w, ref sSimplex2.c[2].w, ref w, ref m);
					break;
				case 4u:
					num2 = ProjectOrigin(ref sSimplex2.c[0].w, ref sSimplex2.c[1].w, ref sSimplex2.c[2].w, ref sSimplex2.c[3].w, ref w, ref m);
					break;
				}
				if (num2 >= 0f)
				{
					sSimplex3.rank = 0u;
					m_ray = IndexedVector3.Zero;
					m_current = num6;
					uint num8 = 0u;
					for (uint rank = sSimplex2.rank; num8 < rank; num8++)
					{
						if ((m & (1 << (int)num8)) != 0)
						{
							sSimplex3.c[sSimplex3.rank] = sSimplex2.c[num8];
							float num9 = w[(int)num8];
							sSimplex3.p[sSimplex3.rank++] = num9;
							m_ray += sSimplex2.c[num8].w * num9;
						}
						else
						{
							m_free[m_nfree++] = sSimplex2.c[num8];
						}
					}
					if (m == 15)
					{
						m_status = GJKStatus.Inside;
					}
					m_status = ((++num < 128) ? m_status : GJKStatus.Failed);
					continue;
				}
				RemoveVertice(m_simplices[m_current]);
				break;
			}
			while (m_status == GJKStatus.Valid);
			m_simplex = m_simplices[m_current];
			switch (m_status)
			{
			case GJKStatus.Valid:
				m_distance = m_ray.Length();
				break;
			case GJKStatus.Inside:
				m_distance = 0f;
				break;
			}
			return m_status;
		}

		public bool EncloseOrigin()
		{
			switch (m_simplex.rank)
			{
			case 1u:
			{
				for (int j = 0; j < 3; j++)
				{
					IndexedVector3 v6 = IndexedVector3.Zero;
					v6[j] = 1f;
					AppendVertice(m_simplex, ref v6);
					if (EncloseOrigin())
					{
						return true;
					}
					RemoveVertice(m_simplex);
					IndexedVector3 v7 = -v6;
					AppendVertice(m_simplex, ref v7);
					if (EncloseOrigin())
					{
						return true;
					}
					RemoveVertice(m_simplex);
				}
				break;
			}
			case 2u:
			{
				IndexedVector3 v = m_simplex.c[1].w - m_simplex.c[0].w;
				for (int i = 0; i < 3; i++)
				{
					IndexedVector3 zero = IndexedVector3.Zero;
					zero[i] = 1f;
					IndexedVector3 v2 = IndexedVector3.Cross(v, zero);
					if (v2.LengthSquared() > 0f)
					{
						AppendVertice(m_simplex, ref v2);
						if (EncloseOrigin())
						{
							return true;
						}
						RemoveVertice(m_simplex);
						IndexedVector3 v3 = -v2;
						AppendVertice(m_simplex, ref v3);
						if (EncloseOrigin())
						{
							return true;
						}
						RemoveVertice(m_simplex);
					}
				}
				break;
			}
			case 3u:
			{
				IndexedVector3 v4 = IndexedVector3.Cross(m_simplex.c[1].w - m_simplex.c[0].w, m_simplex.c[2].w - m_simplex.c[0].w);
				if (v4.LengthSquared() > 0f)
				{
					AppendVertice(m_simplex, ref v4);
					if (EncloseOrigin())
					{
						return true;
					}
					RemoveVertice(m_simplex);
					IndexedVector3 v5 = -v4;
					AppendVertice(m_simplex, ref v5);
					if (EncloseOrigin())
					{
						return true;
					}
					RemoveVertice(m_simplex);
				}
				break;
			}
			case 4u:
				if (Math.Abs(Det(m_simplex.c[0].w - m_simplex.c[3].w, m_simplex.c[1].w - m_simplex.c[3].w, m_simplex.c[2].w - m_simplex.c[3].w)) > 0f)
				{
					return true;
				}
				break;
			}
			return false;
		}

		public void GetSupport(ref IndexedVector3 d, ref sSV sv)
		{
			sv.d = d / d.Length();
			sv.w = m_shape.Support(ref sv.d);
		}

		public void RemoveVertice(sSimplex simplex)
		{
			m_free[m_nfree++] = simplex.c[--simplex.rank];
		}

		public void AppendVertice(sSimplex simplex, ref IndexedVector3 v)
		{
			simplex.p[simplex.rank] = 0f;
			simplex.c[simplex.rank] = m_free[--m_nfree];
			GetSupport(ref v, ref simplex.c[simplex.rank++]);
		}

		public static float Det(IndexedVector3 a, IndexedVector3 b, IndexedVector3 c)
		{
			return Det(ref a, ref b, ref c);
		}

		public static float Det(ref IndexedVector3 a, ref IndexedVector3 b, ref IndexedVector3 c)
		{
			return a.Y * b.Z * c.X + a.Z * b.X * c.Y - a.X * b.Z * c.Y - a.Y * b.X * c.Z + a.X * b.Y * c.Z - a.Z * b.Y * c.X;
		}

		public static float ProjectOrigin(ref IndexedVector3 a, ref IndexedVector3 b, ref IndexedVector4 w, ref uint m)
		{
			IndexedVector3 b2 = b - a;
			float num = b2.LengthSquared();
			if (num > 0f)
			{
				float num2 = ((num > 0f) ? ((0f - IndexedVector3.Dot(ref a, ref b2)) / num) : 0f);
				if (num2 >= 1f)
				{
					w.X = 0f;
					w.Y = 1f;
					m = 2u;
					return b.LengthSquared();
				}
				if (num2 <= 0f)
				{
					w.X = 1f;
					w.Y = 0f;
					m = 1u;
					return a.LengthSquared();
				}
				w.X = 1f - (w.Y = num2);
				m = 3u;
				return (a + b2 * num2).LengthSquared();
			}
			return -1f;
		}

		public static float ProjectOrigin(ref IndexedVector3 a, ref IndexedVector3 b, ref IndexedVector3 c, ref IndexedVector4 w, ref uint m)
		{
			inhere2 = true;
			vt[0] = a;
			vt[1] = b;
			vt[2] = c;
			dl[0] = a - b;
			dl[1] = b - c;
			dl[2] = c - a;
			IndexedVector3 b2 = IndexedVector3.Cross(dl[0], dl[1]);
			float num = b2.LengthSquared();
			if (num > 0f)
			{
				float num2 = -1f;
				IndexedVector4 w2 = default(IndexedVector4);
				uint m2 = 0u;
				for (int i = 0; i < 3; i++)
				{
					if (IndexedVector3.Dot(vt[i], IndexedVector3.Cross(dl[i], b2)) > 0f)
					{
						uint num3 = imd3[i];
						float num4 = ProjectOrigin(ref vt[i], ref vt[num3], ref w2, ref m2);
						if (num2 < 0f || num4 < num2)
						{
							num2 = num4;
							m = (uint)((((m2 & (true ? 1u : 0u)) != 0) ? (1 << i) : 0) + (((m2 & 2u) != 0) ? (1 << (int)num3) : 0));
							w[i] = w2.X;
							w[(int)num3] = w2.Y;
							w[(int)imd3[num3]] = 0f;
						}
					}
				}
				if (num2 < 0f)
				{
					float num5 = IndexedVector3.Dot(ref a, ref b2);
					float num6 = (float)Math.Sqrt(num);
					IndexedVector3 indexedVector = b2 * (num5 / num);
					num2 = indexedVector.LengthSquared();
					m = 7u;
					w.X = IndexedVector3.Cross(dl[1], b - indexedVector).Length() / num6;
					w.Y = IndexedVector3.Cross(dl[2], c - indexedVector).Length() / num6;
					w.Z = 1f - (w.X + w.Y);
				}
				inhere2 = false;
				return num2;
			}
			inhere2 = false;
			return -1f;
		}

		public static float ProjectOrigin(ref IndexedVector3 a, ref IndexedVector3 b, ref IndexedVector3 c, ref IndexedVector3 d, ref IndexedVector4 w, ref uint m)
		{
			inhere1 = true;
			vta[0] = a;
			vta[1] = b;
			vta[2] = c;
			vta[3] = d;
			dla[0] = a - d;
			dla[1] = b - d;
			dla[2] = c - d;
			float num = Det(dl[0], dl[1], dl[2]);
			if (num * IndexedVector3.Dot(a, IndexedVector3.Cross(b - c, a - b)) <= 0f && Math.Abs(num) > 0f)
			{
				float num2 = -1f;
				IndexedVector4 w2 = default(IndexedVector4);
				uint m2 = 0u;
				for (int i = 0; i < 3; i++)
				{
					uint num3 = imd3[i];
					float num4 = num * IndexedVector3.Dot(d, IndexedVector3.Cross(dl[i], dl[num3]));
					if (num4 > 0f)
					{
						float num5 = ProjectOrigin(ref vt[i], ref vt[num3], ref d, ref w2, ref m2);
						if (num2 < 0f || num5 < num2)
						{
							num2 = num5;
							m = (uint)((((m2 & (true ? 1u : 0u)) != 0) ? (1 << i) : 0) + (((m2 & 2u) != 0) ? (1 << (int)num3) : 0) + (((m2 & 4u) != 0) ? 8 : 0));
							w[i] = w2.X;
							w[(int)num3] = w2.Y;
							w[(int)imd3[num3]] = 0f;
							w.W = w2.Z;
						}
					}
				}
				if (num2 < 0f)
				{
					num2 = 0f;
					m = 15u;
					w.X = Det(c, b, d) / num;
					w.Y = Det(a, c, d) / num;
					w.Z = Det(b, a, d) / num;
					w.W = 1f - (w.X + w.Y + w.Z);
				}
				inhere1 = false;
				return num2;
			}
			inhere1 = false;
			return -1f;
		}

		public void Dispose()
		{
			BulletGlobals.GJKPool.Free(this);
		}
	}
}
