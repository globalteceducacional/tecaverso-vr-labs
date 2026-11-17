Shader "Tecaverso/InteractionSilhouette"
{
    SubShader
    {
        Tags {"RenderPipeline"="UniversalPipeline"}
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "VisibleMask"
            Blend 1 One One
            BlendOp 1 Max
            HLSLPROGRAM
            #pragma vertex MaskVertex
            #pragma target 3.5
            #pragma fragment MaskFragment
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            float4 _InteractionTint;
            struct Attributes {float4 positionOS:POSITION;UNITY_VERTEX_INPUT_INSTANCE_ID};
            struct Varyings {float4 positionCS:SV_POSITION;UNITY_VERTEX_OUTPUT_STEREO};
            Varyings MaskVertex(Attributes input)
            {
                Varyings output;UNITY_SETUP_INSTANCE_ID(input);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS=TransformObjectToHClip(input.positionOS.xyz);return output;
            }
            struct MaskOutput {half4 shape:SV_Target0;half visibility:SV_Target1;};
            MaskOutput MaskFragment(Varyings input)
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float depth=SampleSceneDepth(GetNormalizedScreenSpaceUV(input.positionCS));
                #if UNITY_REVERSED_Z
                    half visible=step(depth-.00001,input.positionCS.z);
                #else
                    half visible=step(input.positionCS.z,depth+.00001);
                #endif
                MaskOutput output;output.shape=_InteractionTint;output.visibility=visible;return output;
            }
            ENDHLSL
        }
        Pass
        {
            Name "SilhouetteEdge"
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma target 3.5
            #pragma fragment EdgeFragment
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            TEXTURE2D_X(_InteractionVisibilityTexture);
            half4 EdgeFragment(Varyings input):SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                half center=SAMPLE_TEXTURE2D_X_LOD(_BlitTexture,sampler_LinearClamp,input.texcoord,0).a;
                half4 edge=0;
                [unroll]for(int y=-1;y<=1;y++)[unroll]for(int x=-1;x<=1;x++)
                {
                    float2 uv=input.texcoord+float2(x,y)*_BlitTexture_TexelSize.xy*1.6;
                    half4 sampleColor=SAMPLE_TEXTURE2D_X_LOD(_BlitTexture,sampler_LinearClamp,uv,0);
                    sampleColor.a*=SAMPLE_TEXTURE2D_X_LOD(_InteractionVisibilityTexture,sampler_LinearClamp,uv,0).r;
                    if(sampleColor.a>edge.a)edge=sampleColor;
                }
                edge.a*=1-saturate(center*64);return edge;
            }
            ENDHLSL
        }
    }
}
