Shader "MMD4Mecanim/MMDLit-Transparent-Edge" {
	Properties {
		_Color ("Diffuse", Vector) = (1,1,1,1)
		_Specular ("Specular", Vector) = (1,1,1,1)
		_Ambient ("Ambient", Vector) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 0
		_ShadowLum ("ShadowLum", Range(0, 10)) = 1.5
		_AmbientRate ("AmbientRate", Range(0, 2)) = 0.8
		_AmbientToDiffuse ("AmbientToDiffuse", Float) = 5
		_EdgeColor ("EdgeColor", Vector) = (0,0,0,1)
		_EdgeScale ("EdgeScale", Range(0, 2)) = 0
		_EdgeSize ("EdgeSize", Float) = 0
		_MainTex ("MainTex", 2D) = "white" {}
		_ToonTex ("ToonTex", 2D) = "white" {}
		_SphereCube ("SphereCube", Cube) = "white" {}
		_SphereMode ("SphereMode", Float) = -1
		_Emissive ("Emissive", Vector) = (0,0,0,0)
		_ALPower ("ALPower", Float) = 0
		_AddLightToonCen ("AddLightToonCen", Float) = -0.1
		_AddLightToonMin ("AddLightToonMin", Float) = 0.5
		_Revision ("Revision", Float) = -1
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
	//CustomEditor "MMD4MecanimMaterialInspector"
}