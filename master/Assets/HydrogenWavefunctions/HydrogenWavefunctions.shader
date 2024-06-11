Shader "Unlit/HydrogenWavefunctions" {
    Properties {
        //Quality properties 
        [Header(Quality)]
        _StepSize ("Step Size", Range(0.001,0.01)) = 0.001
        _Scale ("Scale", Range(0,1)) = 0.576
        _Outline ("Outline Strength", Range(0,1)) = 0.127

        //Transfer function properties
        [Header(Transfer Function)]
        [IntRange] _UseSquaredTransfer ("Use Squared Transfer", Range(0,1)) = 0
        [IntRange] _TransferColorCount ("Number of Transfer Colors", Range(1,3)) = 3
        _TransferColor1 ("Transfer Color 1", Color) = (1,0,0,1)
        _TransferColor2 ("Transfer Color 2", Color) = (0,1,0,1)
        _TransferColor3 ("Transfer Color 3", Color) = (0,0,1,1)
        _TransferCutoff1 ("Transfer Cutoff 1", Range(0,1)) = 0.549
        _TransferCutoff2 ("Transfer Cutoff 2", Range(0,1)) = 0.372
        _TransferCutoff3 ("Transfer Cutoff 3", Range(0,1)) = 0.11
        _TransferScale1 ("Transfer Scale 1", Range(0,2)) = 0.172
        _TransferScale2 ("Transfer Scale 2", Range(0,2)) = 0.188
        _TransferScale3 ("Transfer Scale 3", Range(0,2)) = 0.473

        //Hydrogen wave function properties
        [Header(wave Function)]
        [IntRange] _n ("n", Range(1,10)) = 3
        [IntRange] _l ("l", Range(0,10)) = 1
        [IntRange] _m ("m", Range(-10,10)) = 0
        _FunctionAmplitudeScale ("Function Amplitude Scale", Range(0,10)) = 1.82
        _FunctionInputScale ("Function Input Scale", Range(0.01,100)) = 1.3
        _a0 ("Bohr Radius", Range(0.001,10)) = 0.03
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
            #define _PI 3.141592653

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

            int _n;
            int _l;
            int _m;
            float _FunctionAmplitudeScale;
            float _FunctionInputScale;
            float _a0;

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
                return length(position) < 1 + Epsilon;
            }

            bool checkInnerBoundingBox(float3 position) {
                //Spherical inner bounding box
                return length(position) < 1 + Epsilon;

                //Cube inner bounding box
                //return abs(position.x) / _Scale < 0.5 + Epsilon && abs(position.y) / _Scale < 0.5 + Epsilon && abs(position.z) / _Scale < 0.5 + Epsilon;
            }

            //Spherical Coordinate Transforms
            float getR(float3 position) {return length(position);}
            float getTheta(float3 position) {return acos(position.z / length(position));}
            float getPhi(float3 position) {return sign(position.y) * acos(position.x / length(position.xy));}

            //Hydrogen Functions
            float Hydrogen100(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (sqrt(_PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * exp(-p);
            }

            float Hydrogen200(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (4 * sqrt(2 * _PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * (2 - p) * exp(-p/2);
            }

            float Hydrogen210(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (4 * sqrt(2 * _PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * p * exp(-p/2) * cos(theta);
            }

            float Hydrogen211(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (8 * sqrt(_PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * p * exp(-p/2) * sin(theta);
            }

            float Hydrogen300(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (81 * sqrt(3 * _PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * (27 - 18 * p + 2 * pow(p,2)) * exp(-p/2);
            }

            float Hydrogen310(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (81 * sqrt(3 * _PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * (6 - p) * p * exp(-p/3) * cos(theta);
            }

            float Hydrogen311(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (81 * sqrt(3 * _PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * (6 - p) * p * exp(-p/3) * sin(theta);
            }

            float Hydrogen320(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (81 * sqrt(6 * _PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * pow(p,2) * exp(-p/3) * (3 * pow(cos(theta),2) - 1);
            }

            float Hydrogen321(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (81 * sqrt(_PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * pow(p,2) * exp(-p/3) * sin(theta) * cos(theta);
            }

            float Hydrogen322(float3 position) {
                if(!checkInnerBoundingBox(position)) {return 0;}
                position /= _Scale;
                position *= _FunctionInputScale;

                float r = getR(position);
                float theta = getTheta(position);
                float phi = getPhi(position);

                float p = r / _a0;
                float A = 1 / (162 * sqrt(_PI) * pow(_a0, 1.5));

                return _FunctionAmplitudeScale * A * pow(p,2) * exp(-p/3) * pow(sin(theta),2);
            }

            float HydrogenWaveFunction(float3 position) {
                if(_n == 1) {
                    if(_l == 0) {
                        if(_m == 0) {
                            return Hydrogen100(position);
                        } else {
                            return 0;
                        }
                    } else {
                        return 0;
                    }
                } else if(_n == 2) {
                    if(_l == 0) {
                        if(_m == 0) {
                            return Hydrogen200(position);
                        } else {
                            return 0;
                        }
                    } else if(_l == 1) {
                        if(_m == -1) {
                            return Hydrogen211(position);
                        } else if(_m == 0) {
                            return Hydrogen210(position);
                        } else if(_m == 1) {
                            return Hydrogen211(position);
                        } else {
                            return 0;
                        }
                    } else {
                        return 0;
                    }
                } else if(_n == 3) {
                    if(_l == 0) {
                        if(_m == 0) {
                            return Hydrogen300(position);
                        } else {
                            return 0;
                        }
                    } else if(_l == 1) {
                        if(_m == -1) {
                            return Hydrogen311(position);
                        } else if(_m == 0) {
                            return Hydrogen310(position);
                        } else if(_m == 1) {
                            return Hydrogen311(position);
                        } else {
                            return 0;
                        }
                    } else if(_l == 2) {
                        if(_m == -2) {
                            return Hydrogen322(position);
                        } else if(_m == -1) {
                            return Hydrogen321(position);
                        } else if(_m == 0) {
                            return Hydrogen320(position);
                        } else if(_m == 1) {
                            return Hydrogen321(position);
                        } else if(_m == 2) {
                            return Hydrogen322(position);
                        } else {
                            return 0;
                        }
                    } else {
                        return 0;
                    }
                } else {
                    return 0;
                }
            }

            //Transfer functions
            float4 CutoffTransferFunction(float functionValue) {
                if(_UseSquaredTransfer) {functionValue *= functionValue;}

                if(functionValue > _TransferCutoff1) {
                    return float4(_TransferColor1.xyz, functionValue * _TransferScale1);
                } else if(functionValue > _TransferCutoff2 && _TransferColorCount > 1) {
                    return float4(_TransferColor2.xyz, functionValue * _TransferScale2);
                } else if(functionValue > _TransferCutoff3 && _TransferColorCount > 2) {
                    return float4(_TransferColor3.xyz, functionValue * _TransferScale3);
                } else if(functionValue > 0) {
                    return float4(_Outline.xxxx);
                }

                return float4(0,0,0,0);
            }

            float4 WaveFunctionTransferFunction(float functionValue) {
                float probabilityDensity = functionValue * functionValue;

                return(CutoffTransferFunction(probabilityDensity));
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
                    sampledColor = WaveFunctionTransferFunction(HydrogenWaveFunction(samplePosition)) * stepSizeColorCorrection;

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