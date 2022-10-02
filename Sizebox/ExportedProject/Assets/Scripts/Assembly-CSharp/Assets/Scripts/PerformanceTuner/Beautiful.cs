using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.PerformanceTuner
{
	internal class Beautiful : IPerformanceProfile
	{
		public LightShadows ShadowType { get; private set; }

		public LightShadowResolution ShadowResolution { get; private set; }

		public int ReflectionResolution { get; private set; }

		public bool EnableHdr { get; private set; }

		public int ShadowDistance { get; private set; }

		public Beautiful()
		{
			ShadowType = LightShadows.Soft;
			ShadowResolution = LightShadowResolution.High;
			ReflectionResolution = 512;
			EnableHdr = true;
			ShadowDistance = 80;
		}
	}
}
