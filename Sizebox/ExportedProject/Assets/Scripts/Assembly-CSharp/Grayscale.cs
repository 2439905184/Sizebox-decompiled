using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(GrayscaleRenderer), PostProcessEvent.AfterStack, "Custom/Grayscale", true)]
public sealed class Grayscale : PostProcessEffectSettings
{
	[Range(0f, 1f)]
	[Tooltip("Grayscale effect intensity.")]
	public FloatParameter blend = new FloatParameter
	{
		value = 0.5f
	};
}
