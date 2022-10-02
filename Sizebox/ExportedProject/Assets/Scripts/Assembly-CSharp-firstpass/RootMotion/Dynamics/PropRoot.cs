using UnityEngine;

namespace RootMotion.Dynamics
{
	[HelpURL("http://root-motion.com/puppetmasterdox/html/page6.html")]
	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Prop Root")]
	public class PropRoot : MonoBehaviour
	{
		[Tooltip("Reference to the PuppetMaster component.")]
		public PuppetMaster puppetMaster;

		[Tooltip("If a prop is connected, what will it's joint be connected to?")]
		public Rigidbody connectTo;

		[Tooltip("Is there a Prop connected to this PropRoot? Simply assign this value to connect, replace or drop props.")]
		public Prop currentProp;

		private Prop lastProp;

		private bool fixedUpdateCalled;

		[ContextMenu("User Manual")]
		private void OpenUserManual()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page6.html");
		}

		[ContextMenu("Scrpt Reference")]
		private void OpenScriptReference()
		{
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_prop_root.html");
		}

		public void DropImmediate()
		{
			if (!(lastProp == null))
			{
				puppetMaster.RemoveMuscleRecursive(lastProp.muscle, true);
				lastProp.Drop();
				currentProp = null;
				lastProp = null;
			}
		}

		private void Awake()
		{
			if (currentProp != null)
			{
				currentProp.StartPickedUp(this);
			}
		}

		private void Update()
		{
			if (fixedUpdateCalled && currentProp != null && lastProp == currentProp && currentProp.muscle.connectedBody == null)
			{
				currentProp.Drop();
				currentProp = null;
				lastProp = null;
			}
		}

		private void FixedUpdate()
		{
			fixedUpdateCalled = true;
			if (!(currentProp == lastProp))
			{
				if (currentProp == null)
				{
					puppetMaster.RemoveMuscleRecursive(lastProp.muscle, true);
					lastProp.Drop();
				}
				if (lastProp == null)
				{
					AttachProp(currentProp);
				}
				if (lastProp != null && currentProp != null)
				{
					puppetMaster.RemoveMuscleRecursive(lastProp.muscle, true);
					AttachProp(currentProp);
				}
				lastProp = currentProp;
			}
		}

		private void AttachProp(Prop prop)
		{
			prop.transform.position = base.transform.position;
			prop.transform.rotation = base.transform.rotation;
			prop.PickUp(this);
			puppetMaster.AddMuscle(prop.muscle, prop.transform, connectTo, base.transform, prop.muscleProps, false, prop.forceLayers);
			if (prop.additionalPin != null && prop.additionalPinTarget != null)
			{
				puppetMaster.AddMuscle(prop.additionalPin, prop.additionalPinTarget, prop.muscle.GetComponent<Rigidbody>(), prop.transform, new Muscle.Props(prop.additionalPinWeight, 0f, 0f, 0f, false, Muscle.Group.Prop), true, prop.forceLayers);
			}
		}
	}
}
