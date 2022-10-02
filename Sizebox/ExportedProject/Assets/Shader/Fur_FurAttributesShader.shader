Shader "Fur/FurAttributesShader" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_Specular ("Specular", Vector) = (1,1,1,1)
		_Shininess ("Shininess", Range(0.01, 256)) = 8
		_MainTex ("Texture", 2D) = "white" {}
		_FurTex ("Fur Pattern", 2D) = "white" {}
		_FurLength ("Fur Length", Range(0, 15)) = 0.1
		_FurDensity ("Fur Density", Range(0, 2)) = 0.11
		_FurThinness ("Fur Thinness", Range(0.01, 10)) = 1
		_FurShading ("Fur Shading", Range(0, 1)) = 0.25
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}