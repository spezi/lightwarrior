/*{
    "DESCRIPTION": "Vertical lines with organic distortion for sound interaction",
    "CREDIT": "Gemini",
    "ISFVSN": "2",
    "CATEGORIES": ["Visualizer", "Minimalist"],
    "INPUTS": [
        { "NAME": "lineCount", "LABEL": "Line Count", "TYPE": "float", "MIN": 5, "MAX": 200, "DEFAULT": 60 },
        { "NAME": "lineWidth", "LABEL": "Line Width", "TYPE": "float", "MIN": 0.001, "MAX": 0.05, "DEFAULT": 0.005 },
        { "NAME": "distortionAmount", "LABEL": "Distortion Amount", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3 },
        { "NAME": "speed", "LABEL": "Movement Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5 },
        { "NAME": "chaos", "LABEL": "Chaos / Tangle", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 2.0 },
        { "NAME": "scale", "LABEL": "Vertical Scale", "TYPE": "float", "MIN": 0.1, "MAX": 2.0, "DEFAULT": 0.8 },
        { "NAME": "bgColor", "LABEL": "Background Color", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
        { "NAME": "lineColor", "LABEL": "Line Color", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] }
    ]
}*/

float random(float x) {
    return fract(sin(x) * 43758.5453);
}

void main() {
    // 16:9 비율 유지 및 좌표 정규화
    vec2 uv = isf_FragNormCoord;
    
    // 세로 방향 크기 및 중앙 정렬 조절
    float vMask = smoothstep(0.0, 0.02, uv.y - (0.5 - scale * 0.5)) * smoothstep(0.0, 0.02, (0.5 + scale * 0.5) - uv.y);

    // 선의 간격 계산
    float spacing = 1.0 / lineCount;
    float lineID = floor(uv.x * lineCount);
    float localX = fract(uv.x * lineCount);

    // 왜곡 효과 (Time과 Chaos 파라미터 활용)
    // 이 부분의 변수들을 MadMapper의 Audio Analysis(Low, Mid, High)에 개별적으로 링크하세요.
    float noise = sin(uv.y * chaos + TIME * speed + random(lineID) * 10.0);
    float distortion = noise * distortionAmount * sin(uv.y * 3.14159);
    
    // 수평 오프셋 적용 (이미지처럼 선이 옆으로 휘는 효과)
    float xPos = localX - 0.5 + distortion * (random(lineID) - 0.5) * 5.0;
    
    // 선 그리기 (Antialiasing 적용)
    float edge = lineWidth * lineCount * 0.5;
    float lineAlpha = smoothstep(edge, edge - (0.1), abs(xPos));
    
    // 최종 색상 조합
    vec4 finalColor = mix(bgColor, lineColor, lineAlpha * vMask);
    
    gl_FragColor = finalColor;
}