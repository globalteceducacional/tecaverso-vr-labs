Shader "Tecaverso/Biology/AnatomicalFlow"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent-10" "RenderType"="Transparent" }
        Pass
        {
            Name "FlowOnSurface"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back ZWrite Off ZTest LEqual
            Offset -1, -1

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // These planes project each source mesh into its anatomical frame.
            // The CPU advances the clock, so the same wave freezes in both eyes.
            float4 _Travel, _Lateral, _Vertical;
            float4 _Wave;    // phase within this organ, intensity, half-width, pattern
            half4 _Tint, _Arrival;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 field : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            void ColonSegment(float2 sampleUV, float2 a, float2 b, float start, float end,
                              inout float bestDistance, inout float progress)
            {
                float2 segment = b - a;
                float t = saturate(dot(sampleUV - a, segment) / dot(segment, segment));
                float2 delta = sampleUV - lerp(a, b, t);
                float distanceSquared = dot(delta, delta);
                if (distanceSquared < bestDistance)
                {
                    bestDistance = distanceSquared;
                    progress = lerp(start, end, t);
                }
            }

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                float x = dot(_Lateral, input.positionOS);
                float y = dot(_Vertical, input.positionOS);
                float progress = dot(_Travel, input.positionOS);
                if (_Wave.w == 3) progress = saturate((.22 - y) / .22); // wrist to fingertips

                // The colon is one curved mesh. Sequence the ascending, transverse,
                // descending and terminal regions on that surface.
                if (_Wave.w == 4)
                {
                    float d = 8;
                    ColonSegment(float2(x,y), float2(.14,.34), float2(.12,.86), 0,.24,d,progress);
                    ColonSegment(float2(x,y), float2(.12,.86), float2(.88,.87), .24,.48,d,progress);
                    ColonSegment(float2(x,y), float2(.88,.87), float2(.83,.33), .48,.70,d,progress);
                    ColonSegment(float2(x,y), float2(.83,.33), float2(.52,.23), .70,.86,d,progress);
                    ColonSegment(float2(x,y), float2(.52,.23), float2(.5,.02), .86,1,d,progress);
                }
                output.field = float3(progress, x, y);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float distanceBehind = _Wave.x - input.field.x;
                half front = smoothstep(-_Wave.z * .44, 0, distanceBehind);
                half trail = 1 - smoothstep(0, _Wave.z, distanceBehind);
                half band = front * trail;
                half region = 1;
                if (_Wave.w == 1) region = 1 - smoothstep(.42,.62,input.field.y); // right heart
                if (_Wave.w == 2) region = smoothstep(.38,.58,input.field.y);     // left heart
                if (_Wave.w == 3) region = 1 - smoothstep(.13,.24,input.field.z); // hand
                half lighting = .77 + .23 * saturate(dot(normalize(input.normalWS),
                                                        half3(-.36,.68,-.64)) * .5 + .5);
                // Gas exchange changes the region the wave has already reached.
                // The two sides keep their meaning when the student pauses.
                half3 regionColor = lerp(_Tint.rgb, _Arrival.rgb, smoothstep(-.055,.07,distanceBehind));
                half3 color = lerp(regionColor, half3(1,.98,.89), band * .22) * lighting;
                half alpha = lerp(_Tint.a, _Arrival.a, band) * region * _Wave.y;
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
}
