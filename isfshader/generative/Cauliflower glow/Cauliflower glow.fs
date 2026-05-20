/*{
    "DESCRIPTION": "Cauliflower Fractal by gaz: An organic, rotating recursive fractal with distance-based glow.",
    "CREDIT": "gaz (converted by Gemini) - https://www.shadertoy.com/view/WlfXW4",
    "ISFVSN": "2",
    "CATEGORIES": ["Fractal", "Raymarching", "Organic"],
    "INPUTS": [
        {
            "NAME": "speed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 1.0
        },
        {
            "NAME": "glowIntensity",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.25
        },
        {
            "NAME": "stepSharpness",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 1.0,
            "DEFAULT": 0.8
        }
    ]
}*/

precision highp float;

float g_glow = 0.0;

// Standard 2D Rotation
mat2 rotate(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, s, -s, c);
}

// The core fractal folding function
float deCauliflower(vec3 p, vec3 s, float r, float t) {    
    
    for (int i=0; i<6; i++) {
        p = abs(p) - s;
        // The magic 'twist': rotating XY and YZ simultaneously inside the recursion
        mat2 m = rotate(sin(t * 0.8 + 0.3 * sin(t)) * 2.0 + 0.8);
        p.xy *= m;
        p.yz *= m;
        s *= 0.5; // Scale sub-division
    }
    
    // Octahedral symmetry sorting
    p = abs(p) - s;
    if (p.x < p.z) p.xz = p.zx;
    if (p.y < p.z) p.yz = p.zy;        
    p.z = max(0.0, p.z);
    
    return length(p) - r;
}

float map(vec3 p) {
    float t = TIME;
    // We calculate two versions: 
    // One for the 'glow' (very thin radius)
    float de = deCauliflower(p, vec3(4.5), 0.01, t);
    
    // Balkhan's Glow Logic: Accumulates brightness based on proximity to the surface
    // 
    g_glow += 0.1 / (0.3 + de * de * 10.0); 
    
    // One for the actual collision (thicker radius)
    return deCauliflower(p, vec3(4.5), 0.06, t + 0.3);
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    float t_time = TIME * speed;
    
    // Camera setup with orbital movement
    vec3 ro = vec3(0.0, 0.0, 25.0 + sin(t_time * 0.8 + sin(t_time * 0.5) * 0.8) * 5.0);
    ro.xz *= rotate(t_time * 0.3);
    ro.xy *= rotate(t_time * 0.5);
    
    vec3 w = normalize(-ro);
    vec3 u = normalize(cross(w, vec3(0.0, 1.0, 0.0)));
    vec3 rd = mat3(u, cross(u, w), w) * normalize(vec3(uv, 3.0));
    
    vec3 col = vec3(0.05); // Ambient dark background
    float t_dist = 0.0, d;
    
    for(int i = 0; i < 64; i++) {
        // SHARPNESS FIX: Using a multiplier (stepSharpness) to prevent overstepping
        d = map(ro + rd * t_dist);
        t_dist += d * stepSharpness;
        if(d < 0.001) break;
    }
    
    // Apply the accumulated green 'cauliflower' glow
    col += vec3(0.0, 0.7, 0.3) * g_glow * glowIntensity;
    col = clamp(col, 0.0, 1.0);
    
    gl_FragColor = vec4(col, 1.0);
}