using System;

namespace BulletXNA.LinearMath
{
	public abstract class DebugDraw : IDebugDraw
	{
		public abstract void DrawLine(IndexedVector3 from, IndexedVector3 to, IndexedVector3 color);

		public abstract void DrawLine(ref IndexedVector3 from, ref IndexedVector3 to, ref IndexedVector3 color);

		public abstract void Draw3dText(ref IndexedVector3 location, string textString);

		public abstract void DrawContactPoint(IndexedVector3 pointOnB, IndexedVector3 normalOnB, float distance, int lifeTime, IndexedVector3 color);

		public abstract void DrawContactPoint(ref IndexedVector3 pointOnB, ref IndexedVector3 normalOnB, float distance, int lifeTime, ref IndexedVector3 color);

		public abstract void ReportErrorWarning(string warningString);

		public abstract void SetDebugMode(DebugDrawModes debugMode);

		public abstract DebugDrawModes GetDebugMode();

		public virtual void DrawLine(ref IndexedVector3 from, ref IndexedVector3 to, ref IndexedVector3 fromColor, ref IndexedVector3 toColor)
		{
			DrawLine(ref from, ref to, ref fromColor);
		}

		public virtual void DrawBox(ref IndexedVector3 bbMin, ref IndexedVector3 bbMax, ref IndexedVector3 color)
		{
			DrawLine(bbMin, new IndexedVector3(bbMax.X, bbMin.Y, bbMin.Z), color);
			DrawLine(new IndexedVector3(bbMax.X, bbMin.Y, bbMin.Z), new IndexedVector3(bbMax.X, bbMax.Y, bbMin.Z), color);
			DrawLine(new IndexedVector3(bbMax.X, bbMax.Y, bbMin.Z), new IndexedVector3(bbMin.X, bbMax.Y, bbMin.Z), color);
			DrawLine(new IndexedVector3(bbMin.X, bbMax.Y, bbMin.Z), bbMin, color);
			DrawLine(bbMin, new IndexedVector3(bbMin.X, bbMin.Y, bbMax.Z), color);
			DrawLine(new IndexedVector3(bbMax.X, bbMin.Y, bbMin.Z), new IndexedVector3(bbMax.X, bbMin.Y, bbMax.Z), color);
			DrawLine(new IndexedVector3(bbMax.X, bbMax.Y, bbMin.Z), bbMax, color);
			DrawLine(new IndexedVector3(bbMin.X, bbMax.Y, bbMin.Z), new IndexedVector3(bbMin.X, bbMax.Y, bbMax.Z), color);
			DrawLine(new IndexedVector3(bbMin.X, bbMin.Y, bbMax.Z), new IndexedVector3(bbMax.X, bbMin.Y, bbMax.Z), color);
			DrawLine(new IndexedVector3(bbMax.X, bbMin.Y, bbMax.Z), bbMax, color);
			DrawLine(bbMax, new IndexedVector3(bbMin.X, bbMax.Y, bbMax.Z), color);
			DrawLine(new IndexedVector3(bbMin.X, bbMax.Y, bbMax.Z), new IndexedVector3(bbMin.X, bbMin.Y, bbMax.Z), color);
		}

		public virtual void DrawBox(ref IndexedVector3 bbMin, ref IndexedVector3 bbMax, ref IndexedMatrix trans, ref IndexedVector3 color)
		{
			DrawLine(trans * bbMin, trans * new IndexedVector3(bbMax.X, bbMin.Y, bbMin.Z), color);
			DrawLine(trans * new IndexedVector3(bbMax.X, bbMin.Y, bbMin.Z), trans * new IndexedVector3(bbMax.X, bbMax.Y, bbMin.Z), color);
			DrawLine(trans * new IndexedVector3(bbMax.X, bbMax.Y, bbMin.Z), trans * new IndexedVector3(bbMin.X, bbMax.Y, bbMin.Z), color);
			DrawLine(trans * new IndexedVector3(bbMin.X, bbMax.Y, bbMin.Z), trans * bbMin, color);
			DrawLine(trans * bbMin, trans * new IndexedVector3(bbMin.X, bbMin.Y, bbMax.Z), color);
			DrawLine(trans * new IndexedVector3(bbMax.X, bbMin.Y, bbMin.Z), trans * new IndexedVector3(bbMax.X, bbMin.Y, bbMax.Z), color);
			DrawLine(trans * new IndexedVector3(bbMax.X, bbMax.Y, bbMin.Z), trans * bbMax, color);
			DrawLine(trans * new IndexedVector3(bbMin.X, bbMax.Y, bbMin.Z), trans * new IndexedVector3(bbMin.X, bbMax.Y, bbMax.Z), color);
			DrawLine(trans * new IndexedVector3(bbMin.X, bbMin.Y, bbMax.Z), trans * new IndexedVector3(bbMax.X, bbMin.Y, bbMax.Z), color);
			DrawLine(trans * new IndexedVector3(bbMax.X, bbMin.Y, bbMax.Z), trans * bbMax, color);
			DrawLine(trans * bbMax, trans * new IndexedVector3(bbMin.X, bbMax.Y, bbMax.Z), color);
			DrawLine(trans * new IndexedVector3(bbMin.X, bbMax.Y, bbMax.Z), trans * new IndexedVector3(bbMin.X, bbMin.Y, bbMax.Z), color);
		}

		public virtual void DrawSphere(float radius, ref IndexedMatrix transform, ref IndexedVector3 color)
		{
			IndexedVector3 origin = transform._origin;
			IndexedVector3 indexedVector = transform._basis * new IndexedVector3(radius, 0f, 0f);
			IndexedVector3 indexedVector2 = transform._basis * new IndexedVector3(0f, radius, 0f);
			IndexedVector3 indexedVector3 = transform._basis * new IndexedVector3(0f, 0f, radius);
			DrawLine(origin - indexedVector, origin + indexedVector2, color);
			DrawLine(origin + indexedVector2, origin + indexedVector, color);
			DrawLine(origin + indexedVector, origin - indexedVector2, color);
			DrawLine(origin - indexedVector2, origin - indexedVector, color);
			DrawLine(origin - indexedVector, origin + indexedVector3, color);
			DrawLine(origin + indexedVector3, origin + indexedVector, color);
			DrawLine(origin + indexedVector, origin - indexedVector3, color);
			DrawLine(origin - indexedVector3, origin - indexedVector, color);
			DrawLine(origin - indexedVector2, origin + indexedVector3, color);
			DrawLine(origin + indexedVector3, origin + indexedVector2, color);
			DrawLine(origin + indexedVector2, origin - indexedVector3, color);
			DrawLine(origin - indexedVector3, origin - indexedVector2, color);
		}

		public virtual void DrawSphere(IndexedVector3 p, float radius, IndexedVector3 color)
		{
			IndexedMatrix transform = IndexedMatrix.CreateTranslation(p);
			DrawSphere(radius, ref transform, ref color);
		}

		public virtual void DrawSphere(ref IndexedVector3 p, float radius, ref IndexedVector3 color)
		{
			IndexedMatrix transform = IndexedMatrix.CreateTranslation(p);
			DrawSphere(radius, ref transform, ref color);
		}

		public virtual void DrawTriangle(ref IndexedVector3 v0, ref IndexedVector3 v1, ref IndexedVector3 v2, ref IndexedVector3 n0, ref IndexedVector3 n1, ref IndexedVector3 n2, ref IndexedVector3 color, float alpha)
		{
			DrawTriangle(ref v0, ref v1, ref v2, ref color, alpha);
		}

		public virtual void DrawTriangle(ref IndexedVector3 v0, ref IndexedVector3 v1, ref IndexedVector3 v2, ref IndexedVector3 color, float alpha)
		{
			DrawLine(ref v0, ref v1, ref color);
			DrawLine(ref v1, ref v2, ref color);
			DrawLine(ref v2, ref v0, ref color);
		}

		public virtual void DrawAabb(IndexedVector3 from, IndexedVector3 to, IndexedVector3 color)
		{
			DrawAabb(ref from, ref to, ref color);
		}

		public virtual void DrawAabb(ref IndexedVector3 from, ref IndexedVector3 to, ref IndexedVector3 color)
		{
			IndexedVector3 indexedVector = (to - from) * 0.5f;
			IndexedVector3 indexedVector2 = (to + from) * 0.5f;
			IndexedVector3 indexedVector3 = new IndexedVector3(1f, 1f, 1f);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					IndexedVector3 from2 = new IndexedVector3(indexedVector3.X * indexedVector.X, indexedVector3.Y * indexedVector.Y, indexedVector3.Z * indexedVector.Z);
					from2 += indexedVector2;
					indexedVector3[j % 3] *= -1f;
					IndexedVector3 to2 = new IndexedVector3(indexedVector3.X * indexedVector.X, indexedVector3.Y * indexedVector.Y, indexedVector3.Z * indexedVector.Z);
					to2 += indexedVector2;
					DrawLine(from2, to2, color);
				}
				indexedVector3 = new IndexedVector3(-1f, -1f, -1f);
				if (i < 3)
				{
					indexedVector3[i] *= -1f;
				}
			}
		}

		public virtual void DrawTransform(ref IndexedMatrix transform, float orthoLen)
		{
			IndexedVector3 from = transform._origin;
			IndexedVector3 to = from + transform._basis * new IndexedVector3(orthoLen, 0f, 0f);
			IndexedVector3 color = new IndexedVector3(0.7f, 0f, 0f);
			DrawLine(ref from, ref to, ref color);
			to = from + transform._basis * new IndexedVector3(0f, orthoLen, 0f);
			color = new IndexedVector3(0f, 0.7f, 0f);
			DrawLine(ref from, ref to, ref color);
			to = from + transform._basis * new IndexedVector3(0f, 0f, orthoLen);
			color = new IndexedVector3(0f, 0f, 0.7f);
			DrawLine(ref from, ref to, ref color);
		}

		public virtual void DrawArc(ref IndexedVector3 center, ref IndexedVector3 normal, ref IndexedVector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, ref IndexedVector3 color, bool drawSect)
		{
			DrawArc(ref center, ref normal, ref axis, radiusA, radiusB, minAngle, maxAngle, ref color, drawSect, 10f);
		}

		public virtual void DrawArc(ref IndexedVector3 center, ref IndexedVector3 normal, ref IndexedVector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, ref IndexedVector3 color, bool drawSect, float stepDegrees)
		{
			IndexedVector3 indexedVector = axis;
			IndexedVector3 indexedVector2 = IndexedVector3.Cross(normal, axis);
			float num = stepDegrees * ((float)Math.PI / 180f);
			int num2 = (int)((maxAngle - minAngle) / num);
			if (num2 == 0)
			{
				num2 = 1;
			}
			IndexedVector3 to = center + radiusA * indexedVector * (float)Math.Cos(minAngle) + radiusB * indexedVector2 * (float)Math.Sin(minAngle);
			if (drawSect)
			{
				DrawLine(ref center, ref to, ref color);
			}
			for (int i = 1; i <= num2; i++)
			{
				float num3 = minAngle + (maxAngle - minAngle) * (float)i / (float)num2;
				IndexedVector3 to2 = center + radiusA * indexedVector * (float)Math.Cos(num3) + radiusB * indexedVector2 * (float)Math.Sin(num3);
				DrawLine(ref to, ref to2, ref color);
				to = to2;
			}
			if (drawSect)
			{
				DrawLine(ref center, ref to, ref color);
			}
		}

		public virtual void DrawSpherePatch(ref IndexedVector3 center, ref IndexedVector3 up, ref IndexedVector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, ref IndexedVector3 color)
		{
			DrawSpherePatch(ref center, ref up, ref axis, radius, minTh, maxTh, minPs, maxPs, ref color, 10f);
		}

		public virtual void DrawSpherePatch(ref IndexedVector3 center, ref IndexedVector3 up, ref IndexedVector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, ref IndexedVector3 color, float stepDegrees)
		{
			IndexedVector3 from = center + up * radius;
			IndexedVector3 from2 = center - up * radius;
			IndexedVector3 from3 = IndexedVector3.Zero;
			float num = stepDegrees * ((float)Math.PI / 180f);
			IndexedVector3 indexedVector = up;
			IndexedVector3 indexedVector2 = axis;
			IndexedVector3 indexedVector3 = IndexedVector3.Cross(indexedVector, indexedVector2);
			bool flag = false;
			bool flag2 = false;
			if (minTh <= -(float)Math.PI / 2f)
			{
				minTh = -(float)Math.PI / 2f + num;
				flag = true;
			}
			if (maxTh >= (float)Math.PI / 2f)
			{
				maxTh = (float)Math.PI / 2f - num;
				flag2 = true;
			}
			if (minTh > maxTh)
			{
				minTh = -(float)Math.PI / 2f + num;
				maxTh = (float)Math.PI / 2f - num;
				flag = (flag2 = true);
			}
			int num2 = (int)((maxTh - minTh) / num) + 1;
			if (num2 < 2)
			{
				num2 = 2;
			}
			float num3 = (maxTh - minTh) / (float)(num2 - 1);
			bool flag3 = false;
			if (!(minPs > maxPs))
			{
				flag3 = maxPs - minPs >= (float)Math.PI * 2f;
			}
			else
			{
				minPs = -(float)Math.PI + num;
				maxPs = (float)Math.PI;
				flag3 = true;
			}
			int num4 = (int)((maxPs - minPs) / num) + 1;
			if (num4 < 2)
			{
				num4 = 2;
			}
			IndexedVector3[] array = new IndexedVector3[num4];
			IndexedVector3[] array2 = new IndexedVector3[num4];
			IndexedVector3[] array3 = array;
			IndexedVector3[] array4 = array2;
			float num5 = (maxPs - minPs) / (float)(num4 - 1);
			for (int i = 0; i < num2; i++)
			{
				float num6 = minTh + (float)i * num3;
				float num7 = radius * (float)Math.Sin(num6);
				float num8 = radius * (float)Math.Cos(num6);
				for (int j = 0; j < num4; j++)
				{
					float num9 = minPs + (float)j * num5;
					float num10 = (float)Math.Sin(num9);
					float num11 = (float)Math.Cos(num9);
					array4[j] = center + num8 * num11 * indexedVector2 + num8 * num10 * indexedVector3 + num7 * indexedVector;
					if (i != 0)
					{
						DrawLine(array3[j], array4[j], color);
					}
					else if (flag2)
					{
						DrawLine(from2, array4[j], color);
					}
					if (j != 0)
					{
						DrawLine(array4[j - 1], array4[j], color);
					}
					else
					{
						from3 = array4[j];
					}
					if (i == num2 - 1 && flag)
					{
						DrawLine(from, array4[j], color);
					}
					if (flag3)
					{
						if (j == num4 - 1)
						{
							DrawLine(from3, array4[j], color);
						}
					}
					else if ((i == 0 || i == num2 - 1) && (j == 0 || j == num4 - 1))
					{
						DrawLine(center, array4[j], color);
					}
				}
				IndexedVector3[] array5 = array3;
				array3 = array4;
				array4 = array5;
			}
		}

		public virtual void DrawCapsule(float radius, float halfHeight, int upAxis, ref IndexedMatrix transform, ref IndexedVector3 color)
		{
			IndexedVector3 zero = IndexedVector3.Zero;
			zero[upAxis] = 0f - halfHeight;
			IndexedVector3 zero2 = IndexedVector3.Zero;
			zero2[upAxis] = halfHeight;
			IndexedMatrix transform2 = transform;
			transform2._origin = transform * zero;
			DrawSphere(radius, ref transform2, ref color);
			IndexedMatrix transform3 = transform;
			transform3._origin = transform * zero2;
			DrawSphere(radius, ref transform3, ref color);
			IndexedVector3 origin = transform._origin;
			zero[(upAxis + 1) % 3] = radius;
			zero2[(upAxis + 1) % 3] = radius;
			DrawLine(origin + transform._basis * zero, origin + transform._basis * zero2, color);
			zero[(upAxis + 1) % 3] = 0f - radius;
			zero2[(upAxis + 1) % 3] = 0f - radius;
			DrawLine(origin + transform._basis * zero, origin + transform._basis * zero2, color);
			zero[(upAxis + 2) % 3] = radius;
			zero2[(upAxis + 2) % 3] = radius;
			DrawLine(origin + transform._basis * zero, origin + transform._basis * zero2, color);
			zero[(upAxis + 2) % 3] = 0f - radius;
			zero2[(upAxis + 2) % 3] = 0f - radius;
			DrawLine(origin + transform._basis * zero, origin + transform._basis * zero2, color);
		}

		public virtual void DrawCylinder(float radius, float halfHeight, int upAxis, ref IndexedMatrix transform, ref IndexedVector3 color)
		{
			IndexedVector3 origin = transform._origin;
			IndexedVector3 zero = IndexedVector3.Zero;
			zero[upAxis] = halfHeight;
			IndexedVector3 zero2 = IndexedVector3.Zero;
			zero2[(upAxis + 1) % 3] = radius;
			DrawLine(origin + transform._basis * zero + zero2, origin + transform._basis * -zero + zero2, color);
			DrawLine(origin + transform._basis * zero - zero2, origin + transform._basis * -zero - zero2, color);
		}

		public virtual void DrawCone(float radius, float height, int upAxis, ref IndexedMatrix transform, ref IndexedVector3 color)
		{
			IndexedVector3 origin = transform._origin;
			IndexedVector3 zero = IndexedVector3.Zero;
			zero[upAxis] = height * 0.5f;
			IndexedVector3 zero2 = IndexedVector3.Zero;
			zero2[(upAxis + 1) % 3] = radius;
			IndexedVector3 zero3 = IndexedVector3.Zero;
			zero2[(upAxis + 2) % 3] = radius;
			DrawLine(origin + transform._basis * zero, origin + transform._basis * -zero + zero2, color);
			DrawLine(origin + transform._basis * zero, origin + transform._basis * -zero - zero2, color);
			DrawLine(origin + transform._basis * zero, origin + transform._basis * -zero + zero3, color);
			DrawLine(origin + transform._basis * zero, origin + transform._basis * -zero - zero3, color);
		}

		public virtual void DrawPlane(ref IndexedVector3 planeNormal, float planeConst, ref IndexedMatrix transform, ref IndexedVector3 color)
		{
			IndexedVector3 indexedVector = planeNormal * planeConst;
			IndexedVector3 p;
			IndexedVector3 q;
			TransformUtil.PlaneSpace1(ref planeNormal, out p, out q);
			float num = 100f;
			IndexedVector3 indexedVector2 = indexedVector + p * num;
			IndexedVector3 indexedVector3 = indexedVector - p * num;
			IndexedVector3 indexedVector4 = indexedVector + q * num;
			IndexedVector3 indexedVector5 = indexedVector - q * num;
			DrawLine(transform * indexedVector2, transform * indexedVector3, color);
			DrawLine(transform * indexedVector4, transform * indexedVector5, color);
		}
	}
}
