/*{
  "CATEGORIES": [ "Generator", "Texture", "Stylize", "Background" ],
  "DESCRIPTION": "Cinematic crystalline resin background texture with Mad-friendly controls",
  "INPUTS": [
    { "NAME": "center",       "TYPE": "point2D", "DEFAULT": [0.5, 0.5], "MIN": [0.0, 0.0], "MAX": [1.0, 1.0] },

    { "NAME": "scale",        "TYPE": "float",   "DEFAULT": 6.5,  "MIN": 1.0,  "MAX": 20.0 },
    { "NAME": "detail",       "TYPE": "float",   "DEFAULT": 0.65, "MIN": 0.2,  "MAX": 0.9 },
    { "NAME": "speed",        "TYPE": "float",   "DEFAULT": 0.12, "MIN": 0.0,  "MAX": 2.0 },

    { "NAME": "flowAngle",    "TYPE": "float",   "DEFAULT": 0.35, "MIN": -3.14159, "MAX": 3.14159 },
    { "NAME": "flowStrength", "TYPE": "float",   "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 2.0 },

    { "NAME": "distort",      "TYPE": "float",   "DEFAULT": 0.45, "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "microDistort", "TYPE": "float",   "DEFAULT": 0.28, "MIN": 0.0,  "MAX": 2.0 },

    { "NAME": "depth",        "TYPE": "float",   "DEFAULT": 0.9,  "MIN": 0.0,  "MAX": 2.5 },
    { "NAME": "crystal",      "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.5 },
    { "NAME": "sparkle",      "TYPE": "float",   "DEFAULT": 1.1,  "MIN": 0.0,  "MAX": 3.5 },

    { "NAME": "contrast",     "TYPE": "float",   "DEFAULT": 1.2,  "MIN": 0.4,  "MAX": 3.0 },
    { "NAME": "gamma",        "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.4,  "MAX": 2.2 },
    { "NAME": "glow",         "TYPE": "float",   "DEFAULT": 0.95, "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "centerGlow",   "TYPE": "float",   "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "ringAmount",   "TYPE": "float",   "DEFAULT": 0.28, "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "vignette",     "TYPE": "float",   "DEFAULT": 0.45, "MIN": 0.0,  "MAX": 1.5 },

    { "NAME": "paletteShift", "TYPE": "float",   "DEFAULT": 0.0,  "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "grain",        "TYPE": "float",   "DEFAULT": 0.06, "MIN": 0.0,  "MAX": 0.5 },

    { "NAME": "alphaFloor",   "TYPE": "float",   "DEFAULT": 0.72, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "alphaAmount",  "TYPE": "float",   "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 1.0 },

    { "NAME": "shadowColor",  "TYPE": "color",   "DEFAULT": [0.05, 0.02, 0.01, 1.0] },
    { "NAME": "amberColor",   "TYPE": "color",   "DEFAULT": [0.72, 0.28, 0.04, 1.0] },
    { "NAME": "hotColor",     "TYPE": "color",   "DEFAULT": [1.00, 0.74, 0.25, 1.0] },
    { "NAME": "sparkColor",   "TYPE": "color",   "DEFAULT": [1.00, 0.95, 0.75, 1.0] }
  ]
}*/

#define PI 3.14159265359

mat2 rot2(float a) {
    float c = cos(a);
    float s = sin(a);
    return mat2(c, -s, s, c);
}

float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 345.45));
    p += dot(p, p + 34.345);
    return fract(p.x * p.y);
}

vec2 hash22(vec2 p) {
    float n = hash21(p);
    return vec2(n, hash21(p + vec2(n + 19.19)));
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);

    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float ridge(float h) {
    return 1.0 - abs(h * 2.0 - 1.0);
}

float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    float f = 1.0;

    for (int i = 0; i < 6; i++) {
        v += a * noise(p * f);
        f *= 2.0;
        a *= detail;
    }
    return v;
}

float ridgedFbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    float f = 1.0;
    float sharp = mix(1.2, 3.8, clamp(crystal * 0.5, 0.0, 1.0));

    for (int i = 0; i < 5; i++) {
        float n = ridge(noise(p * f));
        n = pow(n, sharp);
        v += a * n;
        f *= 2.1;
        a *= max(detail * 0.92, 0.001);
    }
    return v;
}

float voronoiSpark(vec2 p) {
    vec2 g = floor(p);
    vec2 f = fract(p);

    float md = 10.0;
    float id = 0.0;

    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            vec2 o = vec2(float(x), float(y));
            vec2 h = hash22(g + o);
            vec2 r = o + h - f;
            float d = dot(r, r);
            if (d < md) {
                md = d;
                id = hash21(g + o + vec2(17.0));
            }
        }
    }

    float core = exp(-18.0 * md);
    core *= smoothstep(0.72, 1.0, id);
    return core;
}

vec3 resinPalette(float t) {
    t = clamp(t + paletteShift * 0.25, 0.0, 1.0);

    vec3 c = mix(shadowColor.rgb, amberColor.rgb, smoothstep(0.00, 0.40, t));
    c = mix(c, hotColor.rgb, smoothstep(0.28, 0.95, t));

    return c;
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    vec2 p = uv - center;
    p.x *= RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    float t = TIME * speed;

    mat2 r = rot2(flowAngle);
    vec2 pr = r * p;

    vec2 flow = vec2(cos(flowAngle), sin(flowAngle));
    vec2 flowPerp = vec2(-flow.y, flow.x);

    vec2 driftA = vec2(
        fbm(pr * scale * 0.85 + vec2(1.7, -2.1) + flow * t * 0.55),
        fbm(pr * scale * 0.85 + vec2(-3.2, 0.8) - flowPerp * t * 0.40)
    );

    vec2 driftB = vec2(
        fbm(pr * scale * 2.2 - vec2(4.3, 1.1) - flow * t * 0.85),
        fbm(pr * scale * 2.2 + vec2(0.4, 3.7) + flowPerp * t * 0.65)
    );

    vec2 q = pr * scale;
    q += flow * t * (0.65 + flowStrength * 1.5);
    q += (driftA - 0.5) * distort * 2.0;
    q += (driftB - 0.5) * microDistort * 1.1;

    float bodyA = fbm(q + vec2(0.0, t * 0.45));
    float bodyB = fbm(q * (1.55 + depth * 0.45) - vec2(t * 0.28, 0.0));
    float bodyC = noise(q * 3.2 + vec2(bodyA * 2.0, bodyB * 2.0));
    float cavities = 1.0 - abs(2.0 * bodyA - 1.0);
    float strata = ridgedFbm(q * (1.4 + depth * 1.1) + vec2(8.0, -3.0) + vec2(t * 0.15, -t * 0.08));

    float amber = bodyA * 0.42 + bodyB * 0.23 + bodyC * 0.10 + cavities * 0.13 + strata * 0.12;
    amber += (strata - 0.5) * 0.28 * depth;

    vec2 lightVec = normalize(vec2(cos(flowAngle + 1.1), sin(flowAngle + 1.1)));
    float lightSweep = dot(normalize(pr + vec2(0.001)), lightVec);
    amber += lightSweep * 0.06 * depth;

    amber = clamp(amber, 0.0, 1.0);
    amber = pow(amber, 1.0 / max(contrast, 0.001));

    float veins = ridgedFbm(q * (3.2 + crystal * 2.4) + vec2(2.0, -1.0));
    veins = smoothstep(0.40, 0.98, veins);
    veins *= crystal;

    float facets = 1.0 - smoothstep(0.04, 0.22, abs(noise(q * (5.5 + crystal * 3.0)) - 0.5));
    facets *= 0.18 * crystal;

    float sp1 = voronoiSpark(q * (2.8 + crystal * 0.8) + vec2(t * 0.70, -t * 0.40));
    float sp2 = voronoiSpark(q * (5.5 + crystal * 1.1) - vec2(t * 0.35,  t * 0.55));
    float sp3 = voronoiSpark(q * (9.0 + crystal * 1.4) + vec2(-t * 0.90, t * 0.60));

    float sparkMix = sp1 * 0.85 + sp2 * 0.60 + sp3 * 0.35;
    sparkMix *= sparkle;

    float twinkle = 0.68 + 0.32 * sin(TIME * (4.0 + sparkle * 1.5) + q.x * 2.8 + q.y * 6.7);
    sparkMix *= twinkle;

    float radial = length(p) * 1.3;
    float halo = exp(-radial * radial * 4.0) * centerGlow;

    float ringMask = 1.0 - smoothstep(
        0.0,
        0.18,
        abs(radial - (0.24 + 0.05 * noise(pr * scale * 0.35 + vec2(t * 0.10, -t * 0.07))))
    );
    float ring = ringMask * ringAmount;

    float highlight = smoothstep(0.56, 1.0, amber) * glow;
    highlight += veins * (0.45 + glow * 0.35);
    highlight += facets * 0.75;
    highlight += sparkMix * (1.0 + glow);
    highlight += halo * (0.6 + glow * 0.4);
    highlight += ring * (0.4 + glow * 0.3);

    vec3 col = resinPalette(amber + veins * 0.08);
    col += hotColor.rgb * veins * 0.18;
    col += mix(hotColor.rgb, sparkColor.rgb, 0.55) * facets * 0.25;
    col += sparkColor.rgb * sparkMix;
    col += mix(amberColor.rgb, hotColor.rgb, 0.65) * highlight * 0.22;
    col += hotColor.rgb * halo * 0.15;
    col += hotColor.rgb * ring * 0.12;

    float g = (noise(q * 18.0 + vec2(13.7, 7.9) + vec2(TIME * 0.15, -TIME * 0.11)) - 0.5) * grain;
    col += g;

    float vign = 1.0 - vignette * smoothstep(0.25, 1.15, radial);
    col *= clamp(vign, 0.0, 1.0);

    col = clamp(col, 0.0, 1.0);
    col = pow(col, vec3(1.0 / max(gamma, 0.001)));

    float alphaMask = smoothstep(0.06, 1.0, amber + veins * 0.15 + halo * 0.20 + sparkMix * 0.08);
    float a = mix(alphaFloor, 1.0, alphaMask) * alphaAmount;
    a = clamp(a, 0.0, 1.0);

    gl_FragColor = vec4(col, a);
}
