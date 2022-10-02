Shader "Custom/laser_shader" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_TintColor ("Color", Vector) = (0.01,0.01,0.01,1)
		_Xspeed ("X speed", Float) = 0.2
		_Yspeed ("Y speed", Float) = 0
		_Main_tex2 ("Main_tex2", 2D) = "white" {}
		_Xspeed_copy ("X speed_copy", Float) = 0.1
		_Yspeed_copy ("Y speed_copy", Float) = 0
		_alpha ("alpha", Float) = 1
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