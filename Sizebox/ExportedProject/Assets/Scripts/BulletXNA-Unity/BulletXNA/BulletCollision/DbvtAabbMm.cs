using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public struct DbvtAabbMm
	{
		public IndexedVector3 _min;

		public IndexedVector3 _max;

		public IndexedVector3 Center()
		{
			return (_max + _min) / 2f;
		}

		public IndexedVector3 Extent()
		{
			return (_max - _min) / 2f;
		}

		public IndexedVector3 Mins()
		{
			return _min;
		}

		public IndexedVector3 Maxs()
		{
			return _max;
		}

		public IndexedVector3 Lengths()
		{
			return default(IndexedVector3);
		}

		public static float Proximity(ref DbvtAabbMm a, ref DbvtAabbMm b)
		{
			IndexedVector3 indexedVector = a._min + a._max - (b._min + b._max);
			return Math.Abs(indexedVector.X) + Math.Abs(indexedVector.Y) + Math.Abs(indexedVector.Z);
		}

		public static DbvtAabbMm Merge(ref DbvtAabbMm a, ref DbvtAabbMm b)
		{
			DbvtAabbMm r = default(DbvtAabbMm);
			Merge(ref a, ref b, ref r);
			return r;
		}

		public static void Merge(ref DbvtAabbMm a, ref DbvtAabbMm b, ref DbvtAabbMm r)
		{
			MathUtil.VectorMin(ref a._min, ref b._min, out r._min);
			MathUtil.VectorMax(ref a._max, ref b._max, out r._max);
		}

		public static int Select(ref DbvtAabbMm o, ref DbvtAabbMm a, ref DbvtAabbMm b)
		{
			if (!(Proximity(ref o, ref a) < Proximity(ref o, ref b)))
			{
				return 1;
			}
			return 0;
		}

		public static bool NotEqual(ref DbvtAabbMm a, ref DbvtAabbMm b)
		{
			if (a._min.X == b._min.X && a._min.Y == b._min.Y && a._min.Z == b._min.Z && a._max.X == b._max.X && a._max.Y == b._max.Y)
			{
				return a._max.Z != b._max.Z;
			}
			return true;
		}

		public static DbvtAabbMm FromCE(ref IndexedVector3 c, ref IndexedVector3 e)
		{
			DbvtAabbMm result = default(DbvtAabbMm);
			result._min = c - e;
			result._max = c + e;
			return result;
		}

		public static DbvtAabbMm FromCR(ref IndexedVector3 c, float r)
		{
			IndexedVector3 e = new IndexedVector3(r);
			return FromCE(ref c, ref e);
		}

		public static DbvtAabbMm FromMM(ref IndexedVector3 mi, ref IndexedVector3 mx)
		{
			DbvtAabbMm result = default(DbvtAabbMm);
			result._min = mi;
			result._max = mx;
			return result;
		}

		public static DbvtAabbMm FromPoints(ObjectArray<IndexedVector3> points)
		{
			DbvtAabbMm result = default(DbvtAabbMm);
			result._min = (result._max = points[0]);
			for (int i = 1; i < points.Count; i++)
			{
				IndexedVector3 input = points[i];
				MathUtil.VectorMin(ref input, ref result._min);
				MathUtil.VectorMax(ref input, ref result._max);
			}
			return result;
		}

		public static DbvtAabbMm FromPoints(ObjectArray<ObjectArray<IndexedVector3>> points)
		{
			return default(DbvtAabbMm);
		}

		public void Expand(IndexedVector3 e)
		{
			_min -= e;
			_max += e;
		}

		public void SignedExpand(IndexedVector3 e)
		{
			if (e.X > 0f)
			{
				_max.X += e.X;
			}
			else
			{
				_min.X += e.X;
			}
			if (e.Y > 0f)
			{
				_max.Y += e.Y;
			}
			else
			{
				_min.Y += e.Y;
			}
			if (e.Z > 0f)
			{
				_max.Z += e.Z;
			}
			else
			{
				_min.Z += e.Z;
			}
		}

		public bool Contain(ref DbvtAabbMm a)
		{
			if (_min.X <= a._min.X && _min.Y <= a._min.Y && _min.Z <= a._min.Z && _max.X >= a._max.X && _max.Y >= a._max.Y)
			{
				return _max.Z >= a._max.Z;
			}
			return false;
		}

		public static bool Intersect(ref DbvtAabbMm a, ref DbvtAabbMm b)
		{
			if (a._min.X <= b._max.X && a._max.X >= b._min.X && a._min.Y <= b._max.Y && a._max.Y >= b._min.Y && a._min.Z <= b._max.Z)
			{
				return a._max.Z >= b._min.Z;
			}
			return false;
		}

		public static bool Intersect(DbvtAabbMm a, ref IndexedVector3 b)
		{
			if (b.X >= a._min.X && b.Y >= a._min.Y && b.Z >= a._min.Z && b.X <= a._max.X && b.Y <= a._max.Y)
			{
				return b.Z <= a._max.Z;
			}
			return false;
		}

		public int Classify(ref IndexedVector3 n, float o, int s)
		{
			IndexedVector3 b;
			IndexedVector3 b2;
			switch (s)
			{
			case 0:
				b = new IndexedVector3(_min.X, _min.Y, _min.Z);
				b2 = new IndexedVector3(_max.X, _max.Y, _max.Z);
				break;
			case 1:
				b = new IndexedVector3(_max.X, _min.Y, _min.Z);
				b2 = new IndexedVector3(_min.X, _max.Y, _max.Z);
				break;
			case 2:
				b = new IndexedVector3(_min.X, _max.Y, _min.Z);
				b2 = new IndexedVector3(_max.X, _min.Y, _max.Z);
				break;
			case 3:
				b = new IndexedVector3(_max.X, _max.Y, _min.Z);
				b2 = new IndexedVector3(_min.X, _min.Y, _max.Z);
				break;
			case 4:
				b = new IndexedVector3(_min.X, _min.Y, _max.Z);
				b2 = new IndexedVector3(_max.X, _max.Y, _min.Z);
				break;
			case 5:
				b = new IndexedVector3(_max.X, _min.Y, _max.Z);
				b2 = new IndexedVector3(_min.X, _max.Y, _min.Z);
				break;
			case 6:
				b = new IndexedVector3(_min.X, _max.Y, _max.Z);
				b2 = new IndexedVector3(_max.X, _min.Y, _min.Z);
				break;
			case 7:
				b = new IndexedVector3(_max.X, _max.Y, _max.Z);
				b2 = new IndexedVector3(_min.X, _min.Y, _min.Z);
				break;
			default:
				b = default(IndexedVector3);
				b2 = default(IndexedVector3);
				break;
			}
			if (IndexedVector3.Dot(n, b) + o < 0f)
			{
				return -1;
			}
			if (IndexedVector3.Dot(n, b2) + o >= 0f)
			{
				return 1;
			}
			return 0;
		}

		public float ProjectMinimum(ref IndexedVector3 v, uint signs)
		{
			IndexedVector3[] array = new IndexedVector3[2] { _max, _min };
			IndexedVector3 a = new IndexedVector3(array[signs & 1].X, array[(signs >> 1) & 1].Y, array[(signs >> 2) & 1].Z);
			return IndexedVector3.Dot(a, v);
		}

		public void AddSpan(ref IndexedVector3 d, ref float smi, ref float smx)
		{
			for (int i = 0; i < 3; i++)
			{
				if (d[i] < 0f)
				{
					smi += _max[i] * d[i];
					smx += _min[i] * d[i];
				}
				else
				{
					smi += _min[i] * d[i];
					smx += _max[i] * d[i];
				}
			}
		}
	}
}
