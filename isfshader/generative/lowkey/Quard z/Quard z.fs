/*{
    "CATEGORIES": [
        "Generator", "Glitch", "Rhythm"
    ],
    "CREDIT": "Derived from ebddaf75f7593054478f5b3ab026d950.jpg",
    "DESCRIPTION": "Multi-layered rhythmic glitch patterns compatible with MadMapper.",
    "INPUTS": [
        {
            "NAME": "numLayers",
            "TYPE": "float",
            "DEFAULT": 3.0,
            "MIN": 1.0,
            "MAX": 5.0
        },
        {
            "NAME": "rhythmSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 4.0
        },
        {
            "NAME": "complexity",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.1,
            "MAX": 1.0
        },
        {
            "NAME": "fillColor",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.0, 0.0, 1.0]
        },
        {
            "NAME": "bgColor",
            "TYPE": "color",
            "DEFAULT": [0.95, 0.95, 0.95, 1.0]
        },
        {
            "NAME": "layerOpacity",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.1,
            "MAX": 1.0
        },
        {
            "NAME": "overallScaling",
            "TYPE": "float",
            "DEFAULT": 1.2,
            "MIN": 0.5,
            "MAX": 3.0
        }
    ]
}*/

// 랜덤 함수
float random (vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898,78.233))) * 43758.5453123);
}

// 박스 SDF
float sdBox(vec2 p, vec2 b) {
    vec2 d = abs(p)-b;
    return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
}

void main() {
    // 16:9 캔버스 좌표계 설정
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    uv = uv * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;
    
    float targetAspect = 16.0 / 9.0;
    uv /= (overallScaling * targetAspect);

    vec4 finalColor = bgColor;
    float time = TIME * rhythmSpeed;

    // 매드맵퍼 호환을 위해 루프 횟수를 상수로 고정
    for (int L = 0; L < 5; ++L) {
        if (float(L) >= numLayers) break;
        
        float layerIdx = float(L);
        float layerOffset = sin(time * (1.0 + layerIdx * 0.2) + layerIdx * 1.5) * 0.2;
        float layerVShift = cos(time * 0.7 - layerIdx) * 0.1;
        
        float cols = floor(10.0 + 20.0 * random(vec2(layerIdx, 1.0)));
        float rows = floor(4.0 + 4.0 * random(vec2(layerIdx, 2.0)));
        
        float cellW = (0.5 * complexity) / cols;
        float cellH = 1.2 / rows;

        for (int j = 0; j < 8; ++j) {
            if (float(j) >= rows) break;
            
            float yPos = mix(-0.8, 0.8, float(j)/max(1.0, rows-1.0)) + layerVShift;
            float rowSlide = sin(time * 2.0 + float(j)) * 0.05;

            for (int i = 0; i < 40; ++i) {
                if (float(i) >= cols) break;
                
                vec2 gridPos = vec2(float(i), float(j));
                float xPos = mix(-1.0, 1.0, float(i)/max(1.0, cols-1.0)) + layerOffset + rowSlide;
                
                float seed = random(gridPos + layerIdx);
                if (seed > (1.0 - complexity)) {
                    
                    float bars = floor(mix(2.0, 8.0, random(gridPos * 1.3)));
                    float bW = cellW * 0.4;
                    
                    for (int k = 0; k < 8; ++k) {
                        if (float(k) >= bars) break;
                        
                        float kRel = float(k) / max(1.0, bars - 1.0);
                        float xOff = mix(-cellW, cellW, kRel);
                        
                        float pulse = 0.8 + 0.2 * sin(time * 4.0 + xPos * 2.0);
                        vec2 bSize = vec2(bW, cellH * 0.4 * pulse);
                        
                        float d = sdBox(uv - vec2(xPos + xOff, yPos), bSize);
                        float mask = smoothstep(0.002, 0.0, d);
                        
                        if (mask > 0.0) {
                            vec3 fillRGB = fillColor.rgb;
                            vec3 invColor = 1.0 - fillRGB;
                            vec4 targetColor = (mod(layerIdx, 2.0) == 0.0) ? vec4(fillRGB, 1.0) : vec4(invColor, 1.0);
                            finalColor = mix(finalColor, targetColor, mask * layerOpacity);
                        }
                    }
                }
            }
        }
    }
    
    float vignet = smoothstep(2.0, 0.5, length(uv));
    finalColor.rgb *= (0.9 + 0.1 * random(uv + time));
    finalColor.rgb *= vignet;

    gl_FragColor = finalColor;
}