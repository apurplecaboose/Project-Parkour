Shader "Unlit/PointCloudNormals" {
	Properties {
		_MainTex("Texture (RGB)", 2D) = "white" {}
		_Ratio("Ratio", float) = 1.77
		_Size("Size", Float) = 0.1
	}

	SubShader {
		Tags{ "Queue" = "AlphaTest" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		Blend One OneMinusSrcAlpha
		AlphaToMask On
		Cull Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Ratio;
			float _Size;

			struct GS_INPUT {
				float4 vertex : POSITION;
				//float3 normal	: NORMAL;
				//float4 color	: COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct FS_INPUT {
				float4 vertex : SV_POSITION;
				//float3 normal : NORMAL;
				//float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			GS_INPUT vert(appdata_full v) {
				GS_INPUT o = (GS_INPUT)0;

                //o.vertex = v.texcoord.xyww;
				o.vertex = v.vertex;
				o.vertex.y *= _ProjectionParams.x;
				o.vertex.x -= 0.5; o.vertex.x *= 2;
				o.vertex.y += 0.5; o.vertex.y *= 2;

				//o.normal = v.normal;
				//o.color = v.color;
				return o;
			}

			[maxvertexcount(3)]
			void geom(point GS_INPUT tri[1], inout TriangleStream<FS_INPUT> triStream) {
				FS_INPUT pIn = (FS_INPUT)0;
				//pIn.normal = mul(unity_ObjectToWorld, tri[0].normal);
				//pIn.normal = float3(0, 0, -1);
				//pIn.color = tri[0].color;
				//pIn.color = float4(1.0, 1.0, 1.0, 1.0);

				//float4 vertex = mul(unity_ObjectToWorld, tri[0].vertex);
				float4 vertex = tri[0].vertex;
				float3 tangent = float3(1, 0, 0), up = float3(0, 1, 0);

				//pIn.vertex = mul(UNITY_MATRIX_VP, vertex + float4(tangent * -_Size / 1.5, 0));
				pIn.vertex = vertex + float4(tangent * -_Size / 1.5, 0);
				pIn.texcoord = float2(-0.5, 0);
				triStream.Append(pIn);

				//pIn.vertex = mul(UNITY_MATRIX_VP, vertex + float4(up * _Size, 0));
				pIn.vertex = vertex + float4(up * _Size * _Ratio, 0);
				pIn.texcoord = float2(0.5, 1.5);
				triStream.Append(pIn);

				//pIn.vertex = mul(UNITY_MATRIX_VP, vertex + float4(tangent * _Size / 1.5, 0));
				pIn.vertex = vertex + float4(tangent * _Size / 1.5, 0);
				pIn.texcoord = float2(1.5, 0);
				triStream.Append(pIn);
			}

			float4 frag(FS_INPUT i) : COLOR {
				float4 mainColor = tex2D(_MainTex, i.texcoord);
				return float4(mainColor.r, 0.0, 0.0, mainColor.a);
			}

			ENDCG
		}
	}
}