using UnityEngine;

public class AnimationManager : EntityComponent
{
	private Humanoid _agent;

	public float transitionDuration = 0.08f;

	public Animator unityAnimator;

	private int _animationSpeedHash;

	private int _forwardHash;

	private int _rightHash;

	public float minSpeed = 0.1f;

	public float maxSpeed = 1f;

	private float _lastChange;

	public float speedMultiplier = 1f;

	private static IOManager _ioManager;

	private Giantess _giantess;

	public string nameAnimation { get; private set; }

	private void Awake()
	{
		_animationSpeedHash = Animator.StringToHash("animationSpeed");
		_forwardHash = Animator.StringToHash("forward");
		_rightHash = Animator.StringToHash("right");
		_ioManager = IOManager.Instance;
	}

	public override void Initialize(EntityBase entity)
	{
		_agent = entity as Humanoid;
		if (!_agent)
		{
			Debug.LogError("AnimationManager requires a humanoid entity.");
			return;
		}
		_giantess = _agent as Giantess;
		unityAnimator = _agent.Animator;
		base.initialized = true;
	}

	public static bool AnimationExists(string animation)
	{
		return _ioManager.AnimationControllers.ContainsKey(animation);
	}

	public void PlayAnimation(string animationName, bool poseMode = false, bool immediate = false)
	{
		if (!base.initialized)
		{
			return;
		}
		RuntimeAnimatorController value = null;
		if (animationName == null)
		{
			Debug.LogWarning("No animation name has been given");
			return;
		}
		if (!poseMode)
		{
			if (!_ioManager.AnimationControllers.TryGetValue(animationName, out value))
			{
				Debug.LogError("Animation \"" + animationName + "\" not found.");
				return;
			}
		}
		else if (!IOManager.Instance.Poses.Contains(animationName))
		{
			Debug.LogError("Pose \"" + animationName + "\" not found.");
			return;
		}
		_agent.SetPoseMode(poseMode);
		nameAnimation = animationName;
		if (poseMode)
		{
			unityAnimator.Play(animationName);
			if ((bool)_giantess)
			{
				_giantess.InvokeColliderUpdate();
			}
			return;
		}
		if (unityAnimator.runtimeAnimatorController != value)
		{
			Vector3 position = base.transform.position;
			unityAnimator.SetRuntimeController(value);
			base.transform.position = position;
		}
		UpdateAnimationSpeed();
		if (immediate)
		{
			unityAnimator.Play(animationName);
		}
		else if (Time.time < _lastChange + transitionDuration)
		{
			Invoke("CrossFade", transitionDuration);
		}
		else
		{
			CrossFade();
		}
	}

	public bool IsInPose()
	{
		if (!base.initialized)
		{
			return false;
		}
		return _agent.IsPosed;
	}

	public bool AnimationHasFinished()
	{
		if (!base.initialized)
		{
			return false;
		}
		if (GetAnimationProgress() > 1f)
		{
			return !unityAnimator.IsInTransition(0);
		}
		return false;
	}

	public float GetAnimationTime()
	{
		if (!base.initialized)
		{
			return 0f;
		}
		AnimatorStateInfo currentAnimatorStateInfo = unityAnimator.GetCurrentAnimatorStateInfo(0);
		return currentAnimatorStateInfo.normalizedTime * currentAnimatorStateInfo.length;
	}

	public float GetAnimationLength()
	{
		if (!base.initialized)
		{
			return 0f;
		}
		return unityAnimator.GetCurrentAnimatorStateInfo(0).length;
	}

	public float GetAnimationProgress()
	{
		if (!base.initialized)
		{
			return 0f;
		}
		return unityAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
	}

	public bool TransitionEnded()
	{
		if (!base.initialized)
		{
			return false;
		}
		return !unityAnimator.IsInTransition(0);
	}

	private void CrossFade()
	{
		unityAnimator.CrossFade(nameAnimation, transitionDuration);
		_lastChange = Time.time;
	}

	public void SetWalkSpeed(Vector3 direction, float speed)
	{
		if (base.initialized)
		{
			speed *= GetCurrentSpeed();
			ChangeInternalSpeed(speed);
		}
	}

	public void SetNewWalkSpeed(Vector3 direction)
	{
		if (base.initialized)
		{
			float num = direction.z;
			float num2 = direction.x;
			if (num < 0f)
			{
				num = 0f;
				num2 = Mathf.Sign(num2);
			}
			unityAnimator.SetFloat(_forwardHash, num);
			unityAnimator.SetFloat(_rightHash, num2);
		}
	}

	public void UpdateAnimationSpeed()
	{
		if (base.initialized && !_agent.IsPosed)
		{
			float currentSpeed = GetCurrentSpeed();
			ChangeInternalSpeed(currentSpeed);
			_agent.Movement.speedModifier = currentSpeed;
		}
	}

	private void ChangeInternalSpeed(float speed)
	{
		unityAnimator.SetFloat(_animationSpeedHash, speed);
	}

	public float GetCurrentSpeed()
	{
		if (!base.initialized)
		{
			return 0f;
		}
		float num = 1f;
		if (GlobalPreferences.SlowdownWithSize.value)
		{
			float num2 = Mathf.Log10(_agent.Height);
			num = 1f - num2 / 5f;
			num = Mathf.Clamp(num, minSpeed, maxSpeed);
		}
		return num * speedMultiplier * _agent.animationSpeed;
	}

	public void ChangeSpeed(float newSpeed)
	{
		if (base.initialized)
		{
			speedMultiplier = newSpeed;
			UpdateAnimationSpeed();
		}
	}
}
