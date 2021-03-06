Shader "Hidden/Custom/Wiper" {
    Properties {
		_MainTex("Base", 2D) = "" {}
		_WipeTex("Wipe", 2D) = "" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	sampler2D _WipeTex;

	v2f vert(appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	half4 frag(v2f i) : COLOR {
		float4 mainColor = tex2D(_MainTex, i.uv);
		float alpha = 1.0 - pow(tex2D(_WipeTex, i.uv).a, 2.2);
		mainColor = float4(
			mainColor.r * alpha,
			mainColor.g * alpha,
			mainColor.b * alpha,
			mainColor.a
		);

		return mainColor;
	}

	ENDCG

	Subshader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog {
				Mode off
			}

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback off
}
