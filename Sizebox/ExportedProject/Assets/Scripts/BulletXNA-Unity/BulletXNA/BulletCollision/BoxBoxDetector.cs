using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public static class BoxBoxDetector
	{
		private static float fudge_factor = 1.05f;

		private static float[] s_buffer = new float[12];

		private static float[] s_quadBuffer = new float[16];

		private static float[] s_temp1 = new float[12];

		private static float[] s_temp2 = new float[12];

		private static float[] s_quad = new float[8];

		private static float[] s_ret = new float[16];

		private static float[] s_point = new float[24];

		private static float[] s_dep = new float[8];

		private static float[] s_A = new float[8];

		private static float[] s_rectReferenceFace = new float[2];

		private static int[] s_availablePoints = new int[8];

		public static void GetClosestPoints(BoxShape box1, BoxShape box2, ref ClosestPointInput input, ManifoldResult output, IDebugDraw debugDraw, bool swapResults)
		{
			IndexedMatrix transformA = input.m_transformA;
			IndexedMatrix transformB = input.m_transformB;
			int skip = 0;
			object contact = null;
			IndexedVector3 normal = default(IndexedVector3);
			float depth = 0f;
			int return_code = -1;
			int maxc = 4;
			IndexedVector3 p = new IndexedVector3(transformA._origin);
			IndexedVector3 p2 = new IndexedVector3(transformB._origin);
			IndexedVector3 side = new IndexedVector3(2f * box1.GetHalfExtentsWithMargin());
			IndexedVector3 side2 = new IndexedVector3(2f * box2.GetHalfExtentsWithMargin());
			transformA._basis.Transpose();
			transformB._basis.Transpose();
			for (int i = 0; i < 3; i++)
			{
				s_temp1[4 * i] = transformA._basis[i].X;
				s_temp2[4 * i] = transformB._basis[i].X;
				s_temp1[1 + 4 * i] = transformA._basis[i].Y;
				s_temp2[1 + 4 * i] = transformB._basis[i].Y;
				s_temp1[2 + 4 * i] = transformA._basis[i].Z;
				s_temp2[2 + 4 * i] = transformB._basis[i].Z;
			}
			DBoxBox2(ref p, s_temp1, ref side, ref p2, s_temp2, ref side2, ref normal, ref depth, ref return_code, maxc, contact, skip, output);
		}

		private static int DBoxBox2(ref IndexedVector3 p1, float[] R1, ref IndexedVector3 side1, ref IndexedVector3 p2, float[] R2, ref IndexedVector3 side2, ref IndexedVector3 normal, ref float depth, ref int return_code, int maxc, object contact, int skip, IDiscreteCollisionDetectorInterfaceResult output)
		{
			float[] normalR = null;
			IndexedVector3 indexedVector = side1 * 0.5f;
			IndexedVector3 indexedVector2 = side2 * 0.5f;
			IndexedVector3 C = p2 - p1;
			IndexedVector3 A = default(IndexedVector3);
			DMULTIPLY1_331(ref A, R1, ref C);
			float num = DDOT44(R1, 0, R2, 0);
			float num2 = DDOT44(R1, 0, R2, 1);
			float num3 = DDOT44(R1, 0, R2, 2);
			float num4 = DDOT44(R1, 1, R2, 0);
			float num5 = DDOT44(R1, 1, R2, 1);
			float num6 = DDOT44(R1, 1, R2, 2);
			float num7 = DDOT44(R1, 2, R2, 0);
			float num8 = DDOT44(R1, 2, R2, 1);
			float num9 = DDOT44(R1, 2, R2, 2);
			float num10 = Math.Abs(num);
			float num11 = Math.Abs(num2);
			float num12 = Math.Abs(num3);
			float num13 = Math.Abs(num4);
			float num14 = Math.Abs(num5);
			float num15 = Math.Abs(num6);
			float num16 = Math.Abs(num7);
			float num17 = Math.Abs(num8);
			float num18 = Math.Abs(num9);
			float s = float.MinValue;
			bool invert_normal = false;
			int code = 0;
			int normalROffset = 0;
			if (TST(A.X, indexedVector.X + indexedVector2.X * num10 + indexedVector2.Y * num11 + indexedVector2.Z * num12, R1, ref normalR, 0, ref normalROffset, 1, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST(A.Y, indexedVector.Y + indexedVector2.X * num13 + indexedVector2.Y * num14 + indexedVector2.Z * num15, R1, ref normalR, 1, ref normalROffset, 2, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST(A.Z, indexedVector.Z + indexedVector2.X * num16 + indexedVector2.Y * num17 + indexedVector2.Z * num18, R1, ref normalR, 2, ref normalROffset, 3, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST(DDOT41(R2, 0, ref C, 0), indexedVector.X * num10 + indexedVector.Y * num13 + indexedVector.Z * num16 + indexedVector2.X, R2, ref normalR, 0, ref normalROffset, 4, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST(DDOT41(R2, 1, ref C, 0), indexedVector.X * num11 + indexedVector.Y * num14 + indexedVector.Z * num17 + indexedVector2.Y, R2, ref normalR, 1, ref normalROffset, 5, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST(DDOT41(R2, 2, ref C, 0), indexedVector.X * num12 + indexedVector.Y * num15 + indexedVector.Z * num18 + indexedVector2.Z, R2, ref normalR, 2, ref normalROffset, 6, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			IndexedVector3 normalC = default(IndexedVector3);
			float num19 = 1E-05f;
			num10 += num19;
			num11 += num19;
			num12 += num19;
			num13 += num19;
			num14 += num19;
			num15 += num19;
			num16 += num19;
			num17 += num19;
			num18 += num19;
			if (TST2(A.Z * num4 - A.Y * num7, indexedVector.Y * num16 + indexedVector.Z * num13 + indexedVector2.Y * num12 + indexedVector2.Z * num11, 0f, 0f - num7, num4, ref normalC, ref normalR, 7, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST2(A.Z * num5 - A.Y * num8, indexedVector.Y * num17 + indexedVector.Z * num14 + indexedVector2.X * num12 + indexedVector2.Z * num10, 0f, 0f - num8, num5, ref normalC, ref normalR, 8, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST2(A.Z * num6 - A.Y * num9, indexedVector.Y * num18 + indexedVector.Z * num15 + indexedVector2.X * num11 + indexedVector2.Y * num10, 0f, 0f - num9, num6, ref normalC, ref normalR, 9, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST2(A.X * num7 - A.Z * num, indexedVector.X * num16 + indexedVector.Z * num10 + indexedVector2.Y * num15 + indexedVector2.Z * num14, num7, 0f, 0f - num, ref normalC, ref normalR, 10, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST2(A.X * num8 - A.Z * num2, indexedVector.X * num17 + indexedVector.Z * num11 + indexedVector2.X * num15 + indexedVector2.Z * num13, num8, 0f, 0f - num2, ref normalC, ref normalR, 11, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST2(A.X * num9 - A.Z * num3, indexedVector.X * num18 + indexedVector.Z * num12 + indexedVector2.X * num14 + indexedVector2.Y * num13, num9, 0f, 0f - num3, ref normalC, ref normalR, 12, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST2(A.Y * num - A.X * num4, indexedVector.X * num13 + indexedVector.Y * num10 + indexedVector2.Y * num18 + indexedVector2.Z * num17, 0f - num4, num, 0f, ref normalC, ref normalR, 13, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST2(A.Y * num2 - A.X * num5, indexedVector.X * num14 + indexedVector.Y * num11 + indexedVector2.X * num18 + indexedVector2.Z * num16, 0f - num5, num2, 0f, ref normalC, ref normalR, 14, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (TST2(A.Y * num3 - A.X * num6, indexedVector.X * num15 + indexedVector.Y * num12 + indexedVector2.X * num17 + indexedVector2.Y * num16, 0f - num6, num3, 0f, ref normalC, ref normalR, 15, ref code, ref s, ref invert_normal))
			{
				return 0;
			}
			if (code == 0)
			{
				return 0;
			}
			if (normalR != null)
			{
				normal.X = normalR[normalROffset];
				normal.Y = normalR[4 + normalROffset];
				normal.Z = normalR[8 + normalROffset];
			}
			else
			{
				DMULTIPLY0_331(ref normal, R1, ref normalC);
			}
			if (invert_normal)
			{
				normal = -normal;
			}
			depth = 0f - s;
			if (code > 6)
			{
				IndexedVector3 pa = p1;
				for (int i = 0; i < 3; i++)
				{
					float num20 = ((DDOT14(ref normal, 0, R1, i) > 0f) ? 1f : (-1f));
					for (int j = 0; j < 3; j++)
					{
						pa[j] += num20 * indexedVector[i] * R1[j * 4 + i];
					}
				}
				IndexedVector3 pb = p2;
				for (int k = 0; k < 3; k++)
				{
					float num21 = ((DDOT14(ref normal, 0, R2, k) > 0f) ? (-1f) : 1f);
					for (int l = 0; l < 3; l++)
					{
						pb[l] += num21 * indexedVector2[k] * R2[l * 4 + k];
					}
				}
				IndexedVector3 ua = default(IndexedVector3);
				IndexedVector3 ub = default(IndexedVector3);
				for (int m = 0; m < 3; m++)
				{
					ua[m] = R1[(code - 7) / 3 + m * 4];
				}
				for (int n = 0; n < 3; n++)
				{
					ub[n] = R2[(code - 7) % 3 + n * 4];
				}
				float alpha;
				float beta;
				DLineClosestApproach(ref pa, ref ua, ref pb, ref ub, out alpha, out beta);
				for (int num22 = 0; num22 < 3; num22++)
				{
					pa[num22] += ua[num22] * alpha;
				}
				for (int num23 = 0; num23 < 3; num23++)
				{
					pb[num23] += ub[num23] * beta;
				}
				output.AddContactPoint(-normal, pb, 0f - depth);
				return_code = code;
				return 1;
			}
			float[] array;
			float[] array2;
			IndexedVector3 indexedVector3;
			IndexedVector3 indexedVector4;
			IndexedVector3 indexedVector5;
			IndexedVector3 indexedVector6;
			if (code <= 3)
			{
				array = R1;
				array2 = R2;
				indexedVector3 = p1;
				indexedVector4 = p2;
				indexedVector5 = indexedVector;
				indexedVector6 = indexedVector2;
			}
			else
			{
				array = R2;
				array2 = R1;
				indexedVector3 = p2;
				indexedVector4 = p1;
				indexedVector5 = indexedVector2;
				indexedVector6 = indexedVector;
			}
			IndexedVector3 A2 = default(IndexedVector3);
			IndexedVector3 C2 = ((code > 3) ? (-normal) : normal);
			DMULTIPLY1_331(ref A2, array2, ref C2);
			IndexedVector3 result;
			A2.Abs(out result);
			int num24;
			int num25;
			int num26;
			if (result.Y > result.X)
			{
				if (result.Y > result.Z)
				{
					num24 = 0;
					num25 = 1;
					num26 = 2;
				}
				else
				{
					num24 = 0;
					num26 = 1;
					num25 = 2;
				}
			}
			else if (result.X > result.Z)
			{
				num25 = 0;
				num24 = 1;
				num26 = 2;
			}
			else
			{
				num24 = 0;
				num26 = 1;
				num25 = 2;
			}
			IndexedVector3 a = ((!(A2[num25] < 0f)) ? new IndexedVector3(indexedVector4.X - indexedVector3.X - indexedVector6[num25] * array2[num25], indexedVector4.Y - indexedVector3.Y - indexedVector6[num25] * array2[num25 + 4], indexedVector4.Z - indexedVector3.Z - indexedVector6[num25] * array2[num25 + 8]) : new IndexedVector3(indexedVector4.X - indexedVector3.X + indexedVector6[num25] * array2[num25], indexedVector4.Y - indexedVector3.Y + indexedVector6[num25] * array2[num25 + 4], indexedVector4.Z - indexedVector3.Z + indexedVector6[num25] * array2[num25 + 8]));
			int num27 = ((code > 3) ? (code - 4) : (code - 1));
			int num28;
			int num29;
			switch (num27)
			{
			case 0:
				num28 = 1;
				num29 = 2;
				break;
			case 1:
				num28 = 0;
				num29 = 2;
				break;
			default:
				num28 = 0;
				num29 = 1;
				break;
			}
			float[] array3 = s_quad;
			float num30 = DDOT14(ref a, 0, array, num28);
			float num31 = DDOT14(ref a, 0, array, num29);
			float num32 = DDOT44(array, num28, array2, num24);
			float num33 = DDOT44(array, num28, array2, num26);
			float num34 = DDOT44(array, num29, array2, num24);
			float num35 = DDOT44(array, num29, array2, num26);
			float num36 = num32 * indexedVector6[num24];
			float num37 = num34 * indexedVector6[num24];
			float num38 = num33 * indexedVector6[num26];
			float num39 = num35 * indexedVector6[num26];
			array3[0] = num30 - num36 - num38;
			array3[1] = num31 - num37 - num39;
			array3[2] = num30 - num36 + num38;
			array3[3] = num31 - num37 + num39;
			array3[4] = num30 + num36 + num38;
			array3[5] = num31 + num37 + num39;
			array3[6] = num30 + num36 - num38;
			array3[7] = num31 + num37 - num39;
			s_rectReferenceFace[0] = indexedVector5[num28];
			s_rectReferenceFace[1] = indexedVector5[num29];
			float[] array4 = s_ret;
			int num40 = IntersectRectQuad2(s_rectReferenceFace, array3, array4);
			if (num40 < 1)
			{
				return 0;
			}
			float[] array5 = s_point;
			float[] array6 = s_dep;
			float num41 = 1f / (num32 * num35 - num33 * num34);
			num32 *= num41;
			num33 *= num41;
			num34 *= num41;
			num35 *= num41;
			int num42 = 0;
			for (int num43 = 0; num43 < num40; num43++)
			{
				float num44 = num35 * (array4[num43 * 2] - num30) - num33 * (array4[num43 * 2 + 1] - num31);
				float num45 = (0f - num34) * (array4[num43 * 2] - num30) + num32 * (array4[num43 * 2 + 1] - num31);
				for (int num46 = 0; num46 < 3; num46++)
				{
					array5[num42 * 3 + num46] = a[num46] + num44 * array2[num46 * 4 + num24] + num45 * array2[num46 * 4 + num26];
				}
				array6[num42] = indexedVector5[num27] - DDOT(ref C2, 0, array5, num42 * 3);
				if (array6[num42] >= 0f)
				{
					array4[num42 * 2] = array4[num43 * 2];
					array4[num42 * 2 + 1] = array4[num43 * 2 + 1];
					num42++;
				}
			}
			if (num42 < 1)
			{
				return 0;
			}
			if (maxc > num42)
			{
				maxc = num42;
			}
			if (maxc < 1)
			{
				maxc = 1;
			}
			if (num42 <= maxc)
			{
				if (code < 4)
				{
					for (int num47 = 0; num47 < num42; num47++)
					{
						IndexedVector3 pointInWorld = default(IndexedVector3);
						for (int num48 = 0; num48 < 3; num48++)
						{
							pointInWorld[num48] = array5[num47 * 3 + num48] + indexedVector3[num48];
						}
						output.AddContactPoint(-normal, pointInWorld, 0f - array6[num47]);
					}
				}
				else
				{
					for (int num49 = 0; num49 < num42; num49++)
					{
						IndexedVector3 pointInWorld2 = default(IndexedVector3);
						for (int num50 = 0; num50 < 3; num50++)
						{
							pointInWorld2[num50] = array5[num49 * 3 + num50] + indexedVector3[num50];
						}
						output.AddContactPoint(-normal, pointInWorld2, 0f - array6[num49]);
					}
				}
			}
			else
			{
				int i2 = 0;
				float num51 = array6[0];
				for (int num52 = 1; num52 < num42; num52++)
				{
					if (array6[num52] > num51)
					{
						num51 = array6[num52];
						i2 = num52;
					}
				}
				int[] array7 = new int[8];
				CullPoints2(num42, array4, maxc, i2, array7);
				for (int num53 = 0; num53 < maxc; num53++)
				{
					IndexedVector3 indexedVector7 = default(IndexedVector3);
					for (int num54 = 0; num54 < 3; num54++)
					{
						indexedVector7[num54] = array5[array7[num53] * 3 + num54] + indexedVector3[num54];
					}
					IndexedVector3 pointInWorld3 = indexedVector7;
					output.AddContactPoint(-normal, pointInWorld3, 0f - array6[array7[num53]]);
				}
				num42 = maxc;
			}
			return_code = code;
			return num42;
		}

		private static void DLineClosestApproach(ref IndexedVector3 pa, ref IndexedVector3 ua, ref IndexedVector3 pb, ref IndexedVector3 ub, out float alpha, out float beta)
		{
			IndexedVector3 v = pb - pa;
			float num = ua.Dot(ref ub);
			float num2 = ua.Dot(ref v);
			float num3 = 0f - ub.Dot(ref v);
			float num4 = 1f - num * num;
			if (num4 <= 0.0001f)
			{
				alpha = 0f;
				beta = 0f;
			}
			else
			{
				num4 = 1f / num4;
				alpha = (num2 + num * num3) * num4;
				beta = (num * num2 + num3) * num4;
			}
		}

		private static int IntersectRectQuad2(float[] h, float[] p, float[] ret)
		{
			int num = 4;
			int num2 = 0;
			float[] array = s_quadBuffer;
			float[] array2 = p;
			float[] array3 = ret;
			for (int i = 0; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j += 2)
				{
					float[] array4 = array2;
					float[] array5 = array3;
					int k = 0;
					int num3 = 0;
					num2 = 0;
					for (int num4 = num; num4 > 0; k += 2, num4--)
					{
						if ((float)j * array4[k + i] < h[i])
						{
							array5[num3] = array4[k];
							array5[num3 + 1] = array4[k + 1];
							num3 += 2;
							num2++;
							if (((uint)num2 & 8u) != 0)
							{
								goto IL_006f;
							}
						}
						float num5 = 0f;
						float num6 = 0f;
						if (num4 > 1)
						{
							num5 = array4[k + 2 + i];
							num6 = array4[k + 2 + (1 - i)];
						}
						else
						{
							num5 = array2[i];
							num6 = array2[1 - i];
						}
						if (!(((float)j * array4[k + i] < h[i]) ^ ((float)j * num5 < h[i])))
						{
							continue;
						}
						array5[num3 + (1 - i)] = array4[k + (1 - i)] + (num6 - array4[k + (1 - i)]) / (num5 - array4[k + i]) * ((float)j * h[i] - array4[k + i]);
						array5[num3 + i] = (float)j * h[i];
						num3 += 2;
						num2++;
						if ((num2 & 8) == 0)
						{
							continue;
						}
						goto IL_0134;
					}
					array2 = array3;
					array3 = ((array2 == ret) ? array : ret);
					num = num2;
				}
				continue;
				IL_0134:
				array2 = array3;
				break;
				IL_006f:
				array2 = array3;
				break;
			}
			if (array2 != ret)
			{
				for (int l = 0; l < num2 * 2; l++)
				{
					ret[l] = array2[l];
				}
			}
			return num2;
		}

		private static void CullPoints2(int n, float[] p, int m, int i0, int[] iret)
		{
			int num = 0;
			float num3;
			float num4;
			switch (n)
			{
			case 1:
				num3 = p[0];
				num4 = p[1];
				break;
			case 2:
				num3 = 0.5f * (p[0] + p[2]);
				num4 = 0.5f * (p[1] + p[3]);
				break;
			default:
			{
				float num2 = 0f;
				num3 = 0f;
				num4 = 0f;
				float num5;
				for (int j = 0; j < n - 1; j++)
				{
					num5 = p[j * 2] * p[j * 2 + 3] - p[j * 2 + 2] * p[j * 2 + 1];
					num2 += num5;
					num3 += num5 * (p[j * 2] + p[j * 2 + 2]);
					num4 += num5 * (p[j * 2 + 1] + p[j * 2 + 3]);
				}
				num5 = p[n * 2 - 2] * p[1] - p[0] * p[n * 2 - 1];
				num2 = ((!(Math.Abs(num2 + num5) > 1.1920929E-07f)) ? 1E+30f : (1f / (3f * (num2 + num5))));
				num3 = num2 * (num3 + num5 * (p[n * 2 - 2] + p[0]));
				num4 = num2 * (num4 + num5 * (p[n * 2 - 1] + p[1]));
				break;
			}
			}
			float[] array = s_A;
			for (int j = 0; j < n; j++)
			{
				array[j] = (float)Math.Atan2(p[j * 2 + 1] - num4, p[j * 2] - num3);
			}
			for (int j = 0; j < n; j++)
			{
				s_availablePoints[j] = 1;
			}
			s_availablePoints[i0] = 0;
			iret[0] = i0;
			num++;
			for (int k = 1; k < m; k++)
			{
				float num2 = (float)k * ((float)Math.PI * 2f / (float)m) + array[i0];
				if (num2 > (float)Math.PI)
				{
					num2 -= (float)Math.PI * 2f;
				}
				float num6 = float.MaxValue;
				iret[num] = i0;
				for (int j = 0; j < n; j++)
				{
					if (s_availablePoints[j] != 0)
					{
						float num7 = Math.Abs(array[j] - num2);
						if (num7 > (float)Math.PI)
						{
							num7 = (float)Math.PI * 2f - num7;
						}
						if (num7 < num6)
						{
							num6 = num7;
							iret[num] = j;
						}
					}
				}
				s_availablePoints[iret[num]] = 0;
				num++;
			}
		}

		private static bool TST(float expr1, float expr2, float[] norm, ref float[] normalR, int offset, ref int normalROffset, int cc, ref int code, ref float s, ref bool invert_normal)
		{
			float num = Math.Abs(expr1) - expr2;
			if (num > 0f)
			{
				return true;
			}
			if (num > s)
			{
				s = num;
				normalR = norm;
				normalROffset = offset;
				invert_normal = expr1 < 0f;
				code = cc;
			}
			return false;
		}

		private static bool TST2(float expr1, float expr2, float n1, float n2, float n3, ref IndexedVector3 normalC, ref float[] normalR, int cc, ref int code, ref float s, ref bool invert_normal)
		{
			float num = Math.Abs(expr1) - expr2;
			if (num > 1.1920929E-07f)
			{
				return true;
			}
			float num2 = (float)Math.Sqrt(n1 * n1 + n2 * n2 + n3 * n3);
			if (num2 > 1.1920929E-07f)
			{
				num /= num2;
				if (num * fudge_factor > s)
				{
					s = num;
					normalR = null;
					normalC.X = n1 / num2;
					normalC.Y = n2 / num2;
					normalC.Z = n3 / num2;
					invert_normal = expr1 < 0f;
					invert_normal = expr1 < 0f;
					code = cc;
				}
			}
			return false;
		}

		private static float DDOT(ref IndexedVector3 a, int aOffset, ref IndexedVector3 b, int bOffset)
		{
			return a[aOffset] * b[bOffset] + a[1 + aOffset] * b[1 + bOffset] + a[2 + aOffset] * b[2 + bOffset];
		}

		private static float DDOT(ref IndexedVector3 a, int aOffset, float[] b, int bOffset)
		{
			return a[aOffset] * b[bOffset] + a[1 + aOffset] * b[1 + bOffset] + a[2 + aOffset] * b[2 + bOffset];
		}

		private static float DDOT(float[] a, int aOffset, ref IndexedVector3 b, int bOffset)
		{
			return a[aOffset] * b[bOffset] + a[1 + aOffset] * b[1 + bOffset] + a[2 + aOffset] * b[2 + bOffset];
		}

		private static float DDOT44(float[] a, int aOffset, float[] b, int bOffset)
		{
			return a[aOffset] * b[bOffset] + a[4 + aOffset] * b[4 + bOffset] + a[8 + aOffset] * b[8 + bOffset];
		}

		private static float DDOT41(float[] a, int aOffset, float[] b, int bOffset)
		{
			return a[aOffset] * b[bOffset] + a[4 + aOffset] * b[1 + bOffset] + a[8 + aOffset] * b[2 + bOffset];
		}

		private static float DDOT41(float[] a, int aOffset, ref IndexedVector3 b, int bOffset)
		{
			return a[aOffset] * b[bOffset] + a[4 + aOffset] * b[1 + bOffset] + a[8 + aOffset] * b[2 + bOffset];
		}

		private static float DDOT14(float[] a, int aOffset, float[] b, int bOffset)
		{
			return a[aOffset] * b[bOffset] + a[1 + aOffset] * b[4 + bOffset] + a[2 + aOffset] * b[8 + bOffset];
		}

		private static float DDOT14(ref IndexedVector3 a, int aOffset, float[] b, int bOffset)
		{
			return a[aOffset] * b[bOffset] + a[1 + aOffset] * b[4 + bOffset] + a[2 + aOffset] * b[8 + bOffset];
		}

		public static void DMULTIPLY1_331(ref IndexedVector3 A, float[] B, ref IndexedVector3 C)
		{
			A.X = DDOT41(B, 0, ref C, 0);
			A.Y = DDOT41(B, 1, ref C, 0);
			A.Z = DDOT41(B, 2, ref C, 0);
		}

		public static void DMULTIPLY0_331(ref IndexedVector3 A, float[] B, ref IndexedVector3 C)
		{
			A.X = DDOT(B, 0, ref C, 0);
			A.Y = DDOT(B, 4, ref C, 0);
			A.Z = DDOT(B, 8, ref C, 0);
		}
	}
}
