/*{
    "DESCRIPTION": "The Ink Dance (Optimized & Extended) - Audio Reactive Ink Wash",
    "CREDIT": "Generated for Media Art",
    "ISFVSN": "2",
    "CATEGORIES": [ "Art", "Image Processing", "Animation" ],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "base_speed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.2
        },
        {
            "NAME": "noise_scale",
            "TYPE": "float",
            "MIN": 2.0,
            "MAX": 30.0,
            "DEFAULT": 10.0
        },
        {
            "NAME": "ink_spread",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.15,
            "DEFAULT": 0.03
        },
        {
            "NAME": "wobble_amount",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.1,
            "DEFAULT": 0.015
        },
        {
            "NAME": "wobble_speed",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 50.0,
            "DEFAULT": 20.0
        },
        {
            "NAME": "audio_level",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "audio_dampening",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.8
        },
        {
            "NAME": "drip_speed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.4
        },
        {
            "NAME": "drip_length",
            "TYPE": "float",
            "MIN": 10.0,
            "MAX": 100.0,
            "DEFAULT": 40.0
        },
        {
            "NAME": "contrast",
            "TYPE": "float",
            "MIN": 0.5,
            "MAX": 3.0,
            "DEFAULT": 1.5
        },
        {
            "NAME": "original_color_mix",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "inkColor",
            "TYPE": "color",
            "DEFAULT": [0.05, 0.05, 0.05, 1.0]
        },
        {
            "NAME": "paperColor",
            "TYPE": "color",
            "DEFAULT": [0.92, 0.9, 0.85, 1.0]
        }
    ]
}*/

// 가볍고 빠른 해시 기반 난수 생성기
float rand(vec2 n) { 
    return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

// 최적화된 2D 노이즈
float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(rand(i), rand(i + vec2(1.0, 0.0)), f.x),
        mix(rand(i + vec2(0.0, 1.0)), rand(i + vec2(1.0, 1.0)), f.x), f.y);
}

// 최적화된 FBM (반복 횟수를 4번으로 제한하여 성능 향상)
float fbm(vec2 x) {
    float v = 0.0;
    float a = 0.5;
    vec2 shift = vec2(100.0);
    for (int i = 0; i < 4; ++i) {
        v += a * noise(x);
        x = x * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    
    // 시간 및 오디오 변수 사전 계산 (중복 연산 방지)
    float time_val = TIME * base_speed;
    float volume = audio_level * audio_dampening;

    // --- 1. 먹 번짐 왜곡 (Distortion) ---
    // noise_scale을 곱해 번지는 입자 크기를 결정합니다.
    vec2 noiseUV = uv * noise_scale + time_val;
    // x와 y에 대해 각각 다른 노이즈 값을 주어 불규칙하게 퍼지도록 합니다 (-0.5를 빼서 양방향 확산).
    vec2 distort = vec2(fbm(noiseUV), fbm(noiseUV + vec2(1.3, 2.7))) - 0.5;
    
    // 오디오가 클수록 확산 범위(spread)가 기하급수적으로 커집니다.
    float dynamic_spread = ink_spread * (1.0 + volume * 2.5);

    // --- 2. 오디오 기반 실루엣 흔들림 (Wobble) ---
    // wobble_speed 패러미터로 흔들림의 진동수를 제어합니다.
    vec2 wobble = vec2(
        sin(uv.y * noise_scale + TIME * wobble_speed),
        cos(uv.x * noise_scale + TIME * wobble_speed)
    ) * wobble_amount * volume;

    // 최종 왜곡된 UV 좌표 도출
    vec2 finalUV = uv + (distort * dynamic_spread) + wobble;
    
    // --- 3. 이미지 샘플링 및 명암 추출 ---
    // 화면 밖으로 밀려난 텍스처를 자연스럽게 처리 (Clamp)
    finalUV = clamp(finalUV, 0.0, 1.0);
    vec4 originalImg = IMG_NORM_PIXEL(inputImage, finalUV);

    // 흑백 명암(Luma) 추출 후 대비(Contrast) 적용
    float inkDensity = dot(originalImg.rgb, vec3(0.299, 0.587, 0.114));
    inkDensity = pow(inkDensity, contrast);
    // 먹물 농도 반전 (어두운 곳이 잉크가 됨)
    float inkAmount = clamp(1.0 - inkDensity, 0.0, 1.0); 

    // --- 4. 잉크 흘러내림(Drip) 애니메이션 ---
    // 잉크가 진한 영역의 아래쪽 테두리를 계산합니다.
    float lowerEdge = smoothstep(0.4, 0.7, uv.y + inkAmount * 0.3);
    
    // drip_length와 drip_speed를 반영하여 흘러내리는 노이즈 생성
    float dripNoise = fbm(vec2(uv.x * noise_scale * 1.5, lowerEdge * drip_length + TIME * drip_speed * 10.0));
    float dripMask = smoothstep(0.1, 0.3, dripNoise * lowerEdge * inkAmount);
    
    // --- 5. 최종 색상 혼합 ---
    // 배경(종이)과 잉크 색상을 혼합합니다.
    vec4 duotoneColor = mix(paperColor, inkColor, inkAmount);
    // 물방울 추가
    duotoneColor = mix(duotoneColor, inkColor, dripMask);

    // original_color_mix 슬라이더를 통해 원본 컬러를 블렌딩
    // 값이 0이면 완전한 수묵화, 1이면 먹이 번지는 풀컬러 영상이 됩니다.
    gl_FragColor = mix(duotoneColor, originalImg, original_color_mix);
}