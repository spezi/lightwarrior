/*{
    "DESCRIPTION": "Organic Kaleidoscope Mandala — scalloped petal-discs with fibrous texture and radial symmetry",
    "CREDIT": "Antigravity AI",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        {
            "NAME": "scale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Scale"
        },
        {
            "NAME": "rotation",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 6.28318,
            "LABEL": "Rotation"
        },
        {
            "NAME": "iterations",
            "TYPE": "float",
            "DEFAULT": 12.0,
            "MIN": 3.0,
            "MAX": 24.0,
            "LABEL": "Iterations"
        }
    ]
}*/

precision mediump float;

#define PI  3.14159265358979
#define TAU 6.28318530717959

// ── Utilities ────────────────────────────────────────────────────────────────

mat2 rot2(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

float h21(vec2 p) {
    p = fract(p * vec2(127.1, 311.7));
    p += dot(p, p.yx + 45.32);
    return fract(p.x * p.y);
}

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(h21(i),             h21(i + vec2(1,0)), u.x),
        mix(h21(i + vec2(0,1)), h21(i + vec2(1,1)), u.x),
        u.y
    );
}

float fbm(vec2 p) {
    float v = 0.0;
    v += vnoise(p)       * 0.55;
    v += vnoise(p * 2.1) * 0.28;
    v += vnoise(p * 4.3) * 0.12;
    v += vnoise(p * 8.7) * 0.05;
    return v;
}

// N-fold dihedral symmetry (kaleidoscope fold)
vec2 kfold(vec2 p, float n) {
    float a = atan(p.y, p.x);
    float r = length(p);
    float s = PI / n;
    a = mod(a, 2.0 * s);
    a = abs(a - s);
    return vec2(cos(a), sin(a)) * r;
}

// ── Disc/Petal primitives ────────────────────────────────────────────────────

// 3D convex shading: bright center, dark rim
float discShade(float d, float r) {
    float t = clamp(d / r, 0.0, 1.0);
    return (1.0 - t * t) * (1.0 - t * 0.3);
}

// Soft anti-aliased coverage mask
float discMask(float d, float r) {
    return smoothstep(r + 0.005, r - 0.005, d);
}

// Dark outline between overlapping discs
float discShadow(float d, float r) {
    return smoothstep(r + 0.03, r + 0.002, d) *
           smoothstep(r - 0.006, r + 0.008, d);
}

// Rich fibrous texture inside each petal
float fiberTex(vec2 p, float seed) {
    float a = atan(p.y, p.x);
    float r = length(p);

    // Primary radial veins — strong, sharp
    float veins1 = sin(a * 18.0 + seed * 3.7) * 0.5 + 0.5;
    veins1 = pow(veins1, 3.5);

    // Secondary finer veins
    float veins2 = sin(a * 32.0 - seed * 2.1) * 0.5 + 0.5;
    veins2 = pow(veins2, 4.5) * 0.35;

    // Concentric growth rings
    float rings = sin(r * 30.0 + seed * 0.9 - TIME * 0.06) * 0.5 + 0.5;
    rings *= rings;

    // Organic noise layer
    float n = fbm(p * 12.0 + seed * 1.3);

    // Spokes × rings + noise
    float fiber = (veins1 + veins2) * rings;
    return mix(fiber, n, 0.22);
}

// ── Evaluate one ring of petals ──────────────────────────────────────────────

vec4 evalRing(float ri, vec2 p, float discR, float ringStep, float symN) {
    float ringR = (ri + 0.5) * ringStep;

    // Alternate ring rotation for organic breathing
    float spin = TIME * 0.012 * (mod(ri, 2.0) < 0.5 ? 1.0 : -1.0);
    vec2  lp   = rot2(spin) * p;
          lp   = kfold(lp, symN);

    // Dense angular packing — factor 1.45 for heavy overlap (more scalloped)
    float nDiscs = max(symN, floor(TAU * ringR / (discR * 1.45)));
    float dAngle = TAU / nDiscs;

    float lpA     = atan(lp.y, lp.x);
    float snapped = floor(lpA / dAngle + 0.5) * dAngle;
    vec2  center  = vec2(cos(snapped), sin(snapped)) * ringR;
    vec2  local   = lp - center;
    float dist    = length(local);

    // Elongate petals slightly radially for that leaf/petal shape
    vec2 elongated = local;
    elongated.x *= 0.85;  // compress tangentially → radially elongated petal
    float eDist = length(elongated);

    float mask   = discMask(eDist, discR);
    float shade  = discShade(eDist, discR) * mask;
    float fiber  = fiberTex(local / discR, ri * 4.13) * mask;
    float shadow = discShadow(eDist, discR);

    return vec4(mask, shade, fiber, shadow);
}

// ── Main ─────────────────────────────────────────────────────────────────────

void main() {
    // Fill entire canvas — resolution independent
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Apply rotation + slow time rotation
    uv = rot2(rotation + TIME * 0.025) * uv;

    // Scale — controls how much of the pattern fills the canvas
    uv /= scale;

    // Kaleidoscope fold with iterations
    float symN = floor(iterations);
    vec2 p = kfold(uv, symN);
    float pR = length(p);

    // Disc radius & ring spacing — tuned for dense overlap
    float discR    = 0.30 / (symN * 0.12 + 1.0);
    float ringStep = discR * 1.35;

    // Find base ring, check ±3 neighbors for dense layered overlap
    float baseRing = floor(pR / ringStep);

    float accumMask   = 0.0;
    float accumShade  = 0.0;
    float accumFiber  = 0.0;
    float accumShadow = 0.0;

    for (int di = -3; di <= 3; di++) {
        float ri = baseRing + float(di);
        if (ri < 0.0) continue;

        vec4 r = evalRing(ri, p, discR, ringStep, symN);

        // Front-to-back compositing: inner rings on top
        float alpha = r.x * (1.0 - accumMask * 0.45);
        accumMask   += alpha;
        accumShade  += r.y;
        accumFiber  += r.z;
        accumShadow += r.w;
    }

    accumMask   = clamp(accumMask,   0.0, 1.0);
    accumShade  = clamp(accumShade,  0.0, 1.0);
    accumFiber  = clamp(accumFiber,  0.0, 1.0);
    accumShadow = clamp(accumShadow, 0.0, 1.0);

    // ── Central star & eye ──────────────────────────────────────────────────
    float pAc   = atan(p.y, p.x);
    float starR = 0.025 + 0.035 * abs(cos(pAc * symN * 0.5));
    float star  = smoothstep(0.005, -0.005, pR - starR);
    float eye   = smoothstep(0.018, 0.006, pR);

    // Concentric detail rings near center
    float cRings = sin(pR * 120.0 - TIME * 0.3) * 0.5 + 0.5;
    cRings *= smoothstep(0.12, 0.02, pR);

    // ── Warm monochrome sepia palette ───────────────────────────────────────
    vec3 cBg     = vec3(0.70, 0.67, 0.60);
    vec3 cBright = vec3(0.92, 0.89, 0.82);
    vec3 cMid    = vec3(0.76, 0.73, 0.66);
    vec3 cFiber  = vec3(0.16, 0.13, 0.08);
    vec3 cShadow = vec3(0.05, 0.04, 0.02);
    vec3 cStar   = vec3(0.85, 0.82, 0.76);

    // Background
    vec3 col = cBg;

    // Disc base: mid → bright using 3D convex shading
    vec3 discCol = mix(cMid, cBright, accumShade);
    col = mix(col, discCol, accumMask);

    // Fiber veins darken the surface
    col = mix(col, cFiber, accumFiber * 0.70 * accumMask);

    // Inter-disc shadow outlines
    col = mix(col, cShadow, accumShadow * 0.85);

    // Center ring detail
    col = mix(col, cShadow, cRings * 0.4);

    // Central star
    col = mix(col, cStar,   star * 0.45);
    col = mix(col, cShadow, star * 0.30);
    col = mix(col, cShadow, eye);

    // Subtle organic noise variation across entire surface
    float globalNoise = fbm(uv * 3.0 + TIME * 0.01);
    col *= 0.92 + 0.08 * globalNoise;

    gl_FragColor = vec4(col, 1.0);
}
