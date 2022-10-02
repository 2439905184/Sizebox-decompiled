Shader "Custom/ray_particle_shader_v2" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_TintColor ("Color", Vector) = (1,1,1,1)
		_mod1 ("mod1", 2D) = "white" {}
		_mod2 ("mod2", 2D) = "white" {}
		_mod3 ("mod3", 2D) = "white" {}
		_cloudnoise ("cloudnoise", 2D) = "white" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	//CustomEditor "ShaderForgeMaterialInspector"
}