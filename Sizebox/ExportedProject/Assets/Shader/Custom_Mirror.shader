Shader "Custom/Mirror" {
	Properties {
		_MainTex ("Emissive Texture", 2D) = "black" {}
		_DetailTex ("Detail Texture", 2D) = "white" {}
		_Color ("Detail Tint Color", Vector) = (1,1,1,1)
		_SpecColor ("Specular Color", Vector) = (1,1,1,1)
		_SpecularArea ("Specular Area", Range(0, 0.99)) = 0.1
		_SpecularIntensity ("Specular Intensity", Range(0, 1)) = 0.75
		_ReflectionColor ("Reflection Tint Color", Vector) = (1,1,1,1)
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
	Fallback "Reflective/Specular"
}