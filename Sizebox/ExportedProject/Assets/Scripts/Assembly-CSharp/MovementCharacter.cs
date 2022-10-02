using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SteeringBehaviors;
using UnityEngine;
using UnityEngine.Serialization;

public class MovementCharacter : EntityComponent
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<SteerBehavior> _003C_003E9__46_0;

		internal bool _003CLateUpdate_003Eb__46_0(SteerBehavior x)
		{
			return x == null;
		}
	}

	[Header("Required References")]
	[SerializeField]
	private ObstacleDetector obstacleDetector;

	[SerializeField]
	private AnimationManager anim;

	[FormerlySerializedAs("rbody")]
	[SerializeField]
	private Rigidbody characterRigidbody;

	public float tileWidth = 0.1f;

	public float speedModifier = 1f;

	public float currentSpeedDivider = 1f;

	public float maxSpeedBase = 0.8f;

	public float maxAccel = 0.5f;

	public float maxRotation = 100f;

	public float maxAngularAccel = 1000f;

	public float rotation;

	public Vector3 velocity;

	private SteeringOutput _steering;

	public bool move;

	private List<SteerBehavior> _steeringBehaviors;

	private bool _stop;

	private Transform _myTransform;

	private int _behaviorCount;

	private bool _overrideDirection;

	private Vector3 _overrideVector;

	private SteerBehavior _look;

	private SteerBehavior _avoidWall;

	private SteerBehavior _wander;

	private Vector3 _forward;

	private Vector3 _direction;

	private Vector3 _upVector;

	public WalkController walkController;

	public EntityBase Entity { get; private set; }

	public float MaxSpeed
	{
		get
		{
			return maxSpeedBase * scale * speedModifier;
		}
	}

	public float MaxAccel
	{
		get
		{
			return maxAccel * scale * speedModifier;
		}
	}

	public float MaxRotation
	{
		get
		{
			return maxRotation * speedModifier;
		}
	}

	public float MaxAngularAccel
	{
		get
		{
			return maxAngularAccel * speedModifier;
		}
	}

	public float orientation
	{
		get
		{
			float num = characterRigidbody.rotation.eulerAngles.y;
			if (num < 0f)
			{
				num += 360f;
			}
			else if (num > 360f)
			{
				num -= 360f;
			}
			return num;
		}
	}

	public float scale
	{
		get
		{
			return Entity.AccurateScale;
		}
	}

	public override void Initialize(EntityBase entity)
	{
		Entity = entity;
		velocity = Vector3.zero;
		_steering = new SteeringOutput();
		_steeringBehaviors = new List<SteerBehavior>();
		_myTransform = base.transform;
		_upVector = Vector3.up;
		base.initialized = true;
	}

	public void RemoveBehaviour(SteerBehavior behavior)
	{
		_steeringBehaviors.Remove(behavior);
	}

	private void FixedUpdate()
	{
		if (base.initialized && move && _behaviorCount != 0)
		{
			_forward = (_overrideDirection ? _overrideVector : _myTransform.forward);
			if (rotation != 0f)
			{
				float num = orientation;
				num += rotation * Time.deltaTime;
				characterRigidbody.rotation = Quaternion.AngleAxis(num, _upVector);
			}
			float num2 = _direction.z * Time.deltaTime;
			if (_overrideDirection)
			{
				num2 = Mathf.Abs(num2);
			}
			if (num2 != 0f)
			{
				Vector3 forward = _forward;
				forward.x *= num2;
				forward.y *= num2;
				forward.z *= num2;
				Vector3 position = characterRigidbody.position;
				forward.x += position.x;
				forward.y += position.y;
				forward.z += position.z;
				characterRigidbody.position = forward;
			}
			if (_stop)
			{
				Stop();
			}
		}
	}

	private void LateUpdate()
	{
		if (base.initialized && move)
		{
			_steeringBehaviors.RemoveAll(_003C_003Ec._003C_003E9__46_0 ?? (_003C_003Ec._003C_003E9__46_0 = _003C_003Ec._003C_003E9._003CLateUpdate_003Eb__46_0));
			_behaviorCount = _steeringBehaviors.Count;
			if (_behaviorCount != 0)
			{
				UpdateMovementVariables();
			}
		}
	}

	private void UpdateMovementVariables()
	{
		float deltaTime = Time.deltaTime;
		float num = MaxSpeed / currentSpeedDivider;
		_steering.linear.x = 0f;
		_steering.linear.y = 0f;
		_steering.linear.z = 0f;
		_steering.angular = 0f;
		foreach (SteerBehavior steeringBehavior in _steeringBehaviors)
		{
			SteeringOutput steeringOutput;
			if (steeringBehavior.GetSteering(out steeringOutput) && steeringOutput != null)
			{
				if (steeringOutput.overrideUpdateReset)
				{
					_steering.linear.x += steeringOutput.linear.x;
					_steering.linear.z += steeringOutput.linear.z;
					_steering.angular = 0f;
					_overrideDirection = true;
					_overrideVector = _steering.linear;
					break;
				}
				float weight = steeringBehavior.weight;
				_steering.linear.x += steeringOutput.linear.x * weight;
				_steering.linear.z += steeringOutput.linear.z * weight;
				_steering.angular += steeringOutput.angular * weight;
				_overrideDirection = false;
			}
			else
			{
				steeringBehavior.agent.Stop();
				Debug.LogWarning(string.Concat("Unhandled state from '", steeringBehavior.GetType(), "'"));
			}
		}
		velocity.x += _steering.linear.x * deltaTime;
		velocity.z += _steering.linear.z * deltaTime;
		if (velocity.x * velocity.x + velocity.z * velocity.z > num * num)
		{
			velocity.Normalize();
			velocity.x *= num;
			velocity.z *= num;
		}
		if (_steering.linear.x == 0f && _steering.linear.z == 0f)
		{
			velocity.x = 0f;
			velocity.y = 0f;
			velocity.z = 0f;
		}
		rotation += _steering.angular * deltaTime;
		if (_steering.angular == 0f)
		{
			rotation = 0f;
		}
		_direction = _myTransform.InverseTransformDirection(velocity);
		if ((bool)anim)
		{
			if (num > 0f)
			{
				anim.SetWalkSpeed(_direction, _direction.z / (num * currentSpeedDivider));
			}
			else
			{
				anim.SetWalkSpeed(_direction, 0f);
			}
		}
	}

	private void ResetMovementVariables()
	{
		velocity = Vector3.zero;
		rotation = 0f;
	}

	public void SetSteering(SteeringOutput steering)
	{
		_steering = steering;
	}

	public void StartSeekBehavior(Kinematic target, float separation, float duration)
	{
		ResetMovementVariables();
		SteerBehavior item = new Seek(this, target, separation, duration);
		_steeringBehaviors.Add(item);
		AddSharedBehaviors();
		move = true;
	}

	public void StartFace(Kinematic target)
	{
		ResetMovementVariables();
		SteerBehavior item = new Face(this, target);
		_steeringBehaviors.Add(item);
		move = true;
	}

	public void StartFlee(Kinematic target)
	{
		ResetMovementVariables();
		SteerBehavior item = new Flee(this, target);
		_steeringBehaviors.Add(item);
		if (_wander == null)
		{
			_wander = new Wander(this);
		}
		_wander.weight = 0.6f;
		_steeringBehaviors.Add(_wander);
		AddSharedBehaviors();
		move = true;
	}

	public void Stop()
	{
		if ((bool)anim)
		{
			anim.UpdateAnimationSpeed();
		}
		_steeringBehaviors = new List<SteerBehavior>();
		move = false;
		_stop = false;
	}

	public void StartPursueBehavior(Kinematic target)
	{
		ResetMovementVariables();
		SteerBehavior item = new Pursue(this, target);
		_steeringBehaviors.Add(item);
		AddSharedBehaviors();
		move = true;
	}

	public void StartWanderBehavior()
	{
		ResetMovementVariables();
		if (_wander == null)
		{
			_wander = new Wander(this);
		}
		_wander.weight = 1f;
		_steeringBehaviors.Add(_wander);
		AddSharedBehaviors();
		move = true;
	}

	public SteerBehavior StartArriveBehavior(Kinematic target)
	{
		ResetMovementVariables();
		SteerBehavior steerBehavior = new WaypointArrive(this, target);
		_steeringBehaviors.Add(steerBehavior);
		AddSharedBehaviors();
		move = true;
		return steerBehavior;
	}

	public SteerBehavior StartMoveInDirectionBehavior(VectorKinematic target)
	{
		ResetMovementVariables();
		SteerBehavior steerBehavior = new MoveInDirection(this, target);
		_steeringBehaviors.Add(steerBehavior);
		AddSharedBehaviors();
		move = true;
		return steerBehavior;
	}

	private void AddSharedBehaviors()
	{
		ResetMovementVariables();
		if (_look == null)
		{
			_look = new LookWhereYouAreGoing(this);
		}
		_steeringBehaviors.Add(_look);
		if (_avoidWall == null)
		{
			_avoidWall = new AvoidWall(this, obstacleDetector);
		}
		_steeringBehaviors.Add(_avoidWall);
	}

	public void StartCustomBehavior()
	{
		ResetMovementVariables();
		walkController.Initialize(this);
		_steeringBehaviors.Add(walkController.customSteer);
		if (_look == null)
		{
			_look = new LookWhereYouAreGoing(this);
		}
		_steeringBehaviors.Add(_look);
		move = true;
	}
}
