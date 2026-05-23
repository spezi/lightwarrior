/*{
    "DESCRIPTION": "Minimal paper drawing of intersecting vertical and horizontal lines with animated motion, paper texture, and controllable composition.",
    "CREDIT": "OpenAI - ISF Shader Designer",
    "ISFVSN": "2",
    "CATEGORIES": [ "Generator", "Abstract", "Minimal" ],
    "INPUTS": [
        { "NAME": "backgroundTone",   "TYPE": "float", "DEFAULT": 0.94, "MIN": 0.82, "MAX": 1.0 },
        { "NAME": "textureAmount",    "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "paperScale",       "TYPE": "float", "DEFAULT": 2.2,  "MIN": 0.5,  "MAX": 8.0 },

        { "NAME": "inkIntensity",     "TYPE": "float", "DEFAULT": 0.95, "MIN": 0.2,  "MAX": 1.2 },
        { "NAME": "fineThickness",    "TYPE": "float", "DEFAULT": 1.4,  "MIN": 0.4,  "MAX": 6.0 },
        { "NAME": "boldThickness",    "TYPE": "float", "DEFAULT": 4.8,  "MIN": 1.0,  "MAX": 14.0 },
        { "NAME": "thinOpacity",      "TYPE": "float", "DEFAULT": 0.75, "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "boldOpacity",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 1.2 },
        { "NAME": "edgeSoftness",     "TYPE": "float", "DEFAULT": 0.9,  "MIN": 0.1,  "MAX": 4.0 },

        { "NAME": "globalSpeed",      "TYPE": "float", "DEFAULT": 0.28, "MIN": 0.0,  "MAX": 3.0 },
        { "NAME": "horizontalMotion", "TYPE": "float", "DEFAULT": 8.0,  "MIN": 0.0,  "MAX": 40.0 },
        { "NAME": "verticalMotion",   "TYPE": "float", "DEFAULT": 8.0,  "MIN": 0.0,  "MAX": 40.0 },
        { "NAME": "clusterSpread",    "TYPE": "float", "DEFAULT": 18.0, "MIN": 0.0,  "MAX": 80.0 },
        { "NAME": "clusterBreath",    "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 2.0 },

        { "NAME": "shiftX",           "TYPE": "float", "DEFAULT": 0.0,  "MIN": -0.25, "MAX": 0.25 },
        { "NAME": "shiftY",           "TYPE": "float", "DEFAULT": 0.0,  "MIN": -0.25, "MAX": 0.25 }
    ]
}*/

#ifdef GL_ES
precision mediump float;
#endif

float hash(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 34.45);
    return fract(p.x * p.y);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);

    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += noise(p) * a;
        p *= 2.0;
        a *= 0.5;
    }
    return v;
}

float rangeMask(float v, float a, float b, float blur) {
    return smoothstep(a - blur, a, v) * (1.0 - smoothstep(b, b + blur, v));
}

float hLine(vec2 uv, float y, float thickness, float blur, float x0, float x1) {
    float core = 1.0 - smoothstep(thickness, thickness + blur, abs(uv.y - y));
    float mask = rangeMask(uv.x, x0, x1, blur * 8.0);
    return core * mask;
}

float vLine(vec2 uv, float x, float thickness, float blur, float y0, float y1) {
    float core = 1.0 - smoothstep(thickness, thickness + blur, abs(uv.x - x));
    float mask = rangeMask(uv.y, y0, y1, blur * 8.0);
    return core * mask;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float minRes = min(RENDERSIZE.x, RENDERSIZE.y);

    float thinT = fineThickness / minRes;
    float boldT = boldThickness / minRes;
    float blur  = edgeSoftness / minRes;

    float t = TIME * globalSpeed;

    // Motion amplitudes in normalized space
    float hAmp = horizontalMotion / RENDERSIZE.y;
    float vAmp = verticalMotion / RENDERSIZE.x;
    float spread = clusterSpread / RENDERSIZE.x;
    float breath = 1.0 + sin(t * 0.33) * 0.15 * clusterBreath;

    // Paper background
    vec3 col = vec3(backgroundTone, backgroundTone - 0.01, backgroundTone - 0.03);

    vec2 texUV = uv;
    texUV.x *= RENDERSIZE.x / RENDERSIZE.y;

    float paper1 = fbm(texUV * paperScale * 3.0);
    float paper2 = noise(texUV * paperScale * 22.0);
    float paperGrain = mix(paper1, paper2, 0.35);

    col += (paperGrain - 0.5) * 0.10 * textureAmount;

    // subtle paper fibers
    float fibers = noise(vec2(texUV.x * 120.0, texUV.y * 18.0));
    col += (fibers - 0.5) * 0.03 * textureAmount;

    float thinAcc = 0.0;
    float boldAcc = 0.0;

    // Composition anchors
    float cx = 0.58 + shiftX;
    float cy = 0.47 + shiftY;

    // ------------------------
    // Horizontal lines
    // ------------------------
    thinAcc += hLine(uv, 0.390 + shiftY + sin(t * 0.71 + 0.2) * hAmp, thinT, blur, 0.0, 0.86);
    thinAcc += hLine(uv, 0.430 + shiftY + sin(t * 0.63 + 0.7) * hAmp, thinT, blur, 0.0, 0.86);
    thinAcc += hLine(uv, 0.445 + shiftY + sin(t * 0.56 + 1.1) * hAmp, thinT, blur, 0.0, 0.86);
    thinAcc += hLine(uv, 0.460 + shiftY + sin(t * 0.60 + 1.8) * hAmp, thinT, blur, 0.0, 0.86);
    thinAcc += hLine(uv, 0.475 + shiftY + sin(t * 0.52 + 2.4) * hAmp, thinT, blur, 0.0, 0.86);

    boldAcc += hLine(uv, 0.475 + shiftY + sin(t * 0.42 + 1.7) * hAmp, boldT, blur, 0.0, 0.86);

    thinAcc += hLine(uv, 0.620 + shiftY + sin(t * 0.67 + 0.3) * hAmp, thinT, blur, 0.0, 1.00);
    thinAcc += hLine(uv, 0.635 + shiftY + sin(t * 0.58 + 1.5) * hAmp, thinT, blur, 0.0, 0.88);
    thinAcc += hLine(uv, 0.648 + shiftY + sin(t * 0.49 + 2.1) * hAmp, thinT, blur, 0.0, 0.88);

    thinAcc += hLine(uv, 0.565 + shiftY + sin(t * 0.40 + 1.2) * hAmp, thinT, blur, 0.58, 1.0);
    boldAcc += hLine(uv, 0.590 + shiftY + sin(t * 0.47 + 2.6) * hAmp, boldT, blur, 0.45, 1.0);

    // ------------------------
    // Vertical lines
    // ------------------------
    float mainSpread = spread * breath;

    // Main central cluster
    boldAcc += vLine(uv, cx - mainSpread * 0.70 + sin(t * 0.54 + 0.0) * vAmp, boldT, blur, 0.0, 1.0);
    thinAcc += vLine(uv, cx - mainSpread * 0.18 + sin(t * 0.62 + 0.5) * vAmp, thinT, blur, 0.0, 1.0);
    boldAcc += vLine(uv, cx + mainSpread * 0.15 + sin(t * 0.48 + 1.0) * vAmp, boldT, blur, 0.0, 1.0);
    thinAcc += vLine(uv, cx + mainSpread * 0.55 + sin(t * 0.57 + 1.5) * vAmp, thinT, blur, 0.0, 1.0);

    // Secondary right cluster
    thinAcc += vLine(uv, cx + 0.045 + mainSpread * 0.9 + sin(t * 0.66 + 0.1) * vAmp, thinT, blur, 0.18, 0.98);
    thinAcc += vLine(uv, cx + 0.052 + mainSpread * 1.0 + sin(t * 0.61 + 0.9) * vAmp, thinT, blur, 0.18, 0.98);
    thinAcc += vLine(uv, cx + 0.059 + mainSpread * 1.1 + sin(t * 0.59 + 1.7) * vAmp, thinT, blur, 0.18, 0.98);
    thinAcc += vLine(uv, cx + 0.066 + mainSpread * 1.2 + sin(t * 0.64 + 2.6) * vAmp, thinT, blur, 0.18, 0.98);

    // Lone long verticals
    thinAcc += vLine(uv, 0.44 + shiftX + sin(t * 0.35 + 0.4) * vAmp, thinT, blur, 0.22, 0.92);
    thinAcc += vLine(uv, 0.70 + shiftX + sin(t * 0.39 + 1.4) * vAmp, thinT, blur, 0.10, 0.77);

    // Small extra central support lines
    thinAcc += vLine(uv, cx - 0.010 + sin(t * 0.45 + 2.2) * vAmp, thinT, blur, 0.30, 0.74);
    thinAcc += vLine(uv, cx + 0.030 + sin(t * 0.50 + 2.8) * vAmp, thinT, blur, 0.25, 0.68);

    // Ink accumulation
    thinAcc = clamp(thinAcc, 0.0, 1.0);
    boldAcc = clamp(boldAcc, 0.0, 1.0);

    float inkMixThin = thinAcc * thinOpacity;
    float inkMixBold = boldAcc * boldOpacity;

    vec3 thinInk = vec3(0.10, 0.10, 0.10) * inkIntensity;
    vec3 boldInk = vec3(0.02, 0.02, 0.02) * inkIntensity;

    col = mix(col, thinInk, clamp(inkMixThin, 0.0, 1.0));
    col = mix(col, boldInk, clamp(inkMixBold, 0.0, 1.0));

    // Slight vignette / natural paper falloff
    float vignette = distance(uv, vec2(0.5));
    col -= vignette * 0.04;

    col = clamp(col, 0.0, 1.0);
    gl_FragColor = vec4(col, 1.0);
}