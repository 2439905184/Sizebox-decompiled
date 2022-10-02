using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class Grab : MonoBehaviour
	{
		[Tooltip("The PuppetMaster this muscle belongs to.")]
		public PuppetMaster puppetMaster;

		[Tooltip("Used for switching walk/run by default.")]
		public UserControlMelee userControl;

		[Tooltip("The layers we wish to grab (optimization).")]
		public int grabLayer;

		private bool grabbed;

		private Rigidbody r;

		private Collider c;

		private BehaviourPuppet otherPuppet;

		private Collider otherCollider;

		private ConfigurableJoint joint;

		private float nextGrabTime;

		private const float massMlp = 5f;

		private const int solverIterationMlp = 10;

		private void Start()
		{
			r = GetComponent<Rigidbody>();
			c = GetComponent<Collider>();
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (grabbed || Time.time < nextGrabTime || collision.gameObject.layer != grabLayer || collision.rigidbody == null)
			{
				return;
			}
			MuscleCollisionBroadcaster component = collision.gameObject.GetComponent<MuscleCollisionBroadcaster>();
			if (component == null || component.puppetMaster == puppetMaster)
			{
				return;
			}
			BehaviourBase[] behaviours = component.puppetMaster.behaviours;
			foreach (BehaviourBase behaviourBase in behaviours)
			{
				if (behaviourBase is BehaviourPuppet)
				{
					otherPuppet = behaviourBase as BehaviourPuppet;
					otherPuppet.SetState(BehaviourPuppet.State.Unpinned);
					otherPuppet.canGetUp = false;
				}
			}
			if (!(otherPuppet == null))
			{
				joint = base.gameObject.AddComponent<ConfigurableJoint>();
				joint.connectedBody = collision.rigidbody;
				joint.anchor = new Vector3(-0.35f, 0f, 0f);
				joint.xMotion = ConfigurableJointMotion.Locked;
				joint.yMotion = ConfigurableJointMotion.Locked;
				joint.zMotion = ConfigurableJointMotion.Locked;
				joint.angularXMotion = ConfigurableJointMotion.Locked;
				joint.angularYMotion = ConfigurableJointMotion.Locked;
				joint.angularZMotion = ConfigurableJointMotion.Locked;
				r.mass *= 5f;
				puppetMaster.solverIterationCount *= 10;
				otherCollider = collision.collider;
				Physics.IgnoreCollision(c, otherCollider, true);
				userControl.walkByDefault = true;
				grabbed = true;
			}
		}

		private void Update()
		{
			if (grabbed && Input.GetKeyDown(KeyCode.X))
			{
				Object.Destroy(joint);
				r.mass /= 5f;
				puppetMaster.solverIterationCount /= 10;
				userControl.walkByDefault = false;
				Physics.IgnoreCollision(c, otherCollider, false);
				otherPuppet.canGetUp = true;
				otherPuppet = null;
				otherCollider = null;
				grabbed = false;
				nextGrabTime = Time.time + 1f;
			}
		}
	}
}
