using BulletXNA.BulletDynamics;
using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

public class MMDJoint
{
	public MMDModel _model;

	private int _targetRigidBodyIDA;

	private int _targetRigidBodyIDB;

	private MMDRigidBody _targetRigidBodyA;

	private MMDRigidBody _targetRigidBodyB;

	private Vector3 _position;

	private Vector3 _rotation;

	private Vector3 _limitPosFrom;

	private Vector3 _limitPosTo;

	private Vector3 _limitRotFrom;

	private Vector3 _limitRotTo;

	private Vector3 _springPosition;

	private Vector3 _springRotation;

	private Generic6DofSpringConstraint _bulletConstraint;

	private DiscreteDynamicsWorld _bulletWorld;

	public MMDModel model => _model;

	~MMDJoint()
	{
		Destroy();
	}

	public void Destroy()
	{
		LeaveWorld();
		if (_bulletConstraint != null)
		{
			_bulletConstraint.Cleanup();
			_bulletConstraint = null;
		}
		_bulletWorld = null;
		_model = null;
	}

	public bool Import(BinaryReader binaryReader)
	{
		if (!binaryReader.BeginStruct())
		{
			Debug.LogError("BeginStruct() failed.");
			return false;
		}
		binaryReader.ReadStructInt();
		binaryReader.ReadStructInt();
		binaryReader.ReadStructInt();
		binaryReader.ReadStructInt();
		_targetRigidBodyIDA = binaryReader.ReadStructInt();
		_targetRigidBodyIDB = binaryReader.ReadStructInt();
		_position = binaryReader.ReadStructVector3();
		_rotation = binaryReader.ReadStructVector3();
		_limitPosFrom = binaryReader.ReadStructVector3();
		_limitPosTo = binaryReader.ReadStructVector3();
		_limitRotFrom = binaryReader.ReadStructVector3();
		_limitRotTo = binaryReader.ReadStructVector3();
		_springPosition = binaryReader.ReadStructVector3();
		_springRotation = binaryReader.ReadStructVector3();
		if (_model != null)
		{
			_position *= _model._modelToBulletScale;
			_limitPosFrom *= _model._modelToBulletScale;
			_limitPosTo *= _model._modelToBulletScale;
			_springPosition *= _model._modelToBulletScale;
		}
		if (!binaryReader.EndStruct())
		{
			Debug.LogError("EndStruct() failed.");
			return false;
		}
		if (_model != null)
		{
			_targetRigidBodyA = _model.GetRigidBody(_targetRigidBodyIDA);
			_targetRigidBodyB = _model.GetRigidBody(_targetRigidBodyIDB);
		}
		return true;
	}

	private bool _SetupConstraint()
	{
		if (_bulletConstraint != null)
		{
			return true;
		}
		if (_targetRigidBodyA == null || _targetRigidBodyB == null)
		{
			_ = _targetRigidBodyA;
			_ = _targetRigidBodyB;
			return true;
		}
		BulletXNA.BulletDynamics.RigidBody bulletRigidBody = _targetRigidBodyA.bulletRigidBody;
		BulletXNA.BulletDynamics.RigidBody bulletRigidBody2 = _targetRigidBodyB.bulletRigidBody;
		if (bulletRigidBody == null || bulletRigidBody2 == null)
		{
			return false;
		}
		IndexedMatrix identity = IndexedMatrix.Identity;
		Vector3 rotation = new Vector3(0f - _rotation.x, 0f - _rotation.y, _rotation.z);
		identity._basis = Math.BasisRotationYXZ(ref rotation);
		identity._origin = new IndexedVector3(_position.x, _position.y, 0f - _position.z);
		if (_model != null && _model._rootBone != null)
		{
			identity._origin += _model._rootBone.GetWorldTransform()._origin;
		}
		IndexedMatrix frameInA = bulletRigidBody.GetWorldTransform().Inverse() * identity;
		IndexedMatrix frameInB = bulletRigidBody2.GetWorldTransform().Inverse() * identity;
		_bulletConstraint = new Generic6DofSpringConstraint(bulletRigidBody, bulletRigidBody2, frameInA, frameInB, useLinearReferenceFrameA: true);
		_bulletConstraint.SetLinearUpperLimit(new IndexedVector3(_limitPosTo[0], _limitPosTo[1], 0f - _limitPosFrom[2]));
		_bulletConstraint.SetLinearLowerLimit(new IndexedVector3(_limitPosFrom[0], _limitPosFrom[1], 0f - _limitPosTo[2]));
		_bulletConstraint.SetAngularUpperLimit(new IndexedVector3(0f - _limitRotFrom[0], 0f - _limitRotFrom[1], _limitRotTo[2]));
		_bulletConstraint.SetAngularLowerLimit(new IndexedVector3(0f - _limitRotTo[0], 0f - _limitRotTo[1], _limitRotFrom[2]));
		for (int i = 0; i < 6; i++)
		{
			if (i >= 3 || _springPosition[i] != 0f)
			{
				_bulletConstraint.EnableSpring(i, onOff: true);
				if (i >= 3)
				{
					_bulletConstraint.SetStiffness(i, _springRotation[i - 3]);
				}
				else
				{
					_bulletConstraint.SetStiffness(i, _springPosition[i]);
				}
			}
		}
		return true;
	}

	public bool JoinWorld()
	{
		if (_bulletConstraint == null)
		{
			if (!_SetupConstraint())
			{
				Debug.LogError("PMXJoint::JoinWorld:SetupConstraint() failed.");
				return false;
			}
			if (_bulletConstraint == null)
			{
				return true;
			}
		}
		if (_bulletWorld != null || _bulletConstraint == null || _model == null || _model.bulletWorld == null)
		{
			Debug.LogError("PMXJoint::JoinWorld: null.");
			return false;
		}
		_bulletWorld = _model.bulletWorld;
		_bulletWorld.AddConstraint(_bulletConstraint);
		return true;
	}

	public void LeaveWorld()
	{
		if (_bulletConstraint != null)
		{
			if (_bulletWorld != null)
			{
				_bulletWorld.RemoveConstraint(_bulletConstraint);
			}
			_bulletConstraint.Cleanup();
			_bulletConstraint = null;
		}
		_bulletWorld = null;
	}
}
