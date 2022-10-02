namespace BulletXNA.LinearMath
{
	public struct IndexedVector4
	{
		private static IndexedVector4 _zero = new IndexedVector4(0f);

		private static IndexedVector4 _one = new IndexedVector4(1f);

		public float X;

		public float Y;

		public float Z;

		public float W;

		public static IndexedVector4 Zero
		{
			get
			{
				return _zero;
			}
		}

		public float this[int i]
		{
			get
			{
				switch (i)
				{
				case 0:
					return X;
				case 1:
					return Y;
				case 2:
					return Z;
				case 3:
					return W;
				default:
					return 0f;
				}
			}
			set
			{
				switch (i)
				{
				case 0:
					X = value;
					break;
				case 1:
					Y = value;
					break;
				case 2:
					Z = value;
					break;
				case 3:
					W = value;
					break;
				}
			}
		}

		public IndexedVector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public IndexedVector4(float x)
		{
			X = x;
			Y = x;
			Z = x;
			W = x;
		}

		public IndexedVector4(IndexedVector4 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = v.W;
		}

		public IndexedVector4(IndexedVector3 v, float w)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = w;
		}

		public IndexedVector4(ref IndexedVector4 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = v.W;
		}

		public IndexedVector3 ToVector3()
		{
			return new IndexedVector3(X, Y, Z);
		}

		public static IndexedVector4 operator -(IndexedVector4 value)
		{
			IndexedVector4 result = default(IndexedVector4);
			result.X = 0f - value.X;
			result.Y = 0f - value.Y;
			result.Z = 0f - value.Z;
			result.W = 0f - value.W;
			return result;
		}

		public static IndexedVector4 operator *(IndexedVector4 value, float scaleFactor)
		{
			IndexedVector4 result = default(IndexedVector4);
			result.X = value.X * scaleFactor;
			result.Y = value.Y * scaleFactor;
			result.Z = value.Z * scaleFactor;
			result.W = value.W * scaleFactor;
			return result;
		}
	}
}
