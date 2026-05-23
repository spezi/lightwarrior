/*{
  "DESCRIPTION": "Animated portrait line web / generative connected line mesh inspired by dense geometric threads.",
  "CREDIT": "Created with ChatGPT - ISF Shader Designer",
  "CATEGORIES": [
    "Generator",
    "Stylize",
    "Glitch",
    "Geometry"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "useImage",
      "TYPE": "bool",
      "DEFAULT": true
    },
    {
      "NAME": "density",
      "TYPE": "float",
      "DEFAULT": 34.0,
      "MIN": 8.0,
      "MAX": 90.0
    },
    {
      "NAME": "lineAmount",
      "TYPE": "float",
      "DEFAULT": 0.72,
      "MIN": 0.05,
      "MAX": 1.0
    },
    {
      "NAME": "lineWidth",
      "TYPE": "float",
      "DEFAULT": 0.85,
      "MIN": 0.15,
      "MAX": 4.0
    },
    {
      "NAME": "motionSpeed",
      "TYPE": "float",
      "DEFAULT": 0.22,
      "MIN": 0.0,
      "MAX": 3.0
    },
    {
      "NAME": "motionAmount",
      "TYPE": "float",
      "DEFAULT": 0.42,
      "MIN": 0.0,
      "MAX": 1.5
    },
    {
      "NAME": "morph",
      "TYPE": "float",
      "DEFAULT": 0.55,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "imageInfluence",
      "TYPE": "float",
      "DEFAULT": 0.78,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "edgeInfluence",
      "TYPE": "float",
      "DEFAULT": 0.65,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "nodeSize",
      "TYPE": "float",
      "DEFAULT": 1.1,
      "MIN": 0.0,
      "MAX": 5.0
    },
    {
      "NAME": "contrast",
      "TYPE": "float",
      "DEFAULT": 1.25,
      "MIN": 0.2,
      "MAX": 3.0
    },
    {
      "NAME": "backgroundMix",
      "TYPE": "float",
      "DEFAULT": 0.78,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": false
    },
    {
      "NAME": "lineColor",
      "TYPE": "color",
      "DEFAULT": [
        0.92,
        0.92,
        0.88,
        1.0
      ]
    },
    {
      "NAME": "backgroundColor",
      "TYPE": "color",
      "DEFAULT": [
        0.02,
        0.02,
        0.02,
        1.0
      ]
    },
    {
      "NAME": "shapeMode",
      "TYPE": "long",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 3
    }
  ]
}*/

float hash11(float p) {
    return fract(sin(p * 127.1) * 43758.5453123);
}

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

vec2 hash22(vec2 p) {
    float n = sin(dot(p, vec2(41.0, 289.0)));
    return fract(vec2(262144.0, 32768.0) * n);
}

float luminance(vec3 c) {
    return dot(c, vec3(0.299, 0.587, 0.114));
}

float lineSegmentDistance(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a;
    vec2 ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 0.00001), 0.0, 1.0);
    return length(pa - ba * h);
}

vec4 getImage(vec2 uv) {
    uv = clamp(uv, 0.0, 1.0);
    return IMG_NORM_PIXEL(inputImage, uv);
}

float imageMask(vec2 uv) {
    vec3 img = getImage(uv).rgb;
    float l = luminance(img);

    vec2 px = 1.0 / RENDERSIZE.xy;
    float lx1 = luminance(getImage(uv + vec2(px.x, 0.0)).rgb);
    float lx2 = luminance(getImage(uv - vec2(px.x, 0.0)).rgb);
    float ly1 = luminance(getImage(uv + vec2(0.0, px.y)).rgb);
    float ly2 = luminance(getImage(uv - vec2(0.0, px.y)).rgb);

    float edge = abs(lx1 - lx2) + abs(ly1 - ly2);
    edge = smoothstep(0.02, 0.22, edge * 3.0);

    float faceTone = smoothstep(0.08, 0.95, l);
    return mix(1.0, clamp(faceTone + edge * edgeInfluence, 0.0, 1.8), imageInfluence);
}

vec2 networkPoint(vec2 id, float aspect) {
    vec2 rnd = hash22(id);
    float t = TIME * motionSpeed;

    vec2 base = id + 0.5;

    vec2 driftA = vec2(
        sin(t + id.x * 1.71 + id.y * 0.39),
        cos(t * 0.91 + id.y * 1.43 + id.x * 0.27)
    );

    vec2 driftB = vec2(
        sin(t * 0.43 + length(id) * 0.9),
        cos(t * 0.37 + atan(id.y, id.x))
    );

    vec2 jitter = (rnd - 0.5) * 1.35;
    vec2 drift = mix(driftA, driftB, morph) * motionAmount;

    if (shapeMode == 1) {
        float wave = sin((id.x + id.y) * 0.65 + t * 2.0);
        drift += vec2(wave, -wave) * 0.28 * motionAmount;
    } else if (shapeMode == 2) {
        vec2 center = vec2(aspect * density * 0.5, density * 0.5);
        vec2 dir = normalize(id - center + 0.001);
        float pulse = sin(t * 2.2 + length(id - center) * 0.35);
        drift += dir * pulse * 0.45 * motionAmount;
    } else if (shapeMode == 3) {
        float ang = t + hash21(id) * 6.28318;
        drift += vec2(cos(ang), sin(ang)) * 0.55 * motionAmount;
    }

    return (base + jitter * morph + drift) / density;
}

float drawSegment(vec2 p, vec2 a, vec2 b, float w, float strength) {
    float d = lineSegmentDistance(p, a, b);
    float line = 1.0 - smoothstep(w, w * 2.7, d);
    float glow = exp(-d * d / max(w * w * 14.0, 0.000001)) * 0.18;
    return (line + glow) * strength;
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    vec2 p = vec2(uv.x * aspect, uv.y);
    vec2 grid = p * density;
    vec2 baseCell = floor(grid);

    vec4 src = getImage(uv);
    float gray = luminance(src.rgb);
    gray = pow(clamp(gray, 0.0, 1.0), contrast);

    if (!useImage) {
        float vignette = 1.0 - length((uv - 0.5) * vec2(aspect, 1.0));
        gray = smoothstep(0.0, 1.0, vignette);
    }

    float pxWidth = lineWidth / RENDERSIZE.y;
    float web = 0.0;
    float nodes = 0.0;

    for (int y = -2; y <= 2; ++y) {
        for (int x = -2; x <= 2; ++x) {
            vec2 id = baseCell + vec2(float(x), float(y));
            vec2 a = networkPoint(id, aspect);

            vec2 b1 = networkPoint(id + vec2(1.0, 0.0), aspect);
            vec2 b2 = networkPoint(id + vec2(0.0, 1.0), aspect);
            vec2 b3 = networkPoint(id + vec2(1.0, 1.0), aspect);
            vec2 b4 = networkPoint(id + vec2(-1.0, 1.0), aspect);

            float gate1 = smoothstep(1.0 - lineAmount, 1.0, hash21(id + 13.7));
            float gate2 = smoothstep(1.0 - lineAmount, 1.0, hash21(id + 29.3));
            float gate3 = smoothstep(1.0 - lineAmount * 0.72, 1.0, hash21(id + 71.9));
            float gate4 = smoothstep(1.0 - lineAmount * 0.55, 1.0, hash21(id + 103.4));

            float flicker = 0.72 + 0.28 * sin(TIME * motionSpeed * 5.0 + hash21(id) * 6.28318);

            vec2 auv = vec2(a.x / aspect, a.y);
            float mask = useImage ? imageMask(auv) : 1.0;

            web += drawSegment(p, a, b1, pxWidth, gate1 * flicker * mask);
            web += drawSegment(p, a, b2, pxWidth, gate2 * flicker * mask);
            web += drawSegment(p, a, b3, pxWidth * 0.8, gate3 * flicker * mask);
            web += drawSegment(p, a, b4, pxWidth * 0.65, gate4 * flicker * mask);

            float nd = length(p - a);
            float node = 1.0 - smoothstep(pxWidth * nodeSize, pxWidth * nodeSize * 3.0, nd);
            nodes += node * mask * 0.55;
        }
    }

    web = clamp(web, 0.0, 1.0);
    nodes = clamp(nodes, 0.0, 1.0);

    vec3 bgImage = vec3(gray);
    vec3 bg = mix(backgroundColor.rgb, bgImage, backgroundMix);

    float webMask = clamp(web + nodes, 0.0, 1.0);
    vec3 col = mix(bg, lineColor.rgb, webMask);

    // 이미지의 어두운 부분에는 선이 더 잘 보이도록 약간의 콘트라스트 추가
    col += lineColor.rgb * web * (1.0 - gray) * 0.35;

    if (invert) {
        col = 1.0 - col;
    }

    gl_FragColor = vec4(col, 1.0);
}