namespace UnityEngine.InputSystem.XR.Haptics
{
	public struct HapticState
	{
		public uint samplesQueued { get; private set; }

		public uint samplesAvailable { get; private set; }

		public HapticState(uint samplesQueued, uint samplesAvailable)
		{
			this.samplesQueued = samplesQueued;
			this.samplesAvailable = samplesAvailable;
		}
	}
}
