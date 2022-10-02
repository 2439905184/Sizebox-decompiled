using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.PerformanceTuner
{
	internal class Good : IPerformanceProfile
	{
		public LightShadows ShadowType { get; private set; }

		public LightShadowResolution ShadowResolution { get; private set; }

		public int ReflectionResolution { get; private set; }

		public bool EnableHdr { get; private set; }

		public int ShadowDistance { get; private set; }

		public Good()
		{
			ShadowType = LightShadows.Hard;
			ShadowResolution = LightShadowResolution.High;
			ReflectionResolution = 256;
			EnableHdr = false;
			ShadowDistance = 40;
		}
	}
}
