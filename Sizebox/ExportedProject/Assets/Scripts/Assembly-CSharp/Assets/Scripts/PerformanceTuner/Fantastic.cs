using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.PerformanceTuner
{
	internal class Fantastic : IPerformanceProfile
	{
		public LightShadows ShadowType { get; private set; }

		public LightShadowResolution ShadowResolution { get; private set; }

		public int ReflectionResolution { get; private set; }

		public bool EnableHdr { get; private set; }

		public int ShadowDistance { get; private set; }

		public Fantastic()
		{
			ShadowType = LightShadows.Soft;
			ShadowResolution = LightShadowResolution.VeryHigh;
			ReflectionResolution = 1024;
			EnableHdr = true;
			ShadowDistance = 100;
		}
	}
}
