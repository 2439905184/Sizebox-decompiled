using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[HelpURL("https://www.youtube.com/watch?v=LYusqeqHAUc")]
	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Puppet Master")]
	public class PuppetMaster : MonoBehaviour
	{
		[Serializable]
		public enum Mode
		{
			Active = 0,
			Kinematic = 1,
			Disabled = 2
		}

		public delegate void UpdateDelegate();

		public delegate void MuscleDelegate(Muscle muscle);

		[Serializable]
		public enum UpdateMode
		{
			Normal = 0,
			AnimatePhysics = 1,
			FixedUpdate = 2
		}

		[Serializable]
		public enum State
		{
			Alive = 0,
			Dead = 1,
			Frozen = 2
		}

		[Serializable]
		public struct StateSettings
		{
			[Tooltip("How much does it take to weigh out muscle weight to deadMuscleWeight?")]
			public float killDuration;

			[Tooltip("The muscle weight mlp while the puppet is Dead.")]
			public float deadMuscleWeight;

			[Tooltip("The muscle damper add while the puppet is Dead.")]
			public float deadMuscleDamper;

			[Tooltip("The max square velocity of the ragdoll bones for freezing the puppet.")]
			public float maxFreezeSqrVelocity;

			[Tooltip("If true, PuppetMaster, all it's behaviours and the ragdoll will be destroyed when the puppet is frozen.")]
			public bool freezePermanently;

			[Tooltip("If true, will enable angular limits when killing the puppet.")]
			public bool enableAngularLimitsOnKill;

			[Tooltip("If true, will enable internal collisions when killing the puppet.")]
			public bool enableInternalCollisionsOnKill;

			public static StateSettings Default
			{
				get
				{
					return new StateSettings(1f);
				}
			}

			public StateSettings(float killDuration, float deadMuscleWeight = 0.01f, float deadMuscleDamper = 2f, float maxFreezeSqrVelocity = 0.02f, bool freezePermanently = false, bool enableAngularLimitsOnKill = true, bool enableInternalCollisionsOnKill = true)
			{
				this.killDuration = killDuration;
				this.deadMuscleWeight = deadMuscleWeight;
				this.deadMuscleDamper = deadMuscleDamper;
				this.maxFreezeSqrVelocity = maxFreezeSqrVelocity;
				this.freezePermanently = freezePermanently;
				this.enableAngularLimitsOnKill = enableAngularLimitsOnKill;
				this.enableInternalCollisionsOnKill = enableInternalCollisionsOnKill;
			}
		}

		[Tooltip("Humanoid Config allows you to easily share PuppetMaster properties, including individual muscle props between Humanoid puppets.")]
		public PuppetMasterHumanoidConfig humanoidConfig;

		public Transform targetRoot;

		[LargeHeader("Simulation")]
		[Tooltip("Sets/sets the state of the puppet (Alive, Dead or Frozen). Frozen means the ragdoll will be deactivated once it comes to stop in dead state.")]
		public State state;

		[ContextMenuItem("Reset To Default", "ResetStateSettings")]
		[Tooltip("Settings for killing and freezing the puppet.")]
		public StateSettings stateSettings = StateSettings.Default;

		[Tooltip("Active mode means all muscles are active and the character is physically simulated. Kinematic mode sets rigidbody.isKinematic to true for all the muscles and simply updates their position/rotation to match the target's. Disabled mode disables the ragdoll. Switching modes is done by simply changing this value, blending in/out will be handled automatically by the PuppetMaster.")]
		public Mode mode;

		[Tooltip("The time of blending when switching from Active to Kinematic/Disabled or from Kinematic/Disabled to Active. Switching from Kinematic to Disabled or vice versa will be done instantly.")]
		public float blendTime = 0.1f;

		[Tooltip("If true, will fix the target character's Transforms to their default local positions and rotations in each update cycle to avoid drifting from additive reading-writing. Use this only if the target contains unanimated bones.")]
		public bool fixTargetTransforms = true;

		[Tooltip("Rigidbody.solverIterationCount for the muscles of this Puppet.")]
		public int solverIterationCount = 6;

		[Tooltip("If true, will draw the target's pose as green lines in the Scene view. This runs in the Editor only. If you wish to profile PuppetMaster, switch this off.")]
		public bool visualizeTargetPose = true;

		[LargeHeader("Master Weights")]
		[Tooltip("The weight of mapping the animated character to the ragdoll pose.")]
		[Range(0f, 1f)]
		public float mappingWeight = 1f;

		[Tooltip("The weight of pinning the muscles to the position of their animated targets using simple AddForce.")]
		[Range(0f, 1f)]
		public float pinWeight = 1f;

		[Tooltip("The normalized strength of the muscles.")]
		[Range(0f, 1f)]
		public float muscleWeight = 1f;

		[LargeHeader("Joint and Muscle Settings")]
		[Tooltip("The positionSpring of the ConfigurableJoints' Slerp Drive.")]
		public float muscleSpring = 100f;

		[Tooltip("The positionDamper of the ConfigurableJoints' Slerp Drive.")]
		public float muscleDamper;

		[Tooltip("Adjusts the slope of the pinWeight curve. Has effect only while interpolating pinWeight from 0 to 1 and back.")]
		[Range(1f, 8f)]
		public float pinPow = 4f;

		[Tooltip("Reduces pinning force the farther away the target is. Bigger value loosens the pinning, resulting in sloppier behaviour.")]
		[Range(0f, 100f)]
		public float pinDistanceFalloff = 5f;

		[Tooltip("When the target has animated bones between the muscle bones, the joint anchors need to be updated in every update cycle because the muscles' targets move relative to each other in position space. This gives much more accurate results, but is computationally expensive so consider leaving it off.")]
		public bool updateJointAnchors = true;

		[Tooltip("Enable this if any of the target's bones has translation animation.")]
		public bool supportTranslationAnimation;

		[Tooltip("Should the joints use angular limits? If the PuppetMaster fails to match the target's pose, it might be because the joint limits are too stiff and do not allow for such motion. Uncheck this to see if the limits are clamping the range of your puppet's animation. Since the joints are actuated, most PuppetMaster simulations will not actually require using joint limits at all.")]
		public bool angularLimits;

		[Tooltip("Should the muscles collide with each other? Consider leaving this off while the puppet is pinned for performance and better accuracy.  Since the joints are actuated, most PuppetMaster simulations will not actually require internal collisions at all.")]
		public bool internalCollisions;

		[LargeHeader("Individual Muscle Settings")]
		[Tooltip("The Muscles managed by this PuppetMaster.")]
		public Muscle[] muscles = new Muscle[0];

		public UpdateDelegate OnPostInitiate;

		public UpdateDelegate OnRead;

		public UpdateDelegate OnWrite;

		public UpdateDelegate OnPostLateUpdate;

		public UpdateDelegate OnFixTransforms;

		public UpdateDelegate OnHierarchyChanged;

		public MuscleDelegate OnMuscleRemoved;

		private Animator _targetAnimator;

		[HideInInspector]
		public List<SolverManager> solvers = new List<SolverManager>();

		private bool internalCollisionsEnabled = true;

		private bool angularLimitsEnabled = true;

		private bool fixedFrame;

		private int lastSolverIterationCount;

		private bool isLegacy;

		private bool animatorDisabled;

		private bool awakeFailed;

		private bool interpolated;

		private bool freezeFlag;

		private bool hasBeenDisabled;

		private bool hierarchyIsFlat;

		private bool teleport;

		private Vector3 teleportPosition;

		private Quaternion teleportRotation = Quaternion.identity;

		private bool teleportMoveToTarget;

		private Mode activeMode;

		private Mode lastMode;

		private float mappingBlend = 1f;

		public UpdateDelegate OnFreeze;

		public UpdateDelegate OnUnfreeze;

		public UpdateDelegate OnDeath;

		public UpdateDelegate OnResurrection;

		private State activeState;

		private State lastState;

		private bool angularLimitsEnabledOnKill;

		private bool internalCollisionsEnabledOnKill;

		private bool animationDisabledbyStates;

		[HideInInspector]
		public bool storeTargetMappedState = true;

		private Transform[] targetChildren = new Transform[0];

		private Vector3[] targetMappedPositions;

		private Quaternion[] targetMappedRotations;

		private Vector3[] targetSampledPositions;

		private Quaternion[] targetSampledRotations;

		private bool targetMappedStateStored;

		private bool targetMappedStateSampled;

		private bool sampleTargetMappedState;

		private bool hasProp;

		public Animator targetAnimator
		{
			get
			{
				if (_targetAnimator == null)
				{
					_targetAnimator = targetRoot.GetComponentInChildren<Animator>();
				}
				if (_targetAnimator == null && targetRoot.parent != null)
				{
					_targetAnimator = targetRoot.parent.GetComponentInChildren<Animator>();
				}
				return _targetAnimator;
			}
			set
			{
				_targetAnimator = value;
			}
		}

		public Animation targetAnimation { get; private set; }

		public BehaviourBase[] behaviours { get; private set; }

		public bool isActive
		{
			get
			{
				if (base.isActiveAndEnabled && initiated)
				{
					if (activeMode != 0)
					{
						return isBlending;
					}
					return true;
				}
				return false;
			}
		}

		public bool initiated { get; private set; }

		public UpdateMode updateMode
		{
			get
			{
				if (targetUpdateMode != AnimatorUpdateMode.AnimatePhysics)
				{
					return UpdateMode.Normal;
				}
				if (!isLegacy)
				{
					return UpdateMode.FixedUpdate;
				}
				return UpdateMode.AnimatePhysics;
			}
		}

		public bool controlsAnimator
		{
			get
			{
				if (base.isActiveAndEnabled && isActive && initiated)
				{
					return updateMode == UpdateMode.FixedUpdate;
				}
				return false;
			}
		}

		public bool isBlending
		{
			get
			{
				if (!isSwitchingMode)
				{
					return isSwitchingState;
				}
				return true;
			}
		}

		private AnimatorUpdateMode targetUpdateMode
		{
			get
			{
				if (targetAnimator != null)
				{
					return targetAnimator.updateMode;
				}
				if (targetAnimation != null)
				{
					if (!targetAnimation.animatePhysics)
					{
						return AnimatorUpdateMode.Normal;
					}
					return AnimatorUpdateMode.AnimatePhysics;
				}
				return AnimatorUpdateMode.Normal;
			}
		}

		public bool isSwitchingMode { get; private set; }

		public bool isSwitchingState
		{
			get
			{
				return activeState != state;
			}
		}

		public bool isKilling { get; private set; }

		public bool isAlive
		{
			get
			{
				return activeState == State.Alive;
			}
		}

		public bool isFrozen
		{
			get
			{
				return activeState == State.Frozen;
			}
		}

		[ContextMenu("User Manual (Setup)")]
		private void OpenUserManualSetup()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page4.html");
		}

		[ContextMenu("User Manual (Component)")]
		private void OpenUserManualComponent()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page5.html");
		}

		[ContextMenu("User Manual (Performance)")]
		private void OpenUserManualPerformance()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page8.html");
		}

		[ContextMenu("Scrpt Reference")]
		private void OpenScriptReference()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_puppet_master.html");
		}

		[ContextMenu("TUTORIAL VIDEO (SETUP)")]
		private void OpenSetupTutorial()
		{
			Application.OpenURL("https://www.youtube.com/watch?v=mIN9bxJgfOU&index=2&list=PLVxSIA1OaTOuE2SB9NUbckQ9r2hTg4mvL");
		}

		[ContextMenu("TUTORIAL VIDEO (COMPONENT)")]
		private void OpenComponentTutorial()
		{
			Application.OpenURL("https://www.youtube.com/watch?v=LYusqeqHAUc");
		}

		private void ResetStateSettings()
		{
			stateSettings = StateSettings.Default;
		}

		public void Teleport(Vector3 position, Quaternion rotation, bool moveToTarget)
		{
			teleport = true;
			teleportPosition = position;
			teleportRotation = rotation;
			teleportMoveToTarget = moveToTarget;
		}

		private void OnDisable()
		{
			if (!base.gameObject.activeInHierarchy && initiated && Application.isPlaying)
			{
				Muscle[] array = muscles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Reset();
				}
			}
			hasBeenDisabled = true;
		}

		private void OnEnable()
		{
			if (!base.gameObject.activeInHierarchy || !initiated || !hasBeenDisabled || !Application.isPlaying)
			{
				return;
			}
			isSwitchingMode = false;
			activeMode = mode;
			lastMode = mode;
			mappingBlend = ((mode == Mode.Active) ? 1f : 0f);
			activeState = state;
			lastState = state;
			isKilling = false;
			freezeFlag = false;
			SetAnimationEnabled(state == State.Alive);
			if (state == State.Alive && targetAnimator != null)
			{
				targetAnimator.Update(0.001f);
			}
			Muscle[] array = muscles;
			foreach (Muscle obj in array)
			{
				obj.state.pinWeightMlp = ((state == State.Alive) ? 1f : 0f);
				obj.state.muscleWeightMlp = ((state == State.Alive) ? 1f : stateSettings.deadMuscleWeight);
				obj.state.muscleDamperAdd = 0f;
			}
			BehaviourBase[] array2;
			if (state != State.Frozen && mode != Mode.Disabled)
			{
				ActivateRagdoll(mode == Mode.Kinematic);
				array2 = behaviours;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(true);
				}
			}
			else
			{
				array = muscles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].joint.gameObject.SetActive(false);
				}
				if (state == State.Frozen)
				{
					array2 = behaviours;
					foreach (BehaviourBase behaviourBase in array2)
					{
						if (behaviourBase.gameObject.activeSelf)
						{
							behaviourBase.deactivated = true;
							behaviourBase.gameObject.SetActive(false);
						}
					}
					if (stateSettings.freezePermanently)
					{
						if (behaviours.Length != 0 && behaviours[0] != null)
						{
							UnityEngine.Object.Destroy(behaviours[0].transform.parent.gameObject);
						}
						UnityEngine.Object.Destroy(base.gameObject);
						return;
					}
				}
			}
			array2 = behaviours;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].OnReactivate();
			}
		}

		private void Awake()
		{
			if (muscles.Length != 0)
			{
				Initiate();
				if (!initiated)
				{
					awakeFailed = true;
				}
			}
		}

		private void Start()
		{
			if (!initiated && !awakeFailed)
			{
				Initiate();
			}
			if (initiated)
			{
				SolverManager[] componentsInChildren = targetRoot.GetComponentsInChildren<SolverManager>();
				solvers.AddRange(componentsInChildren);
			}
		}

		public Transform FindTargetRootRecursive(Transform t)
		{
			if (t.parent == null)
			{
				return null;
			}
			foreach (Transform item in t.parent)
			{
				if (item == base.transform)
				{
					return t;
				}
			}
			return FindTargetRootRecursive(t.parent);
		}

		private void Initiate()
		{
			initiated = false;
			if (muscles.Length != 0 && muscles[0].target != null && targetRoot == null)
			{
				targetRoot = FindTargetRootRecursive(muscles[0].target);
			}
			if (targetRoot != null && targetAnimator == null)
			{
				targetAnimator = targetRoot.GetComponentInChildren<Animator>();
				if (targetAnimator == null)
				{
					targetAnimation = targetRoot.GetComponentInChildren<Animation>();
				}
			}
			if (!IsValid(true))
			{
				return;
			}
			if (humanoidConfig != null && targetAnimator != null && targetAnimator.isHuman)
			{
				humanoidConfig.ApplyTo(this);
			}
			isLegacy = targetAnimator == null && targetAnimation != null;
			behaviours = base.transform.GetComponentsInChildren<BehaviourBase>();
			if (behaviours.Length == 0 && base.transform.parent != null)
			{
				behaviours = base.transform.parent.GetComponentsInChildren<BehaviourBase>();
			}
			for (int i = 0; i < muscles.Length; i++)
			{
				muscles[i].Initiate(muscles);
				if (behaviours.Length != 0)
				{
					muscles[i].broadcaster = muscles[i].joint.gameObject.GetComponent<MuscleCollisionBroadcaster>();
					if (muscles[i].broadcaster == null)
					{
						muscles[i].broadcaster = muscles[i].joint.gameObject.AddComponent<MuscleCollisionBroadcaster>();
					}
					muscles[i].broadcaster.puppetMaster = this;
					muscles[i].broadcaster.muscleIndex = i;
				}
				muscles[i].jointBreakBroadcaster = muscles[i].joint.gameObject.GetComponent<JointBreakBroadcaster>();
				if (muscles[i].jointBreakBroadcaster == null)
				{
					muscles[i].jointBreakBroadcaster = muscles[i].joint.gameObject.AddComponent<JointBreakBroadcaster>();
				}
				muscles[i].jointBreakBroadcaster.puppetMaster = this;
				muscles[i].jointBreakBroadcaster.muscleIndex = i;
			}
			UpdateHierarchies();
			hierarchyIsFlat = HierarchyIsFlat();
			initiated = true;
			BehaviourBase[] array = behaviours;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].puppetMaster = this;
			}
			array = behaviours;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].Initiate();
			}
			SwitchStates();
			SwitchModes();
			Muscle[] array2 = muscles;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].Read();
			}
			StoreTargetMappedState();
			if (Singleton<PuppetMasterSettings>.instance != null)
			{
				Singleton<PuppetMasterSettings>.instance.Register(this);
			}
			bool flag = false;
			array = behaviours;
			foreach (BehaviourBase behaviourBase in array)
			{
				if (behaviourBase is BehaviourPuppet && behaviourBase.enabled)
				{
					ActivateBehaviour(behaviourBase);
					flag = true;
					break;
				}
			}
			if (!flag && behaviours.Length != 0)
			{
				array = behaviours;
				foreach (BehaviourBase behaviourBase2 in array)
				{
					if (behaviourBase2.enabled)
					{
						ActivateBehaviour(behaviourBase2);
						break;
					}
				}
			}
			if (OnPostInitiate != null)
			{
				OnPostInitiate();
			}
		}

		private void ActivateBehaviour(BehaviourBase behaviour)
		{
			BehaviourBase[] array = behaviours;
			foreach (BehaviourBase behaviourBase in array)
			{
				behaviourBase.enabled = behaviourBase == behaviour;
				if (behaviourBase.enabled)
				{
					behaviourBase.Activate();
				}
			}
		}

		private void OnDestroy()
		{
			if (Singleton<PuppetMasterSettings>.instance != null)
			{
				Singleton<PuppetMasterSettings>.instance.Unregister(this);
			}
		}

		private bool IsInterpolated()
		{
			if (!initiated)
			{
				return false;
			}
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].rigidbody.interpolation != 0)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void FixedUpdate()
		{
			if (!initiated || muscles.Length == 0)
			{
				return;
			}
			interpolated = IsInterpolated();
			fixedFrame = true;
			if (!isActive)
			{
				return;
			}
			pinWeight = Mathf.Clamp(pinWeight, 0f, 1f);
			muscleWeight = Mathf.Clamp(muscleWeight, 0f, 1f);
			muscleSpring = Mathf.Clamp(muscleSpring, 0f, muscleSpring);
			muscleDamper = Mathf.Clamp(muscleDamper, 0f, muscleDamper);
			pinPow = Mathf.Clamp(pinPow, 1f, 8f);
			pinDistanceFalloff = Mathf.Max(pinDistanceFalloff, 0f);
			if (updateMode == UpdateMode.FixedUpdate)
			{
				FixTargetTransforms();
				if (targetAnimator.enabled || (!targetAnimator.enabled && animatorDisabled))
				{
					targetAnimator.enabled = false;
					animatorDisabled = true;
					targetAnimator.Update(Time.fixedDeltaTime);
				}
				else
				{
					animatorDisabled = false;
					targetAnimator.enabled = false;
				}
				foreach (SolverManager solver in solvers)
				{
					if (solver != null)
					{
						solver.UpdateSolverExternal();
					}
				}
				Read();
			}
			if (!isFrozen)
			{
				SetInternalCollisions(internalCollisions);
				SetAngularLimits(angularLimits);
				if (solverIterationCount != lastSolverIterationCount)
				{
					for (int i = 0; i < muscles.Length; i++)
					{
						muscles[i].rigidbody.solverIterations = solverIterationCount;
					}
					lastSolverIterationCount = solverIterationCount;
				}
				for (int j = 0; j < muscles.Length; j++)
				{
					muscles[j].Update(pinWeight, muscleWeight, muscleSpring, muscleDamper, pinPow, pinDistanceFalloff, true);
				}
			}
			if (updateMode == UpdateMode.AnimatePhysics)
			{
				FixTargetTransforms();
			}
		}

		protected virtual void Update()
		{
			if (initiated && muscles.Length != 0)
			{
				if (animatorDisabled)
				{
					targetAnimator.enabled = true;
					animatorDisabled = false;
				}
				if (updateMode == UpdateMode.Normal)
				{
					FixTargetTransforms();
				}
			}
		}

		protected virtual void LateUpdate()
		{
			if (muscles.Length != 0)
			{
				OnLateUpdate();
				if (OnPostLateUpdate != null)
				{
					OnPostLateUpdate();
				}
			}
		}

		protected virtual void OnLateUpdate()
		{
			if (!initiated)
			{
				return;
			}
			if (animatorDisabled)
			{
				targetAnimator.enabled = true;
				animatorDisabled = false;
			}
			SwitchStates();
			SwitchModes();
			switch (updateMode)
			{
			case UpdateMode.FixedUpdate:
				if (!fixedFrame && !interpolated)
				{
					return;
				}
				break;
			case UpdateMode.AnimatePhysics:
				if (!fixedFrame && !interpolated)
				{
					return;
				}
				if (isActive && !fixedFrame)
				{
					Read();
				}
				break;
			case UpdateMode.Normal:
				if (isActive)
				{
					Read();
				}
				break;
			}
			fixedFrame = false;
			if (!isFrozen)
			{
				mappingWeight = Mathf.Clamp(mappingWeight, 0f, 1f);
				float num = mappingWeight * mappingBlend;
				if (num > 0f)
				{
					if (isActive)
					{
						for (int i = 0; i < muscles.Length; i++)
						{
							muscles[i].Map(num);
						}
					}
				}
				else if (activeMode == Mode.Kinematic)
				{
					MoveToTarget();
				}
				BehaviourBase[] array = behaviours;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].OnWrite();
				}
				if (OnWrite != null)
				{
					OnWrite();
				}
				StoreTargetMappedState();
				Muscle[] array2 = muscles;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].CalculateMappedVelocity();
				}
			}
			if (freezeFlag)
			{
				OnFreezeFlag();
			}
		}

		private void MoveToTarget()
		{
			if (Singleton<PuppetMasterSettings>.instance == null || (Singleton<PuppetMasterSettings>.instance != null && Singleton<PuppetMasterSettings>.instance.UpdateMoveToTarget(this)))
			{
				Muscle[] array = muscles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].MoveToTarget();
				}
			}
		}

		private void Read()
		{
			BehaviourBase[] array2;
			Muscle[] array;
			if (teleport)
			{
				GameObject gameObject = new GameObject();
				gameObject.transform.position = ((base.transform.parent != null) ? base.transform.parent.position : Vector3.zero);
				gameObject.transform.rotation = ((base.transform.parent != null) ? base.transform.parent.rotation : Quaternion.identity);
				Transform parent = base.transform.parent;
				Transform parent2 = targetRoot.parent;
				base.transform.parent = gameObject.transform;
				targetRoot.parent = gameObject.transform;
				Vector3 position = base.transform.parent.position;
				Quaternion quaternion = QuaTools.FromToRotation(targetRoot.rotation, teleportRotation);
				base.transform.parent.rotation = quaternion * base.transform.parent.rotation;
				Vector3 vector = teleportPosition - targetRoot.position;
				base.transform.parent.position += vector;
				base.transform.parent = parent;
				targetRoot.parent = parent2;
				UnityEngine.Object.Destroy(gameObject);
				targetMappedPositions[0] = position + quaternion * (targetMappedPositions[0] - position) + vector;
				targetSampledPositions[0] = position + quaternion * (targetSampledPositions[0] - position) + vector;
				targetMappedRotations[0] = quaternion * targetMappedRotations[0];
				targetSampledRotations[0] = quaternion * targetSampledRotations[0];
				if (teleportMoveToTarget)
				{
					array = muscles;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].MoveToTarget();
					}
				}
				array = muscles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].ClearVelocities();
				}
				array2 = behaviours;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].OnTeleport(quaternion, vector, position, teleportMoveToTarget);
				}
				teleport = false;
			}
			if (OnRead != null)
			{
				OnRead();
			}
			array2 = behaviours;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].OnRead();
			}
			if (!isAlive)
			{
				return;
			}
			array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Read();
			}
			if (isAlive && updateJointAnchors)
			{
				for (int j = 0; j < muscles.Length; j++)
				{
					muscles[j].UpdateAnchor(supportTranslationAnimation);
				}
			}
		}

		private void FixTargetTransforms()
		{
			if (!isAlive)
			{
				return;
			}
			if (OnFixTransforms != null)
			{
				OnFixTransforms();
			}
			BehaviourBase[] array = behaviours;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnFixTransforms();
			}
			if ((!fixTargetTransforms && !hasProp) || !isActive)
			{
				return;
			}
			mappingWeight = Mathf.Clamp(mappingWeight, 0f, 1f);
			if (mappingWeight * mappingBlend <= 0f)
			{
				return;
			}
			for (int j = 0; j < muscles.Length; j++)
			{
				if (fixTargetTransforms || muscles[j].props.group == Muscle.Group.Prop)
				{
					muscles[j].FixTargetTransforms();
				}
			}
		}

		private void VisualizeTargetPose()
		{
			if (!visualizeTargetPose || !Application.isEditor || !isActive)
			{
				return;
			}
			Muscle[] array = muscles;
			foreach (Muscle muscle in array)
			{
				if (!(muscle.joint.connectedBody != null) || !(muscle.connectedBodyTarget != null))
				{
					continue;
				}
				Debug.DrawLine(muscle.target.position, muscle.connectedBodyTarget.position, Color.cyan);
				bool flag = true;
				Muscle[] array2 = muscles;
				foreach (Muscle muscle2 in array2)
				{
					if (muscle != muscle2 && muscle2.joint.connectedBody == muscle.rigidbody)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					VisualizeHierarchy(muscle.target, Color.cyan);
				}
			}
		}

		private void VisualizeHierarchy(Transform t, Color color)
		{
			for (int i = 0; i < t.childCount; i++)
			{
				Debug.DrawLine(t.position, t.GetChild(i).position, color);
				VisualizeHierarchy(t.GetChild(i), color);
			}
		}

		private void SetInternalCollisions(bool collide)
		{
			if (internalCollisionsEnabled == collide)
			{
				return;
			}
			for (int i = 0; i < muscles.Length; i++)
			{
				for (int j = i; j < muscles.Length; j++)
				{
					if (i != j)
					{
						muscles[i].IgnoreCollisions(muscles[j], !collide);
					}
				}
			}
			internalCollisionsEnabled = collide;
		}

		private void SetAngularLimits(bool limited)
		{
			if (angularLimitsEnabled != limited)
			{
				for (int i = 0; i < muscles.Length; i++)
				{
					muscles[i].IgnoreAngularLimits(!limited);
				}
				angularLimitsEnabled = limited;
			}
		}

		public void AddMuscle(ConfigurableJoint joint, Transform target, Rigidbody connectTo, Transform targetParent, Muscle.Props muscleProps = null, bool forceTreeHierarchy = false, bool forceLayers = true)
		{
			if (!CheckIfInitiated())
			{
				return;
			}
			if (!initiated)
			{
				Debug.LogWarning("PuppetMaster has not been initiated.", base.transform);
				return;
			}
			if (ContainsJoint(joint))
			{
				Debug.LogWarning("Joint " + joint.name + " is already used by a Muscle", base.transform);
				return;
			}
			if (target == null)
			{
				Debug.LogWarning("AddMuscle was called with a null 'target' reference.", base.transform);
				return;
			}
			if (connectTo == joint.GetComponent<Rigidbody>())
			{
				Debug.LogWarning("ConnectTo is the joint's own Rigidbody, can not add muscle.", base.transform);
				return;
			}
			if (!isActive)
			{
				Debug.LogWarning("Adding muscles to inactive PuppetMasters is not currently supported.", base.transform);
				return;
			}
			if (muscleProps == null)
			{
				muscleProps = new Muscle.Props();
			}
			Muscle muscle = new Muscle();
			muscle.props = muscleProps;
			muscle.joint = joint;
			muscle.target = target;
			muscle.joint.transform.parent = (((hierarchyIsFlat || connectTo == null) && !forceTreeHierarchy) ? base.transform : connectTo.transform);
			if (forceLayers)
			{
				joint.gameObject.layer = base.gameObject.layer;
				target.gameObject.layer = targetRoot.gameObject.layer;
			}
			if (connectTo != null)
			{
				muscle.target.parent = targetParent;
				Vector3 position = GetMuscle(connectTo).transform.InverseTransformPoint(muscle.target.position);
				Quaternion quaternion = Quaternion.Inverse(GetMuscle(connectTo).transform.rotation) * muscle.target.rotation;
				joint.transform.position = connectTo.transform.TransformPoint(position);
				joint.transform.rotation = connectTo.transform.rotation * quaternion;
				joint.connectedBody = connectTo;
			}
			muscle.Initiate(muscles);
			if (connectTo != null)
			{
				muscle.rigidbody.velocity = connectTo.velocity;
				muscle.rigidbody.angularVelocity = connectTo.angularVelocity;
			}
			if (!internalCollisions)
			{
				for (int i = 0; i < muscles.Length; i++)
				{
					muscle.IgnoreCollisions(muscles[i], true);
				}
			}
			Array.Resize(ref muscles, muscles.Length + 1);
			muscles[muscles.Length - 1] = muscle;
			muscle.IgnoreAngularLimits(!angularLimits);
			if (behaviours.Length != 0)
			{
				muscle.broadcaster = muscle.joint.gameObject.AddComponent<MuscleCollisionBroadcaster>();
				muscle.broadcaster.puppetMaster = this;
				muscle.broadcaster.muscleIndex = muscles.Length - 1;
			}
			muscle.jointBreakBroadcaster = muscle.joint.gameObject.AddComponent<JointBreakBroadcaster>();
			muscle.jointBreakBroadcaster.puppetMaster = this;
			muscle.jointBreakBroadcaster.muscleIndex = muscles.Length - 1;
			UpdateHierarchies();
			CheckMassVariation(100f, true);
			BehaviourBase[] array = behaviours;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].OnMuscleAdded(muscle);
			}
		}

		public void RemoveMuscleRecursive(ConfigurableJoint joint, bool attachTarget, bool blockTargetAnimation = false, MuscleRemoveMode removeMode = MuscleRemoveMode.Sever)
		{
			if (!CheckIfInitiated())
			{
				return;
			}
			if (joint == null)
			{
				Debug.LogWarning("RemoveMuscleRecursive was called with a null 'joint' reference.", base.transform);
				return;
			}
			if (!ContainsJoint(joint))
			{
				Debug.LogWarning("No Muscle with the specified joint was found, can not remove muscle.", base.transform);
				return;
			}
			int muscleIndex = GetMuscleIndex(joint);
			Muscle[] array = new Muscle[muscles.Length - (muscles[muscleIndex].childIndexes.Length + 1)];
			int num = 0;
			for (int i = 0; i < muscles.Length; i++)
			{
				if (i != muscleIndex && !muscles[muscleIndex].childFlags[i])
				{
					array[num] = muscles[i];
					num++;
					continue;
				}
				if (muscles[i].broadcaster != null)
				{
					muscles[i].broadcaster.enabled = false;
					UnityEngine.Object.Destroy(muscles[i].broadcaster);
				}
				if (muscles[i].jointBreakBroadcaster != null)
				{
					muscles[i].jointBreakBroadcaster.enabled = false;
					UnityEngine.Object.Destroy(muscles[i].jointBreakBroadcaster);
				}
			}
			switch (removeMode)
			{
			case MuscleRemoveMode.Sever:
			{
				DisconnectJoint(muscles[muscleIndex].joint);
				for (int k = 0; k < muscles[muscleIndex].childIndexes.Length; k++)
				{
					KillJoint(muscles[muscles[muscleIndex].childIndexes[k]].joint);
				}
				break;
			}
			case MuscleRemoveMode.Explode:
			{
				DisconnectJoint(muscles[muscleIndex].joint);
				for (int l = 0; l < muscles[muscleIndex].childIndexes.Length; l++)
				{
					DisconnectJoint(muscles[muscles[muscleIndex].childIndexes[l]].joint);
				}
				break;
			}
			case MuscleRemoveMode.Numb:
			{
				KillJoint(muscles[muscleIndex].joint);
				for (int j = 0; j < muscles[muscleIndex].childIndexes.Length; j++)
				{
					KillJoint(muscles[muscles[muscleIndex].childIndexes[j]].joint);
				}
				break;
			}
			}
			muscles[muscleIndex].transform.parent = null;
			for (int m = 0; m < muscles[muscleIndex].childIndexes.Length; m++)
			{
				if (removeMode == MuscleRemoveMode.Explode || muscles[muscles[muscleIndex].childIndexes[m]].transform.parent == base.transform)
				{
					muscles[muscles[muscleIndex].childIndexes[m]].transform.parent = null;
				}
			}
			BehaviourBase[] array2 = behaviours;
			foreach (BehaviourBase behaviourBase in array2)
			{
				behaviourBase.OnMuscleRemoved(muscles[muscleIndex]);
				for (int num2 = 0; num2 < muscles[muscleIndex].childIndexes.Length; num2++)
				{
					Muscle m2 = muscles[muscles[muscleIndex].childIndexes[num2]];
					behaviourBase.OnMuscleRemoved(m2);
				}
			}
			if (attachTarget)
			{
				muscles[muscleIndex].target.parent = muscles[muscleIndex].transform;
				muscles[muscleIndex].target.position = muscles[muscleIndex].transform.position;
				muscles[muscleIndex].target.rotation = muscles[muscleIndex].transform.rotation * muscles[muscleIndex].targetRotationRelative;
				for (int num3 = 0; num3 < muscles[muscleIndex].childIndexes.Length; num3++)
				{
					Muscle muscle = muscles[muscles[muscleIndex].childIndexes[num3]];
					muscle.target.parent = muscle.transform;
					muscle.target.position = muscle.transform.position;
					muscle.target.rotation = muscle.transform.rotation;
				}
			}
			if (blockTargetAnimation)
			{
				muscles[muscleIndex].target.gameObject.AddComponent<AnimationBlocker>();
				for (int num4 = 0; num4 < muscles[muscleIndex].childIndexes.Length; num4++)
				{
					muscles[muscles[muscleIndex].childIndexes[num4]].target.gameObject.AddComponent<AnimationBlocker>();
				}
			}
			if (OnMuscleRemoved != null)
			{
				OnMuscleRemoved(muscles[muscleIndex]);
			}
			for (int num5 = 0; num5 < muscles[muscleIndex].childIndexes.Length; num5++)
			{
				Muscle muscle2 = muscles[muscles[muscleIndex].childIndexes[num5]];
				if (OnMuscleRemoved != null)
				{
					OnMuscleRemoved(muscle2);
				}
			}
			if (!internalCollisionsEnabled)
			{
				Muscle[] array3 = array;
				for (int n = 0; n < array3.Length; n++)
				{
					Collider[] colliders = array3[n].colliders;
					foreach (Collider collider in colliders)
					{
						Collider[] colliders2 = muscles[muscleIndex].colliders;
						foreach (Collider collider2 in colliders2)
						{
							Physics.IgnoreCollision(collider, collider2, false);
						}
						for (int num8 = 0; num8 < muscles[muscleIndex].childIndexes.Length; num8++)
						{
							colliders2 = muscles[num8].colliders;
							foreach (Collider collider3 in colliders2)
							{
								Physics.IgnoreCollision(collider, collider3, false);
							}
						}
					}
				}
			}
			muscles = array;
			UpdateHierarchies();
		}

		public void ReplaceMuscle(ConfigurableJoint oldJoint, ConfigurableJoint newJoint)
		{
			if (CheckIfInitiated())
			{
				Debug.LogWarning("@todo", base.transform);
			}
		}

		public void SetMuscles(Muscle[] newMuscles)
		{
			if (CheckIfInitiated())
			{
				Debug.LogWarning("@todo", base.transform);
			}
		}

		public void DisableMuscleRecursive(ConfigurableJoint joint)
		{
			if (CheckIfInitiated())
			{
				Debug.LogWarning("@todo", base.transform);
			}
		}

		public void EnableMuscleRecursive(ConfigurableJoint joint)
		{
			if (CheckIfInitiated())
			{
				Debug.LogWarning("@todo", base.transform);
			}
		}

		[ContextMenu("Flatten Muscle Hierarchy")]
		public void FlattenHierarchy()
		{
			Muscle[] array = muscles;
			foreach (Muscle muscle in array)
			{
				if (muscle.joint != null)
				{
					muscle.joint.transform.parent = base.transform;
				}
			}
			hierarchyIsFlat = true;
		}

		[ContextMenu("Tree Muscle Hierarchy")]
		public void TreeHierarchy()
		{
			Muscle[] array = muscles;
			foreach (Muscle muscle in array)
			{
				if (muscle.joint != null)
				{
					muscle.joint.transform.parent = ((muscle.joint.connectedBody != null) ? muscle.joint.connectedBody.transform : base.transform);
				}
			}
			hierarchyIsFlat = false;
		}

		[ContextMenu("Fix Muscle Positions")]
		public void FixMusclePositions()
		{
			Muscle[] array = muscles;
			foreach (Muscle muscle in array)
			{
				if (muscle.joint != null && muscle.target != null)
				{
					muscle.joint.transform.position = muscle.target.position;
				}
			}
		}

		private void AddIndexesRecursive(int index, ref int[] indexes)
		{
			int num = indexes.Length;
			Array.Resize(ref indexes, indexes.Length + 1 + muscles[index].childIndexes.Length);
			indexes[num] = index;
			if (muscles[index].childIndexes.Length != 0)
			{
				for (int i = 0; i < muscles[index].childIndexes.Length; i++)
				{
					AddIndexesRecursive(muscles[index].childIndexes[i], ref indexes);
				}
			}
		}

		private bool HierarchyIsFlat()
		{
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].joint.transform.parent != base.transform)
				{
					return false;
				}
			}
			return true;
		}

		private void DisconnectJoint(ConfigurableJoint joint)
		{
			joint.connectedBody = null;
			KillJoint(joint);
			joint.xMotion = ConfigurableJointMotion.Free;
			joint.yMotion = ConfigurableJointMotion.Free;
			joint.zMotion = ConfigurableJointMotion.Free;
			joint.angularXMotion = ConfigurableJointMotion.Free;
			joint.angularYMotion = ConfigurableJointMotion.Free;
			joint.angularZMotion = ConfigurableJointMotion.Free;
		}

		private void KillJoint(ConfigurableJoint joint)
		{
			joint.targetRotation = Quaternion.identity;
			JointDrive slerpDrive = default(JointDrive);
			slerpDrive.positionSpring = 0f;
			slerpDrive.positionDamper = 0f;
			joint.slerpDrive = slerpDrive;
		}

		public void DisableImmediately()
		{
			mappingBlend = 0f;
			isSwitchingMode = false;
			mode = Mode.Disabled;
			activeMode = mode;
			lastMode = mode;
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].rigidbody.gameObject.SetActive(false);
			}
		}

		protected virtual void SwitchModes()
		{
			if (!initiated)
			{
				return;
			}
			if (isKilling)
			{
				mode = Mode.Active;
			}
			if (!isAlive)
			{
				mode = Mode.Active;
			}
			BehaviourBase[] array = behaviours;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].forceActive)
				{
					mode = Mode.Active;
					break;
				}
			}
			if (mode == lastMode || isSwitchingMode || (isKilling && mode != 0) || (state != 0 && mode != 0))
			{
				return;
			}
			isSwitchingMode = true;
			if (lastMode == Mode.Disabled)
			{
				if (mode == Mode.Kinematic)
				{
					DisabledToKinematic();
				}
				else if (mode == Mode.Active)
				{
					StartCoroutine(DisabledToActive());
				}
			}
			else if (lastMode == Mode.Kinematic)
			{
				if (mode == Mode.Disabled)
				{
					KinematicToDisabled();
				}
				else if (mode == Mode.Active)
				{
					StartCoroutine(KinematicToActive());
				}
			}
			else if (lastMode == Mode.Active)
			{
				if (mode == Mode.Disabled)
				{
					StartCoroutine(ActiveToDisabled());
				}
				else if (mode == Mode.Kinematic)
				{
					StartCoroutine(ActiveToKinematic());
				}
			}
			lastMode = mode;
		}

		private void DisabledToKinematic()
		{
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Reset();
			}
			array = muscles;
			foreach (Muscle obj in array)
			{
				obj.rigidbody.gameObject.SetActive(true);
				obj.rigidbody.isKinematic = true;
			}
			internalCollisionsEnabled = true;
			SetInternalCollisions(internalCollisions);
			array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MoveToTarget();
			}
			activeMode = Mode.Kinematic;
			isSwitchingMode = false;
		}

		private IEnumerator DisabledToActive()
		{
			Muscle[] array = muscles;
			foreach (Muscle muscle in array)
			{
				if (!muscle.rigidbody.gameObject.activeInHierarchy)
				{
					muscle.Reset();
				}
			}
			array = muscles;
			foreach (Muscle muscle2 in array)
			{
				muscle2.rigidbody.gameObject.SetActive(true);
				muscle2.rigidbody.isKinematic = false;
				muscle2.rigidbody.WakeUp();
				muscle2.rigidbody.velocity = muscle2.mappedVelocity;
				muscle2.rigidbody.angularVelocity = muscle2.mappedAngularVelocity;
			}
			internalCollisionsEnabled = true;
			SetInternalCollisions(internalCollisions);
			Read();
			array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MoveToTarget();
			}
			UpdateInternalCollisions();
			while (mappingBlend < 1f)
			{
				mappingBlend = Mathf.Clamp(mappingBlend + Time.deltaTime / blendTime, 0f, 1f);
				yield return null;
			}
			activeMode = Mode.Active;
			isSwitchingMode = false;
		}

		private void KinematicToDisabled()
		{
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].rigidbody.gameObject.SetActive(false);
			}
			activeMode = Mode.Disabled;
			isSwitchingMode = false;
		}

		private IEnumerator KinematicToActive()
		{
			Muscle[] array = muscles;
			foreach (Muscle muscle in array)
			{
				muscle.rigidbody.isKinematic = false;
				muscle.rigidbody.WakeUp();
				muscle.rigidbody.velocity = muscle.mappedVelocity;
				muscle.rigidbody.angularVelocity = muscle.mappedAngularVelocity;
			}
			Read();
			array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MoveToTarget();
			}
			UpdateInternalCollisions();
			while (mappingBlend < 1f)
			{
				mappingBlend = Mathf.Min(mappingBlend + Time.deltaTime / blendTime, 1f);
				yield return null;
			}
			activeMode = Mode.Active;
			isSwitchingMode = false;
		}

		private IEnumerator ActiveToDisabled()
		{
			while (mappingBlend > 0f)
			{
				mappingBlend = Mathf.Max(mappingBlend - Time.deltaTime / blendTime, 0f);
				yield return null;
			}
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].rigidbody.gameObject.SetActive(false);
			}
			activeMode = Mode.Disabled;
			isSwitchingMode = false;
		}

		private IEnumerator ActiveToKinematic()
		{
			while (mappingBlend > 0f)
			{
				mappingBlend = Mathf.Max(mappingBlend - Time.deltaTime / blendTime, 0f);
				yield return null;
			}
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].rigidbody.isKinematic = true;
			}
			array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MoveToTarget();
			}
			activeMode = Mode.Kinematic;
			isSwitchingMode = false;
		}

		private void UpdateInternalCollisions()
		{
			if (internalCollisions)
			{
				return;
			}
			for (int i = 0; i < muscles.Length; i++)
			{
				for (int j = i; j < muscles.Length; j++)
				{
					if (i != j)
					{
						muscles[i].IgnoreCollisions(muscles[j], true);
					}
				}
			}
		}

		public void SetMuscleWeights(Muscle.Group group, float muscleWeight, float pinWeight = 1f, float mappingWeight = 1f, float muscleDamper = 1f)
		{
			if (!CheckIfInitiated())
			{
				return;
			}
			Muscle[] array = muscles;
			foreach (Muscle muscle in array)
			{
				if (muscle.props.group == group)
				{
					muscle.props.muscleWeight = muscleWeight;
					muscle.props.pinWeight = pinWeight;
					muscle.props.mappingWeight = mappingWeight;
					muscle.props.muscleDamper = muscleDamper;
				}
			}
		}

		public void SetMuscleWeights(Transform target, float muscleWeight, float pinWeight = 1f, float mappingWeight = 1f, float muscleDamper = 1f)
		{
			if (CheckIfInitiated())
			{
				int muscleIndex = GetMuscleIndex(target);
				if (muscleIndex != -1)
				{
					SetMuscleWeights(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
				}
			}
		}

		public void SetMuscleWeights(HumanBodyBones humanBodyBone, float muscleWeight, float pinWeight = 1f, float mappingWeight = 1f, float muscleDamper = 1f)
		{
			if (CheckIfInitiated())
			{
				int muscleIndex = GetMuscleIndex(humanBodyBone);
				if (muscleIndex != -1)
				{
					SetMuscleWeights(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
				}
			}
		}

		public void SetMuscleWeightsRecursive(Transform target, float muscleWeight, float pinWeight = 1f, float mappingWeight = 1f, float muscleDamper = 1f)
		{
			if (!CheckIfInitiated())
			{
				return;
			}
			for (int i = 0; i < muscles.Length; i++)
			{
				if (muscles[i].target == target)
				{
					SetMuscleWeightsRecursive(i, muscleWeight, pinWeight, mappingWeight, muscleDamper);
					break;
				}
			}
		}

		public void SetMuscleWeightsRecursive(int muscleIndex, float muscleWeight, float pinWeight = 1f, float mappingWeight = 1f, float muscleDamper = 1f)
		{
			if (CheckIfInitiated())
			{
				SetMuscleWeights(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
				for (int i = 0; i < muscles[muscleIndex].childIndexes.Length; i++)
				{
					int muscleIndex2 = muscles[muscleIndex].childIndexes[i];
					SetMuscleWeights(muscleIndex2, muscleWeight, pinWeight, mappingWeight, muscleDamper);
				}
			}
		}

		public void SetMuscleWeightsRecursive(HumanBodyBones humanBodyBone, float muscleWeight, float pinWeight = 1f, float mappingWeight = 1f, float muscleDamper = 1f)
		{
			if (CheckIfInitiated())
			{
				int muscleIndex = GetMuscleIndex(humanBodyBone);
				if (muscleIndex != -1)
				{
					SetMuscleWeightsRecursive(muscleIndex, muscleWeight, pinWeight, mappingWeight, muscleDamper);
				}
			}
		}

		public void SetMuscleWeights(int muscleIndex, float muscleWeight, float pinWeight, float mappingWeight, float muscleDamper)
		{
			if (CheckIfInitiated())
			{
				if ((float)muscleIndex < 0f || muscleIndex >= muscles.Length)
				{
					Debug.LogWarning("Muscle index out of range (" + muscleIndex + ").", base.transform);
					return;
				}
				muscles[muscleIndex].props.muscleWeight = muscleWeight;
				muscles[muscleIndex].props.pinWeight = pinWeight;
				muscles[muscleIndex].props.mappingWeight = mappingWeight;
				muscles[muscleIndex].props.muscleDamper = muscleDamper;
			}
		}

		public Muscle GetMuscle(Transform target)
		{
			int muscleIndex = GetMuscleIndex(target);
			if (muscleIndex == -1)
			{
				return null;
			}
			return muscles[muscleIndex];
		}

		public Muscle GetMuscle(Rigidbody rigidbody)
		{
			int muscleIndex = GetMuscleIndex(rigidbody);
			if (muscleIndex == -1)
			{
				return null;
			}
			return muscles[muscleIndex];
		}

		public Muscle GetMuscle(ConfigurableJoint joint)
		{
			int muscleIndex = GetMuscleIndex(joint);
			if (muscleIndex == -1)
			{
				return null;
			}
			return muscles[muscleIndex];
		}

		public bool ContainsJoint(ConfigurableJoint joint)
		{
			if (!CheckIfInitiated())
			{
				return false;
			}
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].joint == joint)
				{
					return true;
				}
			}
			return false;
		}

		public int GetMuscleIndex(HumanBodyBones humanBodyBone)
		{
			if (!CheckIfInitiated())
			{
				return -1;
			}
			if (targetAnimator == null)
			{
				Debug.LogWarning("PuppetMaster 'Target Root' has no Animator component on it nor on it's children.", base.transform);
				return -1;
			}
			if (!targetAnimator.isHuman)
			{
				Debug.LogWarning("PuppetMaster target's Animator does not belong to a Humanoid, can hot get human muscle index.", base.transform);
				return -1;
			}
			Transform boneTransform = targetAnimator.GetBoneTransform(humanBodyBone);
			if (boneTransform == null)
			{
				Debug.LogWarning("PuppetMaster target's Avatar does not contain a bone Transform for " + humanBodyBone, base.transform);
				return -1;
			}
			return GetMuscleIndex(boneTransform);
		}

		public int GetMuscleIndex(Transform target)
		{
			if (!CheckIfInitiated())
			{
				return -1;
			}
			if (target == null)
			{
				Debug.LogWarning("Target is null, can not get muscle index.", base.transform);
				return -1;
			}
			for (int i = 0; i < muscles.Length; i++)
			{
				if (muscles[i].target == target)
				{
					return i;
				}
			}
			Debug.LogWarning("No muscle with target " + target.name + "found on the PuppetMaster.", base.transform);
			return -1;
		}

		public int GetMuscleIndex(Rigidbody rigidbody)
		{
			if (!CheckIfInitiated())
			{
				return -1;
			}
			if (rigidbody == null)
			{
				Debug.LogWarning("Rigidbody is null, can not get muscle index.", base.transform);
				return -1;
			}
			for (int i = 0; i < muscles.Length; i++)
			{
				if (muscles[i].rigidbody == rigidbody)
				{
					return i;
				}
			}
			Debug.LogWarning("No muscle with Rigidbody " + rigidbody.name + "found on the PuppetMaster.", base.transform);
			return -1;
		}

		public int GetMuscleIndex(ConfigurableJoint joint)
		{
			if (!CheckIfInitiated())
			{
				return -1;
			}
			if (joint == null)
			{
				Debug.LogWarning("Joint is null, can not get muscle index.", base.transform);
				return -1;
			}
			for (int i = 0; i < muscles.Length; i++)
			{
				if (muscles[i].joint == joint)
				{
					return i;
				}
			}
			Debug.LogWarning("No muscle with Joint " + joint.name + "found on the PuppetMaster.", base.transform);
			return -1;
		}

		public static PuppetMaster SetUp(Transform target, Transform ragdoll, int characterControllerLayer, int ragdollLayer)
		{
			if (ragdoll != target)
			{
				PuppetMaster puppetMaster = ragdoll.gameObject.AddComponent<PuppetMaster>();
				puppetMaster.SetUpTo(target, characterControllerLayer, ragdollLayer);
				return puppetMaster;
			}
			return SetUp(ragdoll, characterControllerLayer, ragdollLayer);
		}

		public static PuppetMaster SetUp(Transform target, int characterControllerLayer, int ragdollLayer)
		{
			PuppetMaster puppetMaster = UnityEngine.Object.Instantiate(target.gameObject, target.position, target.rotation).transform.gameObject.AddComponent<PuppetMaster>();
			puppetMaster.SetUpTo(target, characterControllerLayer, ragdollLayer);
			RemoveRagdollComponents(target, characterControllerLayer);
			return puppetMaster;
		}

		public void SetUpTo(Transform setUpTo, int characterControllerLayer, int ragdollLayer)
		{
			if (setUpTo == null)
			{
				Debug.LogWarning("SetUpTo is null. Can not set the PuppetMaster up to a null Transform.", base.transform);
				return;
			}
			if (setUpTo == base.transform)
			{
				setUpTo = UnityEngine.Object.Instantiate(setUpTo.gameObject, setUpTo.position, setUpTo.rotation).transform;
				setUpTo.name = base.name;
				RemoveRagdollComponents(setUpTo, characterControllerLayer);
			}
			RemoveUnnecessaryBones();
			Component[] componentsInChildren = GetComponentsInChildren<Component>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!(componentsInChildren[i] is PuppetMaster) && !(componentsInChildren[i] is Transform) && !(componentsInChildren[i] is Rigidbody) && !(componentsInChildren[i] is BoxCollider) && !(componentsInChildren[i] is CapsuleCollider) && !(componentsInChildren[i] is SphereCollider) && !(componentsInChildren[i] is MeshCollider) && !(componentsInChildren[i] is Joint) && !(componentsInChildren[i] is Animator))
				{
					UnityEngine.Object.DestroyImmediate(componentsInChildren[i]);
				}
			}
			Animator[] componentsInChildren2 = GetComponentsInChildren<Animator>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren2[j]);
			}
			componentsInChildren = base.transform.GetComponents<Component>();
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				if (!(componentsInChildren[k] is PuppetMaster) && !(componentsInChildren[k] is Transform))
				{
					UnityEngine.Object.DestroyImmediate(componentsInChildren[k]);
				}
			}
			Rigidbody[] componentsInChildren3 = base.transform.GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody rigidbody in componentsInChildren3)
			{
				if (rigidbody.transform != base.transform && rigidbody.GetComponent<ConfigurableJoint>() == null)
				{
					rigidbody.gameObject.AddComponent<ConfigurableJoint>();
				}
			}
			targetRoot = setUpTo;
			SetUpMuscles(setUpTo);
			base.name = "PuppetMaster";
			Transform transform = ((setUpTo.parent == null || setUpTo.parent != base.transform.parent || setUpTo.parent.name != setUpTo.name + " Root") ? new GameObject(setUpTo.name + " Root").transform : setUpTo.parent);
			transform.parent = base.transform.parent;
			Transform transform2 = new GameObject("Behaviours").transform;
			Comments comments = transform2.gameObject.GetComponent<Comments>();
			if (comments == null)
			{
				comments = transform2.gameObject.AddComponent<Comments>();
			}
			comments.text = "All Puppet Behaviours should be parented to this GameObject, the PuppetMaster will automatically find them from here. All Puppet Behaviours have been designed so that they could be simply copied from one character to another without changing any references. It is important because they contain a lot of parameters and would be otherwise tedious to set up and tweak.";
			transform.position = setUpTo.position;
			transform.rotation = setUpTo.rotation;
			transform2.position = setUpTo.position;
			transform2.rotation = setUpTo.rotation;
			base.transform.position = setUpTo.position;
			base.transform.rotation = setUpTo.rotation;
			transform2.parent = transform;
			base.transform.parent = transform;
			setUpTo.parent = transform;
			targetRoot.gameObject.layer = characterControllerLayer;
			base.gameObject.layer = ragdollLayer;
			Muscle[] array = muscles;
			for (int l = 0; l < array.Length; l++)
			{
				array[l].joint.gameObject.layer = ragdollLayer;
			}
			Physics.IgnoreLayerCollision(characterControllerLayer, ragdollLayer);
		}

		public static void RemoveRagdollComponents(Transform target, int characterControllerLayer)
		{
			if (target == null)
			{
				return;
			}
			Rigidbody[] componentsInChildren = target.GetComponentsInChildren<Rigidbody>();
			Cloth[] componentsInChildren2 = target.GetComponentsInChildren<Cloth>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!(componentsInChildren[i].gameObject != target.gameObject))
				{
					continue;
				}
				Joint component = componentsInChildren[i].GetComponent<Joint>();
				Collider component2 = componentsInChildren[i].GetComponent<Collider>();
				if (component != null)
				{
					UnityEngine.Object.DestroyImmediate(component);
				}
				if (component2 != null)
				{
					if (!IsClothCollider(component2, componentsInChildren2))
					{
						UnityEngine.Object.DestroyImmediate(component2);
					}
					else
					{
						component2.gameObject.layer = characterControllerLayer;
					}
				}
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i]);
			}
			Collider[] componentsInChildren3 = target.GetComponentsInChildren<Collider>();
			for (int j = 0; j < componentsInChildren3.Length; j++)
			{
				if (componentsInChildren3[j].transform != target && !IsClothCollider(componentsInChildren3[j], componentsInChildren2))
				{
					UnityEngine.Object.DestroyImmediate(componentsInChildren3[j]);
				}
			}
			PuppetMaster component3 = target.GetComponent<PuppetMaster>();
			if (component3 != null)
			{
				UnityEngine.Object.DestroyImmediate(component3);
			}
		}

		private void SetUpMuscles(Transform setUpTo)
		{
			ConfigurableJoint[] componentsInChildren = base.transform.GetComponentsInChildren<ConfigurableJoint>();
			if (componentsInChildren.Length == 0)
			{
				Debug.LogWarning("No ConfigurableJoints found, can not build PuppetMaster. Please create ConfigurableJoints to connect the ragdoll bones together.", base.transform);
				return;
			}
			Animator componentInChildren = targetRoot.GetComponentInChildren<Animator>();
			Transform[] componentsInChildren2 = setUpTo.GetComponentsInChildren<Transform>();
			muscles = new Muscle[componentsInChildren.Length];
			int num = -1;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				muscles[i] = new Muscle();
				muscles[i].joint = componentsInChildren[i];
				muscles[i].name = componentsInChildren[i].name;
				muscles[i].props = new Muscle.Props(1f, 1f, 1f, 1f, muscles[i].joint.connectedBody == null);
				if (muscles[i].joint.connectedBody == null && num == -1)
				{
					num = i;
				}
				Transform[] array = componentsInChildren2;
				foreach (Transform transform in array)
				{
					if (!(transform.name == componentsInChildren[i].name))
					{
						continue;
					}
					muscles[i].target = transform;
					if (componentInChildren != null)
					{
						muscles[i].props.group = FindGroup(componentInChildren, muscles[i].target);
						if (muscles[i].props.group == Muscle.Group.Hips || muscles[i].props.group == Muscle.Group.Leg || muscles[i].props.group == Muscle.Group.Foot)
						{
							muscles[i].props.mapPosition = true;
						}
					}
					break;
				}
			}
			if (num != 0)
			{
				Muscle muscle = muscles[0];
				Muscle muscle2 = muscles[num];
				muscles[num] = muscle;
				muscles[0] = muscle2;
			}
			bool flag = true;
			Muscle[] array2 = muscles;
			foreach (Muscle muscle3 in array2)
			{
				if (muscle3.target == null)
				{
					Debug.LogWarning("No target Transform found for PuppetMaster muscle " + muscle3.joint.name + ". Please assign manually.", base.transform);
				}
				if (muscle3.props.group != muscles[0].props.group)
				{
					flag = false;
				}
			}
			if (flag)
			{
				Debug.LogWarning("Muscle groups need to be assigned in the PuppetMaster!", base.transform);
			}
		}

		private static Muscle.Group FindGroup(Animator animator, Transform t)
		{
			if (!animator.isHuman)
			{
				return Muscle.Group.Hips;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.Chest))
			{
				return Muscle.Group.Spine;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.Head))
			{
				return Muscle.Group.Head;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.Hips))
			{
				return Muscle.Group.Hips;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.LeftFoot))
			{
				return Muscle.Group.Foot;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.LeftHand))
			{
				return Muscle.Group.Hand;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.LeftLowerArm))
			{
				return Muscle.Group.Arm;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg))
			{
				return Muscle.Group.Leg;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.LeftUpperArm))
			{
				return Muscle.Group.Arm;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg))
			{
				return Muscle.Group.Leg;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.RightFoot))
			{
				return Muscle.Group.Foot;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.RightHand))
			{
				return Muscle.Group.Hand;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.RightLowerArm))
			{
				return Muscle.Group.Arm;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.RightLowerLeg))
			{
				return Muscle.Group.Leg;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.RightUpperArm))
			{
				return Muscle.Group.Arm;
			}
			if (t == animator.GetBoneTransform(HumanBodyBones.RightUpperLeg))
			{
				return Muscle.Group.Leg;
			}
			return Muscle.Group.Spine;
		}

		private void RemoveUnnecessaryBones()
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				bool flag = false;
				if (componentsInChildren[i].GetComponent<Rigidbody>() != null || componentsInChildren[i].GetComponent<ConfigurableJoint>() != null)
				{
					flag = true;
				}
				if (componentsInChildren[i].GetComponent<Collider>() != null && componentsInChildren[i].GetComponent<Rigidbody>() == null)
				{
					flag = true;
				}
				if (componentsInChildren[i].GetComponent<CharacterController>() != null)
				{
					flag = false;
				}
				if (!flag)
				{
					Transform[] array = new Transform[componentsInChildren[i].childCount];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = componentsInChildren[i].GetChild(j);
					}
					for (int k = 0; k < array.Length; k++)
					{
						array[k].parent = componentsInChildren[i].parent;
					}
					UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
				}
			}
		}

		private static bool IsClothCollider(Collider collider, Cloth[] cloths)
		{
			if (cloths == null)
			{
				return false;
			}
			foreach (Cloth cloth in cloths)
			{
				if (cloth == null)
				{
					return false;
				}
				CapsuleCollider[] capsuleColliders = cloth.capsuleColliders;
				foreach (CapsuleCollider capsuleCollider in capsuleColliders)
				{
					if (capsuleCollider != null && capsuleCollider.gameObject == collider.gameObject)
					{
						return true;
					}
				}
				ClothSphereColliderPair[] sphereColliders = cloth.sphereColliders;
				for (int j = 0; j < sphereColliders.Length; j++)
				{
					ClothSphereColliderPair clothSphereColliderPair = sphereColliders[j];
					if (clothSphereColliderPair.first != null && clothSphereColliderPair.first.gameObject == collider.gameObject)
					{
						return true;
					}
					if (clothSphereColliderPair.second != null && clothSphereColliderPair.second.gameObject == collider.gameObject)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Kill()
		{
			state = State.Dead;
		}

		public void Kill(StateSettings stateSettings)
		{
			this.stateSettings = stateSettings;
			state = State.Dead;
		}

		public void Freeze()
		{
			state = State.Frozen;
		}

		public void Freeze(StateSettings stateSettings)
		{
			this.stateSettings = stateSettings;
			state = State.Frozen;
		}

		public void Resurrect()
		{
			state = State.Alive;
		}

		protected virtual void SwitchStates()
		{
			if (state == lastState || isKilling)
			{
				return;
			}
			if (freezeFlag)
			{
				if (state == State.Alive)
				{
					activeState = State.Dead;
					lastState = State.Dead;
					freezeFlag = false;
				}
				else if (state == State.Dead)
				{
					lastState = State.Dead;
					freezeFlag = false;
					return;
				}
				if (freezeFlag)
				{
					return;
				}
			}
			if (lastState == State.Alive)
			{
				if (state == State.Dead)
				{
					StartCoroutine(AliveToDead(false));
				}
				else if (state == State.Frozen)
				{
					StartCoroutine(AliveToDead(true));
				}
			}
			else if (lastState == State.Dead)
			{
				if (state == State.Alive)
				{
					DeadToAlive();
				}
				else if (state == State.Frozen)
				{
					DeadToFrozen();
				}
			}
			else if (lastState == State.Frozen)
			{
				if (state == State.Alive)
				{
					FrozenToAlive();
				}
				else if (state == State.Dead)
				{
					FrozenToDead();
				}
			}
			lastState = state;
		}

		private IEnumerator AliveToDead(bool freeze)
		{
			isKilling = true;
			mode = Mode.Active;
			if (stateSettings.enableAngularLimitsOnKill && !angularLimits)
			{
				angularLimits = true;
				angularLimitsEnabledOnKill = true;
			}
			if (stateSettings.enableInternalCollisionsOnKill && !internalCollisions)
			{
				internalCollisions = true;
				internalCollisionsEnabledOnKill = true;
			}
			Muscle[] array = muscles;
			foreach (Muscle muscle in array)
			{
				muscle.state.pinWeightMlp = 0f;
				muscle.state.muscleDamperAdd = stateSettings.deadMuscleDamper;
				muscle.rigidbody.velocity = muscle.mappedVelocity;
				muscle.rigidbody.angularVelocity = muscle.mappedAngularVelocity;
			}
			float range = muscles[0].state.muscleWeightMlp - stateSettings.deadMuscleWeight;
			BehaviourBase[] array2 = behaviours;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].KillStart();
			}
			if (stateSettings.killDuration > 0f && range > 0f)
			{
				float mW = muscles[0].state.muscleWeightMlp;
				while (mW > stateSettings.deadMuscleWeight)
				{
					mW = Mathf.Max(mW - Time.deltaTime * (range / stateSettings.killDuration), stateSettings.deadMuscleWeight);
					array = muscles;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].state.muscleWeightMlp = mW;
					}
					yield return null;
				}
			}
			array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].state.muscleWeightMlp = stateSettings.deadMuscleWeight;
			}
			SetAnimationEnabled(false);
			isKilling = false;
			activeState = State.Dead;
			if (freeze)
			{
				freezeFlag = true;
			}
			array2 = behaviours;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].KillEnd();
			}
			if (OnDeath != null)
			{
				OnDeath();
			}
		}

		private void OnFreezeFlag()
		{
			if (!CanFreeze())
			{
				return;
			}
			SetAnimationEnabled(false);
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].joint.gameObject.SetActive(false);
			}
			BehaviourBase[] array2 = behaviours;
			foreach (BehaviourBase behaviourBase in array2)
			{
				behaviourBase.Freeze();
				if (behaviourBase.gameObject.activeSelf)
				{
					behaviourBase.deactivated = true;
					behaviourBase.gameObject.SetActive(false);
				}
			}
			freezeFlag = false;
			activeState = State.Frozen;
			if (OnFreeze != null)
			{
				OnFreeze();
			}
			if (stateSettings.freezePermanently)
			{
				if (behaviours.Length != 0 && behaviours[0] != null)
				{
					UnityEngine.Object.Destroy(behaviours[0].transform.parent.gameObject);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void DeadToAlive()
		{
			Muscle[] array = muscles;
			foreach (Muscle obj in array)
			{
				obj.state.pinWeightMlp = 1f;
				obj.state.muscleWeightMlp = 1f;
				obj.state.muscleDamperAdd = 0f;
			}
			if (angularLimitsEnabledOnKill)
			{
				angularLimits = false;
				angularLimitsEnabledOnKill = false;
			}
			if (internalCollisionsEnabledOnKill)
			{
				internalCollisions = false;
				internalCollisionsEnabledOnKill = false;
			}
			BehaviourBase[] array2 = behaviours;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Resurrect();
			}
			SetAnimationEnabled(true);
			activeState = State.Alive;
			if (OnResurrection != null)
			{
				OnResurrection();
			}
		}

		private void SetAnimationEnabled(bool to)
		{
			animatorDisabled = false;
			if (targetAnimator != null)
			{
				targetAnimator.enabled = to;
			}
			if (targetAnimation != null)
			{
				targetAnimation.enabled = to;
			}
		}

		private void DeadToFrozen()
		{
			freezeFlag = true;
		}

		private void FrozenToAlive()
		{
			freezeFlag = false;
			Muscle[] array = muscles;
			foreach (Muscle obj in array)
			{
				obj.state.pinWeightMlp = 1f;
				obj.state.muscleWeightMlp = 1f;
				obj.state.muscleDamperAdd = 0f;
			}
			if (angularLimitsEnabledOnKill)
			{
				angularLimits = false;
				angularLimitsEnabledOnKill = false;
			}
			if (internalCollisionsEnabledOnKill)
			{
				internalCollisions = false;
				internalCollisionsEnabledOnKill = false;
			}
			ActivateRagdoll();
			BehaviourBase[] array2 = behaviours;
			foreach (BehaviourBase behaviourBase in array2)
			{
				behaviourBase.Unfreeze();
				behaviourBase.Resurrect();
				if (behaviourBase.deactivated)
				{
					behaviourBase.gameObject.SetActive(true);
				}
			}
			if (targetAnimator != null)
			{
				targetAnimator.enabled = true;
			}
			if (targetAnimation != null)
			{
				targetAnimation.enabled = true;
			}
			activeState = State.Alive;
			if (OnUnfreeze != null)
			{
				OnUnfreeze();
			}
			if (OnResurrection != null)
			{
				OnResurrection();
			}
		}

		private void FrozenToDead()
		{
			freezeFlag = false;
			ActivateRagdoll();
			BehaviourBase[] array = behaviours;
			foreach (BehaviourBase behaviourBase in array)
			{
				behaviourBase.Unfreeze();
				if (behaviourBase.deactivated)
				{
					behaviourBase.gameObject.SetActive(true);
				}
			}
			activeState = State.Dead;
			if (OnUnfreeze != null)
			{
				OnUnfreeze();
			}
		}

		private void ActivateRagdoll(bool kinematic = false)
		{
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Reset();
			}
			array = muscles;
			foreach (Muscle obj in array)
			{
				obj.joint.gameObject.SetActive(true);
				obj.rigidbody.isKinematic = kinematic;
				obj.rigidbody.velocity = Vector3.zero;
				obj.rigidbody.angularVelocity = Vector3.zero;
			}
			internalCollisionsEnabled = true;
			SetInternalCollisions(internalCollisions);
			Read();
			array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MoveToTarget();
			}
		}

		private bool CanFreeze()
		{
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].rigidbody.velocity.sqrMagnitude > stateSettings.maxFreezeSqrVelocity)
				{
					return false;
				}
			}
			return true;
		}

		public void SampleTargetMappedState()
		{
			if (!CheckIfInitiated())
			{
				return;
			}
			sampleTargetMappedState = true;
			if (!targetMappedStateStored)
			{
				sampleTargetMappedState = true;
				return;
			}
			for (int i = 0; i < targetChildren.Length; i++)
			{
				targetSampledPositions[i] = targetMappedPositions[i];
				targetSampledRotations[i] = targetMappedRotations[i];
			}
			targetMappedStateSampled = true;
		}

		public void FixTargetToSampledState(float weight)
		{
			if (!CheckIfInitiated() || weight <= 0f || !targetMappedStateSampled)
			{
				return;
			}
			for (int i = 0; i < targetChildren.Length; i++)
			{
				if (targetChildren[i] == null)
				{
					Debug.LogWarning("PuppetMaster.UpdateTargetHierarchy() needs to be called when any of the child Transforms of the targetRoot are unparented or removed.", base.transform);
					return;
				}
				if (i == 0)
				{
					targetChildren[i].position = Vector3.Lerp(targetChildren[i].position, targetSampledPositions[i], weight);
					targetChildren[i].rotation = Quaternion.Lerp(targetChildren[i].rotation, targetSampledRotations[i], weight);
				}
				else
				{
					targetChildren[i].position = Vector3.Lerp(targetChildren[i].position, targetSampledPositions[0] + targetSampledRotations[0] * targetSampledPositions[i], weight);
					targetChildren[i].rotation = Quaternion.Lerp(targetChildren[i].rotation, targetSampledRotations[0] * targetSampledRotations[i], weight);
				}
			}
			Muscle[] array = muscles;
			foreach (Muscle muscle in array)
			{
				muscle.positionOffset = muscle.target.position - muscle.rigidbody.position;
			}
		}

		public void StoreTargetMappedState()
		{
			if (!CheckIfInitiated() || !storeTargetMappedState)
			{
				return;
			}
			for (int i = 0; i < targetChildren.Length; i++)
			{
				if (i == 0)
				{
					targetMappedPositions[i] = targetChildren[i].position;
					targetMappedRotations[i] = targetChildren[i].rotation;
				}
				else
				{
					targetMappedPositions[i] = Quaternion.Inverse(targetChildren[0].rotation) * (targetChildren[i].position - targetChildren[0].position);
					targetMappedRotations[i] = Quaternion.Inverse(targetChildren[0].rotation) * targetChildren[i].rotation;
				}
			}
			targetMappedStateStored = true;
			if (sampleTargetMappedState)
			{
				SampleTargetMappedState();
			}
			sampleTargetMappedState = false;
		}

		private void UpdateHierarchies()
		{
			targetChildren = new Transform[muscles.Length];
			for (int i = 0; i < muscles.Length; i++)
			{
				targetChildren[i] = muscles[i].target;
			}
			targetMappedPositions = new Vector3[targetChildren.Length];
			targetMappedRotations = new Quaternion[targetChildren.Length];
			targetSampledPositions = new Vector3[targetChildren.Length];
			targetSampledRotations = new Quaternion[targetChildren.Length];
			targetMappedStateStored = false;
			targetMappedStateSampled = false;
			AssignParentAndChildIndexes();
			AssignKinshipDegrees();
			UpdateBroadcasterMuscleIndexes();
			internalCollisionsEnabled = !internalCollisions;
			SetInternalCollisions(internalCollisions);
			angularLimitsEnabled = !angularLimits;
			SetAngularLimits(angularLimits);
			hasProp = HasProp();
			if (OnHierarchyChanged != null)
			{
				OnHierarchyChanged();
			}
		}

		private bool HasProp()
		{
			Muscle[] array = muscles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].props.group == Muscle.Group.Prop)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateBroadcasterMuscleIndexes()
		{
			for (int i = 0; i < muscles.Length; i++)
			{
				if (muscles[i].broadcaster != null)
				{
					muscles[i].broadcaster.muscleIndex = i;
				}
				if (muscles[i].jointBreakBroadcaster != null)
				{
					muscles[i].jointBreakBroadcaster.muscleIndex = i;
				}
			}
		}

		private void AssignParentAndChildIndexes()
		{
			for (int i = 0; i < muscles.Length; i++)
			{
				muscles[i].parentIndexes = new int[0];
				if (muscles[i].joint.connectedBody != null)
				{
					AddToParentsRecursive(muscles[i].joint.connectedBody.GetComponent<ConfigurableJoint>(), ref muscles[i].parentIndexes);
				}
				muscles[i].childIndexes = new int[0];
				muscles[i].childFlags = new bool[muscles.Length];
				for (int j = 0; j < muscles.Length; j++)
				{
					if (i != j && muscles[j].joint.connectedBody == muscles[i].rigidbody)
					{
						AddToChildrenRecursive(muscles[j].joint, ref muscles[i].childIndexes, ref muscles[i].childFlags);
					}
				}
			}
		}

		private void AddToParentsRecursive(ConfigurableJoint joint, ref int[] indexes)
		{
			if (joint == null)
			{
				return;
			}
			int muscleIndexLowLevel = GetMuscleIndexLowLevel(joint);
			if (muscleIndexLowLevel != -1)
			{
				Array.Resize(ref indexes, indexes.Length + 1);
				indexes[indexes.Length - 1] = muscleIndexLowLevel;
				if (!(joint.connectedBody == null))
				{
					AddToParentsRecursive(joint.connectedBody.GetComponent<ConfigurableJoint>(), ref indexes);
				}
			}
		}

		private void AddToChildrenRecursive(ConfigurableJoint joint, ref int[] indexes, ref bool[] childFlags)
		{
			if (joint == null)
			{
				return;
			}
			int muscleIndexLowLevel = GetMuscleIndexLowLevel(joint);
			if (muscleIndexLowLevel == -1)
			{
				return;
			}
			Array.Resize(ref indexes, indexes.Length + 1);
			indexes[indexes.Length - 1] = muscleIndexLowLevel;
			childFlags[muscleIndexLowLevel] = true;
			for (int i = 0; i < muscles.Length; i++)
			{
				if (i != muscleIndexLowLevel && muscles[i].joint.connectedBody == joint.GetComponent<Rigidbody>())
				{
					AddToChildrenRecursive(muscles[i].joint, ref indexes, ref childFlags);
				}
			}
		}

		private void AssignKinshipDegrees()
		{
			for (int i = 0; i < muscles.Length; i++)
			{
				muscles[i].kinshipDegrees = new int[muscles.Length];
				AssignKinshipsDownRecursive(ref muscles[i].kinshipDegrees, 1, i);
				AssignKinshipsUpRecursive(ref muscles[i].kinshipDegrees, 1, i);
			}
		}

		private void AssignKinshipsDownRecursive(ref int[] kinshipDegrees, int degree, int index)
		{
			for (int i = 0; i < muscles.Length; i++)
			{
				if (i != index && muscles[i].joint.connectedBody == muscles[index].rigidbody)
				{
					kinshipDegrees[i] = degree;
					AssignKinshipsDownRecursive(ref kinshipDegrees, degree + 1, i);
				}
			}
		}

		private void AssignKinshipsUpRecursive(ref int[] kinshipDegrees, int degree, int index)
		{
			for (int i = 0; i < muscles.Length; i++)
			{
				if (i == index || !(muscles[i].rigidbody == muscles[index].joint.connectedBody))
				{
					continue;
				}
				kinshipDegrees[i] = degree;
				AssignKinshipsUpRecursive(ref kinshipDegrees, degree + 1, i);
				for (int j = 0; j < muscles.Length; j++)
				{
					if (j != i && j != index && muscles[j].joint.connectedBody == muscles[i].rigidbody)
					{
						kinshipDegrees[j] = degree + 1;
						AssignKinshipsDownRecursive(ref kinshipDegrees, degree + 2, j);
					}
				}
			}
		}

		private int GetMuscleIndexLowLevel(ConfigurableJoint joint)
		{
			for (int i = 0; i < muscles.Length; i++)
			{
				if (muscles[i].joint == joint)
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsValid(bool log)
		{
			if (muscles == null)
			{
				if (log)
				{
					Debug.LogWarning("PuppetMaster Muscles is null.", base.transform);
				}
				return false;
			}
			if (muscles.Length == 0)
			{
				if (log)
				{
					Debug.LogWarning("PuppetMaster has no muscles.", base.transform);
				}
				return false;
			}
			for (int i = 0; i < muscles.Length; i++)
			{
				if (muscles[i] == null)
				{
					if (log)
					{
						Debug.LogWarning("Muscle is null, PuppetMaster muscle setup is invalid.", base.transform);
					}
					return false;
				}
				if (!muscles[i].IsValid(log))
				{
					return false;
				}
			}
			if (targetRoot == null)
			{
				if (log)
				{
					Debug.LogWarning("'Target Root' of PuppetMaster is null.");
				}
				return false;
			}
			if (targetRoot.position != base.transform.position)
			{
				if (log)
				{
					Debug.LogWarning("The position of the animated character (Target) must match with the position of the PuppetMaster when initiating PuppetMaster. If you are creating the Puppet in runtime, make sure you don't move the Target to another position immediatelly after instantiation. Move the Root Transform instead.");
				}
				return false;
			}
			if (targetRoot == null)
			{
				if (log)
				{
					Debug.LogWarning("Invalid PuppetMaster setup. (targetRoot not found)", base.transform);
				}
				return false;
			}
			for (int j = 0; j < muscles.Length; j++)
			{
				for (int k = 0; k < muscles.Length; k++)
				{
					if (j != k && (muscles[j] == muscles[k] || muscles[j].joint == muscles[k].joint))
					{
						if (log)
						{
							Debug.LogWarning("Joint " + muscles[j].joint.name + " is used by multiple muscles (indexes " + j + " and " + k + "), PuppetMaster muscle setup is invalid.", base.transform);
						}
						return false;
					}
				}
			}
			if (muscles[0].joint.connectedBody != null && muscles.Length > 1)
			{
				for (int l = 1; l < muscles.Length; l++)
				{
					if (muscles[l].joint.GetComponent<Rigidbody>() == muscles[0].joint.connectedBody)
					{
						if (log)
						{
							Debug.LogWarning("The first muscle needs to be the one that all the others are connected to (the hips).", base.transform);
						}
						return false;
					}
				}
			}
			for (int m = 0; m < muscles.Length; m++)
			{
				if (Vector3.SqrMagnitude(muscles[m].joint.transform.position - muscles[m].target.position) > 0.001f)
				{
					if (log)
					{
						Debug.LogWarning("The position of each muscle needs to match with the position of it's target. Muscle '" + muscles[m].joint.name + "' position does not match with it's target. Right-click on the PuppetMaster component's header and select 'Fix Muscle Positions' from the context menu.", muscles[m].joint.transform);
					}
					return false;
				}
			}
			CheckMassVariation(100f, true);
			return true;
		}

		private bool CheckMassVariation(float threshold, bool log)
		{
			float num = float.PositiveInfinity;
			float num2 = 0f;
			for (int i = 0; i < muscles.Length; i++)
			{
				float mass = muscles[i].joint.GetComponent<Rigidbody>().mass;
				if (mass < num)
				{
					num = mass;
				}
				if (mass > num2)
				{
					num2 = mass;
				}
			}
			if (num2 / num > threshold)
			{
				if (log)
				{
					Debug.LogWarning("Mass variation between the Rigidbodies in the ragdoll is more than " + threshold.ToString() + " times. This might cause instability and unwanted results with Rigidbodies connected by Joints. Min mass: " + num + ", max mass: " + num2, base.transform);
				}
				return false;
			}
			return true;
		}

		private bool CheckIfInitiated()
		{
			if (!initiated)
			{
				Debug.LogError("PuppetMaster has not been initiated yet.");
			}
			return initiated;
		}
	}
}
