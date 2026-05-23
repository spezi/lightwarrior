/*
{
  "CATEGORIES": [
    "Abstract", "Organic", "Fluid"
  ],
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "scale", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "smoothness", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "cellular_blend", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "invert", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "col_low", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "col_high", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "color_shift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "depth_blur", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 }
  ]
}
*/

#ifdef GL_ES
precision mediump float;
#endif

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash(i + vec2(0.0, 0.0)), hash(i + vec2(1.0, 0.0)), f.x),
               mix(hash(i + vec2(0.0, 1.0)), hash(i + vec2(1.0, 1.0)), f.x), f.y);
}

// 수정된 부분: octaves를 상수로 고정 (상용 GLSL 호환성)
float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    mat2 m = mat2(0.8, 0.6, -0.6, 0.8);
    for (int i = 0; i < 5; ++i) { // 반복 횟수를 '5'와 같은 상수로 명시
        v += a * noise(p);
        p = m * p * 2.0;
        a *= 0.5;
    }
    return v;
}

float cellular(vec2 p) {
    vec2 n = floor(p);
    vec2 f = fract(p);
    float minDist = 1.0;
    for (int y = -1; y <= 1; ++y) {
        for (int x = -1; x <= 1; ++x) {
            vec2 g = vec2(float(x), float(y));
            vec2 o = hash(n + g) * vec2(0.3, 0.3);
            float dist = length(g + o - f);
            minDist = min(minDist, dist);
        }
    }
    return minDist;
}

vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * (q.x - min(q.w, q.y)) + 1e-10)), d / (q.x + 1e-10), q.x);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    uv = (uv * 2.0 - 1.0) * vec2(RENDERSIZE.x/RENDERSIZE.y, 1.0);
    uv *= scale;

    float t = TIME * speed;
    vec2 offset = vec2(sin(t * 0.1), cos(t * 0.15)) * 2.0;
    offset += vec2(fbm(uv * 0.3 + t * 0.05), fbm(uv * 0.3 - t * 0.07)) * 0.2;

    float cell_raw = cellular(uv + offset);
    float cell = smoothstep(0.0, max(0.001, smoothness), cell_raw);
    float cloud = fbm(uv + offset * 0.5);

    float q = fbm(uv + offset + vec2(0.0, 1.0));
    float r = fbm(uv + offset + vec2(5.2, 1.3) + q);
    float structure = fbm(uv + offset + vec2(8.1, 9.2) + r);
    
    structure = 1.0 - abs(structure * 2.0 - 1.0);
    structure = mix(structure, cell, cellular_blend);
    structure = pow(structure, 1.5 + smoothness);

    float blur_fbm = fbm(uv + offset * 1.5);
    float blur = smoothstep(0.1, 0.8, 1.0 - abs(blur_fbm * 2.0 - 1.0)) * depth_blur;
    float final_val = mix(structure, structure * 0.5 + blur * 0.5, depth_blur);

    vec3 base_col = mix(col_low.rgb, col_high.rgb, final_val);
    
    if (color_shift > 0.0) {
        vec3 hsv = rgb2hsv(base_col);
        hsv.x = fract(hsv.x + color_shift);
        base_col = hsv2rgb(hsv);
    }

    if (invert) base_col = 1.0 - base_col;

    gl_FragColor = vec4(base_col, 1.0);
}