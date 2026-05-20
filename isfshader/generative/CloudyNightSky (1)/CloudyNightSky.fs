/*{
  "CREDIT": "GEN8 and DeepSeek",
  "DESCRIPTION": "Procedural night sky with scattered stars, flickering effect, and realistic nebulae.",
  "CATEGORIES": ["Generator", "Space", "Stars"],
  "INPUTS": [
    {
      "NAME": "lightIntensity",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 2.0
    },
    {
      "NAME": "numStars",
      "TYPE": "float",
      "DEFAULT": 500.0,
      "MIN": 100.0,
      "MAX": 1000.0
    },
    {
      "NAME": "nebulaDensity",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.1,
      "MAX": 1.0
    },
    {
      "NAME": "nebulaColor",
      "TYPE": "color",
      "DEFAULT": [0.5, 0.3, 0.8, 1.0]
    },
    {
      "NAME": "starTwinkle",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "starSize",
      "TYPE": "float",
      "DEFAULT": 0.01,
      "MIN": 0.001,
      "MAX": 0.1
    },
    {
      "NAME": "starFlick",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "flickerSpeed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 5.0
    },
    {
      "NAME": "nebulaDetail",
      "TYPE": "float",
      "DEFAULT": 3.0,
      "MIN": 1.0,
      "MAX": 5.0
    }
  ]
}*/

// Random function for star generation
float random(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

// Simplex noise function for nebulae
vec3 mod289(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec4 mod289(vec4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec4 permute(vec4 x) { return mod289(((x * 34.0) + 1.0) * x); }
vec4 taylorInvSqrt(vec4 r) { return 1.79284291400159 - 0.85373472095314 * r; }
float snoise(vec3 v) {
  const vec2 C = vec2(1.0 / 6.0, 1.0 / 3.0);
  vec3 i = floor(v + dot(v, C.yyy));
  vec3 x0 = v - i + dot(i, C.xxx);
  vec3 g = step(x0.yzx, x0.xyz);
  vec3 l = 1.0 - g;
  vec3 i1 = min(g.xyz, l.zxy);
  vec3 i2 = max(g.xyz, l.zxy);
  vec3 x1 = x0 - i1 + C.xxx;
  vec3 x2 = x0 - i2 + C.yyy;
  vec3 x3 = x0 - 0.5;
  i = mod289(i);
  vec4 p = permute(permute(permute(
             i.z + vec4(0.0, i1.z, i2.z, 1.0))
           + i.y + vec4(0.0, i1.y, i2.y, 1.0))
           + i.x + vec4(0.0, i1.x, i2.x, 1.0));
  vec4 j = p - 49.0 * floor(p * (1.0 / 49.0));
  vec4 x_ = floor(j * (1.0 / 7.0));
  vec4 y_ = floor(j - 7.0 * x_);
  vec4 x = (x_ * 2.0 + 0.5) / 7.0 - 1.0;
  vec4 y = (y_ * 2.0 + 0.5) / 7.0 - 1.0;
  vec4 h = 1.0 - abs(x) - abs(y);
  vec4 b0 = vec4(x.xy, y.xy);
  vec4 b1 = vec4(x.zw, y.zw);
  vec4 s0 = floor(b0) * 2.0 + 1.0;
  vec4 s1 = floor(b1) * 2.0 + 1.0;
  vec4 sh = -step(h, vec4(0.0));
  vec4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
  vec4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
  vec3 g0 = vec3(a0.xy, h.x);
  vec3 g1 = vec3(a0.zw, h.y);
  vec3 g2 = vec3(a1.xy, h.z);
  vec3 g3 = vec3(a1.zw, h.w);
  vec4 norm = taylorInvSqrt(vec4(dot(g0, g0), dot(g1, g1), dot(g2, g2), dot(g3, g3)));
  g0 *= norm.x;
  g1 *= norm.y;
  g2 *= norm.z;
  g3 *= norm.w;
  vec4 m = max(0.6 - vec4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
  m = m * m;
  return 42.0 * dot(m * m, vec4(dot(g0, x0), dot(g1, x1), dot(g2, x2), dot(g3, x3)));
}

// Fractal noise function for nebula
float fractalNoise(vec3 coord, float layers, float persistence) {
  float total = 0.0;
  float frequency = 1.0;
  float amplitude = 1.0;
  float maxValue = 0.0;

  // Use a fixed number of iterations and scale based on layers
  const int MAX_LAYERS = 5; // Maximum number of layers
  for (int i = 0; i < MAX_LAYERS; i++) {
    if (float(i) >= layers) break; // Stop early if layers < MAX_LAYERS
    total += snoise(coord * frequency) * amplitude;
    maxValue += amplitude;
    amplitude *= persistence;
    frequency *= 2.0;
  }

  return total / maxValue;
}

// Main function
void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  uv.y = 1.0 - uv.y; // Flip Y for correct orientation

  // Generate stars
  float stars = 0.0;
  const int MAX_STARS = 1000; // Maximum number of stars
  for (int i = 0; i < MAX_STARS; i++) {
    // Randomize star position
    vec2 starPos = vec2(random(vec2(float(i))), random(vec2(float(i + 1))));

    // Randomly skip stars based on numStars to control density
    if (random(vec2(float(i + 2))) > numStars / float(MAX_STARS)) continue;

    // Randomize star brightness and size
    float starBrightness = random(vec2(float(i + 3))) * lightIntensity;
    float size = starSize * random(vec2(float(i + 4)));

    // Twinkle effect
    float twinkle = sin(TIME * speed + float(i)) * starTwinkle;

    // Flickering effect with speed control
    float flicker = 1.0 + starFlick * sin(TIME * flickerSpeed * 10.0 + float(i) * 10.0) * random(vec2(float(i + 5)));

    // Draw star
    stars += starBrightness * smoothstep(size, 0.0, length(uv - starPos)) * (1.0 + twinkle) * flicker;
  }

  // Generate nebulae using fractal noise
  vec3 noiseCoord = vec3(uv * 5.0, TIME * speed);
  float nebula = fractalNoise(noiseCoord, nebulaDetail, 0.5) * nebulaDensity;
  vec3 nebulaColorFinal = nebulaColor.rgb * nebula * lightIntensity;

  // Combine stars and nebulae
  vec3 color = vec3(stars) + nebulaColorFinal;

  // Output
  gl_FragColor = vec4(color, 1.0);
}