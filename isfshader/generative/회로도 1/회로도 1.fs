/*{
    "NAME": "Circuit_Signal_Flow",
    "CREDIT": "Generated for Artist",
    "DESCRIPTION": "Inspired by electronic circuit schematics with flowing data pulses.",
    "CATEGORIES": [
        "Generative",
        "Techno"
    ],
    "INPUTS": [
        {
            "NAME": "speed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0
        },
        {
            "NAME": "gridScale",
            "TYPE": "float",
            "DEFAULT": 15.0,
            "MIN": 5.0,
            "MAX": 50.0
        },
        {
            "NAME": "lineThickness",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.01,
            "MAX": 0.2
        },
        {
            "NAME": "signalIntensity",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.1,
            "MAX": 2.0
        },
        {
            "NAME": "baseColor",
            "TYPE": "color",
            "DEFAULT": [0.1, 0.2, 0.4, 1.0]
        },
        {
            "NAME": "signalColor",
            "TYPE": "color",
            "DEFAULT": [0.0, 1.0, 0.9, 1.0]
        },
        {
            "NAME": "complexity",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        }
    ]
}*/

#ifdef GL_ES
precision highp float;
#endif

float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 st = uv * gridScale;
    vec2 ipos = floor(st);  // Integer position (grid cell)
    vec2 fpos = fract(st);  // Fractional position (inside cell)

    float time = TIME * speed;
    
    // Create circuit lines based on grid
    float line = 0.0;
    float rnd = random(ipos);
    
    // Horizontal and vertical lines decision
    if (rnd > 0.5 - (complexity * 0.4)) {
        line = smoothstep(lineThickness, 0.0, abs(fpos.y - 0.5));
    }
    if (random(ipos + 0.1) > 0.5 - (complexity * 0.4)) {
        line = max(line, smoothstep(lineThickness, 0.0, abs(fpos.x - 0.5)));
    }

    // Moving signal pulses
    float pulse = 0.0;
    float pulseSpeed = (random(ipos) > 0.5 ? 1.0 : -1.0) * (0.5 + random(ipos + 0.2));
    float offset = random(ipos + 0.3) * 6.28;
    
    // Calculate pulse position along the grid lines
    float p = mod(time * pulseSpeed + offset, 1.0);
    float dist = distance(fpos, vec2(p));
    
    if (line > 0.0) {
        pulse = smoothstep(0.2, 0.0, dist) * signalIntensity;
    }

    // Background glow based on the original blueprint blue
    vec3 bgColor = baseColor.rgb * (0.5 + 0.2 * sin(time * 0.2 + uv.y));
    
    // Final color composition
    vec3 finalLines = baseColor.rgb * 2.0 * line;
    vec3 finalPulse = signalColor.rgb * pulse;
    
    vec3 color = bgColor + finalLines + finalPulse;

    // Add a slight flicker for a retro-electronic feel
    float flicker = 0.95 + 0.05 * random(vec2(time));
    
    gl_FragColor = vec4(color * flicker, 1.0);
}