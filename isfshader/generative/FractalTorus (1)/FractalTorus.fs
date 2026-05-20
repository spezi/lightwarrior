/*{
    "DESCRIPTION": "Fractal Torus with Neon Colors and Rotation Controls",
    "CREDIT": "Converted from Shadertoy by Igor Molochevski with added controls",
    "ISFVSN": "2.0",
    "CATEGORIES": ["Generator", "Animation"],
    "INPUTS": [
        {
            "NAME": "color1",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.5, 1.0, 1.0]
        },
        {
            "NAME": "color2",
            "TYPE": "color",
            "DEFAULT": [1.0, 0.8, 0.0, 1.0]
        },
        {
            "NAME": "rotateSpeedX",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": -2.0,
            "MAX": 2.0
        },
        {
            "NAME": "rotateSpeedY",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": -2.0,
            "MAX": 2.0
        },
        {
            "NAME": "rotateSpeedZ",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": -2.0,
            "MAX": 2.0
        }
    ]
}*/

#define PI 3.14159265359

mat2 rot(float a) {
    float s = sin(a), c = cos(a);
    return mat2(c, s, -s, c);
}

float sdTorus(vec3 p, vec2 t) {
    vec2 q = vec2(length(p.xz) - t.x, p.y);
    return length(q) - t.y;
}

float de(vec3 p) {
    float t = -sdTorus(p, vec2(2.3, 2.));
    p.y += 1.;
    float d = 100., s = 2.;
    p *= 0.5;
    
    // Modified rotation with XYZ controls
    for(int i = 0; i < 6; i++) {
        p.xz *= rot(TIME * rotateSpeedX);
        p.xy *= rot(TIME * rotateSpeedY);
        p.yz *= rot(TIME * rotateSpeedZ);
        p.xz = abs(p.xz);
        float inv = 1.0 / clamp(dot(p, p), 0.0, 1.0);
        p = p * inv - 1.0;
        s *= inv;
        d = min(d, length(p.xz) + fract(p.y * 0.05 + TIME * 0.2) - 0.1);
    }
    return min(d / s, t);
}

float march(vec3 from, vec3 dir) {
    float td = 0.0, g = 0.0;
    vec3 p;
    for(int i = 0; i < 100; i++) {
        p = from + dir * td;
        float d = de(p);
        if(d < 0.002) break;
        g++;
        td += d;
    }
    float glow = exp(-0.07 * td * td) * sin(p.y * 10.0 + TIME * 10.0);
    float pattern = smoothstep(0.3, 0.0, abs(0.5 - fract(p.y * 15.0)));
    return mix(pattern * glow, g * g * 0.00008, 0.3);
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) - 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float t = TIME * 0.5;
    vec3 from = vec3(cos(t), 0.0, -3.3);
    vec3 dir = normalize(vec3(uv, 0.7));
    
    // Apply rotation controls to camera
    dir.xy *= rot(0.5 * sin(t * rotateSpeedX));
    
    float intensity = march(from, dir);
    
    // Color blending with neon effect
    vec3 finalColor = mix(
        color1.rgb * intensity * 2.0,
        color2.rgb * (1.0 - intensity) * 1.5,
        smoothstep(0.2, 0.8, intensity)
    );
    
    // Add blue glow
    finalColor += vec3(0.4, 0.6, 1.0) * pow(intensity, 3.0);
    
    gl_FragColor = vec4(finalColor, 1.0);
}
