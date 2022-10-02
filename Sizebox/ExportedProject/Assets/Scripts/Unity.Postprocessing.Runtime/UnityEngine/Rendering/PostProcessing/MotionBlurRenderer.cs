using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	[Preserve]
	internal sealed class MotionBlurRenderer : PostProcessEffectRenderer<MotionBlur>
	{
		private enum Pass
		{
			VelocitySetup = 0,
			TileMax1 = 1,
			TileMax2 = 2,
			TileMaxV = 3,
			NeighborMax = 4,
			Reconstruction = 5
		}

		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}

		private void CreateTemporaryRT(PostProcessRenderContext context, int nameID, int width, int height, RenderTextureFormat RTFormat)
		{
			CommandBuffer command = context.command;
			RenderTextureDescriptor descriptor = context.GetDescriptor(0, RTFormat, RenderTextureReadWrite.Linear);
			descriptor.width = width;
			descriptor.height = height;
			command.GetTemporaryRT(nameID, descriptor, FilterMode.Point);
		}

		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			if (m_ResetHistory)
			{
				command.BlitFullscreenTriangle(context.source, context.destination);
				m_ResetHistory = false;
				return;
			}
			RenderTextureFormat rTFormat = RenderTextureFormat.RGHalf;
			RenderTextureFormat rTFormat2 = (RenderTextureFormat.ARGB2101010.IsSupported() ? RenderTextureFormat.ARGB2101010 : RenderTextureFormat.ARGB32);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.motionBlur);
			command.BeginSample("MotionBlur");
			int num = (int)(5f * (float)context.height / 100f);
			int num2 = ((num - 1) / 8 + 1) * 8;
			float value = (float)base.settings.shutterAngle / 360f;
			propertySheet.properties.SetFloat(ShaderIDs.VelocityScale, value);
			propertySheet.properties.SetFloat(ShaderIDs.MaxBlurRadius, num);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxBlurRadius, 1f / (float)num);
			int velocityTex = ShaderIDs.VelocityTex;
			CreateTemporaryRT(context, velocityTex, context.width, context.height, rTFormat2);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, velocityTex, propertySheet, 0);
			int tile2RT = ShaderIDs.Tile2RT;
			CreateTemporaryRT(context, tile2RT, context.width / 2, context.height / 2, rTFormat);
			command.BlitFullscreenTriangle(velocityTex, tile2RT, propertySheet, 1);
			int tile4RT = ShaderIDs.Tile4RT;
			CreateTemporaryRT(context, tile4RT, context.width / 4, context.height / 4, rTFormat);
			command.BlitFullscreenTriangle(tile2RT, tile4RT, propertySheet, 2);
			command.ReleaseTemporaryRT(tile2RT);
			int tile8RT = ShaderIDs.Tile8RT;
			CreateTemporaryRT(context, tile8RT, context.width / 8, context.height / 8, rTFormat);
			command.BlitFullscreenTriangle(tile4RT, tile8RT, propertySheet, 2);
			command.ReleaseTemporaryRT(tile4RT);
			Vector2 vector = Vector2.one * ((float)num2 / 8f - 1f) * -0.5f;
			propertySheet.properties.SetVector(ShaderIDs.TileMaxOffs, vector);
			propertySheet.properties.SetFloat(ShaderIDs.TileMaxLoop, (int)((float)num2 / 8f));
			int tileVRT = ShaderIDs.TileVRT;
			CreateTemporaryRT(context, tileVRT, context.width / num2, context.height / num2, rTFormat);
			command.BlitFullscreenTriangle(tile8RT, tileVRT, propertySheet, 3);
			command.ReleaseTemporaryRT(tile8RT);
			int neighborMaxTex = ShaderIDs.NeighborMaxTex;
			CreateTemporaryRT(context, neighborMaxTex, context.width / num2, context.height / num2, rTFormat);
			command.BlitFullscreenTriangle(tileVRT, neighborMaxTex, propertySheet, 4);
			command.ReleaseTemporaryRT(tileVRT);
			propertySheet.properties.SetFloat(ShaderIDs.LoopCount, Mathf.Clamp((int)base.settings.sampleCount / 2, 1, 64));
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5);
			command.ReleaseTemporaryRT(velocityTex);
			command.ReleaseTemporaryRT(neighborMaxTex);
			command.EndSample("MotionBlur");
		}
	}
}
