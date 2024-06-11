Shader "Instanced/LineSegmentVisual" {
	Properties {

	}

	SubShader {

		Tags {"Queue" = "Geometry" }

		Pass {
			CGPROGRAM

			#pragma vertex Vertex
			#pragma fragment Fragment
			#pragma target 5.0

			#include "UnityCG.cginc"
			
			StructuredBuffer<float4> _LineSegments;
			StructuredBuffer<float4> _LineDirections;

			float _LineLength;
			float _LineThickness;

			int _FieldLerpType;
			float _FieldLerpScale;
			float4 _LerpColor1;
			float4 _LerpColor2;

			struct MeshData {
				float4 positionOS : POSITION;
				uint id: SV_VertexID;
			};

			struct Interpolators {
				float4 positionCS : SV_POSITION;
				float signedMagnitude : TEXCOORD0;
			};

			//Helper function to create a transformation matrix to align an object to a vector
			float4x4 PointInDirection(float3 direction) {  
				float3 worldUp = float3(0,1,0);
				float3 dir1 = normalize(cross(direction, worldUp));
				float3 dir2 = normalize(cross(dir1, direction));   

				float4x4 transformationMatrix = float4x4(float4(dir1, 0),
														 float4(dir2, 0),
														 float4(direction, 0),
														 float4(0, 0, 0, 1));
				
				return transpose(transformationMatrix);
			}

			//Vertex shader
			Interpolators Vertex(MeshData input, uint instanceID : SV_InstanceID) {
                Interpolators output;

				//Throw out the filler points that the compute shader marked
				if(_LineSegments[instanceID].w == 0) {
					input.positionOS.z = 1000000;
				}

				//Pass along the signed magnitude for the edge of the line segment
				output.signedMagnitude = _LineDirections[instanceID].w;

				if(input.positionOS.z > 0 && _LineSegments[instanceID].w != -1) {
					output.signedMagnitude = _LineDirections[instanceID+1].w;
				}

				//Create a line segment by stretching the cube mesh along the forward direction
				input.positionOS.z *= _LineLength / _LineThickness;

				//Rotate the line segment to the field direction
				input.positionOS = mul(PointInDirection(_LineDirections[instanceID].xyz), input.positionOS);

				//Scale the line segment to set the line thickness
                input.positionOS *= _LineThickness;

				//Translate the line segment to the correct position
				input.positionOS.xyz -= (_LineLength/2) * _LineDirections[instanceID].xyz;
				input.positionOS.xyz += _LineSegments[instanceID].xyz;

				//Calculate the clip space position of the vertex
				output.positionCS = UnityObjectToClipPos(input.positionOS);

				return output;
			}

			//Fragment shader
			float4 Fragment(Interpolators input) : SV_Target {
				//Lerp the two lerp colors using the signed field magnitude to determine the output fragment color
				if(_FieldLerpType == 0) {
					return lerp(_LerpColor1, _LerpColor2, sign(input.signedMagnitude) * sqrt(abs(input.signedMagnitude))/(2 * _FieldLerpScale) + 0.5);
				} else if(_FieldLerpType == 1) {
					return lerp(_LerpColor1, _LerpColor2, sqrt(abs(input.signedMagnitude))/(2 * _FieldLerpScale));
				}

				return _LerpColor1;
			}

			ENDCG
		}
	}
}