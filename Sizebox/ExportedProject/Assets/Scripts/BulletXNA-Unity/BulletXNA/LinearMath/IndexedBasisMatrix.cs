using System;

namespace BulletXNA.LinearMath
{
	public struct IndexedBasisMatrix
	{
		private static IndexedBasisMatrix _identity = new IndexedBasisMatrix(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

		public IndexedVector3 _el0;

		public IndexedVector3 _el1;

		public IndexedVector3 _el2;

		public static IndexedBasisMatrix Identity
		{
			get
			{
				return _identity;
			}
		}

		public float this[int i, int j]
		{
			get
			{
				switch (i)
				{
				case 0:
					switch (j)
					{
					case 0:
						return _el0.X;
					case 1:
						return _el0.Y;
					case 2:
						return _el0.Z;
					}
					break;
				case 1:
					switch (j)
					{
					case 0:
						return _el1.X;
					case 1:
						return _el1.Y;
					case 2:
						return _el1.Z;
					}
					break;
				case 2:
					switch (j)
					{
					case 0:
						return _el2.X;
					case 1:
						return _el2.Y;
					case 2:
						return _el2.Z;
					}
					break;
				}
				return 0f;
			}
			set
			{
				switch (i)
				{
				case 0:
					switch (j)
					{
					case 0:
						_el0.X = value;
						break;
					case 1:
						_el0.Y = value;
						break;
					case 2:
						_el0.Z = value;
						break;
					}
					break;
				case 1:
					switch (j)
					{
					case 0:
						_el1.X = value;
						break;
					case 1:
						_el1.Y = value;
						break;
					case 2:
						_el1.Z = value;
						break;
					}
					break;
				case 2:
					switch (j)
					{
					case 0:
						_el2.X = value;
						break;
					case 1:
						_el2.Y = value;
						break;
					case 2:
						_el2.Z = value;
						break;
					}
					break;
				}
			}
		}

		public IndexedVector3 this[int i]
		{
			get
			{
				switch (i)
				{
				case 0:
					return _el0;
				case 1:
					return _el1;
				case 2:
					return _el2;
				default:
					return IndexedVector3.Zero;
				}
			}
			set
			{
				switch (i)
				{
				case 0:
					_el0 = value;
					break;
				case 1:
					_el1 = value;
					break;
				case 2:
					_el2 = value;
					break;
				}
			}
		}

		public IndexedBasisMatrix Scaled(IndexedVector3 s)
		{
			return new IndexedBasisMatrix(_el0.X * s.X, _el0.Y * s.Y, _el0.Z * s.Z, _el1.X * s.X, _el1.Y * s.Y, _el1.Z * s.Z, _el2.X * s.X, _el2.Y * s.Y, _el2.Z * s.Z);
		}

		public IndexedBasisMatrix Scaled(ref IndexedVector3 s)
		{
			return new IndexedBasisMatrix(_el0.X * s.X, _el0.Y * s.Y, _el0.Z * s.Z, _el1.X * s.X, _el1.Y * s.Y, _el1.Z * s.Z, _el2.X * s.X, _el2.Y * s.Y, _el2.Z * s.Z);
		}

		public IndexedBasisMatrix(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
		{
			_el0 = new IndexedVector3(m11, m12, m13);
			_el1 = new IndexedVector3(m21, m22, m23);
			_el2 = new IndexedVector3(m31, m32, m33);
		}

		public IndexedBasisMatrix(IndexedVector3 row0, IndexedVector3 row1, IndexedVector3 row2)
		{
			_el0 = row0;
			_el1 = row1;
			_el2 = row2;
		}

		public IndexedBasisMatrix(ref IndexedVector3 row0, ref IndexedVector3 row1, ref IndexedVector3 row2)
		{
			_el0 = row0;
			_el1 = row1;
			_el2 = row2;
		}

		public IndexedBasisMatrix(IndexedQuaternion q)
		{
			float num = q.LengthSquared();
			float num2 = 2f / num;
			float num3 = q.X * num2;
			float num4 = q.Y * num2;
			float num5 = q.Z * num2;
			float num6 = q.W * num3;
			float num7 = q.W * num4;
			float num8 = q.W * num5;
			float num9 = q.X * num3;
			float num10 = q.X * num4;
			float num11 = q.X * num5;
			float num12 = q.Y * num4;
			float num13 = q.Y * num5;
			float num14 = q.Z * num5;
			_el0 = new IndexedVector3(1f - (num12 + num14), num10 - num8, num11 + num7);
			_el1 = new IndexedVector3(num10 + num8, 1f - (num9 + num14), num13 - num6);
			_el2 = new IndexedVector3(num11 - num7, num13 + num6, 1f - (num9 + num12));
		}

		public IndexedBasisMatrix(ref IndexedQuaternion q)
		{
			float num = q.LengthSquared();
			float num2 = 2f / num;
			float num3 = q.X * num2;
			float num4 = q.Y * num2;
			float num5 = q.Z * num2;
			float num6 = q.W * num3;
			float num7 = q.W * num4;
			float num8 = q.W * num5;
			float num9 = q.X * num3;
			float num10 = q.X * num4;
			float num11 = q.X * num5;
			float num12 = q.Y * num4;
			float num13 = q.Y * num5;
			float num14 = q.Z * num5;
			_el0 = new IndexedVector3(1f - (num12 + num14), num10 - num8, num11 + num7);
			_el1 = new IndexedVector3(num10 + num8, 1f - (num9 + num14), num13 - num6);
			_el2 = new IndexedVector3(num11 - num7, num13 + num6, 1f - (num9 + num12));
		}

		public void SetValue(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
		{
			_el0 = new IndexedVector3(m11, m12, m13);
			_el1 = new IndexedVector3(m21, m22, m23);
			_el2 = new IndexedVector3(m31, m32, m33);
		}

		public IndexedVector3 GetColumn(int i)
		{
			return new IndexedVector3(_el0[i], _el1[i], _el2[i]);
		}

		public IndexedVector3 GetRow(int i)
		{
			switch (i)
			{
			case 0:
				return _el0;
			case 1:
				return _el1;
			case 2:
				return _el2;
			default:
				return IndexedVector3.Zero;
			}
		}

		public static IndexedBasisMatrix Transpose(IndexedBasisMatrix IndexedMatrix)
		{
			return new IndexedBasisMatrix(IndexedMatrix._el0.X, IndexedMatrix._el1.X, IndexedMatrix._el2.X, IndexedMatrix._el0.Y, IndexedMatrix._el1.Y, IndexedMatrix._el2.Y, IndexedMatrix._el0.Z, IndexedMatrix._el1.Z, IndexedMatrix._el2.Z);
		}

		public static void Transpose(ref IndexedBasisMatrix IndexedMatrix, out IndexedBasisMatrix result)
		{
			result = new IndexedBasisMatrix(IndexedMatrix._el0.X, IndexedMatrix._el1.X, IndexedMatrix._el2.X, IndexedMatrix._el0.Y, IndexedMatrix._el1.Y, IndexedMatrix._el2.Y, IndexedMatrix._el0.Z, IndexedMatrix._el1.Z, IndexedMatrix._el2.Z);
		}

		public static bool operator ==(IndexedBasisMatrix matrix1, IndexedBasisMatrix matrix2)
		{
			if (matrix1._el0 == matrix2._el0 && matrix1._el1 == matrix2._el1)
			{
				return matrix1._el2 == matrix2._el2;
			}
			return false;
		}

		public static bool operator !=(IndexedBasisMatrix matrix1, IndexedBasisMatrix matrix2)
		{
			if (!(matrix1._el0 != matrix2._el0) && !(matrix1._el1 != matrix2._el1))
			{
				return matrix1._el2 != matrix2._el2;
			}
			return true;
		}

		public static IndexedVector3 operator *(IndexedBasisMatrix m, IndexedVector3 v)
		{
			return new IndexedVector3(m._el0.X * v.X + m._el0.Y * v.Y + m._el0.Z * v.Z, m._el1.X * v.X + m._el1.Y * v.Y + m._el1.Z * v.Z, m._el2.X * v.X + m._el2.Y * v.Y + m._el2.Z * v.Z);
		}

		public static void Multiply(ref IndexedVector3 vout, ref IndexedBasisMatrix m, ref IndexedVector3 v)
		{
			vout = new IndexedVector3(m._el0.X * v.X + m._el0.Y * v.Y + m._el0.Z * v.Z, m._el1.X * v.X + m._el1.Y * v.Y + m._el1.Z * v.Z, m._el2.X * v.X + m._el2.Y * v.Y + m._el2.Z * v.Z);
		}

		public static IndexedVector3 operator *(IndexedVector3 v, IndexedBasisMatrix m)
		{
			return new IndexedVector3(m.TDotX(ref v), m.TDotY(ref v), m.TDotZ(ref v));
		}

		public static void Multiply(ref IndexedVector3 vout, ref IndexedVector3 vin, ref IndexedBasisMatrix m)
		{
			vout = new IndexedVector3(m.TDotX(ref vin), m.TDotY(ref vin), m.TDotZ(ref vin));
		}

		public static IndexedBasisMatrix operator *(IndexedBasisMatrix m1, IndexedBasisMatrix m2)
		{
			return new IndexedBasisMatrix(m2.TDotX(ref m1._el0), m2.TDotY(ref m1._el0), m2.TDotZ(ref m1._el0), m2.TDotX(ref m1._el1), m2.TDotY(ref m1._el1), m2.TDotZ(ref m1._el1), m2.TDotX(ref m1._el2), m2.TDotY(ref m1._el2), m2.TDotZ(ref m1._el2));
		}

		public static IndexedBasisMatrix operator *(IndexedBasisMatrix m1, float s)
		{
			return new IndexedBasisMatrix(m1._el0 * s, m1._el1 * s, m1._el2 * s);
		}

		public static IndexedBasisMatrix CreateScale(IndexedVector3 scale)
		{
			return new IndexedBasisMatrix(new IndexedVector3(scale.X, 0f, 0f), new IndexedVector3(0f, scale.Y, 0f), new IndexedVector3(0f, 0f, scale.Z));
		}

		public void SetEulerZYX(float eulerX, float eulerY, float eulerZ)
		{
			float num = (float)Math.Cos(eulerX);
			float num2 = (float)Math.Cos(eulerY);
			float num3 = (float)Math.Cos(eulerZ);
			float num4 = (float)Math.Sin(eulerX);
			float num5 = (float)Math.Sin(eulerY);
			float num6 = (float)Math.Sin(eulerZ);
			float num7 = num * num3;
			float num8 = num * num6;
			float num9 = num4 * num3;
			float num10 = num4 * num6;
			SetValue(num2 * num3, num5 * num9 - num8, num5 * num7 + num10, num2 * num6, num5 * num10 + num7, num5 * num8 - num9, 0f - num5, num2 * num4, num2 * num);
		}

		public float TDotX(ref IndexedVector3 v)
		{
			return _el0.X * v.X + _el1.X * v.Y + _el2.X * v.Z;
		}

		public float TDotY(ref IndexedVector3 v)
		{
			return _el0.Y * v.X + _el1.Y * v.Y + _el2.Y * v.Z;
		}

		public float TDotZ(ref IndexedVector3 v)
		{
			return _el0.Z * v.X + _el1.Z * v.Y + _el2.Z * v.Z;
		}

		public IndexedBasisMatrix Inverse()
		{
			IndexedVector3 v = new IndexedVector3(Cofac(1, 1, 2, 2), Cofac(1, 2, 2, 0), Cofac(1, 0, 2, 1));
			float num = _el0.Dot(v);
			float num2 = 1f / num;
			return new IndexedBasisMatrix(v.X * num2, Cofac(0, 2, 2, 1) * num2, Cofac(0, 1, 1, 2) * num2, v.Y * num2, Cofac(0, 0, 2, 2) * num2, Cofac(0, 2, 1, 0) * num2, v.Z * num2, Cofac(0, 1, 2, 0) * num2, Cofac(0, 0, 1, 1) * num2);
		}

		public float Cofac(int r1, int c1, int r2, int c2)
		{
			return this[r1][c1] * this[r2][c2] - this[r1][c2] * this[r2][c1];
		}

		public IndexedBasisMatrix TransposeTimes(IndexedBasisMatrix m)
		{
			return new IndexedBasisMatrix(_el0.X * m._el0.X + _el1.X * m._el1.X + _el2.X * m._el2.X, _el0.X * m._el0.Y + _el1.X * m._el1.Y + _el2.X * m._el2.Y, _el0.X * m._el0.Z + _el1.X * m._el1.Z + _el2.X * m._el2.Z, _el0.Y * m._el0.X + _el1.Y * m._el1.X + _el2.Y * m._el2.X, _el0.Y * m._el0.Y + _el1.Y * m._el1.Y + _el2.Y * m._el2.Y, _el0.Y * m._el0.Z + _el1.Y * m._el1.Z + _el2.Y * m._el2.Z, _el0.Z * m._el0.X + _el1.Z * m._el1.X + _el2.Z * m._el2.X, _el0.Z * m._el0.Y + _el1.Z * m._el1.Y + _el2.Z * m._el2.Y, _el0.Z * m._el0.Z + _el1.Z * m._el1.Z + _el2.Z * m._el2.Z);
		}

		public IndexedBasisMatrix TransposeTimes(ref IndexedBasisMatrix m)
		{
			return new IndexedBasisMatrix(_el0.X * m._el0.X + _el1.X * m._el1.X + _el2.X * m._el2.X, _el0.X * m._el0.Y + _el1.X * m._el1.Y + _el2.X * m._el2.Y, _el0.X * m._el0.Z + _el1.X * m._el1.Z + _el2.X * m._el2.Z, _el0.Y * m._el0.X + _el1.Y * m._el1.X + _el2.Y * m._el2.X, _el0.Y * m._el0.Y + _el1.Y * m._el1.Y + _el2.Y * m._el2.Y, _el0.Y * m._el0.Z + _el1.Y * m._el1.Z + _el2.Y * m._el2.Z, _el0.Z * m._el0.X + _el1.Z * m._el1.X + _el2.Z * m._el2.X, _el0.Z * m._el0.Y + _el1.Z * m._el1.Y + _el2.Z * m._el2.Y, _el0.Z * m._el0.Z + _el1.Z * m._el1.Z + _el2.Z * m._el2.Z);
		}

		public IndexedBasisMatrix TimesTranspose(IndexedBasisMatrix m)
		{
			return new IndexedBasisMatrix(_el0.Dot(m._el0), _el0.Dot(m._el1), _el0.Dot(m._el2), _el1.Dot(m._el0), _el1.Dot(m._el1), _el1.Dot(m._el2), _el2.Dot(m._el0), _el2.Dot(m._el1), _el2.Dot(m._el2));
		}

		public IndexedBasisMatrix Transpose()
		{
			return new IndexedBasisMatrix(_el0.X, _el1.X, _el2.X, _el0.Y, _el1.Y, _el2.Y, _el0.Z, _el1.Z, _el2.Z);
		}

		public IndexedBasisMatrix Absolute()
		{
			return new IndexedBasisMatrix(_el0.Abs(), _el1.Abs(), _el2.Abs());
		}

		public IndexedQuaternion GetRotation()
		{
			float num = _el0.X + _el1.Y + _el2.Z;
			float num2;
			float w;
			if (num > 0f)
			{
				num2 = (float)Math.Sqrt(num + 1f);
				w = num2 * 0.5f;
				num2 = 0.5f / num2;
				return new IndexedQuaternion((_el2.Y - _el1.Z) * num2, (_el0.Z - _el2.X) * num2, (_el1.X - _el0.Y) * num2, w);
			}
			IndexedVector3 indexedVector = default(IndexedVector3);
			int num3 = ((!(_el0.X < _el1.Y)) ? ((_el0.X < _el2.Z) ? 2 : 0) : ((!(_el1.Y < _el2.Z)) ? 1 : 2));
			int num4 = (num3 + 1) % 3;
			int i = (num4 + 1) % 3;
			num2 = (float)Math.Sqrt(this[num3][num3] - this[num4][num4] - this[i][i] + 1f);
			indexedVector[num3] = num2 * 0.5f;
			num2 = 0.5f / num2;
			w = (this[i][num4] - this[num4][i]) * num2;
			indexedVector[num4] = (this[num4][num3] + this[num3][num4]) * num2;
			indexedVector[i] = (this[i][num3] + this[num3][i]) * num2;
			return new IndexedQuaternion(indexedVector.X, indexedVector.Y, indexedVector.Z, w);
		}

		public void SetRotation(IndexedQuaternion q)
		{
			float num = q.LengthSquared();
			float num2 = 2f / num;
			float num3 = q.X * num2;
			float num4 = q.Y * num2;
			float num5 = q.Z * num2;
			float num6 = q.W * num3;
			float num7 = q.W * num4;
			float num8 = q.W * num5;
			float num9 = q.X * num3;
			float num10 = q.X * num4;
			float num11 = q.X * num5;
			float num12 = q.Y * num4;
			float num13 = q.Y * num5;
			float num14 = q.Z * num5;
			SetValue(1f - (num12 + num14), num10 - num8, num11 + num7, num10 + num8, 1f - (num9 + num14), num13 - num6, num11 - num7, num13 + num6, 1f - (num9 + num12));
		}

		public void SetRotation(ref IndexedQuaternion q)
		{
			float num = q.LengthSquared();
			float num2 = 2f / num;
			float num3 = q.X * num2;
			float num4 = q.Y * num2;
			float num5 = q.Z * num2;
			float num6 = q.W * num3;
			float num7 = q.W * num4;
			float num8 = q.W * num5;
			float num9 = q.X * num3;
			float num10 = q.X * num4;
			float num11 = q.X * num5;
			float num12 = q.Y * num4;
			float num13 = q.Y * num5;
			float num14 = q.Z * num5;
			SetValue(1f - (num12 + num14), num10 - num8, num11 + num7, num10 + num8, 1f - (num9 + num14), num13 - num6, num11 - num7, num13 + num6, 1f - (num9 + num12));
		}

		public void Diagonalize(out IndexedMatrix rot, float threshold, int maxSteps)
		{
			rot = IndexedMatrix.Identity;
			for (int num = maxSteps; num > 0; num--)
			{
				int num2 = 0;
				int num3 = 1;
				int num4 = 2;
				float num5 = Math.Abs(_el0.Y);
				float num6 = Math.Abs(_el0.Z);
				if (num6 > num5)
				{
					num3 = 2;
					num4 = 1;
					num5 = num6;
				}
				num6 = Math.Abs(_el1.Z);
				if (num6 > num5)
				{
					num2 = 1;
					num3 = 2;
					num4 = 0;
					num5 = num6;
				}
				float num7 = threshold * (Math.Abs(_el0.X) + Math.Abs(_el1.Y) + Math.Abs(_el2.Z));
				if (num5 <= num7)
				{
					if (num5 <= 1.1920929E-07f * num7)
					{
						break;
					}
					num = 1;
				}
				float num8 = this[num2][num3];
				float num9 = (this[num3][num3] - this[num2][num2]) / (2f * num8);
				float num10 = num9 * num9;
				float num11;
				float num12;
				if (num10 * num10 < 83886080f)
				{
					num7 = ((num9 >= 0f) ? (1f / (num9 + (float)Math.Sqrt(1f + num10))) : (1f / (num9 - (float)Math.Sqrt(1f + num10))));
					num11 = 1f / (float)Math.Sqrt(1f + num7 * num7);
					num12 = num11 * num7;
				}
				else
				{
					num7 = 1f / (num9 * (2f + 0.5f / num10));
					num11 = 1f - 0.5f * num7 * num7;
					num12 = num11 * num7;
				}
				this[num2, num3] = 0f;
				this[num3, num2] = 0f;
				this[num2, num2] -= num7 * num8;
				this[num3, num3] += num7 * num8;
				float num13 = this[num4][num2];
				float num14 = this[num4][num3];
				int i = num4;
				int j = num2;
				float value = (this[num2, num4] = num11 * num13 - num12 * num14);
				this[i, j] = value;
				int i2 = num4;
				int j2 = num3;
				float value2 = (this[num3, num4] = num11 * num14 + num12 * num13);
				this[i2, j2] = value2;
				for (int k = 0; k < 3; k++)
				{
					num13 = this[k, num2];
					num14 = this[k, num3];
					this[k, num2] = num11 * num13 - num12 * num14;
					this[k, num3] = num11 * num14 + num12 * num13;
				}
			}
		}

		public static IndexedBasisMatrix CreateRotationX(float radians)
		{
			float num = (float)Math.Cos(radians);
			float num2 = (float)Math.Sin(radians);
			IndexedBasisMatrix result = default(IndexedBasisMatrix);
			result._el0 = new IndexedVector3(1f, 0f, 0f);
			result._el1 = new IndexedVector3(0f, num, num2);
			result._el2 = new IndexedVector3(0f, 0f - num2, num);
			return result;
		}

		public static void CreateRotationX(float radians, out IndexedBasisMatrix _basis)
		{
			float num = (float)Math.Cos(radians);
			float num2 = (float)Math.Sin(radians);
			_basis._el0 = new IndexedVector3(1f, 0f, 0f);
			_basis._el1 = new IndexedVector3(0f, num, num2);
			_basis._el2 = new IndexedVector3(0f, 0f - num2, num);
		}

		public static IndexedBasisMatrix CreateRotationZ(float radians)
		{
			float num = (float)Math.Cos(radians);
			float num2 = (float)Math.Sin(radians);
			IndexedBasisMatrix result = default(IndexedBasisMatrix);
			result._el0 = new IndexedVector3(num, num2, 0f);
			result._el1 = new IndexedVector3(0f - num2, num, 0f);
			result._el2 = new IndexedVector3(0f, 0f, 1f);
			return result;
		}

		public static void CreateRotationZ(float radians, out IndexedBasisMatrix _basis)
		{
			float num = (float)Math.Cos(radians);
			float num2 = (float)Math.Sin(radians);
			_basis._el0 = new IndexedVector3(num, num2, 0f);
			_basis._el1 = new IndexedVector3(0f - num2, num, 0f);
			_basis._el2 = new IndexedVector3(0f, 0f, 1f);
		}

		public static IndexedBasisMatrix CreateRotationY(float radians)
		{
			float num = (float)Math.Cos(radians);
			float num2 = (float)Math.Sin(radians);
			return new IndexedBasisMatrix(num, 0f, 0f - num2, 0f, 1f, 0f, num2, 0f, num);
		}

		public static IndexedBasisMatrix CreateFromAxisAngle(IndexedVector3 axis, float angle)
		{
			float x = axis.X;
			float y = axis.Y;
			float z = axis.Z;
			float num = (float)Math.Sin(angle);
			float num2 = (float)Math.Cos(angle);
			float num3 = x * x;
			float num4 = y * y;
			float num5 = z * z;
			float num6 = x * y;
			float num7 = x * z;
			float num8 = y * z;
			return new IndexedBasisMatrix(num3 + num2 * (1f - num3), (float)((double)num6 - (double)num2 * (double)num6 + (double)num * (double)z), (float)((double)num7 - (double)num2 * (double)num7 - (double)num * (double)y), (float)((double)num6 - (double)num2 * (double)num6 - (double)num * (double)z), num4 + num2 * (1f - num4), (float)((double)num8 - (double)num2 * (double)num8 + (double)num * (double)x), (float)((double)num7 - (double)num2 * (double)num7 + (double)num * (double)y), (float)((double)num8 - (double)num2 * (double)num8 - (double)num * (double)x), num5 + num2 * (1f - num5));
		}

		public void GetOpenGLMatrix(out IndexedVector3 v1, out IndexedVector3 v2, out IndexedVector3 v3)
		{
			v1.X = _el0.X;
			v1.Y = _el1.X;
			v1.Z = _el2.X;
			v2.X = _el0.Y;
			v2.Y = _el1.Y;
			v2.Z = _el2.Y;
			v3.X = _el0.Z;
			v3.Y = _el1.Z;
			v3.Z = _el2.Z;
		}

		public void SetOpenGLMatrix(IndexedVector3 v1, IndexedVector3 v2, IndexedVector3 v3)
		{
			SetOpenGLMatrix(ref v1, ref v2, ref v3);
		}

		public void SetOpenGLMatrix(ref IndexedVector3 v1, ref IndexedVector3 v2, ref IndexedVector3 v3)
		{
			_el0 = new IndexedVector3(v1.X, v2.X, v3.X);
			_el1 = new IndexedVector3(v1.Y, v2.Y, v3.Y);
			_el2 = new IndexedVector3(v1.Z, v2.Z, v3.Z);
		}
	}
}
