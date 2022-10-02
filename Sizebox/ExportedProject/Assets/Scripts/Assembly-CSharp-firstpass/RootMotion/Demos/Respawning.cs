using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class Respawning : MonoBehaviour
	{
		[Tooltip("Pooled characters will be parented to this deactivated GameObject.")]
		public Transform pool;

		[Tooltip("Reference to the BehaviourPuppet component of the character you wish to respawn.")]
		public BehaviourPuppet puppet;

		[Tooltip("The animation to play on respawn.")]
		public string idleAnimation;

		private Transform puppetRoot;

		private bool isPooled
		{
			get
			{
				return puppet.transform.root == pool;
			}
		}

		private void Start()
		{
			puppetRoot = puppet.transform.root;
			pool.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				puppet.puppetMaster.state = PuppetMaster.State.Alive;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				puppet.puppetMaster.state = PuppetMaster.State.Dead;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				puppet.puppetMaster.state = PuppetMaster.State.Frozen;
			}
			if (Input.GetKeyDown(KeyCode.P) && !isPooled)
			{
				Pool();
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				Vector2 vector = Random.insideUnitCircle * 2f;
				Respawn(new Vector3(vector.x, 0f, vector.y), Quaternion.LookRotation(new Vector3(0f - vector.x, 0f, 0f - vector.y)));
			}
		}

		private void Pool()
		{
			puppetRoot.parent = pool;
		}

		private void Respawn(Vector3 position, Quaternion rotation)
		{
			puppet.puppetMaster.state = PuppetMaster.State.Alive;
			puppet.puppetMaster.targetAnimator.Play(idleAnimation, 0, 0f);
			puppet.SetState(BehaviourPuppet.State.Puppet);
			puppet.puppetMaster.Teleport(position, rotation, true);
			puppetRoot.parent = null;
		}
	}
}
