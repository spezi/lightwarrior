/*{
  "CATEGORIES": [
    "Generator",
    "Audio Reactive",
    "Glitch",
    "Media Art"
  ],
  "DESCRIPTION": "Audio reactive wireframe portrait transformer for MadMapper. Uses an input image and procedural line network.",
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "audioLevel",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "bass",
      "TYPE": "float",
      "DEFAULT": 0.15,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "mid",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "treble",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "complexity",
      "TYPE": "float",
      "DEFAULT": 0.55,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "lineBrightness",
      "TYPE": "float",
      "DEFAULT": 0.85,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "lineWidth",
      "TYPE": "float",
      "DEFAULT": 0.0035,
      "MIN": 0.0005,
      "MAX": 0.02
    },
    {
      "NAME": "lineOpacity",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "imageMix",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "imageContrast",
      "TYPE": "float",
      "DEFAULT": 1.25,
      "MIN": 0.2,
      "MAX": 3.0
    },
    {
      "NAME": "imageWarp",
      "TYPE": "float",
      "DEFAULT": 0.025,
      "MIN": 0.0,
      "MAX": 0.15
    },
    {
      "NAME": "glitchAmount",
      "TYPE": "float",
      "DEFAULT": 0.025,
      "MIN": 0.0,
      "MAX": 0.2
    },
    {
      "NAME": "motionSpeed",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": 0.0,
      "MAX": 3.0
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.5,
      "MAX": 2.0
    },
    {
      "NAME": "rotation",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -3.14159,
      "MAX": 3.14159
    },
    {
      "NAME": "colorMode",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 3.0
    },
    {
      "NAME": "tintA",
      "TYPE": "color",
      "DEFAULT": [
        1.0,
        1.0,
        1.0,
        1.0
      ]
    },
    {
      "NAME": "tintB",
      "TYPE": "color",
      "DEFAULT": [
        0.35,
        0.65,
        1.0,
        1.0
      ]
    },
    {
      "NAME": "backgroundLevel",
      "TYPE": "float",
      "DEFAULT": 0.02,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "invertImage",
      "TYPE": "bool",
      "DEFAULT": false
    },
    {
      "NAME": "showOriginalImage",
      "TYPE": "bool",
      "DEFAULT": true
    },
    {
      "NAME": "showGeneratedLines",
      "TYPE": "bool",
      "DEFAULT": true
    }
  ],
  "ISFVSN": "2"
}*/

#define MAX_LINES 90

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

float luma(vec3 c) {
    return dot(c, vec3(0.299, 0.587, 0.114));
}

mat2 rot2d(float a) {
    float s = sin(a);
    float c = cos(a);
    return mat2(c, -s, s, c);
}

float lineSegment(vec2 p, vec2 a, vec2 b, float w) {
    vec2 pa = p - a;
    vec2 ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 0.0001), 0.0, 1.0);
    float d = length(pa - ba * h);
    return smoothstep(w, 0.0, d);
}

vec2 aspectUV(vec2 uv) {
    vec2 p = uv - 0.5;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
    return p;
}

vec2 imageUV(vec2 p) {
    p.x /= RENDERSIZE.x / RENDERSIZE.y;
    return p + 0.5;
}

vec3 colorize(float v, float lineMask) {
    float mode = floor(colorMode + 0.5);

    vec3 mono = vec3(v);
    vec3 cold = mix(tintA.rgb * v, tintB.rgb, lineMask);
    vec3 pulse = mix(tintA.rgb * v, tintB.rgb * (0.4 + audioLevel * 1.8), lineMask);
    vec3 spectral = vec3(
        v * (0.7 + treble * 1.4),
        v * (0.7 + mid * 1.2),
        v * (0.8 + bass * 1.5)
    );

    if (mode < 0.5) {
        return mono;
    } else if (mode < 1.5) {
        return cold;
    } else if (mode < 2.5) {
        return pulse;
    } else {
        return spectral;
    }
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    vec2 p = aspectUV(uv);

    float t = TIME * motionSpeed;

    // 16:9 중심 기준 변환
    vec2 q = p;
    q *= zoom;
    q = rot2d(rotation + sin(t * 0.18) * 0.035 * audioLevel) * q;

    // 이미지 왜곡
    float n1 = hash21(floor(q * 12.0 + t));
    float n2 = hash21(floor(q * 24.0 - t * 0.7));

    vec2 warp;
    warp.x = sin(q.y * 8.0 + t * 1.2 + n1 * 6.2831);
    warp.y = cos(q.x * 7.0 - t * 0.9 + n2 * 6.2831);
    warp *= imageWarp * (0.35 + audioLevel * 1.8);

    // 글리치: 수평 슬라이스 이동
    float slice = floor(uv.y * 32.0);
    float sliceRnd = hash11(slice + floor(t * 6.0));
    float glitch = (sliceRnd - 0.5) * glitchAmount * (0.2 + bass * 2.0);

    vec2 imgUV = imageUV(q + warp);
    imgUV.x += glitch;

    vec4 src = IMG_NORM_PIXEL(inputImage, imgUV);

    float img = luma(src.rgb);
    img = pow(img, 1.0 / max(imageContrast, 0.001));
    img = clamp((img - 0.5) * imageContrast + 0.5, 0.0, 1.0);

    if (invertImage) {
        img = 1.0 - img;
    }

    // 이미지 엣지 추출
    float px = 1.0 / RENDERSIZE.x;
    float py = 1.0 / RENDERSIZE.y;

    float imgR = luma(IMG_NORM_PIXEL(inputImage, imgUV + vec2(px * 2.0, 0.0)).rgb);
    float imgL = luma(IMG_NORM_PIXEL(inputImage, imgUV - vec2(px * 2.0, 0.0)).rgb);
    float imgU = luma(IMG_NORM_PIXEL(inputImage, imgUV + vec2(0.0, py * 2.0)).rgb);
    float imgD = luma(IMG_NORM_PIXEL(inputImage, imgUV - vec2(0.0, py * 2.0)).rgb);

    float edge = abs(imgR - imgL) + abs(imgU - imgD);
    edge = smoothstep(0.04, 0.32, edge);

    // 절차적 라인 네트워크
    float lines = 0.0;
    float fineLines = 0.0;

    float lineCountF = mix(12.0, float(MAX_LINES), complexity);
    lineCountF += audioLevel * 20.0 + treble * 12.0;
    lineCountF = clamp(lineCountF, 1.0, float(MAX_LINES));

    for (int i = 0; i < MAX_LINES; i++) {
        float fi = float(i);

        if (fi < lineCountF) {
            vec2 seedA = vec2(fi * 1.17, fi * 2.31);
            vec2 seedB = vec2(fi * 3.71 + 9.1, fi * 0.83 + 4.7);

            vec2 a = hash22(seedA);
            vec2 b = hash22(seedB);

            a = a * 2.0 - 1.0;
            b = b * 2.0 - 1.0;

            a.x *= RENDERSIZE.x / RENDERSIZE.y;
            b.x *= RENDERSIZE.x / RENDERSIZE.y;

            float driftA = sin(t * (0.15 + hash11(fi) * 0.35) + fi);
            float driftB = cos(t * (0.12 + hash11(fi + 3.4) * 0.4) - fi);

            a += vec2(driftA, driftB) * 0.08 * (0.25 + mid);
            b += vec2(driftB, driftA) * 0.10 * (0.25 + treble);

            // 음악 반응으로 라인 확장/수축
            float expand = 1.0 + bass * 0.35 + audioLevel * 0.25;
            a *= expand;
            b *= expand;

            float w = lineWidth * (0.7 + treble * 2.0);
            float ln = lineSegment(p, a, b, w);

            // 원본 이미지 밝기와 엣지가 있는 곳에서 라인 강화
            float imageFollow = 0.35 + img * 0.55 + edge * 0.9;

            float randomPulse = 0.55 + 0.45 * sin(t * 1.5 + fi * 0.73);
            float amp = mix(0.35, 1.0, hash11(fi * 2.7));
            amp *= randomPulse;
            amp *= imageFollow;

            lines += ln * amp;

            // 얇은 보조 라인
            float lnFine = lineSegment(p, a * 0.92, b * 1.05, w * 0.45);
            fineLines += lnFine * amp * 0.45;
        }
    }

    lines = clamp(lines, 0.0, 1.6);
    fineLines = clamp(fineLines, 0.0, 1.0);

    // 이미지 자체의 살아있는 변화
    float breathing = 0.5 + 0.5 * sin(t * 0.7);
    float imagePulse = 1.0 + audioLevel * 0.55 + breathing * 0.12;

    float baseImage = img * imagePulse;
    baseImage += edge * (0.25 + treble * 0.8);
    baseImage *= imageMix;

    // 배경
    vec3 bg = vec3(backgroundLevel);

    // 라인 밝기
    float lineMask = (lines + fineLines * 0.75) * lineOpacity * lineBrightness;
    lineMask *= 0.8 + audioLevel * 1.4;

    // 최종 합성
    float finalValue = backgroundLevel;

    if (showOriginalImage) {
        finalValue += baseImage;
    }

    if (showGeneratedLines) {
        finalValue += lineMask;
    }

    finalValue = clamp(finalValue, 0.0, 1.0);

    vec3 col = colorize(finalValue, clamp(lineMask, 0.0, 1.0));

    // 고대비 미디어아트 느낌
    col = pow(col, vec3(0.85));
    col += bg * 0.25;

    gl_FragColor = vec4(col, 1.0);
}