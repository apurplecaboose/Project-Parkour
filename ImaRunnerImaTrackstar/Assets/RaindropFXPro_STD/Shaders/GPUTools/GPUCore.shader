Shader "Custom/RaindropFX/GPUCore" {
    Properties {
		_MainTex("Base", 2D) = "" {}
		_NoiseTex("Noise", 2D) = "" {}
		_TurbulenceLUT("Turbulence LUT", 2D) = "" {}
		_TurbScale("Force", Range(0, 10)) = 0
		_ForceLUT("Force LUT", 2D) = "black" {}
		//_SignLUT("Sign LUT", 2D) = "" {}
		_Force("Force", Vector) = (0, 0, 0, 0)

		_staDens("Static Drop Spawn Rate", range(0, 32)) = 5.0
		_dynDens("Dynamic Drop Spawn Rate", range(0, 32)) = 2.0
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _NoiseTex;
	sampler2D _TurbulenceLUT;
	sampler2D _ForceLUT;
	//sampler2D _SignLUT;
	float _TurbScale;
	float4 _Force;
	float4 _MainTex_TexelSize;

	float _staDens;
	float _dynDens;

	int _fadeout;

	const float epsilon = 1e-6;

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

	// color level
	float GetPixelLevel(float inPixel, float2 uv, float shrinkSpeed) {
		float _inBlack = shrinkSpeed, _inWhite = 255.0;
		float _outWhite = 255.0, _outBlack = 0.0;
		return (((inPixel * 255.0) - _inBlack) / (_inWhite - _inBlack) * (_outWhite - _outBlack) + _outBlack) / 255.0;
	}

	// force generator
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

	float2 SamplingForceLUT(float2 uv) {
		float4 data = tex2D(_ForceLUT, uv);
        //float2 sign = tex2D(_SignLUT, uv).rg;

        //if (sign.r < 0.5) data.r *= -1;
        //if (sign.g > 0.5) data.g *= -1;

        if (data.b < 0.5) data.r *= -1;
        if (data.a > 0.5) data.g *= -1;

		return data.rg;
	}

	float Turbulence(float2 pos) {
		return (
			sin(pos.x * 4.0 * 0.01 + pos.y * 0.05) +
			cos(pos.x * 0.1 * 0.01 + pos.y * 0.1) * 2.0 +
			sin(pos.x * 16.0 * 0.01 + pos.y * 0.1)
		) * 0.5;
	}

	float DistanceField(float2 p, float2 d) {
		float tmin = 0, tmax = 1;
		d = p + d * 2.0;

		for (int i = 0; i < 2; i++) {
			if (abs(d[i] - p[i]) < epsilon) {
				if (p[i] < 0 || p[i] > 1) return -1;
			} else {
				float ood = 1.0 / d[i];
				float t1 = (0 - p[i]) * ood;
				float t2 = (1 - p[i]) * ood;
				if (t1 > t2) { float tmp = t1; t1 = t2; t2 = tmp; }
				if (t1 > tmin) tmin = t1;
				if (t2 < tmax) tmax = t2;
				if (tmin > tmax) return -1;
			}
		}

		return tmin > 0 ? tmin : tmax;
	}

	// raindrop generator
	float4 permute(float4 x) {
	  return fmod((34.0 * x + 1.0) * x, 289.0);
	}
	float3 permute(float3 x) {
	  return fmod((34.0 * x + 1.0) * x, 289.0);
	}

	float2 cellular2x2x2(float3 P) {
		float K = 0.142857142857;
		float Ko = 0.428571428571;
		float K2 = 0.020408163265306;
		float Kz = 0.166666666667;
		float Kzo = 0.416666666667;
		float jitter = 0.8;

		float3 Pi = fmod(floor(P), 289.0);
 		float3 Pf = frac(P);
		float4 Pfx = Pf.x + float4(0.0, -1.0, 0.0, -1.0);
		float4 Pfy = Pf.y + float4(0.0, 0.0, -1.0, -1.0);
		float4 p = permute(Pi.x + float4(0.0, 1.0, 0.0, 1.0));
		p = permute(p + Pi.y + float4(0.0, 0.0, 1.0, 1.0));
		float4 p1 = permute(p + Pi.z);
		float4 p2 = permute(p + Pi.z + float4(1.0, 1.0, 1.0, 1.0));
		float4 ox1 = frac(p1 * K) - Ko;
		float4 oy1 = fmod(floor(p1 * K), 7.0) * K - Ko;
		float4 oz1 = floor(p1 * K2) * Kz - Kzo;
		float4 ox2 = frac(p2 * K) - Ko;
		float4 oy2 = fmod(floor(p2 * K), 7.0) * K - Ko;
		float4 oz2 = floor(p2 * K2) * Kz - Kzo;
		float4 dx1 = Pfx + jitter * ox1;
		float4 dy1 = Pfy + jitter * oy1;
		float4 dz1 = Pf.z + jitter * oz1;
		float4 dx2 = Pfx + jitter * ox2;
		float4 dy2 = Pfy + jitter * oy2;
		float4 dz2 = Pf.z - 1.0 + jitter * oz2;
		float4 d1 = dx1  *  dx1 + dy1 * dy1 + dz1 * dz1;
		float4 d2 = dx2 * dx2 + dy2 * dy2 + dz2 * dz2;

		float4 d = min(d1,d2);
		d2 = max(d1,d2);
		d.xy = (d.x < d.y) ? d.xy : d.yx;
		d.xz = (d.x < d.z) ? d.xz : d.zx;
		d.xw = (d.x < d.w) ? d.xw : d.wx;
		d.yzw = min(d.yzw, d2.yzw);
		d.y = min(d.y, d.z);
		d.y = min(d.y, d.w);
		d.y = min(d.y, d2.x);
		return sqrt(d.xy);
	}

	float randomDrop(float2 uv, float time) {
		float2 st = uv * 10.0;
		st.x *= _MainTex_TexelSize.z / _MainTex_TexelSize.w;

		float2 F = cellular2x2x2(float3(st, time / 4.0));
		float n = smoothstep(0.1, 0.01, F.x);
		//return float4(n, n, n, 1.0);
		return n;
	}

	float randomDropFast(float2 uv, float time, float rad, float _dens, bool _hard) {
		float dens = _dens / 255.0;
		float blue = tex2D(
			_NoiseTex, 
			trunc(uv / rad) * _MainTex_TexelSize.xy + 
			float2(time, time)
		).r;
    
		float adj = blue / dens, res = 0.0;
		if (blue < dens) {
			res += clamp(smoothstep(
				1.0, 1.0 - 4.0 / adj, 
				length(2.0 * frac(uv / rad) - 1.0)
			) * (_hard ? 128.0 : 12.0), 0, 1);

			if (!_hard) res = pow(res, 4.0);
		}

		return res;
	}

	float GradNoise(float2 uv) {
		return frac(sin(dot(uv, float2(12.9898,78.233))) * 43758.5453123);
		//return frac(52.9829189f * frac(uv.x * 0.06711056f + uv.y * 0.00583715f));
	}

	float BoxFilter(float2 uv) {
		int kernal = 1;
		float sum = 0.0;
		for (int x = -kernal; x <= kernal; x++) {
			for (int y = -kernal; y <= kernal; y++) {
				sum += tex2D(_MainTex, uv + float2(x, y) * _MainTex_TexelSize.xy).g;
			}
		}
		return sum /= pow(kernal * 2.0 + 1.0, 2.0);
	}

	float2 RTDir(float2 p) {
		return float2(p.y, -p.x);
	}

	half4 frag(v2f i) : COLOR {
		//float drop = randomDropFast(i.uv * _MainTex_TexelSize.zw, GradNoise(_Time.xy) * 100.0);
		//return float4(drop, drop, drop, 1.0);

		float randomNum = GradNoise(_Time.xy);
		float shrink = randomNum * 0.15;
		float fade_sta = clamp(randomNum, 0.94, 0.99);
		float fade = clamp(randomNum, 0.998, 0.999);

		float4 pCol = tex2D(_MainTex, i.uv);
		float4 mainColor = float4(0.0, 
			//max(pCol.r, GetPixelLevel(pCol.g * 0.9, i.uv)), 
			//max(pCol.r, GetPixelLevel(BoxFilter(i.uv) * fade_sta, i.uv, shrink)), 
			max(pCol.r, GetPixelLevel(tex2D(_MainTex, i.uv).g * fade_sta, i.uv, shrink)),
			max(_fadeout * randomDropFast(
					i.uv * _MainTex_TexelSize.zw, _Time.x * 5.0, 
					randomNum * 24.0, _staDens, false
				), 
				GetPixelLevel(pCol.b * fade_sta, i.uv, shrink)
			), 1.0
		);

		//float perlin = (perlin_noise(i.uv * 8.0) + 1.0) / 2.0;
		//float2 sp = i.uv - (tex2D(_TurbulenceLUT, i.uv).xy * perlin * 15.0 + _Force.xy) * _MainTex_TexelSize.xy;
		//float2 sp = i.uv - SamplingForceLUT(i.uv) * _MainTex_TexelSize.xy;
		//float2 sp = i.uv - float2(0, (1.2 - i.uv.y) * -0.016) - Turbulence(i.uv * 256.0) * _TurbScale * 0.001;
		
		float2 dir = normalize(_Force.xy);
		float acc = 0.1 + DistanceField(i.uv, -dir);
		float2 turb = _TurbScale < epsilon ? float2(0, 0) :
			(RTDir(dir) * Turbulence(float2(i.uv.x, acc) * 256.0) * _TurbScale * 0.001);
		float2 sp = i.uv - turb - _Force.xy * acc * 0.0032 - SamplingForceLUT(i.uv) * 0.005;// *_MainTex_TexelSize.zw;
		if (sp.x >= 0 && sp.x <= 1 && sp.y >= 0 && sp.y <= 1)
			mainColor.r = max(
				GetPixelLevel(
					tex2D(_MainTex, sp).r * fade,
					i.uv, shrink * 8.0
				), 
				(_SinTime.w * _fadeout) == 0 ? 0.0 :
				randomDropFast(
					i.uv * _MainTex_TexelSize.zw, 
					randomNum * 100.0, 
					clamp(randomNum, 0.8, 1.5) * 6.0, 
					_dynDens, true
				)
			);
		//return float4(1.0, 1.0, 1.0, 1.0) * perlin;
		//return tex2D(_TurbulenceLUT, i.uv);
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
