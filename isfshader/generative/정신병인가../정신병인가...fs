/*{
    "DESCRIPTION": "Extreme 3D Independence: XYZ Particle & Wireframe Chaos",
    "CREDIT": "Gemini",
    "ISFVSN": "2.0",
    "INPUTS": [
        { "NAME": "highFreq", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "High: Pixel Jitter/Spark" },
        { "NAME": "midFreq", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Mid: Color Spectrum/Line" },
        { "NAME": "lowFreq", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Low: Z-Depth Warp/Scale" }
    ]
}*/

precision highp float;

// 3D 회전 및 랜덤 함수
float hash(vec3 p) {
    p = fract(p * vec3(123.34, 456.21, 789.18));
    p += dot(p, p.yzx + 45.32);
    return fract((p.x + p.y) * p.z);
}

mat3 rotX(float a) { float s=sin(a), c=cos(a); return mat3(1,0,0,0,c,-s,0,s,c); }
mat3 rotY(float a) { float s=sin(a), c=cos(a); return mat3(c,0,s,0,1,0,-s,0,c); }
mat3 rotZ(float a) { float s=sin(a), c=cos(a); return mat3(c,-s,0,s,c,0,0,0,1); }

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / min(RENDERSIZE.y, RENDERSIZE.x);
    vec3 finalCol = vec3(0.0);
    float t = TIME * 0.5;

    // --- 3D 공간 제어 (X, Y, Z 독립 회전) ---
    // 저주파(lowFreq)에 따라 공간 전체의 깊이와 뒤틀림이 결정됨
    mat3 rotation = rotX(t * 0.2 + lowFreq) * rotY(t * 0.3 - lowFreq) * rotZ(t * 0.15);
    
    // --- 수백 가지 요소의 독립적 움직임 (Loop System) ---
    // i값이 커질수록 Z축 깊숙한 곳의 요소를 생성
    for(int i=0; i<40; i++) {
        float fi = float(i);
        float h = hash(vec3(fi, 12.3, 45.6));
        
        // 1. 각 개체의 독립적 위치 (XYZ 공간 배치)
        vec3 p = vec3(uv * (1.0 + fi * 0.05), -1.0 + fi * 0.05);
        p *= rotation; // 공간 전체 회전 적용
        
        // 2. 개별적 꿈틀거림 (Z축 방향 뒤틀림 포함)
        p.x += sin(t * h * 2.0 + fi) * 0.2 * midFreq;
        p.y += cos(t * (1.0-h) * 1.5 + fi) * 0.2 * midFreq;
        p.z += sin(t + fi) * 0.5 * lowFreq;

        // 3. 요소의 형태 (날카로운 선 + 유기적 점)
        float dist = length(p);
        
        // 고주파(highFreq)에 반응하는 파티클 디테일
        float size = 0.002 + highFreq * 0.008;
        float edge = abs(sin(dist * 15.0 - t * 2.0 + fi));
        float strength = smoothstep(size, 0.0, edge * 0.1);

        // 4. 독립적 색상 부여 (노랑, 파랑, 핑크가 층층이 쌓임)
        vec3 col;
        if(h < 0.33) col = vec3(1.0, 0.9, 0.2);      // Yellow
        else if(h < 0.66) col = vec3(0.2, 0.6, 1.0); // Blue
        else col = vec3(1.0, 0.3, 0.8);              // Pink/Magenta

        // 5. 입체적 광원 및 깊이감 조절
        float depthFade = pow(1.0 - (fi / 40.0), 2.0);
        finalCol += col * strength * depthFade * (0.5 + midFreq);
    }

    // --- High Freq: 배경 자글거림 (Noise Jitter) ---
    finalCol += (hash(vec3(uv, t)) - 0.5) * highFreq * 0.15;

    // 톤 매핑 (색 번짐 방지 및 대비 강화)
    finalCol = smoothstep(0.0, 1.0, finalCol);
    finalCol = pow(finalCol, vec3(0.8)); // 감마 보정

    gl_FragColor = vec4(finalCol, 1.0);
}