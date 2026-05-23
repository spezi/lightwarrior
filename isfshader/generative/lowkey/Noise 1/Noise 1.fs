/*
{
  "CATEGORIES": ["Noise", "Audio Reactive", "Generative", "Minimal"],
  "DESCRIPTION": "Fluffy Audio Reactive Noise for MadMapper",
  "INPUTS": [
    {
      "NAME": "audio_react",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "noise_scale",
      "TYPE": "float",
      "DEFAULT": 3.0,
      "MIN": 0.5,
      "MAX": 10.0
    },
    {
      "NAME": "noise_detail",
      "TYPE": "float",
      "DEFAULT": 5.0,
      "MIN": 1.0,
      "MAX": 10.0
    },
    {
      "NAME": "flow_speed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "audio_flow_boost",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": 0.0,
      "MAX": 5.0
    },
    {
      "NAME": "brightness",
      "TYPE": "float",
      "DEFAULT": 1.2,
      "MIN": 0.5,
      "MAX": 3.0
    },
    {
      "NAME": "contrast",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.5,
      "MAX": 5.0
    },
    {
      "NAME": "radial_fade",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "invert_color",
      "TYPE": "bool",
      "DEFAULT": false
    }
  ]
}
*/

// --- Simplex Noise Functions ---
vec3 mod289(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec2 mod289(vec2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x*34.0)+1.0)*x); }

float snoise(vec2 v) {
  const vec4 C = vec4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                      0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                     -0.577350269189626,  // -1.0 + 2.0 * C.x
                      0.024390243902439); // 1.0 / 41.0
  vec2 i  = floor(v + dot(v, C.yy) );
  vec2 x0 = v -   i + dot(i, C.xx);
  vec2 i1;
  i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
  vec4 x12 = x0.xyxy + C.xxzz;
  x12.xy -= i1;
  i = mod289(i); 
  vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 ))
		+ i.x + vec3(0.0, i1.x, 1.0 ));
  vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m ;
  m = m*m ;
  vec3 x = 2.0 * fract(p * C.www) - 1.0;
  vec3 h = abs(x) - 0.5;
  vec3 ox = floor(x + 0.5);
  vec3 a0 = x - ox;
  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
  vec3 g;
  g.x  = a0.x  * x0.x  + h.x  * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return 130.0 * dot(m, g);
}

// --- Fractal Brownian Motion (fBM) ---
float fbm(vec2 st) {
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 0.0;
    
    // noise_detail 값에 따라 루프 횟수 조절 (정수로 변환)
    int octaves = int(floor(noise_detail));
    
    for (int i = 0; i < 10; i++) {
        if (i >= octaves) break;
        value += amplitude * snoise(st);
        st *= 2.0;
        amplitude *= 0.5;
    }
    return value;
}

void main() {
    // 1. 화면 설정 (16:9 비율)
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 st = uv * 2.0 - 1.0;
    st.x *= RENDERSIZE.x / RENDERSIZE.y;

    // 2. 오디오 반응형 시간축 (소리가 클수록 흐름이 빨라짐)
    float time_val = TIME * flow_speed + (audio_react * audio_flow_boost * TIME);

    // 3. 도메인 워핑 (Domain Warping): 노이즈가 물결치듯 일렁이게 만드는 기법
    // q: 첫 번째 왜곡 벡터
    vec2 q = vec2(0.0);
    q.x = fbm(st * noise_scale + vec2(time_val * 0.1, time_val * 0.2));
    q.y = fbm(st * noise_scale + vec2(time_val * 0.3, time_val * 0.4));

    // r: 두 번째 왜곡 벡터 (q를 입력으로 사용)
    vec2 r = vec2(0.0);
    // 오디오 반응에 따라 워핑(일그러짐)이 심해짐
    float distort_amount = 1.0 + (audio_react * 2.0);
    r.x = fbm(st * noise_scale + distort_amount * q + vec2(1.7, 9.2) + time_val * 0.15);
    r.y = fbm(st * noise_scale + distort_amount * q + vec2(8.3, 2.8) + time_val * 0.12);

    // 4. 최종 노이즈 값 계산 (r을 입력으로 사용)
    float f = fbm(st * noise_scale + r);

    // 5. 색상 및 질감 보정 (대비, 밝기)
    // f 값이 -1.0 ~ 1.0 근처이므로 0.0 ~ 1.0으로 조정
    f = (f + 1.0) * 0.5; 
    
    // 대비(Contrast) 조절
    f = pow(f, contrast);
    
    // 오디오 반응에 따른 밝기 추가
    f *= brightness + (audio_react * 0.5);

    // 6. 비네팅 / 방사형 페이드 (선택 사항)
    // 외곽으로 갈수록 어두워지게 하여 솜털 같은 느낌 강조
    float dist = length(st);
    float fade = smoothstep(1.5, 0.0, dist * radial_fade);
    f *= fade;

    // 7. 색상 반전 처리
    if (invert_color) {
        f = 1.0 - f;
    }

    // 최종 출력 (흑백)
    gl_FragColor = vec4(vec3(f), 1.0);
}