Shader "Custom/WaterRipple"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionMap ("Emission Map", 2D) = "white" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0)
        _RippleDistance("Ripple Dist", float) = 1
        _RippleWidth("RippleWidth", float) = 0.5

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex, _EmissionMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionMap;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color, _EmissionColor;

        float _RippleDistance, _RippleWidth;
        uniform float4 _MulPlayerPos[30];


        uniform float2 _rings[30];
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        //UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        //UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            fixed4 e = tex2D(_EmissionMap, IN.uv_EmissionMap) * _EmissionColor;


            //float dist = length(IN.worldPos - _MulPlayerPos[0]) - ((-4*_MulPlayerPos[0].w) + 5);

            //float dist = length(IN.worldPos - _MulPlayerPos[0]) - (sin(_Time.w/2)/1.5 + 4);
            //float halfWidth = 0.15 + sin(_Time.y)/10;//(_SinTime.w/2) + 0.6;
            //float lowerDistance = dist - halfWidth;
            //float upperDistance = dist + halfWidth;



            for(int i=0; i < 30; i++){
                float dist = length(IN.worldPos - _MulPlayerPos[i].xyz) - (_MulPlayerPos[i].w + 2);
                ///>02 > 9\\ .2 > 0
                float halfWidth = _RippleWidth/(_MulPlayerPos[i].w+2) - .1;// - _MulPlayerPos[i].w;
                _rings[i].x = dist - halfWidth;
                _rings[i].y = dist + halfWidth;
            }



            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            o.Albedo = c.rgb;


            float ringDraw = 0;
            // = (lowerDistance < 0 && upperDistance > 0);
            for(int i=0; i < 30; i++){
                ringDraw += (_rings[i].x < 0 && _rings[i].y > 0);
            }

            o.Emission = e.rgb + ringDraw;
            //o.Emission = e.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
