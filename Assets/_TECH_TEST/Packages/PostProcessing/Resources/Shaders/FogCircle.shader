Shader "Custom/FogCircle"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (1,1,1,1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"RenderType"="Opaque"}
        LOD 200
        Cull Off


        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert
        #pragma target 4.0

        struct Input
        {
            float2 uv_NoiseTex;
            float3 worldPos;
        };
       
        sampler2D _NoiseTex;
        fixed4 _Color, _Color2;

        void vert (inout appdata_full v, out Input o) {
          UNITY_INITIALIZE_OUTPUT(Input,o);
          o.worldPos = mul (unity_ObjectToWorld, v.vertex);
        }
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_NoiseTex, IN.uv_NoiseTex/32 + (IN.worldPos.y));
            clip (c.r - (0.5 + sin(_Time.y/2 + IN.worldPos.y/50)/10));
            o.Albedo = c.r > (0.7 + sin(_Time.y/2 + IN.worldPos.y/50)/20) ? _Color : _Color2;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
