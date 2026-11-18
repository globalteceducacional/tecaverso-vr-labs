Shader "Tecaverso/Biology/TrophicRibbon"
{
    Properties
    {
        _Tint ("Ink",Color)=(1,.72,.26,1)
        _Phase ("Travel in metres",Float)=0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
            half4 _Tint;
            float _Phase;
            CBUFFER_END
            struct Input { float3 positionOS : POSITION; float2 uv : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Output { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; UNITY_VERTEX_OUTPUT_STEREO };
            Output Vert(Input i)
            {
                Output o=(Output)0;
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos=TransformObjectToHClip(i.positionOS);
                o.uv=i.uv;
                return o;
            }
            half4 Frag(Output i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float crest=frac((i.uv.x - _Phase + abs(i.uv.y-.5)*.042)/.19);
                float highlight=smoothstep(.56,.78,crest)*(1-smoothstep(.78,.96,crest));
                return half4(lerp(_Tint.rgb*.74,_Tint.rgb*1.24+.10,highlight),1);
            }
            ENDHLSL
        }
    }
}
