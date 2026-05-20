/*{
  "DESCRIPTION": "Blueprint Circuit Signal Flow - procedural circuit diagram inspired by technical schematics.",
  "CREDIT": "Generated for ISF Shader Designer",
  "ISFVSN": "2",
  "CATEGORIES": [
    "Generator",
    "Stylize",
    "Glitch"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "useImage",
      "TYPE": "bool",
      "DEFAULT": false
    },
    {
      "NAME": "imageBlend",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.4,
      "MAX": 3.0
    },
    {
      "NAME": "lineDensity",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.3,
      "MAX": 2.5
    },
    {
      "NAME": "lineWidth",
      "TYPE": "float",
      "DEFAULT": 1.15,
      "MIN": 0.3,
      "MAX": 4.0
    },
    {
      "NAME": "glow",
      "TYPE": "float",
      "DEFAULT": 0.65,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "signalSpeed",
      "TYPE": "float",
      "DEFAULT": 0.7,
      "MIN": -3.0,
      "MAX": 3.0
    },
    {
      "NAME": "signalIntensity",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 3.0
    },
    {
      "NAME": "scanlineAmount",
      "TYPE": "float",
      "DEFAULT": 0.18,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "noiseAmount",
      "TYPE": "float",
      "DEFAULT": 0.08,
      "MIN": 0.0,
      "MAX": 0.5
    },
    {
      "NAME": "blueprintColor",
      "TYPE": "color",
      "DEFAULT": [
        0.04,
        0.22,
        0.48,
        1.0
      ]
    },
    {
      "NAME": "lineColor",
      "TYPE": "color",
      "DEFAULT": [
        0.78,
        0.93,
        1.0,
        1.0
      ]
    },
    {
      "NAME": "signalColor",
      "TYPE": "color",
      "DEFAULT": [
        0.25,
        0.85,
        1.0,
        1.0
      ]
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": false
    }
  ]
}*/

float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a;
    vec2 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

float lineSegment(vec2 p, vec2 a, vec2 b, float w) {
    float d = sdSegment(p, a, b);
    return 1.0 - smoothstep(w, w * 2.2, d);
}

float glowSegment(vec2 p, vec2 a, vec2 b, float w) {
    float d = sdSegment(p, a, b);
    return exp(-d * 75.0 / max(w * 200.0, 0.001));
}

float boxOutline(vec2 p, vec2 c, vec2 s, float w) {
    vec2 q = abs(p - c) - s;
    float outside = length(max(q, 0.0));
    float inside = min(max(q.x, q.y), 0.0);
    float d = outside + inside;
    return 1.0 - smoothstep(w, w * 2.0, abs(d));
}

float filledBox(vec2 p, vec2 c, vec2 s, float edge) {
    vec2 q = abs(p - c) - s;
    float d = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0);
    return 1.0 - smoothstep(0.0, edge, d);
}

float circleOutline(vec2 p, vec2 c, float r, float w) {
    float d = abs(length(p - c) - r);
    return 1.0 - smoothstep(w, w * 2.0, d);
}

float dotNode(vec2 p, vec2 c, float r) {
    return 1.0 - smoothstep(r, r * 1.8, length(p - c));
}

float dashedBox(vec2 p, vec2 c, vec2 s, float w) {
    vec2 q = abs(p - c) - s;
    float d = abs(length(max(q, 0.0)) + min(max(q.x, q.y), 0.0));
    float dash = step(0.5, fract((p.x + p.y) * 32.0));
    return (1.0 - smoothstep(w, w * 2.0, d)) * dash;
}

float tracePulse(vec2 p, vec2 a, vec2 b, float w, float offset) {
    vec2 ba = b - a;
    float len2 = dot(ba, ba);
    vec2 pa = p - a;
    float h = clamp(dot(pa, ba) / len2, 0.0, 1.0);
    float d = length(pa - ba * h);

    float t = fract(TIME * signalSpeed + offset);
    float pulse = exp(-pow((h - t) * 13.0, 2.0));
    return pulse * (1.0 - smoothstep(w * 1.2, w * 4.0, d));
}

void addBusHorizontal(in vec2 p, inout float circuit, inout float pulse, float y, float x0, float x1, int count, float spacing, float w, float seed) {
    for (int i = 0; i < 12; i++) {
        if (i >= count) break;
        float yy = y + (float(i) - float(count - 1) * 0.5) * spacing;
        vec2 a = vec2(x0, yy);
        vec2 b = vec2(x1, yy);
        circuit += lineSegment(p, a, b, w);
        circuit += glowSegment(p, a, b, w) * glow;
        pulse += tracePulse(p, a, b, w, seed + float(i) * 0.07);
    }
}

void addBusVertical(in vec2 p, inout float circuit, inout float pulse, float x, float y0, float y1, int count, float spacing, float w, float seed) {
    for (int i = 0; i < 12; i++) {
        if (i >= count) break;
        float xx = x + (float(i) - float(count - 1) * 0.5) * spacing;
        vec2 a = vec2(xx, y0);
        vec2 b = vec2(xx, y1);
        circuit += lineSegment(p, a, b, w);
        circuit += glowSegment(p, a, b, w) * glow;
        pulse += tracePulse(p, a, b, w, seed + float(i) * 0.06);
    }
}

void addIC(in vec2 p, inout float circuit, inout float pulse, vec2 c, vec2 s, int pins, float w, float seed) {
    circuit += boxOutline(p, c, s, w);
    circuit += filledBox(p, c, s * 0.96, w * 0.4) * 0.15;

    for (int i = 0; i < 14; i++) {
        if (i >= pins) break;

        float fy = -s.y + (float(i) + 0.5) / float(pins) * s.y * 2.0;

        vec2 l0 = c + vec2(-s.x, fy);
        vec2 l1 = c + vec2(-s.x - 0.045, fy);
        vec2 r0 = c + vec2(s.x, fy);
        vec2 r1 = c + vec2(s.x + 0.045, fy);

        circuit += lineSegment(p, l0, l1, w);
        circuit += lineSegment(p, r0, r1, w);

        pulse += tracePulse(p, l1, l0, w, seed + float(i) * 0.04);
        pulse += tracePulse(p, r0, r1, w, seed + float(i) * 0.05);
    }
}

void addResistorBank(in vec2 p, inout float circuit, inout float pulse, vec2 start, int count, float w, float seed) {
    for (int i = 0; i < 9; i++) {
        if (i >= count) break;

        float x = start.x + float(i) * 0.055;
        vec2 a = vec2(x, start.y);
        vec2 b = vec2(x, start.y + 0.23);

        circuit += lineSegment(p, a, b, w);
        circuit += boxOutline(p, vec2(x, start.y + 0.115), vec2(0.015, 0.07), w);
        circuit += lineSegment(p, b, b + vec2(0.0, 0.08), w);
        pulse += tracePulse(p, a, b + vec2(0.0, 0.08), w, seed + float(i) * 0.08);
    }
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 aspect = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

    vec2 p = (uv - 0.5) * aspect;
    p /= zoom;
    p += vec2(0.05 * sin(TIME * 0.07), 0.025 * cos(TIME * 0.05));

    float w = 0.0018 * lineWidth / max(lineDensity, 0.001);

    float vignette = smoothstep(0.85, 0.15, length((uv - 0.5) * vec2(1.2, 1.0)));
    vec3 bg = blueprintColor.rgb;
    bg *= 0.72 + 0.28 * vignette;
    bg += vec3(0.02, 0.05, 0.09) * (1.0 - uv.y);

    float circuit = 0.0;
    float pulse = 0.0;

    addIC(p, circuit, pulse, vec2(-0.55, 0.04), vec2(0.055, 0.22), 12, w, 0.1);
    addIC(p, circuit, pulse, vec2(-0.18, -0.10), vec2(0.075, 0.05), 8, w, 0.3);
    addIC(p, circuit, pulse, vec2(0.02, 0.12), vec2(0.045, 0.07), 10, w, 0.5);
    addIC(p, circuit, pulse, vec2(0.34, -0.25), vec2(0.065, 0.055), 7, w, 0.7);

    addBusHorizontal(p, circuit, pulse, 0.19, -0.77, 0.42, 9, 0.012, w, 0.15);
    addBusHorizontal(p, circuit, pulse, -0.06, -0.72, 0.20, 8, 0.014, w, 0.34);
    addBusHorizontal(p, circuit, pulse, -0.34, -0.76, 0.64, 7, 0.012, w, 0.56);
    addBusVertical(p, circuit, pulse, -0.32, -0.46, 0.42, 10, 0.011, w, 0.25);
    addBusVertical(p, circuit, pulse, -0.02, -0.35, 0.50, 7, 0.012, w, 0.42);
    addBusVertical(p, circuit, pulse, 0.58, -0.28, 0.16, 8, 0.011, w, 0.68);

    circuit += dashedBox(p, vec2(0.42, 0.31), vec2(0.33, 0.19), w);
    addResistorBank(p, circuit, pulse, vec2(0.19, 0.22), 8, w, 0.8);

    addBusHorizontal(p, circuit, pulse, 0.43, -0.12, 0.66, 5, 0.01, w, 0.9);
    addBusHorizontal(p, circuit, pulse, 0.10, 0.02, 0.66, 6, 0.012, w, 1.1);

    addResistorBank(p, circuit, pulse, vec2(0.03, -0.39), 7, w, 1.3);
    circuit += boxOutline(p, vec2(0.14, -0.47), vec2(0.028, 0.038), w);
    circuit += boxOutline(p, vec2(0.28, -0.44), vec2(0.06, 0.035), w);

    addBusHorizontal(p, circuit, pulse, -0.48, -0.12, 0.55, 5, 0.018, w, 1.5);
    addBusVertical(p, circuit, pulse, -0.62, -0.52, -0.22, 5, 0.014, w, 1.7);

    vec2 rp = p - vec2(0.68, -0.05);
    circuit += lineSegment(rp, vec2(-0.06, -0.33), vec2(-0.06, 0.30), w);
    circuit += lineSegment(rp, vec2(-0.06, 0.18), vec2(0.20, 0.18), w);
    circuit += lineSegment(rp, vec2(0.20, -0.15), vec2(0.20, 0.30), w);
    circuit += lineSegment(rp, vec2(-0.06, -0.15), vec2(0.20, -0.15), w);
    circuit += circleOutline(rp, vec2(-0.02, -0.08), 0.018, w);
    circuit += boxOutline(rp, vec2(0.05, -0.24), vec2(0.02, 0.04), w);
    circuit += lineSegment(rp, vec2(0.03, 0.02), vec2(0.17, 0.02), w);
    circuit += lineSegment(rp, vec2(0.03, 0.10), vec2(0.17, 0.10), w);
    pulse += tracePulse(rp, vec2(-0.06, -0.33), vec2(-0.06, 0.30), w, 0.2);
    pulse += tracePulse(rp, vec2(-0.06, 0.18), vec2(0.20, 0.18), w, 0.4);

    for (int i = 0; i < 42; i++) {
        float fi = float(i);
        vec2 c = vec2(
            mix(-0.72, 0.68, hash21(vec2(fi, 1.7))),
            mix(-0.48, 0.46, hash21(vec2(fi, 8.3)))
        );

        float n = hash21(c * 13.0);
        float active = step(0.35, n);

        circuit += dotNode(p, c, 0.0035 + 0.002 * n) * active;
        circuit += circleOutline(p, c + vec2(0.018, 0.0), 0.008, w) * step(0.72, n);
    }

    // fwidth() 없이 동작하는 고정 폭 블루프린트 그리드
    vec2 gridUV = p * 42.0;
    vec2 gridCell = abs(fract(gridUV - 0.5) - 0.5);
    float gridX = 1.0 - smoothstep(0.0, 0.035, gridCell.x);
    float gridY = 1.0 - smoothstep(0.0, 0.035, gridCell.y);
    float grid = max(gridX, gridY) * 0.045;

    pulse *= signalIntensity;
    pulse = clamp(pulse, 0.0, 1.0);

    float brightLines = clamp(circuit, 0.0, 1.0);
    float glowField = clamp(circuit * 0.22 + pulse * 0.9, 0.0, 1.0);

    vec3 col = bg;
    col += grid * lineColor.rgb;
    col += brightLines * lineColor.rgb * 0.92;
    col += glowField * lineColor.rgb * glow * 0.55;
    col += pulse * signalColor.rgb * 1.6;

    float scan = sin(uv.y * RENDERSIZE.y * 1.4 + TIME * 8.0) * 0.5 + 0.5;
    col *= 1.0 - scanlineAmount * scan * 0.16;

    float flicker = 0.96 + 0.04 * sin(TIME * 17.0) + 0.025 * sin(TIME * 37.0);
    col *= flicker;

    float n = hash21(uv * RENDERSIZE.xy + TIME * 19.17);
    col += (n - 0.5) * noiseAmount;

    if (useImage) {
        vec4 src = IMG_NORM_PIXEL(inputImage, uv);
        float srcLum = dot(src.rgb, vec3(0.299, 0.587, 0.114));
        vec3 imgTint = mix(bg, lineColor.rgb, srcLum);
        col = mix(col, imgTint + col * 0.45, imageBlend);
    }

    col *= 0.82 + 0.18 * vignette;

    if (invert) {
        col = 1.0 - col;
    }

    gl_FragColor = vec4(col, 1.0);
}