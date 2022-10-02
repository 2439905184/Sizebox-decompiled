using UnityEngine;

namespace RootMotion.Dynamics
{
	public abstract class Prop : MonoBehaviour
	{
		[Tooltip("This has no other purpose but helping you distinguish props by PropRoot.currentProp.propType.")]
		public int propType;

		[LargeHeader("Muscle")]
		[Tooltip("The Muscle of this prop.")]
		public ConfigurableJoint muscle;

		[Tooltip("The muscle properties that will be applied to the Muscle on pickup.")]
		public Muscle.Props muscleProps = new Muscle.Props();

		[Tooltip("If true, this prop's layer will be forced to PuppetMaster layer and target's layer forced to PuppetMaster's Target Root's layer when the prop is picked up.")]
		public bool forceLayers = true;

		[LargeHeader("Additional Pin")]
		[Tooltip("Optinal additional pin, useful for long melee weapons that would otherwise require a lot of muscle force and solver iterations to be swinged quickly. Should normally be without any colliders attached. The position of the pin, it's mass and the pin weight will effect how the prop will handle.")]
		public ConfigurableJoint additionalPin;

		[Tooltip("Target Transform for the additional pin.")]
		public Transform additionalPinTarget;

		[Tooltip("The pin weight of the additional pin. Increasing this weight will make the prop follow animation better, but will increase jitter when colliding with objects.")]
		[Range(0f, 1f)]
		public float additionalPinWeight = 1f;

		private ConfigurableJointMotion xMotion;

		private ConfigurableJointMotion yMotion;

		private ConfigurableJointMotion zMotion;

		private ConfigurableJointMotion angularXMotion;

		private ConfigurableJointMotion angularYMotion;

		private ConfigurableJointMotion angularZMotion;

		private Collider[] colliders = new Collider[0];

		public bool isPickedUp
		{
			get
			{
				return propRoot != null;
			}
		}

		public PropRoot propRoot { get; private set; }

		public void PickUp(PropRoot propRoot)
		{
			muscle.xMotion = xMotion;
			muscle.yMotion = yMotion;
			muscle.zMotion = zMotion;
			muscle.angularXMotion = angularXMotion;
			muscle.angularYMotion = angularYMotion;
			muscle.angularZMotion = angularZMotion;
			this.propRoot = propRoot;
			muscle.gameObject.layer = propRoot.puppetMaster.gameObject.layer;
			Collider[] array = colliders;
			foreach (Collider collider in array)
			{
				if (!collider.isTrigger)
				{
					collider.gameObject.layer = muscle.gameObject.layer;
				}
			}
			OnPickUp(propRoot);
		}

		public void Drop()
		{
			propRoot = null;
			OnDrop();
		}

		public void StartPickedUp(PropRoot propRoot)
		{
			this.propRoot = propRoot;
		}

		protected virtual void OnPickUp(PropRoot propRoot)
		{
		}

		protected virtual void OnDrop()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void Awake()
		{
			if (base.transform.position != muscle.transform.position)
			{
				Debug.LogError("Prop target position must match exactly with it's muscle's position!", base.transform);
			}
			xMotion = muscle.xMotion;
			yMotion = muscle.yMotion;
			zMotion = muscle.zMotion;
			angularXMotion = muscle.angularXMotion;
			angularYMotion = muscle.angularYMotion;
			angularZMotion = muscle.angularZMotion;
			colliders = muscle.GetComponentsInChildren<Collider>();
		}

		private void Start()
		{
			if (!isPickedUp)
			{
				ReleaseJoint();
			}
			OnStart();
		}

		private void ReleaseJoint()
		{
			muscle.connectedBody = null;
			muscle.targetRotation = Quaternion.identity;
			JointDrive slerpDrive = default(JointDrive);
			slerpDrive.positionSpring = 0f;
			muscle.slerpDrive = slerpDrive;
			muscle.xMotion = ConfigurableJointMotion.Free;
			muscle.yMotion = ConfigurableJointMotion.Free;
			muscle.zMotion = ConfigurableJointMotion.Free;
			muscle.angularXMotion = ConfigurableJointMotion.Free;
			muscle.angularYMotion = ConfigurableJointMotion.Free;
			muscle.angularZMotion = ConfigurableJointMotion.Free;
		}

		private void OnDrawGizmos()
		{
			if (!(muscle == null) && !Application.isPlaying)
			{
				base.transform.position = muscle.transform.position;
				base.transform.rotation = muscle.transform.rotation;
				if (additionalPinTarget != null && additionalPin != null)
				{
					additionalPinTarget.position = additionalPin.transform.position;
				}
				muscleProps.group = Muscle.Group.Prop;
			}
		}
	}
}
