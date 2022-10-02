using UnityEngine;

namespace RootMotion.Demos
{
	public class RotateShoulderToTarget : MonoBehaviour
	{
		public Transform shoulder;

		public Vector3 euler;

		private void OnPuppetMasterRead()
		{
			shoulder.rotation = Quaternion.Euler(euler) * shoulder.rotation;
		}
	}
}
