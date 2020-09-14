Shader "Unlit/FlatSkyImageShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct appdata{
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f{
        float4 position : SV_POSITION;
        float4 screenPosition : TEXCOORD0;
    };

    sampler2D _MainTex;
    float4 _MainTex_ST;

    v2f vert(appdata v){
        v2f o;
                //convert the vertex positions from object space to clip space so they can be rendered
        o.position = UnityObjectToClipPos(v.vertex);
        o.screenPosition = ComputeScreenPos(o.position);
        return o;
    }

    fixed4 frag(v2f i) : SV_TARGET{
        float2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;
        fixed4 col = tex2D(_MainTex, textureCoordinate);
        return col;
    }

    ENDCG


    SubShader{
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
}