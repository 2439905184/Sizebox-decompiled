using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public sealed class GrayscaleRenderer : PostProcessEffectRenderer<Grayscale>
{
	private static readonly int Blend = Shader.PropertyToID("_Blend");

	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Grayscale"));
		propertySheet.properties.SetFloat(Blend, base.settings.blend);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
