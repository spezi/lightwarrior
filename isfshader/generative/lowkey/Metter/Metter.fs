/*{
  "DESCRIPTION": "Monochrome spectral glitch skyline inspired by vertical digital noise, scanlines, waveform spikes, and fragmented blocks.",
  "CREDIT": "Generated with ChatGPT - ISF Shader Designer",
  "CATEGORIES": [
    "Generator",
    "Glitch",
    "Abstract"
  ],
  "INPUTS": [
    {
      "NAME": "intensity",
      "TYPE": "float",
      "DEFAULT": 0.85,
      "MIN": 0.0,
      "MAX": 2.0,
      "LABEL": "Overall Intensity"
    },
    {
      "NAME": "density",
      "TYPE": "float",
      "DEFAULT": 0.72,
      "MIN": 0.05,
      "MAX": 1.5,
      "LABEL": "Line Density"
    },
    {
      "NAME": "spikeHeight",
      "TYPE": "float",
      "DEFAULT": 0.62,
      "MIN": 0.0,
      "MAX": 1.4,
      "LABEL": "Spike Height"
    },
    {
      "NAME": "bandWidth",
      "TYPE": "float",
      "DEFAULT": 0.22,
      "MIN": 0.02,
      "MAX": 0.6,
      "LABEL": "Central Band Width"
    },
    {
      "NAME": "blockAmount",
      "TYPE": "float",
      "DEFAULT": 0.52,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Glitch Blocks"
    },
    {
      "NAME": "scanlineAmount",
      "TYPE": "float",
      "DEFAULT": 0.45,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Scanlines"
    },
    {
      "NAME": "fineNoise",
      "TYPE": "float",
      "DEFAULT": 0.36,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Fine Noise"
    },
    {
      "NAME": "symmetry",
      "TYPE": "float",
      "DEFAULT": 0.72,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Vertical Reflection"
    },
    {
      "NAME": "drift",
      "TYPE": "float",
      "DEFAULT": 0.18,
      "MIN": -2.0,
      "MAX": 2.0,
      "LABEL": "Horizontal Drift"
    },
    {
      "NAME": "flicker",
      "TYPE": "float",
      "DEFAULT": 0.28,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Flicker"
    },
    {
      "NAME": "contrast",
      "TYPE": "float",
      "DEFAULT": 1.35,
      "MIN": 0.2,
      "MAX": 4.0,
      "LABEL": "Contrast"
    },
    {
      "NAME": "gamma",
      "TYPE": "float",
      "DEFAULT": 0.78,
      "MIN": 0.2,
      "MAX": 2.5,
      "LABEL": "Gamma"
    },
    {
      "NAME": "lineSharpness",
      "TYPE": "float",
      "DEFAULT": 0.72,
      "MIN": 0.05,
      "MAX": 2.0,
      "LABEL": "Line Sharpness"
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": false,
      "LABEL": "Invert"
    },
    {
      "NAME": "tint",
      "TYPE": "color",
      "DEFAULT": [
        0.82,
        0.82,
        0.78,
        1.0
      ],
      "LABEL": "Tint"
    }
  ]
}*/

#ifdef GL_ES
precision mediump float;
#endif

// ------------------------------------------------------------
// Hash / noise utilities
// ------------------------------------------------------------

float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float noise2(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);

    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;

    for (int i = 0; i < 5; i++) {
        v += a * noise2(p);
        p *= 2.03;
        a *= 0.5;
    }

    return v;
}

float rectMask(vec2 uv, vec2 center, vec2 size, float softness) {
    vec2 d = abs(uv - center) - size;
    float outside = length(max(d, 0.0));
    float inside = min(max(d.x, d.y), 0.0);
    return 1.0 - smoothstep(0.0, softness, outside + inside);
}

float thinLine(float x, float pos, float width) {
    return 1.0 - smoothstep(0.0, width, abs(x - pos));
}

// ------------------------------------------------------------
// Main visual components
// ------------------------------------------------------------

float verticalSpikeField(vec2 uv, float t) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Discrete columns create the barcode / spectral-line structure.
    float cols = mix(80.0, 420.0, clamp(density, 0.0, 1.5) / 1.5);
    float cx = floor((uv.x + t * drift * 0.015) * cols);
    float localX = fract((uv.x + t * drift * 0.015) * cols);

    float rnd = hash11(cx * 17.13);
    float rnd2 = hash11(cx * 71.41 + 19.0);
    float rnd3 = hash11(cx * 131.7);

    // Very thin column core.
    float lineCore = 1.0 - smoothstep(0.0, 0.055 / max(lineSharpness, 0.05), abs(localX - 0.5));

    // Most columns are faint; a few become tall bright spikes.
    float columnPresence = step(0.47 + (1.0 - density) * 0.22, rnd);
    float brightSpike = step(0.88 - density * 0.18, rnd2);

    float center = 0.5 + 0.055 * sin(cx * 0.073 + t * 0.7);
    float distFromCenter = abs(uv.y - center);

    float baseHeight = mix(0.08, 0.55, rnd);
    baseHeight += brightSpike * mix(0.18, 0.58, rnd3);
    baseHeight *= spikeHeight;

    // Taper vertical visibility around the center axis.
    float verticalReach = 1.0 - smoothstep(baseHeight, baseHeight + 0.18, distFromCenter);

    // Extra thin lines extending above and below.
    float longNeedle = brightSpike * (1.0 - smoothstep(baseHeight * 1.1, baseHeight * 1.1 + 0.32, distFromCenter));

    float shimmer = 0.65 + 0.35 * sin(t * 7.0 + cx * 0.33);
    float field = lineCore * columnPresence * verticalReach * shimmer;
    field += lineCore * longNeedle * 0.75;

    // Sparse black interruptions in columns.
    float dropout = step(0.18, noise2(vec2(cx * 0.12, floor(uv.y * 130.0) + t * 4.0)));
    field *= dropout;

    return field;
}

float centralNoiseBand(vec2 uv, float t) {
    float y = abs(uv.y - 0.5);
    float band = 1.0 - smoothstep(bandWidth, bandWidth + 0.18, y);

    vec2 p = uv;
    p.x += 0.04 * sin(uv.y * 30.0 + t * 0.4);
    p.y += t * 0.025;

    float coarse = fbm(vec2(p.x * 18.0, p.y * 7.0 + t * 0.15));
    float mid = fbm(vec2(p.x * 80.0, p.y * 24.0 - t * 0.35));
    float fine = hash21(floor(vec2(uv.x * 900.0, uv.y * 260.0 + t * 20.0)));

    float texture = coarse * 0.55 + mid * 0.35 + fine * 0.18 * fineNoise;

    // Horizontal smearing, like damaged video data.
    float rows = floor(uv.y * 180.0);
    float rowNoise = hash11(rows + floor(t * 8.0) * 19.0);
    float rowGate = smoothstep(0.15, 0.95, rowNoise);

    return band * texture * rowGate;
}

float glitchBlocks(vec2 uv, float t) {
    float sum = 0.0;

    // Layer 1: medium rectangular digital fragments.
    vec2 gridA = vec2(42.0, 22.0);
    vec2 cellA = floor(vec2(uv.x * gridA.x, uv.y * gridA.y));
    float rA = hash21(cellA + floor(t * 2.0));
    float gateA = step(1.0 - blockAmount * 0.35, rA);

    vec2 cellUV = fract(vec2(uv.x * gridA.x, uv.y * gridA.y));
    float edge = smoothstep(0.02, 0.09, cellUV.x) *
                 smoothstep(0.02, 0.09, cellUV.y) *
                 smoothstep(0.02, 0.09, 1.0 - cellUV.x) *
                 smoothstep(0.02, 0.09, 1.0 - cellUV.y);

    float centerBand = 1.0 - smoothstep(0.31, 0.58, abs(uv.y - 0.5));
    sum += gateA * edge * centerBand * 0.65;

    // Layer 2: tiny broken pixels clustered around the central mass.
    vec2 gridB = vec2(180.0, 90.0);
    vec2 cellB = floor(vec2(uv.x * gridB.x, uv.y * gridB.y));
    float rB = hash21(cellB + vec2(floor(t * 5.0), 7.0));
    float gateB = step(1.0 - blockAmount * 0.52, rB);
    float tiny = gateB * centerBand * hash21(cellB * 3.17);

    sum += tiny * 0.45;

    // Layer 3: occasional horizontal broken slabs.
    float row = floor(uv.y * 70.0);
    float rowRnd = hash11(row + floor(t * 3.0) * 13.0);
    float slab = step(1.0 - blockAmount * 0.22, rowRnd);
    float slabNoise = smoothstep(0.35, 0.92, fbm(vec2(uv.x * 22.0, row * 0.21 + t)));
    sum += slab * slabNoise * centerBand * 0.38;

    return sum;
}

float scanlines(vec2 uv) {
    float lines = sin(uv.y * RENDERSIZE.y * 3.14159);
    lines = 0.5 + 0.5 * lines;
    float fine = pow(lines, 2.5);

    float larger = sin(uv.y * 420.0);
    larger = 0.5 + 0.5 * larger;

    return mix(1.0, fine * 0.65 + larger * 0.35, scanlineAmount);
}

float horizonLines(vec2 uv, float t) {
    float h = 0.0;

    h += thinLine(uv.y, 0.500, 0.0018) * 0.75;
    h += thinLine(uv.y, 0.515 + 0.004 * sin(t * 0.5), 0.0012) * 0.32;
    h += thinLine(uv.y, 0.480 + 0.003 * sin(t * 0.37), 0.0010) * 0.26;
    h += thinLine(uv.y, 0.548, 0.0010) * 0.18;
    h += thinLine(uv.y, 0.452, 0.0010) * 0.18;

    return h;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float t = TIME;

    // Slight coordinate warping for analog instability.
    vec2 warped = uv;
    warped.x += 0.004 * sin(uv.y * 70.0 + t * 1.1) * blockAmount;
    warped.x += 0.002 * sin(uv.y * 230.0 - t * 2.0) * fineNoise;

    // Blend original and vertically mirrored structures.
    vec2 mirrorUV = vec2(warped.x, 1.0 - warped.y);
    float spikesA = verticalSpikeField(warped, t);
    float spikesB = verticalSpikeField(mirrorUV, t + 11.7);
    float spikes = mix(spikesA, max(spikesA, spikesB), symmetry);

    float band = centralNoiseBand(warped, t);
    float blocks = glitchBlocks(warped, t);
    float horizons = horizonLines(warped, t);

    // Sparse ultra-bright needle lines.
    float needleCols = 540.0;
    float nCol = floor(warped.x * needleCols);
    float nRnd = hash11(nCol * 9.17 + floor(t * 1.5));
    float nLocal = fract(warped.x * needleCols);
    float needle = step(0.985 - density * 0.015, nRnd);
    needle *= 1.0 - smoothstep(0.0, 0.018 / max(lineSharpness, 0.05), abs(nLocal - 0.5));
    needle *= 1.0 - smoothstep(0.38 * spikeHeight, 0.78 * spikeHeight + 0.001, abs(warped.y - 0.5));

    float value = 0.0;
    value += band * 0.85;
    value += blocks * 0.75;
    value += spikes * 1.15;
    value += needle * 1.4;
    value += horizons;

    // Fine film/digital grain.
    float grain = hash21(gl_FragCoord.xy + vec2(t * 37.0, -t * 19.0));
    value += (grain - 0.5) * fineNoise * 0.22;

    // Temporal flicker.
    float flick = mix(1.0, 0.78 + 0.22 * hash11(floor(t * 18.0)), flicker);
    value *= flick;

    // Scanline modulation.
    value *= scanlines(warped);

    // Contrast / gamma shaping.
    value *= intensity;
    value = pow(max(value, 0.0), gamma);
    value = (value - 0.5) * contrast + 0.5;
    value = clamp(value, 0.0, 1.0);

    // Push most of the frame toward black, preserving sharp white structures.
    value = smoothstep(0.08, 1.0, value);

    vec3 col = value * tint.rgb;

    if (invert) {
        col = 1.0 - col;
    }

    gl_FragColor = vec4(col, 1.0);
}