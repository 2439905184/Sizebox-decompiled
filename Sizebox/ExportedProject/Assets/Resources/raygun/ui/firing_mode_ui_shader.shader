Shader "Custom/firing_mode_ui_shader" {
	Properties {
		[PerRendererData] _MainTex ("MainTex", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		_MaskTex ("MaskTex", 2D) = "white" {}
		_mode ("mode", Float) = 1
		_alpha ("alpha", Float) = 1
		_UI_Color ("UI_Color", Vector) = (0,0,0,1)
		[HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Stencil ("Stencil ID", Float) = 0
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilComp ("Stencil Comparison", Float) = 8
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilOpFail ("Stencil Fail Operation", Float) = 0
		_StencilOpZFail ("Stencil Z-Fail Operation", Float) = 0
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
	Fallback "UI/Default"
	//CustomEditor "ShaderForgeMaterialInspector"
}