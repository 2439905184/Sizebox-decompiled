using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.PerformanceTuner
{
	internal interface IPerformanceProfile
	{
		LightShadows ShadowType { get; }

		LightShadowResolution ShadowResolution { get; }

		int ReflectionResolution { get; }

		bool EnableHdr { get; }

		int ShadowDistance { get; }
	}
}
