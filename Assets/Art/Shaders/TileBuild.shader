Shader "Custom/TileBuild"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _NoiseTex ("Assembly Noise", 2D) = "white" {}
        _BuildProgress ("Build Progress", Range(0, 1)) = 0
        _AssembleDistance ("Assemble Fall Distance", Float) = 1.5
        _Scatter ("Vertical Scatter", Float) = 0.5
        _RightBias ("Right Entry Bias", Float) = 0.4
        _EdgeWidth ("Assembly Edge Width", Range(0.001, 0.5)) = 0.08
        _EdgeColor ("Assembly Edge Glow", Color) = (0, 1, 1, 1)
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
            float _BuildProgress;
            float _AssembleDistance;
            float _Scatter;
            float _RightBias;
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

                // Mirrors the crumble shader's per-cell stagger, so cells don't all
                // finish assembling in perfect lockstep, but every cell is guaranteed
                // fully settled by the time _BuildProgress reaches 1.
                float cellStart = seed * 0.35;
                float cellProgress = saturate((_BuildProgress - cellStart) / (1.0 - cellStart));

                // Ease-out: pieces move fast at first and decelerate into place,
                // reading as a snap-into-position settle rather than a free fall.
                float settleCurve = 1.0 - pow(1.0 - cellProgress, 3.0);
                float remaining = 1.0 - settleCurve;

                float dirY = hash(seed * 17.0 + 3.0) * 2.0 - 1.0;

                // _RightBias is a constant, non-random horizontal entry direction
                // (pieces fly in from the right). _Scatter now jitters the vertical
                // fall distance per cell instead, so pieces settle from staggered
                // heights rather than staggered left/right positions.
                float3 offset = float3(
                    _RightBias * remaining,
                    (_AssembleDistance + dirY * _Scatter) * remaining,
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

                // Inverse of the crumble dissolve: threshold starts at 1 (nothing
                // visible) and falls to 0 (fully visible) as _BuildProgress rises.
                float threshold = 1.0 - _BuildProgress;
                clip(noise - threshold);

                float edge = smoothstep(0.0, _EdgeWidth, noise - threshold);
                fixed4 col = lerp(_EdgeColor, tex, edge);
                col.a = tex.a;

                return col;
            }
            ENDCG
        }
    }
}