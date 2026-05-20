/*
{
  "CATEGORIES" : [
    "Distortion",
    "Line"
  ],
  "DESCRIPTION" : "Melting Minimalist Face Lines",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "flow_speed",
      "TYPE" : "float",
      "MIN" : 0.0,
      "MAX" : 2.0,
      "DEFAULT" : 0.4,
      "LABEL" : "Flow Speed"
    },
    {
      "NAME" : "drip_strength",
      "TYPE" : "float",
      "MIN" : 0.0,
      "MAX" : 0.5,
      "DEFAULT" : 0.15,
      "LABEL" : "Drip Strength"
    },
    {
      "NAME" : "melt_center_x",
      "TYPE" : "float",
      "MIN" : 0.0,
      "MAX" : 1.0,
      "DEFAULT" : 0.5,
      "LABEL" : "Melt Center X"
    },
    {
      "NAME" : "melt_width",
      "TYPE" : "float",
      "MIN" : 0.1,
      "MAX" : 1.0,
      "DEFAULT" : 0.6,
      "LABEL" : "Melt Width"
    },
    {
      "NAME" : "line_threshold",
      "TYPE" : "float",
      "MIN" : 0.0,
      "MAX" : 0.8,
      "DEFAULT" : 0.2,
      "LABEL" : "Line Sensitivity"
    },
    {
      "NAME" : "noise_scale",
      "TYPE" : "float",
      "MIN" : 1.0,
      "MAX" : 20.0,
      "DEFAULT" : 8.0,
      "LABEL" : "Distortion Scale"
    }
  ]
}
*/

// --- Noise Function ---
float hash(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

void main() {
    // 1. 기본 좌표 설정
    vec2 uv = isf_FragNormCoord.xy;
    
    // 2. 흘러내림 효과를 위한 노이즈 생성
    // Y축 방향으로 흐르는 움직임을 만듭니다.
    float n = noise(vec2(uv.x * noise_scale, uv.y * noise_scale * 0.5 - TIME * flow_speed));
    
    // 3. 녹아내리는 범위(Melt Area) 설정
    // 중심점(melt_center_x)에서 멀어질수록 효과가 감쇠되도록 설계
    float meltMask = smoothstep(melt_width, 0.0, abs(uv.x - melt_center_x));
    
    // 4. 좌표 왜곡 (Distortion)
    // 원본 이미지의 선을 아래로 끌어내리는 변위값 계산
    vec2 distUv = uv;
    distUv.y += n * drip_strength * meltMask;
    
    // 화면 밖으로 나가는 픽셀 처리
    distUv = clamp(distUv, 0.0, 1.0);

    // 5. 이미지 샘플링
    vec4 srcColor = IMG_NORM_PIXEL(inputImage, uv);          // 원본 이미지
    vec4 meltColor = IMG_NORM_PIXEL(inputImage, distUv);    // 흘러내리는 이미지
    
    // 6. 선(Edge) 강조 및 합성
    // 제공된 이미지처럼 검은 배경에 흰 선인 경우를 가정하여 밝기값 기반으로 추출
    float lineIntensity = length(meltColor.rgb);
    float finalLine = smoothstep(line_threshold, line_threshold + 0.1, lineIntensity);
    
    // 원본 선과 흘러내리는 효과를 조화롭게 섞음
    vec3 result = mix(srcColor.rgb, meltColor.rgb, meltMask * 0.8);
    
    // 최종 출력 (검은 배경 유지, 선의 느낌 강조)
    gl_FragColor = vec4(result, 1.0);
}