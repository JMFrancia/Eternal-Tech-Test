// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Heathen/Mobile/Glow/DiffuseBars" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_GlowColor ("Glow Color", Color) = (1,1,1,1)
	_Inner ("Inner Intensity", Range(0.0,10)) = 2.0
	_Outter ("Outter Intensity", Range(0.0,10)) = 2.0
	_GlowMap ("Glow (A)", 2D) = "white" {}

    _Color ("Color", Color) = (1,1,1,1)
    _LineWidth ("Percent Fill", Range(0, 1)) = 0.5
    _LineAmount ("Ring Amount", Range(0, 50)) = 2.5

     _Speed ("Speed", Range(-5, 5)) = 0.0
}
SubShader {
	Tags { "RenderType"="SelectiveGlow" "Queue"="Transparent" "ForceNoShadowCasting"="True"}
	LOD 150
    Cull Off
    ZWrite On

CGPROGRAM
#pragma surface surf Lambert noforwardadd alpha:fade
#pragma target 3.0

sampler2D _MainTex;
fixed4 _Color;
float _LineWidth, _LineAmount, _Speed;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = _Color * ceil(_LineWidth - frac((20*IN.uv_MainTex.y + (_Time.w * -_Speed)) * _LineAmount));
	o.Albedo = c.rgb;
    
    o.Alpha = saturate(lerp(1, -.5, IN.uv_MainTex.y));
    o.Alpha = c.r > 0.5 ? o.Alpha : 0;
    //o.Emission = c.rgb;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
