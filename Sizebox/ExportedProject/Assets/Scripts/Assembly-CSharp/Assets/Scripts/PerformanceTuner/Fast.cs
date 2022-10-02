using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.PerformanceTuner
{
	internal class Fast : IPerformanceProfile
	{
		public LightShadows ShadowType { get; private set; }

		public LightShadowResolution ShadowResolution { get; private set; }

		public int ReflectionResolution { get; private set; }

		public bool EnableHdr { get; private set; }

		public int ShadowDistance { get; private set; }

		public Fast()
		{
			ShadowType = LightShadows.None;
			ShadowResolution = LightShadowResolution.Low;
			ReflectionResolution = 32;
			EnableHdr = false;
			ShadowDistance = 0;
		}
	}
}
