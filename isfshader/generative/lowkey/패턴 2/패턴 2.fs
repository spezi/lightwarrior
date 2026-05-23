/*{
  "DESCRIPTION": "Abstract analog data barcode and gel electrophoresis generator.",
  "CREDIT": "Gemini",
  "CATEGORIES": [
    "Generator", "Abstract", "Data", "Minimal", "Glitch"
  ],
  "INPUTS": [
    {
      "NAME": "bgColor",
      "TYPE": "color",
      "DEFAULT": [0.95, 0.95, 0.95, 1.0],
      "LABEL": "Background Color"
    },
    {
      "NAME": "inkColor",
      "TYPE": "color",
      "DEFAULT": [0.05, 0.05, 0.07, 1.0],
      "LABEL": "Ink/Data Color"
    },
    {
      "NAME": "densityX",
      "TYPE": "float",
      "MIN": 5.0,
      "MAX": 150.0,
      "DEFAULT": 45.0,
      "LABEL": "Vertical Lane Density"
    },
    {
      "NAME": "densityY",
      "TYPE": "float",
      "MIN": 10.0,
      "MAX": 300.0,
      "DEFAULT": 120.0,
      "LABEL": "Horizontal Band Density"
    },
    {
      "NAME": "bandThickness",
      "TYPE": "float",
      "MIN": 0.01,
      "MAX": 0.5,
      "DEFAULT": 0.06,
      "LABEL": "Band Thickness"
    },
    {
      "NAME": "scrollSpeed",
      "TYPE": "float",
      "MIN": -2.0,
      "MAX": 2.0,
      "DEFAULT": 0.05,
      "LABEL": "Scroll Speed"
    },
    {
      "NAME": "inkBleed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 0.8,
      "LABEL": "Ink Bleed / Smudge"
    },
    {
      "NAME": "verticalCutoff",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 0.4,
      "DEFAULT": 0.15,
      "LABEL": "Edge Cutoff Amount"
    },
    {
      "NAME": "contrast",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 3.0,
      "DEFAULT": 1.2,
      "LABEL": "Global Contrast"
    },
    {
      "NAME": "seed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 100.0,
      "DEFAULT": 12.3,
      "LABEL": "Random Seed"
    }
  ]
}*/

#ifdef GL_ES
precision highp float;
#endif

// ------------------------------------------------------------
// 해시 및 노이즈 함수
// ------------------------------------------------------------
float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

float hash21(vec2 p) {
    vec3 p3  = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float noise1D(float x) {
    float i = floor(x);
    float f = fract(x);
    float u = f * f * (3.0 - 2.0 * f);
    return mix(hash11(i), hash11(i + 1.0), u);
}

// ------------------------------------------------------------
// 단일 레이어 렌더링 함수
// ------------------------------------------------------------
float drawLayer(vec2 p, float lanes, float bands, float layerSeed) {
    // 수직 레인의 너비를 불규칙하게 만들기 위해 X좌표를 워핑(Warping)
    float warpedX = p.x + noise1D(p.x * 4.0 + layerSeed) * 0.15;
    float nx = warpedX * lanes;
    float laneId = floor(nx);
    
    // 각 레인별 고유 랜덤 값
    float laneHash = hash11(laneId + layerSeed * 13.37 + seed);
    
    // 일정 확률로 레인을 비워서 빈 공간(투명한 배경) 생성
    if (laneHash < 0.4) return 0.0;
    
    // 레인의 기본 배경 명암 (이미지의 희미한 세로줄 텍스처)
    float colBaseShade = (laneHash - 0.4) * 0.4; 
    
    // 수평 밴드 계산 (Y축 스크롤 적용)
    float yScroll = p.y + TIME * scrollSpeed;
    float ny = yScroll * bands;
    float bandId = floor(ny);
    
    // 수평선 생성 확률: 레이어 전체를 가로지르는 선과 레인 내에만 있는 선 혼합
    float isGlobalBand = step(0.85, hash11(bandId + layerSeed * 9.9 + seed));
    float bandHash = mix(hash21(vec2(bandId, laneId + layerSeed + seed)), hash11(bandId + layerSeed + seed), isGlobalBand);
    
    // 수평선이 그려질 확률 마스크
    float bandMask = step(0.82, bandHash); 
    
    // 밴드의 두께 및 잉크 번짐(Bleed) 효과 계산 (가우시안 곡선 형태 활용)
    float localY = fract(ny);
    float thickness = max(bandThickness * (laneHash + 0.5), 0.001); // 레인마다 굵기가 약간 다름
    float lineDraw = exp(-pow(abs(localY - 0.5) / thickness, 2.0 / max(inkBleed, 0.1)));
    
    // 노이즈 점(Artifacts / Smudges) 추가
    float smudge = step(0.98, hash21(vec2(ny, nx + seed))) * 0.4;
    
    // 해당 위치의 최종 잉크 농도 합산
    float intensity = colBaseShade + (bandMask * lineDraw * laneHash) + smudge;
    
    // 수직 그라데이션 적용 (위아래 잉크 농도가 달라지도록)
    float vGradient = mix(p.y, 1.0 - p.y, step(0.5, hash11(laneId * 2.1)));
    intensity *= (0.4 + vGradient * 0.8);
    
    // 화면 위아래 끝부분이 무작위로 잘려나가는(Cutoff) 느낌 구현
    float yMin = 0.02 + hash11(laneId * 7.1 + seed) * verticalCutoff;
    float yMax = 0.98 - hash11(laneId * 3.2 + seed) * verticalCutoff;
    float edgeFader = smoothstep(yMin - 0.05, yMin, p.y) * smoothstep(yMax + 0.05, yMax, p.y);
    
    return clamp(intensity * edgeFader, 0.0, 1.0);
}

// ------------------------------------------------------------
// Main
// ------------------------------------------------------------
void main() {
    vec2 uv = isf_FragNormCoord.xy;
    
    // 3개의 서로 다른 크기와 디테일을 가진 레이어를 겹쳐서 깊이감 형성
    float totalInk = 0.0;
    
    // Base Layer (가장 굵고 흐릿한 배경 레이어)
    totalInk += drawLayer(uv, densityX * 0.5, densityY * 0.5, 1.0) * 0.5;
    
    // Main Layer (명확한 수평선과 기둥)
    totalInk += drawLayer(uv, densityX, densityY, 2.0);
    
    // Detail Layer (가늘고 조밀한 데이터 텍스처)
    totalInk += drawLayer(uv, densityX * 1.8, densityY * 1.5, 3.0) * 0.6;
    
    // 전체 대비 조절
    totalInk = pow(clamp(totalInk, 0.0, 1.0), 1.0 / contrast);
    
    // 배경색(흰색/회색) 위에 잉크색(검은색)을 덮어씌움 (Multiply blend 느낌)
    vec3 finalColor = mix(bgColor.rgb, inkColor.rgb, totalInk);
    
    gl_FragColor = vec4(finalColor, 1.0);
}