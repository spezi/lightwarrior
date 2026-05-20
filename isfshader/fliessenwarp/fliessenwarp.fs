/*{
  "DESCRIPTION": "Kaleidoscopic animation based on a Shadertoy. Original tutorial: https://youtu.be/f4s1h2YETNY",
  "CREDIT": "Ported from Shadertoy - Original Author from YouTube tutorial",
  "CATEGORIES": [
    "Generative",
    "Animation"
  ],
  "INPUTS": [
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 0.4,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.1,
      "MAX": 5,
      "LABEL": "Zoom"
    },
    {
      "NAME": "iterations",
      "TYPE": "float",
      "DEFAULT": 4,
      "MIN": 1,
      "MAX": 10,
      "LABEL": "Iterations"
    }
  ]
}*/

// Common ISF header stuff (can be omitted if host provides them)
// precision highp float; // Shadertoy often defaults to highp

// ISF provides RENDERSIZE (vec4: width, height, 1/width, 1/height) and TIME (float: seconds)

// https://iquilezles.org/articles/palettes/
const float MAX_ITERATIONS = 10.0;
vec3 palette( float t ) {
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 1.0); // This was 1.0, 1.0, 1.0 in the original
    vec3 d = vec3(0.263,0.416,0.557);

    return a + b*cos( 6.28318*(c*t+d) );
}

void main() {
    // Shadertoy: vec2 uv = (fragCoord * 2.0 - iResolution.xy) / iResolution.y;
    // ISF: gl_FragCoord.xy is pixel coordinates, RENDERSIZE.xy is resolution
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 uv0 = uv; // Store original uv
    
    vec3 finalColor = vec3(0.0);
    
    // iterations is now an input
    for (float i = 0.0; i < MAX_ITERATIONS; i++) {
        if (i >= iterations) {
            break;
        }
        // uv = fract(uv * 1.5) - 0.5; // zoom is now an input
        uv = fract(uv * zoom) - 0.5; 

        float d = length(uv) * exp(-length(uv0));

        // TIME replaces iTime, speed is an input multiplier
        vec3 col = palette(length(uv0) + i*0.4 + TIME*speed); 

        d = sin(d*8.0 + TIME*speed)/8.0; // Also apply speed to the sin wave
        d = abs(d);

        // This can create very large numbers if d is close to 0, potentially problematic.
        // Clamping d to avoid division by zero or extremely small numbers might be good.
        d = max(d, 0.00001); 
        d = pow(0.01 / d, 1.2);

        finalColor += col * d;
    }
        
    gl_FragColor = vec4(finalColor, 1.0);
}