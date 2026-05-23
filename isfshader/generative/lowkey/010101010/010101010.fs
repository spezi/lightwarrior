/*{
    "ISFVSN":"2",
    "TYPE":"GENERATOR",
    "CREDIT":"OpenAI",
    "DESCRIPTION":"Binary code texture generator with controllable 0/1 digits, glitch, flicker, density, scrolling, and color. MadMapper compatible.",
    "INPUTS":[
        {
            "NAME":"Columns",
            "TYPE":"float",
            "DEFAULT":72.0,
            "MIN":8.0,
            "MAX":220.0
        },
        {
            "NAME":"Rows",
            "TYPE":"float",
            "DEFAULT":42.0,
            "MIN":6.0,
            "MAX":140.0
        },
        {
            "NAME":"ScrollX",
            "TYPE":"float",
            "DEFAULT":-1.2,
            "MIN":-20.0,
            "MAX":20.0
        },
        {
            "NAME":"ScrollY",
            "TYPE":"float",
            "DEFAULT":0.0,
            "MIN":-20.0,
            "MAX":20.0
        },
        {
            "NAME":"ChangeRate",
            "TYPE":"float",
            "DEFAULT":0.2,
            "MIN":0.0,
            "MAX":20.0
        },
        {
            "NAME":"OneProbability",
            "TYPE":"float",
            "DEFAULT":0.5,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"ActiveDensity",
            "TYPE":"float",
            "DEFAULT":1.0,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"Seed",
            "TYPE":"float",
            "DEFAULT":1.0,
            "MIN":0.0,
            "MAX":100.0
        },
        {
            "NAME":"Jitter",
            "TYPE":"float",
            "DEFAULT":0.0,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"PixelGap",
            "TYPE":"float",
            "DEFAULT":0.12,
            "MIN":0.0,
            "MAX":0.45
        },
        {
            "NAME":"Softness",
            "TYPE":"float",
            "DEFAULT":0.03,
            "MIN":0.001,
            "MAX":0.25
        },
        {
            "NAME":"Glow",
            "TYPE":"float",
            "DEFAULT":0.35,
            "MIN":0.0,
            "MAX":2.0
        },
        {
            "NAME":"Brightness",
            "TYPE":"float",
            "DEFAULT":1.0,
            "MIN":0.0,
            "MAX":4.0
        },
        {
            "NAME":"Contrast",
            "TYPE":"float",
            "DEFAULT":1.1,
            "MIN":0.0,
            "MAX":3.0
        },
        {
            "NAME":"FlickerAmount",
            "TYPE":"float",
            "DEFAULT":0.15,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"FlickerSpeed",
            "TYPE":"float",
            "DEFAULT":5.0,
            "MIN":0.0,
            "MAX":40.0
        },
        {
            "NAME":"GlitchAmount",
            "TYPE":"float",
            "DEFAULT":0.08,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"GlitchRate",
            "TYPE":"float",
            "DEFAULT":4.0,
            "MIN":0.1,
            "MAX":30.0
        },
        {
            "NAME":"Scanlines",
            "TYPE":"float",
            "DEFAULT":0.12,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"AudioLevel",
            "TYPE":"float",
            "DEFAULT":0.0,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"AudioToBrightness",
            "TYPE":"float",
            "DEFAULT":0.7,
            "MIN":0.0,
            "MAX":3.0
        },
        {
            "NAME":"AudioToGlitch",
            "TYPE":"float",
            "DEFAULT":0.3,
            "MIN":0.0,
            "MAX":2.0
        },
        {
            "NAME":"TextColor",
            "TYPE":"color",
            "DEFAULT":[1.0,1.0,1.0,1.0]
        },
        {
            "NAME":"BackgroundColor",
            "TYPE":"color",
            "DEFAULT":[0.0,0.0,0.0,1.0]
        }
    ]
}*/

precision highp float;

float hash21(vec2 p) {
    p += Seed * 17.31;
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

float feq(float a, float b) {
    return 1.0 - step(0.5, abs(a - b));
}

float pixelInterior(vec2 sub, float gap, float soft) {
    float size = max(0.02, 0.5 - gap);
    float d = max(abs(sub.x), abs(sub.y));
    return 1.0 - smoothstep(size, size + soft, d);
}

float digitMask01(vec2 uv, float digit, float gap, float soft) {
    vec2 p = uv * vec2(3.0, 5.0);
    vec2 g = floor(p);
    vec2 sub = fract(p) - 0.5;

    float x0 = feq(g.x, 0.0);
    float x1 = feq(g.x, 1.0);
    float x2 = feq(g.x, 2.0);

    float y0 = feq(g.y, 0.0);
    float y1 = feq(g.y, 1.0);
    float y2 = feq(g.y, 2.0);
    float y3 = feq(g.y, 3.0);
    float y4 = feq(g.y, 4.0);

    // 0 : 사각 픽셀 폰트 스타일
    float zero = max(max(x0, x2), max(y0, y4));
    zero = clamp(zero, 0.0, 1.0);

    // 1 : 가운데 세로 막대
    float one = x1;
    one = clamp(one, 0.0, 1.0);

    float onCell = mix(zero, one, step(0.5, digit));
    return onCell * pixelInterior(sub, gap, soft);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;

    float t = TIME;
    float audioBright = 1.0 + AudioLevel * AudioToBrightness;
    float effectiveGlitch = clamp(GlitchAmount + AudioLevel * AudioToGlitch, 0.0, 1.5);

    // 행 단위 글리치
    float rowIndex = floor(uv.y * Rows);
    float glitchStep = floor(t * GlitchRate);
    float rowRand = hash21(vec2(rowIndex, glitchStep + 10.0));
    float glitchGate = step(1.0 - effectiveGlitch, rowRand);
    float glitchOffset = (hash21(vec2(rowIndex + 23.7, glitchStep + 91.1)) - 0.5) * 0.28 * effectiveGlitch * glitchGate;
    uv.x += glitchOffset;

    // 기본 그리드
    vec2 grid = vec2(Columns, Rows);
    vec2 p = uv * grid + vec2(t * ScrollX, t * ScrollY);

    vec2 cellID = floor(p);
    vec2 localUV = fract(p);

    // 셀별 지터
    vec2 jitterVec = vec2(
        hash21(cellID + vec2(1.3, 7.1)),
        hash21(cellID + vec2(9.2, 3.4))
    ) - 0.5;
    localUV = fract(localUV + jitterVec * Jitter * 0.18);

    // 숫자 변화
    float timeIndex = floor(t * ChangeRate);
    float rnd = hash21(cellID + vec2(timeIndex * 13.7, timeIndex * 7.9));

    // 1의 등장 비율
    float digit = 1.0 - step(OneProbability, rnd);

    // 활성 밀도
    float activeRnd = hash21(cellID + vec2(41.2, 93.8));
    float active = 1.0 - step(ActiveDensity, activeRnd);

    // 선명한 마스크
    float sharpMask = digitMask01(localUV, digit, PixelGap, Softness);

    // 글로우용 약간 두꺼운 마스크
    float fatGap = max(PixelGap * 0.35, 0.0);
    float glowMask = digitMask01(localUV, digit, fatGap, Softness + 0.08);
    glowMask = max(glowMask - sharpMask, 0.0);

    // 깜빡임
    float flickPhase = hash21(cellID + vec2(14.7, 2.1)) * 6.28318530718;
    float flicker = mix(1.0, 0.65 + 0.35 * sin(t * FlickerSpeed + flickPhase), FlickerAmount);

    // 최종 밝기
    float value = active * (
        sharpMask * Brightness * audioBright +
        glowMask * Glow * (0.8 + AudioLevel * 1.2)
    ) * flicker;

    // 스캔라인
    float scan = mix(
        1.0,
        0.9 + 0.1 * sin(uv.y * RENDERSIZE.y * 1.25 + t * 0.5),
        Scanlines
    );
    value *= scan;

    vec3 color = BackgroundColor.rgb + TextColor.rgb * value;

    // 대비
    color = (color - 0.5) * Contrast + 0.5;

    // 안전 클램프
    color = clamp(color, 0.0, 1.0);

    gl_FragColor = vec4(color, 1.0);
}