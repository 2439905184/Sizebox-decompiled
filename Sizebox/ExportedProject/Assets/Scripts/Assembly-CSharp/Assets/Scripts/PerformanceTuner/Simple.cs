using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.PerformanceTuner
{
	internal class Simple : IPerformanceProfile
	{
		public LightShadows ShadowType { get; private set; }

		public LightShadowResolution ShadowResolution { get; private set; }

		public int ReflectionResolution { get; private set; }

		public bool EnableHdr { get; private set; }

		public int ShadowDistance { get; private set; }

		public Simple()
		{
			ShadowType = LightShadows.Hard;
			ShadowResolution = LightShadowResolution.Medium;
			ReflectionResolution = 128;
			EnableHdr = false;
			ShadowDistance = 20;
		}
	}
}
