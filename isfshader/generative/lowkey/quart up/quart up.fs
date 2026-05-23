/*
{
  "CATEGORIES": [
    "Pattern"
  ],
  "DESCRIPTION": "Dense vertical lines with animated rectangular highlight blocks",
  "INPUTS": [
    {
      "NAME": "line_density",
      "TYPE": "float",
      "MIN": 50.0,
      "MAX": 1000.0,
      "DEFAULT": 400.0,
      "LABEL": "Line Density"
    },
    {
      "NAME": "block_thickness",
      "TYPE": "float",
      "MIN": 1.0,
      "MAX": 20.0,
      "DEFAULT": 4.0,
      "LABEL": "Block Thickness"
    },
    {
      "NAME": "block_height",
      "TYPE": "float",
      "MIN": 1.0,
      "MAX": 50.0,
      "DEFAULT": 10.0,
      "LABEL": "Block Height"
    },
    {
      "NAME": "block_sparsity",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.98,
      "LABEL": "Block Sparsity"
    },
    {
      "NAME": "anim_speed",
      "TYPE": "float",
      "MIN": -10.0,
      "MAX": 10.0,
      "DEFAULT": 1.5,
      "LABEL": "Animation Speed"
    }
  ]
}
*/

// 난수 생성 함수 (패턴의 불규칙성을 위해 사용)
float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898,78.233))) * 43758.5453123);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;

    // 1. 기본 수직선 (Base Vertical Lines) 생성
    // 화면을 line_density 만큼 쪼개어 각각의 고유한 선 ID를 만듭니다.
    float lineId = floor(uv.x * line_density);
    float lineUv = fract(uv.x * line_density);
    
    // 선마다 미세한 밝기 차이를 주어 자연스러운 질감(종이/직물 느낌)을 줍니다.
    float lineNoise = random(vec2(lineId, 0.0));
    
    // 부드러운 안티앨리어싱이 적용된 선 그리기
    float baseLine = smoothstep(0.2, 0.4, lineUv) * smoothstep(0.8, 0.6, lineUv);
    
    // 짙은 회색 계열의 배경 선 렌더링
    vec3 color = mix(vec3(0.0 + lineNoise * 0.0), vec3(0.0), baseLine);

    // 2. 직사각형 하이라이트 블록 (Rectangular Highlight Blocks) 생성
    // 얇은 선들을 몇 개씩 묶어서 하나의 '기둥(Column)'으로 만듭니다.
    float colId = floor(lineId / block_thickness);
    
    // 기둥마다 떨어지는 속도와 시작 위치를 다르게 설정
    float colRand = random(vec2(colId, 12.34));
    
    // 시간에 따라 위아래로 움직이는 Y 좌표 계산
    float blockY = uv.y * block_height - TIME * anim_speed * (0.5 + colRand * 1.5);
    
    // Y축을 칸(Cell)으로 나누어 사각형 덩어리를 만듭니다.
    float cellId = floor(blockY);
    float cellRand = random(vec2(colId, cellId));
    
    // 3. 블록 렌더링 (애니메이션 적용 부분)
    // 난수값이 block_sparsity(희소성)를 넘을 때만 하얀색 사각형 블록을 그립니다.
    if (cellRand > block_sparsity) {
        // 하이라이트 블록 영역 안에서는 선이 더 선명하고 하얗게 보이도록 처리
        float brightLine = smoothstep(0.1, 0.3, lineUv) * smoothstep(0.9, 0.7, lineUv);
        color = mix(vec3(0.8), vec3(1.0), brightLine);
    }

    // 약간의 아날로그 노이즈를 전체적으로 추가하여 밋밋함을 없앱니다.
    float grain = random(uv + TIME) * 0.08;
    color -= grain;

    gl_FragColor = vec4(color, 1.0);
}