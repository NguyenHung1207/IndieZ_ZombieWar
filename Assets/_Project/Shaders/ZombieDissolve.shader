Shader "IndieZ/ZombieDissolve"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _EdgeWidth ("Edge Width", Range(0.001,0.3)) = 0.08
        _EdgeColor ("Edge Color", Color) = (1,0.25,0.02,1)
        _NoiseScale ("Noise Scale", Float) = 3
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _DissolveAmount;
                float _EdgeWidth;
                float4 _EdgeColor;
                float _NoiseScale;
            CBUFFER_END
            struct Attributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; float2 uv:TEXCOORD0; };
            struct Varyings { float4 positionHCS:SV_POSITION; float3 positionWS:TEXCOORD0; float3 normalWS:TEXCOORD1; float2 uv:TEXCOORD2; };
            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs position = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = position.positionCS;
                output.positionWS = position.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = input.uv;
                return output;
            }
            float hash21(float2 p) { p = frac(p * float2(123.34,456.21)); p += dot(p,p+45.32); return frac(p.x*p.y); }
            half4 frag(Varyings input):SV_Target
            {
                float3 cell = floor(input.positionWS * _NoiseScale);
                float noise = hash21(cell.xy + cell.z);
                clip(noise - _DissolveAmount);
                half3 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).rgb * _BaseColor.rgb;
                Light light = GetMainLight();
                half lighting = saturate(dot(normalize(input.normalWS), light.direction)) * 0.65h + 0.35h;
                float edge = 1.0 - smoothstep(0.0, _EdgeWidth, noise - _DissolveAmount);
                return half4(baseColor * lighting + _EdgeColor.rgb * edge, 1);
            }
            ENDHLSL
        }
    }
}
