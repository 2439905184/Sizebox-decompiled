using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GjkEpaSolver2
	{
		public const int GJK_MAX_ITERATIONS = 128;

		public const float GJK_ACCURARY = 0.0001f;

		public const float GJK_MIN_DISTANCE = 0.0001f;

		public const float GJK_DUPLICATED_EPS = 0.0001f;

		public const float GJK_SIMPLEX2_EPS = 0f;

		public const float GJK_SIMPLEX3_EPS = 0f;

		public const float GJK_SIMPLEX4_EPS = 0f;

		public const int EPA_MAX_VERTICES = 64;

		public const int EPA_MAX_FACES = 128;

		public const int EPA_MAX_ITERATIONS = 255;

		public const float EPA_ACCURACY = 0.0001f;

		public const float EPA_FALLBACK = 0.0009999999f;

		public const float EPA_PLANE_EPS = 1E-05f;

		public const float EPA_INSIDE_EPS = 0.01f;

		private static EPA epa = new EPA();

		public static void Initialize(ConvexShape shape0, ref IndexedMatrix wtrs0, ConvexShape shape1, ref IndexedMatrix wtrs1, ref GjkEpaSolver2Results results, GjkEpaSolver2MinkowskiDiff shapeR, bool withmargins)
		{
			results.witnesses0 = IndexedVector3.Zero;
			results.witnesses1 = IndexedVector3.Zero;
			results.status = GjkEpaSolver2Status.Separated;
			shapeR.m_shapes[0] = shape0;
			shapeR.m_shapes[1] = shape1;
			shapeR.m_toshape1 = wtrs1._basis.TransposeTimes(ref wtrs0._basis);
			shapeR.m_toshape0 = wtrs0.InverseTimes(ref wtrs1);
			shapeR.EnableMargin(withmargins);
		}

		public static bool Distance(ConvexShape shape0, ref IndexedMatrix wtrs0, ConvexShape shape1, ref IndexedMatrix wtrs1, ref IndexedVector3 guess, ref GjkEpaSolver2Results results)
		{
			using (GjkEpaSolver2MinkowskiDiff gjkEpaSolver2MinkowskiDiff = BulletGlobals.GjkEpaSolver2MinkowskiDiffPool.Get())
			{
				using (GJK gJK = BulletGlobals.GJKPool.Get())
				{
					Initialize(shape0, ref wtrs0, shape1, ref wtrs1, ref results, gjkEpaSolver2MinkowskiDiff, false);
					gJK.Initialise();
					GJKStatus gJKStatus = gJK.Evaluate(gjkEpaSolver2MinkowskiDiff, ref guess);
					if (gJKStatus == GJKStatus.Valid)
					{
						IndexedVector3 zero = IndexedVector3.Zero;
						IndexedVector3 zero2 = IndexedVector3.Zero;
						for (uint num = 0u; num < gJK.m_simplex.rank; num++)
						{
							float num2 = gJK.m_simplex.p[num];
							zero += gjkEpaSolver2MinkowskiDiff.Support(ref gJK.m_simplex.c[num].d, 0u) * num2;
							IndexedVector3 d = -gJK.m_simplex.c[num].d;
							zero2 += gjkEpaSolver2MinkowskiDiff.Support(ref d, 1u) * num2;
						}
						results.witnesses0 = wtrs0 * zero;
						results.witnesses1 = wtrs0 * zero2;
						results.normal = zero - zero2;
						results.distance = results.normal.Length();
						results.normal /= ((results.distance > 0.0001f) ? results.distance : 1f);
						return true;
					}
					results.status = ((gJKStatus == GJKStatus.Inside) ? GjkEpaSolver2Status.Penetrating : GjkEpaSolver2Status.GJK_Failed);
					return false;
				}
			}
		}

		public static bool Penetration(ConvexShape shape0, ref IndexedMatrix wtrs0, ConvexShape shape1, ref IndexedMatrix wtrs1, ref IndexedVector3 guess, ref GjkEpaSolver2Results results)
		{
			return Penetration(shape0, ref wtrs0, shape1, ref wtrs1, ref guess, ref results, true);
		}

		public static bool Penetration(ConvexShape shape0, ref IndexedMatrix wtrs0, ConvexShape shape1, ref IndexedMatrix wtrs1, ref IndexedVector3 guess, ref GjkEpaSolver2Results results, bool usemargins)
		{
			using (GjkEpaSolver2MinkowskiDiff gjkEpaSolver2MinkowskiDiff = BulletGlobals.GjkEpaSolver2MinkowskiDiffPool.Get())
			{
				using (GJK gJK = BulletGlobals.GJKPool.Get())
				{
					Initialize(shape0, ref wtrs0, shape1, ref wtrs1, ref results, gjkEpaSolver2MinkowskiDiff, usemargins);
					gJK.Initialise();
					IndexedVector3 guess2 = -guess;
					switch (gJK.Evaluate(gjkEpaSolver2MinkowskiDiff, ref guess2))
					{
					case GJKStatus.Inside:
					{
						eStatus eStatus2 = epa.Evaluate(gJK, ref guess2);
						if (eStatus2 != eStatus.Failed)
						{
							IndexedVector3 zero = IndexedVector3.Zero;
							for (uint num = 0u; num < epa.m_result.rank; num++)
							{
								zero += gjkEpaSolver2MinkowskiDiff.Support(ref epa.m_result.c[num].d, 0u) * epa.m_result.p[num];
							}
							results.status = GjkEpaSolver2Status.Penetrating;
							results.witnesses0 = wtrs0 * zero;
							results.witnesses1 = wtrs0 * (zero - epa.m_normal * epa.m_depth);
							results.normal = -epa.m_normal;
							results.distance = 0f - epa.m_depth;
							return true;
						}
						results.status = GjkEpaSolver2Status.EPA_Failed;
						break;
					}
					case GJKStatus.Failed:
						results.status = GjkEpaSolver2Status.GJK_Failed;
						break;
					}
				}
			}
			return false;
		}

		public float SignedDistance(ref IndexedVector3 position, float margin, ConvexShape shape0, ref IndexedMatrix wtrs0, ref GjkEpaSolver2Results results)
		{
			using (GjkEpaSolver2MinkowskiDiff gjkEpaSolver2MinkowskiDiff = BulletGlobals.GjkEpaSolver2MinkowskiDiffPool.Get())
			{
				using (GJK gJK = BulletGlobals.GJKPool.Get())
				{
					SphereShape sphereShape = BulletGlobals.SphereShapePool.Get();
					sphereShape.Initialize(margin);
					IndexedMatrix wtrs = IndexedMatrix.CreateFromQuaternion(IndexedQuaternion.Identity);
					wtrs0._origin = position;
					Initialize(shape0, ref wtrs0, sphereShape, ref wtrs, ref results, gjkEpaSolver2MinkowskiDiff, false);
					gJK.Initialise();
					IndexedVector3 guess = new IndexedVector3(1f);
					switch (gJK.Evaluate(gjkEpaSolver2MinkowskiDiff, ref guess))
					{
					case GJKStatus.Valid:
					{
						IndexedVector3 zero = IndexedVector3.Zero;
						IndexedVector3 zero2 = IndexedVector3.Zero;
						for (int i = 0; i < gJK.m_simplex.rank; i++)
						{
							float num2 = gJK.m_simplex.p[i];
							zero += gjkEpaSolver2MinkowskiDiff.Support(ref gJK.m_simplex.c[i].d, 0u) * num2;
							IndexedVector3 d = -gJK.m_simplex.c[i].d;
							zero2 += gjkEpaSolver2MinkowskiDiff.Support(ref d, 1u) * num2;
						}
						results.witnesses0 = wtrs0 * zero;
						results.witnesses1 = wtrs0 * zero2;
						IndexedVector3 indexedVector2 = results.witnesses1 - results.witnesses0;
						float num3 = shape0.GetMarginNonVirtual() + sphereShape.GetMarginNonVirtual();
						float num4 = indexedVector2.Length();
						results.normal = indexedVector2 / num4;
						results.witnesses0 += results.normal * num3;
						return num4 - num3;
					}
					case GJKStatus.Inside:
						if (Penetration(shape0, ref wtrs0, sphereShape, ref wtrs, ref gJK.m_ray, ref results))
						{
							IndexedVector3 indexedVector = results.witnesses0 - results.witnesses1;
							float num = indexedVector.Length();
							if (num >= 1.1920929E-07f)
							{
								results.normal = indexedVector / num;
							}
							return 0f - num;
						}
						break;
					}
					BulletGlobals.SphereShapePool.Free(sphereShape);
				}
			}
			return float.MaxValue;
		}

		public bool SignedDistance(ConvexShape shape0, ref IndexedMatrix wtrs0, ConvexShape shape1, ref IndexedMatrix wtrs1, ref IndexedVector3 guess, ref GjkEpaSolver2Results results)
		{
			if (!Distance(shape0, ref wtrs0, shape1, ref wtrs1, ref guess, ref results))
			{
				return Penetration(shape0, ref wtrs0, shape1, ref wtrs1, ref guess, ref results, false);
			}
			return true;
		}
	}
}
