/*{
	"CREDIT": "by mojovideotech",
	"CATEGORIES" : [ "generator" ],
	"DESCRIPTION" : "",
	"INPUTS" : [
		{ "NAME": "seed1", "TYPE": "float", "DEFAULT": 155, "MIN": 34, "MAX": 233 },
		{ "NAME": "seed2", "TYPE": "float", "DEFAULT": 649, "MIN": 89, "MAX": 987 },
		{ "NAME": "scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.25, "MAX": 2.0 },
		{ "NAME": "rate", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 3.0 },
		{ "NAME": "zoom", "TYPE": "float", "DEFAULT": 0.175, "MIN": -1.0, "MAX": 1.0 },
		{ "NAME": "line", "TYPE": "float", "DEFAULT": 0.367, "MIN": 0.0, "MAX": 0.5 },
		{ "NAME": "flash", "TYPE": "float", "DEFAULT": 7.5, "MIN": 0.5, "MAX": 10.0 },
		{ "NAME": "mirror", "TYPE": "bool", "DEFAULT": false },
		{ "NAME": "color", "TYPE": "bool", "DEFAULT": true },
		{ "NAME": "cycle", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.05, "MAX": 20.0 },
		{ "NAME": "dotBright", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
		{ "NAME": "armBright", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
		{ "NAME": "flakeSize", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 }
	]
}*/

#ifdef GL_ES
precision highp float;
#endif

#define S(a, b, t) smoothstep(a, b, t)

float N1(float n) { return fract(sin(n) * 43758.5453123); }
float N11(float p) {
	float fl = floor(p);
	float fc = fract(p);
	return mix(N1(fl), N1(fl + 1.0), fc);
}
float N21(vec2 p) { return fract(sin(p.x * floor(seed1) + p.y * floor(seed2)) * floor(seed2 + seed1)); }
vec2 N22(vec2 p) { return vec2(N21(p), N21(p + floor(seed2))); }

float L(vec2 p, vec2 a, vec2 b) {
	vec2 pa = p - a, ba = b - a;
	float t = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
	float d = length(pa - ba * t);
	float m = S(0.02, 0.0, d);
	d = length(a - b);
	float f = S(1.0, 0.8, d);
	m *= f;
	m += m * S(0.05, 0.06, abs(d - 0.5)) * 2.0;
	return m;
}

vec2 GetPos(vec2 p, vec2 o) {
	p += o;
	vec2 n = N22(p) * TIME * rate;
	p = sin(n) * line;
	return o + p;
}

float seg(vec2 p, vec2 a, vec2 b) {
	vec2 pa = p - a, ba = b - a;
	float t = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
	return length(pa - ba * t);
}

float snowflake(vec2 uv, vec2 center, float sz, float rot, float thickness) {
	vec2 p = uv - center;
	float d = 1e6;

	float a0 = rot;
	float a1 = rot + 1.0472;
	float a2 = rot + 2.0944;
	float a3 = rot + 3.1416;
	float a4 = rot + 4.1888;
	float a5 = rot + 5.2360;

	vec2 d0 = vec2(cos(a0), sin(a0));
	vec2 d1 = vec2(cos(a1), sin(a1));
	vec2 d2 = vec2(cos(a2), sin(a2));
	vec2 d3 = vec2(cos(a3), sin(a3));
	vec2 d4 = vec2(cos(a4), sin(a4));
	vec2 d5 = vec2(cos(a5), sin(a5));

	d = min(d, seg(p, vec2(0.0), d0 * sz));
	d = min(d, seg(p, vec2(0.0), d1 * sz));
	d = min(d, seg(p, vec2(0.0), d2 * sz));
	d = min(d, seg(p, vec2(0.0), d3 * sz));
	d = min(d, seg(p, vec2(0.0), d4 * sz));
	d = min(d, seg(p, vec2(0.0), d5 * sz));

	float bsz = sz * 0.3;

	vec2 b0a = vec2(cos(a0 + 1.5708), sin(a0 + 1.5708));
	vec2 b0p1 = d0 * sz * 0.4;
	vec2 b0p2 = d0 * sz * 0.65;
	d = min(d, seg(p, b0p1 - b0a * bsz * 0.5, b0p1 + b0a * bsz * 0.5));
	d = min(d, seg(p, b0p2 - b0a * bsz * 0.35, b0p2 + b0a * bsz * 0.35));

	vec2 b1a = vec2(cos(a1 + 1.5708), sin(a1 + 1.5708));
	vec2 b1p1 = d1 * sz * 0.4;
	vec2 b1p2 = d1 * sz * 0.65;
	d = min(d, seg(p, b1p1 - b1a * bsz * 0.5, b1p1 + b1a * bsz * 0.5));
	d = min(d, seg(p, b1p2 - b1a * bsz * 0.35, b1p2 + b1a * bsz * 0.35));

	vec2 b2a = vec2(cos(a2 + 1.5708), sin(a2 + 1.5708));
	vec2 b2p1 = d2 * sz * 0.4;
	vec2 b2p2 = d2 * sz * 0.65;
	d = min(d, seg(p, b2p1 - b2a * bsz * 0.5, b2p1 + b2a * bsz * 0.5));
	d = min(d, seg(p, b2p2 - b2a * bsz * 0.35, b2p2 + b2a * bsz * 0.35));

	vec2 b3a = vec2(cos(a3 + 1.5708), sin(a3 + 1.5708));
	vec2 b3p1 = d3 * sz * 0.4;
	vec2 b3p2 = d3 * sz * 0.65;
	d = min(d, seg(p, b3p1 - b3a * bsz * 0.5, b3p1 + b3a * bsz * 0.5));
	d = min(d, seg(p, b3p2 - b3a * bsz * 0.35, b3p2 + b3a * bsz * 0.35));

	vec2 b4a = vec2(cos(a4 + 1.5708), sin(a4 + 1.5708));
	vec2 b4p1 = d4 * sz * 0.4;
	vec2 b4p2 = d4 * sz * 0.65;
	d = min(d, seg(p, b4p1 - b4a * bsz * 0.5, b4p1 + b4a * bsz * 0.5));
	d = min(d, seg(p, b4p2 - b4a * bsz * 0.35, b4p2 + b4a * bsz * 0.35));

	vec2 b5a = vec2(cos(a5 + 1.5708), sin(a5 + 1.5708));
	vec2 b5p1 = d5 * sz * 0.4;
	vec2 b5p2 = d5 * sz * 0.65;
	d = min(d, seg(p, b5p1 - b5a * bsz * 0.5, b5p1 + b5a * bsz * 0.5));
	d = min(d, seg(p, b5p2 - b5a * bsz * 0.35, b5p2 + b5a * bsz * 0.35));

	return S(thickness, thickness * 0.3, d);
}

// Returns vec2: x = dot contribution, y = arm contribution
vec2 G(vec2 uv) {
	vec2 id = floor(uv);
	uv = fract(uv) - 0.5;
	vec2 g = GetPos(id, vec2(0));
	float mLines = 0.0;
	float mDot = 0.0;
	float mArm = 0.0;

	for (float y = -1.0; y <= 1.0; y++) {
		for (float x = -1.0; x <= 1.0; x++) {
			vec2 offs = vec2(x, y);
			vec2 p = GetPos(id, offs);
			mLines += L(uv, g, p);

			float sparkle = pow(sin(N21(id + offs) * 6.2831 + (flash * TIME)) * 0.4 + 0.6, flash);
			vec2 a = p - uv;
			float dist2 = dot(a, a);
			float intensity = 0.003 / max(dist2, 0.00005);

			// Center dot: pure point glow, unaffected by flakeSize
			float dotGlow = intensity * sparkle;
			mDot += dotGlow;

			// Snowflake arms: size driven by flakeSize
			float sz = clamp(intensity * 0.07 * flakeSize, 0.008 * flakeSize, 0.075 * flakeSize);
			float rot = N21(id + offs + vec2(7.3, 2.1)) * 6.2831 + TIME * 0.4;
			float thickness = sz * 0.08;
			float sf = snowflake(uv, p, sz, rot, thickness);
			mArm += sf * sparkle * clamp(intensity * 0.6, 0.0, 3.0);
		}
	}

	mLines += L(uv, GetPos(id, vec2(-1, 0)), GetPos(id, vec2(0, -1)));
	mLines += L(uv, GetPos(id, vec2(0, -1)), GetPos(id, vec2(1, 0)));
	mLines += L(uv, GetPos(id, vec2(1, 0)), GetPos(id, vec2(0, 1)));
	mLines += L(uv, GetPos(id, vec2(0, 1)), GetPos(id, vec2(-1, 0)));

	return vec2(mLines + mDot, mArm);
}

void main() {
	vec2 uv = (2.25 - scale) * (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
	if (mirror) { if (uv.x < 0.0) uv.x = abs(uv.x); }

	float mDot = 0.0;
	float mArm = 0.0;

	for (float i = 0.0; i < 1.0; i += 0.2) {
		float z = fract(i + TIME * zoom);
		float s = mix(10.0, 0.5, z);
		float f = S(0.0, 0.4, z) * S(1.0, 0.8, z);
		vec2 contrib = G(uv * s + (N11(i) * 100.0) * i);
		mDot += contrib.x * f;
		mArm += contrib.y * f;
	}

	vec3 col;
	if (color) { col = 0.5 + sin(vec3(1.0, 0.5, 0.75) * TIME * cycle) * 0.5; }
	else col = vec3(1.0);

	float m = mDot * dotBright + mArm * armBright;
	col *= m;
	gl_FragColor = vec4(col, 1.0);
}