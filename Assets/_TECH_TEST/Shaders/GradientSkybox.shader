// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Skybox/Gradient Skybox"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 1, 1, 0)
        _Color2 ("Color 2", Color) = (1, 1, 1, 0)
        _Color3 ("Color 3", Color) = (1, 1, 1, 0)
        _Color4 ("Color 4", Color) = (1, 1, 1, 0)
        _UpVector ("Up Vector", Vector) = (0, 1, 0, 0)
        _LeftVector ("Left Vector", Vector) = (0, 1, 0, 0)
        _ForwardVector ("Forward Vector", Vector) = (0, 1, 0, 0)
        _Intensity ("Intensity", Float) = 1.0
        _Exponent ("Exponent", Float) = 1.0

    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct appdata
    {
        float4 position : POSITION;
        float3 texcoord : TEXCOORD0;
    };
    
    struct v2f
    {
        float4 position : SV_POSITION;
        float3 texcoord : TEXCOORD0;
    };
    
    half4 _Color1;
    half4 _Color2;
    half4 _Color3;
    half4 _Color4;
    half4 _UpVector;
    half4 _LeftVector;
    half4 _ForwardVector;
    half _Intensity;
    half _Exponent;
    
    v2f vert (appdata v)
    {
        v2f o;
        o.position = UnityObjectToClipPos (v.position);
        o.texcoord = v.texcoord;
        return o;
    }
    
    fixed4 frag (v2f i) : COLOR
    {
        half d1 = dot (normalize (i.texcoord), _UpVector) * 0.5f + 0.5f;
        half d2 = dot (normalize (i.texcoord), _LeftVector) * 0.5f + 0.5f;
        half d3 = dot (normalize (i.texcoord), _ForwardVector) * 0.5f + 0.5f;
        float4 grad1 = lerp (_Color1, _Color2, pow (d1, _Exponent)) * _Intensity;
        float4 grad2 = lerp (_Color3, _Color4, pow (d2, _Exponent)) * _Intensity;
        float4 gradSum = lerp (grad1, grad2, pow (d3, _Exponent)) * _Intensity;
        return gradSum;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" }
        Pass
        {
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    CustomEditor "GradientSkyboxInspector"
}