using RootMotion.FinalIK;
using UnityEngine;

namespace RootMotion.Demos
{
	public class VRIKPlatform : MonoBehaviour
	{
		public VRIK ik;

		private Vector3 lastPosition;

		private Quaternion lastRotation = Quaternion.identity;

		private void Start()
		{
			lastPosition = base.transform.position;
			lastRotation = base.transform.rotation;
		}

		private void Update()
		{
			ik.solver.AddPlatformMotion(base.transform.position - lastPosition, base.transform.rotation * Quaternion.Inverse(lastRotation), base.transform.position);
			lastRotation = base.transform.rotation;
			lastPosition = base.transform.position;
		}
	}
}
