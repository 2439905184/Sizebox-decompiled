using System;
using System.Collections;
using System.Runtime.CompilerServices;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.Events;

public abstract class Micro : Humanoid, ICrushable, IDamagable, IGameObject
{
	private enum StuckState
	{
		Alive = 0,
		Stuck = 1,
		Peeling = 2,
		Settling = 3,
		Complete = 4
	}

	[SerializeField]
	private GameObject bloodPrefab;

	private static float _bodyLandingSettleDuration = 1f;

	private static float _stuckCooldown = 0.5f;

	private Gravity _gravity;

	private CapsuleCollider _capsule;

	private float _peelOffTime;

	private float _stuckTime;

	private float _bloodTime;

	private BipedRagdollReferences _ragDollRefs;

	private Rigidbody _ragDollRigidbody;

	private Coroutine _ragDollUpdater;

	public GiantessBone parentBone;

	public GiantessBoneParenting.CrushCollection CrushData;

	private StuckState _stuckState;

	private EntityBase _currentParentEntity;

	private Transform _currentParentTransform;

	public override EntityType EntityType
	{
		get
		{
			return EntityType.MICRO;
		}
	}

	public CapsuleCollider Capsule
	{
		get
		{
			return _capsule;
		}
	}

	public new Gravity Gravity
	{
		get
		{
			return _gravity;
		}
	}

	public bool isCrushed { get; private set; }

	public bool RagDollEnabled { get; private set; }

	private bool CanRagDoll
	{
		get
		{
			Transform[] ragdollTransforms = _ragDollRefs.GetRagdollTransforms();
			uint num = 0u;
			Transform[] array = ragdollTransforms;
			for (int i = 0; i < array.Length; i++)
			{
				if ((bool)array[i])
				{
					num++;
				}
			}
			switch (num)
			{
			case 15u:
				return !_ragDollRefs.chest;
			case 16u:
				return true;
			default:
				return false;
			}
		}
	}

	protected override void Awake()
	{
		isMicro = true;
		ObjectManager.UpdateMicroSpeed(this);
		_gravity = base.gameObject.GetComponent<Gravity>();
		if (_gravity == null)
		{
			_gravity = base.gameObject.AddComponent<Gravity>();
		}
		_capsule = GetComponent<CapsuleCollider>();
		base.Awake();
	}

	protected override void InitializeModel(GameObject modelPrefab)
	{
		base.InitializeModel(modelPrefab);
		base.gameObject.SetLayerRecursively(Layers.microLayer);
		base.model.transform.localScale = Vector3.one * (2f / meshHeight);
		meshHeight = 2f;
		if ((bool)base.Animator)
		{
			_ragDollRefs = new BipedRagdollReferences
			{
				root = base.transform,
				head = base.Animator.GetBoneTransform(HumanBodyBones.Head),
				chest = base.Animator.GetBoneTransform(HumanBodyBones.Chest),
				spine = base.Animator.GetBoneTransform(HumanBodyBones.Spine),
				hips = base.Animator.GetBoneTransform(HumanBodyBones.Hips),
				leftUpperArm = base.Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm),
				leftLowerArm = base.Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm),
				leftHand = base.Animator.GetBoneTransform(HumanBodyBones.LeftHand),
				rightUpperArm = base.Animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
				rightLowerArm = base.Animator.GetBoneTransform(HumanBodyBones.RightLowerArm),
				rightHand = base.Animator.GetBoneTransform(HumanBodyBones.RightHand),
				leftUpperLeg = base.Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
				leftLowerLeg = base.Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
				leftFoot = base.Animator.GetBoneTransform(HumanBodyBones.LeftFoot),
				rightUpperLeg = base.Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
				rightLowerLeg = base.Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
				rightFoot = base.Animator.GetBoneTransform(HumanBodyBones.RightFoot)
			};
		}
	}

	protected override void FinishInitialization()
	{
		base.FinishInitialization();
		MicroManager.Instance.AddMicro(this);
	}

	public bool EnableRagDoll()
	{
		if (RagDollEnabled || !GlobalPreferences.RagDollEnabled.value || !CanRagDoll)
		{
			return false;
		}
		SetColliderTrigger(true);
		base.Animator.enabled = false;
		bool priorAiState = ai.IsEnabled();
		ai.DisableAI();
		BipedRagdollCreator.Options options = default(BipedRagdollCreator.Options);
		options.torsoColliders = RagdollCreator.ColliderType.Capsule;
		options.armColliders = RagdollCreator.ColliderType.Capsule;
		options.legColliders = RagdollCreator.ColliderType.Capsule;
		options.colliderLengthOverlap = -0.25f;
		options.joints = RagdollCreator.JointType.Character;
		options.weight = base.Rigidbody.mass;
		BipedRagdollCreator.Options options2 = options;
		BipedRagdollCreator.Create(_ragDollRefs, options2);
		Transform boneTransform = base.Animator.GetBoneTransform(HumanBodyBones.Hips);
		if (!boneTransform)
		{
			return false;
		}
		Vector3 position = boneTransform.position;
		base.transform.position = position;
		boneTransform.position = position;
		ConfigurableJoint configurableJoint = boneTransform.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		configurableJoint.connectedBody = base.Rigidbody;
		base.Rigidbody.constraints = RigidbodyConstraints.None;
		base.Rigidbody.mass = 0.0001f;
		boneTransform.GetComponent<CapsuleCollider>().radius *= 0.5f;
		_ragDollRigidbody = base.model.GetComponentInChildren<Rigidbody>();
		_ragDollUpdater = StartCoroutine(RagDollUpdate(priorAiState));
		RagDollEnabled = true;
		ChangeParent(null);
		return true;
	}

	public void DisableRagDoll()
	{
		BipedRagdollCreator.ClearBipedRagdoll(_ragDollRefs);
		SetColliderTrigger(false);
		_ragDollRigidbody = null;
		if (_ragDollUpdater != null)
		{
			StopCoroutine(_ragDollUpdater);
		}
		_ragDollUpdater = null;
		base.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		UpdateMass();
		RagDollEnabled = false;
		if (isPlayer)
		{
			health.Increase(health.MaxValue);
			Dead = false;
		}
		if (Dead)
		{
			base.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			base.Rigidbody.isKinematic = true;
		}
		else
		{
			base.Animator.enabled = true;
		}
	}

	private void SetColliderTrigger(bool value)
	{
		if (ColliderList == null)
		{
			FindColliders();
		}
		Collider[] colliderList = ColliderList;
		for (int i = 0; i < colliderList.Length; i++)
		{
			colliderList[i].isTrigger = value;
		}
	}

	private IEnumerator RagDollUpdate(bool priorAiState)
	{
		Transform t = base.transform;
		float getUpTimer = Time.time + 4.5f;
		while (true)
		{
			Vector3 lastPosition = t.position;
			yield return new WaitForSeconds(0.5f);
			if ((lastPosition - t.position).sqrMagnitude > Mathf.Pow(0.5f * base.AccurateScale, 2f))
			{
				getUpTimer = Time.time + 4.5f;
			}
			else if (Time.time > getUpTimer)
			{
				break;
			}
		}
		DisableRagDoll();
		if (!Dead && priorAiState)
		{
			ai.EnableAI();
		}
	}

	protected override void OnDestroy()
	{
		ClearParentBoneScript();
		MicroManager.Instance.RemoveMicro(base.id);
		ChangeParentOnDestroy();
		base.OnDestroy();
	}

	protected virtual void Crush(Collision collisionData = null, EntityBase entity = null, Collider crushingCollider = null)
	{
		if ((!GlobalPreferences.CrushNpcEnabled.value && !isPlayer) || (!GlobalPreferences.CrushPlayerEnabled.value && isPlayer) || (entity != null && ((!GlobalPreferences.PlayerCrushingEnabled.value && entity.isPlayer) || (!GlobalPreferences.NpcMicroCrushingEnabled.value && !entity.isPlayer && entity.isMicro) || (!GlobalPreferences.NpcGtsCrushingEnabled.value && !entity.isPlayer && entity.isGiantess))))
		{
			return;
		}
		if (!isCrushed)
		{
			EventManager.SendEvent(new CrushEvent(this, entity));
			SoundManager.Instance.PlayCrushed(base.transform.position, Scale);
			isCrushed = true;
			ai.behaviorController.StopAllBehaviors();
			ai.DisableAI();
			ai.actionController.ClearAll();
			base.Animator.SetRuntimeController(IOManager.Instance.microAnimatorController);
			base.Animator.Play("Fall");
			Transform obj = base.transform;
			Vector3 localScale = obj.localScale;
			localScale = new Vector3(localScale.x, localScale.y / GlobalPreferences.CrushFlatness.value, localScale.z);
			obj.localScale = localScale;
			MicroLookAtController component = GetComponent<MicroLookAtController>();
			if (component != null && component.enabled)
			{
				component.LookAt(null);
			}
		}
		if (!_capsule.isTrigger)
		{
			_capsule.isTrigger = true;
			_capsule.direction = 2;
			_capsule.center = Vector3.zero;
			base.Rigidbody.velocity = Vector3.zero;
			_gravity.enabled = false;
			PlaceSelfBlood();
		}
		if (GlobalPreferences.CrushStick.value && (bool)entity && entity.isGiantess && collisionData != null)
		{
			GameObject gameObject = PlaceGtsBlood(collisionData, crushingCollider);
			if ((bool)crushingCollider)
			{
				parentBone = crushingCollider.gameObject.GetComponent<GiantessBone>();
				float num = ((Math.Abs(GameController.macroSpeed) < float.Epsilon) ? 1f : GameController.macroSpeed);
				if (GlobalPreferences.CrushStickDuration.value > 0f && UnityEngine.Random.value < GlobalPreferences.CrushStickChance.value && _stuckTime + _stuckCooldown / num < Time.time)
				{
					StartCoroutine(StuckRoutine(collisionData, crushingCollider, gameObject));
					return;
				}
				if (gameObject != null)
				{
					parentBone.boneParentingScript.BeginParentingStuckObject(gameObject.transform, null, gameObject);
				}
			}
		}
		_stuckTime = Time.time;
		CancelInvoke("DestroyMe");
		Invoke("DestroyMe", GlobalPreferences.BodyDuration.value);
	}

	protected override void Kill()
	{
		if (!Dead)
		{
			if (!isCrushed)
			{
				EnableRagDoll();
			}
			base.Kill();
		}
	}

	public void DestroyMe()
	{
		if (!isPlayer)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void PlaceSelfBlood()
	{
		RaycastHit hitInfo;
		if (GlobalPreferences.BloodEnabled.value && !(_bloodTime + 1f > Time.time) && Physics.Raycast(base.transform.position + Vector3.up * Scale, Vector3.down, out hitInfo, Scale * 2f, Layers.pathfindingMask))
		{
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, hitInfo.normal), hitInfo.normal) * Quaternion.AngleAxis(UnityEngine.Random.value * 360f, Vector3.up);
			GameObject obj = UnityEngine.Object.Instantiate(bloodPrefab, hitInfo.point + hitInfo.normal * (Scale * 0.02f), rotation);
			obj.transform.localScale *= base.transform.lossyScale.y;
			UnityEngine.Object.Destroy(obj, GlobalPreferences.BloodDuration.value);
			_bloodTime = Time.time;
		}
	}

	private GameObject PlaceGtsBlood(Collision collisionData, Collider crushingCollider)
	{
		if (!GlobalPreferences.BloodEnabled.value || _bloodTime + 1f > Time.time)
		{
			return null;
		}
		Vector3 position;
		Quaternion rotation;
		if (!GetCrushPositioning(collisionData, out position, out rotation, crushingCollider))
		{
			return null;
		}
		rotation *= Quaternion.AngleAxis(UnityEngine.Random.value * 360f, Vector3.up);
		GameObject gameObject = UnityEngine.Object.Instantiate(bloodPrefab, position, rotation, base.transform);
		float value = GlobalPreferences.BloodSize.value;
		gameObject.transform.localScale = new Vector3(value, value, value);
		if (BloodBounds.IsOddlyPlaced(gameObject.transform, crushingCollider.GetComponent<Collider>()))
		{
			UnityEngine.Object.Destroy(gameObject, 0f);
			return null;
		}
		UnityEngine.Object.Destroy(gameObject, GlobalPreferences.BloodDuration.value);
		_bloodTime = Time.time;
		gameObject.transform.parent = crushingCollider.transform;
		crushingCollider.gameObject.GetComponentInParent<Giantess>().AddBlood(gameObject);
		return gameObject;
	}

	private bool GetCrushPositioning(Collision collisionData, out Vector3 position, out Quaternion rotation, Collider crushingCollider)
	{
		ContactPoint contact = collisionData.GetContact(0);
		Vector3 point = contact.point;
		Vector3 normal = contact.normal;
		float num = 2f * crushingCollider.bounds.extents.magnitude;
		point -= num * normal;
		Ray ray = new Ray(point, normal);
		RaycastHit hitInfo;
		if (!crushingCollider.Raycast(ray, out hitInfo, 2f * num))
		{
			position = Vector3.zero;
			rotation = Quaternion.identity;
			return false;
		}
		position = hitInfo.point + 0.001f * hitInfo.normal;
		rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, hitInfo.normal), hitInfo.normal);
		return true;
	}

	private float GetRandomStickDuration()
	{
		float value = GlobalPreferences.CrushStickDuration.value;
		float num = 90f / (1f - value) + 2f * value - 90f;
		float value2 = UnityEngine.Random.value;
		return num * (Mathf.Pow(2.1f * value2 - 0.84f, 3f) + value2 / 2f + 0.75f);
	}

	private IEnumerator StuckRoutine(Collision collisionData, Collider crushingCollider, GameObject gtsBlood)
	{
		Vector3 position;
		Quaternion rotation;
		if (GetCrushPositioning(collisionData, out position, out rotation, crushingCollider))
		{
			base.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			Transform obj = base.transform;
			obj.position = position;
			obj.rotation = rotation;
			ChangeParent(crushingCollider.transform);
			parentBone.boneParentingScript.BeginParentingStuckObject(base.transform, this, gtsBlood);
			_stuckState = StuckState.Stuck;
			CancelInvoke("DestroyMe");
			if (Math.Abs(GlobalPreferences.CrushStickDuration.value - 1f) < float.Epsilon)
			{
				yield break;
			}
			_peelOffTime = Time.time + GetRandomStickDuration();
			_stuckTime = Time.time;
			yield return new WaitUntil(_003CStuckRoutine_003Eb__46_0);
			float distance = 1000f * collisionData.gameObject.transform.lossyScale.y;
			Vector3 myOffset = new Vector3(0f, distance, 0f);
			while (Physics.Raycast(base.transform.position + myOffset, Vector3.down, distance, Layers.pathfindingMask))
			{
				yield return new WaitForSeconds((GameController.macroSpeed < float.Epsilon) ? 10f : (0.1f / GameController.macroSpeed));
			}
		}
		_stuckTime = Time.time;
		if (_stuckState == StuckState.Stuck)
		{
			_stuckState = StuckState.Peeling;
			ChangeParent(null);
			base.Animator.applyRootMotion = false;
			_gravity.enabled = true;
			base.Rigidbody.constraints = RigidbodyConstraints.None;
			base.Rigidbody.isKinematic = false;
			base.Rigidbody.angularDrag = 1f;
			base.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			_capsule.isTrigger = false;
			EnableRagDoll();
			Invoke("DestroyMe", GlobalPreferences.BodyDuration.value);
		}
	}

	private IEnumerator StuckLandingRoutine()
	{
		_stuckState = StuckState.Settling;
		base.Rigidbody.angularDrag = 15f;
		yield return new WaitForSeconds(_bodyLandingSettleDuration);
		EndSettling();
	}

	private void EndSettling()
	{
		if (_stuckState == StuckState.Settling)
		{
			_stuckState = StuckState.Complete;
			base.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			_stuckTime = Time.time;
		}
	}

	public override bool IsTargetAble()
	{
		if (!Dead && !IsStuck())
		{
			return !isCrushed;
		}
		return false;
	}

	public bool IsStuck()
	{
		return _stuckState == StuckState.Stuck;
	}

	public virtual void StandUp()
	{
		if (isCrushed)
		{
			base.Animator.SetRuntimeController(IOManager.Instance.playerAnimatorController);
			base.Animator.Play("wakeup");
			base.Animator.applyRootMotion = true;
			base.Rigidbody.isKinematic = false;
			base.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			base.Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			if (!IsStuck())
			{
				Transform transform = base.transform;
				transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
			}
			Revive();
			isCrushed = false;
			_stuckState = StuckState.Alive;
			_capsule.isTrigger = false;
			_capsule.direction = 1;
			_capsule.center = Vector3.up * (_capsule.height * 0.5f);
			_gravity.enabled = true;
		}
	}

	public override bool CanDecide()
	{
		if (!Dead)
		{
			return _stuckState == StuckState.Alive;
		}
		return false;
	}

	private bool IsOnGround()
	{
		return IsOnSurface(Layers.pathfindingMask);
	}

	public bool IsOnSurface(LayerMask layer)
	{
		float radius = 0.25f * base.AccurateScale;
		return Physics.CheckSphere(base.transform.position, radius, layer, QueryTriggerInteraction.Ignore);
	}

	public bool IsOnGiantess()
	{
		return IsOnLayer(Layers.gtsBodyMask);
	}

	private bool IsOnLayer(LayerMask layerMask)
	{
		Vector3 p;
		Vector3 p2;
		float r;
		PhysicsHelper.ProcessCapsule(_capsule, out p, out p2, out r);
		float num = _capsule.bounds.size.y - base.Height * 0.1f;
		p.y += num;
		p2.y += num;
		RaycastHit hitInfo;
		return Physics.CapsuleCast(p, p2, r, Vector3.down, out hitInfo, num + base.Height * 0.1f, layerMask);
	}

	public void OnStep(AnimationEvent e)
	{
	}

	public bool TryToCrush(float mass, Vector3 velocity, Collision collisionData = null, EntityBase crusher = null, Collider crushingCollider = null)
	{
		if (!base.Initialized)
		{
			return false;
		}
		if (((!GlobalPreferences.CrushNpcEnabled.value || isPlayer) && (!GlobalPreferences.CrushPlayerEnabled.value || !isPlayer || IsOnSurface(Layers.gtsBodyMask) || !IsOnGround())) || _currentParentEntity == crusher)
		{
			return false;
		}
		if (Dead)
		{
			return false;
		}
		if (!crusher || collisionData == null)
		{
			return false;
		}
		if ((bool)crusher && crusher.isPlayer && crusher.isMicro && crusher.transform.lossyScale.x < base.transform.lossyScale.x * 2f)
		{
			return false;
		}
		if (crusher.isGiantess && !crusher.isPlayer && GlobalPreferences.MicroDurability.value < 0.01f)
		{
			Crush(collisionData, crusher, crushingCollider);
			return true;
		}
		float magnitude = velocity.magnitude;
		float num = mass * magnitude;
		float num2 = CalculateMass();
		float num3 = GlobalPreferences.MicroDurability.value * 2f;
		float num4 = 20f * num2 * Mathf.Pow(8f, base.AccurateScale - 1f) * num3;
		bool flag = num > num4;
		if (!GlobalPreferences.RagDollEnabled.value)
		{
			if (flag)
			{
				Crush(collisionData, crusher, crushingCollider);
			}
			return flag;
		}
		Vector3 point = collisionData.GetContact(0).point;
		float num5 = Mathf.Abs(velocity.y);
		if (velocity.y < 0f && num5 > Mathf.Abs(velocity.x) && num5 > Mathf.Abs(velocity.z) && flag)
		{
			Crush(collisionData, crusher, crushingCollider);
			return true;
		}
		if (magnitude < 5f * base.AccurateScale)
		{
			return false;
		}
		float num6 = 10f * (num / num4) - 10f;
		if (RagDollEnabled)
		{
			num6 /= 4f;
		}
		Damage(num6);
		if (num6 > 0f && EnableRagDoll())
		{
			Vector3 b = base.transform.position - point;
			b.y = 0f;
			b = Vector3.Lerp(velocity, b, 0.5f);
			Quaternion quaternion = Quaternion.LookRotation(b);
			_ragDollRigidbody.AddForce(quaternion * Vector3.forward * (magnitude * 0.25f * num2), ForceMode.Impulse);
		}
		return health.IsAtMin;
	}

	public bool TryToCrush(Vector3 force, EntityBase crusher = null)
	{
		float num = GlobalPreferences.MicroDurability.value * 2f;
		float num2 = 20f * CalculateMass() * Mathf.Pow(8f, base.AccurateScale - 1f) * num;
		bool num3 = force.magnitude > num2;
		if (num3)
		{
			Crush(null, crusher);
		}
		return num3;
	}

	public void Damage(float amount)
	{
		health.Decrease(amount);
		if (health.IsAtMin)
		{
			Kill();
		}
	}

	private void OnCollisionEnter(Collision collisionData)
	{
		HandleContact(collisionData);
		Giantess componentInParent = collisionData.gameObject.GetComponentInParent<Giantess>();
		if ((bool)componentInParent)
		{
			HandleGiantessContact(collisionData, componentInParent);
		}
	}

	private void HandleContact(Collision collisionData)
	{
		int layer = collisionData.gameObject.layer;
		bool flag = layer == Layers.playerLayer;
		bool flag2 = layer == Layers.microLayer;
		bool flag3 = layer == Layers.gtsBodyLayer;
		if (_stuckState == StuckState.Peeling)
		{
			if (flag2)
			{
				MicroNpc component = collisionData.gameObject.GetComponent<MicroNpc>();
				if (component != null && component._stuckState == StuckState.Settling)
				{
					component.EndSettling();
				}
			}
			else if (!flag3 && !flag)
			{
				StartCoroutine(StuckLandingRoutine());
			}
		}
		if (layer != Layers.mapLayer && layer != Layers.defaultLayer && layer != Layers.buildingLayer)
		{
			return;
		}
		if (_stuckState == StuckState.Stuck)
		{
			PlaceSelfBlood();
			float value = GlobalPreferences.CrushStickChance.value;
			if (!(GlobalPreferences.CrushStickDuration.value > 0.99f))
			{
				float a = Time.time + GetRandomStickDuration();
				float b = _stuckTime + GetRandomStickDuration();
				float num = Mathf.Min(a, b);
				float num2 = Mathf.Max(a, b);
				_peelOffTime = value * num2 + (1f - value) * num;
				Vector3 position;
				Quaternion rotation;
				if (_peelOffTime < Time.time && GetCrushPositioning(collisionData, out position, out rotation, collisionData.collider))
				{
					base.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
					Transform obj = base.transform;
					obj.position = position;
					obj.rotation = rotation;
					_stuckState = StuckState.Complete;
					Invoke("DestroyMe", GlobalPreferences.BodyDuration.value);
				}
			}
		}
		else
		{
			ChangeParent(null);
		}
	}

	private void HandleGiantessContact(Collision collisionData, Giantess giantess)
	{
		if (!IsOnGround() && !isCrushed && !RagDollEnabled && !giantess.IsPosed)
		{
			ChangeParent(collisionData.transform);
		}
	}

	private void ClearParentBoneScript()
	{
		if (CrushData != null)
		{
			CrushData.micro = null;
		}
	}

	public void ChangeParent(Transform newParent)
	{
		ClearParentBoneScript();
		if ((bool)_currentParentEntity)
		{
			EntityBase currentParentEntity = _currentParentEntity;
			currentParentEntity.SizeChanging = (UnityAction<float, float>)Delegate.Remove(currentParentEntity.SizeChanging, new UnityAction<float, float>(OnGiantessParentScale));
		}
		if ((bool)newParent)
		{
			_currentParentEntity = newParent.GetComponentInParent<EntityBase>();
			if ((bool)_currentParentEntity)
			{
				if (_currentParentEntity.CanSupportChild(this))
				{
					EntityBase currentParentEntity2 = _currentParentEntity;
					currentParentEntity2.SizeChanging = (UnityAction<float, float>)Delegate.Combine(currentParentEntity2.SizeChanging, new UnityAction<float, float>(OnGiantessParentScale));
				}
				else
				{
					_currentParentEntity = null;
				}
			}
		}
		else
		{
			_currentParentEntity = null;
		}
		base.transform.SetParent(newParent, true);
		if (isPlayer)
		{
			LocalClient.Instance.Player.transform.parent = newParent;
		}
		_currentParentTransform = newParent;
	}

	private void ChangeParentOnDestroy()
	{
		if ((bool)_currentParentEntity)
		{
			EntityBase currentParentEntity = _currentParentEntity;
			currentParentEntity.SizeChanging = (UnityAction<float, float>)Delegate.Remove(currentParentEntity.SizeChanging, new UnityAction<float, float>(OnGiantessParentScale));
		}
	}

	private void OnGiantessParentScale(float oldScale, float newScale)
	{
		Transform transform = base.transform;
		if (_currentParentTransform != transform.parent)
		{
			ChangeParent(transform.parent);
		}
		else if ((bool)_currentParentEntity && !_currentParentEntity.CanSupportChild(this))
		{
			ChangeParent(null);
		}
		else
		{
			transform.localScale *= oldScale / newScale;
		}
	}

	protected override float ClampScale(float scale)
	{
		return Mathf.Clamp(scale, MapSettingInternal.minPlayerSize, MapSettingInternal.maxPlayerSize);
	}

	public override void ChangeVerticalOffset(float newOffset)
	{
		_gravity.enabled = !(newOffset < 1f);
		base.ChangeVerticalOffset(newOffset);
	}

	[SpecialName]
	GameObject IGameObject.get_gameObject()
	{
		return base.gameObject;
	}

	[SpecialName]
	Transform IGameObject.get_transform()
	{
		return base.transform;
	}

	[CompilerGenerated]
	private bool _003CStuckRoutine_003Eb__46_0()
	{
		return Time.time > _peelOffTime;
	}
}
