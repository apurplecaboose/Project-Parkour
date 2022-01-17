Shader "Custom/RaindropFX/Painter" {
    Properties {
		_MainTex("Base", 2D) = "" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	float _dropNum;
	sampler2D _MainTex;

	uniform vector _Drops[1024];

	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f vert(appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	half4 frag(v2f i) : COLOR {
		float4 mainColor = tex2D(_MainTex, i.uv);

		return mainColor;
	}

	ENDCG

	Subshader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback off
}
