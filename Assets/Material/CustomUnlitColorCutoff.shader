Shader "Custom/UnlitTransparentCutout"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "TransparentCutout" }
            LOD 100

            Pass
            {
                ZWrite On
                Blend SrcAlpha OneMinusSrcAlpha
                AlphaTest Greater[_Cutoff]

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _Color;
                float _Cutoff;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 texcol = tex2D(_MainTex, i.uv);
                    if (texcol.a < _Cutoff)
                        discard;
                    return texcol * _Color;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
