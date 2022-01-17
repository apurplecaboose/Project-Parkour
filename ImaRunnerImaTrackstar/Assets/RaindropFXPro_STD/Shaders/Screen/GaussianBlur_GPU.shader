Shader "Hidden/Custom/GaussianBlur_GPU" {
	Properties{
		_MainTex("Base", 2D) = "" {}
	}
	SubShader{
		ZTest Always ZWrite Off Cull Off Fog { Mode Off }

		CGINCLUDE
		const static float WEIGHTS[8] = {  0.013,  0.067,  0.194,  0.226, 0.226, 0.194, 0.067, 0.013 };
		const static float OFFSETS[8] = { -6.264, -4.329, -2.403, -0.649, 0.649, 2.403, 4.329, 6.264 };

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;

		struct vsin {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};
		struct vs2psDown {
			float4 vertex : POSITION;
			float2 uv[4] : TEXCOORD0;
		};
		struct vs2psBlur {
			float4 vertex : POSITION;
			float2 uv[9] : TEXCOORD0;
		};

		vs2psDown vertDownsample(vsin IN) {
			vs2psDown OUT;
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			OUT.uv[0] = IN.uv;
			OUT.uv[1] = IN.uv + float2(-0.5, -0.5) * _MainTex_TexelSize.xy;
			OUT.uv[2] = IN.uv + float2(0.5, -0.5) * _MainTex_TexelSize.xy;
			OUT.uv[3] = IN.uv + float2(-0.5,  0.5) * _MainTex_TexelSize.xy;
			return OUT;
		}
		float4 fragDownsample(vs2psDown IN) : COLOR {
			float4 c = 0;
			for (uint i = 0; i < 4; i++) {
				float4 col = tex2D(_MainTex, IN.uv[i]);
				c += max(col.r, col.g) * 0.25;
			} 
			c.b = tex2D(_MainTex, IN.uv[0]).b;
			return c;
		}

		vs2psBlur vertBlurH(vsin IN) {
			vs2psBlur OUT;
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			for (uint i = 0; i < 8; i++) 
				OUT.uv[i] = IN.uv + float2(OFFSETS[i], 0) * _MainTex_TexelSize.xy;
			OUT.uv[8] = IN.uv;
			return OUT;
		}
		vs2psBlur vertBlurV(vsin IN) {
			vs2psBlur OUT;
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			for (uint i = 0; i < 8; i++) 
				OUT.uv[i] = IN.uv + float2(0, OFFSETS[i]) * _MainTex_TexelSize.xy;
			OUT.uv[8] = IN.uv;
			return OUT;
		}
		float4 fragBlur(vs2psBlur IN) : COLOR {
			float4 c = 0;
			for (uint i = 0; i < 8; i++) {
				float4 col = tex2D(_MainTex, IN.uv[i]);
				c += max(col.r, col.g) * WEIGHTS[i];
			}
			c.b = tex2D(_MainTex, IN.uv[8]).b;
			return c;
		}
		ENDCG

		Pass {
			CGPROGRAM
			#pragma vertex vertDownsample
			#pragma fragment fragDownsample
			ENDCG
		}
		Pass {
			CGPROGRAM
			#pragma vertex vertBlurH
			#pragma fragment fragBlur
			ENDCG
		}
		Pass {
			CGPROGRAM
			#pragma vertex vertBlurV
			#pragma fragment fragBlur
			ENDCG
		}
	}

	FallBack Off
}
