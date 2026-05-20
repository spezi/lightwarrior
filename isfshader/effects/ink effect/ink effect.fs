/*{
	"DESCRIPTION": "中国传统水墨风格滤镜，模拟墨色晕染、宣纸纹理和笔触质感",
	"CREDIT": "Custom Ink Wash Filter",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Stylize"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image",
			"LABEL": "输入图像"
		},
		{
			"NAME": "inkAmount",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0,
			"LABEL": "墨色浓度"
		},
		{
			"NAME": "spread",
			"TYPE": "float",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 1.0,
			"LABEL": "晕染程度"
		},
		{
			"NAME": "paperTexture",
			"TYPE": "float",
			"DEFAULT": 0.3,
			"MIN": 0.0,
			"MAX": 1.0,
			"LABEL": "宣纸纹理强度"
		},
		{
			"NAME": "brushTexture",
			"TYPE": "float",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 1.0,
			"LABEL": "笔触纹理强度"
		},
		{
			"NAME": "edgeDarken",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 0.5,
			"LABEL": "边缘墨韵"
		},
		{
			"NAME": "warmth",
			"TYPE": "float",
			"DEFAULT": 0.15,
			"MIN": 0.0,
			"MAX": 0.3,
			"LABEL": "宣纸暖色"
		},
		{
			"NAME": "inkStyle",
			"TYPE": "long",
			"VALUES": [0, 1, 2],
			"LABELS": ["浓墨", "淡墨", "焦墨"],
			"DEFAULT": 0,
			"LABEL": "墨色风格"
		},
		{
			"NAME": "paperColorR",
			"TYPE": "float",
			"DEFAULT": 0.96,
			"MIN": 0.7,
			"MAX": 1.0,
			"LABEL": "宣纸底色R"
		},
		{
			"NAME": "paperColorG",
			"TYPE": "float",
			"DEFAULT": 0.92,
			"MIN": 0.7,
			"MAX": 1.0,
			"LABEL": "宣纸底色G"
		},
		{
			"NAME": "paperColorB",
			"TYPE": "float",
			"DEFAULT": 0.86,
			"MIN": 0.7,
			"MAX": 1.0,
			"LABEL": "宣纸底色B"
		},
		{
			"NAME": "enableDryBrush",
			"TYPE": "bool",
			"DEFAULT": 1.0,
			"LABEL": "启用飞白效果"
		}
	]
}*/

// 伪随机函数
float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898,78.233))) * 43758.5453123);
}

// 噪声函数
float noise(vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);
    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

// 分形噪声
float fbm(vec2 st, int octaves) {
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 4.0;
    for(int i = 0; i < 5; i++) {
        if(i >= octaves) break;
        value += amplitude * noise(st * frequency);
        amplitude *= 0.5;
        frequency *= 2.0;
    }
    return value;
}

// 模拟飞白效果的干笔触
float dryBrush(vec2 uv, float inkValue) {
    float brushPattern = noise(uv * 35.0);
    float streak = smoothstep(0.65, 0.85, brushPattern);
    return mix(inkValue, 0.3, streak * (1.0 - inkValue) * 0.8);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    
    // 晕染效果：采样周围像素模拟墨色扩散
    float spreadAmount = spread * 0.06;
    vec2 offsetX = vec2(spreadAmount, 0.0);
    vec2 offsetY = vec2(0.0, spreadAmount);
    
    vec4 colCenter = IMG_THIS_PIXEL(inputImage);
    vec4 colLeft = IMG_NORM_PIXEL(inputImage, uv - offsetX);
    vec4 colRight = IMG_NORM_PIXEL(inputImage, uv + offsetX);
    vec4 colUp = IMG_NORM_PIXEL(inputImage, uv - offsetY);
    vec4 colDown = IMG_NORM_PIXEL(inputImage, uv + offsetY);
    vec4 colUL = IMG_NORM_PIXEL(inputImage, uv - offsetX - offsetY);
    vec4 colUR = IMG_NORM_PIXEL(inputImage, uv + offsetX - offsetY);
    vec4 colDL = IMG_NORM_PIXEL(inputImage, uv - offsetX + offsetY);
    vec4 colDR = IMG_NORM_PIXEL(inputImage, uv + offsetX + offsetY);
    
    // 高斯风格加权平均
    vec4 blurred = (colCenter * 0.2 + 
                    (colLeft + colRight + colUp + colDown) * 0.15 +
                    (colUL + colUR + colDL + colDR) * 0.05);
    
    // 计算亮度
    float luminance = dot(blurred.rgb, vec3(0.299, 0.587, 0.114));
    
    // 墨色浓度曲线 - 根据风格调整
    float ink = luminance;
    if (inkStyle == 0) { // 浓墨 - 强烈对比
        ink = pow(luminance, 1.0 - inkAmount * 0.8);
        ink = 1.0 - (1.0 - ink) * 1.2;
    } else if (inkStyle == 1) { // 淡墨 - 柔和层次
        ink = pow(luminance, 1.2 - inkAmount * 0.5);
    } else { // 焦墨 - 枯笔感
        ink = pow(luminance, 0.6 - inkAmount * 0.3);
        ink = ink * 0.9 + 0.1;
    }
    ink = clamp(ink, 0.0, 1.0);
    
    // 边缘墨韵：基于亮度梯度强化边缘
    float lumCenter = luminance;
    float lumLeft = dot(colLeft.rgb, vec3(0.299, 0.587, 0.114));
    float lumUp = dot(colUp.rgb, vec3(0.299, 0.587, 0.114));
    float gradX = abs(lumCenter - lumLeft);
    float gradY = abs(lumCenter - lumUp);
    float edge = (gradX + gradY) * edgeDarken * 6.0;
    ink = ink - edge;
    ink = clamp(ink, 0.0, 1.0);
    
    // 宣纸纹理
    vec2 paperUV = uv * 14.0;
    float paperNoise = fbm(paperUV, 3);
    paperNoise = (paperNoise - 0.5) * paperTexture * 0.25;
    
    // 笔触纹理 - 使用不同频率叠加
    vec2 brushUV1 = uv * 20.0;
    vec2 brushUV2 = uv * 45.0;
    float brushNoise1 = noise(brushUV1);
    float brushNoise2 = noise(brushUV2);
    float brushNoise = (brushNoise1 * 0.6 + brushNoise2 * 0.4) * brushTexture;
    brushNoise = (brushNoise - 0.5) * 0.3;
    
    // 应用纹理
    ink = ink + paperNoise + brushNoise;
    ink = clamp(ink, 0.0, 1.0);
    
    // 飞白效果（干笔触）
    if (enableDryBrush) {
        ink = dryBrush(uv, ink);
        ink = clamp(ink, 0.0, 1.0);
    }
    
    // 宣纸底色
    vec3 paperColor = vec3(paperColorR, paperColorG, paperColorB);
    paperColor = paperColor + vec3(paperNoise * 0.06);
    paperColor = paperColor * (1.0 + warmth * 0.6);
    
    // 墨色 - 根据风格选择不同色相
    vec3 inkColor;
    if (inkStyle == 0) {
        inkColor = vec3(0.04, 0.04, 0.05); // 浓墨偏冷
    } else if (inkStyle == 1) {
        inkColor = vec3(0.08, 0.06, 0.05); // 淡墨偏暖
    } else {
        inkColor = vec3(0.02, 0.02, 0.03); // 焦墨更黑
    }
    inkColor = mix(inkColor, vec3(0.10, 0.07, 0.05), ink * 0.25);
    
    // 最终混合
    vec3 finalColor = mix(paperColor, inkColor, ink);
    
    // 整体色调微调，增强国画韵味
    finalColor = finalColor * (0.94 + luminance * 0.1);
    
    gl_FragColor = vec4(finalColor, 1.0);
}
