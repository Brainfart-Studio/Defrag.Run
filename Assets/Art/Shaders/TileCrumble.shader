Shader "Custom/TileCrumble"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _NoiseTex ("Dissolve Noise", 2D) = "white" {}
        _DecayProgress ("Decay Progress", Range(0, 1)) = 0
        _FallDistance ("Fall Distance", Float) = 1.5
        _Scatter ("Horizontal Scatter", Float) = 0.5
        _EdgeWidth ("Dissolve Edge Width", Range(0.001, 0.5)) = 0.08
        _EdgeColor ("Dissolve Edge Glow", Color) = (0, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float seed : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float _DecayProgress;
            float _FallDistance;
            float _Scatter;
            float _EdgeWidth;
            float4 _EdgeColor;

            float hash(float n)
            {
                return frac(sin(n * 12.9898) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;

                float seed = v.color.r;

                // Each cell starts falling at a slightly different point in the decay
                // window so the tile doesn't crumble as one rigid slab.
                float cellStart = seed * 0.35;
                float cellProgress = saturate((_DecayProgress - cellStart) / (1.0 - cellStart));
                float fallCurve = cellProgress * cellProgress;

                float dirX = hash(seed * 17.0 + 3.0) * 2.0 - 1.0;

                float3 offset = float3(
                    dirX * _Scatter * cellProgress,
                    -_FallDistance * fallCurve,
                    0.0
                );

                float4 displaced = v.vertex + float4(offset, 0.0);

                o.pos = UnityObjectToClipPos(displaced);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.seed = seed;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed noise = tex2D(_NoiseTex, i.uv + i.seed).r;

                clip(noise - _DecayProgress);

                float edge = smoothstep(0.0, _EdgeWidth, noise - _DecayProgress);
                fixed4 col = lerp(_EdgeColor, tex, edge);
                col.a = tex.a;

                return col;
            }
            ENDCG
        }
    }
}