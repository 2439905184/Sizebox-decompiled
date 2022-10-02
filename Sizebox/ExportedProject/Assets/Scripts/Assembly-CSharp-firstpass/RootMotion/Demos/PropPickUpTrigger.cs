using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class PropPickUpTrigger : MonoBehaviour
	{
		public Prop prop;

		public LayerMask characterLayers;

		private CharacterPuppet characterPuppet;

		private void OnTriggerEnter(Collider collider)
		{
			if (!prop.isPickedUp && LayerMaskExtensions.Contains(characterLayers, collider.gameObject.layer))
			{
				characterPuppet = collider.GetComponent<CharacterPuppet>();
				if (!(characterPuppet == null) && characterPuppet.puppet.state == BehaviourPuppet.State.Puppet && !(characterPuppet.propRoot == null) && !(characterPuppet.propRoot.currentProp != null))
				{
					characterPuppet.propRoot.currentProp = prop;
				}
			}
		}
	}
}
