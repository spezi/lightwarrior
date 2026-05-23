/*{
  "ISFVSN": "2",
  "DESCRIPTION": "MadMapper-safe audio reactive typographic wall with code-like background text texture.",
  "CATEGORIES": [
    "Generator",
    "Audio Reactive",
    "Typography"
  ],
  "INPUTS": [
    { "NAME": "audioLevel", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioLow", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioMid", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioHigh", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "drift", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "waveAmount", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "rotation", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14159, "MAX": 3.14159 },
    { "NAME": "density", "TYPE": "float", "DEFAULT": 0.72, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "smallScale", "TYPE": "float", "DEFAULT": 26.0, "MIN": 6.0, "MAX": 80.0 },
    { "NAME": "largeScale", "TYPE": "float", "DEFAULT": 7.5, "MIN": 2.0, "MAX": 24.0 },
    { "NAME": "layerMix", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "strokeWidth", "TYPE": "float", "DEFAULT": 0.68, "MIN": 0.2, "MAX": 1.0 },
    { "NAME": "softness", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 0.35 },
    { "NAME": "flicker", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.35, "MIN": 0.2, "MAX": 4.0 },
    { "NAME": "meshAmount", "TYPE": "float", "DEFAULT": 0.28, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "scanlineAmount", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "backgroundTextAmount", "TYPE": "float", "DEFAULT": 0.34, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "backgroundTextScale", "TYPE": "float", "DEFAULT": 13.0, "MIN": 5.0, "MAX": 32.0 },
    { "NAME": "backgroundTextSpeed", "TYPE": "float", "DEFAULT": 0.08, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "vignette", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "foregroundColor", "TYPE": "color", "DEFAULT": [0.92, 0.96, 1.0, 1.0] },
    { "NAME": "accentColor", "TYPE": "color", "DEFAULT": [0.36, 0.62, 1.0, 1.0] },
    { "NAME": "backgroundColor", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "invertAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 }
  ]
}
*/

float hash12(vec2 p)
{
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 = p3 + dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec2 rot(vec2 p, float a)
{
    float s = sin(a);
    float c = cos(a);
    return mat2(c, -s, s, c) * p;
}

float rect(vec2 p, vec2 b, float blur)
{
    vec2 d = abs(p - 0.5) - b;
    float o = length(max(d, 0.0));
    float i = min(max(d.x, d.y), 0.0);
    return 1.0 - smoothstep(0.0, blur, o + i);
}

float sameVal(float a, float b)
{
    return 1.0 - step(0.5, abs(a - b));
}

float wordMask(float c, float a, float b)
{
    return step(a, c) * (1.0 - step(b, c));
}

float glyph(vec2 p, float id)
{
    vec2 scaled = p * vec2(5.0, 7.0);
    vec2 g = floor(scaled);
    vec2 f = fract(scaled);
    float x = g.x;
    float y = g.y;
    float l = 1.0 - step(0.5, x);
    float r = step(3.5, x);
    float mx = 1.0 - step(0.5, abs(x - 2.0));
    float t = 1.0 - step(0.5, y);
    float m = 1.0 - step(0.5, abs(y - 3.0));
    float b = step(5.5, y);
    float da = 1.0 - step(0.55, abs(x - y * 0.66));
    float db = 1.0 - step(0.55, abs((4.0 - x) - y * 0.66));
    float fam = floor(fract(id * 13.17) * 7.0);
    float a0 = max(max(l, r), max(t, m));
    float a1 = max(l, max(t, max(m, b)));
    float a2 = max(r, max(t, max(m, b)));
    float a3 = max(max(l, r), max(m, b));
    float a4 = max(max(l, mx), max(t, b));
    float a5 = max(max(da, db), b);
    float a6 = max(max(t, b), max(mx, m));
    float pad = mix(0.23, 0.04, strokeWidth);
    float v = a0;
    v = mix(v, a1, step(0.5, fam));
    v = mix(v, a2, step(1.5, fam));
    v = mix(v, a3, step(2.5, fam));
    v = mix(v, a4, step(3.5, fam));
    v = mix(v, a5, step(4.5, fam));
    v = mix(v, a6, step(5.5, fam));
    return v * rect(f, vec2(0.5 - pad), max(softness, 0.001));
}

float textField(vec2 uv, float sc, float seed, float drive, float op)
{
    float tt = TIME * speed;
    vec2 q = uv * sc;
    vec2 cell = vec2(0.0);
    vec2 f = vec2(0.0);
    float id = 0.0;
    float nudgeX = 0.0;
    float nudgeY = 0.0;
    float showMask = 0.0;
    float shimmerVal = 0.0;

    q.x = q.x + sin(uv.y * 5.5 + tt * 0.73 + seed) * waveAmount * (0.35 + audioMid);
    q.y = q.y + cos(uv.x * 4.0 - tt * 0.51 + seed) * waveAmount * (0.2 + audioLow);
    q = q + drift * vec2(sin(tt * 0.19 + seed), cos(tt * 0.13 + seed * 1.7));
    cell = floor(q);
    f = fract(q);
    id = hash12(cell * 2.31 + seed * 9.7);
    nudgeX = sin(TIME * (0.12 + id * 0.08) + id * 20.0);
    nudgeY = cos(TIME * (0.10 + id * 0.07) + id * 17.0);
    f.x = f.x + nudgeX * 0.06 * drift * (0.35 + drive);
    f.y = f.y + nudgeY * 0.06 * drift * (0.35 + drive);
    showMask = step(1.0 - density, hash12(cell + seed));
    shimmerVal = mix(1.0, hash12(cell + floor(TIME * 8.0)), flicker * (0.25 + audioHigh));
    return glyph(f, id + seed) * showMask * (0.72 + drive * 0.9) * shimmerVal * op;
}

float codeField(vec2 uv, float drive)
{
    vec2 q = uv;
    vec2 cell = vec2(0.0);
    vec2 f = vec2(0.0);
    float row = 0.0;
    float c = 0.0;
    float laneA = 0.0;
    float laneB = 0.0;
    float laneC = 0.0;
    float phraseMask = 0.0;
    float rowNoise = 0.0;
    float missing = 0.0;
    float codeGlyph = 0.0;
    float punctuation = 0.0;
    float marks = 0.0;
    float audioFlicker = 0.0;

    q.x = q.x + TIME * backgroundTextSpeed;
    q.y = q.y + sin(uv.x * 3.0 + TIME * 0.12) * 0.025 * (0.3 + drive);
    q = q * vec2(backgroundTextScale, backgroundTextScale * 0.52);

    cell = floor(q);
    f = fract(q);
    row = mod(cell.y, 8.0);
    c = mod(cell.x + mod(cell.y, 2.0) * 4.0, 24.0);

    laneA = wordMask(c, 0.0, 5.0);
    laneB = wordMask(c, 6.0, 12.0);
    laneC = wordMask(c, 14.0, 21.0);
    phraseMask = min(laneA + laneB + laneC, 1.0);

    rowNoise = hash12(vec2(row, 4.0));
    missing = step(0.16, hash12(vec2(c, row * 9.0)));
    codeGlyph = glyph(f, row * 23.0 + c * 3.1);
    punctuation = rect(f, vec2(0.08), 0.02) * (sameVal(c, 5.0) + sameVal(c, 13.0) + sameVal(c, 22.0));
    marks = max(codeGlyph * phraseMask * missing, punctuation * 0.75);
    audioFlicker = mix(1.0, hash12(cell + floor(TIME * 5.0)), flicker * audioHigh * 0.5);

    return marks * (0.55 + 0.45 * rowNoise) * audioFlicker * backgroundTextAmount;
}

void main()
{
    vec2 uv = isf_FragNormCoord.xy;
    vec2 asp = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    vec2 p = (uv - 0.5) * asp;
    vec2 ruv = p / asp + 0.5;
    float a = clamp(audioLevel, 0.0, 1.0);
    float lo = clamp(audioLow, 0.0, 1.0);
    float mi = clamp(audioMid, 0.0, 1.0);
    float hi = clamp(audioHigh, 0.0, 1.0);
    float small;
    float med;
    float large;
    float code;
    float letters;
    float grid;
    float mesh;
    float scan;
    float vig;
    vec3 fg;
    vec3 col;

    p = rot(p, rotation);
    ruv = p / asp + 0.5;
    small = textField(ruv, smallScale * (1.0 + hi * 0.12), 11.0, max(a, hi), 0.70);
    med = textField(ruv + vec2(0.04, -0.02), smallScale * 0.53, 29.0, max(a, mi), 0.55);
    large = textField(ruv + vec2(-0.02, 0.03), largeScale * (1.0 + lo * 0.10), 71.0, max(a, lo), 1.15);
    code = codeField(ruv, max(a, mi));

    letters = mix(small + med, large, layerMix);
    letters = letters + small * (1.0 - layerMix) * 0.55;
    letters = letters + code * (0.55 + hi * 0.35);
    letters = pow(clamp(letters, 0.0, 1.0), 1.0 / max(contrast, 0.001));

    grid = sin(uv.x * RENDERSIZE.x * 0.55) * sin(uv.y * RENDERSIZE.y * 0.55);
    mesh = mix(1.0, 0.68 + 0.32 * step(0.15, grid), meshAmount);
    scan = mix(1.0, 0.78 + 0.22 * sin(uv.y * RENDERSIZE.y * 3.14159), scanlineAmount);
    vig = smoothstep(1.05, 0.22, length((uv - 0.5) * vec2(asp.x, 1.0)));
    vig = mix(1.0, vig, vignette);

    fg = mix(foregroundColor.rgb, accentColor.rgb, hi * 0.45 + mi * 0.15);
    col = mix(backgroundColor.rgb, fg, letters * mesh * scan * vig);
    col = mix(col, 1.0 - col, invertAmount);
    gl_FragColor = vec4(col, 1.0);
}