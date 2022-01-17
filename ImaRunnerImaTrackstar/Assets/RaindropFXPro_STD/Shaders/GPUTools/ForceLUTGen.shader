Shader "Hidden/Custom/ForceLUTShader" {
	Properties{
		_Gravity("Gravity", Vector) = (0, -9.8, 0, 0)
		_Wind("Wind", Vector) = (0, 0, 0, 0)
		_Velocity("Velocity", Vector) = (0, 0, 0, 0)

		_TurbTex("TurbTex", 2D) = "bump" {}

		_Turbulence("Turbulence", Range(0, 256)) = 1.0
	}

	SubShader{
		Pass{
			//ZTest Always Cull Off ZWrite Off
			//Tags{ "Queue" = "AlphaTest" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				//float3 color : COLOR0;
				float2 uv : TEXCOORD0;
			};

			float _NegSign;
			float _Turbulence;
			float4 _Wind;
			float4 _Gravity;
			float4 _Velocity;

			sampler2D _TurbTex;
			float4 _TurbTex_ST;
			int _useNormalMap;

			float3 decomposition(float3 norm, float3 force) {
				if (acos(dot(norm, normalize(force))) >= 3.14159 / 2.0) return force;
				else {
					float3 project = norm * dot(force, norm);
					return force - project;
				}
			}

			float4 decompToUVSpace(float3 bitangent, float3 tangent, float3 force) {		
				float3 projOnTangent = tangent * dot(force, tangent);
				float3 projOnBitangent = bitangent * dot(force, bitangent);

				float signTan = dot(normalize(projOnTangent), tangent);
				float signBiTan = dot(normalize(projOnBitangent), bitangent);

				return float4(
					length(projOnTangent), 
					length(projOnBitangent),
					signTan, signBiTan
				);
			}

			float3 UnityObjectToWorldDir(in float3 dir) {
				return normalize(mul((float3x3)unity_ObjectToWorld, dir));
			}

			float2 hash22(float2 p) {
				p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
				return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
			}

			float perlin_noise(float2 p) {
				float2 pi = floor(p);
				float2 pf = p - pi;
				float2 w = pf * pf*(3.0 - 2.0*pf);
				return lerp(lerp(dot(hash22(pi + float2(0.0, 0.0)), pf - float2(0.0, 0.0)),
					dot(hash22(pi + float2(1.0, 0.0)), pf - float2(1.0, 0.0)), w.x),
					lerp(dot(hash22(pi + float2(0.0, 1.0)), pf - float2(0.0, 1.0)),
						dot(hash22(pi + float2(1.0, 1.0)), pf - float2(1.0, 1.0)), w.x), w.y);
			}

			v2f vert(a2v v) {
				v2f o;
				o.pos = v.texcoord.xyww;
				o.pos.y *= _ProjectionParams.x;
				o.pos.x -= 0.5; o.pos.x *= 2;
				o.pos.y += 0.5; o.pos.y *= 2;

				o.normal = UnityObjectToWorldDir(v.normal);
				//o.color = o.normal * 0.5 + float3(0.5, 0.5, 0.5);
				v.tangent.xyz = UnityObjectToWorldDir(v.tangent.xyz);
				o.tangent = v.tangent;
				o.uv = v.texcoord.xy;
				return o;
			}

			float4 frag(v2f i) : SV_Target{

				if (_NegSign > 1) {
					float3 norm = normalize(float3(i.normal.xyz));
					float3 tang = normalize(float3(i.tangent.xyz));
					float3 bitan = normalize(cross(norm, tang));
					
					if (_useNormalMap > 0) {
						float4x4 tangentToWorld = float4x4(
							tang.x, bitan.x, norm.x, 0.0,
							tang.y, bitan.y, norm.y, 0.0,
							tang.z, bitan.z, norm.z, 0.0,
							0.0,	0.0,	 0.0,	 1.0
						);
					
						//float perlin = (perlin_noise(i.uv * 16.0) + 1) * 0.5;
						float4 turbCol = tex2D(_TurbTex, i.uv * _TurbTex_ST.xy + _TurbTex_ST.zw) * 2 - 1;
						//float4 turbCol = tex2D(_TurbTex, i.uv);
						//return turbCol * 2 - 1;

						norm = float3(turbCol.gb * 2.0, 1.0);
						float3 tan_tan = cross(float3(0, 1, 0), norm);
						norm = normalize(mul(tangentToWorld, norm));
						tang = normalize(mul(tangentToWorld, tan_tan));
						bitan = normalize(cross(norm, tang));
					}
					//return float4(tang, 1.0);

					float4 decomp = decompToUVSpace(bitan, tang, _Gravity.xyz + _Wind.xyz - _Velocity.xyz);
					
					//if (_NegSign > 2) {
					//	if (decomp.b < 0) decomp.r = 0; else decomp.r = 1;
					//	if (decomp.a < 0) decomp.g = 0; else decomp.g = 1;
					//}
					//if (_NegSign > 3) {
					//	decomp.rg *= decomp.ba;
					//}

					return decomp;
				} else {
					float3 norm = -normalize(float3(i.normal.xyz));
					float3 decomp = decomposition(norm, _Gravity.xyz + _Wind.xyz - _Velocity.xyz);

					if (_NegSign > 0) {
						if (decomp.r < 0) decomp.r = 0; else decomp.r = 1;
						if (decomp.g < 0) decomp.g = 0; else decomp.g = 1;
						if (decomp.b < 0) decomp.b = 0; else decomp.b = 1;
					}

					return float4(abs(decomp), 1.0);
				}

			}

			ENDCG
		}
	}
}