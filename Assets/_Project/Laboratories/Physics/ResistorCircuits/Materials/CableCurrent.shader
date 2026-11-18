Shader "Tecaverso/Física/Corrente convencional"
{
    Properties
    {
        _BaseColor("Cor", Color) = (0.3,0.8,1,1)
        _Phase("Fase", Float) = 0
        _Direction("Sentido", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Cull Off
            ZWrite Off
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float _Phase, _Direction;
            CBUFFER_END
            struct Input { float4 positionOS : POSITION; float2 uv : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Varying { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; UNITY_VERTEX_OUTPUT_STEREO };
            Varying Vert(Input v)
            {
                Varying o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }
            half4 Frag(Varying i) : SV_Target
            {
                float x = i.uv.x * _Direction * 2.8 - _Phase;
                float across = abs(i.uv.y * 2 - 1);
                float edge = abs(frac(x + across * .10) - .5) - .09;
                float aa = max(fwidth(x), .009);
                float ink = 1 - smoothstep(-aa, aa, edge);
                return half4(_BaseColor.rgb, .10 + .9 * ink);
            }
            ENDHLSL
        }
    }
}
