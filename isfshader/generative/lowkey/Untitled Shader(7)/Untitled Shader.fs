/*{
    "DESCRIPTION": "Botanical Mandala — Infinite Zoom Loop & Custom RGB Control",
    "CREDIT": "Antigravity AI / Edited by Gemini",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        {
            "NAME": "scale",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.1,
            "MAX": 5.0,
            "LABEL": "Base Scale"
        },
        {
            "NAME": "rotation",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 6.28318,
            "LABEL": "Rotation"
        },
        {
            "NAME": "density",
            "TYPE": "float",
            "DEFAULT": 4.0,
            "MIN": 1.0,
            "MAX": 20.0,
            "LABEL": "Density"
        },
        {
            "NAME": "zoom_speed",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Zoom Speed (Loop)"
        },
        {
            "NAME": "custom_red",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Red Intensity"
        },
        {
            "NAME": "custom_green",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Green Intensity"
        },
        {
            "NAME": "custom_blue",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Blue Intensity"
        }
    ]
}*/

precision mediump float;

#define PI  3.14159265358979
#define TAU 6.28318530717959

mat2 rot2(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

float h21(vec2 p) {
    p = fract(p * vec2(127.1, 311.7));
    p += dot(p, p.yx + 45.32);
    return fract(p.x * p.y);
}

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(h21(i), h21(i + vec2(1,0)), u.x), mix(h21(i + vec2(0,1)), h21(i + vec2(1,1)), u.x), u.y);
}

vec2 dfold(vec2 p, float n) {
    float a = atan(p.y, p.x);
    float r = length(p);
    float s = PI / n;
    a = mod(a, 2.0 * s);
    a = abs(a - s);
    return vec2(cos(a), sin(a)) * r;
}

float softDisc(float d, float r) { return smoothstep(r + 0.01, r - 0.008, d); }
float discEdge(float d, float r) { return smoothstep(r + 0.02, r + 0.004, d) * smoothstep(r - 0.002, r + 0.006, d); }

float fiberTex(vec2 p, float seed, float time) {
    float a = atan(p.y, p.x), r = length(p);
    float spokes = pow(sin(a * 18.0 + seed) * 0.5 + 0.5, 2.5);
    float rings = sin(r * 40.0 + seed * 0.7 - time * 0.1) * 0.5 + 0.5;
    return mix(spokes * rings, vnoise(p * 18.0 + seed), 0.22);
}

vec3 evalRing(float ri, vec2 p, float discR, float ringStep) {
    float ringR = (ri + 0.5) * ringStep;
    float spin = TIME * 0.018 * (mod(ri, 2.0) < 0.5 ? 1.0 : -1.0);
    vec2 lp = dfold(rot2(spin) * p, 8.0);
    float nDiscs = max(8.0, floor(TAU * ringR / (discR * 1.78)));
    float dAngle = TAU / nDiscs;
    float snapped = floor(atan(lp.y, lp.x) / dAngle + 0.5) * dAngle;
    vec2 local = lp - vec2(cos(snapped), sin(snapped)) * ringR;
    float dist = length(local);
    float cov = softDisc(dist, discR);
    return vec3(cov, cov * fiberTex(local / discR, ri * 4.13, TIME), discEdge(dist, discR));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // --- INFINITE ZOOM LOOP ---
    // fract() creates a 0 to 1 loop, pow() makes it look more linear in perspective
    float progress = fract(TIME * zoom_speed * 0.2);
    float zoom = pow(2.0, progress * 4.0); 
    
    uv = rot2(rotation + TIME * 0.035) * uv;
    uv /= (scale * zoom);

    vec2 p = dfold(uv, 8.0);
    float pR = length(p);
    float discR = 0.32 / density;
    float ringStep = discR * 1.80;
    float baseRing = floor(pR / ringStep);

    float accumLight = 0.0, accumFiber = 0.0, accumEdge = 0.0;
    for (int di = -1; di <= 1; di++) {
        float ri = baseRing + float(di);
        if (ri < 0.0) continue;
        vec3 r = evalRing(ri, p, discR, ringStep);
        accumLight += r.x; accumFiber += r.y; accumEdge += r.z;
    }

    // --- CUSTOM COLOR LOGIC ---
    vec3 baseRGB = vec3(custom_red, custom_green, custom_blue);
    vec3 cBg = baseRGB * 0.4;
    vec3 cDisc = baseRGB;
    vec3 cFiber = baseRGB * 0.2;
    vec3 cEdge = vec3(0.05);

    vec3 col = mix(cBg, cDisc, clamp(accumLight, 0.0, 1.0) * 0.82);
    col = mix(col, cFiber, clamp(accumFiber, 0.0, 1.0) * 0.9);
    col = mix(col, cEdge, clamp(accumEdge, 0.0, 1.0) * 0.92);

    // Vignette fade for loop smoothness at edges
    float vign = smoothstep(2.0, 0.0, length(uv * 0.5));
    gl_FragColor = vec4(col * vign, 1.0);
}