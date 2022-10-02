using UnityEngine;

namespace RootMotion.Dynamics
{
	public class AnimationBlocker : MonoBehaviour
	{
		private void LateUpdate()
		{
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}
	}
}
