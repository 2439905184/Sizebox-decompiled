using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

public struct FastQuaternion
{
	public IndexedQuaternion q;

	public bool isIdentity;

	public static FastQuaternion Identity
	{
		get
		{
			FastQuaternion result = default(FastQuaternion);
			result.q = IndexedQuaternion.Identity;
			result.isIdentity = true;
			return result;
		}
	}

	public static implicit operator IndexedQuaternion(FastQuaternion fq)
	{
		return fq.q;
	}

	public static implicit operator FastQuaternion(IndexedQuaternion q)
	{
		FastQuaternion result = default(FastQuaternion);
		result.q = q;
		result.isIdentity = q == IndexedQuaternion.Identity;
		return result;
	}

	public static implicit operator FastQuaternion(Quaternion q)
	{
		FastQuaternion result = default(FastQuaternion);
		result.q = q;
		result.isIdentity = q == Quaternion.identity;
		return result;
	}

	public static FastQuaternion operator *(IndexedQuaternion lhs, FastQuaternion rhs)
	{
		if (rhs.isIdentity)
		{
			FastQuaternion result = default(FastQuaternion);
			result.q = lhs;
			result.isIdentity = false;
			return result;
		}
		FastQuaternion result2 = default(FastQuaternion);
		result2.q = lhs * rhs.q;
		result2.isIdentity = false;
		return result2;
	}

	public static FastQuaternion operator *(FastQuaternion lhs, IndexedQuaternion rhs)
	{
		if (lhs.isIdentity)
		{
			FastQuaternion result = default(FastQuaternion);
			result.q = rhs;
			result.isIdentity = false;
			return result;
		}
		FastQuaternion result2 = default(FastQuaternion);
		result2.q = lhs.q * rhs;
		result2.isIdentity = false;
		return result2;
	}

	public static FastQuaternion operator *(FastQuaternion lhs, FastQuaternion rhs)
	{
		if (lhs.isIdentity)
		{
			return rhs;
		}
		if (rhs.isIdentity)
		{
			return lhs;
		}
		FastQuaternion result = default(FastQuaternion);
		result.q = lhs.q * rhs.q;
		result.isIdentity = false;
		return result;
	}

	public override bool Equals(object obj)
	{
		if (obj is FastQuaternion fastQuaternion)
		{
			if (isIdentity == fastQuaternion.isIdentity)
			{
				return q == fastQuaternion.q;
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return q.GetHashCode();
	}

	public static bool operator ==(FastQuaternion a, FastQuaternion b)
	{
		if (a.isIdentity == b.isIdentity)
		{
			return a.q == b.q;
		}
		return false;
	}

	public static bool operator !=(FastQuaternion a, FastQuaternion b)
	{
		if (a.isIdentity)
		{
			return !b.isIdentity;
		}
		if (b.isIdentity)
		{
			return true;
		}
		return a.q != b.q;
	}
}
