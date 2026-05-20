/*{
  "DESCRIPTION": "Monochrome abstract visual with noise and motion",
  "CATEGORIES": [ "Abstract", "Monochrome", "Generator" ],
  "INPUTS": [
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 4.0,
      "MIN": 1.0,
      "MAX": 10.0
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": false
    }
  ]
}*/

float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898,78.233))) * 43758.5453123);
}

float noise(vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);

    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));

    vec2 u = f*f*(3.0 - 2.0*f);

    return mix(a, b, u.x) +
           (c - a)* u.y * (1.0 - u.x) +
           (d - b) * u.x * u.y;
}

void main() {
    vec2 st = gl_FragCoord.xy / RENDERSIZE.xy;
    st.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Tambahkan gerakan dengan waktu dan speed
    float t = TIME * speed;
    st *= scale;

    float n = noise(st + vec2(t, t * 0.5));

    // Tambahkan pola sinusoidal untuk efek tambahan
    float pattern = sin((st.x + n) * 10.0) * cos((st.y + n) * 10.0);

    float color = smoothstep(0.0, 0.1, abs(pattern));

    if (invert) {
        color = 1.0 - color;
    }

    gl_FragColor = vec4(vec3(color), 1.0);
}
