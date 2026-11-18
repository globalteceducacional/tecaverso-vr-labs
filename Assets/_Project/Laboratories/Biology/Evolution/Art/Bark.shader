Shader "Tecaverso/Biology/Bark"
{
    Properties
    {
        _BaseColor("Casca", Color) = (0.67,0.55,0.41,1)
        _GrainColor("Sulcos", Color) = (0.47,0.38,0.29,1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor, _GrainColor;
            CBUFFER_END
            struct AppData { float3 position : POSITION; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Varyings { float4 position : SV_POSITION; float2 p : TEXCOORD0; UNITY_VERTEX_OUTPUT_STEREO };
            Varyings Vert(AppData v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.position = TransformObjectToHClip(v.position);
                o.p = v.position.xz + .5;
                return o;
            }
            float Hash(float2 p) { return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453); }
            float Noise(float2 p)
            {
                float2 cell = floor(p), f = frac(p);
                f = f * f * (3 - 2 * f);
                return lerp(lerp(Hash(cell), Hash(cell + float2(1,0)), f.x),
                    lerp(Hash(cell + float2(0,1)), Hash(cell + 1), f.x), f.y);
            }
            half4 Frag(Varyings i) : SV_Target
            {
                float bend = Noise(i.p * float2(5, 4)) + .24 * Noise(i.p * float2(14, 9));
                float grain = Noise(float2(i.p.x * 48 + 5.5 * bend, i.p.y * 7 + 1.3 * bend));
                float patches = Noise(i.p * float2(15, 10) + bend);
                float2 fineUV = i.p * float2(114, 33) + float2(4 * bend, grain);
                float detail = Noise(fineUV);
                float detailVisibility = 1 - saturate(max(fwidth(fineUV.x), fwidth(fineUV.y)) - .3);
                detail = lerp(.5, detail, detailVisibility);
                float crack = 1 - smoothstep(.026, .08 + fwidth(grain), abs(grain - .48));
                float weathering = .16 * grain + .25 * patches + .17 * detail +
                                  .38 * crack * smoothstep(.25, .68, detail);
                return half4(lerp(_BaseColor.rgb, _GrainColor.rgb, weathering), 1);
            }
            ENDHLSL
        }
    }
}
