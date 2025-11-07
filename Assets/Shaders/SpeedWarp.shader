Shader "SpeedWarp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}    // the screen image
        _WarpX ("Horizontal Stretch", Range(1, 1.5)) = 1.0
        _WarpY ("Vertical Squash",    Range(0.5, 1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always   // always draw over the screen

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv  : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _WarpX;
            float _WarpY;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Center UVs around (0,0)
                float2 uv = i.uv - 0.5;

                // Apply horizontal and vertical scaling
                uv.x *= _WarpX;   // stretch sideways
                uv.y *= _WarpY;   // squeeze vertically

                // Shift back to normal 0–1 UV range
                uv += 0.5;

                // Sample the original screen image
                fixed4 col = tex2D(_MainTex, uv);

                return col;
            }
            ENDCG
        }
    }
}
