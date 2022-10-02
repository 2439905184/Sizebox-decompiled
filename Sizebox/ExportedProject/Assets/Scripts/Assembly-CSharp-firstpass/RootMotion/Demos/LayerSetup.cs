using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	[RequireComponent(typeof(PuppetMaster))]
	public class LayerSetup : MonoBehaviour
	{
		[Header("References")]
		[Tooltip("Reference to the character controller gameobject (the one that has the capsule collider/CharacterController.")]
		public Transform characterController;

		[Header("Layers")]
		[Tooltip("The layer to assign the character controller to. Collisions between this layer and the 'Ragdoll Layer' will be ignored, or else the ragdoll would collide with the character controller.")]
		public int characterControllerLayer;

		[Tooltip("The layer to assign the PuppetMaster and all it's muscles to. Collisions between this layer and the 'Character Controller Layer' will be ignored, or else the ragdoll would collide with the character controller.")]
		public int ragdollLayer;

		[Tooltip("Layers that will be ignored by the character controller")]
		public LayerMask ignoreCollisionWithCharacterController;

		[Tooltip("Layers that will not collide with the Ragdoll layer.")]
		public LayerMask ignoreCollisionWithRagdoll;

		private PuppetMaster puppetMaster;

		private void Awake()
		{
			puppetMaster = GetComponent<PuppetMaster>();
			puppetMaster.gameObject.layer = ragdollLayer;
			Muscle[] muscles = puppetMaster.muscles;
			for (int i = 0; i < muscles.Length; i++)
			{
				muscles[i].joint.gameObject.layer = ragdollLayer;
			}
			characterController.gameObject.layer = characterControllerLayer;
			Physics.IgnoreLayerCollision(characterControllerLayer, ragdollLayer);
			Physics.IgnoreLayerCollision(characterControllerLayer, characterControllerLayer);
			int[] array = ignoreCollisionWithCharacterController.MaskToNumbers();
			foreach (int layer in array)
			{
				Physics.IgnoreLayerCollision(characterControllerLayer, layer);
			}
			array = ignoreCollisionWithRagdoll.MaskToNumbers();
			foreach (int layer2 in array)
			{
				Physics.IgnoreLayerCollision(ragdollLayer, layer2);
			}
			Object.Destroy(this);
		}
	}
}
