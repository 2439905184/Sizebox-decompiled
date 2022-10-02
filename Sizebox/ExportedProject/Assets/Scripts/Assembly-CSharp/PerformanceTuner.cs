using System.Collections.Generic;
using Assets.Scripts.PerformanceTuner;
using UnityEngine;

[DisallowMultipleComponent]
public class PerformanceTuner : MonoBehaviour
{
	private void Start()
	{
		IPerformanceProfile performanceProfile = GetPerformanceProfile(QualitySettings.GetQualityLevel());
		IEnumerable<Light> lights = Resources.FindObjectsOfTypeAll<Light>();
		IEnumerable<ReflectionProbe> reflections = Resources.FindObjectsOfTypeAll<ReflectionProbe>();
		SetLights(performanceProfile, lights);
		SetReflections(performanceProfile, reflections);
	}

	private static void SetLights(IPerformanceProfile profile, IEnumerable<Light> lights)
	{
		foreach (Light light in lights)
		{
			light.shadows = profile.ShadowType;
			light.shadowResolution = profile.ShadowResolution;
		}
	}

	private static void SetReflections(IPerformanceProfile profile, IEnumerable<ReflectionProbe> reflections)
	{
		foreach (ReflectionProbe reflection in reflections)
		{
			if (!reflection.gameObject.CompareTag("MainCamera"))
			{
				reflection.hdr = profile.EnableHdr;
				reflection.resolution = profile.ReflectionResolution;
				reflection.shadowDistance = profile.ShadowDistance;
			}
		}
	}

	private static IPerformanceProfile GetPerformanceProfile(int qualityLevel)
	{
		Debug.Log("Overwriting scene lights and reflection to quality level " + qualityLevel);
		switch (qualityLevel)
		{
		case 0:
			return new Fastest();
		case 1:
			return new Fast();
		case 2:
			return new Simple();
		case 3:
			return new Good();
		case 4:
			return new Beautiful();
		case 5:
			return new Fantastic();
		default:
			Debug.LogError("Unkown profile error, reverting to 0!");
			return new Fastest();
		}
	}
}
