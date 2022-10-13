using System.Collections;
using BulletXNA;
using BulletXNA.BulletCollision;
using BulletXNA.BulletDynamics;
using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet;

public class PhysicsWorld
{
	public const int DefaultFramePerSecond = 60;

	public const int DefaultResetFrameRate = 10;

	public const int DefaultLimitDeltaFrames = 2;

	private static object _cachedThreadQueue;

	private DefaultCollisionConfiguration _collisionConfig;

	private CollisionDispatcher _dispatcher;

	private IBroadphaseInterface _broadphase;

	private SequentialImpulseConstraintSolver _solver;

	private DiscreteDynamicsWorld _world;

	private CollisionShape _floorShape;

	private IMotionState _floorMotionState;

	private BulletXNA.BulletDynamics.RigidBody _floorRigidBody;

	private WorldProperty _worldProperty;

	private int _maxSubSteps;

	private float _subStep;

	private float _elapsedTime;

	private bool _multiThreading = true;

	private bool _isRunningThread;

	private float _gravityScale;

	private float _gravityNoise;

	private float _gravityNoiseTime;

	private float _gravityNoiseRate;

	private IndexedVector3 _gravityDirection = IndexedVector3.Zero;

	private IndexedVector3 _baseGravity = IndexedVector3.Zero;

	private ThreadQueueHandle _threadQueueHandle = default(ThreadQueueHandle);

	private ArrayList _physicsEntityList = new ArrayList();

	public bool isMultiThreading => _multiThreading;

	public DiscreteDynamicsWorld bulletWorld => _world;

	public bool Create(WorldProperty worldProperty)
	{
		Destroy();
		_worldProperty = worldProperty;
		if (_worldProperty == null)
		{
			_worldProperty = new WorldProperty();
		}
		if (_worldProperty.optimizeSettings)
		{
			_worldProperty.accurateStep = false;
		}
		if (_worldProperty.framePerSecond <= 0)
		{
			_worldProperty.framePerSecond = 60;
		}
		if (_worldProperty.resetFrameRate <= 0)
		{
			_worldProperty.resetFrameRate = 10;
		}
		if (_worldProperty.limitDeltaFrames <= 0)
		{
			_worldProperty.limitDeltaFrames = 2;
		}
		if (_worldProperty.optimizeSettings)
		{
			if (_worldProperty.framePerSecond > 60)
			{
				_worldProperty.framePerSecond = 60;
			}
			if (_worldProperty.axisSweepDistance <= 0f)
			{
				_worldProperty.axisSweepDistance = 1000f;
			}
		}
		_maxSubSteps = _worldProperty.framePerSecond;
		_subStep = 1f / (float)_worldProperty.framePerSecond;
		_collisionConfig = new DefaultCollisionConfiguration();
		_dispatcher = new CollisionDispatcher(_collisionConfig);
		if (_worldProperty.axisSweepDistance > 0f)
		{
			float axisSweepDistance = _worldProperty.axisSweepDistance;
			IndexedVector3 worldAabbMin = new IndexedVector3(0f - axisSweepDistance, 0f - axisSweepDistance, 0f - axisSweepDistance);
			IndexedVector3 worldAabbMax = -worldAabbMin;
			_broadphase = new AxisSweep3Internal(ref worldAabbMin, ref worldAabbMax, 65534, ushort.MaxValue, 16384, null, disableRaycastAccelerator: false);
		}
		else
		{
			_broadphase = new DbvtBroadphase();
		}
		_solver = new SequentialImpulseConstraintSolver();
		_world = new DiscreteDynamicsWorld(_dispatcher, _broadphase, _solver, _collisionConfig);
		_gravityScale = _worldProperty.gravityScale;
		_gravityNoise = _worldProperty.gravityNoise;
		_gravityDirection = _worldProperty.gravityDirection;
		IndexedVector3 gravity = new IndexedVector3(0f, -9.8f * _worldProperty.gravityScale, 0f);
		if (_worldProperty.gravityDirection != new Vector3(0f, -1f, 0f))
		{
			Vector3 gravityDirection = _worldProperty.gravityDirection;
			float magnitude = gravityDirection.magnitude;
			gravity = ((!(magnitude > float.Epsilon)) ? IndexedVector3.Zero : ((IndexedVector3)(gravityDirection * (1f / magnitude) * (9.8f * _worldProperty.gravityScale))));
		}
		_world.SetGravity(ref gravity);
		if (_world.GetSolverInfo() != null)
		{
			if (_worldProperty.worldSolverInfoNumIterations <= 0)
			{
				_world.GetSolverInfo().m_numIterations = 600 / _worldProperty.framePerSecond;
				if (_worldProperty.optimizeSettings)
				{
					_world.GetSolverInfo().m_numIterations /= 2;
				}
			}
			else
			{
				_world.GetSolverInfo().m_numIterations = _worldProperty.worldSolverInfoNumIterations;
			}
		}
		if (!_worldProperty.optimizeSettings)
		{
			_world.GetSolverInfo().m_splitImpulse = worldProperty.worldSolverInfoSplitImpulse;
		}
		if (_worldProperty.worldAddFloorPlane)
		{
			_floorShape = new StaticPlaneShape(new IndexedVector3(0f, 1f, 0f), 1f);
			_floorMotionState = new DefaultMotionState(new IndexedMatrix(IndexedBasisMatrix.Identity, new IndexedVector3(0f, -1f, 0f)), IndexedMatrix.Identity);
			RigidBodyConstructionInfo constructionInfo = new RigidBodyConstructionInfo(0f, _floorMotionState, _floorShape, IndexedVector3.Zero);
			_floorRigidBody = new BulletXNA.BulletDynamics.RigidBody(constructionInfo);
			_floorRigidBody.SetCollisionFlags(_floorRigidBody.GetCollisionFlags() | BulletXNA.BulletCollision.CollisionFlags.CF_KINEMATIC_OBJECT);
			_floorRigidBody.SetActivationState(ActivationState.DISABLE_DEACTIVATION);
			_world.AddRigidBody(_floorRigidBody, (CollisionFilterGroups)65535, (CollisionFilterGroups)65535);
		}
		_multiThreading = worldProperty.multiThreading;
		if (_multiThreading && _cachedThreadQueue == null && Global.bridge != null)
		{
			_cachedThreadQueue = Global.bridge.CreateCachedThreadQueue(1);
		}
		return true;
	}

	public void Destroy()
	{
		WaitEndThreading();
		while (_physicsEntityList.Count > 0)
		{
			int index = _physicsEntityList.Count - 1;
			PhysicsEntity physicsEntity = (PhysicsEntity)_physicsEntityList[index];
			_physicsEntityList.RemoveAt(index);
			physicsEntity.LeaveWorld();
		}
		if (_floorRigidBody != null)
		{
			if (_world != null)
			{
				_world.RemoveRigidBody(_floorRigidBody);
			}
			_floorRigidBody.SetUserPointer(null);
			_floorRigidBody.Cleanup();
			_floorRigidBody = null;
		}
		_floorMotionState = null;
		if (_floorShape != null)
		{
			_floorShape.Cleanup();
			_floorShape = null;
		}
		if (_world != null)
		{
			_world.Cleanup();
			_world = null;
		}
		if (_solver != null)
		{
			_solver.Cleanup();
			_solver = null;
		}
		if (_broadphase != null)
		{
			_broadphase.Cleanup();
			_broadphase = null;
		}
		if (_dispatcher != null)
		{
			_dispatcher.Cleanup();
			_dispatcher = null;
		}
		if (_collisionConfig != null)
		{
			_collisionConfig.Cleanup();
			_collisionConfig = null;
		}
	}

	public void SetGravity(float gravityScale, float gravityNoise, Vector3 gravityDirection)
	{
		if (_world == null)
		{
			return;
		}
		_gravityScale = gravityScale;
		_gravityNoise = gravityNoise;
		_gravityDirection = gravityDirection;
		if (gravityDirection != new Vector3(0f, -1f, 0f))
		{
			Vector3 vector = gravityDirection;
			float magnitude = vector.magnitude;
			if (magnitude > float.Epsilon)
			{
				_baseGravity = vector * (1f / magnitude) * (9.8f * gravityScale);
			}
			else
			{
				_baseGravity = IndexedVector3.Zero;
			}
		}
		else
		{
			_baseGravity = new IndexedVector3(0f, -9.8f * gravityScale, 0f);
		}
		IndexedVector3 gravity = _baseGravity + gravityNoise * gravityScale * _gravityNoiseRate * _gravityDirection;
		_world.SetGravity(ref gravity);
	}

	public void Update(float deltaTime, WorldUpdateProperty worldUpdateProperty)
	{
		WaitEndThreading();
		if (worldUpdateProperty != null)
		{
			SetGravity(worldUpdateProperty.gravityScale, worldUpdateProperty.gravityNoise, worldUpdateProperty.gravityDirection);
		}
		if (_gravityNoise != 0f)
		{
			_gravityNoiseTime += deltaTime;
			if (_gravityNoiseTime >= 0.2f)
			{
				_gravityNoiseTime = 0f;
				_gravityNoiseRate += (Random.Range(0f, 1f) - 0.5f) * 0.5f;
				_gravityNoiseRate = Mathf.Clamp01(_gravityNoiseRate);
				if (_world != null)
				{
					IndexedVector3 gravity = _baseGravity + _gravityNoise * _gravityScale * _gravityNoiseRate * _gravityDirection;
					_world.SetGravity(ref gravity);
				}
			}
		}
		float num = 0f;
		bool flag = false;
		for (int i = 0; i < _physicsEntityList.Count; i++)
		{
			num = Mathf.Max(((PhysicsEntity)_physicsEntityList[i])._GetResetWorldTime(), num);
			if (_multiThreading)
			{
				flag |= !((PhysicsEntity)_physicsEntityList[i])._isUpdateAtLeastOnce;
			}
		}
		if (num > 0f)
		{
			for (int j = 0; j < _physicsEntityList.Count; j++)
			{
				((PhysicsEntity)_physicsEntityList[j])._PreResetWorld();
			}
			for (int k = 0; k < _physicsEntityList.Count; k++)
			{
				((PhysicsEntity)_physicsEntityList[k])._StepResetWorld(0f);
			}
			float num2 = 0f;
			bool flag2 = false;
			float num3 = 1f / (float)_worldProperty.resetFrameRate;
			while (num2 < num && !flag2)
			{
				_Entity_Update(num3);
				num2 += num3;
				if (num2 > num)
				{
					num2 = num;
					flag2 = true;
				}
				for (int l = 0; l < _physicsEntityList.Count; l++)
				{
					((PhysicsEntity)_physicsEntityList[l])._StepResetWorld(num2);
				}
			}
			for (int m = 0; m < _physicsEntityList.Count; m++)
			{
				((PhysicsEntity)_physicsEntityList[m])._PostResetWorld();
			}
		}
		if (!_multiThreading || flag)
		{
			_InternalUpdate(deltaTime);
			_Entity_PrepareLateUpdate();
			if (_multiThreading)
			{
				for (int n = 0; n != _physicsEntityList.Count; n++)
				{
					((PhysicsEntity)_physicsEntityList[n])._isUpdateAtLeastOnce = true;
				}
			}
		}
		else
		{
			_Invoke(deltaTime);
		}
	}

	public void JoinWorld(PhysicsEntity physicsEntity)
	{
		if (physicsEntity == null)
		{
			Debug.LogError("");
			return;
		}
		if (physicsEntity.physicsWorld != null)
		{
			Debug.LogError("");
			return;
		}
		WaitEndThreading();
		physicsEntity._physicsWorld = this;
		if (!physicsEntity._JoinWorld())
		{
			Debug.LogError("");
			physicsEntity._physicsWorld = null;
		}
		else
		{
			_physicsEntityList.Add(physicsEntity);
		}
	}

	private void _Invoke(float deltaTime)
	{
		_isRunningThread = true;
		if (_cachedThreadQueue != null && Global.bridge != null)
		{
			_threadQueueHandle = Global.bridge.InvokeCachedThreadQueue(_cachedThreadQueue, delegate
			{
				_InternalUpdate(deltaTime);
			});
		}
	}

	private void _InternalUpdate(float deltaTime)
	{
		float num = _worldProperty.limitDeltaFrames;
		float num2 = num / (float)_worldProperty.framePerSecond;
		if (deltaTime >= num2)
		{
			deltaTime = num2;
		}
		_Entity_PreUpdate();
		bool flag = false;
		_elapsedTime += deltaTime;
		if (_subStep > 0f)
		{
			if (_worldProperty.accurateStep)
			{
				while (_elapsedTime >= _subStep)
				{
					flag = true;
					_Entity_Update(_subStep);
					_elapsedTime -= _subStep;
				}
			}
			else
			{
				float num3 = 0f;
				while (_elapsedTime >= _subStep)
				{
					num3 += _subStep;
					_elapsedTime -= _subStep;
				}
				if (num3 != 0f)
				{
					flag = true;
					_Entity_Update(num3);
				}
			}
		}
		if (!flag)
		{
			_Entity_NoUpdate();
		}
		_Entity_PostUpdate();
	}

	private void _Entity_PreUpdate()
	{
		for (int i = 0; i != _physicsEntityList.Count; i++)
		{
			((PhysicsEntity)_physicsEntityList[i])._PreUpdate();
		}
	}

	private void _Entity_Update(float deltaTime)
	{
		if (_world != null)
		{
			for (int i = 0; i != _physicsEntityList.Count; i++)
			{
				((PhysicsEntity)_physicsEntityList[i])._PreUpdateWorld(deltaTime);
			}
			int num = _world.StepSimulation(deltaTime, _maxSubSteps, _subStep);
			float deltaTime2 = ((num > 0) ? ((float)num * _subStep) : 0f);
			for (int j = 0; j != _physicsEntityList.Count; j++)
			{
				((PhysicsEntity)_physicsEntityList[j])._PostUpdateWorld(deltaTime2);
			}
		}
	}

	private void _Entity_NoUpdate()
	{
		for (int i = 0; i != _physicsEntityList.Count; i++)
		{
			((PhysicsEntity)_physicsEntityList[i])._NoUpdateWorld();
		}
	}

	private void _Entity_PostUpdate()
	{
		for (int i = 0; i != _physicsEntityList.Count; i++)
		{
			((PhysicsEntity)_physicsEntityList[i])._PostUpdate();
		}
	}

	private void _Entity_PrepareLateUpdate()
	{
		for (int i = 0; i != _physicsEntityList.Count; i++)
		{
			((PhysicsEntity)_physicsEntityList[i])._PrepareLateUpdate();
		}
	}

	public void WaitEndThreading()
	{
		if (_multiThreading && _isRunningThread)
		{
			_isRunningThread = false;
			if (Global.bridge != null)
			{
				Global.bridge.WaitEndCachedThreadQueue(_cachedThreadQueue, ref _threadQueueHandle);
			}
			_Entity_PrepareLateUpdate();
		}
	}

	public void _RemoveEntity(PhysicsEntity physicsEntity)
	{
		for (int i = 0; i != _physicsEntityList.Count; i++)
		{
			if ((PhysicsEntity)_physicsEntityList[i] == physicsEntity)
			{
				_physicsEntityList.RemoveAt(i);
				physicsEntity._physicsWorld = null;
				break;
			}
		}
	}
}
