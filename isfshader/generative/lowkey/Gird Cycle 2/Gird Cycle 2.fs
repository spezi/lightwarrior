/*{
  "DESCRIPTION": "Audio-reactive pseudo Voronoi circular lens / flute interaction visual. Black background, thin white organic Voronoi lines, central refractive ring. Designed for MadMapper ISF.",
  "CREDIT": "Generated for Kim Byungki / SaeOnSori performance visual workflow",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "Audio Reactive", "Minimal"],
  "INPUTS": [
    { "NAME": "AudioLevel", "TYPE": "float", "DEFAULT": 0.20, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "AudioLow",   "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "AudioMid",   "TYPE": "float", "DEFAULT": 0.20, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "AudioHigh",  "TYPE": "float", "DEFAULT": 0.30, "MIN": 0.0, "MAX": 1.0 },

    { "NAME": "Speed", "TYPE": "float", "DEFAULT": 0.20, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "CellScale", "TYPE": "float", "DEFAULT": 7.0, "MIN": 2.0, "MAX": 18.0 },
    { "NAME": "LineWidth", "TYPE": "float", "DEFAULT": 0.030, "MIN": 0.005, "MAX": 0.090 },
    { "NAME": "LineSoftness", "TYPE": "float", "DEFAULT": 0.030, "MIN": 0.001, "MAX": 0.120 },

    { "NAME": "RingRadius", "TYPE": "float", "DEFAULT": 0.42, "MIN": 0.10, "MAX": 0.80 },
    { "NAME": "RingWidth", "TYPE": "float", "DEFAULT": 0.018, "MIN": 0.003, "MAX": 0.070 },
    { "NAME": "RingWarp", "TYPE": "float", "DEFAULT": 0.14, "MIN": 0.0, "MAX": 0.50 },
    { "NAME": "RingDetail", "TYPE": "float", "DEFAULT": 22.0, "MIN": 4.0, "MAX": 60.0 },

    { "NAME": "CenterCircleVisible", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

    { "NAME": "Brightness", "TYPE": "float", "DEFAULT": 1.20, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "Contrast", "TYPE": "float", "DEFAULT": 1.40, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "Glow", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "NoiseAmount", "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.0, "MAX": 0.6 },

    { "NAME": "Invert", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "Seed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 100.0 }
  ]
}*/

float hash11(float n)
{
    return fract(sin(n) * 43758.5453123);
}

vec2 hash22(vec2 p)
{
    p = vec2(dot(p, vec2(127.1, 311.7)),
             dot(p, vec2(269.5, 183.3)));
    return fract(sin(p + Seed) * 43758.5453);
}

float noise(vec2 p)
{
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);

    float a = hash11(dot(i, vec2(1.0, 57.0)) + Seed);
    float b = hash11(dot(i + vec2(1.0, 0.0), vec2(1.0, 57.0)) + Seed);
    float c = hash11(dot(i + vec2(0.0, 1.0), vec2(1.0, 57.0)) + Seed);
    float d = hash11(dot(i + vec2(1.0, 1.0), vec2(1.0, 57.0)) + Seed);

    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

mat2 rot(float a)
{
    float c = cos(a);
    float s = sin(a);
    return mat2(c, -s, s, c);
}

float pseudoVoronoiLines(vec2 p, float scale, float t, float audio)
{
    vec2 g = p * scale;
    vec2 id = floor(g);
    vec2 f = fract(g);

    float d1 = 10.0;
    float d2 = 10.0;

    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            vec2 o = vec2(float(x), float(y));
            vec2 r = hash22(id + o);

            r = 0.5 + 0.42 * sin(t * (0.35 + audio) + 6.2831 * r + vec2(0.0, 1.7));

            vec2 diff = o + r - f;
            float d = dot(diff, diff);

            if (d < d1)
            {
                d2 = d1;
                d1 = d;
            }
            else if (d < d2)
            {
                d2 = d;
            }
        }
    }

    float edge = abs(sqrt(d2) - sqrt(d1));
    float organic = noise(p * 12.0 + vec2(t * 0.17, -t * 0.11));

    float w = LineWidth * (0.65 + 1.35 * audio) * (0.65 + organic);
    float s = LineSoftness;

    float line = 1.0 - smoothstep(w, w + s, edge);

    float grain = noise(p * 90.0 + t * 0.7);
    line *= smoothstep(0.10, 0.75, grain + 0.35 + audio * 0.25);

    return line;
}

float ringLine(float r, float target, float width, float softness)
{
    float d = abs(r - target);
    return 1.0 - smoothstep(width, width + softness, d);
}

float angularDash(float a, float count, float t, float audio)
{
    float v = sin(a * count + t * (0.6 + audio * 2.0));
    v += 0.35 * sin(a * count * 0.47 - t * 1.1);
    return smoothstep(0.15, 0.75, v);
}

void main()
{
    vec2 uv = isf_FragNormCoord.xy;
    vec2 p = uv - 0.5;

    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    p.x *= aspect;

    float t = TIME * Speed;

    float audio = clamp(AudioLevel, 0.0, 1.0);
    float low   = clamp(AudioLow,   0.0, 1.0);
    float mid   = clamp(AudioMid,   0.0, 1.0);
    float high  = clamp(AudioHigh,  0.0, 1.0);

    float centerVis = clamp(CenterCircleVisible, 0.0, 1.0);

    float r = length(p);
    float a = atan(p.y, p.x);

    float ringRadius = RingRadius * (0.92 + 0.13 * low);
    float ringMask = ringLine(r, ringRadius, RingWidth * (1.0 + mid), 0.045) * centerVis;
    float innerMask = (1.0 - smoothstep(ringRadius * 0.86, ringRadius * 1.12, r)) * centerVis;

    float wave = sin(a * RingDetail + t * 2.0 + high * 5.0);
    wave += 0.45 * sin(a * (RingDetail * 0.53) - t * 1.4);

    float radialWarp = RingWarp * (0.15 + audio) * ringMask * wave;

    vec2 dir = normalize(p + 0.0001);
    vec2 warped = p + dir * radialWarp;

    warped = rot(0.025 * sin(t * 0.7) + high * 0.025) * warped;

    float scale = CellScale * (1.0 + 0.22 * mid);

    float bgLines = pseudoVoronoiLines(warped, scale, t, audio);

    float largeLines = pseudoVoronoiLines(
        p + 0.03 * vec2(sin(t), cos(t * 0.8)),
        scale * 0.46,
        t * 0.55,
        audio * 0.7
    );

    bgLines = max(bgLines * 0.88, largeLines * 0.65);

    float rings = 0.0;
    rings += ringLine(r + 0.010 * wave * (0.3 + audio), ringRadius, RingWidth * 1.5, 0.016);
    rings += ringLine(r - 0.006 * wave, ringRadius * 0.82, RingWidth * 0.55, 0.010);
    rings += ringLine(r + 0.004 * sin(a * 13.0 - t), ringRadius * 1.08, RingWidth * 0.45, 0.010);
    rings += ringLine(r, ringRadius * 0.58, RingWidth * 0.35, 0.014) * 0.55;
    rings *= centerVis;

    float dash = angularDash(a, RingDetail * 1.45, t, high);

    float rimHighlights = ringLine(
        r + 0.018 * sin(a * 8.0 + t),
        ringRadius * 1.02,
        RingWidth * 2.2,
        0.035
    );

    rimHighlights *= dash * (0.4 + high * 1.8) * centerVis;

    float innerTexture = noise(warped * 20.0 + vec2(t * 0.15, -t * 0.12));
    innerTexture = smoothstep(0.58, 0.92, innerTexture) * innerMask * 0.18;

    float col = 0.0;
    col += bgLines;
    col += rings * (0.85 + audio * 1.1);
    col += rimHighlights;
    col += innerTexture;

    float grain = noise(uv * RENDERSIZE.xy * 0.35 + t * 13.0);
    col += grain * NoiseAmount * (0.20 + high);

    float glowLines = pseudoVoronoiLines(warped + vec2(0.002, -0.001), scale, t + 0.5, audio);
    col += glowLines * Glow * 0.35;
    col += rings * Glow * 0.20;

    col *= Brightness;
    col = pow(max(col, 0.0), 1.0 / max(Contrast, 0.001));
    col = clamp(col, 0.0, 1.0);

    vec3 color = vec3(col);

    if (Invert)
    {
        color = 1.0 - color;
    }

    gl_FragColor = vec4(color, 1.0);
}