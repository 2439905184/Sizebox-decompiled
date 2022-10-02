using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class CreateRagdollInRuntime : MonoBehaviour
	{
		[Tooltip("The character prefab/FBX.")]
		public GameObject prefab;

		private void Start()
		{
			BipedRagdollReferences r = BipedRagdollReferences.FromAvatar(Object.Instantiate(prefab).GetComponent<Animator>());
			BipedRagdollCreator.Options options = BipedRagdollCreator.AutodetectOptions(r);
			BipedRagdollCreator.Create(r, options);
			Debug.Log("A ragdoll was successfully created.");
		}

		private void Update()
		{
		}
	}
}
