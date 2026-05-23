/*{
    "DESCRIPTION": "Procedural Audio-Reactive Morse Code Pattern (Extended)",
    "CREDIT": "Generated for Media Art",
    "ISFVSN": "2",
    "CATEGORIES": [ "Generator" ],
    "INPUTS": [
        {
            "NAME": "scale",
            "TYPE": "float",
            "MIN": 5.0,
            "MAX": 100.0,
            "DEFAULT": 30.0
        },
        {
            "NAME": "speed",
            "TYPE": "float",
            "MIN": -5.0,
            "MAX": 5.0,
            "DEFAULT": 0.5
        },
        {
            "NAME": "row_randomness",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.8
        },
        {
            "NAME": "density",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.6
        },
        {
            "NAME": "symbol_width",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 1.0,
            "DEFAULT": 0.75
        },
        {
            "NAME": "dash_stretch",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 3.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "base_thickness",
            "TYPE": "float",
            "MIN": 0.01,
            "MAX": 0.5,
            "DEFAULT": 0.15
        },
        {
            "NAME": "audio_level",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "glitch_intensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.3
        },
        {
            "NAME": "color_on",
            "TYPE": "color",
            "DEFAULT": [1.0, 1.0, 1.0, 1.0]
        },
        {
            "NAME": "color_bg",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.0, 0.0, 1.0]
        }
    ]
}*/

// 간단한 난수 생성 함수
float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main() {
    vec2 uv = isf_FragNormCoord;

    // 1. 화면을 가로줄(Row)로 분할
    float rows = scale;
    float rowId = floor(uv.y * rows);
    float rowY = fract(uv.y * rows);

    // 2. 오디오 레벨 적용 
    float localAudio = audio_level * (0.5 + 0.5 * rand(vec2(rowId, 0.0)));

    // 3. 줄마다 다른 속도로 흘러가도록 설정 (row_randomness 컨트롤 추가)
    // 0이면 모든 줄이 같은 속도로 이동, 1이면 줄마다 속도 차이가 극대화됨
    float randomSpeedFactor = mix(1.0, (rand(vec2(rowId, 1.23)) - 0.2) * 1.5, row_randomness);
    float rowSpeed = speed * randomSpeedFactor;
    
    // 4. 오디오에 의한 가로축 글리치 효과 (glitch_intensity 컨트롤 추가)
    // 소리가 클 때 좌우로 노이즈처럼 흔들리는 효과
    float glitchOffset = localAudio * glitch_intensity * (rand(vec2(TIME * 10.0, rowId)) - 0.5);
    float xPos = uv.x + (TIME * rowSpeed) + glitchOffset;

    // 5. 모스 부호 패턴 생성 (점과 선)
    float colScale = scale * 1.5;
    float colX = xPos * colScale;
    
    // dash_stretch를 통해 기호들이 묶여서 긴 선(Dash)이 되는 비율 조절
    float wideCellX = floor(colX / dash_stretch); 
    float noise1 = rand(vec2(wideCellX, rowId + 5.0));

    float pattern = 0.0;

    // 밀도에 따라 그릴지 말지 결정
    if (noise1 < density) {
        // symbol_width를 통해 각 기호의 너비(기호 사이의 틈) 조절
        if (fract(colX) < symbol_width) {
            pattern = 1.0;
        }
    }

    // 6. 세로 라인 두께 설정 (base_thickness 컨트롤 추가)
    // 기본 두께에 오디오 진폭을 더해 굵기 변화
    float lineThickness = base_thickness + (localAudio * 0.5); 
    if (abs(rowY - 0.5) > lineThickness) {
        pattern = 0.0;
    }

    // 7. 색상 혼합 및 최종 출력
    gl_FragColor = mix(color_bg, color_on, pattern);
}