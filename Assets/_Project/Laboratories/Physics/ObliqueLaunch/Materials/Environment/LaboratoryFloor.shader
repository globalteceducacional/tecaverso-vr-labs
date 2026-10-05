Shader "Tecaverso/Laboratory Floor"
{
    Properties
    {
        _BaseColor("Floor", Color) = (0.015,0.03,0.085,1)
        _GridColor("Grid", Color) = (0.04,0.25,0.65,1)
        _CellSize("Cell size (m)", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor, _GridColor;
            float _CellSize;
            CBUFFER_END
            struct Attributes { float4 positionOS:POSITION; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Varyings { float4 positionCS:SV_POSITION; float3 positionWS:TEXCOORD0; UNITY_VERTEX_OUTPUT_STEREO };
            Varyings Vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionWS=TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS=TransformWorldToHClip(output.positionWS);
                return output;
            }
            half4 Frag(Varyings input):SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv=input.positionWS.xz/max(_CellSize,.01);
                float2 derivative=max(fwidth(uv),.00001);
                float2 distanceToLine=abs(frac(uv-.5)-.5);
                float2 coverage=1-smoothstep(.003,.003+derivative,distanceToLine);
                float fade=1-smoothstep(35,150,distance(input.positionWS,GetCameraPositionWS()));
                half gridCoverage=max(coverage.x,coverage.y)*fade;
                half wash=.85+.15*saturate((input.positionWS.z+14)/28);
                return half4(lerp(_BaseColor.rgb*wash,_GridColor.rgb,gridCoverage),1);
            }
            ENDHLSL
        }
    }
}
