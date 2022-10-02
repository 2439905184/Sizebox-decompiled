using System;
using UnityEngine;

namespace BulletXNA.LinearMath
{
	public struct IndexedVector3
	{
		private static IndexedVector3 _zero = default(IndexedVector3);

		private static IndexedVector3 _one = new IndexedVector3(1f, 1f, 1f);

		private static IndexedVector3 _unitX = new IndexedVector3(1f, 0f, 0f);

		private static IndexedVector3 _unitY = new IndexedVector3(0f, 1f, 0f);

		private static IndexedVector3 _unitZ = new IndexedVector3(0f, 0f, 1f);

		private static IndexedVector3 _up = new IndexedVector3(0f, 1f, 0f);

		private static IndexedVector3 _down = new IndexedVector3(0f, -1f, 0f);

		private static IndexedVector3 _right = new IndexedVector3(1f, 0f, 0f);

		private static IndexedVector3 _left = new IndexedVector3(-1f, 0f, 0f);

		private static IndexedVector3 _forward = new IndexedVector3(0f, 0f, -1f);

		private static IndexedVector3 _backward = new IndexedVector3(0f, 0f, 1f);

		public float X;

		public float Y;

		public float Z;

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
				}
			}
		}

		public static IndexedVector3 Zero
		{
			get
			{
				return _zero;
			}
		}

		public static IndexedVector3 One
		{
			get
			{
				return _one;
			}
		}

		public static IndexedVector3 Up
		{
			get
			{
				return _up;
			}
		}

		public static IndexedVector3 Down
		{
			get
			{
				return _down;
			}
		}

		public static IndexedVector3 Right
		{
			get
			{
				return _right;
			}
		}

		public static IndexedVector3 Left
		{
			get
			{
				return _left;
			}
		}

		public static IndexedVector3 Forward
		{
			get
			{
				return _forward;
			}
		}

		public static IndexedVector3 Backward
		{
			get
			{
				return _backward;
			}
		}

		public IndexedVector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public IndexedVector3(float x)
		{
			X = x;
			Y = x;
			Z = x;
		}

		public IndexedVector3(IndexedVector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		public IndexedVector3(ref IndexedVector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		public IndexedVector3(ref Vector3 v)
		{
			X = v.x;
			Y = v.y;
			Z = v.z;
		}

		public IndexedVector3(Vector3 v)
		{
			X = v.x;
			Y = v.y;
			Z = v.z;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(X, Y, Z);
		}

		public void ToVector3(out Vector3 result)
		{
			result = new Vector3(X, Y, Z);
		}

		public static IndexedVector3 operator +(Vector3 value1, IndexedVector3 value2)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value1.x + value2.X;
			result.Y = value1.y + value2.Y;
			result.Z = value1.z + value2.Z;
			return result;
		}

		public static IndexedVector3 operator +(IndexedVector3 value1, Vector3 value2)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value1.X + value2.x;
			result.Y = value1.Y + value2.y;
			result.Z = value1.Z + value2.z;
			return result;
		}

		public static IndexedVector3 operator -(Vector3 value1, IndexedVector3 value2)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value1.x - value2.X;
			result.Y = value1.y - value2.Y;
			result.Z = value1.z - value2.Z;
			return result;
		}

		public static IndexedVector3 operator -(IndexedVector3 value1, Vector3 value2)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value1.X - value2.x;
			result.Y = value1.Y - value2.y;
			result.Z = value1.Z - value2.z;
			return result;
		}

		public static implicit operator Vector3(IndexedVector3 v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		public static implicit operator IndexedVector3(Vector3 v)
		{
			return new IndexedVector3(v.x, v.y, v.z);
		}

		public static float Dot(ref IndexedVector3 a, ref Vector3 b)
		{
			return a.X * b.x + a.Y * b.y + a.Z * b.z;
		}

		public static float Dot(IndexedVector3 a, Vector3 b)
		{
			return a.X * b.x + a.Y * b.y + a.Z * b.z;
		}

		public static float Dot(Vector3 a, IndexedVector3 b)
		{
			return a.x * b.X + a.y * b.Y + a.z * b.Z;
		}

		public IndexedVector3(ref IndexedVector4 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		public IndexedVector3(IndexedVector4 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		public float Length()
		{
			return Mathf.Sqrt(X * X + Y * Y + Z * Z);
		}

		public float LengthSquared()
		{
			return X * X + Y * Y + Z * Z;
		}

		public void Abs(out IndexedVector3 result)
		{
			result.X = Math.Abs(X);
			result.Y = Math.Abs(Y);
			result.Z = Math.Abs(Z);
		}

		public IndexedVector3 Abs()
		{
			return new IndexedVector3(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
		}

		public IndexedVector3 Absolute()
		{
			return new IndexedVector3(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
		}

		public void Normalize()
		{
			float num = X * X + Y * Y + Z * Z;
			if (num > float.Epsilon)
			{
				num = 1f / Mathf.Sqrt(num);
				X *= num;
				Y *= num;
				Z *= num;
			}
			else
			{
				X = 0f;
				Y = 0f;
				Z = 0f;
			}
		}

		public IndexedVector3 Normalized()
		{
			float num = X * X + Y * Y + Z * Z;
			if (num > float.Epsilon)
			{
				num = 1f / Mathf.Sqrt(num);
				return new IndexedVector3(X * num, Y * num, Z * num);
			}
			return new IndexedVector3(0f, 0f, 0f);
		}

		public static void Transform(IndexedVector3[] source, ref IndexedMatrix t, IndexedVector3[] dest)
		{
			for (int i = 0; i < source.Length; i++)
			{
				dest[i] = t * source[i];
			}
		}

		public static IndexedVector3 Normalize(IndexedVector3 v)
		{
			float num = v.X * v.X + v.Y * v.Y + v.Z * v.Z;
			if (num > float.Epsilon)
			{
				num = 1f / Mathf.Sqrt(num);
				return new IndexedVector3(v.X * num, v.Y * num, v.Z * num);
			}
			return new IndexedVector3(0f, 0f, 0f);
		}

		public static IndexedVector3 Normalize(ref IndexedVector3 v)
		{
			float num = v.X * v.X + v.Y * v.Y + v.Z * v.Z;
			if (num > float.Epsilon)
			{
				num = 1f / Mathf.Sqrt(num);
				return new IndexedVector3(v.X * num, v.Y * num, v.Z * num);
			}
			return new IndexedVector3(0f, 0f, 0f);
		}

		public IndexedVector3 Cross(ref IndexedVector3 v)
		{
			return new IndexedVector3(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
		}

		public IndexedVector3 Cross(IndexedVector3 v)
		{
			return new IndexedVector3(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
		}

		public static IndexedVector3 Cross(IndexedVector3 v, IndexedVector3 v2)
		{
			return new IndexedVector3(v.Y * v2.Z - v.Z * v2.Y, v.Z * v2.X - v.X * v2.Z, v.X * v2.Y - v.Y * v2.X);
		}

		public static IndexedVector3 Cross(ref IndexedVector3 v, ref IndexedVector3 v2)
		{
			return new IndexedVector3(v.Y * v2.Z - v.Z * v2.Y, v.Z * v2.X - v.X * v2.Z, v.X * v2.Y - v.Y * v2.X);
		}

		public static void Cross(out IndexedVector3 r, ref IndexedVector3 v, ref IndexedVector3 v2)
		{
			r = new IndexedVector3(v.Y * v2.Z - v.Z * v2.Y, v.Z * v2.X - v.X * v2.Z, v.X * v2.Y - v.Y * v2.X);
		}

		public static float Dot(IndexedVector3 a, IndexedVector3 b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		public static float Dot(ref IndexedVector3 a, ref IndexedVector3 b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		public static IndexedVector3 operator +(IndexedVector3 value1, IndexedVector3 value2)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			result.Z = value1.Z + value2.Z;
			return result;
		}

		public static IndexedVector3 operator -(IndexedVector3 value1, IndexedVector3 value2)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			result.Z = value1.Z - value2.Z;
			return result;
		}

		public static IndexedVector3 operator *(IndexedVector3 value, float scaleFactor)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value.X * scaleFactor;
			result.Y = value.Y * scaleFactor;
			result.Z = value.Z * scaleFactor;
			return result;
		}

		public static IndexedVector3 operator /(IndexedVector3 value, float scaleFactor)
		{
			float num = 1f / scaleFactor;
			IndexedVector3 result = default(IndexedVector3);
			result.X = value.X * num;
			result.Y = value.Y * num;
			result.Z = value.Z * num;
			return result;
		}

		public static IndexedVector3 operator *(float scaleFactor, IndexedVector3 value)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value.X * scaleFactor;
			result.Y = value.Y * scaleFactor;
			result.Z = value.Z * scaleFactor;
			return result;
		}

		public static IndexedVector3 operator -(IndexedVector3 value)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = 0f - value.X;
			result.Y = 0f - value.Y;
			result.Z = 0f - value.Z;
			return result;
		}

		public static IndexedVector3 operator *(IndexedVector3 value1, IndexedVector3 value2)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			result.Z = value1.Z * value2.Z;
			return result;
		}

		public static void Multiply(ref IndexedVector3 output, ref IndexedVector3 value1, ref IndexedVector3 value2)
		{
			output.X = value1.X * value2.X;
			output.Y = value1.Y * value2.Y;
			output.Z = value1.Z * value2.Z;
		}

		public static void Multiply(ref IndexedVector3 output, ref IndexedVector3 value1, float value2)
		{
			output.X = value1.X * value2;
			output.Y = value1.Y * value2;
			output.Z = value1.Z * value2;
		}

		public static void Multiply(ref IndexedVector3 output, ref IndexedVector3 value1, ref IndexedVector3 value2, float value3)
		{
			output.X = value1.X * value2.X * value3;
			output.Y = value1.Y * value2.Y * value3;
			output.Z = value1.Z * value2.Z * value3;
		}

		public static void Subtract(out IndexedVector3 output, ref IndexedVector3 value1, ref IndexedVector3 value2)
		{
			output.X = value1.X - value2.X;
			output.Y = value1.Y - value2.Y;
			output.Z = value1.Z - value2.Z;
		}

		public static IndexedVector3 Subtract(ref IndexedVector3 value1, ref IndexedVector3 value2)
		{
			return new IndexedVector3(value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z);
		}

		public static void Add(ref IndexedVector3 output, ref IndexedVector3 value1, ref IndexedVector3 value2)
		{
			output.X = value1.X + value2.X;
			output.Y = value1.Y + value2.Y;
			output.Z = value1.Z + value2.Z;
		}

		public static IndexedVector3 operator /(IndexedVector3 value1, IndexedVector3 value2)
		{
			IndexedVector3 result = default(IndexedVector3);
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			result.Z = value1.Z / value2.Z;
			return result;
		}

		public float[] ToFloatArray()
		{
			return new float[3] { X, Y, Z };
		}

		public static void Lerp(ref IndexedVector3 a, ref IndexedVector3 b, float t, out IndexedVector3 c)
		{
			c = new IndexedVector3(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
		}

		public static IndexedVector3 Lerp(ref IndexedVector3 a, ref IndexedVector3 b, float t)
		{
			return new IndexedVector3(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
		}

		public static bool operator ==(IndexedVector3 value1, IndexedVector3 value2)
		{
			if (value1.X == value2.X && value1.Y == value2.Y)
			{
				return value1.Z == value2.Z;
			}
			return false;
		}

		public static bool operator !=(IndexedVector3 value1, IndexedVector3 value2)
		{
			if (value1.X == value2.X && value1.Y == value2.Y)
			{
				return value1.Z != value2.Z;
			}
			return true;
		}

		public bool Equals(IndexedVector3 other)
		{
			if (X == other.X && Y == other.Y)
			{
				return Z == other.Z;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			if (obj is IndexedVector3)
			{
				result = Equals((IndexedVector3)obj);
			}
			return result;
		}

		public float Dot(ref IndexedVector3 v)
		{
			return X * v.X + Y * v.Y + Z * v.Z;
		}

		public float Dot(IndexedVector3 v)
		{
			return X * v.X + Y * v.Y + Z * v.Z;
		}

		public float Triple(ref IndexedVector3 b, ref IndexedVector3 c)
		{
			return X * (b.Y * c.Z - b.Z * c.Y) + Y * (b.Z * c.X - b.X * c.Z) + Z * (b.X * c.Y - b.Y * c.X);
		}

		public void SetMin(ref IndexedVector3 v)
		{
			if (v.X < X)
			{
				X = v.X;
			}
			if (v.Y < Y)
			{
				Y = v.Y;
			}
			if (v.Z < Z)
			{
				Z = v.Z;
			}
		}

		public void SetMax(ref IndexedVector3 v)
		{
			if (v.X > X)
			{
				X = v.X;
			}
			if (v.Y > Y)
			{
				Y = v.Y;
			}
			if (v.Z > Z)
			{
				Z = v.Z;
			}
		}

		public int MaxAxis()
		{
			if (!(X < Y))
			{
				if (!(X < Z))
				{
					return 0;
				}
				return 2;
			}
			if (!(Y < Z))
			{
				return 1;
			}
			return 2;
		}

		public int MinAxis()
		{
			if (!(X < Y))
			{
				if (!(Y < Z))
				{
					return 2;
				}
				return 1;
			}
			if (!(X < Z))
			{
				return 2;
			}
			return 0;
		}

		public override string ToString()
		{
			return "X : " + X + " Y " + Y + " Z " + Z;
		}
	}
}
