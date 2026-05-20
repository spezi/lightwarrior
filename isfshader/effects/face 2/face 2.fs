/*{
    "ISFVSN":"2",
    "TYPE":"IMAGE",
    "CATEGORIES":[
        "Stylize",
        "Portrait",
        "Film",
        "Glitch"
    ],
    "INPUTS":[
        {
            "NAME":"inputImage",
            "TYPE":"image"
        },

        {
            "NAME":"brightness",
            "TYPE":"float",
            "DEFAULT":1.0,
            "MIN":0.0,
            "MAX":3.0
        },
        {
            "NAME":"contrast",
            "TYPE":"float",
            "DEFAULT":1.15,
            "MIN":0.0,
            "MAX":3.0
        },
        {
            "NAME":"monochrome",
            "TYPE":"float",
            "DEFAULT":0.9,
            "MIN":0.0,
            "MAX":1.0
        },

        {
            "NAME":"tintColor",
            "TYPE":"color",
            "DEFAULT":[0.72,0.74,0.74,1.0]
        },
        {
            "NAME":"tintAmount",
            "TYPE":"float",
            "DEFAULT":0.35,
            "MIN":0.0,
            "MAX":1.0
        },

        {
            "NAME":"blurAmount",
            "TYPE":"float",
            "DEFAULT":2.6,
            "MIN":0.0,
            "MAX":12.0
        },
        {
            "NAME":"fogAmount",
            "TYPE":"float",
            "DEFAULT":0.42,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"grainAmount",
            "TYPE":"float",
            "DEFAULT":0.06,
            "MIN":0.0,
            "MAX":0.4
        },
        {
            "NAME":"vignetteAmount",
            "TYPE":"float",
            "DEFAULT":0.55,
            "MIN":0.0,
            "MAX":1.5
        },

        {
            "NAME":"breathingAmount",
            "TYPE":"float",
            "DEFAULT":0.015,
            "MIN":0.0,
            "MAX":0.15
        },
        {
            "NAME":"breathingSpeed",
            "TYPE":"float",
            "DEFAULT":0.25,
            "MIN":0.0,
            "MAX":3.0
        },

        {
            "NAME":"distortionAmount",
            "TYPE":"float",
            "DEFAULT":0.012,
            "MIN":0.0,
            "MAX":0.2
        },
        {
            "NAME":"distortionSpeed",
            "TYPE":"float",
            "DEFAULT":0.18,
            "MIN":0.0,
            "MAX":4.0
        },

        {
            "NAME":"glitchAmount",
            "TYPE":"float",
            "DEFAULT":0.01,
            "MIN":0.0,
            "MAX":0.25
        },
        {
            "NAME":"glitchSpeed",
            "TYPE":"float",
            "DEFAULT":0.35,
            "MIN":0.0,
            "MAX":8.0
        },

        {
            "NAME":"audioLevel",
            "TYPE":"float",
            "DEFAULT":0.0,
            "MIN":0.0,
            "MAX":1.0
        }
    ]
}*/

float random(vec2 p)
{
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

float noise(vec2 p)
{
    vec2 i = floor(p);
    vec2 f = fract(p);

    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(a, b, u.x)
         + (c - a) * u.y * (1.0 - u.x)
         + (d - b) * u.x * u.y;
}

float fbm(vec2 p)
{
    float v = 0.0;
    float a = 0.5;

    v += a * noise(p);
    p *= 2.02;
    a *= 0.5;

    v += a * noise(p);
    p *= 2.03;
    a *= 0.5;

    v += a * noise(p);
    p *= 2.01;
    a *= 0.5;

    v += a * noise(p);

    return v;
}

vec3 applyContrast(vec3 color, float amount)
{
    return (color - 0.5) * amount + 0.5;
}

vec4 sampleBlurredImage(vec2 uv, float blur)
{
    vec2 px = vec2(blur) / RENDERSIZE.xy;

    vec4 col = vec4(0.0);

    col += IMG_NORM_PIXEL(inputImage, uv) * 0.28;

    col += IMG_NORM_PIXEL(inputImage, uv + vec2( px.x,  0.0)) * 0.11;
    col += IMG_NORM_PIXEL(inputImage, uv + vec2(-px.x,  0.0)) * 0.11;
    col += IMG_NORM_PIXEL(inputImage, uv + vec2( 0.0,  px.y)) * 0.11;
    col += IMG_NORM_PIXEL(inputImage, uv + vec2( 0.0, -px.y)) * 0.11;

    col += IMG_NORM_PIXEL(inputImage, uv + vec2( px.x,  px.y)) * 0.07;
    col += IMG_NORM_PIXEL(inputImage, uv + vec2(-px.x,  px.y)) * 0.07;
    col += IMG_NORM_PIXEL(inputImage, uv + vec2( px.x, -px.y)) * 0.07;
    col += IMG_NORM_PIXEL(inputImage, uv + vec2(-px.x, -px.y)) * 0.07;

    return col;
}

void main()
{
    vec2 uv = isf_FragNormCoord.xy;
    vec2 originalUV = uv;
    vec2 center = vec2(0.5);

    float t = TIME;
    float audio = clamp(audioLevel, 0.0, 1.0);

    // 1. 호흡하듯 확대 / 축소
    float breath = sin(t * breathingSpeed * 6.28318) * breathingAmount;
    vec2 centeredUV = uv - center;
    uv = center + centeredUV * (1.0 - breath - audio * 0.012);

    // 2. 안개 움직임
    float fog = fbm(uv * 2.4 + vec2(t * 0.05, -t * 0.035));

    // 3. 물결 왜곡
    float waveX = sin((uv.y * 8.0 + t * distortionSpeed) * 6.28318);
    float waveY = sin((uv.x * 5.0 - t * distortionSpeed * 0.7) * 6.28318);

    vec2 distortion = vec2(waveX, waveY) * distortionAmount;
    distortion *= 0.45 + fog * 0.55;
    distortion *= 1.0 + audio * 1.2;

    uv += distortion;

    // 4. 가로 글리치 흔들림
    float line = floor(uv.y * 70.0);
    float glitchNoise = random(vec2(line, floor(t * glitchSpeed * 10.0)));
    float glitchMask = step(0.965, glitchNoise);

    uv.x += (glitchNoise - 0.5) * glitchAmount * glitchMask * (1.0 + audio * 1.5);

    // 5. 블러 샘플
    float blur = blurAmount + audio * 2.5;
    vec4 src = sampleBlurredImage(uv, blur);

    vec3 color = src.rgb;

    // 6. 흑백화
    float luma = dot(color, vec3(0.299, 0.587, 0.114));
    vec3 gray = vec3(luma);
    color = mix(color, gray, monochrome);

    // 7. 콘트라스트
    color = applyContrast(color, contrast + audio * 0.25);

    // 8. 색조 입히기
    float tonedLuma = dot(color, vec3(0.299, 0.587, 0.114));
    vec3 tinted = mix(vec3(tonedLuma), tintColor.rgb, 0.45);
    color = mix(color, tinted, tintAmount);

    // 9. 안개 레이어
    float fogMask = smoothstep(0.2, 1.0, fog);
    color = mix(color, vec3(0.58, 0.59, 0.59), fogMask * fogAmount * 0.5);

    // 10. 비네팅
    float dist = distance(originalUV, center);
    float vignette = smoothstep(0.85, 0.2, dist);
    color *= mix(1.0 - vignetteAmount, 1.0, vignette);

    // 11. 필름 그레인
    float grain = random(originalUV * RENDERSIZE.xy + t * 20.0);
    color += (grain - 0.5) * grainAmount;

    // 12. 밝기
    color *= brightness + audio * 0.15;

    color = clamp(color, 0.0, 1.0);

    gl_FragColor = vec4(color, src.a);
}