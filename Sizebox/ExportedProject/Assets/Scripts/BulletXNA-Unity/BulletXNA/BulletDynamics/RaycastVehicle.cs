using System;
using System.Collections.Generic;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class RaycastVehicle : IActionInterface
	{
		public const float sideFrictionStiffness2 = 1f;

		private float m_tau;

		private float m_damping;

		private IVehicleRaycaster m_vehicleRaycaster;

		private float m_pitchControl;

		private float m_steeringValue;

		private float m_currentVehicleSpeedKmHour;

		private RigidBody m_chassisBody;

		private int m_indexRightAxis;

		private int m_indexUpAxis;

		private int m_indexForwardAxis;

		private IndexedVector3[] m_axle = new IndexedVector3[4];

		private IndexedVector3[] m_forwardWS = new IndexedVector3[4];

		private float[] m_forwardImpulse = new float[4];

		private float[] m_sideImpulse = new float[4];

		private int m_userConstraintType;

		private int m_userConstraintId;

		private IList<WheelInfo> m_wheelInfo = new List<WheelInfo>();

		public static RigidBody s_fixedObject = new RigidBody(0f, null, null, IndexedVector3.Zero);

		public RaycastVehicle(VehicleTuning tuning, RigidBody chassis, IVehicleRaycaster raycaster)
		{
			m_vehicleRaycaster = raycaster;
			m_pitchControl = 0f;
			m_chassisBody = chassis;
			m_indexRightAxis = 0;
			m_indexUpAxis = 1;
			m_indexForwardAxis = 2;
			DefaultInit(ref tuning);
		}

		public virtual void UpdateAction(CollisionWorld collisionWorld, float step)
		{
			UpdateVehicle(step);
		}

		public virtual void DebugDraw(IDebugDraw debugDrawer)
		{
			for (int i = 0; i < GetNumWheels(); i++)
			{
				IndexedVector3 indexedVector = new IndexedVector3(0f, 1f, 1f);
				indexedVector = ((!GetWheelInfo(i).m_raycastInfo.m_isInContact) ? new IndexedVector3(1f, 0f, 1f) : new IndexedVector3(0f, 0f, 1f));
				IndexedVector3 origin = GetWheelInfo(i).m_worldTransform._origin;
				IndexedMatrix worldTransform = GetWheelInfo(i).m_worldTransform;
				IndexedVector3 indexedVector2 = new IndexedVector3(GetWheelInfo(i).m_worldTransform._basis._el0[GetRightAxis()], GetWheelInfo(i).m_worldTransform._basis._el1[GetRightAxis()], GetWheelInfo(i).m_worldTransform._basis._el2[GetRightAxis()]);
				debugDrawer.DrawLine(origin, origin + indexedVector2, indexedVector);
				debugDrawer.DrawLine(origin, GetWheelInfo(i).m_raycastInfo.m_contactPointWS, indexedVector);
			}
		}

		public IndexedMatrix GetChassisWorldTransform()
		{
			return GetRigidBody().GetCenterOfMassTransform();
		}

		public float RayCast(WheelInfo wheel)
		{
			UpdateWheelTransformsWS(wheel, false);
			float result = -1f;
			float num = wheel.GetSuspensionRestLength() + wheel.m_wheelsRadius;
			IndexedVector3 indexedVector = wheel.m_raycastInfo.m_wheelDirectionWS * num;
			IndexedVector3 from = wheel.m_raycastInfo.m_hardPointWS;
			wheel.m_raycastInfo.m_contactPointWS = from + indexedVector;
			IndexedVector3 to = wheel.m_raycastInfo.m_contactPointWS;
			float num2 = 0f;
			VehicleRaycasterResult result2 = new VehicleRaycasterResult();
			object obj = m_vehicleRaycaster.CastRay(ref from, ref to, ref result2);
			if (obj != null)
			{
				int debugBodyId = (obj as RigidBody).m_debugBodyId;
				int num9 = 1;
			}
			wheel.m_raycastInfo.m_groundObject = null;
			if (obj != null)
			{
				num2 = result2.m_distFraction;
				result = num * result2.m_distFraction;
				wheel.m_raycastInfo.m_contactNormalWS = result2.m_hitNormalInWorld;
				wheel.m_raycastInfo.m_isInContact = true;
				wheel.m_raycastInfo.m_groundObject = s_fixedObject;
				float num3 = num2 * num;
				wheel.m_raycastInfo.m_suspensionLength = num3 - wheel.m_wheelsRadius;
				float num4 = wheel.GetSuspensionRestLength() - wheel.m_maxSuspensionTravelCm * 0.01f;
				float num5 = wheel.GetSuspensionRestLength() + wheel.m_maxSuspensionTravelCm * 0.01f;
				if (wheel.m_raycastInfo.m_suspensionLength < num4)
				{
					wheel.m_raycastInfo.m_suspensionLength = num4;
				}
				if (wheel.m_raycastInfo.m_suspensionLength > num5)
				{
					wheel.m_raycastInfo.m_suspensionLength = num5;
				}
				Math.Abs(wheel.m_raycastInfo.m_suspensionLength - wheel.m_raycastInfo.m_suspensionLengthBak);
				float num10 = 0.1f;
				wheel.m_raycastInfo.m_suspensionLengthBak = wheel.m_raycastInfo.m_suspensionLength;
				wheel.m_raycastInfo.m_contactPointWS = result2.m_hitPointInWorld;
				float num6 = IndexedVector3.Dot(wheel.m_raycastInfo.m_contactNormalWS, wheel.m_raycastInfo.m_wheelDirectionWS);
				IndexedVector3 rel_pos = wheel.m_raycastInfo.m_contactPointWS - GetRigidBody().GetCenterOfMassPosition();
				IndexedVector3 velocityInLocalPoint = GetRigidBody().GetVelocityInLocalPoint(ref rel_pos);
				float num7 = IndexedVector3.Dot(wheel.m_raycastInfo.m_contactNormalWS, velocityInLocalPoint);
				float num11 = 1f;
				if (num6 >= -0.1f)
				{
					wheel.m_suspensionRelativeVelocity = 0f;
					wheel.m_clippedInvContactDotSuspension = 10f;
				}
				else
				{
					float num8 = -1f / num6;
					wheel.m_suspensionRelativeVelocity = num7 * num8;
					wheel.m_clippedInvContactDotSuspension = num8;
				}
			}
			else
			{
				wheel.m_raycastInfo.m_suspensionLength = wheel.GetSuspensionRestLength();
				wheel.m_suspensionRelativeVelocity = 0f;
				wheel.m_raycastInfo.m_contactNormalWS = -wheel.m_raycastInfo.m_wheelDirectionWS;
				wheel.m_clippedInvContactDotSuspension = 1f;
			}
			return result;
		}

		public virtual void UpdateVehicle(float step)
		{
			int numWheels = GetNumWheels();
			for (int i = 0; i < numWheels; i++)
			{
				UpdateWheelTransform(i, false);
			}
			m_currentVehicleSpeedKmHour = 3.6f * GetRigidBody().GetLinearVelocity().Length();
			IndexedMatrix chassisWorldTransform = GetChassisWorldTransform();
			IndexedVector3 a = new IndexedVector3(chassisWorldTransform._basis[0, m_indexForwardAxis], chassisWorldTransform._basis[1, m_indexForwardAxis], chassisWorldTransform._basis[2, m_indexForwardAxis]);
			if (IndexedVector3.Dot(a, GetRigidBody().GetLinearVelocity()) < 0f)
			{
				m_currentVehicleSpeedKmHour *= -1f;
			}
			float[] array = new float[numWheels];
			float num = 0f;
			for (int j = 0; j < numWheels; j++)
			{
				num = RayCast(m_wheelInfo[j]);
				array[j] = num;
				if (!m_wheelInfo[j].m_raycastInfo.m_isInContact)
				{
					num = RayCast(m_wheelInfo[j]);
					array[j] = num;
				}
			}
			UpdateSuspension(step);
			for (int k = 0; k < numWheels; k++)
			{
				WheelInfo wheelInfo = m_wheelInfo[k];
				float num2 = wheelInfo.m_wheelsSuspensionForce;
				if (num2 > wheelInfo.m_maxSuspensionForce)
				{
					num2 = wheelInfo.m_maxSuspensionForce;
				}
				IndexedVector3 impulse = wheelInfo.m_raycastInfo.m_contactNormalWS * num2 * step;
				IndexedVector3 rel_pos = wheelInfo.m_raycastInfo.m_contactPointWS - GetRigidBody().GetCenterOfMassPosition();
				if (!(impulse.Y < 30f))
				{
					float y = impulse.Y;
					float num5 = 40f;
				}
				GetRigidBody().ApplyImpulse(ref impulse, ref rel_pos);
			}
			UpdateFriction(step);
			for (int l = 0; l < numWheels; l++)
			{
				WheelInfo wheelInfo2 = m_wheelInfo[l];
				IndexedVector3 rel_pos2 = wheelInfo2.m_raycastInfo.m_hardPointWS - GetRigidBody().GetCenterOfMassPosition();
				IndexedVector3 velocityInLocalPoint = GetRigidBody().GetVelocityInLocalPoint(ref rel_pos2);
				if (wheelInfo2.m_raycastInfo.m_isInContact)
				{
					IndexedMatrix chassisWorldTransform2 = GetChassisWorldTransform();
					IndexedVector3 a2 = new IndexedVector3(chassisWorldTransform2._basis[0, m_indexForwardAxis], chassisWorldTransform2._basis[1, m_indexForwardAxis], chassisWorldTransform2._basis[2, m_indexForwardAxis]);
					float num3 = IndexedVector3.Dot(a2, wheelInfo2.m_raycastInfo.m_contactNormalWS);
					a2 -= wheelInfo2.m_raycastInfo.m_contactNormalWS * num3;
					float num4 = IndexedVector3.Dot(a2, velocityInLocalPoint);
					wheelInfo2.m_deltaRotation = num4 * step / wheelInfo2.m_wheelsRadius;
					wheelInfo2.m_rotation += wheelInfo2.m_deltaRotation;
				}
				else
				{
					wheelInfo2.m_rotation += wheelInfo2.m_deltaRotation;
				}
				wheelInfo2.m_deltaRotation *= 0.99f;
			}
		}

		public void ResetSuspension()
		{
			foreach (WheelInfo item in m_wheelInfo)
			{
				item.m_raycastInfo.m_suspensionLength = item.GetSuspensionRestLength();
				item.m_suspensionRelativeVelocity = 0f;
				item.m_raycastInfo.m_contactNormalWS = -item.m_raycastInfo.m_wheelDirectionWS;
				item.m_clippedInvContactDotSuspension = 1f;
			}
		}

		public float GetSteeringValue(int wheel)
		{
			return GetWheelInfo(wheel).m_steering;
		}

		public void SetSteeringValue(float steering, int wheel)
		{
			WheelInfo wheelInfo = GetWheelInfo(wheel);
			wheelInfo.m_steering = steering;
		}

		public void ApplyEngineForce(float force, int wheel)
		{
			WheelInfo wheelInfo = GetWheelInfo(wheel);
			wheelInfo.m_engineForce = force;
		}

		public IndexedMatrix GetWheelTransformWS(int wheelIndex)
		{
			WheelInfo wheelInfo = m_wheelInfo[wheelIndex];
			return wheelInfo.m_worldTransform;
		}

		public void UpdateWheelTransform(int wheelIndex, bool interpolatedTransform)
		{
			WheelInfo wheelInfo = m_wheelInfo[wheelIndex];
			UpdateWheelTransformsWS(wheelInfo, interpolatedTransform);
			IndexedVector3 indexedVector = -wheelInfo.m_raycastInfo.m_wheelDirectionWS;
			IndexedVector3 wheelAxleWS = wheelInfo.m_raycastInfo.m_wheelAxleWS;
			IndexedVector3 indexedVector2 = IndexedVector3.Cross(indexedVector, wheelAxleWS);
			indexedVector2.Normalize();
			float steering = wheelInfo.m_steering;
			IndexedQuaternion q = new IndexedQuaternion(indexedVector, steering);
			IndexedBasisMatrix indexedBasisMatrix = new IndexedBasisMatrix(ref q);
			IndexedQuaternion q2 = new IndexedQuaternion(wheelAxleWS, 0f - wheelInfo.m_rotation);
			IndexedBasisMatrix indexedBasisMatrix2 = new IndexedBasisMatrix(ref q2);
			IndexedBasisMatrix indexedBasisMatrix3 = new IndexedBasisMatrix(wheelAxleWS.X, indexedVector2.X, indexedVector.X, wheelAxleWS.Y, indexedVector2.Y, indexedVector.Y, wheelAxleWS.Z, indexedVector2.Z, indexedVector.Z);
			wheelInfo.m_worldTransform._basis = indexedBasisMatrix * indexedBasisMatrix2 * indexedBasisMatrix3;
			wheelInfo.m_worldTransform._origin = wheelInfo.m_raycastInfo.m_hardPointWS + wheelInfo.m_raycastInfo.m_wheelDirectionWS * wheelInfo.m_raycastInfo.m_suspensionLength;
		}

		public WheelInfo AddWheel(ref IndexedVector3 connectionPointCS0, ref IndexedVector3 wheelDirectionCS0, ref IndexedVector3 wheelAxleCS, float suspensionRestLength, float wheelRadius, VehicleTuning tuning, bool isFrontWheel)
		{
			WheelInfoConstructionInfo ci = default(WheelInfoConstructionInfo);
			ci.m_chassisConnectionCS = connectionPointCS0;
			ci.m_wheelDirectionCS = wheelDirectionCS0;
			ci.m_wheelAxleCS = wheelAxleCS;
			ci.m_suspensionRestLength = suspensionRestLength;
			ci.m_wheelRadius = wheelRadius;
			ci.m_suspensionStiffness = tuning.m_suspensionStiffness;
			ci.m_wheelsDampingCompression = tuning.m_suspensionCompression;
			ci.m_wheelsDampingRelaxation = tuning.m_suspensionDamping;
			ci.m_frictionSlip = tuning.m_frictionSlip;
			ci.m_bIsFrontWheel = isFrontWheel;
			ci.m_maxSuspensionTravelCm = tuning.m_maxSuspensionTravelCm;
			ci.m_maxSuspensionForce = tuning.m_maxSuspensionForce;
			WheelInfo wheelInfo = new WheelInfo(ref ci);
			m_wheelInfo.Add(wheelInfo);
			UpdateWheelTransformsWS(wheelInfo, false);
			UpdateWheelTransform(GetNumWheels() - 1, false);
			return wheelInfo;
		}

		public int GetNumWheels()
		{
			return m_wheelInfo.Count;
		}

		public virtual WheelInfo GetWheelInfo(int index)
		{
			return m_wheelInfo[index];
		}

		public void UpdateWheelTransformsWS(WheelInfo wheel, bool interpolatedTransform)
		{
			wheel.m_raycastInfo.m_isInContact = false;
			IndexedMatrix worldTrans = GetChassisWorldTransform();
			if (interpolatedTransform && GetRigidBody().GetMotionState() != null)
			{
				GetRigidBody().GetMotionState().GetWorldTransform(out worldTrans);
			}
			wheel.m_raycastInfo.m_hardPointWS = worldTrans * wheel.m_chassisConnectionPointCS;
			wheel.m_raycastInfo.m_wheelDirectionWS = worldTrans._basis * wheel.m_wheelDirectionCS;
			wheel.m_raycastInfo.m_wheelAxleWS = worldTrans._basis * wheel.m_wheelAxleCS;
		}

		public void SetBrake(float brake, int wheelIndex)
		{
			GetWheelInfo(wheelIndex).m_brake = brake;
		}

		public void SetPitchControl(float pitch)
		{
			m_pitchControl = pitch;
		}

		public void UpdateSuspension(float deltaTime)
		{
			float num = 1f / m_chassisBody.GetInvMass();
			for (int i = 0; i < GetNumWheels(); i++)
			{
				WheelInfo wheelInfo = m_wheelInfo[i];
				if (wheelInfo.m_raycastInfo.m_isInContact)
				{
					float num2 = 0f;
					float suspensionRestLength = wheelInfo.GetSuspensionRestLength();
					float suspensionLength = wheelInfo.m_raycastInfo.m_suspensionLength;
					float num3 = suspensionRestLength - suspensionLength;
					num2 = wheelInfo.m_suspensionStiffness * num3 * wheelInfo.m_clippedInvContactDotSuspension;
					float suspensionRelativeVelocity = wheelInfo.m_suspensionRelativeVelocity;
					float num4 = ((!(suspensionRelativeVelocity < 0f)) ? wheelInfo.m_wheelsDampingRelaxation : wheelInfo.m_wheelsDampingCompression);
					num2 -= num4 * suspensionRelativeVelocity;
					wheelInfo.m_wheelsSuspensionForce = num2 * num;
					if (wheelInfo.m_wheelsSuspensionForce < 0f)
					{
						wheelInfo.m_wheelsSuspensionForce = 0f;
					}
				}
				else
				{
					wheelInfo.m_wheelsSuspensionForce = 0f;
				}
			}
		}

		public virtual void UpdateFriction(float timeStep)
		{
			int numWheels = GetNumWheels();
			if (numWheels == 0)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < numWheels; i++)
			{
				WheelInfo wheelInfo = m_wheelInfo[i];
				RigidBody rigidBody = wheelInfo.m_raycastInfo.m_groundObject as RigidBody;
				if (rigidBody != null)
				{
					num++;
				}
				m_sideImpulse[i] = 0f;
				m_forwardImpulse[i] = 0f;
			}
			int num14 = 4;
			for (int j = 0; j < numWheels; j++)
			{
				WheelInfo wheelInfo2 = m_wheelInfo[j];
				RigidBody rigidBody2 = wheelInfo2.m_raycastInfo.m_groundObject as RigidBody;
				if (rigidBody2 != null)
				{
					IndexedBasisMatrix basis = GetWheelTransformWS(j)._basis;
					m_axle[j] = new IndexedVector3(basis._el0[m_indexRightAxis], basis._el1[m_indexRightAxis], basis._el2[m_indexRightAxis]);
					IndexedVector3 contactNormalWS = wheelInfo2.m_raycastInfo.m_contactNormalWS;
					float num2 = IndexedVector3.Dot(m_axle[j], contactNormalWS);
					m_axle[j] -= contactNormalWS * num2;
					m_axle[j].Normalize();
					m_forwardWS[j] = IndexedVector3.Cross(contactNormalWS, m_axle[j]);
					m_forwardWS[j].Normalize();
					IndexedVector3 normal = m_axle[j];
					float impulse = m_sideImpulse[j];
					ContactConstraint.ResolveSingleBilateral(m_chassisBody, ref wheelInfo2.m_raycastInfo.m_contactPointWS, rigidBody2, ref wheelInfo2.m_raycastInfo.m_contactPointWS, 0f, ref normal, ref impulse, timeStep);
					m_sideImpulse[j] = impulse * 1f;
				}
			}
			float num3 = 1f;
			float num4 = 0.5f;
			bool flag = false;
			for (int k = 0; k < numWheels; k++)
			{
				WheelInfo wheelInfo3 = m_wheelInfo[k];
				RigidBody rigidBody3 = wheelInfo3.m_raycastInfo.m_groundObject as RigidBody;
				float num5 = 0f;
				if (rigidBody3 != null)
				{
					if (wheelInfo3.m_engineForce != 0f)
					{
						num5 = wheelInfo3.m_engineForce * timeStep;
					}
					else
					{
						float num6 = 0f;
						float maxImpulse = ((wheelInfo3.m_brake != 0f) ? wheelInfo3.m_brake : num6);
						IndexedVector3 frictionDirectionWorld = m_forwardWS[k];
						WheelContactPoint contactPoint = new WheelContactPoint(m_chassisBody, rigidBody3, ref wheelInfo3.m_raycastInfo.m_contactPointWS, ref frictionDirectionWorld, maxImpulse);
						m_forwardWS[k] = frictionDirectionWorld;
						num5 = CalcRollingFriction(contactPoint);
					}
				}
				m_forwardImpulse[k] = 0f;
				m_wheelInfo[k].m_skidInfo = 1f;
				if (rigidBody3 != null)
				{
					m_wheelInfo[k].m_skidInfo = 1f;
					float num7 = wheelInfo3.m_wheelsSuspensionForce * timeStep * wheelInfo3.m_frictionSlip;
					float num8 = num7;
					float num9 = num7 * num8;
					m_forwardImpulse[k] = num5;
					float num10 = m_forwardImpulse[k] * num4;
					float num11 = m_sideImpulse[k] * num3;
					float num12 = num10 * num10 + num11 * num11;
					if (num12 > num9)
					{
						flag = true;
						float num13 = (float)((double)num7 / Math.Sqrt(num12));
						m_wheelInfo[k].m_skidInfo *= num13;
					}
				}
			}
			if (flag)
			{
				for (int l = 0; l < numWheels; l++)
				{
					if (m_sideImpulse[l] != 0f && m_wheelInfo[l].m_skidInfo < 1f)
					{
						m_forwardImpulse[l] *= m_wheelInfo[l].m_skidInfo;
						m_sideImpulse[l] *= m_wheelInfo[l].m_skidInfo;
					}
				}
			}
			for (int m = 0; m < numWheels; m++)
			{
				WheelInfo wheelInfo4 = m_wheelInfo[m];
				IndexedVector3 rel_pos = wheelInfo4.m_raycastInfo.m_contactPointWS - m_chassisBody.GetCenterOfMassPosition();
				if (!(m_forwardImpulse[m] > 5f))
				{
					float num15 = m_sideImpulse[m];
					float num16 = 5f;
				}
				if (m_forwardImpulse[m] != 0f)
				{
					m_chassisBody.ApplyImpulse(m_forwardWS[m] * m_forwardImpulse[m], rel_pos);
				}
				if (m_sideImpulse[m] != 0f)
				{
					RigidBody rigidBody4 = m_wheelInfo[m].m_raycastInfo.m_groundObject as RigidBody;
					IndexedVector3 rel_pos2 = wheelInfo4.m_raycastInfo.m_contactPointWS - rigidBody4.GetCenterOfMassPosition();
					IndexedVector3 impulse2 = m_axle[m] * m_sideImpulse[m];
					IndexedVector3 column = GetRigidBody().GetCenterOfMassTransform()._basis.GetColumn(m_indexUpAxis);
					rel_pos -= column * (IndexedVector3.Dot(column, rel_pos) * (1f - wheelInfo4.m_rollInfluence));
					m_chassisBody.ApplyImpulse(ref impulse2, ref rel_pos);
					IndexedVector3 impulse3 = -impulse2;
					rigidBody4.ApplyImpulse(ref impulse3, ref rel_pos2);
				}
			}
		}

		public RigidBody GetRigidBody()
		{
			return m_chassisBody;
		}

		public int GetRightAxis()
		{
			return m_indexRightAxis;
		}

		public int GetUpAxis()
		{
			return m_indexUpAxis;
		}

		public int GetForwardAxis()
		{
			return m_indexForwardAxis;
		}

		public IndexedVector3 GetForwardVector()
		{
			IndexedMatrix chassisWorldTransform = GetChassisWorldTransform();
			return new IndexedVector3(chassisWorldTransform._basis[0, m_indexForwardAxis], chassisWorldTransform._basis[1, m_indexForwardAxis], chassisWorldTransform._basis[2, m_indexForwardAxis]);
		}

		public float GetCurrentSpeedKmHour()
		{
			return m_currentVehicleSpeedKmHour;
		}

		public virtual void SetCoordinateSystem(int rightIndex, int upIndex, int forwardIndex)
		{
			m_indexRightAxis = rightIndex;
			m_indexUpAxis = upIndex;
			m_indexForwardAxis = forwardIndex;
		}

		public int GetUserConstraintType()
		{
			return m_userConstraintType;
		}

		public void SetUserConstraintType(int userConstraintType)
		{
			m_userConstraintType = userConstraintType;
		}

		public void SetUserConstraintId(int uid)
		{
			m_userConstraintId = uid;
		}

		public int GetUserConstraintId()
		{
			return m_userConstraintId;
		}

		public static float CalcRollingFriction(WheelContactPoint contactPoint)
		{
			float num = 0f;
			IndexedVector3 frictionPositionWorld = contactPoint.m_frictionPositionWorld;
			IndexedVector3 rel_pos = frictionPositionWorld - contactPoint.m_body0.GetCenterOfMassPosition();
			IndexedVector3 rel_pos2 = frictionPositionWorld - contactPoint.m_body1.GetCenterOfMassPosition();
			float maxImpulse = contactPoint.m_maxImpulse;
			IndexedVector3 velocityInLocalPoint = contactPoint.m_body0.GetVelocityInLocalPoint(ref rel_pos);
			IndexedVector3 velocityInLocalPoint2 = contactPoint.m_body1.GetVelocityInLocalPoint(ref rel_pos2);
			IndexedVector3 b = velocityInLocalPoint - velocityInLocalPoint2;
			float num2 = IndexedVector3.Dot(contactPoint.m_frictionDirectionWorld, b);
			num = (0f - num2) * contactPoint.m_jacDiagABInv;
			num = Math.Min(num, maxImpulse);
			return Math.Max(num, 0f - maxImpulse);
		}

		private void DefaultInit(ref VehicleTuning tuning)
		{
			m_currentVehicleSpeedKmHour = 0f;
			m_steeringValue = 0f;
		}
	}
}
