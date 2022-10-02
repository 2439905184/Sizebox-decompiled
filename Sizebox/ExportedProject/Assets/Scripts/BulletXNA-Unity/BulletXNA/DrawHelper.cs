using System;
using BulletXNA.BulletDynamics;
using BulletXNA.LinearMath;
using UnityEngine;

namespace BulletXNA
{
	public static class DrawHelper
	{
		public static short[] s_cubeIndices = new short[36]
		{
			0, 1, 2, 2, 3, 0, 0, 1, 5, 5,
			4, 0, 1, 2, 6, 6, 5, 1, 2, 6,
			7, 7, 3, 2, 3, 7, 4, 4, 0, 3,
			4, 5, 6, 6, 7, 4
		};

		public static void DebugDrawConstraint(TypedConstraint constraint, IDebugDraw debugDraw)
		{
			bool flag = (debugDraw.GetDebugMode() & DebugDrawModes.DBG_DrawConstraints) != 0;
			bool flag2 = (debugDraw.GetDebugMode() & DebugDrawModes.DBG_DrawConstraintLimits) != 0;
			float dbgDrawSize = constraint.GetDbgDrawSize();
			if (dbgDrawSize <= 0f)
			{
				return;
			}
			switch (constraint.GetConstraintType())
			{
			case TypedConstraintType.POINT2POINT_CONSTRAINT_TYPE:
			{
				Point2PointConstraint point2PointConstraint = constraint as Point2PointConstraint;
				IndexedMatrix transform3 = IndexedMatrix.Identity;
				IndexedVector3 pivotInA = point2PointConstraint.GetPivotInA();
				pivotInA = point2PointConstraint.GetRigidBodyA().GetCenterOfMassTransform() * pivotInA;
				transform3._origin = pivotInA;
				debugDraw.DrawTransform(ref transform3, dbgDrawSize);
				pivotInA = point2PointConstraint.GetPivotInB();
				pivotInA = point2PointConstraint.GetRigidBodyB().GetCenterOfMassTransform() * pivotInA;
				transform3._origin = pivotInA;
				if (flag)
				{
					debugDraw.DrawTransform(ref transform3, dbgDrawSize);
				}
				break;
			}
			case TypedConstraintType.HINGE_CONSTRAINT_TYPE:
			{
				HingeConstraint hingeConstraint = constraint as HingeConstraint;
				IndexedMatrix transform5 = hingeConstraint.GetRigidBodyA().GetCenterOfMassTransform() * hingeConstraint.GetAFrame();
				if (flag)
				{
					debugDraw.DrawTransform(ref transform5, dbgDrawSize);
				}
				transform5 = hingeConstraint.GetRigidBodyB().GetCenterOfMassTransform() * hingeConstraint.GetBFrame();
				if (flag)
				{
					debugDraw.DrawTransform(ref transform5, dbgDrawSize);
				}
				float num5 = hingeConstraint.GetLowerLimit();
				float num6 = hingeConstraint.GetUpperLimit();
				if (num5 != num6)
				{
					bool drawSect = true;
					if (num5 > num6)
					{
						num5 = 0f;
						num6 = (float)Math.PI * 2f;
						drawSect = false;
					}
					if (flag2)
					{
						IndexedVector3 center4 = transform5._origin;
						IndexedVector3 normal4 = transform5._basis.GetColumn(2);
						IndexedVector3 axis5 = transform5._basis.GetColumn(0);
						IndexedVector3 color2 = IndexedVector3.Zero;
						debugDraw.DrawArc(ref center4, ref normal4, ref axis5, dbgDrawSize, dbgDrawSize, num5, num6, ref color2, drawSect);
					}
				}
				break;
			}
			case TypedConstraintType.CONETWIST_CONSTRAINT_TYPE:
			{
				ConeTwistConstraint coneTwistConstraint = constraint as ConeTwistConstraint;
				IndexedMatrix transform4 = coneTwistConstraint.GetRigidBodyA().GetCenterOfMassTransform() * coneTwistConstraint.GetAFrame();
				if (flag)
				{
					debugDraw.DrawTransform(ref transform4, dbgDrawSize);
				}
				transform4 = coneTwistConstraint.GetRigidBodyB().GetCenterOfMassTransform() * coneTwistConstraint.GetBFrame();
				if (flag)
				{
					debugDraw.DrawTransform(ref transform4, dbgDrawSize);
				}
				IndexedVector3 fromColor2 = IndexedVector3.Zero;
				if (!flag2)
				{
					break;
				}
				float fLength = dbgDrawSize;
				float fAngleInRadians = 6.086836f;
				IndexedVector3 pointForAngle = coneTwistConstraint.GetPointForAngle(fAngleInRadians, fLength);
				pointForAngle = transform4 * pointForAngle;
				for (int i = 0; i < 32; i++)
				{
					fAngleInRadians = (float)Math.PI * 2f * (float)i / 32f;
					IndexedVector3 pointForAngle2 = coneTwistConstraint.GetPointForAngle(fAngleInRadians, fLength);
					pointForAngle2 = transform4 * pointForAngle2;
					debugDraw.DrawLine(ref pointForAngle, ref pointForAngle2, ref fromColor2);
					if (i % 4 == 0)
					{
						IndexedVector3 from2 = transform4._origin;
						debugDraw.DrawLine(ref from2, ref pointForAngle2, ref fromColor2);
					}
					pointForAngle = pointForAngle2;
				}
				float twistSpan = coneTwistConstraint.GetTwistSpan();
				float twistAngle = coneTwistConstraint.GetTwistAngle();
				transform4 = ((!(coneTwistConstraint.GetRigidBodyB().GetInvMass() > 0f)) ? (coneTwistConstraint.GetRigidBodyA().GetCenterOfMassTransform() * coneTwistConstraint.GetAFrame()) : (coneTwistConstraint.GetRigidBodyB().GetCenterOfMassTransform() * coneTwistConstraint.GetBFrame()));
				IndexedVector3 center3 = transform4._origin;
				IndexedVector3 normal3 = transform4._basis.GetColumn(0);
				IndexedVector3 axis4 = transform4._basis.GetColumn(1);
				debugDraw.DrawArc(ref center3, ref normal3, ref axis4, dbgDrawSize, dbgDrawSize, 0f - twistAngle - twistSpan, 0f - twistAngle + twistSpan, ref fromColor2, true);
				break;
			}
			case TypedConstraintType.D6_CONSTRAINT_TYPE:
			case TypedConstraintType.D6_SPRING_CONSTRAINT_TYPE:
			{
				Generic6DofConstraint generic6DofConstraint = constraint as Generic6DofConstraint;
				IndexedMatrix transform2 = generic6DofConstraint.GetCalculatedTransformA();
				if (flag)
				{
					debugDraw.DrawTransform(ref transform2, dbgDrawSize);
				}
				transform2 = generic6DofConstraint.GetCalculatedTransformB();
				if (flag)
				{
					debugDraw.DrawTransform(ref transform2, dbgDrawSize);
				}
				IndexedVector3 color = IndexedVector3.Zero;
				if (flag2)
				{
					transform2 = generic6DofConstraint.GetCalculatedTransformA();
					IndexedVector3 center2 = generic6DofConstraint.GetCalculatedTransformB()._origin;
					IndexedVector3 up = transform2._basis.GetColumn(1);
					IndexedVector3 axis2 = transform2._basis.GetColumn(0);
					float loLimit = generic6DofConstraint.GetRotationalLimitMotor(1).m_loLimit;
					float hiLimit = generic6DofConstraint.GetRotationalLimitMotor(1).m_hiLimit;
					float loLimit2 = generic6DofConstraint.GetRotationalLimitMotor(2).m_loLimit;
					float hiLimit2 = generic6DofConstraint.GetRotationalLimitMotor(2).m_hiLimit;
					debugDraw.DrawSpherePatch(ref center2, ref up, ref axis2, dbgDrawSize * 0.9f, loLimit, hiLimit, loLimit2, hiLimit2, ref color);
					axis2 = transform2._basis.GetColumn(1);
					float angle = generic6DofConstraint.GetAngle(1);
					float angle2 = generic6DofConstraint.GetAngle(2);
					float num = (float)Math.Cos(angle);
					float num2 = (float)Math.Sin(angle);
					float num3 = (float)Math.Cos(angle2);
					float num4 = (float)Math.Sin(angle2);
					IndexedVector3 axis3 = new IndexedVector3(num * num3 * axis2.X + num * num4 * axis2.Y - num2 * axis2.Z, (0f - num4) * axis2.X + num3 * axis2.Y, num3 * num2 * axis2.X + num4 * num2 * axis2.Y + num * axis2.Z);
					IndexedVector3 normal2 = -generic6DofConstraint.GetCalculatedTransformB()._basis.GetColumn(0);
					float loLimit3 = generic6DofConstraint.GetRotationalLimitMotor(0).m_loLimit;
					float hiLimit3 = generic6DofConstraint.GetRotationalLimitMotor(0).m_hiLimit;
					if (loLimit3 > hiLimit3)
					{
						debugDraw.DrawArc(ref center2, ref normal2, ref axis3, dbgDrawSize, dbgDrawSize, -(float)Math.PI, (float)Math.PI, ref color, false);
					}
					else if (loLimit3 < hiLimit3)
					{
						debugDraw.DrawArc(ref center2, ref normal2, ref axis3, dbgDrawSize, dbgDrawSize, loLimit3, hiLimit3, ref color, false);
					}
					transform2 = generic6DofConstraint.GetCalculatedTransformA();
					IndexedVector3 bbMin = generic6DofConstraint.GetTranslationalLimitMotor().m_lowerLimit;
					IndexedVector3 bbMax = generic6DofConstraint.GetTranslationalLimitMotor().m_upperLimit;
					debugDraw.DrawBox(ref bbMin, ref bbMax, ref transform2, ref color);
				}
				break;
			}
			case TypedConstraintType.SLIDER_CONSTRAINT_TYPE:
			{
				SliderConstraint sliderConstraint = constraint as SliderConstraint;
				IndexedMatrix transform = sliderConstraint.GetCalculatedTransformA();
				if (flag)
				{
					debugDraw.DrawTransform(ref transform, dbgDrawSize);
				}
				transform = sliderConstraint.GetCalculatedTransformB();
				if (flag)
				{
					debugDraw.DrawTransform(ref transform, dbgDrawSize);
				}
				IndexedVector3 fromColor = IndexedVector3.Zero;
				if (flag2)
				{
					IndexedMatrix calculatedTransformA = sliderConstraint.GetCalculatedTransformA();
					IndexedVector3 from = calculatedTransformA * new IndexedVector3(sliderConstraint.GetLowerLinLimit(), 0f, 0f);
					IndexedVector3 to = calculatedTransformA * new IndexedVector3(sliderConstraint.GetUpperLinLimit(), 0f, 0f);
					debugDraw.DrawLine(ref from, ref to, ref fromColor);
					IndexedVector3 normal = transform._basis.GetColumn(0);
					IndexedVector3 axis = transform._basis.GetColumn(1);
					float lowerAngLimit = sliderConstraint.GetLowerAngLimit();
					float upperAngLimit = sliderConstraint.GetUpperAngLimit();
					IndexedVector3 center = sliderConstraint.GetCalculatedTransformB()._origin;
					debugDraw.DrawArc(ref center, ref normal, ref axis, dbgDrawSize, dbgDrawSize, lowerAngLimit, upperAngLimit, ref fromColor, true);
				}
				break;
			}
			case TypedConstraintType.CONTACT_CONSTRAINT_TYPE:
				break;
			}
		}

		public static ShapeData CreateCube()
		{
			IndexedMatrix transform = IndexedMatrix.Identity;
			return CreateBox(IndexedVector3.Zero, new IndexedVector3(1f), Color.yellow, ref transform);
		}

		public static ShapeData CreateBox(IndexedVector3 position, IndexedVector3 sideLength, Color color, ref IndexedMatrix transform)
		{
			ShapeData shapeData = new ShapeData(8, 36);
			int num = 0;
			shapeData.m_verticesArray[num++] = new VertexPositionColor((transform * (position + new IndexedVector3(0f, 0f, 0f))).ToVector3(), color);
			shapeData.m_verticesArray[num++] = new VertexPositionColor((transform * (position + new IndexedVector3(sideLength.X, 0f, 0f))).ToVector3(), color);
			shapeData.m_verticesArray[num++] = new VertexPositionColor((transform * (position + new IndexedVector3(sideLength.X, 0f, sideLength.Z))).ToVector3(), color);
			shapeData.m_verticesArray[num++] = new VertexPositionColor((transform * (position + new IndexedVector3(0f, 0f, sideLength.Z))).ToVector3(), color);
			shapeData.m_verticesArray[num++] = new VertexPositionColor((transform * (position + new IndexedVector3(0f, sideLength.Y, 0f))).ToVector3(), color);
			shapeData.m_verticesArray[num++] = new VertexPositionColor((transform * (position + new IndexedVector3(sideLength.X, sideLength.Y, 0f))).ToVector3(), color);
			shapeData.m_verticesArray[num++] = new VertexPositionColor((transform * (position + new IndexedVector3(sideLength.X, sideLength.Y, sideLength.Z))).ToVector3(), color);
			shapeData.m_verticesArray[num++] = new VertexPositionColor((transform * (position + new IndexedVector3(0f, sideLength.Y, sideLength.Z))).ToVector3(), color);
			shapeData.m_indexArray = s_cubeIndices;
			return shapeData;
		}

		public static ShapeData CreateSphere(int slices, int stacks, float radius, Color color)
		{
			ShapeData shapeData = new ShapeData((slices + 1) * (stacks + 1), slices * stacks * 6);
			float num = 0f;
			float num2 = 0f;
			float num3 = (float)Math.PI / (float)stacks;
			float num4 = (float)Math.PI * 2f / (float)slices;
			short num5 = 0;
			for (int i = 0; i <= stacks; i++)
			{
				num = (float)Math.PI / 2f - (float)i * num3;
				float y = radius * (float)Math.Sin(num);
				float num6 = (0f - radius) * (float)Math.Cos(num);
				for (int j = 0; j <= slices; j++)
				{
					num2 = (float)j * num4;
					float x = num6 * (float)Math.Sin(num2);
					float z = num6 * (float)Math.Cos(num2);
					shapeData.m_verticesArray[num5++] = new VertexPositionColor(new Vector3(x, y, z), color);
				}
			}
			int num7 = slices + 1;
			num5 = 0;
			for (int k = 0; k < stacks; k++)
			{
				for (int l = 0; l < slices; l++)
				{
					shapeData.m_indexList[num5++] = (short)(k * num7 + l);
					shapeData.m_indexList[num5++] = (short)((k + 1) * num7 + l);
					shapeData.m_indexList[num5++] = (short)(k * num7 + l + 1);
					shapeData.m_indexList[num5++] = (short)(k * num7 + l + 1);
					shapeData.m_indexList[num5++] = (short)((k + 1) * num7 + l);
					shapeData.m_indexList[num5++] = (short)((k + 1) * num7 + l + 1);
				}
			}
			return shapeData;
		}
	}
}
