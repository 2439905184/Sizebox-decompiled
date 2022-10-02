using System;
using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class EPA
	{
		public eStatus m_status;

		public sSimplex m_result;

		public IndexedVector3 m_normal;

		public float m_depth;

		public sSV[] m_sv_store = new sSV[64];

		public sFace[] m_fc_store = new sFace[128];

		public uint m_nextsv;

		public IList<sFace> m_hull = new List<sFace>();

		public IList<sFace> m_stock = new List<sFace>();

		private static sFace[] tetra = new sFace[4];

		private static uint[] i1m3 = new uint[3] { 1u, 2u, 0u };

		private static uint[] i2m3 = new uint[3] { 2u, 0u, 1u };

		public EPA()
		{
			Initialize();
		}

		public void Initialize()
		{
			m_status = eStatus.Failed;
			m_normal = IndexedVector3.Zero;
			m_depth = 0f;
			m_nextsv = 0u;
			m_result = new sSimplex();
			for (int i = 0; i < m_sv_store.Length; i++)
			{
				m_sv_store[i] = new sSV();
			}
			for (int j = 0; j < m_fc_store.Length; j++)
			{
				m_fc_store[j] = new sFace();
			}
			for (int k = 0; k < 128; k++)
			{
				Append(m_stock, m_fc_store[128 - k - 1]);
			}
		}

		public static void Bind(sFace fa, uint ea, sFace fb, uint eb)
		{
			fa.e[ea] = eb;
			fa.f[ea] = fb;
			fb.e[eb] = ea;
			fb.f[eb] = fa;
		}

		public static void Append(IList<sFace> list, sFace face)
		{
			list.Add(face);
		}

		public static void Remove(IList<sFace> list, sFace face)
		{
			list.Remove(face);
		}

		private void SwapSv(sSV[] array, int a, int b)
		{
			sSV sSV2 = array[a];
			array[a] = array[b];
			array[b] = sSV2;
		}

		private void SwapFloat(float[] array, int a, int b)
		{
			float num = array[a];
			array[a] = array[b];
			array[b] = num;
		}

		public eStatus Evaluate(GJK gjk, ref IndexedVector3 guess)
		{
			sSimplex simplex = gjk.m_simplex;
			if (simplex.rank > 1 && gjk.EncloseOrigin())
			{
				while (m_hull.Count > 0)
				{
					sFace face = m_hull[0];
					Remove(m_hull, face);
					Append(m_stock, face);
				}
				m_status = eStatus.Valid;
				m_nextsv = 0u;
				if (GJK.Det(simplex.c[0].w - simplex.c[3].w, simplex.c[1].w - simplex.c[3].w, simplex.c[2].w - simplex.c[3].w) < 0f)
				{
					SwapSv(simplex.c, 0, 1);
					SwapFloat(simplex.p, 0, 1);
				}
				tetra[0] = NewFace(simplex.c[0], simplex.c[1], simplex.c[2], true);
				tetra[1] = NewFace(simplex.c[1], simplex.c[0], simplex.c[3], true);
				tetra[2] = NewFace(simplex.c[2], simplex.c[1], simplex.c[3], true);
				tetra[3] = NewFace(simplex.c[0], simplex.c[2], simplex.c[3], true);
				if (m_hull.Count == 4)
				{
					sFace sFace2 = FindBest();
					sFace sFace3 = sFace2;
					uint num = 0u;
					uint num2 = 0u;
					Bind(tetra[0], 0u, tetra[1], 0u);
					Bind(tetra[0], 1u, tetra[2], 0u);
					Bind(tetra[0], 2u, tetra[3], 0u);
					Bind(tetra[1], 1u, tetra[3], 2u);
					Bind(tetra[1], 2u, tetra[2], 1u);
					Bind(tetra[2], 2u, tetra[3], 1u);
					m_status = eStatus.Valid;
					for (; num2 < 255; num2++)
					{
						if (m_nextsv < 64)
						{
							sHorizon horizon = default(sHorizon);
							sSV sv = m_sv_store[m_nextsv++];
							bool flag = true;
							num = (sFace2.pass = num + 1);
							gjk.GetSupport(ref sFace2.n, ref sv);
							float num3 = IndexedVector3.Dot(ref sFace2.n, ref sv.w) - sFace2.d;
							if (num3 > 0.0001f)
							{
								for (int i = 0; i < 3; i++)
								{
									if (!flag)
									{
										break;
									}
									flag &= Expand(num, sv, sFace2.f[i], sFace2.e[i], ref horizon);
								}
								if (flag && horizon.nf >= 3)
								{
									Bind(horizon.cf, 1u, horizon.ff, 2u);
									Remove(m_hull, sFace2);
									Append(m_stock, sFace2);
									sFace2 = FindBest();
									if (sFace2.p >= sFace3.p)
									{
										sFace3 = sFace2;
									}
									continue;
								}
								m_status = eStatus.InvalidHull;
								break;
							}
							m_status = eStatus.AccuraryReached;
							break;
						}
						m_status = eStatus.OutOfVertices;
						break;
					}
					IndexedVector3 indexedVector = sFace3.n * sFace3.d;
					m_normal = sFace3.n;
					m_depth = sFace3.d;
					m_result.rank = 3u;
					m_result.c[0] = sFace3.c[0];
					m_result.c[1] = sFace3.c[1];
					m_result.c[2] = sFace3.c[2];
					m_result.p[0] = IndexedVector3.Cross(sFace3.c[1].w - indexedVector, sFace3.c[2].w - indexedVector).Length();
					m_result.p[1] = IndexedVector3.Cross(sFace3.c[2].w - indexedVector, sFace3.c[0].w - indexedVector).Length();
					m_result.p[2] = IndexedVector3.Cross(sFace3.c[0].w - indexedVector, sFace3.c[1].w - indexedVector).Length();
					float num4 = m_result.p[0] + m_result.p[1] + m_result.p[2];
					m_result.p[0] /= num4;
					m_result.p[1] /= num4;
					m_result.p[2] /= num4;
					return m_status;
				}
			}
			m_status = eStatus.FallBack;
			m_normal = -guess;
			float num5 = m_normal.LengthSquared();
			if (num5 > 0f)
			{
				m_normal.Normalize();
			}
			else
			{
				m_normal = new IndexedVector3(1f, 0f, 0f);
			}
			m_depth = 0f;
			m_result.rank = 1u;
			m_result.c[0] = simplex.c[0];
			m_result.p[0] = 1f;
			return m_status;
		}

		public sFace NewFace(sSV a, sSV b, sSV c, bool forced)
		{
			if (m_stock.Count > 0)
			{
				sFace sFace2 = m_stock[0];
				Remove(m_stock, sFace2);
				Append(m_hull, sFace2);
				sFace2.pass = 0u;
				sFace2.c[0] = a;
				sFace2.c[1] = b;
				sFace2.c[2] = c;
				sFace2.n = IndexedVector3.Cross(b.w - a.w, c.w - a.w);
				float num = sFace2.n.Length();
				bool flag = num > 0.0001f;
				sFace2.p = Math.Min(Math.Min(IndexedVector3.Dot(a.w, IndexedVector3.Cross(sFace2.n, a.w - b.w)), IndexedVector3.Dot(b.w, IndexedVector3.Cross(sFace2.n, b.w - c.w))), IndexedVector3.Dot(c.w, IndexedVector3.Cross(sFace2.n, c.w - a.w))) / (flag ? num : 1f);
				sFace2.p = ((sFace2.p >= -0.01f) ? 0f : sFace2.p);
				if (flag)
				{
					sFace2.d = IndexedVector3.Dot(ref a.w, ref sFace2.n) / num;
					sFace2.n /= num;
					if (forced || sFace2.d >= -1E-05f)
					{
						return sFace2;
					}
					m_status = eStatus.NonConvex;
				}
				else
				{
					m_status = eStatus.Degenerated;
				}
				Remove(m_hull, sFace2);
				Append(m_stock, sFace2);
				return null;
			}
			m_status = ((m_stock.Count > 0) ? eStatus.OutOfVertices : eStatus.OutOfFaces);
			return null;
		}

		public sFace FindBest()
		{
			sFace sFace2 = m_hull[0];
			float num = sFace2.d * sFace2.d;
			float p = sFace2.p;
			for (int i = 0; i < m_hull.Count; i++)
			{
				sFace sFace3 = m_hull[i];
				float num2 = sFace3.d * sFace3.d;
				if (sFace3.p >= p && num2 < num)
				{
					sFace2 = sFace3;
					num = num2;
					p = sFace3.p;
				}
			}
			return sFace2;
		}

		public bool Expand(uint pass, sSV w, sFace f, uint e, ref sHorizon horizon)
		{
			if (f.pass != pass)
			{
				uint num = i1m3[e];
				if (IndexedVector3.Dot(ref f.n, ref w.w) - f.d < -1E-05f)
				{
					sFace sFace2 = NewFace(f.c[num], f.c[e], w, false);
					if (sFace2 != null)
					{
						Bind(sFace2, 0u, f, e);
						if (horizon.cf != null)
						{
							Bind(horizon.cf, 1u, sFace2, 2u);
						}
						else
						{
							horizon.ff = sFace2;
						}
						horizon.cf = sFace2;
						horizon.nf++;
						return true;
					}
				}
				else
				{
					uint num2 = i2m3[e];
					f.pass = pass;
					if (Expand(pass, w, f.f[num], f.e[num], ref horizon) && Expand(pass, w, f.f[num2], f.e[num2], ref horizon))
					{
						Remove(m_hull, f);
						Append(m_stock, f);
						return true;
					}
				}
			}
			return false;
		}
	}
}
