Shader "MMD4Mecanim/MMDLit-NEXTEdge-Pass4" {
	Properties {
		_EdgeColor ("EdgeColor", Vector) = (0.4,1,1,1)
		_EdgeSize ("EdgeSize", Float) = 0.005
		_PostfixRenderQueue ("PostfixRenderQueue", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}