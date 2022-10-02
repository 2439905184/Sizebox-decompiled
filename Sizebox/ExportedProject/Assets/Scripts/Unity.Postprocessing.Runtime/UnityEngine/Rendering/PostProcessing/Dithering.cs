using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	[Preserve]
	internal sealed class Dithering
	{
		private int m_NoiseTextureIndex;

		private System.Random m_Random = new System.Random(1234);

		internal void Render(PostProcessRenderContext context)
		{
			Texture2D[] blueNoise = context.resources.blueNoise64;
			if (++m_NoiseTextureIndex >= blueNoise.Length)
			{
				m_NoiseTextureIndex = 0;
			}
			float z = (float)m_Random.NextDouble();
			float w = (float)m_Random.NextDouble();
			Texture2D texture2D = blueNoise[m_NoiseTextureIndex];
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.properties.SetTexture(ShaderIDs.DitheringTex, texture2D);
			uberSheet.properties.SetVector(ShaderIDs.Dithering_Coords, new Vector4((float)context.screenWidth / (float)texture2D.width, (float)context.screenHeight / (float)texture2D.height, z, w));
		}
	}
}
