Shader "Tecaverso/Biology/MothPigment"
{
    Properties
    {
        _BaseColor("Pigmentação das asas", Color) = (0.82,0.70,0.48,1)
        _MarkColor("Faixas das asas", Color) = (0.46,0.36,0.24,1)
        _BodyColor("Corpo e antenas", Color) = (0.26,0.20,0.14,1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor, _MarkColor, _BodyColor;
            CBUFFER_END
            struct AppData { float3 position : POSITION; half3 normal : NORMAL; float2 uv : TEXCOORD0; half4 part : COLOR; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Varyings { float4 position : SV_POSITION; float2 uv : TEXCOORD0; half3 normal : TEXCOORD1; half body : TEXCOORD2; UNITY_VERTEX_OUTPUT_STEREO };
            Varyings Vert(AppData v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.position = TransformObjectToHClip(v.position);
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.body = v.part.r;
                o.uv = v.uv;
                return o;
            }
            half4 Frag(Varyings i, bool front : SV_IsFrontFace) : SV_Target
            {
                float2 p = float2(abs(i.uv.x), i.uv.y);
                float across = p.x + .19 * p.y + .065 * sin(12 * p.y + p.x) + .03 * sin(25 * p.y);
                float distanceToBand = min(abs(across - .53), abs(across - .80));
                float band = 1 - smoothstep(.023, .023 + max(.015, fwidth(across)), distanceToBand);
                float spot = 1 - smoothstep(.038, .065, length((p - float2(.40, .18)) * float2(1, 1.3)));
                half3 pigment = lerp(_BaseColor.rgb, _MarkColor.rgb, saturate(band + spot));
                pigment = lerp(pigment, _BodyColor.rgb, i.body);
                half3 normal = normalize(front ? i.normal : -i.normal);
                Light sun = GetMainLight();
                half directional = saturate(dot(normal, sun.direction));
                return half4(pigment * (.72h + .30h * directional) * lerp(half3(1,1,1), sun.color, .12h), 1);
            }
            ENDHLSL
        }
    }
}
