Shader "Custom/RockWithTopColor_Lit"
{
    Properties
    {
        _MainTex("Rock Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _TopColor("Top Color", Color) = (0.8,0.8,0.8,1)
        _SideBottomColor("Side Bottom Color", Color) = (0.3,0.3,0.3,1)
        _SideTopColor("Side Top Color", Color) = (0.6,0.6,0.6,1)
        _BlendHeight("Side Blend Height", Float) = 2.0
        _Smoothness("Smoothness", Range(0,1)) = 0.3
        _Metallic("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 300

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fragment _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            // URP includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float  fogFactor   : TEXCOORD3;
            };

            // Zmieniliœmy nazwê z _BaseMap na _MainTex
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            float4 _BaseColor;
            float4 _TopColor;
            float4 _SideBottomColor;
            float4 _SideTopColor;
            float _BlendHeight;
            float _Smoothness;
            float _Metallic;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.fogFactor = ComputeFogFactor(OUT.positionHCS.z);
                return OUT;
            }

half4 frag(Varyings IN) : SV_Target
{
    // sample texture using URP macro
    half4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
    half3 normalWS = normalize(IN.normalWS);

    // gradient: zale¿ny od wysokoœci (y)
    float t = saturate(IN.positionWS.y / _BlendHeight);
    float3 sideColor = lerp(_SideBottomColor.rgb, _SideTopColor.rgb, t);

    // maska góry (czy face jest skierowany ku górze)
    float topMask = saturate(dot(normalWS, float3(0,1,0)));
    float3 finalColor = lerp(sideColor, _TopColor.rgb, smoothstep(0.8, 1.0, topMask));

    // --- przygotowujemy InputData ---
    InputData lightingInput = (InputData)0;
    lightingInput.positionWS = IN.positionWS;
    lightingInput.normalWS = normalWS;
    lightingInput.viewDirectionWS = normalize(_WorldSpaceCameraPos - IN.positionWS);
    lightingInput.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
    lightingInput.fogCoord = IN.fogFactor;

    // --- wa¿ne: wyzeruj SurfaceData przed przypisaniami ---
    SurfaceData surfaceData = (SurfaceData)0;

    // wype³niamy pola surfaceData które u¿ywamy
    surfaceData.albedo     = finalColor * _BaseColor.rgb * baseTex.rgb;
    surfaceData.metallic   = _Metallic;
    surfaceData.smoothness = _Smoothness;
    surfaceData.occlusion  = 1.0;
    surfaceData.emission   = float3(0,0,0);
    surfaceData.alpha      = 1.0;

    // wywo³anie URP PBR (zwraca ju¿ oœwietlony kolor, obs³uguje shadery, cienie itd.)
    half4 color = UniversalFragmentPBR(lightingInput, surfaceData);
    color.rgb = MixFog(color.rgb, IN.fogFactor);
    return color;
}
            ENDHLSL
        }
    }
}
