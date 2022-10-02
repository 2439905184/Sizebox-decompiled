namespace UnityEngine.InputSystem.XR
{
	public struct Bone
	{
		public uint parentBoneIndex { get; set; }

		public Vector3 position { get; set; }

		public Quaternion rotation { get; set; }
	}
}
