using System;
using BulletXNA;
using BulletXNA.BulletCollision;
using BulletXNA.BulletDynamics;
using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

public class MMDRigidBody
{
	public MMDModel _model;

	public MMDBone _bone;

	private int _boneID = -1;

	private uint _collisionGroupID;

	private uint _collisionMask;

	private MMDShapeType _shapeType;

	private Vector3 _shapeSize;

	private Vector3 _shapeSizeScaled;

	private Vector3 _position;

	private Vector3 _rotation;

	private float _mass;

	private float _linearDamping;

	private float _angularDamping;

	private float _restitution;

	private float _friction;

	private MMDRigidBodyType _rigidBodyType;

	private uint _additionalFlags;

	private MMDModelProperty _modelProperty;

	private MMDRigidBodyProperty _rigidBodyProperty = new MMDRigidBodyProperty();

	private CollisionShape _shape;

	private IMotionState _motionState;

	private BulletXNA.BulletDynamics.RigidBody _bulletRigidBody;

	private bool _isFreezed;

	private bool _noBone;

	private IndexedMatrix _boneToBodyTransform = IndexedMatrix.Identity;

	private IndexedMatrix _bodyToBoneTransform = IndexedMatrix.Identity;

	private DiscreteDynamicsWorld _bulletWorld;

	private MMDCollider _collider = new MMDCollider();

	private IndexedMatrix _transform = IndexedMatrix.Identity;

	private bool _isFeedbackTransform;

	private bool _isFeedbackBoneTransform;

	private bool _isTouchKinematic;

	public uint preUpdate_updateRigidBodyFlags;

	public MMDModel model => _model;

	public MMDBone bone => _bone;

	public BulletXNA.BulletDynamics.RigidBody bulletRigidBody => _bulletRigidBody;

	public bool isFreezed => _isFreezed;

	public bool isKinematic => _rigidBodyType == MMDRigidBodyType.Kinematics;

	public bool isSimulated => _rigidBodyType != MMDRigidBodyType.Kinematics;

	public MMDRigidBodyType rigidBodyType => _rigidBodyType;

	public int originalParentBoneID
	{
		get
		{
			if (_bone == null || _bone._originalParentBone == null)
			{
				return -1;
			}
			return _bone._originalParentBone._boneID;
		}
	}

	~MMDRigidBody()
	{
		Destroy();
	}

	public void Destroy()
	{
		LeaveWorld();
		if (_bulletRigidBody != null)
		{
			_bulletRigidBody.SetUserPointer(null);
			_bulletRigidBody.Cleanup();
			_bulletRigidBody = null;
		}
		_motionState = null;
		if (_shape != null)
		{
			_shape.Cleanup();
			_shape = null;
		}
		_bulletWorld = null;
	}

	public bool Import(BinaryReader binaryReader)
	{
		if (!binaryReader.BeginStruct())
		{
			Debug.LogError("");
			return false;
		}
		if (_model != null)
		{
			_modelProperty = _model._modelProperty;
		}
		if (_modelProperty == null)
		{
			_modelProperty = new MMDModelProperty();
		}
		if (_rigidBodyProperty == null)
		{
			_rigidBodyProperty = new MMDRigidBodyProperty();
		}
		_additionalFlags = (uint)binaryReader.ReadStructInt();
		binaryReader.ReadStructInt();
		binaryReader.ReadStructInt();
		_boneID = binaryReader.ReadStructInt();
		_collisionGroupID = (uint)binaryReader.ReadStructInt();
		_collisionMask = (uint)binaryReader.ReadStructInt();
		_shapeType = (MMDShapeType)binaryReader.ReadStructInt();
		_rigidBodyType = (MMDRigidBodyType)binaryReader.ReadStructInt();
		_shapeSize = binaryReader.ReadStructVector3();
		_position = binaryReader.ReadStructVector3();
		_rotation = binaryReader.ReadStructVector3();
		_mass = binaryReader.ReadStructFloat();
		_linearDamping = binaryReader.ReadStructFloat();
		_angularDamping = binaryReader.ReadStructFloat();
		_restitution = binaryReader.ReadStructFloat();
		_friction = binaryReader.ReadStructFloat();
		_shapeSizeScaled = _shapeSize;
		if (!binaryReader.EndStruct())
		{
			Debug.LogError("");
			return false;
		}
		_isFreezed = (_additionalFlags & 1) != 0;
		if (_model != null)
		{
			_shapeSize *= _model._modelToBulletScale;
			_position *= _model._modelToBulletScale;
		}
		_position.z = 0f - _position.z;
		_rotation.x = 0f - _rotation.x;
		_rotation.y = 0f - _rotation.y;
		_boneToBodyTransform._basis = Math.BasisRotationYXZ(ref _rotation);
		_boneToBodyTransform._origin = _position;
		_bodyToBoneTransform = _boneToBodyTransform.Inverse();
		_noBone = _boneID < 0;
		if (_model != null)
		{
			if (_noBone)
			{
				_bone = _model.GetBone(0);
			}
			else
			{
				_bone = _model.GetBone(_boneID);
				if (_bone != null)
				{
					if (_rigidBodyType != 0)
					{
						_bone._simulatedRigidBody = this;
					}
					else
					{
						_bone._isKinematicRigidBody = true;
					}
				}
			}
		}
		return true;
	}

	public void Config(MMDRigidBodyProperty rigidBodyProperty)
	{
		if (_rigidBodyProperty != null && rigidBodyProperty != null)
		{
			_rigidBodyProperty.Copy(rigidBodyProperty);
			_PostfixProperty();
		}
	}

	public void SetFreezed(bool isFreezed)
	{
		_isFreezed = isFreezed;
		if (_rigidBodyProperty != null)
		{
			_rigidBodyProperty.isFreezed = (isFreezed ? 1 : 0);
		}
	}

	public void FeedbackBoneToBodyTransform()
	{
		FeedbackBoneToBodyTransform(forceOverwrite: false);
	}

	public void FeedbackBoneToBodyTransform(bool forceOverwrite)
	{
		if (_bone == null || _noBone)
		{
			return;
		}
		if (_rigidBodyType == MMDRigidBodyType.Kinematics)
		{
			if (_motionState != null)
			{
				((KinematicMotionState)_motionState).m_graphicsWorldTrans = _bone.GetWorldTransform() * _boneToBodyTransform;
			}
		}
		else if ((forceOverwrite || _isFreezed) && _bulletRigidBody != null)
		{
			IndexedVector3 lin_vel = IndexedVector3.Zero;
			IndexedMatrix xform = _bone.GetWorldTransform() * _boneToBodyTransform;
			_bulletRigidBody.SetLinearVelocity(ref lin_vel);
			_bulletRigidBody.SetAngularVelocity(ref lin_vel);
			_bulletRigidBody.ClearForces();
			_bulletRigidBody.SetCenterOfMassTransform(ref xform);
		}
	}

	public void ProcessPreBoneAlignment()
	{
		if (_bulletRigidBody != null && _bone != null && !_noBone && _model != null && _modelProperty != null && _rigidBodyType != 0 && !_isFreezed)
		{
			IndexedMatrix transform = _bulletRigidBody.GetCenterOfMassTransform();
			IndexedMatrix boneTransform = transform * _bodyToBoneTransform;
			if (_ProcessBoneAlignment(ref transform, ref boneTransform, _modelProperty.rigidBodyPreBoneAlignmentLimitLength, _modelProperty.rigidBodyPreBoneAlignmentLossRate))
			{
				_bulletRigidBody.SetCenterOfMassTransform(ref transform);
			}
			_SetWorldTransformToBone(ref boneTransform);
		}
	}

	public void ProcessPostBoneAlignment()
	{
		if (_bulletRigidBody != null && _bone != null && !_noBone && _model != null && _modelProperty != null && _rigidBodyType != 0 && !_isFreezed)
		{
			IndexedMatrix transform = _bulletRigidBody.GetCenterOfMassTransform();
			IndexedMatrix boneTransform = transform * _bodyToBoneTransform;
			if (_ProcessBoneAlignment(ref transform, ref boneTransform, _modelProperty.rigidBodyPostBoneAlignmentLimitLength, _modelProperty.rigidBodyPostBoneAlignmentLossRate))
			{
				_bulletRigidBody.SetCenterOfMassTransform(ref transform);
			}
			_SetWorldTransformToBone(ref boneTransform);
		}
	}

	public void PrepareTransform()
	{
		if (_bulletRigidBody != null)
		{
			_transform = _bulletRigidBody.GetCenterOfMassTransform();
		}
	}

	public void ApplyTransformToBone(float deltaTime, bool isSkipPostprocess)
	{
		if (_rigidBodyType == MMDRigidBodyType.Kinematics || _bone == null || _noBone || _isFreezed || _model == null || _modelProperty == null)
		{
			return;
		}
		IndexedMatrix boneTransform = _bulletRigidBody.GetCenterOfMassTransform() * _bodyToBoneTransform;
		if (!isSkipPostprocess)
		{
			_ProcessVelocityLimit();
			if (_ProcessBoneAlignment(ref _transform, ref boneTransform, _modelProperty.rigidBodyPostBoneAlignmentLimitLength, _modelProperty.rigidBodyPostBoneAlignmentLossRate))
			{
				_isFeedbackTransform = true;
			}
			if (!_model._optimizeSettings && _ProcessForceLimitAngularVelocity(ref _transform, ref boneTransform, deltaTime))
			{
				_isFeedbackTransform = true;
			}
		}
		_SetWorldTransformToBone(ref boneTransform);
	}

	public void PrepareCollider()
	{
		_isTouchKinematic = false;
		_collider.transform = _transform;
		_collider.shape = (int)_shapeType;
		_collider.size = _shapeSizeScaled;
		_collider.isKinematic = _rigidBodyType == MMDRigidBodyType.Kinematics || _isFreezed;
		_collider.isCollision = false;
	}

	public void ProcessCollider(MMDRigidBody rigidBodyB)
	{
		if (_model == null || _modelProperty == null)
		{
			return;
		}
		bool flag = _collider.isKinematic;
		bool flag2 = rigidBodyB._collider.isKinematic;
		if (!flag && flag2)
		{
			_isTouchKinematic = true;
		}
		else if (flag && !flag2)
		{
			rigidBodyB._isTouchKinematic = true;
		}
		if (Math.FastCollide(_collider, rigidBodyB._collider))
		{
			if (_collider.isCollision)
			{
				_transform = _collider.transform;
				_isFeedbackTransform = true;
				_isFeedbackBoneTransform = true;
			}
			if (rigidBodyB._collider.isCollision)
			{
				rigidBodyB._transform = rigidBodyB._collider.transform;
				rigidBodyB._isFeedbackTransform = true;
				rigidBodyB._isFeedbackBoneTransform = true;
			}
		}
	}

	public void FeedbackTransform()
	{
		if (_isFeedbackTransform)
		{
			_isFeedbackTransform = false;
			if (_bulletRigidBody != null)
			{
				_bulletRigidBody.SetCenterOfMassTransform(ref _transform);
			}
		}
		if (_isFeedbackBoneTransform)
		{
			_isFeedbackBoneTransform = false;
			if (!_noBone)
			{
				IndexedMatrix boneTransform = _transform * _bodyToBoneTransform;
				_SetWorldTransformToBone(ref boneTransform);
			}
		}
	}

	public void AntiJitterTransform()
	{
		if (_bone != null && !_noBone && _rigidBodyType != 0)
		{
			if (_isFreezed)
			{
				_bone.AntiJitterWorldTransformOnDisabled();
				return;
			}
			_bone.AntiJitterWorldTransform(_isTouchKinematic);
			_isTouchKinematic = false;
		}
	}

	private void _SetWorldTransformToBone(ref IndexedMatrix boneTransform)
	{
		if (_bone != null && !_noBone)
		{
			_bone._WriteWorldTransform(ref boneTransform);
		}
	}

	private static float _FastScl(float lhs, float rhs)
	{
		if (rhs == 1f)
		{
			return lhs;
		}
		if (lhs == 1f)
		{
			return rhs;
		}
		return lhs * rhs;
	}

	private bool _SetupBody()
	{
		if (_bulletRigidBody != null)
		{
			return true;
		}
		if (_bone == null)
		{
			return false;
		}
		if (_modelProperty == null)
		{
			return false;
		}
		_PostfixProperty();
		if (_shape == null)
		{
			float num = -1f;
			if (_rigidBodyProperty != null)
			{
				num = _rigidBodyProperty.shapeScale;
			}
			if (num < 0f)
			{
				num = 1f;
			}
			_shapeSizeScaled = _shapeSize * num;
			if (_shapeType == MMDShapeType.Sphere)
			{
				_shape = new SphereShape(_shapeSizeScaled.x);
			}
			else if (_shapeType == MMDShapeType.Box)
			{
				_shape = new BoxShape(new IndexedVector3(_shapeSizeScaled));
			}
			else
			{
				if (_shapeType != MMDShapeType.Capsule)
				{
					return false;
				}
				_shape = new CapsuleShape(_shapeSizeScaled.x, _shapeSizeScaled.y);
			}
		}
		float mass = 0f;
		IndexedVector3 inertia = IndexedVector3.Zero;
		if (_shape != null && _rigidBodyType != 0 && _mass != 0f)
		{
			mass = _FastScl(_mass, _modelProperty.rigidBodyMassRate);
			_shape.CalculateLocalInertia(mass, out inertia);
		}
		if (_model == null || _model._rootBone == null)
		{
			return false;
		}
		IndexedMatrix startTrans = _boneToBodyTransform;
		startTrans._origin += _bone._baseOrigin + _model._rootBone.GetWorldTransform()._origin;
		if (_rigidBodyType == MMDRigidBodyType.Kinematics)
		{
			_motionState = new KinematicMotionState(ref startTrans);
		}
		else
		{
			_motionState = new SimpleMotionState(ref startTrans);
		}
		RigidBodyConstructionInfo rigidBodyConstructionInfo = new RigidBodyConstructionInfo(mass, _motionState, _shape, inertia);
		rigidBodyConstructionInfo.m_linearDamping = _FastScl(_linearDamping, _modelProperty.rigidBodyLinearDampingRate);
		rigidBodyConstructionInfo.m_angularDamping = _FastScl(_angularDamping, _modelProperty.rigidBodyAngularDampingRate);
		rigidBodyConstructionInfo.m_restitution = _FastScl(_restitution, _modelProperty.rigidBodyRestitutionRate);
		rigidBodyConstructionInfo.m_friction = _FastScl(_friction, _modelProperty.rigidBodyFrictionRate);
		rigidBodyConstructionInfo.m_additionalDamping = _modelProperty.rigidBodyIsAdditionalDamping;
		if (rigidBodyConstructionInfo.m_linearDamping < 1f && _modelProperty.rigidBodyLinearDampingLossRate > 0f)
		{
			rigidBodyConstructionInfo.m_linearDamping *= Mathf.Max(1f - _modelProperty.rigidBodyLinearDampingLossRate, 0f);
		}
		if (rigidBodyConstructionInfo.m_angularDamping < 1f && _modelProperty.rigidBodyAngularDampingLossRate > 0f)
		{
			rigidBodyConstructionInfo.m_angularDamping *= Mathf.Max(1f - _modelProperty.rigidBodyAngularDampingLossRate, 0f);
		}
		if (_modelProperty.rigidBodyLinearDampingLimit >= 0f)
		{
			rigidBodyConstructionInfo.m_linearDamping = Mathf.Min(rigidBodyConstructionInfo.m_linearDamping, _modelProperty.rigidBodyLinearDampingLimit);
		}
		if (_modelProperty.rigidBodyAngularDampingLimit >= 0f)
		{
			rigidBodyConstructionInfo.m_angularDamping = Mathf.Min(rigidBodyConstructionInfo.m_angularDamping, _modelProperty.rigidBodyAngularDampingLimit);
		}
		_bulletRigidBody = new BulletXNA.BulletDynamics.RigidBody(rigidBodyConstructionInfo);
		if (_bulletRigidBody != null)
		{
			_bulletRigidBody.SetUserPointer(this);
			if (_rigidBodyType == MMDRigidBodyType.Kinematics)
			{
				_bulletRigidBody.SetCollisionFlags(_bulletRigidBody.GetCollisionFlags() | BulletXNA.BulletCollision.CollisionFlags.CF_KINEMATIC_OBJECT);
			}
			if (_model != null && !_model._optimizeSettings && _modelProperty != null && _modelProperty.rigidBodyIsUseCcd)
			{
				float ccdSweptSphereRadius = 0f;
				if (_shapeType == MMDShapeType.Sphere)
				{
					ccdSweptSphereRadius = _shapeSizeScaled[0];
				}
				else if (_shapeType == MMDShapeType.Box)
				{
					ccdSweptSphereRadius = Mathf.Min(Mathf.Min(_shapeSizeScaled[0], _shapeSizeScaled[1]), _shapeSizeScaled[2]);
				}
				else if (_shapeType == MMDShapeType.Capsule)
				{
					ccdSweptSphereRadius = _shapeSizeScaled[0];
				}
				float ccdMotionThreshold = _modelProperty.rigidBodyCcdMotionThreshold * _model._worldToBulletScale;
				_bulletRigidBody.SetCcdMotionThreshold(ccdMotionThreshold);
				_bulletRigidBody.SetCcdSweptSphereRadius(ccdSweptSphereRadius);
			}
			if (_modelProperty != null && !_modelProperty.rigidBodyIsEnableSleeping)
			{
				_bulletRigidBody.SetSleepingThresholds(0f, 0f);
			}
			_bulletRigidBody.SetActivationState(ActivationState.DISABLE_DEACTIVATION);
		}
		return true;
	}

	private void _PostfixProperty()
	{
		if (_rigidBodyProperty != null && _modelProperty != null)
		{
			if (_rigidBodyProperty.isFreezed >= 0)
			{
				_isFreezed = _rigidBodyProperty.isFreezed != 0;
			}
			if (_rigidBodyProperty.shapeSize != Vector3.zero)
			{
				_shapeSize = _rigidBodyProperty.shapeSize;
			}
			if (_rigidBodyProperty.isUseForceAngularVelocityLimit == -1)
			{
				_rigidBodyProperty.isUseForceAngularVelocityLimit = (_modelProperty.rigidBodyIsUseForceAngularVelocityLimit ? 1 : 0);
			}
			if (_rigidBodyProperty.isUseForceAngularAccelerationLimit == -1)
			{
				_rigidBodyProperty.isUseForceAngularAccelerationLimit = (_modelProperty.rigidBodyIsUseForceAngularAccelerationLimit ? 1 : 0);
			}
			if (_rigidBodyProperty.forceAngularVelocityLimit < 0f)
			{
				_rigidBodyProperty.forceAngularVelocityLimit = _modelProperty.rigidBodyForceAngularVelocityLimit;
			}
			if (_rigidBodyProperty.linearVelocityLimit < 0f)
			{
				_rigidBodyProperty.linearVelocityLimit = _modelProperty.rigidBodyLinearVelocityLimit;
			}
			if (_rigidBodyProperty.angularVelocityLimit < 0f)
			{
				_rigidBodyProperty.angularVelocityLimit = _modelProperty.rigidBodyAngularVelocityLimit;
			}
			if (_rigidBodyProperty.shapeScale < 0f)
			{
				_rigidBodyProperty.shapeScale = _modelProperty.rigidBodyShapeScale;
			}
		}
	}

	public bool JoinWorld()
	{
		if (_bulletRigidBody == null && !_SetupBody())
		{
			Debug.LogError("Warning: PMXRigidBody::JoinWorld(): Body is nothing.");
			return false;
		}
		if (_bulletRigidBody == null || _bulletWorld != null || _model == null || _model.bulletWorld == null)
		{
			Debug.LogError("Warning: PMXRigidBody::JoinWorld(): Nothing.");
			return false;
		}
		int group = 1 << (int)_collisionGroupID;
		int collisionMask = (int)_collisionMask;
		_bulletWorld = _model.bulletWorld;
		_bulletWorld.AddRigidBody(_bulletRigidBody, (CollisionFilterGroups)group, (CollisionFilterGroups)collisionMask);
		return true;
	}

	public void LeaveWorld()
	{
		if (_bulletRigidBody != null)
		{
			if (_bulletWorld != null)
			{
				_bulletWorld.RemoveRigidBody(_bulletRigidBody);
			}
			_bulletRigidBody.Cleanup();
			_bulletRigidBody = null;
		}
		_bulletWorld = null;
	}

	private void _ProcessVelocityLimit()
	{
		if (_bulletRigidBody == null || _rigidBodyProperty == null || _model == null)
		{
			return;
		}
		if (_rigidBodyProperty.linearVelocityLimit >= 0f)
		{
			float num = _rigidBodyProperty.linearVelocityLimit * _model._worldToBulletScale;
			IndexedVector3 lin_vel = _bulletRigidBody.GetLinearVelocity();
			for (int i = 0; i < 3; i++)
			{
				lin_vel[i] = Mathf.Clamp(lin_vel[i], 0f - num, num);
			}
			_bulletRigidBody.SetLinearVelocity(ref lin_vel);
		}
		if (_rigidBodyProperty.angularVelocityLimit >= 0f)
		{
			float num2 = _rigidBodyProperty.angularVelocityLimit * ((float)System.Math.PI / 180f);
			IndexedVector3 ang_vel = _bulletRigidBody.GetAngularVelocity();
			for (int j = 0; j < 3; j++)
			{
				ang_vel[j] = Mathf.Clamp(ang_vel[j], 0f - num2, num2);
			}
			_bulletRigidBody.SetAngularVelocity(ref ang_vel);
		}
	}

	private bool _ProcessBoneAlignment(ref IndexedMatrix transform, ref IndexedMatrix boneTransform, float limitLength, float lossRate)
	{
		if (_model == null || _modelProperty == null || _bone == null)
		{
			return false;
		}
		float num = Mathf.Clamp01(1f - lossRate);
		limitLength *= model._worldToBulletScale;
		if (_bone._originalParentBone != null)
		{
			IndexedMatrix worldTransform = _bone._originalParentBone.GetWorldTransform();
			Vector3 vector = boneTransform._origin;
			Vector3 vector2 = worldTransform._origin;
			Vector3 vector3 = vector - vector2;
			Vector3 vector4 = worldTransform._basis.Transpose() * vector3;
			Vector3 vector5 = vector4 - _bone._offset;
			float magnitude = vector5.magnitude;
			if (magnitude > limitLength)
			{
				Vector3 vector6 = vector5 * (limitLength / magnitude) * num;
				Vector3 vector7 = vector6 + _bone._offset;
				Vector3 vector8 = worldTransform._basis * vector7;
				Vector3 vector9 = vector8 + vector2;
				boneTransform._origin = vector9;
				transform._origin += vector9 - vector;
				return true;
			}
		}
		return false;
	}

	private bool _ProcessForceLimitAngularVelocity(ref IndexedMatrix transform, ref IndexedMatrix boneTransform, float deltaTime)
	{
		if (_model == null || _modelProperty == null || _bone == null || _rigidBodyProperty == null)
		{
			return false;
		}
		if (_rigidBodyProperty.isUseForceAngularVelocityLimit != 0 && deltaTime > 0f && _bone._originalParentBone != null)
		{
			float num = _rigidBodyProperty.forceAngularVelocityLimit * ((float)System.Math.PI / 180f);
			float f = num * deltaTime;
			float num2 = Mathf.Cos(f);
			float num3 = num2 * num2;
			float num4 = num2;
			float num5 = num2;
			float limitTheta = num3;
			float limitTheta2 = num3;
			if (_bone._isSetPrevWorldTransform)
			{
				bool flag = false;
				IndexedVector3 column = _bone._prevWorldTransform._basis.GetColumn(0);
				IndexedVector3 column2 = _bone._prevWorldTransform._basis.GetColumn(2);
				IndexedVector3 v = boneTransform._basis.GetColumn(0);
				IndexedVector3 indexedVector = boneTransform._basis.GetColumn(2);
				float num6 = v.Dot(column);
				float num7 = indexedVector.Dot(column2);
				if (_rigidBodyProperty.isUseForceAngularAccelerationLimit != 0 && _bone._isSetPrevWorldTransform2)
				{
					IndexedVector3 column3 = _bone._prevWorldTransform2._basis.GetColumn(0);
					IndexedVector3 column4 = _bone._prevWorldTransform2._basis.GetColumn(2);
					IndexedVector3 angAccVector = Math.GetAngAccVector(column, column3);
					IndexedVector3 angAccVector2 = Math.GetAngAccVector(column2, column4);
					float num8 = angAccVector.Dot(column);
					if (num4 > num8)
					{
						num4 = num8;
						if (num4 < 0f)
						{
							num4 = 0f;
						}
						limitTheta = num4 * num4;
					}
					float num9 = angAccVector2.Dot(column2);
					if (num5 > num9)
					{
						num5 = num9;
						if (num5 < 0f)
						{
							num5 = 0f;
						}
						limitTheta2 = num5 * num5;
					}
				}
				if (num6 < num4)
				{
					flag = true;
					v = Math.ClampDirection(_bone._prevWorldTransform._basis.GetColumn(0), boneTransform._basis.GetColumn(0), num6, num4, limitTheta);
				}
				if (num7 < num5)
				{
					flag = true;
					indexedVector = Math.ClampDirection(_bone._prevWorldTransform._basis.GetColumn(2), boneTransform._basis.GetColumn(2), num7, num5, limitTheta2);
				}
				if (flag)
				{
					IndexedVector3 v2 = indexedVector.Cross(v);
					float num10 = v2.Length();
					if (num10 >= 0.01f)
					{
						v2 *= 1f / num10;
						indexedVector = v.Cross(v2);
						for (int i = 0; i < 3; i++)
						{
							boneTransform._basis[i] = new IndexedVector3(v[i], v2[i], indexedVector[i]);
						}
						transform._basis = boneTransform._basis * _boneToBodyTransform._basis;
						return true;
					}
					boneTransform._basis = _bone._prevWorldTransform._basis;
					transform._basis = boneTransform._basis * _boneToBodyTransform._basis;
					return true;
				}
			}
		}
		return false;
	}
}
