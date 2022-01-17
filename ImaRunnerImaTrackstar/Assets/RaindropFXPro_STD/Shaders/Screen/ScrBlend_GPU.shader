Shader "Custom/RaindropFX/ScreenBlendEffect_GPU" {
	Properties{
		_MainTex("Base", 2D) = "" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	vector _MainTex_TexelSize;
	sampler2D _AdditionalColTex;
	float _ColBlendAmount;
	sampler2D _HeightMap;
	float _Distortion;

	float _inBlack;
	float _inWhite;
	float _outWhite;
	float _outBlack;

	// color level
	float GetPixelLevel(float inPixel) {
		return (((inPixel * 255.0) - _inBlack) / (_inWhite - _inBlack) * (_outWhite - _outBlack) + _outBlack) / 255.0;
	}

	float4 HeightToNormal(float2 nowPos) {
		nowPos.x -= 1.0 * _MainTex_TexelSize.x;
		float4 leftColor = tex2D(_HeightMap, nowPos);
		float xLeft = GetPixelLevel(max(leftColor.r, max(leftColor.g, leftColor.b)));

		nowPos.x += 2.0 * _MainTex_TexelSize.x;
		float4 rightColor = tex2D(_HeightMap, nowPos);
		float xRight = GetPixelLevel(max(rightColor.r, max(rightColor.g, rightColor.b)));

		nowPos.x -= 1.0 * _MainTex_TexelSize.x;
		nowPos.y += 1.0 * _MainTex_TexelSize.y;
		float4 upColor = tex2D(_HeightMap, nowPos);
		float yUp = GetPixelLevel(max(upColor.r, max(upColor.g, upColor.b)));

		nowPos.y -= 2.0 * _MainTex_TexelSize.y;
		float4 downColor = tex2D(_HeightMap, nowPos);
		float yDown = GetPixelLevel(max(downColor.r, max(downColor.g, downColor.b)));

		float xDelta = ((xLeft - xRight) + 1.0) * 0.5;
		float yDelta = ((yUp - yDown) + 1.0) * 0.5;

		return float4(clamp(xDelta, 0.0, 1.0), clamp(yDelta, 0.0, 1.0), 1.0f, 1.0f);
	}

	v2f vert(appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	float4 frag(v2f i) : COLOR {
		float4 mainColor = HeightToNormal(i.uv);

		float2 bump = (mainColor * 2 - 1).rg;
		//float4 addCol = tex2D(_AdditionalColTex, i.uv);
		//mainColor = tex2D(_MainTex, i.uv + bump * _Distortion) * (1.0 - _ColBlendAmount) + (addCol * 0.5 + tex2D(_MainTex, i.uv) * 0.5) * _ColBlendAmount;
		mainColor = tex2D(_MainTex, i.uv + bump * _Distortion);
		//mainColor = mainColor;
		return mainColor;
	}

	ENDCG

	Subshader {
		Pass{
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