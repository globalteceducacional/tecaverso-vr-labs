Shader "Tecaverso/Biology/FieldSpecimens"
{
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
            struct Input { float3 positionOS : POSITION; half3 normalOS : NORMAL; half4 color : COLOR; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct Output { float4 positionCS : SV_POSITION; half3 normalWS : TEXCOORD0; half4 color : COLOR; UNITY_VERTEX_OUTPUT_STEREO };
            Output Vert(Input i)
            {
                Output o = (Output)0;
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionCS=TransformObjectToHClip(i.positionOS);
                o.normalWS=TransformObjectToWorldNormal(i.normalOS);
                o.color=i.color;
                return o;
            }
            half4 Frag(Output i, FRONT_FACE_TYPE face : FRONT_FACE_SEMANTIC) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                Light light = GetMainLight();
                half3 n=normalize(i.normalWS)*IS_FRONT_VFACE(face,1,-1);
                half diffuse=saturate(dot(n,light.direction)*.75+.25);
                return half4(i.color.rgb*(.52+.60*diffuse*light.color),1);
            }
            ENDHLSL
        }
    }
}
