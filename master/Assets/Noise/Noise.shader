Shader "Unlit/Noise" {
    Properties {
        //Quality properties 
        [Header(Quality)]
        _StepSize ("Step Size", Range(0.001,0.01)) = 0.001
        _Scale ("Scale", Range(0,1)) = 0.584
        _Outline ("Outline Strength", Range(0,1)) = 0.088

        //Transfer function properties
        [Header(Transfer Function)]
        [IntRange] _UseSquaredTransfer ("Use Squared Transfer", Range(0,1)) = 1
        [IntRange] _TransferColorCount ("Number of Transfer Colors", Range(1,3)) = 2
        _TransferColor1 ("Transfer Color 1", Color) = (1,0,0,1)
        _TransferColor2 ("Transfer Color 2", Color) = (0,1,0,1)
        _TransferColor3 ("Transfer Color 3", Color) = (0,0,1,1)
        _TransferCutoff1 ("Transfer Cutoff 1", Range(0,1)) = 0.88
        _TransferCutoff2 ("Transfer Cutoff 2", Range(0,1)) = 0.588
        _TransferCutoff3 ("Transfer Cutoff 3", Range(0,1)) = 0.116
        _TransferScale1 ("Transfer Scale 1", Range(0.01,2)) = 0.612
        _TransferScale2 ("Transfer Scale 2", Range(0.01,2)) = 0.362
        _TransferScale3 ("Transfer Scale 3", Range(0.01,2)) = 1
        _TransparencyScale ("TransparencyScale", float) = 1

        [Header(Noise)]
        _NoiseOffset ("Noise Offset", vector) = (5,5,5)
        _NoiseScale ("Noise Scale", vector) = (1,1,1)
        _BooleanNoiseOffset ("Boolean Noise Offset", vector) = (0,0,0)
        _NoiseValueCutoff ("Noise Value Cutoff", Range(0,1)) = 0.8
        _NoiseLatitudeCutoff ("Noise Latitude Cutoff", Range(0,0.5)) = 0.25
        _NoiseSharpness ("Noise Sharpness", Range(0,10)) = 3
        _AuroraSharpness ("Aurora Sharpness", Range(0,10)) = 2
    }

    SubShader {
        Tags { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "UnityCG.cginc"

            //Shader input structs
            struct MeshData {
                float4 positionOS : POSITION;
            };

            struct Interpolators {
                float4 positionCS : SV_POSITION;
                float4 positionOS : TEXCOORD0;
                float3 cameraDirectionOS : TEXCOORD1;
            };

            //Constants
            #define Epsilon 0.000001
            #define PI 3.141592653

            //Shader variables
            float _StepSize;
            float _Height;
            float _Width;
            float _Scale;
            float _Outline;

            bool _UseSquaredTransfer;
            float _TransferColorCount; 
            float4 _TransferColor1;
            float4 _TransferColor2; 
            float4 _TransferColor3;
            float _TransferCutoff1;
            float _TransferCutoff2;
            float _TransferCutoff3;
            float _TransferScale1;
            float _TransferScale2;
            float _TransferScale3;
            float _TransparencyScale;

            float3 _NoiseOffset;
            float3 _NoiseScale;
            float3 _BooleanNoiseOffset;
            float _NoiseValueCutoff;
            float _NoiseLatitudeCutoff;
            float _NoiseSharpness;
            float _AuroraSharpness;

            //Utility functions
            float InverseLerp(float a, float b, float v) {return (v-a)/(b-a);}
            float Remap(float iMin, float iMax, float oMin, float oMax, float v) {float t = InverseLerp(iMin, iMax, v); return lerp(oMin, oMax, t);}

            //Vertex shader
            Interpolators Vertex(MeshData input) {
                Interpolators output;

                //Pass vertex position in object space and clip space
                output.positionOS = input.positionOS;
                output.positionCS = UnityObjectToClipPos(input.positionOS);

                //Find direction from camera position to vertex position in object space
                float4 cameraPositionOS = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
                output.cameraDirectionOS = normalize(cameraPositionOS - input.positionOS);

                return output;
            }

            //Bounding box functions
            bool checkBoundingBox(float3 position) { 
                return length(position) < 1 + Epsilon && length(position) > 0.35;
            }

            bool checkInnerBoundingBox(float3 position) {
                return length(position) / _Scale < 1 - Epsilon;
            }

            //Simplex noise
            /*
            #define NOISE_SIMPLEX_1_DIV_289 0.00346020761245674740484429065744f
            
            float4 permute(float4 x) {return fmod(x*x*34.0 + x, 289.0);} 
            float taylorInvSqrt(float r) {return 1.79284291400159 - 0.85373472095314 * r;}
            float4 taylorInvSqrt(float4 r) {return 1.79284291400159 - 0.85373472095314 * r;}
            
            float4 grad4(float j, float4 ip) {
                const float4 ones = float4(1.0, 1.0, 1.0, -1.0);
                float4 p, s;
                p.xyz = floor( frac(j * ip.xyz) * 7.0) * ip.z - 1.0;
                p.w = 1.5 - dot( abs(p.xyz), ones.xyz );
            
                p.xyz -= sign(p.xyz) * (p.w < 0);
            
                return p;
            }
            
            
            float snoise(float3 v) {
                const float2 C = float2(
                    0.166666666666666667, 
                    0.333333333333333333  
                );
                const float4 D = float4(0.0, 0.5, 1.0, 2.0);
            
                float3 i = floor( v + dot(v, C.yyy) );
                float3 x0 = v - i + dot(i, C.xxx);
            
                float3 g = step(x0.yzx, x0.xyz);
                float3 l = 1 - g;
                float3 i1 = min(g.xyz, l.zxy);
                float3 i2 = max(g.xyz, l.zxy);
            
                float3 x1 = x0 - i1 + C.xxx;
                float3 x2 = x0 - i2 + C.yyy; 
                float3 x3 = x0 - D.yyy;      
            
                i = fmod(i,289.0);
                float4 p = permute(permute(permute(i.z + float4(0.0, i1.z, i2.z, 1.0)) + i.y + float4(0.0, i1.y, i2.y, 1.0)) + i.x + float4(0.0, i1.x, i2.x, 1.0 ));
            
                float n_ = 0.142857142857; 
                float3 ns = n_ * D.wyz - D.xzx;
            
                float4 j = p - 49.0 * floor(p * ns.z * ns.z); 
            
                float4 x_ = floor(j * ns.z);
                float4 y_ = floor(j - 7.0 * x_ ); 
            
                float4 x = x_ *ns.x + ns.yyyy;
                float4 y = y_ *ns.x + ns.yyyy;
                float4 h = 1.0 - abs(x) - abs(y);
            
                float4 b0 = float4( x.xy, y.xy );
                float4 b1 = float4( x.zw, y.zw );
            
                float4 s0 = floor(b0)*2.0 + 1.0;
                float4 s1 = floor(b1)*2.0 + 1.0;
                float4 sh = -step(h, 0.0);
            
                float4 a0 = b0.xzyw + s0.xzyw*sh.xxyy ;
                float4 a1 = b1.xzyw + s1.xzyw*sh.zzww ;
            
                float3 p0 = float3(a0.xy,h.x);
                float3 p1 = float3(a0.zw,h.y);
                float3 p2 = float3(a1.xy,h.z);
                float3 p3 = float3(a1.zw,h.w);
            
                float4 norm = taylorInvSqrt(float4(dot(p0, p0), dot(p1, p1), dot(p2, p2), dot(p3, p3)));
                p0 *= norm.x;
                p1 *= norm.y;
                p2 *= norm.z;
                p3 *= norm.w;
            
                float4 m = max(0.6 - float4(dot(x0, x0),dot(x1, x1),dot(x2, x2),dot(x3, x3)),0.0);
                m = m * m;
                return 42.0 * dot(m*m,float4(dot(p0, x0),dot(p1, x1),dot(p2, x2),dot(p3, x3)));
            } */

            float hash( float n ) {
                return frac(sin(n)*43758.5453);
            }
        
            float noise( float3 x ) {
                // The noise function returns a value in the range -1.0f -> 1.0f
                float3 p = floor(x);
                float3 f = frac(x);
            
                f = f*f*(3.0-2.0*f);
                float n = p.x + p.y*57.0 + 113.0*p.z;
            
                return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
                    lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
                    lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
                    lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
            }

            float getR(float3 position) {return length(position);}
            float getTheta(float3 position) {return acos(position.z / length(position));}
            float getPhi(float3 position) {return sign(position.y) * acos(position.x / length(position.xy));}

            float ExampleWave(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                
                if(getR(position) < 0.7) {
                    return -1;
                } 

                if(getTheta(position) / PI > 0.5 - _NoiseLatitudeCutoff && getTheta(position) / PI < 0.5 + _NoiseLatitudeCutoff) {
                    return 0;
                } 

                _NoiseOffset.x = _Time.x * 5;

                float altitude = InverseLerp(0.7, 1, getR(position));
                float altitudeNoiseMap = clamp(1 - abs(noise(position * 10 + _NoiseOffset) - noise((position + _BooleanNoiseOffset) * 10 + _NoiseOffset)),0.2,1);

                position = normalize(position);
                float noiseMap = 1 - abs(noise(position * _NoiseScale + _NoiseOffset) - noise((position + _BooleanNoiseOffset) * _NoiseScale + _NoiseOffset));
                noiseMap = pow(noiseMap, _NoiseSharpness);

                if(noiseMap < _NoiseValueCutoff) {
                    return 0;
                }

                return noiseMap * (pow(1 - altitude, _AuroraSharpness));
                //return noiseMap * (pow(1 - altitude, _AuroraSharpness)) * pow(altitudeNoiseMap + 0.3, 2);
            }

            //Transfer functions
            float4 CutoffTransferFunction(float functionValue) {
                if(_UseSquaredTransfer) {functionValue *= functionValue;}

                if(functionValue > _TransferCutoff1) {
                    return float4(_TransferColor1.xyz, functionValue * _TransferScale1) / _TransparencyScale;
                } else if(functionValue > _TransferCutoff2 && _TransferColorCount > 1) {
                    return float4(_TransferColor2.xyz, functionValue * _TransferScale2) / _TransparencyScale;
                } else if(functionValue > _TransferCutoff3 && _TransferColorCount > 2) {
                    return float4(_TransferColor3.xyz, functionValue * _TransferScale3) / _TransparencyScale;
                } else if(functionValue > 0) {
                    return float4(_Outline.xxxx);
                }

                return float4(0,0,0,0);
            }

            float4 WaveFunctionTransferFunction(float functionValue) {
                float probabilityDensity = functionValue * functionValue;

                return(CutoffTransferFunction(probabilityDensity));
            }

            float4 NewTransferFunction(float functionValue) {
                if(functionValue < 0) {return float4(0,0,1,1);}
                return CutoffTransferFunction(functionValue);
            }

            //Blending functions
            float3 blendColors(float3 color1, float3 color2, float alpha1) {
                return color1 + color2 * (1 - alpha1);
            }

            float blendAlphas(float1 alpha1, float2 alpha2) {
                return alpha1 + alpha2 * (1 - alpha1);
            }

            //Fragment shader
            float4 Fragment(Interpolators input) : SV_Target {
                float3 rayOrigin = input.positionOS;
                float3 samplePosition = rayOrigin;

                float4 currentColor = float4(0, 0, 0, 0);
                float4 sampledColor;
                float stepSizeColorCorrection = _StepSize * 10;

                //Raymarch through the sphere until it leaves the outer bounding box
                while(checkBoundingBox(samplePosition)) {
                    sampledColor = NewTransferFunction(ExampleWave(samplePosition)) * stepSizeColorCorrection;

                    currentColor.rgb = blendColors(currentColor.rgb, sampledColor.rgb, currentColor.a);
                    currentColor.a = blendAlphas(currentColor.a, sampledColor.a);

                    samplePosition -= _StepSize * input.cameraDirectionOS;
                }

                return currentColor;
            }

            ENDCG
        }
    }
}