using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	public struct FastVector3
	{
		public IndexedVector3 v;

		public bool isZero;

		public static FastVector3 Zero
		{
			get
			{
				FastVector3 result = default(FastVector3);
				result.v = IndexedVector3.Zero;
				result.isZero = true;
				return result;
			}
		}

		public static implicit operator IndexedVector3(FastVector3 fv)
		{
			return fv.v;
		}

		public static implicit operator FastVector3(IndexedVector3 v)
		{
			FastVector3 result = default(FastVector3);
			result.v = v;
			result.isZero = v == IndexedVector3.Zero;
			return result;
		}

		public static implicit operator FastVector3(Vector3 v)
		{
			FastVector3 result = default(FastVector3);
			result.v = v;
			result.isZero = v == Vector3.zero;
			return result;
		}

		public static FastVector3 operator +(IndexedVector3 lhs, FastVector3 rhs)
		{
			if (rhs.isZero)
			{
				FastVector3 result = default(FastVector3);
				result.v = lhs;
				result.isZero = false;
				return result;
			}
			FastVector3 result2 = default(FastVector3);
			result2.v = lhs + rhs.v;
			result2.isZero = false;
			return result2;
		}

		public static FastVector3 operator +(FastVector3 lhs, IndexedVector3 rhs)
		{
			if (lhs.isZero)
			{
				FastVector3 result = default(FastVector3);
				result.v = rhs;
				result.isZero = false;
				return result;
			}
			FastVector3 result2 = default(FastVector3);
			result2.v = lhs.v + rhs;
			result2.isZero = false;
			return result2;
		}

		public static FastVector3 operator +(FastVector3 lhs, FastVector3 rhs)
		{
			if (lhs.isZero)
			{
				return rhs;
			}
			if (rhs.isZero)
			{
				return lhs;
			}
			FastVector3 result = default(FastVector3);
			result.v = lhs.v + rhs.v;
			result.isZero = false;
			return result;
		}

		public override bool Equals(object obj)
		{
			if (obj is FastVector3)
			{
				FastVector3 fastVector = (FastVector3)obj;
				if (isZero == fastVector.isZero)
				{
					return v == fastVector.v;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return v.GetHashCode();
		}

		public static bool operator ==(FastVector3 a, FastVector3 b)
		{
			if (a.isZero == b.isZero)
			{
				return a.v == b.v;
			}
			return false;
		}

		public static bool operator !=(FastVector3 a, FastVector3 b)
		{
			if (a.isZero)
			{
				return !b.isZero;
			}
			if (b.isZero)
			{
				return true;
			}
			return a.v != b.v;
		}
	}
}
