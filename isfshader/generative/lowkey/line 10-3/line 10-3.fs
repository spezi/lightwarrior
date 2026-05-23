/*{
  "DESCRIPTION": "Organic fluid vertical lines with localized distortion.",
  "CREDIT": "Gemini",
  "ISFVSN": "2",
  "CATEGORIES": [ "Generative", "Visuals" ],
  "INPUTS": [
    {
      "NAME": "lineCount",
      "TYPE": "float",
      "MIN": 10.0,
      "MAX": 250.0,
      "DEFAULT": 80.0,
      "LABEL": "Number of Lines"
    },
    {
      "NAME": "lineThickness",
      "TYPE": "float",
      "MIN": 0.0001,
      "MAX": 0.01,
      "DEFAULT": 0.0015,
      "LABEL": "Line Thickness"
    },
    {
      "NAME": "noiseAmount",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 0.5,
      "DEFAULT": 0.18,
      "LABEL": "Max Distortion"
    },
    {
      "NAME": "noiseFreqY",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 10.0,
      "DEFAULT": 3.0,
      "LABEL": "Wave Frequency (Y)"
    },
    {
      "NAME": "noiseFreqX",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 20.0,
      "DEFAULT": 4.0,
      "LABEL": "Line Difference (X)"
    },
    {
      "NAME": "speed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 5.0,
      "DEFAULT": 0.5,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "distortionMask",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.8,
      "LABEL": "Straight vs Wavy Mix"
    },
    {
      "NAME": "audioMod",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.0,
      "LABEL": "Audio FFT Input"
    },
    {
      "NAME": "lineColor",
      "TYPE": "color",
      "DEFAULT": [ 1.0, 1.0, 1.0, 1.0 ]
    },
    {
      "NAME": "bgColor",
      "TYPE": "color",
      "DEFAULT": [ 0.0, 0.0, 0.0, 1.0 ]
    }
  ]
}*/

// 2D Random Hash 함수
float hash(vec2 p) {
    p = fract(p * vec2(234.34, 435.345));
    p += dot(p, p + 34.23);
    return fract(p.x * p.y);
}

// 2D Value Noise 함수
float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// 유기적인 곡선을 위한 FBM (Fractal Brownian Motion)
float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    vec2 shift = vec2(100.0);
    for (int i = 0; i < 4; ++i) {
        v += a * noise(p);
        p = p * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float minDist = 1.0;
    
    // 라인 개수 제한 (프레임 드랍 방지)
    int maxLines = int(clamp(lineCount, 10.0, 250.0));
    
    // 기본 왜곡 값에 오디오 모듈레이션 값 합산 (사운드 리액티브 용)
    float currentNoiseAmount = noiseAmount + (audioMod * 0.3);

    for(int i = 0; i < 250; i++) {
        if(i >= maxLines) break;
        
        // 0.0 ~ 1.0 사이로 라인 인덱스 정규화
        float n = float(i) / float(maxLines - 1);
        
        // 화면 좌우 여백을 약간 남긴 기본 X 좌표
        float baseX = mix(0.05, 0.95, n);
        
        // 1. 왜곡 마스크(Distortion Mask) 계산
        // 이미지처럼 직선 구간과 구부러진 구간을 자연스럽게 분리합니다.
        vec2 maskCoord = vec2(baseX * 2.0, uv.y * 1.5 - TIME * speed * 0.2);
        float maskVal = fbm(maskCoord);
        // mask 값이 높을수록 강하게 얽히고, 낮을수록 직선을 유지합니다.
        float mask = mix(1.0, smoothstep(0.4, 0.7, maskVal), distortionMask);
        
        // 2. 실제 웨이브 변위 계산
        vec2 waveCoord = vec2(baseX * noiseFreqX, uv.y * noiseFreqY + TIME * speed);
        float displacement = (fbm(waveCoord) - 0.5) * 2.0; // -1.0 ~ 1.0 범위로 매핑
        
        // 마스크를 적용한 최종 X 위치
        float finalX = baseX + (displacement * currentNoiseAmount * mask);
        
        // 현재 픽셀에서 선까지의 거리
        float dist = abs(uv.x - finalX);
        minDist = min(minDist, dist);
    }
    
    // 안티앨리어싱을 적용한 부드러운 선 렌더링
    float lineAlpha = smoothstep(lineThickness, lineThickness * 0.2, minDist);
    
    // 배경색과 선 색상 합성
    vec4 finalColor = mix(bgColor, lineColor, lineAlpha);
    
    gl_FragColor = finalColor;
}