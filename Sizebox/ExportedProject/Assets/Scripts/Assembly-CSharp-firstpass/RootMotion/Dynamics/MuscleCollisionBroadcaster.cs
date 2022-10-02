using UnityEngine;

namespace RootMotion.Dynamics
{
	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Muscle Collision Broadcaster")]
	public class MuscleCollisionBroadcaster : MonoBehaviour
	{
		[SerializeField]
		[HideInInspector]
		public PuppetMaster puppetMaster;

		[SerializeField]
		[HideInInspector]
		public int muscleIndex;

		private const string onMuscleHit = "OnMuscleHit";

		private const string onMuscleCollision = "OnMuscleCollision";

		private const string onMuscleCollisionExit = "OnMuscleCollisionExit";

		private MuscleCollisionBroadcaster otherBroadcaster;

		public void Hit(float unPin, Vector3 force, Vector3 position)
		{
			if (base.enabled)
			{
				BehaviourBase[] behaviours = puppetMaster.behaviours;
				for (int i = 0; i < behaviours.Length; i++)
				{
					behaviours[i].OnMuscleHit(new MuscleHit(muscleIndex, unPin, force, position));
				}
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (base.enabled && !(puppetMaster == null) && !(collision.collider.transform.root == base.transform.root))
			{
				BehaviourBase[] behaviours = puppetMaster.behaviours;
				for (int i = 0; i < behaviours.Length; i++)
				{
					behaviours[i].OnMuscleCollision(new MuscleCollision(muscleIndex, collision));
				}
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			if (base.enabled && !(puppetMaster == null) && (!(Singleton<PuppetMasterSettings>.instance != null) || Singleton<PuppetMasterSettings>.instance.collisionStayMessages) && !(collision.collider.transform.root == base.transform.root))
			{
				BehaviourBase[] behaviours = puppetMaster.behaviours;
				for (int i = 0; i < behaviours.Length; i++)
				{
					behaviours[i].OnMuscleCollision(new MuscleCollision(muscleIndex, collision, true));
				}
			}
		}

		private void OnCollisionExit(Collision collision)
		{
			if (base.enabled && !(puppetMaster == null) && (!(Singleton<PuppetMasterSettings>.instance != null) || Singleton<PuppetMasterSettings>.instance.collisionExitMessages) && !(collision.collider.transform.root == base.transform.root))
			{
				BehaviourBase[] behaviours = puppetMaster.behaviours;
				for (int i = 0; i < behaviours.Length; i++)
				{
					behaviours[i].OnMuscleCollisionExit(new MuscleCollision(muscleIndex, collision));
				}
			}
		}
	}
}
