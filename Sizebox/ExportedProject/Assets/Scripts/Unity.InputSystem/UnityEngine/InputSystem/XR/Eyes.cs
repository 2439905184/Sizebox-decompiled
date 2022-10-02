namespace UnityEngine.InputSystem.XR
{
	public struct Eyes
	{
		public Vector3 leftEyePosition { get; set; }

		public Quaternion leftEyeRotation { get; set; }

		public Vector3 rightEyePosition { get; set; }

		public Quaternion rightEyeRotation { get; set; }

		public Vector3 fixationPoint { get; set; }

		public float leftEyeOpenAmount { get; set; }

		public float rightEyeOpenAmount { get; set; }
	}
}
