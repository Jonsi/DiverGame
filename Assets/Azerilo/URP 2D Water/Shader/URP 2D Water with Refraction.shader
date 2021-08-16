Shader "Azerilo/URP 2D Water with Refraction"
{
    Properties
    {
        Color_234C1B0B("Water Top Color", Color) = (1, 1, 1, 0)
        Vector1_8DA20226("Water Top Width", Float) = 5
        Color_D12BF231("Water Color", Color) = (0.8156863, 0.9568627, 1, 0)
        Vector1_56BE0FD1("Water Level", Range(0, 1)) = 0.8
        Vector1_745B9376("Wave Speed", Float) = 3
        Vector1_5CB84537("Wave Frequency", Float) = 18
        Vector1_AF318A03("Wave Depth", Range(0, 20)) = 1.4
        Vector1_63384718("Refraction Speed", Range(0, 20)) = 10
        Vector1_73BD9798("Refraction Noise", Float) = 70
        Vector1_D775EAAC("Refraction Strength", Range(-2, 2)) = 0.7
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent+0"
        }
        
        Pass
        {
            Name "Pass"
            Tags 
            { 
                // LightMode: <None>
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Back
            ZTest LEqual
            ZWrite Off
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_POSITION_WS 
            #define VARYINGS_NEED_TEXCOORD0
            #define SHADERPASS_UNLIT
            #define REQUIRE_DEPTH_TEXTURE
            #define REQUIRE_OPAQUE_TEXTURE
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Color_234C1B0B;
            float Vector1_8DA20226;
            float4 Color_D12BF231;
            float Vector1_56BE0FD1;
            float Vector1_745B9376;
            float Vector1_5CB84537;
            float Vector1_AF318A03;
            float Vector1_63384718;
            float Vector1_73BD9798;
            float Vector1_D775EAAC;
            CBUFFER_END
            float4 Color_20C936C9;
        
            // Graph Functions
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
            }
            
            inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
            {
                return (1.0-t)*a + (t*b);
            }
            
            
            inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
            
                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                float r3 = Unity_SimpleNoise_RandomValue_float(c3);
            
                float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                return t;
            }
            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
            {
                float t = 0.0;
            
                float freq = pow(2.0, float(0));
                float amp = pow(0.5, float(3-0));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3-1));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3-2));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                Out = t;
            }
            
            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }
            
            void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
            
            void Unity_Comparison_Less_float(float A, float B, out float Out)
            {
                Out = A < B ? 1 : 0;
            }
            
            void Unity_Branch_float2(float Predicate, float2 True, float2 False, out float2 Out)
            {
                Out = Predicate ? True : False;
            }
            
            void Unity_SceneColor_float(float4 UV, out float3 Out)
            {
                Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Sine_float(float In, out float Out)
            {
                Out = sin(In);
            }
            
            void Unity_Comparison_Greater_float(float A, float B, out float Out)
            {
                Out = A > B ? 1 : 0;
            }
            
            void Unity_Branch_float(float Predicate, float True, float False, out float Out)
            {
                Out = Predicate ? True : False;
            }
            
            void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
            {
                Out = lerp(A, B, T);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
        
            // Graph Vertex
            // GraphVertex: <None>
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpacePosition;
                float4 ScreenPosition;
                float4 uv0;
                float3 TimeParameters;
            };
            
            struct SurfaceDescription
            {
                float3 Color;
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _ScreenPosition_58243190_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                float _Property_19E417E7_Out_0 = Vector1_63384718;
                float _Divide_DEBF01FB_Out_2;
                Unity_Divide_float(_Property_19E417E7_Out_0, 100, _Divide_DEBF01FB_Out_2);
                float _Multiply_E04EB79A_Out_2;
                Unity_Multiply_float(_Divide_DEBF01FB_Out_2, IN.TimeParameters.x, _Multiply_E04EB79A_Out_2);
                float2 _TilingAndOffset_F53F68E6_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), (_Multiply_E04EB79A_Out_2.xx), _TilingAndOffset_F53F68E6_Out_3);
                float _Property_2F6C5875_Out_0 = Vector1_73BD9798;
                float _SimpleNoise_E6D55630_Out_2;
                Unity_SimpleNoise_float(_TilingAndOffset_F53F68E6_Out_3, _Property_2F6C5875_Out_0, _SimpleNoise_E6D55630_Out_2);
                float4 _Property_972A11FD_Out_0 = Color_20C936C9;
                float _Property_DF0D10BB_Out_0 = Vector1_D775EAAC;
                float _Divide_8C785937_Out_2;
                Unity_Divide_float(_Property_DF0D10BB_Out_0, 100, _Divide_8C785937_Out_2);
                float4 _Add_F8F86DF3_Out_2;
                Unity_Add_float4(_Property_972A11FD_Out_0, (_Divide_8C785937_Out_2.xxxx), _Add_F8F86DF3_Out_2);
                float4 _Multiply_CAA37872_Out_2;
                Unity_Multiply_float((_SimpleNoise_E6D55630_Out_2.xxxx), _Add_F8F86DF3_Out_2, _Multiply_CAA37872_Out_2);
                float2 _TilingAndOffset_5394B664_Out_3;
                Unity_TilingAndOffset_float((_ScreenPosition_58243190_Out_0.xy), float2 (1, 1), (_Multiply_CAA37872_Out_2.xy), _TilingAndOffset_5394B664_Out_3);
                float _SceneDepth_EF697AC0_Out_1;
                Unity_SceneDepth_Eye_float((float4(_TilingAndOffset_5394B664_Out_3, 0.0, 1.0)), _SceneDepth_EF697AC0_Out_1);
                float4 _ScreenPosition_7BEBFEBC_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
                float _Split_CA486DC2_R_1 = _ScreenPosition_7BEBFEBC_Out_0[0];
                float _Split_CA486DC2_G_2 = _ScreenPosition_7BEBFEBC_Out_0[1];
                float _Split_CA486DC2_B_3 = _ScreenPosition_7BEBFEBC_Out_0[2];
                float _Split_CA486DC2_A_4 = _ScreenPosition_7BEBFEBC_Out_0[3];
                float _Subtract_4A4D0AC4_Out_2;
                Unity_Subtract_float(_SceneDepth_EF697AC0_Out_1, _Split_CA486DC2_A_4, _Subtract_4A4D0AC4_Out_2);
                float _Comparison_CA371A8D_Out_2;
                Unity_Comparison_Less_float(_Subtract_4A4D0AC4_Out_2, 0, _Comparison_CA371A8D_Out_2);
                float2 _Branch_B5B761EE_Out_3;
                Unity_Branch_float2(_Comparison_CA371A8D_Out_2, (_ScreenPosition_58243190_Out_0.xy), _TilingAndOffset_5394B664_Out_3, _Branch_B5B761EE_Out_3);
                float3 _SceneColor_FE3F5E6F_Out_1;
                Unity_SceneColor_float((float4(_Branch_B5B761EE_Out_3, 0.0, 1.0)), _SceneColor_FE3F5E6F_Out_1);
                float4 _Property_FE76EC02_Out_0 = Color_234C1B0B;
                float4 _UV_F41150A7_Out_0 = IN.uv0;
                float4 _Add_5D6BB924_Out_2;
                Unity_Add_float4(_Property_FE76EC02_Out_0, _UV_F41150A7_Out_0, _Add_5D6BB924_Out_2);
                float4 _Property_395588DF_Out_0 = Color_D12BF231;
                float4 _UV_B12C1A65_Out_0 = IN.uv0;
                float _Split_3D7BB18F_R_1 = _UV_B12C1A65_Out_0[0];
                float _Split_3D7BB18F_G_2 = _UV_B12C1A65_Out_0[1];
                float _Split_3D7BB18F_B_3 = _UV_B12C1A65_Out_0[2];
                float _Split_3D7BB18F_A_4 = _UV_B12C1A65_Out_0[3];
                float _Property_8E0D6409_Out_0 = Vector1_5CB84537;
                float _Multiply_2DD36437_Out_2;
                Unity_Multiply_float(_Split_3D7BB18F_R_1, _Property_8E0D6409_Out_0, _Multiply_2DD36437_Out_2);
                float _Property_785B75BD_Out_0 = Vector1_745B9376;
                float _Multiply_BFFB3336_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_785B75BD_Out_0, _Multiply_BFFB3336_Out_2);
                float _Add_4B65A87C_Out_2;
                Unity_Add_float(_Multiply_2DD36437_Out_2, _Multiply_BFFB3336_Out_2, _Add_4B65A87C_Out_2);
                float _Sine_19740ADC_Out_1;
                Unity_Sine_float(_Add_4B65A87C_Out_2, _Sine_19740ADC_Out_1);
                float _Property_52F1CEB_Out_0 = Vector1_AF318A03;
                float _Divide_36E0C2B0_Out_2;
                Unity_Divide_float(_Property_52F1CEB_Out_0, 100, _Divide_36E0C2B0_Out_2);
                float _Multiply_A5F7F243_Out_2;
                Unity_Multiply_float(_Sine_19740ADC_Out_1, _Divide_36E0C2B0_Out_2, _Multiply_A5F7F243_Out_2);
                float _Property_17F92798_Out_0 = Vector1_56BE0FD1;
                float _Property_709E000B_Out_0 = Vector1_8DA20226;
                float _Divide_BBDF3FF8_Out_2;
                Unity_Divide_float(_Property_709E000B_Out_0, 1000, _Divide_BBDF3FF8_Out_2);
                float _Subtract_CCC903B6_Out_2;
                Unity_Subtract_float(_Property_17F92798_Out_0, _Divide_BBDF3FF8_Out_2, _Subtract_CCC903B6_Out_2);
                float _Add_B81AF8CF_Out_2;
                Unity_Add_float(_Multiply_A5F7F243_Out_2, _Subtract_CCC903B6_Out_2, _Add_B81AF8CF_Out_2);
                float _Comparison_C9700DB_Out_2;
                Unity_Comparison_Greater_float(_Split_3D7BB18F_G_2, _Add_B81AF8CF_Out_2, _Comparison_C9700DB_Out_2);
                float _Branch_8F05124C_Out_3;
                Unity_Branch_float(_Comparison_C9700DB_Out_2, 0, 1, _Branch_8F05124C_Out_3);
                float4 _Lerp_C1696AD9_Out_3;
                Unity_Lerp_float4(_Add_5D6BB924_Out_2, _Property_395588DF_Out_0, (_Branch_8F05124C_Out_3.xxxx), _Lerp_C1696AD9_Out_3);
                float3 _Multiply_338E8473_Out_2;
                Unity_Multiply_float(_SceneColor_FE3F5E6F_Out_1, (_Lerp_C1696AD9_Out_3.xyz), _Multiply_338E8473_Out_2);
                float _Add_911FAD97_Out_2;
                Unity_Add_float(_Multiply_A5F7F243_Out_2, _Property_17F92798_Out_0, _Add_911FAD97_Out_2);
                float _Comparison_6D4427F8_Out_2;
                Unity_Comparison_Greater_float(_Split_3D7BB18F_G_2, _Add_911FAD97_Out_2, _Comparison_6D4427F8_Out_2);
                float _Branch_16000569_Out_3;
                Unity_Branch_float(_Comparison_6D4427F8_Out_2, 0, 1, _Branch_16000569_Out_3);
                surface.Color = _Multiply_338E8473_Out_2;
                surface.Alpha = _Branch_16000569_Out_3;
                surface.AlphaClipThreshold = 0;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float4 texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float3 interp00 : TEXCOORD0;
                float4 interp01 : TEXCOORD1;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                output.interp01.xyzw = input.texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                output.texCoord0 = input.interp01.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.WorldSpacePosition =          input.positionWS;
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                output.uv0 =                         input.texCoord0;
                output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags 
            { 
                "LightMode" = "ShadowCaster"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Back
            ZTest LEqual
            ZWrite On
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define SHADERPASS_SHADOWCASTER
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Color_234C1B0B;
            float Vector1_8DA20226;
            float4 Color_D12BF231;
            float Vector1_56BE0FD1;
            float Vector1_745B9376;
            float Vector1_5CB84537;
            float Vector1_AF318A03;
            float Vector1_63384718;
            float Vector1_73BD9798;
            float Vector1_D775EAAC;
            CBUFFER_END
            float4 Color_20C936C9;
        
            // Graph Functions
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Sine_float(float In, out float Out)
            {
                Out = sin(In);
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Comparison_Greater_float(float A, float B, out float Out)
            {
                Out = A > B ? 1 : 0;
            }
            
            void Unity_Branch_float(float Predicate, float True, float False, out float Out)
            {
                Out = Predicate ? True : False;
            }
        
            // Graph Vertex
            // GraphVertex: <None>
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float4 uv0;
                float3 TimeParameters;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _UV_B12C1A65_Out_0 = IN.uv0;
                float _Split_3D7BB18F_R_1 = _UV_B12C1A65_Out_0[0];
                float _Split_3D7BB18F_G_2 = _UV_B12C1A65_Out_0[1];
                float _Split_3D7BB18F_B_3 = _UV_B12C1A65_Out_0[2];
                float _Split_3D7BB18F_A_4 = _UV_B12C1A65_Out_0[3];
                float _Property_8E0D6409_Out_0 = Vector1_5CB84537;
                float _Multiply_2DD36437_Out_2;
                Unity_Multiply_float(_Split_3D7BB18F_R_1, _Property_8E0D6409_Out_0, _Multiply_2DD36437_Out_2);
                float _Property_785B75BD_Out_0 = Vector1_745B9376;
                float _Multiply_BFFB3336_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_785B75BD_Out_0, _Multiply_BFFB3336_Out_2);
                float _Add_4B65A87C_Out_2;
                Unity_Add_float(_Multiply_2DD36437_Out_2, _Multiply_BFFB3336_Out_2, _Add_4B65A87C_Out_2);
                float _Sine_19740ADC_Out_1;
                Unity_Sine_float(_Add_4B65A87C_Out_2, _Sine_19740ADC_Out_1);
                float _Property_52F1CEB_Out_0 = Vector1_AF318A03;
                float _Divide_36E0C2B0_Out_2;
                Unity_Divide_float(_Property_52F1CEB_Out_0, 100, _Divide_36E0C2B0_Out_2);
                float _Multiply_A5F7F243_Out_2;
                Unity_Multiply_float(_Sine_19740ADC_Out_1, _Divide_36E0C2B0_Out_2, _Multiply_A5F7F243_Out_2);
                float _Property_17F92798_Out_0 = Vector1_56BE0FD1;
                float _Add_911FAD97_Out_2;
                Unity_Add_float(_Multiply_A5F7F243_Out_2, _Property_17F92798_Out_0, _Add_911FAD97_Out_2);
                float _Comparison_6D4427F8_Out_2;
                Unity_Comparison_Greater_float(_Split_3D7BB18F_G_2, _Add_911FAD97_Out_2, _Comparison_6D4427F8_Out_2);
                float _Branch_16000569_Out_3;
                Unity_Branch_float(_Comparison_6D4427F8_Out_2, 0, 1, _Branch_16000569_Out_3);
                surface.Alpha = _Branch_16000569_Out_3;
                surface.AlphaClipThreshold = 0;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float4 interp00 : TEXCOORD0;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyzw = input.texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.interp00.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.uv0 =                         input.texCoord0;
                output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags 
            { 
                "LightMode" = "DepthOnly"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define SHADERPASS_DEPTHONLY
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Color_234C1B0B;
            float Vector1_8DA20226;
            float4 Color_D12BF231;
            float Vector1_56BE0FD1;
            float Vector1_745B9376;
            float Vector1_5CB84537;
            float Vector1_AF318A03;
            float Vector1_63384718;
            float Vector1_73BD9798;
            float Vector1_D775EAAC;
            CBUFFER_END
            float4 Color_20C936C9;
        
            // Graph Functions
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Sine_float(float In, out float Out)
            {
                Out = sin(In);
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Comparison_Greater_float(float A, float B, out float Out)
            {
                Out = A > B ? 1 : 0;
            }
            
            void Unity_Branch_float(float Predicate, float True, float False, out float Out)
            {
                Out = Predicate ? True : False;
            }
        
            // Graph Vertex
            // GraphVertex: <None>
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float4 uv0;
                float3 TimeParameters;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _UV_B12C1A65_Out_0 = IN.uv0;
                float _Split_3D7BB18F_R_1 = _UV_B12C1A65_Out_0[0];
                float _Split_3D7BB18F_G_2 = _UV_B12C1A65_Out_0[1];
                float _Split_3D7BB18F_B_3 = _UV_B12C1A65_Out_0[2];
                float _Split_3D7BB18F_A_4 = _UV_B12C1A65_Out_0[3];
                float _Property_8E0D6409_Out_0 = Vector1_5CB84537;
                float _Multiply_2DD36437_Out_2;
                Unity_Multiply_float(_Split_3D7BB18F_R_1, _Property_8E0D6409_Out_0, _Multiply_2DD36437_Out_2);
                float _Property_785B75BD_Out_0 = Vector1_745B9376;
                float _Multiply_BFFB3336_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_785B75BD_Out_0, _Multiply_BFFB3336_Out_2);
                float _Add_4B65A87C_Out_2;
                Unity_Add_float(_Multiply_2DD36437_Out_2, _Multiply_BFFB3336_Out_2, _Add_4B65A87C_Out_2);
                float _Sine_19740ADC_Out_1;
                Unity_Sine_float(_Add_4B65A87C_Out_2, _Sine_19740ADC_Out_1);
                float _Property_52F1CEB_Out_0 = Vector1_AF318A03;
                float _Divide_36E0C2B0_Out_2;
                Unity_Divide_float(_Property_52F1CEB_Out_0, 100, _Divide_36E0C2B0_Out_2);
                float _Multiply_A5F7F243_Out_2;
                Unity_Multiply_float(_Sine_19740ADC_Out_1, _Divide_36E0C2B0_Out_2, _Multiply_A5F7F243_Out_2);
                float _Property_17F92798_Out_0 = Vector1_56BE0FD1;
                float _Add_911FAD97_Out_2;
                Unity_Add_float(_Multiply_A5F7F243_Out_2, _Property_17F92798_Out_0, _Add_911FAD97_Out_2);
                float _Comparison_6D4427F8_Out_2;
                Unity_Comparison_Greater_float(_Split_3D7BB18F_G_2, _Add_911FAD97_Out_2, _Comparison_6D4427F8_Out_2);
                float _Branch_16000569_Out_3;
                Unity_Branch_float(_Comparison_6D4427F8_Out_2, 0, 1, _Branch_16000569_Out_3);
                surface.Alpha = _Branch_16000569_Out_3;
                surface.AlphaClipThreshold = 0;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float4 interp00 : TEXCOORD0;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyzw = input.texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.interp00.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.uv0 =                         input.texCoord0;
                output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
            ENDHLSL
        }
        
    }
}
