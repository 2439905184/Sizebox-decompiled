using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.PerformanceTuner
{
	internal class Fastest : IPerformanceProfile
	{
		public LightShadows ShadowType { get; private set; }

		public LightShadowResolution ShadowResolution { get; private set; }

		public int ReflectionResolution { get; private set; }

		public bool EnableHdr { get; private set; }

		public int ShadowDistance { get; private set; }

		public Fastest()
		{
			ShadowType = LightShadows.None;
			ShadowResolution = LightShadowResolution.Low;
			ReflectionResolution = 16;
			EnableHdr = false;
			ShadowDistance = 0;
		}
	}
}
