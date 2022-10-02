using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class CreatePuppetInRuntime : MonoBehaviour
	{
		[Tooltip("Creating a Puppet from a ragdoll character prefab.")]
		public Transform ragdollPrefab;

		[Tooltip("What will the Puppet be named?")]
		public string instanceName = "My Character";

		[Tooltip("The layer to assign the character controller to. Collisions between this layer and the 'Ragdoll Layer' will be ignored, or else the ragdoll would collide with the character controller.")]
		public int characterControllerLayer;

		[Tooltip("The layer to assign the PuppetMaster and all it's muscles to. Collisions between this layer and the 'Character Controller Layer' will be ignored, or else the ragdoll would collide with the character controller.")]
		public int ragdollLayer;

		private void Start()
		{
			Transform obj = Object.Instantiate(ragdollPrefab, base.transform.position, base.transform.rotation);
			obj.name = instanceName;
			PuppetMaster.SetUp(obj, characterControllerLayer, ragdollLayer);
			Debug.Log("A ragdoll was successfully converted to a Puppet.");
		}
	}
}
