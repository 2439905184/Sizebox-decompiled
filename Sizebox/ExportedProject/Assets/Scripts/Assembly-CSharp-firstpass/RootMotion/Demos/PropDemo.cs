using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class PropDemo : MonoBehaviour
	{
		[Tooltip("The Prop you wish to pick up.")]
		public Prop prop;

		[Tooltip("The PropRoot of the left hand.")]
		public PropRoot propRootLeft;

		[Tooltip("The PropRoot of the right hand.")]
		public PropRoot propRootRight;

		[Tooltip("If true, the prop will be picked up when PuppetMaster initiates")]
		public bool pickUpOnStart;

		private bool right = true;

		private PropRoot connectTo
		{
			get
			{
				if (!right)
				{
					return propRootLeft;
				}
				return propRootRight;
			}
		}

		private void Start()
		{
			if (pickUpOnStart)
			{
				connectTo.currentProp = prop;
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.P))
			{
				connectTo.currentProp = prop;
			}
			if (Input.GetKeyDown(KeyCode.X))
			{
				connectTo.currentProp = null;
			}
			if (Input.GetKeyDown(KeyCode.S))
			{
				connectTo.DropImmediate();
				right = !right;
				connectTo.currentProp = prop;
			}
		}
	}
}
