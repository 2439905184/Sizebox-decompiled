using System;
using BulletXNA.LinearMath;

namespace BulletXNA
{
	public static class AabbUtil2
	{
		public static void AabbExpand(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, ref IndexedVector3 expansionMin, ref IndexedVector3 expansionMax)
		{
			aabbMin += expansionMin;
			aabbMax += expansionMax;
		}

		public static bool TestPointAgainstAabb2(ref IndexedVector3 aabbMin1, ref IndexedVector3 aabbMax1, ref IndexedVector3 point)
		{
			bool flag = true;
			flag = !(aabbMin1.X > point.X) && !(aabbMax1.X < point.X) && flag;
			flag = !(aabbMin1.Z > point.Z) && !(aabbMax1.Z < point.Z) && flag;
			return !(aabbMin1.Y > point.Y) && !(aabbMax1.Y < point.Y) && flag;
		}

		public static bool TestAabbAgainstAabb2(ref IndexedVector3 aabbMin1, ref IndexedVector3 aabbMax1, ref IndexedVector3 aabbMin2, ref IndexedVector3 aabbMax2)
		{
			bool flag = true;
			flag = !(aabbMin1.X > aabbMax2.X) && !(aabbMax1.X < aabbMin2.X) && flag;
			flag = !(aabbMin1.Z > aabbMax2.Z) && !(aabbMax1.Z < aabbMin2.Z) && flag;
			return !(aabbMin1.Y > aabbMax2.Y) && !(aabbMax1.Y < aabbMin2.Y) && flag;
		}

		public static bool TestTriangleAgainstAabb2(IndexedVector3[] vertices, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			if (Math.Min(Math.Min(vertices[0].X, vertices[1].X), vertices[2].X) > aabbMax.X)
			{
				return false;
			}
			if (Math.Max(Math.Max(vertices[0].X, vertices[1].X), vertices[2].X) < aabbMin.X)
			{
				return false;
			}
			if (Math.Min(Math.Min(vertices[0].Z, vertices[1].Z), vertices[2].Z) > aabbMax.Z)
			{
				return false;
			}
			if (Math.Max(Math.Max(vertices[0].Z, vertices[1].Z), vertices[2].Z) < aabbMin.Z)
			{
				return false;
			}
			if (Math.Min(Math.Min(vertices[0].Y, vertices[1].Y), vertices[2].Y) > aabbMax.Y)
			{
				return false;
			}
			if (Math.Max(Math.Max(vertices[0].Y, vertices[1].Y), vertices[2].Y) < aabbMin.Y)
			{
				return false;
			}
			return true;
		}

		public static int Outcode(ref IndexedVector3 p, ref IndexedVector3 halfExtent)
		{
			return ((p.X < 0f - halfExtent.X) ? 1 : 0) | ((p.X > halfExtent.X) ? 8 : 0) | ((p.Y < 0f - halfExtent.Y) ? 2 : 0) | ((p.Y > halfExtent.Y) ? 16 : 0) | ((p.Z < 0f - halfExtent.Z) ? 4 : 0) | ((p.Z > halfExtent.Z) ? 32 : 0);
		}

		public static bool RayAabb2(ref IndexedVector3 rayFrom, ref IndexedVector3 rayInvDirection, bool[] raySign, IndexedVector3[] bounds, out float tmin, float lambda_min, float lambda_max)
		{
			tmin = (bounds[raySign[0] ? 1 : 0].X - rayFrom.X) * rayInvDirection.X;
			float num = (bounds[1 - (raySign[0] ? 1 : 0)].X - rayFrom.X) * rayInvDirection.X;
			float num2 = (bounds[raySign[1] ? 1 : 0].Y - rayFrom.Y) * rayInvDirection.Y;
			float num3 = (bounds[1 - (raySign[1] ? 1 : 0)].Y - rayFrom.Y) * rayInvDirection.Y;
			if (tmin > num3 || num2 > num)
			{
				return false;
			}
			if (num2 > tmin)
			{
				tmin = num2;
			}
			if (num3 < num)
			{
				num = num3;
			}
			float num4 = (bounds[raySign[2] ? 1 : 0].Z - rayFrom.Z) * rayInvDirection.Z;
			float num5 = (bounds[1 - (raySign[2] ? 1 : 0)].Z - rayFrom.Z) * rayInvDirection.Z;
			if (tmin > num5 || num4 > num)
			{
				return false;
			}
			if (num4 > tmin)
			{
				tmin = num4;
			}
			if (num5 < num)
			{
				num = num5;
			}
			if (tmin < lambda_max)
			{
				return num > lambda_min;
			}
			return false;
		}

		public static bool RayAabb2Alt(ref IndexedVector3 rayFrom, ref IndexedVector3 rayInvDirection, bool raySign0, bool raySign1, bool raySign2, ref IndexedVector3 minBounds, ref IndexedVector3 maxBounds, out float tmin, float lambda_min, float lambda_max)
		{
			IndexedVector3 obj = (raySign0 ? maxBounds : minBounds);
			tmin = (obj.X - rayFrom.X) * rayInvDirection.X;
			IndexedVector3 obj2 = (raySign0 ? minBounds : maxBounds);
			float num = (obj2.X - rayFrom.X) * rayInvDirection.X;
			IndexedVector3 obj3 = (raySign1 ? maxBounds : minBounds);
			float num2 = (obj3.Y - rayFrom.Y) * rayInvDirection.Y;
			IndexedVector3 obj4 = (raySign1 ? minBounds : maxBounds);
			float num3 = (obj4.Y - rayFrom.Y) * rayInvDirection.Y;
			if (tmin > num3 || num2 > num)
			{
				return false;
			}
			if (num2 > tmin)
			{
				tmin = num2;
			}
			if (num3 < num)
			{
				num = num3;
			}
			IndexedVector3 obj5 = (raySign2 ? maxBounds : minBounds);
			float num4 = (obj5.Z - rayFrom.Z) * rayInvDirection.Z;
			IndexedVector3 obj6 = (raySign2 ? minBounds : maxBounds);
			float num5 = (obj6.Z - rayFrom.Z) * rayInvDirection.Z;
			if (tmin > num5 || num4 > num)
			{
				return false;
			}
			if (num4 > tmin)
			{
				tmin = num4;
			}
			if (num5 < num)
			{
				num = num5;
			}
			if (tmin < lambda_max)
			{
				return num > lambda_min;
			}
			return false;
		}

		public static bool RayAabb(IndexedVector3 rayFrom, IndexedVector3 rayTo, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, ref float param, out IndexedVector3 normal)
		{
			return RayAabb(ref rayFrom, ref rayTo, ref aabbMin, ref aabbMax, ref param, out normal);
		}

		public static bool RayAabb(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, ref float param, out IndexedVector3 normal)
		{
			IndexedVector3 halfExtent = (aabbMax - aabbMin) * 0.5f;
			IndexedVector3 indexedVector = (aabbMax + aabbMin) * 0.5f;
			IndexedVector3 p = rayFrom - indexedVector;
			IndexedVector3 p2 = rayTo - indexedVector;
			int num = Outcode(ref p, ref halfExtent);
			int num2 = Outcode(ref p2, ref halfExtent);
			if ((num & num2) == 0)
			{
				float num3 = 0f;
				float num4 = param;
				IndexedVector3 indexedVector2 = p2 - p;
				float num5 = 1f;
				IndexedVector3 zero = IndexedVector3.Zero;
				int num6 = 1;
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j != 3; j++)
					{
						if ((num & num6) != 0)
						{
							float num7 = (0f - p[j] - halfExtent[j] * num5) / indexedVector2[j];
							if (num3 <= num7)
							{
								num3 = num7;
								zero = IndexedVector3.Zero;
								zero[j] = num5;
							}
						}
						else if ((num2 & num6) != 0)
						{
							float val = (0f - p[j] - halfExtent[j] * num5) / indexedVector2[j];
							num4 = Math.Min(num4, val);
						}
						num6 <<= 1;
					}
					num5 = -1f;
				}
				if (num3 <= num4)
				{
					param = num3;
					normal = zero;
					return true;
				}
			}
			param = 0f;
			normal = IndexedVector3.Zero;
			return false;
		}

		public static bool TestQuantizedAabbAgainstQuantizedAabb(ref UShortVector3 aabbMin1, ref UShortVector3 aabbMax1, ref UShortVector3 aabbMin2, ref UShortVector3 aabbMax2)
		{
			bool flag = true;
			flag = aabbMin1.X <= aabbMax2.X && aabbMax1.X >= aabbMin2.X && flag;
			flag = aabbMin1.Z <= aabbMax2.Z && aabbMax1.Z >= aabbMin2.Z && flag;
			return aabbMin1.Y <= aabbMax2.Y && aabbMax1.Y >= aabbMin2.Y && flag;
		}

		public static void TransformAabb(IndexedVector3 halfExtents, float margin, ref IndexedMatrix t, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
		{
			TransformAabb(ref halfExtents, margin, ref t, out aabbMinOut, out aabbMaxOut);
		}

		public static void TransformAabb(ref IndexedVector3 halfExtents, float margin, ref IndexedMatrix t, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
		{
			IndexedVector3 v = halfExtents + new IndexedVector3(margin);
			IndexedBasisMatrix indexedBasisMatrix = t._basis.Absolute();
			IndexedVector3 origin = t._origin;
			IndexedVector3 indexedVector = new IndexedVector3(indexedBasisMatrix._el0.Dot(ref v), indexedBasisMatrix._el1.Dot(ref v), indexedBasisMatrix._el2.Dot(ref v));
			aabbMinOut = origin - indexedVector;
			aabbMaxOut = origin + indexedVector;
		}

		public static void TransformAabb(IndexedVector3 localAabbMin, IndexedVector3 localAabbMax, float margin, ref IndexedMatrix trans, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
		{
			IndexedVector3 v = -0.5f * (localAabbMax - localAabbMin);
			v += new IndexedVector3(margin);
			IndexedVector3 indexedVector = 0.5f * (localAabbMax + localAabbMin);
			IndexedBasisMatrix indexedBasisMatrix = trans._basis.Absolute();
			IndexedVector3 indexedVector2 = trans * indexedVector;
			IndexedVector3 indexedVector3 = new IndexedVector3(indexedBasisMatrix._el0.Dot(ref v), indexedBasisMatrix._el1.Dot(ref v), indexedBasisMatrix._el2.Dot(ref v));
			aabbMinOut = indexedVector2 - indexedVector3;
			aabbMaxOut = indexedVector2 + indexedVector3;
		}

		public static void TransformAabb(ref IndexedVector3 localAabbMin, ref IndexedVector3 localAabbMax, float margin, ref IndexedMatrix trans, out IndexedVector3 aabbMinOut, out IndexedVector3 aabbMaxOut)
		{
			IndexedVector3 v = 0.5f * (localAabbMax - localAabbMin);
			v += new IndexedVector3(margin);
			IndexedVector3 indexedVector = 0.5f * (localAabbMax + localAabbMin);
			IndexedBasisMatrix indexedBasisMatrix = trans._basis.Absolute();
			IndexedVector3 indexedVector2 = trans * indexedVector;
			IndexedVector3 indexedVector3 = new IndexedVector3(indexedBasisMatrix._el0.Dot(ref v), indexedBasisMatrix._el1.Dot(ref v), indexedBasisMatrix._el2.Dot(ref v));
			aabbMinOut = indexedVector2 - indexedVector3;
			aabbMaxOut = indexedVector2 + indexedVector3;
		}
	}
}
