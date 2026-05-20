/*{
    "DESCRIPTION": "Blocky Newspaper Glitch / Collage Effect",
    "CREDIT": "Generated for Media Art",
    "ISFVSN": "2",
    "CATEGORIES": [ "Effect", "Glitch", "Stylize" ],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "rows",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 100.0,
            "DEFAULT": 35.0
        },
        {
            "NAME": "cols",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 50.0,
            "DEFAULT": 8.0
        },
        {
            "NAME": "glitch_amount",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.3
        },
        {
            "NAME": "audio_level",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "update_rate",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 30.0,
            "DEFAULT": 8.0
        },
        {
            "NAME": "bw_mode",
            "TYPE": "bool",
            "DEFAULT": true
        },
        {
            "NAME": "invert_chance",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.25
        }
    ]
}*/

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void main() {
    vec2 uv = isf_FragNormCoord;

    // 1. 시간에 따른 갱신 속도 설정 (글리치가 뚝뚝 끊기며 변하도록)
    float timeStep = floor(TIME * update_rate);

    // 2. 화면을 직사각형 블록 형태(그리드)로 분할
    // 원본 이미지처럼 가로로 긴 조각을 만들기 위해 rows와 cols 비율을 다르게 줍니다.
    vec2 grid = vec2(cols, rows);
    vec2 blockId = floor(uv * grid);

    // 3. 오디오 반응형 글리치 강도
    float currentGlitch = glitch_amount + (audio_level * 1.5);

    // 4. 블록별 고유 난수(노이즈) 생성
    float noise1 = rand(blockId + timeStep);
    float noise2 = rand(blockId + timeStep + 100.0);

    // 5. UV 왜곡 (콜라주 찢기)
    vec2 offsetUv = uv;
    
    // 특정 확률에서만 글리치 발생시켜 원본과 깨진 부분이 섞이게 함
    if (noise1 < 0.6) {
        // 가로축(x)으로 강하게 밀어내어 슬릿스캔 느낌 강조
        float rowShift = (rand(vec2(blockId.y, timeStep)) - 0.5) * currentGlitch;
        offsetUv.x += rowShift;
        
        // 세로축(y)으로는 같은 세로줄(Column)끼리 미세하게 어긋나게 함
        float colShift = (rand(vec2(blockId.x, timeStep + 50.0)) - 0.5) * currentGlitch * 0.2;
        offsetUv.y += colShift;
    }

    // 화면 밖으로 밀려난 텍스처가 반대편에서 나타나도록 래핑(Wrapping)
    offsetUv = fract(offsetUv);

    // 왜곡된 UV 좌표로 원본 이미지 픽셀 가져오기
    vec4 color = IMG_NORM_PIXEL(inputImage, offsetUv);

    // 6. 극단적인 흑백(고대비) 모드 적용
    // 신문이나 문서 텍스처 느낌을 극대화
    if (bw_mode) {
        float luma = dot(color.rgb, vec3(0.299, 0.587, 0.114));
        float bw = step(0.4, luma); // 문턱값(Threshold)으로 계조를 날려버림
        color = vec4(vec3(bw), color.a);
    }

    // 7. 음각/양각(색상 반전) 콜라주
    // 올려주신 이미지처럼 부분적으로 흑/백이 반전된 네거티브 조각 삽입
    float invertNoise = rand(blockId + timeStep + 200.0);
    if (invertNoise < invert_chance) {
        color.rgb = 1.0 - color.rgb;
    }

    gl_FragColor = color;
}