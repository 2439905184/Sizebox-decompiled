using BulletXNA;
using BulletXNA.BulletCollision;
using BulletXNA.BulletDynamics;
using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	public class RigidBody : PhysicsEntity
	{
		public struct CreateProperty
		{
			public bool isKinematic;

			public bool isAdditionalDamping;

			public int group;

			public int mask;

			public int shapeType;

			public Vector3 shapeSize;

			public Vector3 position;

			public Quaternion rotation;

			public float mass;

			public float linearDamping;

			public float angularDamping;

			public float restitution;

			public float friction;

			public float unityScale;
		}

		public class UpdateData
		{
			public int updateFlags;

			public bool updateTransform;

			public bool lateUpdateTransform;

			public Vector3 updatePosition;

			public Quaternion updateRotation;

			public Vector3 lateUpdatePosition;

			public Quaternion lateUpdateRotation;
		}

		private bool _isKinematic;

		private int _group = -1;

		private int _mask = -1;

		private float _worldToBulletScale;

		private float _bulletToWorldScale;

		private CollisionShape _shape;

		private IMotionState _motionState;

		private BulletXNA.BulletDynamics.RigidBody _body;

		private DiscreteDynamicsWorld _world;

		private bool _isFreezed;

		private bool _isJoinedWorld;

		private bool _isDestroyed;

		private bool _isMultiThreading;

		private UpdateData _sync_unusedData;

		private UpdateData _sync_updateData;

		private UpdateData _sync_updatedData;

		private UpdateData _sync_lateUpdateData;

		private UpdateData _updateData;

		~RigidBody()
		{
			_DestroyImmediate();
		}

		public bool Create(ref CreateProperty createProperty)
		{
			_isKinematic = createProperty.isKinematic;
			_group = createProperty.group;
			_mask = createProperty.mask;
			_bulletToWorldScale = 1f;
			_worldToBulletScale = 1f;
			if (createProperty.unityScale > float.Epsilon)
			{
				_bulletToWorldScale = createProperty.unityScale;
				_worldToBulletScale = 1f / _bulletToWorldScale;
			}
			Vector3 vector = createProperty.shapeSize * _worldToBulletScale;
			switch (createProperty.shapeType)
			{
			case 0:
				_shape = new SphereShape(vector.x);
				break;
			case 1:
				_shape = new BoxShape(new IndexedVector3(vector.x, vector.y, vector.z));
				break;
			case 2:
				_shape = new CapsuleShape(vector.x, vector.y);
				break;
			default:
				return false;
			}
			Vector3 position = createProperty.position;
			position.x = 0f - position.x;
			position *= _worldToBulletScale;
			Quaternion rotation = createProperty.rotation;
			rotation.y = 0f - rotation.y;
			rotation.z = 0f - rotation.z;
			IndexedMatrix startTrans = Math.MakeIndexedMatrix(ref position, ref rotation);
			if (_isKinematic)
			{
				_motionState = new KinematicMotionState(ref startTrans);
			}
			else
			{
				_motionState = new SimpleMotionState(ref startTrans);
			}
			float num = (_isKinematic ? 0f : createProperty.mass);
			bool flag = num != 0f;
			IndexedVector3 inertia = IndexedVector3.Zero;
			if (flag)
			{
				_shape.CalculateLocalInertia(num, out inertia);
			}
			RigidBodyConstructionInfo rigidBodyConstructionInfo = new RigidBodyConstructionInfo(num, _motionState, _shape, inertia);
			rigidBodyConstructionInfo.m_additionalDamping = createProperty.isAdditionalDamping;
			_body = new BulletXNA.BulletDynamics.RigidBody(rigidBodyConstructionInfo);
			if (_isKinematic)
			{
				_body.SetCollisionFlags(_body.GetCollisionFlags() | BulletXNA.BulletCollision.CollisionFlags.CF_KINEMATIC_OBJECT);
				_body.SetActivationState(ActivationState.DISABLE_DEACTIVATION);
			}
			return true;
		}

		public void Destroy()
		{
			_isDestroyed = true;
			if (!_isJoinedWorld)
			{
				_DestroyImmediate();
			}
		}

		private UpdateData _PrepareUpdate()
		{
			UpdateData updateData = null;
			if (_isMultiThreading)
			{
				lock (this)
				{
					updateData = _sync_unusedData;
					_sync_unusedData = null;
				}
			}
			else
			{
				updateData = _sync_unusedData;
				_sync_unusedData = null;
			}
			if (updateData == null)
			{
				updateData = new UpdateData();
			}
			return updateData;
		}

		private void _PostUpdate(UpdateData updateData)
		{
			if (_isMultiThreading)
			{
				lock (this)
				{
					_sync_updateData = updateData;
					return;
				}
			}
			_sync_updateData = updateData;
		}

		public void Update(int updateFlags)
		{
			UpdateData updateData = _PrepareUpdate();
			updateData.updateFlags = updateFlags;
			updateData.updateTransform = false;
			_PostUpdate(updateData);
		}

		public void Update(int updateFlags, ref Vector3 position, ref Quaternion rotation)
		{
			UpdateData updateData = _PrepareUpdate();
			updateData.updateFlags = updateFlags;
			updateData.updateTransform = true;
			updateData.updatePosition = position;
			updateData.updateRotation = rotation;
			_PostUpdate(updateData);
		}

		public int LateUpdate(ref Vector3 position, ref Quaternion rotation)
		{
			UpdateData updateData = null;
			if (_isMultiThreading)
			{
				lock (this)
				{
					updateData = _sync_lateUpdateData;
				}
			}
			else
			{
				updateData = _sync_lateUpdateData;
			}
			if (updateData == null)
			{
				return 0;
			}
			if (updateData.lateUpdateTransform)
			{
				position = updateData.lateUpdatePosition;
				rotation = updateData.lateUpdateRotation;
				return 1;
			}
			return 0;
		}

		private void _DestroyImmediate()
		{
			if (_body != null)
			{
				_body.Cleanup();
				_body = null;
			}
			_motionState = null;
			_shape = null;
		}

		private void _LockUpdateData()
		{
			if (_isMultiThreading)
			{
				lock (this)
				{
					_updateData = _sync_updateData;
					_sync_updateData = null;
					return;
				}
			}
			_updateData = _sync_updateData;
			_sync_updateData = null;
		}

		private void _UnlockUpdateData()
		{
			if (_isMultiThreading)
			{
				lock (this)
				{
					_sync_updatedData = _updateData;
				}
			}
			else
			{
				_sync_updatedData = _updateData;
			}
			_updateData = null;
		}

		private void _FeedbackUpdateData()
		{
			if (_updateData == null)
			{
				return;
			}
			_isFreezed = (_updateData.updateFlags & 1) != 0;
			if (!_updateData.updateTransform || (!_isKinematic && !_isFreezed))
			{
				return;
			}
			Vector3 position = _updateData.updatePosition;
			Quaternion rotation = _updateData.updateRotation;
			position.x = 0f - position.x;
			position *= _worldToBulletScale;
			rotation.y = 0f - rotation.y;
			rotation.z = 0f - rotation.z;
			if (_isKinematic)
			{
				if (_motionState != null)
				{
					Math.MakeIndexedMatrix(ref ((KinematicMotionState)_motionState).m_graphicsWorldTrans, ref position, ref rotation);
				}
			}
			else if (_isFreezed && _body != null)
			{
				IndexedMatrix matrix = IndexedMatrix.Identity;
				Math.MakeIndexedMatrix(ref matrix, ref position, ref rotation);
				_body.SetCenterOfMassTransform(ref matrix);
			}
		}

		private void _FeedbackLateUpdateData()
		{
			if (_updateData != null)
			{
				_updateData.lateUpdateTransform = false;
				if (!_isKinematic && !_isFreezed && _motionState != null)
				{
					_updateData.lateUpdateTransform = true;
					IndexedMatrix worldTrans;
					_motionState.GetWorldTransform(out worldTrans);
					_updateData.lateUpdatePosition = worldTrans._origin;
					_updateData.lateUpdateRotation = worldTrans.GetRotation();
					_updateData.lateUpdatePosition.x = 0f - _updateData.lateUpdatePosition.x;
					_updateData.lateUpdatePosition *= _bulletToWorldScale;
					_updateData.lateUpdateRotation.y = 0f - _updateData.lateUpdateRotation.y;
					_updateData.lateUpdateRotation.z = 0f - _updateData.lateUpdateRotation.z;
				}
			}
		}

		public override bool _JoinWorld()
		{
			if (_body == null)
			{
				return false;
			}
			if (base.physicsWorld == null || base.bulletWorld == null)
			{
				return false;
			}
			_isJoinedWorld = true;
			_isMultiThreading = base.physicsWorld.isMultiThreading;
			_world = base.bulletWorld;
			if (_group < 0 && _mask < 0)
			{
				_world.AddRigidBody(_body);
			}
			else
			{
				_world.AddRigidBody(_body, (CollisionFilterGroups)_group, (CollisionFilterGroups)_mask);
			}
			return true;
		}

		public override void _LeaveWorld()
		{
			if (_body != null && _world != null)
			{
				_world.RemoveRigidBody(_body);
			}
			_world = null;
			_isJoinedWorld = false;
			_isMultiThreading = false;
			if (_isDestroyed)
			{
				_DestroyImmediate();
			}
		}

		public override void _PreUpdate()
		{
			_LockUpdateData();
			_FeedbackUpdateData();
		}

		public override void _PostUpdate()
		{
			_FeedbackLateUpdateData();
			_UnlockUpdateData();
		}

		public override void _PrepareLateUpdate()
		{
			if (_isMultiThreading)
			{
				lock (this)
				{
					_sync_unusedData = _sync_lateUpdateData;
					_sync_lateUpdateData = _sync_updatedData;
					_sync_updatedData = null;
					return;
				}
			}
			_sync_unusedData = _sync_lateUpdateData;
			_sync_lateUpdateData = _sync_updatedData;
			_sync_updatedData = null;
		}
	}
}
