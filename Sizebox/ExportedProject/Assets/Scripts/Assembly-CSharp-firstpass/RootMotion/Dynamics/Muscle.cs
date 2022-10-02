using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[Serializable]
	public class Muscle
	{
		[Serializable]
		public enum Group
		{
			Hips = 0,
			Spine = 1,
			Head = 2,
			Arm = 3,
			Hand = 4,
			Leg = 5,
			Foot = 6,
			Tail = 7,
			Prop = 8
		}

		[Serializable]
		public class Props
		{
			[Tooltip("Which body part does this muscle belong to?")]
			public Group group;

			[Tooltip("The weight (multiplier) of mapping this muscle's target to the muscle.")]
			[Range(0f, 1f)]
			public float mappingWeight = 1f;

			[Tooltip("The weight (multiplier) of pinning this muscle to it's target's position using a simple AddForce command.")]
			[Range(0f, 1f)]
			public float pinWeight = 1f;

			[Tooltip("The muscle strength (multiplier).")]
			[Range(0f, 1f)]
			public float muscleWeight = 1f;

			[Tooltip("Multiplier of the positionDamper of the ConfigurableJoints' Slerp Drive.")]
			[Range(0f, 1f)]
			public float muscleDamper = 1f;

			[Tooltip("If true, will map the target to the world space position of the muscle. Normally this should be true for only the root muscle (the hips).")]
			public bool mapPosition;

			public Props()
			{
				mappingWeight = 1f;
				pinWeight = 1f;
				muscleWeight = 1f;
				muscleDamper = 1f;
			}

			public Props(float pinWeight, float muscleWeight, float mappingWeight, float muscleDamper, bool mapPosition, Group group = Group.Hips)
			{
				this.pinWeight = pinWeight;
				this.muscleWeight = muscleWeight;
				this.mappingWeight = mappingWeight;
				this.muscleDamper = muscleDamper;
				this.group = group;
				this.mapPosition = mapPosition;
			}

			public void Clamp()
			{
				mappingWeight = Mathf.Clamp(mappingWeight, 0f, 1f);
				pinWeight = Mathf.Clamp(pinWeight, 0f, 1f);
				muscleWeight = Mathf.Clamp(muscleWeight, 0f, 1f);
				muscleDamper = Mathf.Clamp(muscleDamper, 0f, 1f);
			}
		}

		public struct State
		{
			public float mappingWeightMlp;

			public float pinWeightMlp;

			public float muscleWeightMlp;

			public float maxForceMlp;

			public float muscleDamperMlp;

			public float muscleDamperAdd;

			public float immunity;

			public float impulseMlp;

			public Vector3 velocity;

			public Vector3 angularVelocity;

			public static State Default
			{
				get
				{
					State result = default(State);
					result.mappingWeightMlp = 1f;
					result.pinWeightMlp = 1f;
					result.muscleWeightMlp = 1f;
					result.muscleDamperMlp = 1f;
					result.muscleDamperAdd = 0f;
					result.maxForceMlp = 1f;
					result.immunity = 0f;
					result.impulseMlp = 1f;
					return result;
				}
			}

			public void Clamp()
			{
				mappingWeightMlp = Mathf.Clamp(mappingWeightMlp, 0f, 1f);
				pinWeightMlp = Mathf.Clamp(pinWeightMlp, 0f, 1f);
				muscleWeightMlp = Mathf.Clamp(muscleWeightMlp, 0f, muscleWeightMlp);
				immunity = Mathf.Clamp(immunity, 0f, 1f);
				impulseMlp = Mathf.Max(impulseMlp, 0f);
			}
		}

		[HideInInspector]
		public string name;

		public ConfigurableJoint joint;

		public Transform target;

		public Props props = new Props();

		public State state = State.Default;

		[HideInInspector]
		public int[] parentIndexes = new int[0];

		[HideInInspector]
		public int[] childIndexes = new int[0];

		[HideInInspector]
		public bool[] childFlags = new bool[0];

		[HideInInspector]
		public int[] kinshipDegrees = new int[0];

		[HideInInspector]
		public MuscleCollisionBroadcaster broadcaster;

		[HideInInspector]
		public JointBreakBroadcaster jointBreakBroadcaster;

		[HideInInspector]
		public Vector3 positionOffset;

		private JointDrive slerpDrive;

		private float lastJointDriveRotationWeight = -1f;

		private float lastRotationDamper = -1f;

		private Vector3 defaultPosition;

		private Vector3 defaultTargetLocalPosition;

		private Vector3 lastMappedPosition;

		private Quaternion defaultLocalRotation;

		private Quaternion localRotationConvert;

		private Quaternion toParentSpace;

		private Quaternion toJointSpaceInverse;

		private Quaternion toJointSpaceDefault;

		private Quaternion targetAnimatedRotation;

		private Quaternion targetAnimatedWorldRotation;

		private Quaternion defaultRotation;

		private Quaternion rotationRelativeToTarget;

		private Quaternion defaultTargetLocalRotation;

		private Quaternion lastMappedRotation;

		private Transform targetParent;

		private Transform connectedBodyTransform;

		private ConfigurableJointMotion angularXMotionDefault;

		private ConfigurableJointMotion angularYMotionDefault;

		private ConfigurableJointMotion angularZMotionDefault;

		private bool directTargetParent;

		private bool initiated;

		private Collider[] _colliders = new Collider[0];

		private float lastReadTime;

		private float lastWriteTime;

		private bool[] disabledColliders = new bool[0];

		public Transform transform { get; private set; }

		public Rigidbody rigidbody { get; private set; }

		public Transform connectedBodyTarget { get; private set; }

		public Vector3 targetAnimatedPosition { get; private set; }

		public Collider[] colliders
		{
			get
			{
				return _colliders;
			}
		}

		public Vector3 targetVelocity { get; private set; }

		public Vector3 targetAngularVelocity { get; private set; }

		public Vector3 mappedVelocity { get; private set; }

		public Vector3 mappedAngularVelocity { get; private set; }

		public Quaternion targetRotationRelative { get; private set; }

		private Quaternion localRotation
		{
			get
			{
				return Quaternion.Inverse(parentRotation) * transform.rotation;
			}
		}

		private Quaternion parentRotation
		{
			get
			{
				if (joint.connectedBody != null)
				{
					return joint.connectedBody.rotation;
				}
				if (transform.parent == null)
				{
					return Quaternion.identity;
				}
				return transform.parent.rotation;
			}
		}

		private Quaternion targetParentRotation
		{
			get
			{
				if (targetParent == null)
				{
					return Quaternion.identity;
				}
				return targetParent.rotation;
			}
		}

		private Quaternion targetLocalRotation
		{
			get
			{
				return Quaternion.Inverse(targetParentRotation * toParentSpace) * target.rotation;
			}
		}

		public bool IsValid(bool log)
		{
			if (joint == null)
			{
				if (log)
				{
					Debug.LogError("Muscle joint is null");
				}
				return false;
			}
			if (target == null)
			{
				if (log)
				{
					Debug.LogError("Muscle " + joint.name + "target is null, please remove the muscle from PuppetMaster or disable PuppetMaster before destroying a muscle's target.");
				}
				return false;
			}
			if (props == null && log)
			{
				Debug.LogError("Muscle " + joint.name + "props is null");
			}
			return true;
		}

		public virtual void Initiate(Muscle[] colleagues)
		{
			initiated = false;
			if (!IsValid(true))
			{
				return;
			}
			name = joint.name;
			state = State.Default;
			if (joint.connectedBody != null)
			{
				for (int i = 0; i < colleagues.Length; i++)
				{
					if (colleagues[i].joint.GetComponent<Rigidbody>() == joint.connectedBody)
					{
						connectedBodyTarget = colleagues[i].target;
					}
				}
			}
			transform = joint.transform;
			rigidbody = transform.GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			UpdateColliders();
			if (_colliders.Length == 0)
			{
				Vector3 size = Vector3.one * 0.1f;
				Renderer component = transform.GetComponent<Renderer>();
				if (component != null)
				{
					size = component.bounds.size;
				}
				rigidbody.inertiaTensor = CalculateInertiaTensorCuboid(size, rigidbody.mass);
			}
			targetParent = ((connectedBodyTarget != null) ? connectedBodyTarget : target.parent);
			defaultLocalRotation = localRotation;
			Vector3 normalized = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
			Vector3 normalized2 = Vector3.Cross(normalized, joint.axis).normalized;
			if (normalized == normalized2)
			{
				Debug.LogError("Joint " + joint.name + " secondaryAxis is in the exact same direction as it's axis. Please make sure they are not aligned.");
				return;
			}
			rotationRelativeToTarget = Quaternion.Inverse(target.rotation) * transform.rotation;
			Quaternion quaternion = Quaternion.LookRotation(normalized, normalized2);
			toJointSpaceInverse = Quaternion.Inverse(quaternion);
			toJointSpaceDefault = defaultLocalRotation * quaternion;
			toParentSpace = Quaternion.Inverse(targetParentRotation) * parentRotation;
			localRotationConvert = Quaternion.Inverse(targetLocalRotation) * localRotation;
			if (joint.connectedBody != null)
			{
				joint.autoConfigureConnectedAnchor = false;
				connectedBodyTransform = joint.connectedBody.transform;
				directTargetParent = target.parent == connectedBodyTarget;
			}
			angularXMotionDefault = joint.angularXMotion;
			angularYMotionDefault = joint.angularYMotion;
			angularZMotionDefault = joint.angularZMotion;
			if (joint.connectedBody == null)
			{
				props.mapPosition = true;
			}
			targetRotationRelative = Quaternion.Inverse(rigidbody.rotation) * target.rotation;
			if (joint.connectedBody == null)
			{
				defaultPosition = transform.localPosition;
				defaultRotation = transform.localRotation;
			}
			else
			{
				defaultPosition = joint.connectedBody.transform.InverseTransformPoint(transform.position);
				defaultRotation = Quaternion.Inverse(joint.connectedBody.transform.rotation) * transform.rotation;
			}
			defaultTargetLocalPosition = target.localPosition;
			defaultTargetLocalRotation = target.localRotation;
			joint.rotationDriveMode = RotationDriveMode.Slerp;
			if (!joint.gameObject.activeInHierarchy)
			{
				Debug.LogError("Can not initiate a puppet that has deactivated muscles.", joint.transform);
				return;
			}
			joint.configuredInWorldSpace = false;
			joint.projectionMode = JointProjectionMode.None;
			if (joint.anchor != Vector3.zero)
			{
				Debug.LogError("PuppetMaster joint anchors need to be Vector3.zero. Joint axis on " + transform.name + " is " + joint.anchor, transform);
			}
			else
			{
				targetAnimatedPosition = target.position;
				targetAnimatedWorldRotation = target.rotation;
				targetAnimatedRotation = targetLocalRotation * localRotationConvert;
				Read();
				lastReadTime = Time.time;
				lastWriteTime = Time.time;
				lastMappedPosition = target.position;
				lastMappedRotation = target.rotation;
				initiated = true;
			}
		}

		public void UpdateColliders()
		{
			_colliders = new Collider[0];
			AddColliders(joint.transform, ref _colliders, true);
			int childCount = joint.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				AddCompoundColliders(joint.transform.GetChild(i), ref _colliders);
			}
			disabledColliders = new bool[_colliders.Length];
		}

		public void DisableColliders()
		{
			for (int i = 0; i < _colliders.Length; i++)
			{
				disabledColliders[i] = _colliders[i].enabled;
				_colliders[i].enabled = false;
			}
		}

		public void EnableColliders()
		{
			for (int i = 0; i < _colliders.Length; i++)
			{
				if (disabledColliders[i])
				{
					_colliders[i].enabled = true;
				}
				disabledColliders[i] = false;
			}
		}

		private void AddColliders(Transform t, ref Collider[] C, bool includeMeshColliders)
		{
			Collider[] components = t.GetComponents<Collider>();
			int num = 0;
			Collider[] array = components;
			foreach (Collider obj in array)
			{
				bool flag = obj is MeshCollider;
				if (!obj.isTrigger && (!includeMeshColliders || !flag))
				{
					num++;
				}
			}
			if (num == 0)
			{
				return;
			}
			int num2 = C.Length;
			Array.Resize(ref C, num2 + num);
			int num3 = 0;
			for (int j = 0; j < components.Length; j++)
			{
				bool flag2 = components[j] is MeshCollider;
				if (!components[j].isTrigger && (!includeMeshColliders || !flag2))
				{
					C[num2 + num3] = components[j];
					num3++;
				}
			}
		}

		private void AddCompoundColliders(Transform t, ref Collider[] colliders)
		{
			if (!(t.GetComponent<Rigidbody>() != null))
			{
				AddColliders(t, ref colliders, false);
				int childCount = t.childCount;
				for (int i = 0; i < childCount; i++)
				{
					AddCompoundColliders(t.GetChild(i), ref colliders);
				}
			}
		}

		public void IgnoreCollisions(Muscle m, bool ignore)
		{
			Collider[] array = colliders;
			foreach (Collider collider in array)
			{
				Collider[] array2 = m.colliders;
				foreach (Collider collider2 in array2)
				{
					if (collider != null && collider2 != null && collider.enabled && collider2.enabled && collider.gameObject.activeInHierarchy && collider2.gameObject.activeInHierarchy)
					{
						Physics.IgnoreCollision(collider, collider2, ignore);
					}
				}
			}
		}

		public void IgnoreAngularLimits(bool ignore)
		{
			if (initiated)
			{
				joint.angularXMotion = (ignore ? ConfigurableJointMotion.Free : angularXMotionDefault);
				joint.angularYMotion = (ignore ? ConfigurableJointMotion.Free : angularYMotionDefault);
				joint.angularZMotion = (ignore ? ConfigurableJointMotion.Free : angularZMotionDefault);
			}
		}

		public void FixTargetTransforms()
		{
			if (initiated)
			{
				target.localPosition = defaultTargetLocalPosition;
				target.localRotation = defaultTargetLocalRotation;
			}
		}

		public void Reset()
		{
			if (initiated && !(joint == null))
			{
				if (joint.connectedBody == null)
				{
					transform.localPosition = defaultPosition;
					transform.localRotation = defaultRotation;
				}
				else
				{
					transform.position = joint.connectedBody.transform.TransformPoint(defaultPosition);
					transform.rotation = joint.connectedBody.transform.rotation * defaultRotation;
				}
			}
		}

		public void MoveToTarget()
		{
			if (initiated)
			{
				transform.position = target.position;
				transform.rotation = target.rotation * rotationRelativeToTarget;
			}
		}

		public void Read()
		{
			float num = Time.time - lastReadTime;
			lastReadTime = Time.time;
			if (num > 0f)
			{
				targetVelocity = (target.position - targetAnimatedPosition) / num;
				targetAngularVelocity = QuaTools.FromToRotation(targetAnimatedWorldRotation, target.rotation).eulerAngles / num;
			}
			targetAnimatedPosition = target.position;
			targetAnimatedWorldRotation = target.rotation;
			if (joint.connectedBody != null)
			{
				targetAnimatedRotation = targetLocalRotation * localRotationConvert;
			}
		}

		public void ClearVelocities()
		{
			targetVelocity = Vector3.zero;
			targetAngularVelocity = Vector3.zero;
			mappedVelocity = Vector3.zero;
			mappedAngularVelocity = Vector3.zero;
			targetAnimatedPosition = target.position;
			targetAnimatedWorldRotation = target.rotation;
			lastMappedPosition = target.position;
			lastMappedRotation = target.rotation;
		}

		public void UpdateAnchor(bool supportTranslationAnimation)
		{
			if (!(joint.connectedBody == null) && !(connectedBodyTarget == null) && (!directTargetParent || supportTranslationAnimation))
			{
				Vector3 vector2 = (joint.connectedAnchor = InverseTransformPointUnscaled(connectedBodyTarget.position, connectedBodyTarget.rotation * toParentSpace, target.position));
				Vector3 vector3 = vector2;
				float num = 1f / connectedBodyTransform.lossyScale.x;
				joint.connectedAnchor = vector3 * num;
			}
		}

		public virtual void Update(float pinWeightMaster, float muscleWeightMaster, float muscleSpring, float muscleDamper, float pinPow, float pinDistanceFalloff, bool rotationTargetChanged)
		{
			state.velocity = rigidbody.velocity;
			state.angularVelocity = rigidbody.angularVelocity;
			props.Clamp();
			state.Clamp();
			Pin(pinWeightMaster, pinPow, pinDistanceFalloff);
			if (rotationTargetChanged)
			{
				MuscleRotation(muscleWeightMaster, muscleSpring, muscleDamper);
			}
		}

		public void Map(float mappingWeightMaster)
		{
			float num = props.mappingWeight * mappingWeightMaster * state.mappingWeightMlp;
			if (num <= 0f)
			{
				return;
			}
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			if (num >= 1f)
			{
				target.rotation = rotation * targetRotationRelative;
				if (props.mapPosition)
				{
					if (connectedBodyTransform != null)
					{
						Vector3 position2 = connectedBodyTransform.InverseTransformPoint(position);
						target.position = connectedBodyTarget.TransformPoint(position2);
					}
					else
					{
						target.position = position;
					}
				}
				return;
			}
			target.rotation = Quaternion.Lerp(target.rotation, rotation * targetRotationRelative, num);
			if (props.mapPosition)
			{
				if (connectedBodyTransform != null)
				{
					Vector3 position3 = connectedBodyTransform.InverseTransformPoint(position);
					target.position = Vector3.Lerp(target.position, connectedBodyTarget.TransformPoint(position3), num);
				}
				else
				{
					target.position = Vector3.Lerp(target.position, position, num);
				}
			}
		}

		public void CalculateMappedVelocity()
		{
			float num = Time.time - lastWriteTime;
			if (num > 0f)
			{
				mappedVelocity = (target.position - lastMappedPosition) / num;
				mappedAngularVelocity = QuaTools.FromToRotation(lastMappedRotation, target.rotation).eulerAngles / num;
				lastWriteTime = Time.time;
			}
			lastMappedPosition = target.position;
			lastMappedRotation = target.rotation;
		}

		private void Pin(float pinWeightMaster, float pinPow, float pinDistanceFalloff)
		{
			positionOffset = targetAnimatedPosition - rigidbody.position;
			if (float.IsNaN(positionOffset.x))
			{
				positionOffset = Vector3.zero;
			}
			float num = pinWeightMaster * props.pinWeight * state.pinWeightMlp;
			if (!(num <= 0f))
			{
				num = Mathf.Pow(num, pinPow);
				Vector3 vector = positionOffset / Time.fixedDeltaTime;
				Vector3 vector2 = -rigidbody.velocity + targetVelocity + vector;
				vector2 *= num;
				if (pinDistanceFalloff > 0f)
				{
					vector2 /= 1f + positionOffset.sqrMagnitude * pinDistanceFalloff;
				}
				rigidbody.velocity += vector2;
			}
		}

		private void MuscleRotation(float muscleWeightMaster, float muscleSpring, float muscleDamper)
		{
			float num = muscleWeightMaster * props.muscleWeight * muscleSpring * state.muscleWeightMlp * 10f;
			if (joint.connectedBody == null)
			{
				num = 0f;
			}
			else if (num > 0f)
			{
				joint.targetRotation = LocalToJointSpace(targetAnimatedRotation);
			}
			float num2 = props.muscleDamper * muscleDamper * state.muscleDamperMlp + state.muscleDamperAdd;
			if (num != lastJointDriveRotationWeight || num2 != lastRotationDamper)
			{
				lastJointDriveRotationWeight = num;
				lastRotationDamper = num2;
				slerpDrive.positionSpring = num;
				slerpDrive.maximumForce = Mathf.Max(num, num2) * state.maxForceMlp;
				slerpDrive.positionDamper = num2;
				joint.slerpDrive = slerpDrive;
			}
		}

		private Quaternion LocalToJointSpace(Quaternion localRotation)
		{
			return toJointSpaceInverse * Quaternion.Inverse(localRotation) * toJointSpaceDefault;
		}

		private static Vector3 InverseTransformPointUnscaled(Vector3 position, Quaternion rotation, Vector3 point)
		{
			return Quaternion.Inverse(rotation) * (point - position);
		}

		private Vector3 CalculateInertiaTensorCuboid(Vector3 size, float mass)
		{
			float num = Mathf.Pow(size.x, 2f);
			float num2 = Mathf.Pow(size.y, 2f);
			float num3 = Mathf.Pow(size.z, 2f);
			float num4 = 1f / 12f * mass;
			return new Vector3(num4 * (num2 + num3), num4 * (num + num3), num4 * (num + num2));
		}
	}
}
